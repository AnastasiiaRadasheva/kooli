# Iseseisev töö: Keeltekooli infosüsteem — Projekti kokkuvõte

See dokument koondab kogu projekti olulisima informatsiooni olemasoleva koodi põhjal. Kõik näited ja viited on pärit projekti lähtekoodist (ASP.NET MVC, .NET Framework 4.7.2).

## Kiirülevaade
- Projekt kaustas `Kool`.
- Tehnoloogiad: ASP.NET MVC 5, Entity Framework 6, ASP.NET Identity, Bootstrap.
- Peamised rollid: `Admin`, `Opetaja`, `Opilane`.

## Olulised failid
- `Kool\Models\IdentityModels.cs` — `ApplicationUser`, `ApplicationDbContext` (DbSet'id: `Keelekursused`, `Opetajad`, `Koolitused`, `Registreerimised`).
- `Kool\Controllers\OpetajasController.cs` — õpetajate CRUD, pildi üleslaadimine, kasutaja loomine ja rolli määramine.
- `Kool\Controllers\KoolitusController.cs` — koolituste haldus, registreerimine, rollipõhised vaated (`MinuKoolitused`).
- `Kool\Controllers\RegistreeriminesController.cs` — registreerimiste vood: Create(GET/POST), Pending, Approve, Reject.
- Vaated: `Views/Koolitus/*`, `Views/Opetajas/*`, `Views/Registreerimines/*`.

## Andmemudel (peamised väljad)
- `Opetaja`: `Id`, `Nimi`, `Kvalifikatsioon`, `FotoPath`, `ApplicationUserId`.
- `Koolitus`: `Id`, `KeelekursusId`, `OpetajaId`, `AlgusKuupaev`, `LoppKuupaev`, `Hind`, `MaxOsalejaid`.
- `Registreerimine`: `Id`, `KoolitusId`, `ApplicationUserId`, `Staatus` (Pending/Approved/Rejected), `CreatedAt`.

## Olulised koodinäited (otseselt projektist)

1) `IdentityModels` — DbSet'id ja indeks

```csharp
// Kool\Models\IdentityModels.cs
public DbSet<Keelekursus> Keelekursused { get; set; }
public DbSet<Opetaja> Opetajad { get; set; }
public DbSet<Koolitus> Koolitused { get; set; }
public DbSet<Registreerimine> Registreerimised { get; set; }

protected override void OnModelCreating(DbModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Registreerimine>()
        .Property(r => r.KoolitusId)
        .HasColumnAnnotation(
            IndexAnnotation.AnnotationName,
            new IndexAnnotation(new IndexAttribute("IX_UserKoolitus", 1) { IsUnique = true })
        );

    modelBuilder.Entity<Registreerimine>()
        .Property(r => r.ApplicationUserId)
        .HasColumnAnnotation(
            IndexAnnotation.AnnotationName,
            new IndexAnnotation(new IndexAttribute("IX_UserKoolitus", 2) { IsUnique = true })
        );
}
```

2) `OpetajasController.Create` — konto ja õpetaja loomine + pildi laadimine

```csharp
// Kool\Controllers\OpetajasController.cs (trimmitud)
[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Admin")]
public ActionResult Create(OpetajaCreateVM vm, HttpPostedFileBase foto)
{
    if (!ModelState.IsValid)
        return View(vm);

    var user = new ApplicationUser { UserName = vm.Email, Email = vm.Email };
    var result = userManager.Create(user, vm.Password);
    if (!result.Succeeded) { /* käsitlus */ }

    if (!roleManager.RoleExists("Opetaja"))
        roleManager.Create(new IdentityRole("Opetaja"));

    userManager.AddToRole(user.Id, "Opetaja");

    string fileName = "default.png";
    if (foto != null && foto.ContentLength > 0)
    {
        var ext = Path.GetExtension(foto.FileName).ToLower();
        var allowed = new[] { ".jpg", ".jpeg", ".png" };
        if (!allowed.Contains(ext)) { ModelState.AddModelError("", "Ainult JPG ja PNG."); return View(vm); }

        fileName = Guid.NewGuid() + ext;
        var path = Server.MapPath("~/Content/uploads/opetajad/");
        Directory.CreateDirectory(path);
        foto.SaveAs(Path.Combine(path, fileName));
    }

    var opetaja = new Opetaja { Nimi = vm.Nimi, Kvalifikatsioon = vm.Kvalifikatsioon, FotoPath = fileName, ApplicationUserId = user.Id };
    db.Opetajad.Add(opetaja);
    db.SaveChanges();

    return RedirectToAction("Index");
}
```

3) `RegistreeriminesController.Create` — GET: kontrollid enne vormi näitamist

```csharp
// Kool\Controllers\RegistreeriminesController.cs (GET Create)
[Authorize(Roles = "Opilane")]
public ActionResult Create(int? koolitusId)
{
    if (koolitusId == null)
        return RedirectToAction("Index", "Koolitus");

    var koolitus = db.Koolitused.Include(k => k.Registreerimised).FirstOrDefault(k => k.Id == koolitusId.Value);
    if (koolitus == null) return HttpNotFound();

    string userId = User.Identity.GetUserId();

    bool already = db.Registreerimised.Any(r => r.KoolitusId == koolitusId.Value && r.ApplicationUserId == userId);
    if (already) { TempData["msg"] = "Sul on juba olemas taotlus (või oled juba registreeritud)."; return RedirectToAction("MinuKoolitused"); }

    int approved = koolitus.Registreerimised.Count(r => r.Staatus == RegistreerimineStaatus.Approved);
    if (approved >= koolitus.MaxOsalejaid) { TempData["msg"] = "GRUPP TÄIS. Registreerimine pole võimalik."; return RedirectToAction("Index", "Koolitus"); }

    return View(new Registreerimine { KoolitusId = koolitusId.Value });
}
```

4) `RegistreeriminesController.Approve` — kinnitamine ja e‑posti saatmine

```csharp
// Kool\Controllers\RegistreeriminesController.cs (Approve)
public ActionResult Approve(int id)
{
    var reg = db.Registreerimised
        .Include(r => r.Koolitus)
        .Include(r => r.Koolitus.Keelekursus)
        .Include(r => r.ApplicationUser)
        .FirstOrDefault(r => r.Id == id);

    if (reg == null) return HttpNotFound();

    if (reg.Staatus == RegistreerimineStaatus.Pending)
    {
        reg.Staatus = RegistreerimineStaatus.Approved;
        db.SaveChanges();

        try
        {
            WebMail.SmtpServer = "smtp.gmail.com";
            WebMail.SmtpPort = 587;
            WebMail.EnableSsl = true;
            WebMail.UserName = "eha20082@gmail.com"; // leidub koodis
            WebMail.Password = "..."; // leitav koodis
            WebMail.From = "eha20082@gmail.com";

            string sisu = $@"<h2>Tere, {reg.ApplicationUser.UserName}!</h2>\n<p>Teid on vastu võetud kursusele: <b>{reg.Koolitus.Keelekursus.Nimetus}</b>.</p>";

            WebMail.Send(to: reg.ApplicationUser.Email, subject: "Kinnitus: " + reg.Koolitus.Keelekursus.Nimetus, body: sisu, isBodyHtml: true);
            TempData["msg"] = "Kasutaja on kinnitatud ja e-kiri saadetud!";
        }
        catch (Exception ex)
        {
            TempData["msg"] = "Kinnitatud, kuid e-kirja viga: " + ex.Message;
        }
    }

    return RedirectToAction("Registrations", "Koolitus", new { id = reg.KoolitusId });
}
```

5) Kiire registreerimine `KoolitusController.Register` (POST) — duplikaatkontroll

```csharp
// Kool\Controllers\KoolitusController.cs (Register)
[HttpPost]
[ValidateAntiForgeryToken]
[Authorize]
public ActionResult Register(int koolitusId)
{
    string userId = User.Identity.GetUserId();

    bool exists = db.Registreerimised.Any(r => r.KoolitusId == koolitusId && r.ApplicationUserId == userId);

    if (!exists)
    {
        db.Registreerimised.Add(new Registreerimine { KoolitusId = koolitusId, ApplicationUserId = userId, Staatus = RegistreerimineStaatus.Pending });
        db.SaveChanges();
        TempData["Msg"] = "Taotlus saadetud. Oota Admin kinnitust.";
    }
    else
    {
        TempData["Msg"] = "Sul on juba olemas taotlus (või oled juba registreeritud).";
    }

    return RedirectToAction("Details", new { id = koolitusId });
}
```

6) Vaade `IndexK` — peidab registreeru nupu kui grupp täis

```razor
// Views/Koolitus/IndexK.cshtml (fragment)
@if (Request.IsAuthenticated && User.IsInRole("Opilane"))
{
    if (!onTais)
    {
        @Html.ActionLink("Registreeru", "Create", "Registreerimines", new { koolitusId = item.Id }, new { @class = "btn btn-primary btn-sm" })
    }
}
```

## Soovitused ja järeldused
- Eemalda kõvakodeeritud SMTP‑saladused (`WebMail.UserName`/`Password`) ja kasuta `web.config` või muud turvalist salvestust.
- Ühtlusta registreerimisloogika ühte kohta, et vältida erinõudeid `KoolitusController.Register` ja `RegistreeriminesController.Create` vahel.
- Kuvada `TempData["msg"]` väärtus `Koolitus/Details` ja `Registreerimines/MinuKoolitused` vaadetes, et kasutaja näeks põhjust, miks ta suunati mujale.
- Laienda failiüleslaadimise kontrolli (MIME kontroll, suuruspiirang).

---

Dokumentation lisatud faili `Docs/Projektikonspet.md` on genereeritud otseselt projekti lähtekoodist. Kui soovite, saan automaatselt lisada TempData kuva näited vaadetesse (patš), või lisada veel täpsemaid koodipilte (nt kogu controlleri meetodid).