#  KeelteKooli

Keelekursuste haldamise rakendus, mis on tehtud **C#** ja **Windows Forms** abil. Projekt on valminud lõputööna ning selle eesmärk on pakkuda lihtsat ja töökindlat lauarakendust, millega saab hallata keelekursuseid, õpetajaid ja õpilaste registreerimisi — ilma keeruka veebi- või pilvetaristuta.

##  Mida see teeb

Väiksemad koolid ja keelekursuste pakkujad ei vaja alati suurt veebisüsteemi — sageli piisab lihtsast kohalikust rakendusest. Kooli võimaldabki:

- lisada, muuta ja kustutada **keelekursuseid**
- hallata **õpetajate** andmeid
- vaadata ja hallata **õpilaste registreerimisi** kursustele

##  Tehnoloogiad

| Tehnoloogia | Kasutuseesmärk |
|---|---|
| **C#** | rakenduse loogika |
| **Entity Framework Core** (Code First) | andmemudel ja andmebaasi haldus |
| **LocalDB** | kohalik andmebaas arenduseks ja testimiseks |
| **EF Core Migrations** | andmebaasi skeemi versioonihaldus |
| **Visual Studio** | arenduskeskkond |

##  Arhitektuur

Rakendus on üles ehitatud kolmekihiliselt, et kood oleks selge ja hooldatav:

```
Presentation 
        ↓
Business Logic (teenused, valideerimine)
        ↓
Data Access (DbContext, repository)
```

Iga kiht vastutab oma osa eest — vorm ei tea andmebaasist midagi, äriloogika ei tea vormidest midagi. See teeb koodi lihtsamini testitavaks ja muudetavaks.

##  Andmemudel

Kolm peamist olemit:

- **Keelekursus** — nimetus, keel, tase, kirjeldus
- **Õpetaja** — eesnimi, perekonnanimi, e-mail, telefon
- **Registreerimine** — õpilase nimi, kuupäev, seotud kursus

Üks õpetaja võib vedada mitut kursust, ühel kursusel võib olla mitu registreerimist.

##  Repo struktuur

```
├── Kool/          # rakenduse lähtekood (Visual Studio projekt)
├── Docs/          # dokumentatsioon
├── Kool.slnx      # Visual Studio lahenduse fail
└── loputoo.md     # lõputöö tekst (teooria + praktiline kirjeldus)
```

##  Käivitamine

1. Ava `Kool.slnx` Visual Studios
2. Veendu, et LocalDB on paigaldatud
3. Käivita Package Manager Console'is:
   ```
   Update-Database
   ```
4. Käivita rakendus (F5)

##  Mida saaks edasi arendada

- Veebiversioon (ASP.NET Core), et rakendust saaks kasutada mitu kasutajat korraga
- Kasutajarollid ja autentimine (admin vs õpilane)
- Automaattestid ja CI/CD (nt GitHub Actions)
- Parem andmebaasi varundus tootmiskeskkonnas

---

**Autor:** Anastasiia Radasheva
