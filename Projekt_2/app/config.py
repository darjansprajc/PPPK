import os
from dotenv import load_dotenv

load_dotenv()

MINIO_ENDPOINT = os.getenv("MINIO_ENDPOINT", "localhost:9000")
MINIO_ACCESS_KEY = os.getenv("MINIO_ACCESS_KEY", "minio")
MINIO_SECRET_KEY = os.getenv("MINIO_SECRET_KEY", "minio12345")
MINIO_BUCKET = os.getenv("MINIO_BUCKET", "tcga")
MINIO_SECURE = False
MINIO_FOLDER = "expression/"

MONGO_URL = os.getenv("MONGO_URL", "mongodb://localhost:27017")
DB_NAME = os.getenv("DB_NAME", "cancer_genome")

GENES = [
        "C6orf150", "CCL5", "CXCL10", "TMEM173", "CXCL9",
        "CXCL11", "NFKB1", "IKBKE", "IRF3", "TREX1", "ATM",
        "IL6", "IL8"
    ]
