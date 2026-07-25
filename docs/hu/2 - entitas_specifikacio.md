# Tűzvédelmi Eszközkezelő Rendszer
## Entitás- és relációs modell fejlesztői specifikáció

**Dokumentum típusa:** fejlesztői adatmodell-specifikáció  
**Nyelv:** magyar  
**Formátum:** Markdown  
**Hatókör:** a rendszer elfogadott domainentitásai, mezőik, kapcsolataik, megszorításaik és fő üzleti szabályaik

---

# 1. Cél és hatókör

Ez a dokumentum a Tűzvédelmi Eszközkezelő Rendszer relációs adatmodelljének fejlesztői tervét rögzíti.

A dokumentum célja, hogy egységes alapot biztosítson:

- az adatbázisséma megtervezéséhez;
- az ASP.NET Core backend domain- és perzisztenciarétegének kialakításához;
- az API-erőforrások későbbi megtervezéséhez;
- a jogosultságkezeléshez;
- a karbantartási, hibakezelési és dokumentációs folyamatok implementálásához.

A specifikáció az alábbi entitásokat tartalmazza:

1. `User`
2. `CustomerCompany`
3. `CustomerMembership`
4. `Site`
5. `EquipmentType`
6. `Equipment`
7. `ServiceReport`
8. `Maintenance`
9. `MaintenanceIssue`
10. `Issue`
11. `WorkSheet`
12. `Document`
13. `Notification`

---

# 2. Általános modellezési alapelvek

## 2.1. Elsődleges kulcsok

Minden fő entitás saját, technikai elsődleges kulccsal rendelkezik.

| Tulajdonság | Javaslat |
|---|---|
| Kulcs neve | `Id` |
| Típus | UUID / GUID |
| Generálás | alkalmazás- vagy adatbázisoldalon |
| Láthatóság | belső technikai azonosító |

Az ember által használt azonosítók — például eszközazonosító, hibaszám, munkalapszám — külön mezőként szerepelnek.

## 2.2. Időpontok és dátumok

| Adattípus | Használat |
|---|---|
| Időpont időzóna-információval | létrehozás, módosítás, küldés, lezárás, bejelentkezés |
| Naptári dátum | ellenőrzési dátum, karbantartási nap, üzembe helyezés |
| Évszám | gyártási év |

## 2.3. Auditmezők

Az üzleti entitások többségén javasolt:

- `CreatedAt`
- `CreatedByUserId`
- `UpdatedAt`
- `UpdatedByUserId`

Állapotmegszüntetésnél vagy archiválásnál:

- `ArchivedAt`
- `ArchivedByUserId`
- `DeactivatedAt`
- `DeactivatedByUserId`
- `CancelledAt`
- `CancelledByUserId`

## 2.4. Fizikai törlés

A történeti vagy jogilag releváns rekordokat nem szabad fizikailag törölni.

Elsődlegesen archiválás, deaktiválás, érvénytelenítés vagy soft delete alkalmazandó az alábbiaknál:

- felhasználó;
- ügyfélcég;
- telephely;
- eszköz;
- karbantartás;
- hibabejelentés;
- munkalap;
- dokumentum;
- értesítési napló.

## 2.5. Konkurenciakezelés

A módosítható üzleti rekordoknál javasolt optimista konkurenciakezelés.

| Mező | Szerep |
|---|---|
| `RowVersion` | annak felismerése, hogy a rekordot más folyamat időközben módosította |

## 2.6. Jogosultsági modell

A jogosultság két szintből áll:

1. rendszerszintű jogosultság;
2. ügyfélcéghez kötött tagsági szerepkör.

A `User` opcionális rendszerszintű szerepkört kap. Az ügyfélcég-specifikus szerepkör a `CustomerMembership` rekordban található.

---

# 3. User

## 3.1. Üzleti jelentés

A `User` a rendszerbe belépő természetes személyt reprezentálja.

A felhasználó több ügyfélcéghez is kapcsolódhat, és ugyanazon felhasználó különböző ügyfélcégeknél eltérő szerepkört tölthet be.

## 3.2. Mezők

| Mező | Típus | Kötelező | Egyedi | Leírás |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | igen | igen | Elsődleges kulcs |
| `Email` | szöveg | igen | igen | Bejelentkezési és kapcsolattartási e-mail |
| `NormalizedEmail` | szöveg | igen | igen | Normalizált e-mail kereséshez |
| `FirstName` | szöveg | igen | nem | Keresztnév |
| `LastName` | szöveg | igen | nem | Vezetéknév |
| `PhoneNumber` | szöveg | nem | nem | Telefonszám |
| `SystemRole` | enum | igen | nem | Rendszerszintű szerepkör |
| `Status` | enum | igen | nem | Felhasználói fiók állapota |
| `EmailConfirmed` | logikai | igen | nem | Megerősítették-e az e-mail-címet |
| `LastLoginAt` | időpont | nem | nem | Utolsó sikeres bejelentkezés |
| `CreatedAt` | időpont | igen | nem | Létrehozás időpontja |
| `CreatedByUserId` | UUID / GUID | nem | nem | Létrehozó felhasználó |
| `UpdatedAt` | időpont | nem | nem | Utolsó módosítás |
| `UpdatedByUserId` | UUID / GUID | nem | nem | Utolsó módosító |
| `DeactivatedAt` | időpont | nem | nem | Deaktiválás időpontja |
| `DeactivatedByUserId` | UUID / GUID | nem | nem | Deaktiválást végző felhasználó |
| `RowVersion` | verzióérték | igen | nem | Optimista konkurenciakezelés |

## 3.3. SystemRole

| Érték | Jelentés |
|---|---|
| `None` | Nincs rendszerszintű kiemelt jogosultság |
| `Admin` | Teljes rendszerszintű hozzáférés |

## 3.4. UserStatus

| Érték | Jelentés |
|---|---|
| `Invited` | Meghívott, de még nem aktivált |
| `Active` | Aktív és bejelentkezhet |
| `Suspended` | Ideiglenesen felfüggesztett |
| `Deactivated` | Tartósan deaktivált |

## 3.5. Kapcsolatok

| Kapcsolódó entitás | Kardinalitás | Leírás |
|---|---:|---|
| `CustomerMembership` | 1:N | Egy felhasználó több ügyfélcéghez tartozhat |
| `Maintenance` | 1:N | Kijelölt vagy végrehajtó technikus |
| `ServiceReport` | 1:N | Elsődleges kijelölt technikus, létrehozó, módosító |
| `Issue` | 1:N | Bejelentő, felelős, megoldó vagy lezáró |
| `WorkSheet` | 1:N | Kiállító |
| `Document` | 1:N | Feltöltő vagy törlő |
| `Notification` | 1:N | Címzett vagy visszavonó |
| `User` | önhivatkozó 1:N | Auditkapcsolatok |

## 3.6. Megszorítások

- A normalizált e-mail rendszerszinten egyedi.
- Csak `Active` állapotú felhasználó jelentkezhet be.
- Az `Admin` rendszerszinten minden ügyfélcéget elér.
- Nem admin felhasználó csak aktív membership alapján fér hozzá egy ügyfélcéghez.
- A felhasználó deaktiválása nem törölhet történeti rekordokat.

---

# 4. CustomerCompany

## 4.1. Üzleti jelentés

A `CustomerCompany` a rendszerben kezelt ügyfélcéget, szervezetet vagy intézményt reprezentálja.

## 4.2. Mezők

| Mező | Típus | Kötelező | Egyedi | Leírás |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | igen | igen | Elsődleges kulcs |
| `Name` | szöveg | igen | nem | Rövid vagy közismert cégnév |
| `LegalName` | szöveg | nem | nem | Hivatalos cégnév |
| `CompanyRegistrationNumber` | szöveg | nem | javasolt | Cégjegyzékszám |
| `TaxNumber` | szöveg | nem | javasolt | Adószám |
| `Status` | enum | igen | nem | Ügyfélcég állapota |
| `BillingEmail` | szöveg | nem | nem | Számlázási e-mail |
| `PhoneNumber` | szöveg | nem | nem | Központi telefonszám |
| `WebsiteUrl` | szöveg | nem | nem | Honlap |
| `ContactName` | szöveg | nem | nem | Elsődleges kapcsolattartó |
| `ContactEmail` | szöveg | nem | nem | Kapcsolattartó e-mail |
| `ContactPhoneNumber` | szöveg | nem | nem | Kapcsolattartó telefonszám |
| `BillingCountryCode` | szöveg | nem | nem | Számlázási cím országkódja |
| `BillingPostalCode` | szöveg | nem | nem | Irányítószám |
| `BillingCity` | szöveg | nem | nem | Település |
| `BillingAddressLine` | szöveg | nem | nem | Utca, házszám, egyéb címadat |
| `Notes` | hosszú szöveg | nem | nem | Belső megjegyzés |
| `CreatedAt` | időpont | igen | nem | Létrehozás |
| `CreatedByUserId` | UUID / GUID | nem | nem | Létrehozó |
| `UpdatedAt` | időpont | nem | nem | Utolsó módosítás |
| `UpdatedByUserId` | UUID / GUID | nem | nem | Utolsó módosító |
| `ArchivedAt` | időpont | nem | nem | Archiválás |
| `ArchivedByUserId` | UUID / GUID | nem | nem | Archiváló |
| `RowVersion` | verzióérték | igen | nem | Konkurenciakezelés |

## 4.3. CustomerCompanyStatus

- `Active`
- `Suspended`
- `Archived`

## 4.4. Kapcsolatok

| Kapcsolódó entitás | Kardinalitás |
|---|---:|
| `CustomerMembership` | 1:N |
| `Site` | 1:N |
| `Notification` | 1:N |

## 4.5. Megszorítások

- A név kötelező.
- Az adószám megadás esetén egyedi.
- A cégjegyzékszám megadás esetén egyedi.
- Archivált ügyfélcéghez nem hozható létre új telephely vagy membership.
- Fizikai törlés helyett archiválás alkalmazandó.

---

# 5. CustomerMembership

## 5.1. Üzleti jelentés

A `CustomerMembership` a `User` és a `CustomerCompany` közötti kapcsolatot reprezentálja.

A rekord meghatározza, hogy a felhasználó melyik ügyfélcéghez, milyen szerepkörrel és milyen állapotban fér hozzá.

## 5.2. Mezők

| Mező | Típus | Kötelező | Egyedi | Leírás |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | igen | igen | Elsődleges kulcs |
| `UserId` | UUID / GUID | igen | összetetten | Felhasználó |
| `CustomerCompanyId` | UUID / GUID | igen | összetetten | Ügyfélcég |
| `Role` | enum | igen | nem | Cégspecifikus szerepkör |
| `Status` | enum | igen | nem | Membership állapota |
| `CreatedAt` | időpont | igen | nem | Létrehozás |
| `CreatedByUserId` | UUID / GUID | nem | nem | Létrehozó |
| `ActivatedAt` | időpont | nem | nem | Aktiválás |
| `ActivatedByUserId` | UUID / GUID | nem | nem | Aktiváló |
| `SuspendedAt` | időpont | nem | nem | Felfüggesztés |
| `SuspendedByUserId` | UUID / GUID | nem | nem | Felfüggesztő |
| `DeactivatedAt` | időpont | nem | nem | Deaktiválás |
| `DeactivatedByUserId` | UUID / GUID | nem | nem | Deaktiváló |
| `Note` | szöveg | nem | nem | Belső megjegyzés |
| `RowVersion` | verzióérték | igen | nem | Konkurenciakezelés |

## 5.3. CustomerMembershipRole

| Érték | Jogosultság |
|---|---|
| `Customer` | Megtekintés, hibabejelentés, dokumentumletöltés |
| `MaintenanceTechnician` | Eszközök, karbantartások, hibák és jelentések kezelése |
| `CompanyAdministrator` | Opcionális későbbi szerepkör az adott ügyfélcég adminisztrációjára |

## 5.4. CustomerMembershipStatus

- `Invited`
- `Active`
- `Suspended`
- `Deactivated`

## 5.5. Megszorítások

- `UserId + CustomerCompanyId` egyedi.
- Csak `Active` membership biztosít hozzáférést.
- A szerepkör kizárólag az adott ügyfélcégre érvényes.
- Egy deaktivált membership újraaktiválásakor a meglévő rekord módosítandó.

---

# 6. Site

## 6.1. Üzleti jelentés

A `Site` egy ügyfélcég konkrét fizikai telephelyét reprezentálja.

## 6.2. Mezők

| Mező | Típus | Kötelező | Egyedi | Leírás |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | igen | igen | Elsődleges kulcs |
| `CustomerCompanyId` | UUID / GUID | igen | nem | Tulajdonos ügyfélcég |
| `Name` | szöveg | igen | cégen belül javasolt | Telephely neve |
| `Code` | rövid szöveg | nem | cégen belül javasolt | Belső telephelykód |
| `Status` | enum | igen | nem | Telephely állapota |
| `CountryCode` | rövid szöveg | igen | nem | Országkód |
| `PostalCode` | rövid szöveg | igen | nem | Irányítószám |
| `City` | szöveg | igen | nem | Település |
| `AddressLine` | szöveg | igen | nem | Utca, házszám, épület |
| `Latitude` | decimális szám | nem | nem | Földrajzi szélesség |
| `Longitude` | decimális szám | nem | nem | Földrajzi hosszúság |
| `ContactName` | szöveg | nem | nem | Telephelyi kapcsolattartó |
| `ContactEmail` | szöveg | nem | nem | Kapcsolattartó e-mail |
| `ContactPhoneNumber` | szöveg | nem | nem | Kapcsolattartó telefonszám |
| `Notes` | hosszú szöveg | nem | nem | Belső megjegyzés |
| audit- és archiválási mezők | vegyes | igen/részben | nem | Létrehozás, módosítás, archiválás |
| `RowVersion` | verzióérték | igen | nem | Konkurenciakezelés |

## 6.3. SiteStatus

- `Active`
- `Inactive`
- `Archived`

## 6.4. Kapcsolatok

| Kapcsolódó entitás | Kardinalitás |
|---|---:|
| `CustomerCompany` | N:1 |
| `Equipment` | 1:N |
| `ServiceReport` | 1:N |

## 6.5. Megszorítások

- Minden telephely pontosan egy ügyfélcéghez tartozik.
- `CustomerCompanyId + Code` megadás esetén egyedi.
- Archivált telephelyhez új eszköz vagy ServiceReport nem hozható létre.
- Eszközzel rendelkező telephely fizikailag nem törölhető.

---

# 7. EquipmentType

## 7.1. Üzleti jelentés

Az `EquipmentType` az eszközök kategóriáját és az alapértelmezett ellenőrzési szabályokat reprezentálja.

## 7.2. Mezők

| Mező | Típus | Kötelező | Egyedi | Leírás |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | igen | igen | Elsődleges kulcs |
| `Name` | szöveg | igen | igen | Típus neve |
| `Code` | rövid szöveg | igen | igen | Stabil típusazonosító |
| `Description` | hosszú szöveg | nem | nem | Típus leírása |
| `DefaultInspectionIntervalMonths` | egész szám | nem | nem | Alapértelmezett periódus |
| `RequiresSerialNumber` | logikai | igen | nem | Gyári szám kötelező-e |
| `RequiresManufacturingYear` | logikai | igen | nem | Gyártási év kötelező-e |
| `RequiresCommissioningDate` | logikai | igen | nem | Üzembe helyezési dátum kötelező-e |
| `IsActive` | logikai | igen | nem | Választható-e új eszköznél |
| auditmezők | vegyes | igen/részben | nem | Létrehozás és módosítás |
| `RowVersion` | verzióérték | igen | nem | Konkurenciakezelés |

## 7.3. Kapcsolatok

| Kapcsolódó entitás | Kardinalitás |
|---|---:|
| `Equipment` | 1:N |

## 7.4. Megszorítások

- A név és a kód rendszerszinten egyedi.
- Az ellenőrzési intervallum pozitív egész.
- Használatban lévő típus nem törölhető.
- Inaktív típus új eszközhöz nem rendelhető.

---

# 8. Equipment

## 8.1. Üzleti jelentés

Az `Equipment` egy konkrét fizikai tűzvédelmi vagy biztonsági eszközt reprezentál.

## 8.2. Mezők

| Mező | Típus | Kötelező | Egyedi | Leírás |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | igen | igen | Belső kulcs |
| `SiteId` | UUID / GUID | igen | nem | Telephely |
| `EquipmentTypeId` | UUID / GUID | igen | nem | Eszköztípus |
| `AssetIdentifier` | rövid szöveg | igen | igen | Ember által olvasható eszközazonosító |
| `PublicToken` | UUID vagy biztonságos token | igen | igen | QR-kódos publikus azonosító |
| `SerialNumber` | szöveg | feltételes | nem | Gyári szám |
| `Manufacturer` | szöveg | nem | nem | Gyártó |
| `Model` | szöveg | nem | nem | Modell |
| `ManufacturingYear` | évszám | feltételes | nem | Gyártási év |
| `CommissionedOn` | dátum | feltételes | nem | Üzembe helyezés |
| `Building` | szöveg | nem | nem | Épület |
| `Floor` | szöveg | nem | nem | Szint |
| `Room` | szöveg | nem | nem | Helyiség |
| `ExactLocation` | szöveg | nem | nem | Pontos hely |
| `InspectionIntervalMonths` | egész szám | nem | nem | Egyedi ellenőrzési periódus |
| `LastInspectionDate` | dátum | nem | nem | Utolsó ellenőrzés |
| `NextInspectionDueDate` | dátum | nem | nem | Következő esedékesség |
| `LifecycleStatus` | enum | igen | nem | Tárolt életciklus-státusz |
| `OutOfServiceReason` | hosszú szöveg | nem | nem | Üzemen kívül helyezés oka |
| `DecommissionedOn` | dátum | nem | nem | Végleges kivonás dátuma |
| `Notes` | hosszú szöveg | nem | nem | Megjegyzés |
| audit- és archiválási mezők | vegyes | igen/részben | nem | Létrehozás, módosítás, archiválás |
| `RowVersion` | verzióérték | igen | nem | Konkurenciakezelés |

## 8.3. EquipmentLifecycleStatus

- `Active`
- `OutOfService`
- `Decommissioned`
- `Archived`

## 8.4. Számított működési státusz

| Státusz | Feltétel |
|---|---|
| `Ok` | Aktív, nincs jelentős nyitott hiba, nem közeli az esedékesség |
| `Warning` | 30 napon belüli esedékesség vagy nem kritikus nyitott hiba |
| `Expired` | Esedékesség elmúlt |
| `Faulty` | Magas vagy kritikus aktív hiba |
| `OutOfService` | Tárolt életciklus-státusz alapján |
| `Decommissioned` | Véglegesen kivont |

## 8.5. Kapcsolatok

| Kapcsolódó entitás | Kardinalitás |
|---|---:|
| `Site` | N:1 |
| `EquipmentType` | N:1 |
| `Maintenance` | 1:N |
| `Issue` | 1:N |
| `Document` | 1:N |
| `Notification` | 1:N |

## 8.6. Megszorítások

- `AssetIdentifier` rendszerszinten egyedi.
- `PublicToken` rendszerszinten egyedi.
- A gyártási év nem lehet jövőbeli.
- A következő ellenőrzési dátum nem lehet korábbi az utolsónál.
- Selejtezett eszközhöz új normál karbantartás vagy hiba nem indítható.
- Telephelyváltás külön auditált üzleti művelet.

---

# 9. ServiceReport

## 9.1. Üzleti jelentés

A `ServiceReport` egy adott telephelyhez tartozó teljes karbantartási vagy szervizalkalmat fog össze.

Egy ServiceReport több eszközön végzett `Maintenance` rekordot tartalmazhat.

## 9.2. Mezők

| Mező | Típus | Kötelező | Egyedi | Leírás |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | igen | igen | Elsődleges kulcs |
| `ReportNumber` | rövid szöveg | igen | igen | Jelentés- vagy munkavégzési azonosító |
| `SiteId` | UUID / GUID | igen | nem | Érintett telephely |
| `Status` | enum | igen | nem | Folyamatállapot |
| `ScheduledStartAt` | időpont | nem | nem | Tervezett kezdés |
| `ScheduledEndAt` | időpont | nem | nem | Tervezett befejezés |
| `StartedAt` | időpont | nem | nem | Tényleges kezdés |
| `CompletedAt` | időpont | nem | nem | Tényleges befejezés |
| `PrimaryTechnicianUserId` | UUID / GUID | nem | nem | Elsődleges technikus |
| `Description` | hosszú szöveg | nem | nem | Munkavégzés célja |
| `CustomerRepresentativeName` | szöveg | nem | nem | Ügyfél helyszíni képviselője |
| `CustomerRepresentativeTitle` | szöveg | nem | nem | Beosztás vagy szerep |
| `GeneralFindings` | hosszú szöveg | nem | nem | Általános megállapítások |
| `Notes` | hosszú szöveg | nem | nem | Belső megjegyzések |
| `CancellationReason` | hosszú szöveg | törléskor | nem | Törlés indoka |
| audit- és törlési mezők | vegyes | igen/részben | nem | Létrehozás, módosítás, törlés |
| `RowVersion` | verzióérték | igen | nem | Konkurenciakezelés |

## 9.3. ServiceReportStatus

- `Planned`
- `InProgress`
- `Completed`
- `Cancelled`

## 9.4. Kapcsolatok

| Kapcsolódó entitás | Kardinalitás |
|---|---:|
| `Site` | N:1 |
| `Maintenance` | 1:N |
| `WorkSheet` | 1:N |
| `Document` | 1:N |
| `User` mint elsődleges technikus | N:1 opcionális |

## 9.5. Megszorítások

- Egy ServiceReport pontosan egy telephelyhez tartozik.
- Minden kapcsolódó Maintenance eszközének ugyanahhoz a telephelyhez kell tartoznia.
- `Completed` állapotba csak akkor kerülhet, ha a kötelező Maintenance tételek lezártak.
- `Cancelled` állapotnál az indoklás kötelező.
- Lezárt rekord normál felhasználó számára nem módosítható szabadon.

---

# 10. Maintenance

## 10.1. Üzleti jelentés

A `Maintenance` egy konkrét eszközön végzett ellenőrzést, javítást, karbantartást vagy más műszaki műveletet reprezentál.

## 10.2. Mezők

| Mező | Típus | Kötelező | Egyedi | Leírás |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | igen | igen | Elsődleges kulcs |
| `ServiceReportId` | UUID / GUID | igen | nem | Összefogó ServiceReport |
| `EquipmentId` | UUID / GUID | igen | nem | Érintett eszköz |
| `Type` | enum | igen | nem | Karbantartás típusa |
| `Status` | enum | igen | nem | Folyamatállapot |
| `ScheduledDate` | dátum | nem | nem | Tervezett dátum |
| `StartedAt` | időpont | nem | nem | Kezdés |
| `PerformedDate` | dátum | lezáráskor | nem | Tényleges végrehajtás |
| `AssignedTechnicianUserId` | UUID / GUID | nem | nem | Kijelölt technikus |
| `PerformedByUserId` | UUID / GUID | lezáráskor | nem | Végrehajtó |
| `Result` | enum | lezáráskor | nem | Eredmény |
| `WorkDescription` | hosszú szöveg | nem | nem | Elvégzett munka |
| `Findings` | hosszú szöveg | nem | nem | Megállapítások |
| `Recommendation` | hosszú szöveg | nem | nem | Javasolt további teendő |
| `NextInspectionDueDate` | dátum | nem | nem | Következő esedékesség |
| `CancellationReason` | hosszú szöveg | törléskor | nem | Megszakítás indoka |
| `CompletedAt` | időpont | lezáráskor | nem | Lezárás időpontja |
| `CompletedByUserId` | UUID / GUID | lezáráskor | nem | Lezáró |
| auditmezők | vegyes | igen/részben | nem | Létrehozás és módosítás |
| `RowVersion` | verzióérték | igen | nem | Konkurenciakezelés |

## 10.3. MaintenanceType

- `Inspection`
- `PreventiveMaintenance`
- `Repair`
- `Replacement`
- `Commissioning`
- `Decommissioning`

## 10.4. MaintenanceStatus

- `Planned`
- `InProgress`
- `Completed`
- `Cancelled`

## 10.5. MaintenanceResult

- `Passed`
- `PassedWithRemarks`
- `Failed`
- `RepairRequired`
- `ReplacementRequired`

## 10.6. Kapcsolatok

| Kapcsolódó entitás | Kardinalitás |
|---|---:|
| `ServiceReport` | N:1 |
| `Equipment` | N:1 |
| `User` mint kijelölt technikus | N:1 opcionális |
| `User` mint végrehajtó | N:1 opcionális |
| `Document` | 1:N |
| `Issue` | N:M a `MaintenanceIssue` kapcsolaton keresztül |

## 10.7. Megszorítások

- Csak az eszköz ügyfélcégéhez megfelelő membershippel rendelkező technikus jelölhető ki.
- `Completed` állapotnál az eredmény, végrehajtó és tényleges dátum kötelező.
- Sikertelen eredményhez hiba kapcsolható vagy új hiba hozható létre.
- Lezáráskor az eszköz ellenőrzési dátumai tranzakcióban frissíthetők.
- Lezárt rekord szabad módosítása nem megengedett.

---

# 11. MaintenanceIssue

## 11.1. Üzleti jelentés

A `MaintenanceIssue` kapcsolótábla a Maintenance és Issue közötti N:M kapcsolatot kezeli.

## 11.2. Mezők

| Mező | Típus | Kötelező | Egyedi | Leírás |
|---|---|---:|---:|---|
| `MaintenanceId` | UUID / GUID | igen | összetetten | Karbantartás |
| `IssueId` | UUID / GUID | igen | összetetten | Hiba |
| `RelationType` | enum | igen | nem | Kapcsolat jelentése |
| `CreatedAt` | időpont | igen | nem | Kapcsolat létrehozása |
| `CreatedByUserId` | UUID / GUID | nem | nem | Létrehozó |

## 11.3. RelationType

- `Detected`
- `Resolved`
- `Related`

## 11.4. Megszorítások

- `MaintenanceId + IssueId + RelationType` egyedi lehet.
- A két rekordnak ugyanahhoz az eszközhöz kell kapcsolódnia.
- A kapcsolat létrehozása jogosultság-ellenőrzéshez kötött.

---

# 12. Issue

## 12.1. Üzleti jelentés

Az `Issue` egy eszközhöz kapcsolódó hibát, sérülést vagy működési rendellenességet reprezentál.

## 12.2. Mezők

| Mező | Típus | Kötelező | Egyedi | Leírás |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | igen | igen | Elsődleges kulcs |
| `EquipmentId` | UUID / GUID | igen | nem | Érintett eszköz |
| `IssueNumber` | rövid szöveg | igen | igen | Ember által olvasható hibaazonosító |
| `Title` | szöveg | igen | nem | Rövid összefoglaló |
| `Description` | hosszú szöveg | igen | nem | Részletes leírás |
| `Severity` | enum | igen | nem | Súlyosság |
| `Status` | enum | igen | nem | Folyamatállapot |
| `ReportedAt` | időpont | igen | nem | Bejelentés ideje |
| `ReportedByUserId` | UUID / GUID | igen | nem | Bejelentő |
| `AssignedToUserId` | UUID / GUID | nem | nem | Felelős |
| `AcknowledgedAt` | időpont | nem | nem | Tudomásulvétel |
| `AcknowledgedByUserId` | UUID / GUID | nem | nem | Tudomásul vevő |
| `Resolution` | hosszú szöveg | megoldáskor | nem | Megoldás leírása |
| `ResolvedAt` | időpont | nem | nem | Megoldás ideje |
| `ResolvedByUserId` | UUID / GUID | nem | nem | Megoldó |
| `ClosedAt` | időpont | nem | nem | Végleges lezárás |
| `ClosedByUserId` | UUID / GUID | nem | nem | Lezáró |
| `RejectionReason` | hosszú szöveg | elutasításkor | nem | Elutasítás indoka |
| auditmezők | vegyes | igen/részben | nem | Létrehozás, módosítás |
| `RowVersion` | verzióérték | igen | nem | Konkurenciakezelés |

## 12.3. IssueSeverity

- `Low`
- `Medium`
- `High`
- `Critical`

## 12.4. IssueStatus

- `Open`
- `Acknowledged`
- `InProgress`
- `Resolved`
- `Closed`
- `Rejected`

## 12.5. Kapcsolatok

| Kapcsolódó entitás | Kardinalitás |
|---|---:|
| `Equipment` | N:1 |
| `User` mint bejelentő | N:1 |
| `User` mint felelős | N:1 opcionális |
| `Document` | 1:N |
| `Maintenance` | N:M |
| `Notification` | 1:N |

## 12.6. Megszorítások

- Cím és leírás kötelező.
- A bejelentőnek hozzá kell férnie az eszköz ügyfélcégéhez.
- `Resolved` állapotnál a megoldás leírása kötelező.
- `Rejected` állapotnál indoklás kötelező.
- `Closed` állapot csak `Resolved` állapotból érhető el.
- Magas vagy kritikus aktív hiba az eszköz számított státuszát `Faulty` értékre módosítja.

---

# 13. WorkSheet

## 13.1. Üzleti jelentés

A `WorkSheet` a karbantartó cég által a megrendelő részére kiállított hivatalos igazolás, amely tanúsítja, hogy a munka elvégzésre került.

A WorkSheet nem azonos:

- a ServiceReport folyamatrekorddal;
- a Maintenance technikai tétellel;
- a PDF-fájllal.

A WorkSheet az igazolás üzleti és jogi reprezentációja. A hozzá tartozó végleges PDF a `Document` rendszerben tárolódik.

## 13.2. Mezők

| Mező | Típus | Kötelező | Egyedi | Leírás |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | igen | igen | Elsődleges kulcs |
| `WorkSheetNumber` | rövid szöveg | igen | igen | Hivatalos munkalapszám |
| `ServiceReportId` | UUID / GUID | igen | nem | Alapul szolgáló munkavégzés |
| `Status` | enum | igen | nem | Kiállítási állapot |
| `IssuedAt` | időpont | kiállításkor | nem | Kiállítás időpontja |
| `IssuedByUserId` | UUID / GUID | kiállításkor | nem | Kiállító felhasználó |
| `IssuerCompanyName` | szöveg | igen | nem | Karbantartó cég neve a kiállításkor |
| `IssuerCompanyTaxNumber` | szöveg | nem | nem | Karbantartó cég adószáma |
| `IssuerCompanyAddress` | szöveg | nem | nem | Karbantartó cég címe |
| `CustomerCompanyName` | szöveg | igen | nem | Megrendelő neve a kiállításkor |
| `CustomerCompanyTaxNumber` | szöveg | nem | nem | Megrendelő adószáma |
| `CustomerCompanyAddress` | szöveg | nem | nem | Megrendelő címe |
| `SiteName` | szöveg | igen | nem | Telephely neve a kiállításkor |
| `SiteAddress` | szöveg | igen | nem | Telephely címe a kiállításkor |
| `WorkStartedAt` | időpont | nem | nem | Munka tényleges kezdése |
| `WorkCompletedAt` | időpont | igen | nem | Munka tényleges befejezése |
| `WorkSummary` | hosszú szöveg | igen | nem | Elvégzett munka összefoglalása |
| `GeneralFindings` | hosszú szöveg | nem | nem | Általános megállapítások |
| `CustomerRepresentativeName` | szöveg | nem | nem | Átvevő vagy képviselő neve |
| `CustomerRepresentativeTitle` | szöveg | nem | nem | Beosztás |
| `AcceptedAt` | időpont | nem | nem | Átvétel vagy elfogadás |
| `VersionNumber` | egész szám | igen | összetetten | Munkalap verziója |
| `ReplacesWorkSheetId` | UUID / GUID | nem | nem | Korábbi munkalap, amelyet helyettesít |
| `GeneratedDocumentId` | UUID / GUID | kiállítás után | igen | Végleges PDF |
| `CreatedAt` | időpont | igen | nem | Rekord létrehozása |
| `CreatedByUserId` | UUID / GUID | igen | nem | Létrehozó |
| `RowVersion` | verzióérték | igen | nem | Konkurenciakezelés |

## 13.3. WorkSheetStatus

- `Draft`
- `Issued`
- `Accepted`
- `Superseded`
- `Cancelled`

## 13.4. Kapcsolatok

| Kapcsolódó entitás | Kardinalitás |
|---|---:|
| `ServiceReport` | N:1 |
| `Document` | 1:N |
| `User` mint kiállító | N:1 |
| `WorkSheet` önhivatkozás | N:1 opcionális |

## 13.5. Megszorítások

- Csak `Completed` ServiceReport alapján állítható ki.
- A munkalapszám egyedi és kiállítás után nem módosítható.
- A kiállított munkalap normál szerkesztéssel nem módosítható.
- Korrekciókor új verzió vagy helyesbítő WorkSheet készül.
- Korábbi változat nem írható felül.
- A kiállításkori cég- és telephelyadatokat snapshotként meg kell őrizni.
- Egy ServiceReporthoz több WorkSheet tartozhat verziózás miatt.
- Egy időben csak egy aktuális, érvényes kiállított változat lehet.

---

# 14. Document

## 14.1. Üzleti jelentés

A `Document` fájlok metaadatait reprezentálja.

A tényleges bináris fájl objektumtárolóban vagy fájlrendszerben található. Az adatbázis csak a metaadatokat és az üzleti kapcsolatokat tárolja.

## 14.2. Mezők

| Mező | Típus | Kötelező | Egyedi | Leírás |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | igen | igen | Elsődleges kulcs |
| `Type` | enum | igen | nem | Dokumentumtípus |
| `OriginalFileName` | szöveg | igen | nem | Eredeti fájlnév |
| `StorageKey` | szöveg | igen | igen | Tárolóbeli kulcs |
| `ContentType` | szöveg | igen | nem | MIME-típus |
| `FileExtension` | rövid szöveg | igen | nem | Kiterjesztés |
| `SizeInBytes` | egész szám | igen | nem | Méret |
| `Sha256Hash` | szöveg | nem | nem | Integritási hash |
| `Title` | szöveg | nem | nem | Megjelenített cím |
| `Description` | hosszú szöveg | nem | nem | Leírás |
| `EquipmentId` | UUID / GUID | feltételes | nem | Eszközkapcsolat |
| `ServiceReportId` | UUID / GUID | feltételes | nem | Jelentéskapcsolat |
| `MaintenanceId` | UUID / GUID | feltételes | nem | Karbantartáskapcsolat |
| `IssueId` | UUID / GUID | feltételes | nem | Hibakapcsolat |
| `WorkSheetId` | UUID / GUID | feltételes | nem | Munkalapkapcsolat |
| `GeneratedBySystem` | logikai | igen | nem | Rendszer generálta-e |
| `DocumentVersion` | egész szám | nem | nem | Dokumentumverzió |
| `UploadedAt` | időpont | igen | nem | Feltöltés vagy generálás |
| `UploadedByUserId` | UUID / GUID | nem | nem | Feltöltő |
| `DeletedAt` | időpont | nem | nem | Soft delete |
| `DeletedByUserId` | UUID / GUID | nem | nem | Törlő |
| `RowVersion` | verzióérték | igen | nem | Konkurenciakezelés |

## 14.3. DocumentType

- `EquipmentPhoto`
- `ServiceReportPhoto`
- `MaintenancePhoto`
- `IssuePhoto`
- `WorkSheetPdf`
- `WorkSheetAttachment`
- `CustomerSignature`
- `InspectionReport`
- `Certificate`
- `ManufacturerDocument`
- `Other`

## 14.4. Kapcsolatok

A dokumentum pontosan egy közvetlen üzleti szülőhöz tartozzon:

- `Equipment`
- `ServiceReport`
- `Maintenance`
- `Issue`
- `WorkSheet`

## 14.5. Megszorítások

- Pontosan egy üzleti szülőkapcsolat kötelező.
- `StorageKey` egyedi.
- A fájlméret pozitív és konfigurált korlát alatti.
- Csak engedélyezett MIME-típus tölthető fel.
- Az eredeti fájlnév nem használható tárolási kulcsként.
- Letöltés előtt mindig jogosultság-ellenőrzés szükséges.
- Kiállított munkalap PDF-je fizikailag nem törölhető normál művelettel.

---

# 15. Notification

## 15.1. Üzleti jelentés

A `Notification` egy konkrét, elküldendő vagy már elküldött rendszerértesítést reprezentál.

A rekord küldési naplóként és újrapróbálkozási egységként is működik.

## 15.2. Mezők

| Mező | Típus | Kötelező | Egyedi | Leírás |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | igen | igen | Elsődleges kulcs |
| `Type` | enum | igen | nem | Értesítéstípus |
| `Channel` | enum | igen | nem | Küldési csatorna |
| `Status` | enum | igen | nem | Küldési állapot |
| `RecipientUserId` | UUID / GUID | nem | nem | Belső címzett |
| `RecipientEmail` | szöveg | igen | nem | Küldéskori e-mail |
| `CustomerCompanyId` | UUID / GUID | igen | nem | Ügyfélcéges hatókör |
| `EquipmentId` | UUID / GUID | nem | nem | Kapcsolódó eszköz |
| `ServiceReportId` | UUID / GUID | nem | nem | Kapcsolódó jelentés |
| `MaintenanceId` | UUID / GUID | nem | nem | Kapcsolódó karbantartás |
| `IssueId` | UUID / GUID | nem | nem | Kapcsolódó hiba |
| `WorkSheetId` | UUID / GUID | nem | nem | Kapcsolódó munkalap |
| `Subject` | szöveg | igen | nem | Tárgy |
| `Body` | hosszú szöveg | igen | nem | Tartalom |
| `ScheduledAt` | időpont | igen | nem | Tervezett küldés |
| `SentAt` | időpont | nem | nem | Sikeres küldés |
| `FailedAt` | időpont | nem | nem | Sikertelen küldés |
| `AttemptCount` | egész szám | igen | nem | Próbálkozások száma |
| `NextAttemptAt` | időpont | nem | nem | Következő próbálkozás |
| `LastError` | hosszú szöveg | nem | nem | Legutóbbi hiba |
| `ExternalMessageId` | szöveg | nem | nem | Külső szolgáltatói azonosító |
| `DeduplicationKey` | szöveg | igen | igen | Duplikáció elleni kulcs |
| `CreatedAt` | időpont | igen | nem | Rekord létrehozása |
| `CancelledAt` | időpont | nem | nem | Visszavonás |
| `CancelledByUserId` | UUID / GUID | nem | nem | Visszavonó |
| `RowVersion` | verzióérték | igen | nem | Konkurenciakezelés |

## 15.3. NotificationType

- `InspectionDueIn30Days`
- `InspectionDueIn14Days`
- `InspectionDueIn7Days`
- `InspectionDueToday`
- `InspectionExpired`
- `IssueReported`
- `IssueAssigned`
- `IssueResolved`
- `MaintenanceAssigned`
- `MaintenanceCompleted`
- `ServiceReportCompleted`
- `WorkSheetIssued`
- `UserInvitation`
- `Custom`

## 15.4. NotificationChannel

- `Email`
- `InApp`
- `Sms`

Az első verzióban elegendő az `Email`.

## 15.5. NotificationStatus

- `Pending`
- `Processing`
- `Sent`
- `Failed`
- `Cancelled`
- `DeadLettered`

## 15.6. Kapcsolatok

| Kapcsolódó entitás | Kardinalitás |
|---|---:|
| `User` mint címzett | N:1 opcionális |
| `CustomerCompany` | N:1 |
| `Equipment` | N:1 opcionális |
| `ServiceReport` | N:1 opcionális |
| `Maintenance` | N:1 opcionális |
| `Issue` | N:1 opcionális |
| `WorkSheet` | N:1 opcionális |

## 15.7. Megszorítások

- `RecipientEmail` kötelező.
- `DeduplicationKey` egyedi.
- `SentAt` csak `Sent` állapotnál tölthető.
- Sikertelen küldésnél `LastError` tárolandó.
- A próbálkozásszám nem lehet negatív.
- Az elküldött tárgy és törzs változatlan naplóként megőrzendő.
- Az értesítési rekordokat normál működésben nem szabad fizikailag törölni.

---

# 16. Összesített relációs modell

```text
User
 ├── CustomerMembership ── CustomerCompany
 ├── Maintenance
 ├── Issue
 ├── ServiceReport
 ├── WorkSheet
 ├── Document
 └── Notification

CustomerCompany
 └── Site
      ├── Equipment
      │    ├── Maintenance
      │    │    ├── Document
      │    │    └── MaintenanceIssue ── Issue
      │    ├── Issue
      │    │    └── Document
      │    ├── Document
      │    └── Notification
      │
      └── ServiceReport
           ├── Maintenance
           ├── Document
           └── WorkSheet
                ├── Document
                └── WorkSheet verziólánc

EquipmentType
 └── Equipment
```

---

# 17. Kardinalitások összefoglalása

| Forrás | Kapcsolat | Cél |
|---|---:|---|
| `User` | 1:N | `CustomerMembership` |
| `CustomerCompany` | 1:N | `CustomerMembership` |
| `CustomerCompany` | 1:N | `Site` |
| `Site` | 1:N | `Equipment` |
| `EquipmentType` | 1:N | `Equipment` |
| `Site` | 1:N | `ServiceReport` |
| `ServiceReport` | 1:N | `Maintenance` |
| `Equipment` | 1:N | `Maintenance` |
| `Equipment` | 1:N | `Issue` |
| `Maintenance` | N:M | `Issue` |
| `ServiceReport` | 1:N | `WorkSheet` |
| `WorkSheet` | 1:N | `Document` |
| `Equipment` | 1:N | `Document` |
| `ServiceReport` | 1:N | `Document` |
| `Maintenance` | 1:N | `Document` |
| `Issue` | 1:N | `Document` |
| `CustomerCompany` | 1:N | `Notification` |

---

# 18. Fő üzleti invariánsok

1. Egy `User` több ügyfélcéghez tartozhat.
2. Egy user szerepköre ügyfélcégenként eltérhet.
3. Az ügyfélcég-specifikus szerepkör a `CustomerMembership` rekordban található.
4. A rendszerszintű adminjog a `User.SystemRole` mezőben található.
5. Egy telephely pontosan egy ügyfélcéghez tartozik.
6. Egy eszköz pontosan egy telephelyhez és egy eszköztípushoz tartozik.
7. Egy ServiceReport pontosan egy telephelyhez tartozik.
8. Egy ServiceReport több Maintenance tételt foghat össze.
9. A ServiceReport Maintenance rekordjainak eszközei ugyanazon telephelyhez tartoznak.
10. A WorkSheet csak lezárt ServiceReport alapján állítható ki.
11. A WorkSheet a hivatalos teljesítésigazolás üzleti rekordja.
12. A WorkSheet PDF-je a Document rendszerben tárolódik.
13. A kiállított munkalap történeti adatai nem módosulhatnak visszamenőleg.
14. A dokumentumfájlok nem az adatbázisban, hanem külső fájl- vagy objektumtárolóban találhatók.
15. A dokumentumletöltéshez mindig ügyfélcég-szintű jogosultság-ellenőrzés szükséges.
16. Az eszköz működési állapota részben számított adat.
17. A duplikált értesítéseket egyedi deduplikációs kulcs akadályozza meg.
18. A történeti és hivatalos rekordok fizikai törlése nem megengedett.

---

# 19. Következő tervezési lépések

A jelen dokumentum elfogadása után javasolt sorrend:

1. kötelező és opcionális mezők véglegesítése;
2. enumértékek véglegesítése;
3. adatbázisnév-konvenciók meghatározása;
4. törlési és idegenkulcs-viselkedések rögzítése;
5. konkrét adatbázisséma és migrációs terv;
6. domainüzleti szabályok és állapotátmenetek részletesítése;
7. API-erőforrások és végpontok tervezése;
8. frontend képernyők és űrlapok illesztése az entitásokhoz.
