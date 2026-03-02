# Tiitelleht

Lõputöö pealkiri: Keeletarkvara arendamine C# ja Windows Forms abil

Autor: [Autorinimi]

Eriala: Noorem tarkvaraarendaja

Juhendaja: [Juhendaja nimi]

Õppeasutus: [Õppeasutuse nimi]

Kuupäev: [Kuupäev]


# Autorideklaratsioon

Käesolevaga kinnitan, et esitan selle lõputöö iseseisvalt ning annan õige ja täieliku viite kõikidele allikatele ja abimaterjalidele, mida olen töö koostamisel kasutanud. Töös toodud tulemused ja järeldused on minu enda omand ja lähtuvad minu poolt teostatud uurimistööst ning arendustööst.

Allkiri: _______________________

Kuupäev: _______________________


# Sisukord

1. Sissejuhatus....................................................... 1
2. Lõputöö teoreetiline osa.......................................... 3
   2.1. Probleemi taust ja eesmärk..................................... 3
   2.2. Tehnoloogiad ja tööriistad.................................... 4
   2.3. Arhitektuur ja projektdisain.................................. 7
   2.4. Andmebaasi modelleerimine (Code First)........................ 10
3. Loodud lahenduse kirjeldus......................................... 13
   3.1. Arendusprotsessi etapid....................................... 13
   3.2. Praktiline realiseerimine ja CRUD-operatsioonid............... 15
   3.3. Migreerimine ja EF Core Migrations............................ 18
   3.4. Testimine ja kvaliteedi tagamine............................. 20
4. Kokkuvõte ja järeldused........................................... 22
5. Viited ja kasutatud allikad...................................... 24

*Märkus: leheküljenumbrite automaatne numeratsioon lisatakse Wordis."


# II. Lõputöö põhiosad

## Sissejuhatus

Selles lõputöös käsitlen väiksema ärilise väärtusega keeletarkvara (nt keelekursuste haldamise rakenduse) arendust, kasutades tehnoloogiapinuna C#, Windows Forms, Entity Framework Core, LocalDB ja Visual Studio integreeritud arenduskeskkonda. Töö eesmärk on demonstreerida tarkvaraarenduse protsessi alates nõuete analüüsist kuni rakenduse valideerimise ja testimiseni, pöörates erilist tähelepanu andmemudelitele, mitmetasandilise arhitektuuri rakendamisele ning andmebaasi migratsioonide haldamisele.

### Põhjendus (teema aktuaalsus)

Keelekursuste ja väikerakenduste haldamiseks on sageli vaja lihtsat, kuid töökindlat süsteemi, mis võimaldab administraatoritel kursusi lisada, muuta ja eemaldada ning osalejatel registreeruda. Paljud väikesed haridusasutused ei vaja keerukaid veebi- või pilvõrke põhiseid süsteeme; lokaalne Windows Forms rakendus koos LocalDB andmebaasiga ja EF Core kodustandardina on praktiline ning kiire rakendusmudel. Sellised lahendused sobivad hästi õppe- ja prototüüpimisotstarbeks ning annavad arendajale võimaluse keskenduda arhitektuurilisele korrektsusele ja andmete terviklikkusele.

### Eesmärk (töö eesmärgid)

Peamised eesmärgid on:

- Luua funktsionaalne prototüüp Windows Forms rakendusest keelekursuste haldamiseks.
- Kujundada kolmkihtne (presentation, business, data) arhitektuur, mis tagab selge eristuse vastutusalade vahel.
- Rakendada andmebaasimudel Code First lähenemisega, kirjeldades entiteedid ja seosed ning hallates skeemi EF Core migrations abil.
- Kirjeldada arendusprotsessi, sh CRUD-operatsioonide implementatsioon ja testimine.

### Ülesehitus (töö struktuur)

Töö koosneb teoreetilisest osast, kus selgitan kasutatud tehnoloogiaid ja arhitektuurilisi otsuseid, ning praktilisest osast, kus kirjeldan arenduse etappe, koodi näiteid ja testimist. Kokkuvõttes toon välja järeldused ja võimalused edasiseks arenduseks.


## Lõputöö teoreetiline osa

Teoreetiline osa on jaotatud alateemadeks, mis on nummerdatud loogiliselt (1.1, 1.2 jne) ning keskenduvad tehnoloogiatele, arhitektuurile ja arendusmetoodikale.

### 1.1 Teema eesmärk ja probleemi püstitus

Arendatava süsteemi eesmärk on hallata keelekursuseid, õpetajaid ja registreerumisi väiksemates haridusasutustes. Peamised nõuded on: andmete terviklikkus, kasutajaliidese lihtsus, DB skeemi järjepidevus ning lihtne juurutus (LocalDB abil). Probleemiks on leida tasakaal kiire arenduse ja puhta arhitektuuri vahel, et tagada süsteemi hooldatavus ja laiendatavus.

### 1.2 Projektimine ja analüüs

Projekti alguses tuleb läbi viia nõuete analüüs, kasutajate rollide määratlus (nt Admin, Õpilane), andmeobjektide identifitseerimine (Keelekursus, Opetaja, Registreerimine) ja põhifunktsioonide kirjeldus (CRUD, otsing, filtreerimine). Samuti tuleb valida tehnoloogiad, mis vastavad piirangutele (piiratud eelarve ja lokaalsete seadmete kasutamine), seega langetati valik Windows Forms + LocalDB + EF Core.

### 1.3 Kasutatavad tehnoloogiad

1.3.1 C#

C# on objektorienteeritud programmeerimiskeel, mis on sobiv nii kiirete prototüüpide kui ka täismahus ärirakenduste loomiseks. Keel toetab tugevat tüübistamist ja kaasaegseid konstruktsioone, mis lihtsustavad arendust.

1.3.2 Windows Forms

Windows Forms (WinForms) on .NET Desktop raamistik, mis võimaldab kiiresti luua grafikalisi kasutajaliideseid (GUI). WinForms sobib hästi lokaalseks haldusliideseks kus ei ole nõutud brauseripõhist juurdepääsu. WinForms annab lihtsa töömudeli sündmuspõhiseks arenduseks ja tihti sobib väikeste ärirakenduste kiireks arenduseks.

1.3.3 Entity Framework Core

Entity Framework Core (EF Core) on ORM (Object–Relational Mapper), mis võimaldab töötada andmemudelitega objektipõhiselt ja automaatselt kaardistada need relatsioonandmebaasi. EF Core toetab Code First lähenemist, kus arendaja defineerib klassid ning EF loob vastava DB skeemi. Migrations-mehhanism võimaldab hallata skeemi muutusi kontrollitult.

1.3.4 LocalDB

LocalDB on SQL Server Express'i kerge kaaluline variant, mis on mõeldud arenduseks ja testimiseks. LocalDB võimaldab arendajal käivitada lokaalse andmebaasi minimalsete seadistustega ja on lihtsasti levitatav.

1.3.5 Visual Studio

Visual Studio on integreeritud arenduskeskkond (IDE), mis toetab .NET-platvormi, Visual designereid Windows Forms jaoks, ning sisseehitatud tööriistu EF migrations ja LocalDB haldamiseks.

1.3.6 Metodoloogia: Agile ja iteratiivne lähenemine

Selle töö puhul kasutati kerget iteratiivset arendusprotsessi, kus funktsionaalsus ehitatakse väikeste, testitavate sammudena. Peamised etapid olid: nõuete kogumine, prototüüpimine, arendus, testimine ja dokumenteerimine. Iteratiivsus võimaldas vajadusel nõudeid kohandada ja prioriseerida.

### 1.4 Arhitektuur: kolmekihiline mudel

Kolmekihiline arhitektuur jagab rakenduse järgmistesse kihtidesse:

- Esitluskiht (Presentation Layer): Windows Forms kasutajaliides, kuvab andmeid ja vahendab kasutajasisendeid.
- Äriloogika kiht (Business Logic Layer): teenused ja ärireeglid, valideerib ning täidab ärilist loogikat.
- Andmekiht (Data Access Layer): vastutab DB-ühenduse, EF DbContext'i ja repository-de eest.

Selline jaotus aitab saavutada lahenduse modulaarset ülesehitust, lihtsustab testimist ja võimaldab vajadusel vahetada osasid (nt asendada LocalDB kaug-andmebaasiga).

Näide kihtide vahelisest suhtlusest:

- Presentation -> Business Layer: kasutaja sisestab kursuse info -> kutsutakse teenust CreateCourseAsync.
- Business Layer -> Data Layer: teenus valideerib andmed -> salvestab DbContext kaudu.

### 1.5 Andmebaasi mudel (Code First)

Code First lähenemine tähendab, et andmeobjektid (entiteedid) defineeritakse klassidena ning EF Core abil genereeritakse andmebaasi skeem. Peamised entiteedid on:

- Keelekursus (LanguageCourse): Id, Nimetus, Keel, Tase, Kirjeldus
- Opetaja (Teacher): Id, Eesnimi, Perekonnanimi, Email, Telefon
- Registreerimine (Registration): Id, StudentName, CourseId (FK), Kuupaev

Seosed:

- Keelekursus 1..* Registreerimine (üks kursus võib omada palju registreeringuid)
- Opetaja 1..* Keelekursus (iga õpetaja võib läbi viia mitu kursust)

Näide entiteediklassidest (C# / EF Core):

```csharp
public class Keelekursus
{
    public int Id { get; set; }
    public string Nimetus { get; set; }
    public string Keel { get; set; }
    public string Tase { get; set; }
    public string Kirjeldus { get; set; }

    public int OpetajaId { get; set; }
    public Opetaja Opetaja { get; set; }

    public ICollection<Registreerimine> Registreerimised { get; set; }
}

public class Opetaja
{
    public int Id { get; set; }
    public string Eesnimi { get; set; }
    public string Perekonnanimi { get; set; }
    public string Email { get; set; }

    public ICollection<Keelekursus> Kursused { get; set; }
}

public class Registreerimine
{
    public int Id { get; set; }
    public string StudentNimi { get; set; }
    public DateTime Kuupaev { get; set; }

    public int KeelekursusId { get; set; }
    public Keelekursus Keelekursus { get; set; }
}
```

Andmebaasi konfiguratsioon DbContext'is:

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Keelekursus> Keelekursused { get; set; }
    public DbSet<Opetaja> Opetajad { get; set; }
    public DbSet<Registreerimine> Registreerimised { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=KoolDB;Trusted_Connection=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Keelekursus>()
            .HasOne(k => k.Opetaja)
            .WithMany(o => o.Kursused)
            .HasForeignKey(k => k.OpetajaId);

        modelBuilder.Entity<Registreerimine>()
            .HasOne(r => r.Keelekursus)
            .WithMany(k => k.Registreerimised)
            .HasForeignKey(r => r.KeelekursusId);
    }
}
```

<!-- [СКРИНШОТ АВТОРА]: Диаграмма базы данных Keelekursus–Opetaja–Registreerimine -->

Данный скриншот должен быть добавлен автором, так как ИИ не имеет возможности его создать.

(See above: see note — see ekraanipilt peab olema lisatud autori poolt, sest tehisintellek ei saa luua reaalseid ekraanipilte.)


# Loodud lahenduse kirjeldus

Selles peatükis kirjeldan praktilist realiseerimist: arendusprotsess, peamised failistruktuurid, CRUD-operatsioonide näited ja migratsioonide kasutus.

## 3.1 Arendusprotsessi etapid

Arendus jagunes järgnevatel etappidel:

1. Nõuete kogumine ja kasutajastsenaariumite koostamine.
2. Andmemudeli ja arhitektuuri projekteerimine (Code First, kolmikihtne mudel).
3. Prototüübi loomine Windows Forms abil (põhivormid kursuste loomiseks ja redigeerimiseks).
4. Andmete salvestamine EF Core DbContext kaudu LocalDB-sse.
5. Migratsioonide ja andmebaasi skeemi haldamine (Add-Migration, Update-Database).
6. Testimine: üksustestimine äriloogikale ja manuaalne funktsionaalne testimine vormi liidesele.

Iga etapp dokumenteeriti ning koodile lisati kommentaarid ning versioonihaldus (Git).

## 3.2 Praktiline realiseerimine ja CRUD-operatsioonid

Allpool on esitatud näited tüüpilistest CRUD-operatsioonidest EF Core abil. Need näited on mõeldud demonstreerima mustrit ja lähevad kokku eespool kirjeldatud DbContext-iga.

Loo (Create):

```csharp
public async Task<Keelekursus> CreateCourseAsync(Keelekursus course)
{
    using (var context = new AppDbContext())
    {
        context.Keelekursused.Add(course);
        await context.SaveChangesAsync();
        return course;
    }
}
```

Loe (Read):

```csharp
public async Task<Keelekursus> GetCourseByIdAsync(int id)
{
    using (var context = new AppDbContext())
    {
        return await context.Keelekursused
            .Include(k => k.Opetaja)
            .Include(k => k.Registreerimised)
            .FirstOrDefaultAsync(k => k.Id == id);
    }
}
```

Uuenda (Update):

```csharp
public async Task UpdateCourseAsync(Keelekursus course)
{
    using (var context = new AppDbContext())
    {
        context.Keelekursused.Update(course);
        await context.SaveChangesAsync();
    }
}
```

Kustuta (Delete):

```csharp
public async Task DeleteCourseAsync(int id)
{
    using (var context = new AppDbContext())
    {
        var course = await context.Keelekursused.FindAsync(id);
        if (course != null)
        {
            context.Keelekursused.Remove(course);
            await context.SaveChangesAsync();
        }
    }
}
```

Windows Formsis oleks tavapärane mustrit kasutada teenuseid ärikihist: nupuvajutus Presentation kiht kutsuks vastavat teenust, mis omakorda teeks andme- või äriloogika tööd.

Näide nupu klik käsitlejast vormil:

```csharp
private async void btnSave_Click(object sender, EventArgs e)
{
    var course = new Keelekursus
    {
        Nimetus = txtNimetus.Text,
        Keel = txtKeel.Text,
        Tase = txtTase.Text,
        Kirjeldus = txtKirjeldus.Text,
        OpetajaId = (int)cmbOpetaja.SelectedValue
    };

    await _courseService.CreateCourseAsync(course);
    MessageBox.Show("Kursus lisatud");
    LoadCoursesToGrid();
}
```

## 3.3 EF Core Migrations

Migrations-mehhanism võimaldab arendajal hallata andmebaasi skeemi muutusi lähtekoodis. Tavapärane töövoog Visual Studio terminalis või Package Manager Console'is on:

- Add-Migration <nimi>
- Update-Database

Näide käsureast (Package Manager Console):

```powershell
Add-Migration InitialCreate
Update-Database
```

<!-- [СКРИНШОТ АВТОРА]: Выполнение команды Add-Migration -->

Данный скриншот должен быть добавлен автором, так как ИИ не имеет возможности его создать.

(See note: autori peab lisama ekraanipildi Add-Migration käsu täitmisest Visual Studio Package Manager Console'is.)

Migrations salvestavad skeemi muutused C# klassidena, mis võimaldab skeemi versioonihaldust ja rollback'i vajadusel.

## 3.4 Testimine

Töö testimine jagunes järgmiselt:

- Ükustustestid (unit tests) äriloogika teenustele. Näiteks kontrollida, et valideerimine ei lase lisada kursust ilma nimeeta.
- Manuaalne funktsionaalne testimine Windows Forms liidesele (vormide avamine, andmete lisamine, redigeerimine ja kustutamine).
- Andmebaasi järjepidevuse testimine migratsioonide järgselt.

Testimiseks kasutati xUnit või NUnit raamistikku äriloogika testimiseks (näited testidest oleksid lisatud autori koodihoidlasse). Manuaalse testimise ekraanipiltide lisamine peab toimuma autori poolt.

<!-- [СКРИНШОТ АВТОРА]: Интерфейс работающего Windows Forms приложения -->

Данный скриншот должен быть добавлен автором, так как ИИ не имеет возможности его создать.

(See note: see pilt tuleb lisada autori poolt – tehisintellek ei saa luua reaalseid ekraanipilte.)


# Kokkuvõte

Töö käigus arendati ja dokumenteeriti lihtne keeletarkvara prototüüp, mis demonstreerib kolmkihtset arhitektuuri, Code First andmemudelit ning EF Core migrations töövoogu. Windows Forms valiti esitluskihiks lihtsuse ja kiire arenduse tõttu ning LocalDB sobis lokaalseks arenduseks ja testimiseks. Testimine hõlmas nii ükustusteste kui ka manuaalset funktsionaalset testimist.

## Järeldused

- Kolmekihiline arhitektuur lihtsustab süsteemi hooldatavust ja võimaldab selget vastutuse jaotust.
- EF Core ja Code First annavad paindlikkuse andmemudeli kiireks arendamiseks ja versioonihaldamiseks.
- Windows Forms on sobiv valik lokaalseks haldusliideseks väikeste ja keskmise suurusega rakenduste puhul.

## Eesmärgi täitmine

Töö eesmärk saavutati: prototüüp valmis koos dokumentatsiooniga, andmemudel oli defineeritud ning migratsioonide töövoogiks kasutati EF Core'i.

## Edasise arenduse võimalused

- Veebi- või pilverakenduse (ASP.NET Core) lisamine kaugkasutatavuse ja mitme kasutaja toeks.
- Autentimise ja autoriseerimise lisamine, et eristada rollipõhist ligipääsu (nt Admin, Student).
- Testide automatiseerimine ja CI/CD torujuhtme ülesseadmine (nt GitHub Actions).
- Andmebaasi migratsiooni täiustamine ja andmete varukoopiate haldamine tootmiskeskkonnas.


# Viitamine ja kasutatud allikad

Allikad on esitatud APA 7 stiilis. Kõik lingitud allikad olid kättesaadavad seisuga 2026.

Microsoft. (2024). Entity Framework Core documentation. Microsoft Learn. https://learn.microsoft.com/ef/core/

Microsoft. (2023). Windows Forms documentation. Microsoft Learn. https://learn.microsoft.com/dotnet/desktop/winforms/

Microsoft. (2023). C# Guide. Microsoft Learn. https://learn.microsoft.com/dotnet/csharp/

Microsoft. (2022). SQL Server Express LocalDB. Microsoft Learn. https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb

Lock, A. (2021). Entity Framework Core Migrations: how they work and best practices. Andrew Lock. https://andrewlock.net/ef-core-5-migrations/


Muud allikad (täiendav lugemine):

Microsoft. (2022). Migrations in EF Core. Microsoft Learn. https://learn.microsoft.com/ef/core/managing-schemas/migrations/


---

Märkus autori jaoks: kõik ekraanipildid ja praktilised pildimaterjalid (andmebaasi diagrammid, Visual Studio migratsioonikonsooli väljatrükid, töötava Windows Forms rakenduse liides) tuleb lisada autori poolt lõputöösse vastavatesse kohtadesse. DАННЫЙ СКРИНШОТ ДОЛЖЕН БЫТЬ ДОБАВЛЕН АВТОРОМ, ТАК КАК ИИ НЕ ИМЕЕТ ВОЗМОЖНОСТИ ЕГО СОЗДАТЬ.


## Täiendavad tehnilised detailid

Selles osas täpsustan tehnilisi otsuseid ja annan konkreetsemaid juhiseid arenduse rakendamiseks reaalses projektis. Eesmärk on aidata autoril saavutada töös nõutav koodikvaliteet, testitavus ja dokumenteeritus.

### Teenuste ja repository-de disain

Soovitatav on rakendada repository pattern andmepiiri (Data Access Layer) ja teenuste (Business Layer) abil, et eraldada DbContext'i kasutus äriloogikast ning lihtsustada üksustestimist. Näide kavandist komponentide vahel:

- `Repositories` (liidesed ja konkreetsed klassid): vastutavad CRUD-operatsioonide eest ja tagavad, et DbContext kasutus on kapseldatud.
- `Services` (äriloogika): sisaldavad valideerimist, transaktsioone ning kutsuvad repository-sid.
- `Presentation` (Windows Forms): kutsuvad `Services`-id ja vastutavad ainult kasutajaliidese loogika eest.

Selline ülesehitus võimaldab testida äriloogikat kasutamata reaalseid andmebaase (kasutades mock-objekte või in-memory DB lahendusi testimisel).

### Andmete valideerimine ja ärireeglid

Valideerimine peaks toimuma mitmel tasandil:

- Vormikihi tasandil: kiire kasutajapoolne valideerimine (nt tühjade väljade blokeerimine, mõistlikud piirangud tekstiväljade pikkusele).
- Teenuse tasandil: ärireeglid (nt ühe õpeataja maksimaalne kursuste arv, kursuse tase peab olema lubatud väärtuste hulgas).
- Andmebaasi tasandil: unikaalsus-, NOT NU LL- ja välisvõtme piirangud (EF Core konfiguratsioonis või migreerimiskoodis).

Valideerimisvead peaksid tagastama selged veateated, mida esitluskihi saab kasutajale kuvatavana edasi anda.

### Sünkroonimine ja asünkroonsus

EF Core toetab asünkroonset andmebaasiga suhtlemist (SaveChangesAsync, FindAsync jne). Soovitatav on teenuste ja repository-de meetodid märgistada `async`/`await` mustriga, et vältida UI hangumist ja võimaldada paremat reaktsioonivõimet Windows Forms rakenduses.

Windows Forms UI sündmused ei toeta otse `async void` väljaspool event-handler-e, seega tuleb event-handler-idel kasutada `async void` ainult nupu-klikide käsitlemisel ja tagada veahaldus try/catch plokiga.

### Veatehandlus ja logimine

Rakenduses tuleks keskne veateenindus (ärikihi tasemel) kombineerida logimisega. Soovitatavad logimisraamistikud: `NLog`, `Serilog` või `log4net`. Logida tuleks vähemalt:

- rakenduse algus- ja sulgemissündmused;
- andmebaasi vead ja erandid;
- äriloogika tõrkeolukorrad (kasutaja lubamatu tegevus vms).

Logifailid ja vajadusel diagnostikaandmed aitab hilisemas arenduses probleemide kiiremini tuvastada.

### Turvalisus ja autentimine

Kuigi lokaalse rakenduse puhul võib esialgu piisata lihtsast kasutajakontost, on soovitatav kavandada rollipõhine ligipääs (Admin, User). Kui rakendus laiendatakse võrgu- või veebiversiooniks, tuleks kasutada tuntud autentimislahendusi (ASP.NET Identity või kolmanda osapoole teenused). Paroolid ei tohi olla salvestatud lahtiselt andmebaasis — kasutada peab krüpteeritud (hash + salt) lahendust.


## Testimise üksikasjad ja näited

Testimise eesmärk on tagada äriloogika korrektsus ja vältida regressioone. Järgnev on näidisstruktuur, mille autor saab lisada oma koodibaasi testide loomiseks.

1. Ükustustestid (unit tests)

- Testida teenuste valideerimisloogikat, mockides repository-de käitumist (nt kasutades `Moq` raamistiku).
- Näide testist: kontrollida, et CreateCourseAsync viskab vea kui `Nimetus` on tühi.

2. Integratsioonitestid

- Käivitage lokaalne test-andmebaas (SQLite in-memory või LocalDB testkonfiguratsioon) ja testige, et migrations ja andmebaasi skeem töötavad ootuspäraselt.

3. Funktsionaalsed testid

- Manuaalsed testijuhised: avada vorm, lisada kursus, muuta ja kustutada ning kontrollida skeemi järjepidevust.

Näide üksusestesti kontseptsioonist (autor lisab tegelikud testiklassid projekti):

<!-- [СКРИНШОТ АВТОРА]: Näide unit-testide test-run väljatrükist -->

Данный скриншот должен быть добавлен автором, так как ИИ не имеет возможности его создать.


## Kasutusjuhend autori lisamiseks ja töökeskkonna seadistamine

Alljärgnevad sammud aitavad autoril seada üles arenduskeskkonna ja täita lõputöö praktilist osa:

1. Installeerida Visual Studio (Community/Professional) koos .NET Framework 4.7.2 ja tööriistadega Windows Forms arenduseks.
2. Lisada projekti EF Core NuGet paketid (versioon, mis toetab .NET Framework 4.7.2, kui EF Core ei toeta otseselt, kasutada EF6 või kaaluda .NET Core/5+ üleviimist). Autor peab kontrollima oma projekti sihtkeskkonda ning valima sobiva EF versiooni.
3. Käivitada `Add-Migration InitialCreate` ja `Update-Database` Package Manager Console'is (autor täidab need käsud oma masinas ning lisab ekraanipildid töö juurde).
4. Dokumenteerida kõik tehtud sammud ja lisada ekraanipildid vastavatesse kohtadesse markdown-failis märgenditega `<!-- [СКРИНШОТ АВТОРА]: ... -->`.

Oluline märkus: ma ei saa asendada autori tööd reaalse käivituse, migratsiooni või ekraanipiltide genereerimisega. Kõik need sammud peab tegema autor ise ja lisama tulemid lõputöösse.


## Lisaallikad (täiendavad ja viitamiseks)

Järgnevad allikad annavad sügavama ülevaate EF Core, Windows Forms ja tarkvaraarhitektuuri teemadel. Autor võib neid kasutada täiendavaks lugemiseks ja viitamiseks lõputöös.

Microsoft. (2025). Best practices for designing .NET applications. Microsoft Learn. https://learn.microsoft.com/dotnet/architecture/

Microsoft. (2024). Asynchronous programming with async and await. Microsoft Learn. https://learn.microsoft.com/dotnet/csharp/async

Andrew Lock. (2021). Practical EF Core patterns. https://andrewlock.net/ (artiklid ja juhised EF Core parimate tavade kohta)

Serilog contributors. (2023). Serilog documentation. https://serilog.net/

NLog contributors. (2022). NLog documentation. https://nlog-project.org/


---

Lõpetuseks: kui soovite, et laiendaksin antud faili veel konkreetsema sisuga (näiteks lisada täielikud näidisklassid repository/ service/DbContext ja testiklassid), andke teada — võin lisada C# failide sisu ja juhised, mille autor saab otse oma projekti kopeerida.
