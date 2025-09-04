import streamlit as st
from pymongo import MongoClient
import pandas as pd
import matplotlib.pyplot as plt
from config import MONGO_URL, DB_NAME, GENES

def main():
    mongo_client = MongoClient(MONGO_URL)
    db = mongo_client[DB_NAME]

    st.title("Gene Expression Visualization (cGAS-STING pathway)")

    cohorts = db.list_collection_names()
    selected_cohort = st.selectbox("Odaberite TCGA kohortu:", cohorts)

    collection = db[selected_cohort]
    data = list(collection.find())
    if not data:
        st.warning("Nema podataka u ovoj kohorti.")
        st.stop()

    df = pd.DataFrame(data)


    selected_genes = st.multiselect("Odaberite gene za prikaz:", GENES, default=GENES[:5])

    patient_ids = df["patient_id"].tolist()
    selected_patient = st.selectbox("Odaberite pacijenta (opcionalno):", ["Svi pacijenti"] + patient_ids)

    if selected_patient != "Svi pacijenti":
        df_plot = df[df["patient_id"] == selected_patient]
    else:
        df_plot = df

    st.subheader("Expression values")

    if selected_genes:
        fig, ax = plt.subplots(figsize=(10, 6))
        df_plot[selected_genes].boxplot(ax=ax)
        ax.set_title(f"Expression of selected genes ({selected_cohort})")
        ax.set_ylabel("Expression value")
        st.pyplot(fig)

    st.subheader("Raw data")
    st.dataframe(df_plot[["patient_id", "cancer_cohort"] + selected_genes + ["DSS", "OS", "clinical_stage"]])

if __name__ == "__main__":
    main()
