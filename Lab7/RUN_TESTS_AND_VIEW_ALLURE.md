# Cum să rulezi testele și să vezi raportul Allure

## Pași pentru a vedea raportul Allure cu date:

### 1. Rulează testele

Rulează testele folosind una dintre metodele:

**Din terminal (PowerShell):**
```powershell
dotnet test
```

**Sau din Visual Studio:**
- Deschide **Test Explorer** (Test → Test Explorer)
- Click dreapta pe proiect → **Run All Tests**

### 2. Verifică că rezultatele au fost generate

După rularea testelor, ar trebui să existe folderul `allure-results` în root-ul proiectului sau în `bin\Debug\net8.0\allure-results`.

### 3. Generează și deschide raportul Allure

**Opțiunea 1 - Serve direct (recomandat):**
```powershell
allure serve allure-results
```

Dacă rezultatele sunt în alt loc:
```powershell
allure serve bin\Debug\net8.0\allure-results
```

**Opțiunea 2 - Generează și deschide:**
```powershell
# Generează raportul
allure generate allure-results --clean --output allure-report

# Deschide raportul
allure open allure-report
```

## Notă importantă:

- **allure-results** conține rezultatele testelor (JSON, screenshot-uri) - se creează automat când rulezi testele
- **allure-report** conține raportul HTML generat - se creează când rulezi `allure generate`
- Dacă vezi mesajul "allure-results does not exist", înseamnă că testele nu au fost rulate încă sau nu au generat rezultate

## Troubleshooting:

**Dacă nu vezi rezultate:**
1. Asigură-te că testele au fost rulate: `dotnet test`
2. Verifică dacă există folderul `allure-results` în root sau în `bin\Debug\net8.0\`
3. Dacă rezultatele sunt în `bin\Debug\net8.0\allure-results`, rulează:
   ```powershell
   allure serve bin\Debug\net8.0\allure-results
   ```

**Dacă vrei să salvezi rezultatele întotdeauna în root:**
- Testele vor salva automat în `allure-results` dacă rulezi din root-ul proiectului
- Allure.XUnit salvează rezultatele în directorul de lucru curent

