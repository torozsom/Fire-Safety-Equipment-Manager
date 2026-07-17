# Tűzvédelmi Eszközkezelő Rendszer
## Domainmodell fejlesztői specifikáció

**Dokumentum státusza:** elfogadott domainmodell  
**Nyelv:** magyar  
**Célközönség:** backend- és frontendfejlesztők, rendszertervezők, tesztelők  
**Technológiai kontextus:** ASP.NET Core Web API backend, Angular frontend

---

## 1. A dokumentum célja

Ez a dokumentum a Tűzvédelmi Eszközkezelő Rendszer elfogadott, magas szintű domainmodelljét írja le.

A dokumentum kizárólag az alábbiakat rögzíti:

- a rendszer fő domainentitásait;
- az entitások közötti kapcsolatokat;
- a felhasználók és ügyfélcégek közötti hozzárendelési modellt;
- a globális szerepkörök működését;
- az ügyféladatokhoz való hozzáférés alapelveit;
- a domain szintű kardinalitásokat és integritási szabályokat;
- a jogosultság-ellenőrzés magas szintű követelményeit.

Az egyes entitások részletes mezői, validációs szabályai, állapotgépei, API-végpontjai és adatbázis-migrációi nem képezik ennek a dokumentumnak a tárgyát. Ezek külön tervezési lépésben kerülnek meghatározásra.

---

## 2. Domainmodell áttekintése

A rendszer központi üzleti objektumai:

1. `ApplicationUser` – a rendszer felhasználója;
2. `Customer` – ügyfélcég;
3. `CustomerMembership` – egy felhasználó és egy ügyfélcég közötti hozzárendelés;
4. `Site` – egy ügyfélcég telephelye;
5. `Equipment` – egy telephelyen nyilvántartott tűzvédelmi vagy biztonsági eszköz;
6. `Maintenance` – egy eszközhöz tartozó karbantartási esemény;
7. `Issue` – egy eszközhöz tartozó hibabejelentés vagy hibaesemény;
8. `Document` – egy eszközhöz tartozó dokumentum, fénykép vagy egyéb fájl.

A modell központi hierarchiája:

```text
ApplicationUser
    └── CustomerMembership
            └── Customer
                    └── Site
                            └── Equipment
                                    ├── Maintenance
                                    ├── Issue
                                    └── Document
```

A `CustomerMembership` kapcsolóentitás biztosítja, hogy egy felhasználó több ügyfélcéghez is hozzáférhessen, illetve egy ügyfélcéghez több felhasználó is tartozhasson.

---

## 3. Fő domainentitások

### 3.1. ApplicationUser

Az `ApplicationUser` a rendszerben hitelesített természetes személyt reprezentálja.

Felelőssége:

- a felhasználó azonosítása;
- a felhasználó globális szerepkörének tárolása;
- az ügyfélcégekhez való hozzárendelések kapcsolódási pontjának biztosítása;
- az auditálható műveletek végrehajtójának reprezentálása.

Egy felhasználónak pontosan egy globális szerepköre lehet.

A szerepkör nem ügyfélcégenként változik. Ha például egy felhasználó karbantartó, akkor minden olyan ügyfélnél karbantartóként jár el, amelyhez aktív hozzárendeléssel rendelkezik.

### 3.2. Customer

A `Customer` egy ügyfélcéget reprezentál.

Felelőssége:

- a hozzá tartozó telephelyek összefogása;
- a hozzáféréssel rendelkező felhasználók kapcsolati célpontjának biztosítása;
- az ügyfélszintű adatelkülönítés logikai határának meghatározása.

Egy ügyfélcéghez:

- több felhasználó tartozhat;
- több telephely tartozhat.

A felhasználók nem közvetlen idegen kulccsal kapcsolódnak a `Customer` entitáshoz, hanem a `CustomerMembership` kapcsolóentitáson keresztül.

### 3.3. CustomerMembership

A `CustomerMembership` explicit kapcsolóentitás az `ApplicationUser` és a `Customer` között.

Egy rekord jelentése:

> Az adott felhasználó az adott ügyfélcéghez hozzá van rendelve, és a saját globális szerepkörének megfelelő műveleteket végezheti el annak adatai felett.

A kapcsolóentitás alkalmazása kötelező. Nem javasolt az Entity Framework Core implicit many-to-many kapcsolata, mert a kapcsolat önálló üzleti jelentéssel és életciklussal rendelkezik.

A kapcsolóentitás előnyei:

- auditálhatóvá teszi a hozzárendelést;
- lehetővé teszi a kapcsolat aktiválását és inaktiválását;
- tárolható a létrehozás ideje és létrehozója;
- később bővíthető időbeli vagy egyedi hozzáférési korlátozásokkal;
- lehetővé teszi a fizikai törlés helyett a hozzáférés visszavonását.

A kapcsolatnak legalább aktív és inaktív állapotot kell tudnia megkülönböztetni. A részletes státuszmodell később bővíthető például meghívott, felfüggesztett vagy eltávolított állapotokkal.

### 3.4. Site

A `Site` egy ügyfélcéghez tartozó telephelyet reprezentál.

Felelőssége:

- a helyhez kötött eszközök csoportosítása;
- az ügyfél szervezeti vagy földrajzi struktúrájának reprezentálása;
- az eszközlisták és kimutatások telephely szerinti szűrésének támogatása.

Minden telephely pontosan egy ügyfélcéghez tartozik.

Egy telephelyhez több eszköz tartozhat.

### 3.5. Equipment

Az `Equipment` egy konkrét, nyilvántartott tűzvédelmi vagy biztonsági eszközt reprezentál.

Minden eszköz pontosan egy telephelyhez tartozik.

Egy eszközhöz több:

- karbantartási esemény;
- hibabejelentés;
- dokumentum vagy fénykép

tartozhat.

Az eszköz az alkalmazás egyik központi aggregációs pontja. Az eszközhöz kapcsolódó események és fájlok életciklusa az eszköz kontextusában értelmezendő.

### 3.6. Maintenance

A `Maintenance` egy eszközhöz kapcsolódó karbantartási vagy ellenőrzési eseményt reprezentál.

Minden karbantartási esemény pontosan egy eszközhöz tartozik.

Egy eszköznek időben több karbantartási eseménye lehet, amelyek együtt alkotják az eszköz karbantartási előzményeit.

A karbantartási események részletes mezői, eredményei, munkalap- és jegyzőkönyvkapcsolatai külön specifikációban kerülnek meghatározásra.

### 3.7. Issue

Az `Issue` egy eszközhöz kötött hibabejelentést vagy hibával kapcsolatos eseményt reprezentál.

Minden hibabejelentés pontosan egy eszközhöz tartozik.

Egy eszközhöz több hibabejelentés kapcsolódhat. A hibabejelentések lehetnek egymástól függetlenek, és saját állapottal, leírással, időponttal, valamint csatolmányokkal rendelkezhetnek.

A részletes hibaállapotok és a lezárási folyamat később kerül meghatározásra.

### 3.8. Document

A `Document` egy eszközhöz kötött fájlt reprezentál.

Ide tartozhat például:

- fénykép;
- jegyzőkönyv;
- munkalap;
- tanúsítvány;
- egyéb kapcsolódó dokumentum.

A jelenlegi domainmodellben a dokumentumok elsődlegesen az `Equipment` entitáshoz kapcsolódnak.

A későbbi részletes tervezés során eldönthető, hogy szükséges-e a dokumentumokat közvetlenül `Maintenance` vagy `Issue` entitásokhoz is kapcsolni. Ez a jelenlegi magas szintű modellt nem módosítja, csak pontosítja.

---

## 4. Kapcsolatok és kardinalitások

### 4.1. Felhasználó és ügyfélcég

```text
ApplicationUser 1 ──── * CustomerMembership
Customer        1 ──── * CustomerMembership
```

Ebből következően:

```text
ApplicationUser * ──── * Customer
```

A több-a-többhöz kapcsolat explicit kapcsolóentitással valósul meg.

Szabályok:

- egy felhasználó nulla, egy vagy több ügyfélcéghez rendelhető;
- egy ügyfélcéghez nulla, egy vagy több felhasználó rendelhető;
- ugyanaz a felhasználó ugyanahhoz az ügyfélhez legfeljebb egyszer rendelhető hozzá;
- az inaktív hozzárendelés nem biztosít hozzáférést.

### 4.2. Ügyfélcég és telephely

```text
Customer 1 ──── * Site
```

Szabályok:

- minden telephely pontosan egy ügyfélcéghez tartozik;
- egy ügyfélcéghez több telephely tartozhat;
- telephely nem létezhet ügyfélcég nélkül.

### 4.3. Telephely és eszköz

```text
Site 1 ──── * Equipment
```

Szabályok:

- minden eszköz pontosan egy telephelyhez tartozik;
- egy telephelyhez több eszköz tartozhat;
- eszköz nem létezhet telephely nélkül.

### 4.4. Eszköz és karbantartás

```text
Equipment 1 ──── * Maintenance
```

Szabályok:

- minden karbantartási esemény pontosan egy eszközhöz tartozik;
- egy eszköznek több karbantartási eseménye lehet.

### 4.5. Eszköz és hibabejelentés

```text
Equipment 1 ──── * Issue
```

Szabályok:

- minden hibabejelentés pontosan egy eszközhöz tartozik;
- egy eszközhöz több hibabejelentés tartozhat.

### 4.6. Eszköz és dokumentum

```text
Equipment 1 ──── * Document
```

Szabályok:

- minden dokumentum legalább egy eszköz kontextusában értelmezett;
- egy eszközhöz több dokumentum vagy fénykép tartozhat.

---

## 5. Szerepkörmodell

A rendszerben egy felhasználónak pontosan egy globális szerepköre lehet.

Elfogadott szerepkörök:

```text
Admin
MaintenanceTechnician
CustomerUser
```

Magyar jelentésük:

| Technikai név | Üzleti jelentés |
|---|---|
| `Admin` | rendszergazda vagy teljes hozzáférésű adminisztrátor |
| `MaintenanceTechnician` | karbantartó |
| `CustomerUser` | ügyféloldali felhasználó |

A szerepkört az `ApplicationUser` tárolja. A szerepkör nem része a `CustomerMembership` entitásnak.

Ennek következménye:

- egy karbantartó minden hozzárendelt ügyfélnél karbantartói jogosultsággal rendelkezik;
- egy ügyféloldali felhasználó minden hozzárendelt ügyfélnél ügyféloldali jogosultsággal rendelkezik;
- egy felhasználó nem lehet az egyik ügyfélnél karbantartó, a másiknál ügyféloldali felhasználó;
- szerepkörváltás esetén a változás minden aktív ügyfél-hozzárendelésre érvényes.

---

## 6. Hozzáférési modell

A hozzáférés két egymástól elkülönülő kérdésből áll:

1. **Hozzáférhet-e a felhasználó az adott ügyfélhez?**
2. **Milyen műveleteket végezhet az ügyfél adatai felett?**

### 6.1. Ügyfélhez való hozzáférés

Nem admin felhasználó akkor férhet hozzá egy ügyfélhez, ha létezik aktív `CustomerMembership` rekord az adott felhasználó és ügyfél között.

Formális szabály:

```text
HasCustomerAccess(userId, customerId) =
    létezik aktív CustomerMembership,
    amelynek UserId = userId és CustomerId = customerId
```

### 6.2. Műveleti jogosultság

A műveleti jogosultságot a felhasználó globális szerepköre határozza meg.

Magas szintű jogosultsági modell:

| Szerepkör | Ügyfél-hozzárendelés szükséges | Hozzáférés jellege |
|---|---:|---|
| `Admin` | nem | teljes, globális hozzáférés |
| `MaintenanceTechnician` | igen | hozzárendelt ügyfelek eszközeinek és karbantartási adatainak kezelése |
| `CustomerUser` | igen | hozzárendelt ügyfelek adatainak megtekintése, hibabejelentés, dokumentumok elérése |

A részletes műveleti mátrix külön jogosultsági specifikációban kerül kidolgozásra.

### 6.3. Admin hozzáférés

Az `Admin` globális szerepkör.

Elfogadott szabály:

- az admin minden ügyfélhez hozzáfér;
- az adminnak nem szükséges `CustomerMembership` rekord;
- az admin minden ügyfél, telephely, eszköz és kapcsolódó adat felett teljes jogosultsággal rendelkezik.

### 6.4. Karbantartó hozzáférés

A `MaintenanceTechnician`:

- kizárólag az aktívan hozzárendelt ügyfeleket érheti el;
- az összes ilyen ügyfél valamennyi telephelyét láthatja;
- az összes ilyen telephely eszközeit kezelheti;
- az engedélyezett karbantartási és munkalap-műveleteket elvégezheti;
- több ügyfélhez is hozzárendelhető.

### 6.5. Ügyféloldali felhasználó hozzáférése

A `CustomerUser`:

- kizárólag az aktívan hozzárendelt ügyfeleket érheti el;
- az adott ügyfelek összes telephelyét és eszközét megtekintheti;
- hibabejelentést hozhat létre;
- az elérhető dokumentumokat letöltheti vagy megtekintheti;
- módosítási jogosultsága korlátozott, és a részletes jogosultsági mátrix szerint kerül meghatározásra.

---

## 7. Ügyfélszintű adatelkülönítés

A `Customer` a rendszer elsődleges adatelkülönítési határa.

Minden ügyfélhez kötött objektumnak egyértelműen visszavezethetőnek kell lennie pontosan egy ügyfélre:

```text
Site.CustomerId
Equipment → Site → Customer
Maintenance → Equipment → Site → Customer
Issue → Equipment → Site → Customer
Document → Equipment → Site → Customer
```

A hozzáférés-ellenőrzés során nem elegendő kizárólag a kliens által küldött `customerId` értéket vizsgálni. Az erőforrás tényleges tulajdonosi láncát is ellenőrizni kell.

Példa:

```text
PUT /api/customers/{customerId}/equipment/{equipmentId}
```

A backendnek ellenőriznie kell, hogy:

1. az `equipmentId` azonosítójú eszköz létezik;
2. az eszköz telephelye valóban a megadott `customerId` ügyfélhez tartozik;
3. a felhasználó hozzáférhet az adott ügyfélhez;
4. a felhasználó szerepköre engedélyezi a módosítást.

Ez megakadályozza a kliensoldali azonosítók manipulálásával történő jogosulatlan hozzáférést.

---

## 8. CustomerMembership életciklus

A hozzárendelés létrehozása adminisztrátori művelet.

Javasolt folyamat:

1. az admin kiválaszt egy meglévő felhasználót, vagy meghív egy új felhasználót;
2. kiválaszt egy vagy több ügyfélcéget;
3. a rendszer ügyfelenként létrehoz egy `CustomerMembership` rekordot;
4. aktív kapcsolat esetén a felhasználó hozzáfér az ügyfélhez;
5. hozzáférés visszavonásakor a kapcsolat inaktívvá válik;
6. a korábbi kapcsolat auditcélból megőrizhető.

A rendszernek meg kell akadályoznia ugyanazon aktív vagy logikailag azonos kapcsolat duplikált létrehozását.

---

## 9. Adatintegritási szabályok

### 9.1. Egyedi felhasználó–ügyfél kapcsolat

A `CustomerMembership` entitáson összetett egyedi megszorítás szükséges:

```text
UNIQUE(UserId, CustomerId)
```

Ez garantálja, hogy ugyanaz a felhasználó ugyanahhoz az ügyfélhez csak egy kapcsolati rekorddal tartozzon.

Amennyiben a rendszer később történeti, több időszakos tagságot kíván támogatni, az egyediség szabályát módosítani kell. A jelenlegi modellben egyetlen kapcsolat státusza változik.

### 9.2. Kötelező szülőkapcsolatok

Az alábbi kapcsolatok kötelezőek:

- `CustomerMembership.UserId`;
- `CustomerMembership.CustomerId`;
- `Site.CustomerId`;
- `Equipment.SiteId`;
- `Maintenance.EquipmentId`;
- `Issue.EquipmentId`;
- `Document.EquipmentId`.

Árva rekordok nem engedélyezettek.

### 9.3. Törlési viselkedés

A fizikai kaszkád törlés általános használata nem javasolt.

Ajánlott elv:

- üzleti jelentőségű rekordok esetén logikai törlés vagy archiválás;
- felhasználó–ügyfél kapcsolat esetén inaktiválás;
- ügyfél, telephely vagy eszköz törlése előtt kapcsolódó történeti adatok vizsgálata;
- karbantartások, hibák és dokumentumok megőrzése audit- és jogszabályi okokból.

A részletes törlési és archiválási szabályok külön specifikációban kerülnek meghatározásra.

---

## 10. Navigációs tulajdonságok – javasolt szerkezet

Az alábbi példa kizárólag a kapcsolatok szemléltetésére szolgál. A végleges mezők és típusok később kerülnek meghatározásra.

```csharp
public class ApplicationUser
{
    public Guid Id { get; set; }

    public UserRole Role { get; set; }

    public ICollection<CustomerMembership> CustomerMemberships { get; set; }
        = new List<CustomerMembership>();
}

public class Customer
{
    public Guid Id { get; set; }

    public ICollection<CustomerMembership> Memberships { get; set; }
        = new List<CustomerMembership>();

    public ICollection<Site> Sites { get; set; }
        = new List<Site>();
}

public class CustomerMembership
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}

public class Site
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public ICollection<Equipment> Equipment { get; set; }
        = new List<Equipment>();
}

public class Equipment
{
    public Guid Id { get; set; }

    public Guid SiteId { get; set; }
    public Site Site { get; set; } = null!;

    public ICollection<Maintenance> MaintenanceEvents { get; set; }
        = new List<Maintenance>();

    public ICollection<Issue> Issues { get; set; }
        = new List<Issue>();

    public ICollection<Document> Documents { get; set; }
        = new List<Document>();
}
```

---

## 11. Entity Framework Core kapcsolatkonfiguráció – irányelv

A több-a-többhöz kapcsolat explicit kapcsolóentitással konfigurálandó.

Példa:

```csharp
public sealed class CustomerMembershipConfiguration
    : IEntityTypeConfiguration<CustomerMembership>
{
    public void Configure(EntityTypeBuilder<CustomerMembership> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.UserId, x.CustomerId })
            .IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.CustomerMemberships)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Memberships)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

Az `OnDelete(DeleteBehavior.Restrict)` használata irányelvként javasolt annak elkerülésére, hogy egy felhasználó vagy ügyfél törlése történeti hozzárendeléseket automatikusan eltávolítson.

---

## 12. Backend jogosultság-ellenőrzési követelmények

A jogosultság-ellenőrzést minden esetben a backend végzi.

A frontend feladata kizárólag:

- a felhasználó számára nem engedélyezett funkciók elrejtése;
- az elérhető ügyfelek megjelenítése;
- az aktív ügyfél kiválasztásának támogatása;
- megfelelő felhasználói visszajelzés biztosítása.

A frontend korlátozásai nem tekinthetők biztonsági kontrollnak.

Javasolt backend ellenőrzési sorrend:

```text
1. Hitelesített-e a felhasználó?
2. Admin szerepkörű-e?
3. Ha nem admin, van-e aktív CustomerMembership?
4. Az erőforrás valóban a vizsgált ügyfélhez tartozik-e?
5. A szerepkör engedélyezi-e a kért műveletet?
```

A hozzáférés-ellenőrzést célszerű központi szolgáltatásba vagy authorization policy-kba szervezni, nem pedig minden controllerben egyedileg megismételni.

Lehetséges absztrakció:

```csharp
public interface ICustomerAccessService
{
    Task<bool> CanAccessCustomerAsync(
        Guid userId,
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task EnsureCanAccessCustomerAsync(
        Guid userId,
        Guid customerId,
        CancellationToken cancellationToken = default);
}
```

A részletes implementáció külön tervezési feladat.

---

## 13. Frontend működés több ügyfél esetén

Ha egy felhasználó több ügyfélhez tartozik, az Angular alkalmazásnak ügyfélkontextust kell kezelnie.

Javasolt működés:

- egy ügyfél esetén a rendszer automatikusan kiválaszthatja azt;
- több ügyfél esetén a felhasználó ügyfélválasztót kap;
- az aktív ügyfél megjelenik az alkalmazás fejlécében vagy navigációs területén;
- az ügyfélváltás újratölti az ügyfélhez kötött listákat és dashboard-adatokat;
- az ügyfélazonosító URL-paraméterként vagy route-szegmensként továbbítható az API felé.

Példa:

```http
GET /api/customers/{customerId}/equipment
```

Az aktív ügyfél kiválasztása kliensoldali kényelmi funkció, nem jogosultsági bizonyíték.

---

## 14. Aggregátumhatárok – jelenlegi irányelv

A részletes Domain-Driven Design aggregátumhatárok még nem véglegesek, de az elfogadott modell alapján az alábbi irány tekinthető kiindulópontnak:

### Customer aggregátumkörnyezet

```text
Customer
├── CustomerMembership
└── Site
```

### Equipment aggregátumkörnyezet

```text
Equipment
├── Maintenance
├── Issue
└── Document
```

A `Site` az ügyfélhez tartozik, az `Equipment` pedig a saját eseményeinek és dokumentumainak természetes központja.

A végleges aggregátumhatárokat az üzleti műveletek, tranzakciós követelmények és konzisztenciaszabályok részletes megtervezése után kell rögzíteni.

---

## 15. Kifejezetten elutasított alternatívák

### 15.1. CustomerId közvetlenül a User entitáson

Nem megfelelő, mert egy felhasználó több ügyfélhez is tartozhat.

### 15.2. Szerepkör tárolása a CustomerMembership entitáson

A jelenlegi követelmények szerint nem szükséges, mert egy felhasználónak pontosan egy, minden ügyfélnél azonos szerepköre van.

### 15.3. Implicit many-to-many kapcsolat

Nem javasolt, mert a felhasználó–ügyfél kapcsolat üzleti állapottal, auditadatokkal és későbbi bővítési igénnyel rendelkezik.

### 15.4. Telephelyenkénti felhasználói jogosultság

A jelenlegi modellben nincs rá szükség.

Elfogadott szabály:

> Ha egy felhasználó hozzáfér egy ügyfélhez, akkor az ügyfél összes telephelyéhez hozzáfér a szerepkörének megfelelő módon.

A telephelyszintű hozzárendelés későbbi bővítésként bevezethető, de nem része az első verziónak.

### 15.5. Ügyfelenként eltérő szerepkör

Nem támogatott.

Egy felhasználó nem lehet például az egyik ügyfélnél karbantartó, a másiknál ügyféloldali felhasználó.

---

## 16. Nyitott kérdések a következő tervezési fázishoz

A domainmodell kapcsolati struktúrája elfogadott. A következő fázisban az entitások részletes mezőit és szabályait kell megtervezni.

Különösen:

- `ApplicationUser` mezői és ASP.NET Core Identity integrációja;
- `Customer` cég- és kapcsolattartói adatai;
- `CustomerMembership` státuszai és auditmezői;
- `Site` cím- és kapcsolattartási adatai;
- `Equipment` típusa, azonosítója, QR-kódja és állapotmodellje;
- `Maintenance` munkafolyamata és eredményei;
- `Issue` állapotai és lezárása;
- `Document` fájltípusai, tárolása és kapcsolódási lehetőségei;
- logikai törlés és archiválás;
- auditmezők egységesítése;
- időzónák és dátumtípusok kezelése;
- részletes jogosultsági mátrix.

---

## 17. Elfogadott döntések összefoglalása

1. Egy `Customer` entitáshoz több `ApplicationUser` és több `Site` tartozhat.
2. Egy `ApplicationUser` több `Customer` entitáshoz is hozzárendelhető.
3. A felhasználó–ügyfél kapcsolat explicit `CustomerMembership` kapcsolóentitással valósul meg.
4. Egy felhasználónak pontosan egy globális szerepköre lehet.
5. A szerepkör az `ApplicationUser` entitáson található.
6. A szerepkör minden hozzárendelt ügyfélnél azonos.
7. Az `Admin` globálisan minden ügyfélhez hozzáfér, hozzárendelés nélkül.
8. A `MaintenanceTechnician` csak a hozzárendelt ügyfelekhez fér hozzá, de ott módosításokat végezhet.
9. A `CustomerUser` csak a hozzárendelt ügyfelekhez fér hozzá, korlátozott műveleti jogosultsággal.
10. Egy ügyfélhez való hozzáférés az ügyfél összes telephelyére kiterjed.
11. Egy `Site` több `Equipment` entitást tartalmazhat.
12. Egy `Equipment` több `Maintenance`, `Issue` és `Document` rekorddal rendelkezhet.
13. A `CustomerMembership` táblán egyedi `(UserId, CustomerId)` megszorítás szükséges.
14. A jogosultságokat minden API-kérésnél a backendnek kell ellenőriznie.
15. A frontend jogosultsági elrejtése kizárólag felhasználói felületi funkció, nem biztonsági kontroll.

---

## 18. Végleges domainmodell diagram

```text
┌─────────────────────┐
│   ApplicationUser   │
│                     │
│  pontosan 1 Role    │
└──────────┬──────────┘
           │ 1
           │
           │ *
┌──────────▼──────────┐
│ CustomerMembership  │
│                     │
│ UserId + CustomerId │
│ aktív / inaktív     │
└──────────┬──────────┘
           │ *
           │
           │ 1
┌──────────▼──────────┐
│      Customer       │
└──────────┬──────────┘
           │ 1
           │
           │ *
┌──────────▼──────────┐
│        Site         │
└──────────┬──────────┘
           │ 1
           │
           │ *
┌──────────▼──────────┐
│      Equipment      │
└───────┬───────┬─────┘
        │       │
        │       │
        ▼       ▼
 Maintenance  Issue
        │
        └──────────────► Document / Photo
```

A diagram a kapcsolatok logikai összefoglalása. A `Document` jelenlegi elsődleges kapcsolata az `Equipment`, a karbantartási és hibabejelentési közvetlen kapcsolatok lehetőségét a következő tervezési fázis pontosítja.

---

**Dokumentum vége**
