# TCGA Gene Expression Project

Ovaj projekt omogućuje dohvat, pohranu i vizualizaciju podataka o genskoj ekspresiji onkoloških pacijenata iz TCGA projekta koristeći:
- **MiniO** (nestrukturirana pohrana podataka)
- **MongoDB** (nerelacijska baza podataka)
- **Streamlit** (vizualizacija podataka)

---

## Preduvjeti

1. **Python** 

2. **MiniO (community build)**  
   - Preuzmi najnoviji `minio.exe` za Windows s GitHub releases:  
     👉 [https://github.com/minio/minio/releases](https://github.com/minio/minio/releases)  
   - Spremi ga u `C:\minio\minio.exe`.  
   - Pokreni:
     ```powershell
     cd C:\minio
     setx MINIO_ROOT_USER "minio"
     setx MINIO_ROOT_PASSWORD "minio12345"
     .\minio.exe server C:\minio\data --console-address ":9001"
     ```
   - Web konzola: [http://localhost:9001](http://localhost:9001)  
   - Endpoint: `http://localhost:9000`  

3. **MongoDB Community Server**  
   - Preuzmi i instaliraj s: [https://www.mongodb.com/try/download/community](https://www.mongodb.com/try/download/community)  
   - GUI alat (preporuka): **MongoDB Compass**.  
   - Testiraj u PowerShellu:
     ```powershell
     mongosh
     ```

---

## Postavljanje Python okruženja

1. **Kloniraj ili napravi mapu projekta:**
   ```
   powershell
   cd C:\Users\<TvojeIme>\
   mkdir tcga_project
   cd tcga_project 
   ```

2. **Napravi virtualno okruženje:**
	```
	python -m venv venv
	.\venv\Scripts\activate
	```
3. **Instaliraj ovisnosti:**
	```
   pip install -r requirements.txt`
   ```
		
## Radni tok

1. **Pokreni MiniO**
	```
	C:\minio\start_minio.bat
	```
2. **Pokreni MongoDB.**

3. **Dohvati i uploadaj TSV datoteke u MinIO**
   ```
	python .\app\scrape_and_upload.py
   ```

4. **Transformiraj i spremi u MongoDB**
   ```
	python .\app\transform_to_mongo.py
   ```

5. **Pokreni vizualizaciju (Streamlit app)**
   ```
	streamlit run .\app\visualize_app.py
   ```

**Otvori http://localhost:8501**