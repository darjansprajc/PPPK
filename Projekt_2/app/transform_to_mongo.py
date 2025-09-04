from minio import Minio
from minio.error import S3Error
import os
import pandas as pd
from pymongo import MongoClient
from config import MINIO_ENDPOINT, MINIO_ACCESS_KEY, MINIO_SECRET_KEY, MINIO_BUCKET, MONGO_URL, DB_NAME, GENES, MINIO_FOLDER


client = Minio(
    MINIO_ENDPOINT,
    access_key=MINIO_ACCESS_KEY,
    secret_key=MINIO_SECRET_KEY,
    secure=False
)

mongo_client = MongoClient(MONGO_URL)
db = mongo_client[DB_NAME]


def download_from_minio(object_name, local_path):
    if os.path.exists(local_path):
        print(f"Već postoji lokalno: {local_path}, preskačem download")
        return True

    os.makedirs(os.path.dirname(local_path), exist_ok=True)
    try:
        client.fget_object(MINIO_BUCKET, object_name, local_path)
        print(f"Preuzeto iz MinIO: {object_name}")
        return True
    except S3Error as e:
        print(f"Greška pri MinIO downloadu: {object_name}, {e}")
        return False

def main():

    survival_data_local_path = "data/TCGA_clinical_survival_data.tsv"
    download_from_minio("clinical/TCGA_clinical_survival_data.tsv", survival_data_local_path)
    survival_df = pd.read_csv(survival_data_local_path, sep="\t")
    survival_df = survival_df[['bcr_patient_barcode', 'DSS', 'OS', 'clinical_stage']]

    objects = client.list_objects(MINIO_BUCKET, prefix=MINIO_FOLDER, recursive=True)

    for obj in objects:
        tsv_name = obj.object_name.split("/")[-1]
        local_path = os.path.join("data", tsv_name)

        if not download_from_minio(obj.object_name, local_path):
            continue

        print(f"\nObrada datoteke: {tsv_name}")

        genes_df = pd.read_csv(local_path, sep="\t")

        genes_df_t = genes_df.set_index("sample").T.reset_index()
        genes_df_t = genes_df_t.rename(columns={"index": "patient_id"})

        cohort_name = tsv_name.split(".")[1]
        genes_df_t["cancer_cohort"] = cohort_name

        columns = ["patient_id", "cancer_cohort"] + [g for g in GENES if g in genes_df_t.columns]
        genes_df_t = genes_df_t[columns]

        genes_df_t["patient_id_short"] = genes_df_t["patient_id"].str[:12]
        genes_df_merged = genes_df_t.merge(
            survival_df,
            left_on="patient_id_short",
            right_on="bcr_patient_barcode",
            how="left"
        )

        genes_df_merged = genes_df_merged.drop(columns=["bcr_patient_barcode", "patient_id_short"], errors="ignore")


        cohort_collection_name = f"{cohort_name}_expression"
        collection = db[cohort_collection_name]

        records = genes_df_merged.to_dict(orient="records")
        if records:
            collection.insert_many(records)
            print(f"Insertano {len(records)} zapisa iz {tsv_name}")
        else:
            print(f"Nema zapisa za insert u {tsv_name}")

    print("\nPrebacivanje u mongoDB završeno!")


if __name__ == "__main__":
    main()
