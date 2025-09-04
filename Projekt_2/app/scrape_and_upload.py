from config import MINIO_ENDPOINT, MINIO_ACCESS_KEY, MINIO_SECRET_KEY, MINIO_BUCKET, MINIO_FOLDER

import os
import time
import requests
import shutil
import gzip

from bs4 import BeautifulSoup
from minio import Minio
from minio.error import S3Error
from selenium import webdriver
from selenium.webdriver.chrome.options import Options


client = Minio(
    MINIO_ENDPOINT,
    access_key=MINIO_ACCESS_KEY,
    secret_key=MINIO_SECRET_KEY,
    secure=False
)

if not client.bucket_exists(MINIO_BUCKET):
    client.make_bucket(MINIO_BUCKET)

BASE_URL = "https://xenabrowser.net/datapages/"
START_PAGE = f"{BASE_URL}?hub=https://tcga.xenahubs.net:443"

def download_file(url, local_path):
    resp = requests.get(url, stream=True)
    if resp.status_code == 200:
        with open(local_path, "wb") as f:
            for chunk in resp.iter_content(chunk_size=8192):
                f.write(chunk)
        print(f"Preuzeto: {local_path}")
        return True
    else:
        print(f"Greška pri preuzimanju {url} (status {resp.status_code})")
        return False

def unpack_gz(gz_path, tsv_path):
    with gzip.open(gz_path, 'rb') as f_in:
        with open(tsv_path, 'wb') as f_out:
            shutil.copyfileobj(f_in, f_out)
    print(f"Otpakirano u {tsv_path}")

def upload_to_minio(local_path, object_name):
    try:
        client.fput_object(MINIO_BUCKET, object_name, local_path)
        print(f"Uploadano u MinIO: {object_name}")
    except S3Error as e:
        print(f"Greška pri uploadu: {e}")

def main():
    chrome_options = Options()
    chrome_options.headless = True
    driver = webdriver.Chrome(options=chrome_options)
    driver.get(START_PAGE)
    time.sleep(5)

    soup = BeautifulSoup(driver.page_source, "html.parser")

    cohort_links = []
    for a in soup.find_all("a", href=True):
        href = a["href"]
        if href.startswith("?cohort=TCGA"):
            full_url = BASE_URL + href
            cohort_links.append(full_url)

    print(f"Pronađeno {len(cohort_links)} TCGA kohorti")

    for cohort_url in cohort_links:
        print(f"\n Obrada: {cohort_url}")

        driver.get(cohort_url)
        time.sleep(5)
        soup = BeautifulSoup(driver.page_source, "html.parser")

        dataset_page_url = None
        for a in soup.find_all("a", href=True):
            if "IlluminaHiSeq" in a.text and "pancan normalized" in a.text.lower():
                dataset_page_url = BASE_URL + a["href"]
                break

        if not dataset_page_url:
            print("Nema IlluminaHiSeq pancan normalized linka")
            continue

        print(f"Dataset stranica: {dataset_page_url}")
        driver.get(dataset_page_url)
        time.sleep(5)
        dataset_soup = BeautifulSoup(driver.page_source, "html.parser")

        download_link = None
        for a in dataset_soup.find_all("a", href=True):
            if a["href"].endswith(".gz") and "download" in a["href"]:
                download_link = a["href"]
                break

        if not download_link:
            print("Nije pronađen .gz download link na dataset stranici")
            continue

        print(f"Pronađena datoteka: {download_link}")

        os.makedirs("data", exist_ok=True)
        gz_filename = download_link.split("/")[-1]
        gz_path = os.path.join("data", gz_filename)
        tsv_filename = gz_filename.replace(".gz", "")
        tsv_path = os.path.join("data", tsv_filename)

        if download_file(download_link, gz_path):
            unpack_gz(gz_path, tsv_path)
            upload_to_minio(tsv_path, f"{MINIO_FOLDER}{tsv_filename}")

    print("\n Uploadanje u MinIO završeno!")

    driver.quit()

if __name__ == "__main__":
    main()
