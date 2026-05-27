# Muhasebe Uygulamasi Roadmap Dokumani

## 1. Proje Ozeti

Novacare ERP reposu bir muhasebe/ERP uygulamasina temel olacak sekilde planlanmalidir. Hedef; firmalarin cari hesaplarini, musteri/tedarikci iliskilerini, gelir-giderlerini, faturalarini, odeme/tahsilatlarini, kasa/banka hareketlerini ve temel raporlarini tek panelden yonetebilmesidir.

Hedef kullanici tipleri:

- **Sistem yoneticisi:** Platform genel ayarlari, paketler, firma hesaplari ve global denetim.
- **Firma sahibi / admin:** Firma bilgileri, kullanicilar, roller, muhasebe kayitlari ve raporlar.
- **Muhasebe personeli:** Cari hesap, fatura, gelir-gider, odeme/tahsilat islemleri.
- **Satis/operasyon kullanicisi:** Musteri, urun/hizmet, teklif/fatura ve tahsilat takibi.
- **Salt okunur kullanici / denetci:** Raporlari ve kayitlari sadece goruntuler.

**Varsayim:** Uygulama Turkiye odakli muhasebe ihtiyaclarini hedefliyor. KDV, fatura numarasi, firma bazli veri izolasyonu ve audit log bu nedenle kritik kabul edilmistir.

## 2. Mevcut Durum Analizi

Repo inceleme tarihi: 2026-05-28.

Mevcut repo durumu:

- Repo klasorunde sadece `.git` klasoru bulunuyor.
- Kaynak kod, proje dosyasi, README, backend, frontend, mobile veya shared katman yok.
- `git status` sonucu: `No commits yet on master`.
- `rg --files` hic dosya dondurmedi.

Klasor yapisi:

```text
Novacare ERP/
  .git/
```

Kullanilan teknolojiler:

- Henuz tespit edilen teknoloji yok.
- .NET Core backend/web tarafinin planlandigi kullanici isteginden anlasiliyor.

Backend/frontend/mobile/shared durumu:

- Backend yok.
- Web/admin panel yok.
- Mobile katman yok.
- Shared/domain katmani yok.

Entity/model yapisi:

- Henuz entity, DTO, view model veya domain model yok.

Servis/repository yapisi:

- Henuz servis, repository, unit of work veya CQRS yapisi yok.

Authentication/authorization:

- Henuz auth yapisi yok.
- JWT, refresh token, role/permission ve tenant izolasyonu sifirdan tasarlanmali.

Database/migration:

- Henuz DbContext, connection string, migration veya seed data yok.

Eksik veya yarim birakilmis alanlar:

- Repo bos oldugu icin tum urun ve teknik temel eksik.
- README dahil proje aciklamasi yok.
- CI/CD, test, docker, environment ayarlari yok.
- Domain sinirlari ve modul sahipligi tanimli degil.

Kod kalitesi ve mimari problemler:

- Kod olmadigi icin kod kalitesi problemi olculmedi.
- En buyuk mimari risk, projeye ilk adimda plansiz ve katmansiz baslanmasidir.
- Muhasebe uygulamasi veri dogrulugu, yetki, audit ve tenant izolasyonu gerektirdigi icin basit CRUD mimarisi uzun vadede yetersiz kalir.

## 3. Hedef Mimari

Onerilen yapi: **Clean Architecture tabanli layered architecture**.

Baslangic icin tek solution icinde moduler monolith onerilir. Mikroservis erken asamada gereksiz operasyonel maliyet olusturur. Modul sinirlari net tutulursa ileride ayrisma mumkun olur.

Onerilen solution yapisi:

```text
src/
  NovacareERP.Api/
  NovacareERP.Application/
  NovacareERP.Domain/
  NovacareERP.Infrastructure/
  NovacareERP.Web/
tests/
  NovacareERP.UnitTests/
  NovacareERP.IntegrationTests/
docs/
  MUHASEBE_UYGULAMASI_ROADMAP.md
```

### API Katmani

Sorumluluklar:

- REST endpointleri.
- Request/response DTO giris-cikisi.
- Auth middleware, tenant middleware.
- Swagger/OpenAPI.
- Rate limit, global exception handling.

Onerilen teknoloji:

- ASP.NET Core Web API
- Minimal API veya Controller; muhasebe gibi genis modulde Controller daha okunabilir olabilir.
- FluentValidation
- JWT Bearer Authentication
- API versioning

### Application Katmani

Sorumluluklar:

- Use case servisleri.
- Command/query handler yapisi.
- DTO, validation, business flow.
- Transaction sinirlari.
- Yetki ve tenant kontrolunun uygulama seviyesindeki garantileri.

Onerilen yaklasim:

- Baslangicta servis tabanli yapi: `ICurrentAccountService`, `IInvoiceService`.
- Proje buyudukce MediatR/CQRS dusunulebilir.

### Domain Katmani

Sorumluluklar:

- Entity ve value object modelleri.
- Domain kurallari.
- Enumlar.
- Ortak audit/soft delete/tenant interface'leri.

Kritik domain ilkeleri:

- Para icin `decimal` kullanilmali.
- Tum finansal kayitlar `CompanyId` tasimali.
- Silme islemleri muhasebe izlenebilirligi icin varsayilan olarak soft delete olmali.
- Fatura ve odeme gibi kayitlarda durum gecisleri domain kuralina baglanmali.

### Infrastructure Katmani

Sorumluluklar:

- EF Core DbContext.
- Repository implementasyonlari.
- Migration.
- E-posta, dosya, PDF, Excel export servisleri.
- Token, hash, storage gibi teknik servisler.

Onerilen teknoloji:

- EF Core
- PostgreSQL veya SQL Server
- Serilog
- ClosedXML veya EPPlus
- QuestPDF veya DinkToPdf

### Web/Admin Panel Katmani

Onerilen iki secenek:

- **ASP.NET Core MVC/Razor Pages:** Tek .NET stack, hizli MVP, server-rendered admin panel.
- **Blazor WebAssembly/Server:** .NET odakli interaktif panel.

Baslangic onerisi:

- API ile ayrik calisan admin panel hedefleniyorsa Blazor veya React dusunulebilir.
- Kullanici ozellikle .NET Core web/admin dedigi icin ilk MVP icin **ASP.NET Core MVC veya Blazor Server** uygulanabilir.

### Database Yapisi

Onerilen:

- Baslangic: PostgreSQL veya SQL Server.
- EF Core migration ile versiyonlama.
- Her tenant verisinde `CompanyId`.
- Ortak kolonlar: `Id`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, `DeletedAt`, `DeletedBy`, `IsDeleted`.
- Finansal kayitlarda `CurrencyCode`, `ExchangeRate`, `NetAmount`, `TaxAmount`, `GrossAmount`.

### Authentication Yapisi

- ASP.NET Core Identity veya custom Identity modeli.
- JWT access token.
- Refresh token tablosu.
- Role based authorization.
- Permission/policy bazli yetki.
- Firma bazli tenant context.
- Login sonrasi aktif firma secimi veya kullanicinin varsayilan firmasi.

## 4. Temel Moduller

### Firma Yonetimi

- Firma kaydi, vergi bilgileri, adres, iletisim.
- Firma ayarlari: para birimi, fatura numara serisi, KDV varsayilanlari.
- Multi-company kullanici destegi.

### Kullanici/Rol Yonetimi

- Kullanici daveti.
- Rol atama.
- Permission/policy matrisi.
- Aktif/pasif kullanici.

### Cari Hesaplar

- Musteri/tedarikci/karma cari tipi.
- Borc/alacak hareketleri.
- Cari bakiye.
- Mutabakat ve hesap ekstresi.

### Musteri/Tedarikci Yonetimi

- Cari hesaba bagli profil bilgileri.
- Vergi dairesi/no, adresler, iletisim kisileri.
- Risk limiti ve vade gunu.

### Gelir-Gider Takibi

- Kategori bazli kayit.
- Belge/fis/fatura iliskisi.
- Odeme durumu.
- Tekrarlayan gider opsiyonu.

### Fatura Yonetimi

- Satis/alis faturasi.
- Kalem bazli urun/hizmet.
- KDV hesaplama.
- Fatura durumu: Taslak, Kesildi, Kismi Odendi, Odendi, Iptal.
- PDF cikti.

### Tahsilat/Odeme Yonetimi

- Faturaya bagli veya bagimsiz odeme.
- Kasa/banka hesabi ile iliski.
- Kismi odeme.
- Cari bakiyeyi otomatik etkileme.

### Kasa/Banka Yonetimi

- Kasa hesaplari.
- Banka hesaplari.
- Para transferleri.
- Hesap hareketleri.

### Urun/Hizmet Yonetimi

- Urun veya hizmet karti.
- Birim, fiyat, KDV orani.
- Stok takibi opsiyonel.

### Stok Yonetimi

- Depo, stok hareketi, minimum stok.
- Faturaya bagli stok cikisi/girisi.
- Stok degerleme MVP sonrasi ele alinabilir.

### Raporlama

- Dashboard ozetleri.
- Gelir-gider raporu.
- Cari bakiye raporu.
- Fatura yaslandirma.
- KDV raporu.

### Vergi/KDV Hesaplari

- KDV oranlari.
- Kalem bazli vergi.
- KDV dahil/haric fiyat.
- Yuvarlama kurallari.

### Abonelik/Paket Yonetimi

- Paket tanimlari.
- Firma aboneligi.
- Limitler: kullanici sayisi, fatura sayisi, storage.

### Bildirim Sistemi

- Vadesi gelen fatura/odeme.
- Kullanici daveti.
- Sistem uyarilari.

### Dosya/PDF/Excel Export Sistemi

- Fatura PDF.
- Cari ekstre PDF/Excel.
- Rapor Excel export.
- Dosya yukleme: makbuz, dekont, fatura gorseli.

## 5. Database Tasarimi

Tum tablolarda onerilen ortak alanlar:

- `Id` UUID veya long identity.
- `CompanyId` tenant verisi olan tablolarda zorunlu.
- `CreatedAt`, `CreatedBy`.
- `UpdatedAt`, `UpdatedBy`.
- `IsDeleted`, `DeletedAt`, `DeletedBy`.
- `RowVersion` optimistic concurrency icin.

### Users

Alanlar:

- `Id`
- `Email`
- `PasswordHash`
- `FullName`
- `PhoneNumber`
- `IsActive`
- `LastLoginAt`
- `DefaultCompanyId`

Iliskiler:

- User birden fazla firmada rol alabilir.
- `UserCompanyRoles` ara tablosu onerilir.

Kritik noktalar:

- Email unique olmali.
- Password hash kesinlikle plain text olmamali.

### Roles

Alanlar:

- `Id`
- `Name`
- `Description`
- `IsSystemRole`

Iliskiler:

- Role permissions ile iliskili.
- User-company-role baglantisi ile firmaya ozel atanabilir.

### Companies

Alanlar:

- `Id`
- `Name`
- `TaxNumber`
- `TaxOffice`
- `Email`
- `Phone`
- `Address`
- `DefaultCurrencyCode`
- `InvoicePrefix`
- `CurrentInvoiceNumber`
- `IsActive`

Kritik noktalar:

- Vergi numarasi unique olmayabilir; sube/deneme durumlari dusunulmeli.
- Fatura numarasi concurrency korumali uretilmeli.

### Customers

Alanlar:

- `Id`
- `CompanyId`
- `CurrentAccountId`
- `Name`
- `TaxNumber`
- `TaxOffice`
- `Email`
- `Phone`
- `Address`
- `CreditLimit`
- `PaymentTermDays`

Iliskiler:

- Bir customer bir current account ile eslesir.

### Suppliers

Alanlar:

- `Id`
- `CompanyId`
- `CurrentAccountId`
- `Name`
- `TaxNumber`
- `TaxOffice`
- `Email`
- `Phone`
- `Address`
- `PaymentTermDays`

Iliskiler:

- Bir supplier bir current account ile eslesir.

### CurrentAccounts

Alanlar:

- `Id`
- `CompanyId`
- `Code`
- `Name`
- `Type` (`Customer`, `Supplier`, `Both`)
- `OpeningBalance`
- `OpeningBalanceType` (`Debit`, `Credit`)
- `CurrencyCode`
- `Balance`
- `IsActive`

Iliskiler:

- Invoices, payments, income/expense records ile baglantili.

Kritik noktalar:

- `Balance` hesaplanabilir veya cache alan olabilir. Cache tutulursa her hareket transaction icinde guncellenmeli.

### Invoices

Alanlar:

- `Id`
- `CompanyId`
- `CurrentAccountId`
- `InvoiceNumber`
- `InvoiceType` (`Sales`, `Purchase`)
- `IssueDate`
- `DueDate`
- `CurrencyCode`
- `ExchangeRate`
- `NetAmount`
- `TaxAmount`
- `DiscountAmount`
- `GrossAmount`
- `PaidAmount`
- `Status`
- `Notes`

Iliskiler:

- InvoiceItems ile bire-cok.
- Payments ile bire-cok veya payment allocation tablosu ile cok-cok.

Kritik noktalar:

- `CompanyId + InvoiceNumber` unique olmali.
- Tutarlar kalemlerden hesaplanmali, request'ten gelen toplamlar dogrulanmali.

### InvoiceItems

Alanlar:

- `Id`
- `InvoiceId`
- `ProductId`
- `ServiceId`
- `Description`
- `Quantity`
- `UnitPrice`
- `TaxRateId`
- `TaxRate`
- `DiscountRate`
- `NetAmount`
- `TaxAmount`
- `GrossAmount`

Kritik noktalar:

- Product veya Service opsiyonel olabilir; aciklama satiri desteklenmeli.

### Payments

Alanlar:

- `Id`
- `CompanyId`
- `CurrentAccountId`
- `InvoiceId`
- `PaymentType` (`Collection`, `Payment`)
- `PaymentDate`
- `Amount`
- `CurrencyCode`
- `ExchangeRate`
- `Method` (`Cash`, `BankTransfer`, `CreditCard`, `Other`)
- `CashAccountId`
- `BankAccountId`
- `ReferenceNumber`
- `Description`

Kritik noktalar:

- Kismi odeme desteklenmeli.
- Odeme silinirse fatura ve cari bakiye yeniden hesaplanmali.

### Expenses

Alanlar:

- `Id`
- `CompanyId`
- `SupplierId`
- `CategoryId`
- `ExpenseDate`
- `Description`
- `NetAmount`
- `TaxAmount`
- `GrossAmount`
- `PaymentStatus`
- `InvoiceId`

### IncomeRecords

Alanlar:

- `Id`
- `CompanyId`
- `CustomerId`
- `CategoryId`
- `IncomeDate`
- `Description`
- `NetAmount`
- `TaxAmount`
- `GrossAmount`
- `PaymentStatus`
- `InvoiceId`

### BankAccounts

Alanlar:

- `Id`
- `CompanyId`
- `BankName`
- `BranchName`
- `Iban`
- `CurrencyCode`
- `OpeningBalance`
- `CurrentBalance`
- `IsActive`

### CashAccounts

Alanlar:

- `Id`
- `CompanyId`
- `Name`
- `CurrencyCode`
- `OpeningBalance`
- `CurrentBalance`
- `IsActive`

### Products

Alanlar:

- `Id`
- `CompanyId`
- `Code`
- `Name`
- `Unit`
- `SalesPrice`
- `PurchasePrice`
- `TaxRateId`
- `TrackStock`
- `IsActive`

### Services

Alanlar:

- `Id`
- `CompanyId`
- `Code`
- `Name`
- `Unit`
- `SalesPrice`
- `PurchasePrice`
- `TaxRateId`
- `IsActive`

### Stocks

Alanlar:

- `Id`
- `CompanyId`
- `ProductId`
- `WarehouseId`
- `Quantity`
- `MinimumQuantity`

Ek tablo onerisi:

- `StockMovements`: giris/cikis hareketleri icin sart.

### TaxRates

Alanlar:

- `Id`
- `CompanyId`
- `Name`
- `Rate`
- `IsDefault`
- `IsActive`

Kritik noktalar:

- Turkiye KDV oranlari degisebilir; hard-code edilmemeli.

### Subscriptions

Alanlar:

- `Id`
- `CompanyId`
- `PlanId`
- `Status`
- `StartDate`
- `EndDate`
- `BillingPeriod`
- `MaxUsers`
- `MaxInvoicesPerMonth`

Ek tablo onerisi:

- `Plans`: paket tanimlari.

### AuditLogs

Alanlar:

- `Id`
- `CompanyId`
- `UserId`
- `EntityName`
- `EntityId`
- `Action`
- `OldValues`
- `NewValues`
- `IpAddress`
- `UserAgent`
- `CreatedAt`

Kritik noktalar:

- Finansal kayitlarda audit log opsiyonel degil, zorunlu olmali.

## 6. Backend Roadmap

### 1. Proje Mimarisini Duzenleme

Ne yapilacak:

- Solution ve katman projeleri olusturulacak.
- Ortak coding conventions, nullable reference types ve analyzers ayarlanacak.
- API calisir hale getirilecek.

Dosyalar:

- `NovacareERP.sln`
- `src/NovacareERP.Api/`
- `src/NovacareERP.Application/`
- `src/NovacareERP.Domain/`
- `src/NovacareERP.Infrastructure/`
- `tests/NovacareERP.UnitTests/`
- `tests/NovacareERP.IntegrationTests/`

Endpointler:

- `GET /api/health`

Dikkat:

- Katman referanslari ters baglanmamali. Domain hicbir katmana bagimli olmamali.

Kabul kriterleri:

- Solution build alir.
- Health endpoint calisir.
- Swagger acilir.

### 2. Entity Modellerini Olusturma

Ne yapilacak:

- Base entity, audit, soft delete, tenant interface'leri yazilacak.
- Temel domain entity'leri eklenecek.

Dosyalar:

- `Domain/Common/BaseEntity.cs`
- `Domain/Common/IAuditableEntity.cs`
- `Domain/Common/ISoftDelete.cs`
- `Domain/Common/ICompanyScoped.cs`
- `Domain/Entities/Company.cs`
- `Domain/Entities/CurrentAccount.cs`
- `Domain/Entities/Invoice.cs`
- `Domain/Entities/InvoiceItem.cs`
- `Domain/Entities/Payment.cs`

Endpointler:

- Bu adimda endpoint zorunlu degil.

Dikkat:

- Finansal alanlar `decimal` olmali.
- Entity'lerde gereksiz public setter yerine kontrollu metotlar dusunulmeli.

Kabul kriterleri:

- Entity'ler compile olur.
- Basit unit testlerle fatura toplam hesaplama dogrulanir.

### 3. DbContext ve Migration Hazirlama

Ne yapilacak:

- EF Core DbContext yazilacak.
- Entity configuration dosyalari ayrilacak.
- Ilk migration alinacak.

Dosyalar:

- `Infrastructure/Persistence/AppDbContext.cs`
- `Infrastructure/Persistence/Configurations/*.cs`
- `Infrastructure/Persistence/Migrations/`
- `Api/appsettings.json`

Endpointler:

- `GET /api/health/db`

Dikkat:

- Unique indexler: fatura no, cari kod, urun kodu.
- Global query filter: `IsDeleted`, `CompanyId`.

Kabul kriterleri:

- Migration basarili calisir.
- Database olusur.
- Temel tablolar beklenen kolonlarla gelir.

### 4. Authentication Sistemi

Ne yapilacak:

- Register, login, refresh token, logout akislari.
- Password hashing.
- JWT uretimi.

Dosyalar:

- `Application/Auth/AuthService.cs`
- `Application/Auth/Dto/*.cs`
- `Infrastructure/Auth/JwtTokenService.cs`
- `Domain/Entities/User.cs`
- `Domain/Entities/RefreshToken.cs`
- `Api/Controllers/AuthController.cs`

Endpointler:

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/refresh`
- `POST /api/auth/logout`
- `GET /api/auth/me`

Dikkat:

- Refresh token hash'lenerek saklanmali.
- Login rate limit uygulanmali.

Kabul kriterleri:

- Kullanici kaydolur, login olur, token alir.
- Gecersiz token korumali endpointlere erisemez.

### 5. Firma Bazli Tenant Yapisi

Ne yapilacak:

- Kullanici-firma iliskisi.
- Aktif firma context'i.
- Tenant middleware.

Dosyalar:

- `Domain/Entities/UserCompanyRole.cs`
- `Application/Common/ITenantContext.cs`
- `Infrastructure/Tenancy/TenantContext.cs`
- `Api/Middleware/TenantMiddleware.cs`

Endpointler:

- `GET /api/companies`
- `POST /api/companies`
- `POST /api/companies/{id}/switch`

Dikkat:

- Kullanici baska firmanin `CompanyId` degerini request'e koyarak veri okuyamamali.

Kabul kriterleri:

- Firma verileri kullaniciya gore izole edilir.
- Cross-tenant request 403/404 doner.

### 6. Cari Hesap Modulu

Ne yapilacak:

- Cari hesap CRUD.
- Musteri/tedarikci baglantisi.
- Cari hareket ve bakiye mantigi.

Dosyalar:

- `Domain/Entities/CurrentAccount.cs`
- `Domain/Entities/CurrentAccountTransaction.cs`
- `Application/CurrentAccounts/*`
- `Api/Controllers/CurrentAccountsController.cs`

Endpointler:

- `GET /api/current-accounts`
- `GET /api/current-accounts/{id}`
- `POST /api/current-accounts`
- `PUT /api/current-accounts/{id}`
- `DELETE /api/current-accounts/{id}`
- `GET /api/current-accounts/{id}/statement`

Dikkat:

- Bakiye hesaplari transaction icinde guncellenmeli.

Kabul kriterleri:

- Cari hesap acilir, listelenir, ekstre gorulur.
- Silme soft delete yapar.

### 7. Fatura Modulu

Ne yapilacak:

- Fatura ve kalemleri.
- KDV, indirim, toplam hesaplari.
- Fatura numarasi uretimi.

Dosyalar:

- `Application/Invoices/*`
- `Api/Controllers/InvoicesController.cs`
- `Domain/Services/InvoiceCalculator.cs`

Endpointler:

- `GET /api/invoices`
- `GET /api/invoices/{id}`
- `POST /api/invoices`
- `PUT /api/invoices/{id}`
- `POST /api/invoices/{id}/issue`
- `POST /api/invoices/{id}/cancel`
- `GET /api/invoices/{id}/pdf`

Dikkat:

- Taslak fatura degisebilir; kesilmis fatura kontrollu degismeli.
- Fatura numarasi unique ve sirali olmali.

Kabul kriterleri:

- Fatura kalemlerinden dogru toplam hesaplanir.
- Fatura cari bakiyeyi etkiler.

### 8. Gelir-Gider Modulu

Ne yapilacak:

- Gelir/gider kayitlari.
- Kategori yapisi.
- Odeme durumu.

Dosyalar:

- `Domain/Entities/Expense.cs`
- `Domain/Entities/IncomeRecord.cs`
- `Domain/Entities/FinancialCategory.cs`
- `Application/Expenses/*`
- `Application/IncomeRecords/*`

Endpointler:

- `GET /api/expenses`
- `POST /api/expenses`
- `PUT /api/expenses/{id}`
- `DELETE /api/expenses/{id}`
- `GET /api/income-records`
- `POST /api/income-records`

Dikkat:

- Faturadan uretilen gelir/gider kaydi ile manuel kayit ayrilmali.

Kabul kriterleri:

- Tarih araligina gore gelir/gider raporlanabilir.

### 9. Odeme/Tahsilat Modulu

Ne yapilacak:

- Faturaya bagli ve bagimsiz odeme.
- Kasa/banka bakiyesi guncelleme.
- Kismi odeme.

Dosyalar:

- `Application/Payments/*`
- `Api/Controllers/PaymentsController.cs`
- `Domain/Entities/Payment.cs`
- `Domain/Entities/BankAccount.cs`
- `Domain/Entities/CashAccount.cs`

Endpointler:

- `GET /api/payments`
- `POST /api/payments`
- `GET /api/payments/{id}`
- `DELETE /api/payments/{id}`

Dikkat:

- Odeme iptali cari, fatura ve kasa/banka bakiyesini geri almalidir.

Kabul kriterleri:

- Kismi odeme sonrasi fatura durumu `PartiallyPaid` olur.
- Tam odeme sonrasi `Paid` olur.

### 10. Raporlama API'leri

Ne yapilacak:

- Dashboard metrikleri.
- Gelir-gider, cari bakiye, KDV raporlari.

Dosyalar:

- `Application/Reports/*`
- `Api/Controllers/ReportsController.cs`

Endpointler:

- `GET /api/reports/dashboard`
- `GET /api/reports/income-expense`
- `GET /api/reports/current-balances`
- `GET /api/reports/tax`

Dikkat:

- Rapor sorgulari paging ve tarih filtresi kullanmali.

Kabul kriterleri:

- Raporlar sadece aktif firmaya ait veriyi dondurur.

### 11. Export Servisleri

Ne yapilacak:

- PDF ve Excel export.
- Fatura PDF.
- Cari ekstre Excel.

Dosyalar:

- `Application/Exports/*`
- `Infrastructure/Exports/PdfExportService.cs`
- `Infrastructure/Exports/ExcelExportService.cs`

Endpointler:

- `GET /api/invoices/{id}/pdf`
- `GET /api/current-accounts/{id}/statement/export`
- `GET /api/reports/income-expense/export`

Dikkat:

- Export yetkisi ayri permission olabilir.

Kabul kriterleri:

- Dosyalar dogru content-type ile iner.

### 12. Loglama ve Audit

Ne yapilacak:

- Serilog.
- Audit log interceptor.
- Kritik islemlerde eski/yeni deger kaydi.

Dosyalar:

- `Infrastructure/Logging/*`
- `Infrastructure/Persistence/Interceptors/AuditSaveChangesInterceptor.cs`
- `Domain/Entities/AuditLog.cs`

Endpointler:

- `GET /api/audit-logs`

Dikkat:

- Sifre/token gibi hassas alanlar audit log'a yazilmamali.

Kabul kriterleri:

- Fatura, odeme, cari gibi islemler audit log uretir.

### 13. Testler

Ne yapilacak:

- Unit, integration, API, auth, permission testleri.

Dosyalar:

- `tests/NovacareERP.UnitTests/*`
- `tests/NovacareERP.IntegrationTests/*`

Dikkat:

- Finansal hesaplama testleri snapshot gibi kirilgan degil, net beklenen tutarlar ile yazilmali.

Kabul kriterleri:

- CI'da tum testler gecer.
- KDV, kismi odeme, tenant izolasyonu testlenir.

### 14. Production Hazirligi

Ne yapilacak:

- Dockerfile, compose, environment config.
- Health checks.
- Migration stratejisi.
- Secret yonetimi.

Dosyalar:

- `Dockerfile`
- `docker-compose.yml`
- `.github/workflows/ci.yml`
- `src/NovacareERP.Api/appsettings.Production.json`

Dikkat:

- Production secret'lari repoya konmamalidir.

Kabul kriterleri:

- Uygulama production config ile ayağa kalkar.
- CI build/test calisir.

## 7. Web/Admin Panel Roadmap

### Login/Register

Amac:

- Kullanici girisi ve yeni firma/kullanici kaydi.

Alanlar:

- Email, sifre, ad soyad, firma adi.

API:

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/refresh`

Validasyon:

- Email format.
- Guclu sifre.
- Zorunlu alanlar.

Akis:

- Kayit sonrasi firma olusturulur, kullanici admin role atanir.
- Login sonrasi dashboard'a yonlendirilir.

State'ler:

- Loading: buton disabled.
- Hata: invalid credentials, rate limit.
- Bos: form alanlari.

### Dashboard

Amac:

- Firma finansal durumunu ozetlemek.

Alanlar:

- Toplam gelir, gider, net durum.
- Odenmemis faturalar.
- Vadesi gecen odemeler.
- Cari borc/alacak ozeti.

API:

- `GET /api/reports/dashboard`

Validasyon:

- Tarih araligi mantikli olmali.

Akis:

- Kullanici login sonrasi ilk bu ekrani gorur.

State'ler:

- Loading skeleton.
- Veri yoksa bos dashboard.
- API hata mesaji.

### Firma Ayarlari

Amac:

- Firma bilgileri ve muhasebe varsayilanlarini yonetmek.

Alanlar:

- Firma adi, vergi no, vergi dairesi, adres, para birimi, fatura prefix.

API:

- `GET /api/companies/{id}`
- `PUT /api/companies/{id}`

Validasyon:

- Fatura prefix bos olmamali.
- Vergi no uzunluk kontrolu.

Akis:

- Admin firma ayarlarini gunceller.

State'ler:

- Kaydediliyor.
- Basarili toast.
- Yetkisiz erisim.

### Cari Hesap Listesi/Detayi

Amac:

- Cari hesaplari listelemek, filtrelemek, bakiye ve ekstre goruntulemek.

Alanlar:

- Kod, ad, tip, bakiye, para birimi, durum.

API:

- `GET /api/current-accounts`
- `GET /api/current-accounts/{id}`
- `GET /api/current-accounts/{id}/statement`

Validasyon:

- Kod firma icinde unique olmali.

Akis:

- Liste > detay > hareketler > export.

State'ler:

- Bos cari listesi.
- Filtre sonucu yok.
- Ekstre yuklenemedi.

### Musteri/Tedarikci Ekranlari

Amac:

- Musteri ve tedarikci bilgilerini yonetmek.

Alanlar:

- Ad, vergi no, email, telefon, adres, vade, risk limiti.

API:

- `GET /api/customers`
- `POST /api/customers`
- `PUT /api/customers/{id}`
- `GET /api/suppliers`
- `POST /api/suppliers`

Validasyon:

- Email format.
- Risk limiti negatif olamaz.

Akis:

- Kayit olusturuldugunda otomatik cari hesap acilir.

State'ler:

- Bos liste.
- Duplicate cari uyarisi.

### Fatura Olusturma

Amac:

- Satis/alis faturasi olusturmak.

Alanlar:

- Cari hesap, fatura tipi, tarih, vade, kalemler, KDV, indirim, not.

API:

- `POST /api/invoices`
- `GET /api/current-accounts`
- `GET /api/products`
- `GET /api/services`
- `GET /api/tax-rates`

Validasyon:

- En az bir kalem.
- Miktar ve fiyat sifirdan buyuk.
- KDV orani gecerli.

Akis:

- Taslak kaydet veya faturayi kes.

State'ler:

- Kalem yok.
- Hesaplama hatasi.
- Kayit basarili.

### Fatura Listesi/Detayi

Amac:

- Faturalari takip etmek ve PDF almak.

Alanlar:

- Fatura no, cari, tarih, vade, toplam, odenen, durum.

API:

- `GET /api/invoices`
- `GET /api/invoices/{id}`
- `POST /api/invoices/{id}/cancel`
- `GET /api/invoices/{id}/pdf`

Validasyon:

- Iptal gerekcesi istenebilir.

Akis:

- Liste > detay > odeme ekle > PDF indir.

State'ler:

- Fatura bulunamadi.
- Yetkisiz iptal.

### Gelir-Gider Ekrani

Amac:

- Manuel gelir/gider kaydi yapmak ve takip etmek.

Alanlar:

- Tip, kategori, tarih, aciklama, tutar, KDV, odeme durumu.

API:

- `GET /api/expenses`
- `POST /api/expenses`
- `GET /api/income-records`
- `POST /api/income-records`

Validasyon:

- Tutar pozitif.
- Tarih bos olamaz.

Akis:

- Kayit girilir, dashboard ve rapora yansir.

State'ler:

- Bos kategori.
- Validasyon hatasi.

### Kasa/Banka Ekrani

Amac:

- Nakit ve banka hesaplarini, hareketlerini izlemek.

Alanlar:

- Hesap adi, banka, IBAN, para birimi, bakiye.

API:

- `GET /api/cash-accounts`
- `POST /api/cash-accounts`
- `GET /api/bank-accounts`
- `POST /api/bank-accounts`

Validasyon:

- IBAN format.
- Para birimi zorunlu.

Akis:

- Hesap acilir, odeme/tahsilat bu hesaplara baglanir.

State'ler:

- Hareket yok.
- Hesap pasif.

### Raporlar

Amac:

- Finansal karar icin ozet ve detay raporlar.

Alanlar:

- Tarih araligi, cari, kategori, durum filtreleri.

API:

- `GET /api/reports/income-expense`
- `GET /api/reports/current-balances`
- `GET /api/reports/tax`

Validasyon:

- Baslangic tarihi bitisten sonra olamaz.

Akis:

- Filtrele > goruntule > export.

State'ler:

- Veri yok.
- Export hazirlaniyor.

### Kullanici/Rol Yonetimi

Amac:

- Firma kullanicilarini ve yetkilerini yonetmek.

Alanlar:

- Email, ad soyad, rol, durum.

API:

- `GET /api/users`
- `POST /api/users/invite`
- `PUT /api/users/{id}`
- `GET /api/roles`

Validasyon:

- Admin kendini son admin olmaktan cikaramaz.

Akis:

- Davet gonder > kullanici kabul eder > rol atanir.

State'ler:

- Davet bekliyor.
- Yetkisiz islem.

### Abonelik/Paket Ekrani

Amac:

- Firma paketini ve limitlerini gostermek.

Alanlar:

- Paket adi, baslangic/bitis, kullanici limiti, fatura limiti.

API:

- `GET /api/subscriptions/current`
- `GET /api/subscriptions/plans`
- `POST /api/subscriptions/change-plan`

Validasyon:

- Paket dusurmede mevcut kullanim limitleri kontrol edilmeli.

Akis:

- Mevcut paket gorulur, uygun paket secilir.

State'ler:

- Paket yok.
- Odeme/plan degisikligi hatasi.

## 8. API Endpoint Listesi

Standart hata response onerisi:

```json
{
  "traceId": "string",
  "code": "VALIDATION_ERROR",
  "message": "Validation failed.",
  "errors": {
    "field": ["error"]
  }
}
```

### Auth

| Method | URL | Request | Response | Yetki | Hata |
|---|---|---|---|---|---|
| POST | `/api/auth/register` | email, password, fullName, companyName | user, company, tokens | Anonymous | email exists, weak password |
| POST | `/api/auth/login` | email, password | accessToken, refreshToken, expiresAt | Anonymous | invalid credentials, inactive user |
| POST | `/api/auth/refresh` | refreshToken | accessToken, refreshToken | Anonymous | expired/revoked token |
| POST | `/api/auth/logout` | refreshToken | success | Authenticated | token not found |
| GET | `/api/auth/me` | - | user, companies, roles | Authenticated | unauthorized |

### Companies

| Method | URL | Request | Response | Yetki | Hata |
|---|---|---|---|---|---|
| GET | `/api/companies` | - | company list | Authenticated | unauthorized |
| POST | `/api/companies` | name, tax fields | company | Authenticated | validation |
| GET | `/api/companies/{id}` | - | company detail | CompanyMember | not found/forbidden |
| PUT | `/api/companies/{id}` | company fields | company | CompanyAdmin | validation/forbidden |
| POST | `/api/companies/{id}/switch` | - | active company context | CompanyMember | forbidden |

### Customers

| Method | URL | Request | Response | Yetki | Hata |
|---|---|---|---|---|---|
| GET | `/api/customers` | query filters | paged customers | Customer.Read | forbidden |
| POST | `/api/customers` | customer fields | customer | Customer.Write | validation |
| GET | `/api/customers/{id}` | - | customer | Customer.Read | not found |
| PUT | `/api/customers/{id}` | customer fields | customer | Customer.Write | validation |
| DELETE | `/api/customers/{id}` | - | success | Customer.Delete | has active invoices |

### Suppliers

| Method | URL | Request | Response | Yetki | Hata |
|---|---|---|---|---|---|
| GET | `/api/suppliers` | query filters | paged suppliers | Supplier.Read | forbidden |
| POST | `/api/suppliers` | supplier fields | supplier | Supplier.Write | validation |
| GET | `/api/suppliers/{id}` | - | supplier | Supplier.Read | not found |
| PUT | `/api/suppliers/{id}` | supplier fields | supplier | Supplier.Write | validation |
| DELETE | `/api/suppliers/{id}` | - | success | Supplier.Delete | has active records |

### CurrentAccounts

| Method | URL | Request | Response | Yetki | Hata |
|---|---|---|---|---|---|
| GET | `/api/current-accounts` | filters, paging | paged accounts | CurrentAccount.Read | forbidden |
| POST | `/api/current-accounts` | code, name, type | account | CurrentAccount.Write | duplicate code |
| GET | `/api/current-accounts/{id}` | - | account detail | CurrentAccount.Read | not found |
| PUT | `/api/current-accounts/{id}` | account fields | account | CurrentAccount.Write | validation |
| DELETE | `/api/current-accounts/{id}` | - | success | CurrentAccount.Delete | active balance |
| GET | `/api/current-accounts/{id}/statement` | date filters | statement | CurrentAccount.Read | not found |

### Invoices

| Method | URL | Request | Response | Yetki | Hata |
|---|---|---|---|---|---|
| GET | `/api/invoices` | filters, paging | paged invoices | Invoice.Read | forbidden |
| POST | `/api/invoices` | invoice header/items | invoice | Invoice.Write | invalid totals |
| GET | `/api/invoices/{id}` | - | invoice detail | Invoice.Read | not found |
| PUT | `/api/invoices/{id}` | invoice data | invoice | Invoice.Write | issued invoice locked |
| POST | `/api/invoices/{id}/issue` | - | invoice | Invoice.Issue | invalid state |
| POST | `/api/invoices/{id}/cancel` | reason | invoice | Invoice.Cancel | paid invoice |
| GET | `/api/invoices/{id}/pdf` | - | file | Invoice.Read | not found |

### Payments

| Method | URL | Request | Response | Yetki | Hata |
|---|---|---|---|---|---|
| GET | `/api/payments` | filters | paged payments | Payment.Read | forbidden |
| POST | `/api/payments` | payment fields | payment | Payment.Write | amount exceeds invoice |
| GET | `/api/payments/{id}` | - | payment | Payment.Read | not found |
| DELETE | `/api/payments/{id}` | reason | success | Payment.Delete | locked period |

### Expenses

| Method | URL | Request | Response | Yetki | Hata |
|---|---|---|---|---|---|
| GET | `/api/expenses` | filters | paged expenses | Expense.Read | forbidden |
| POST | `/api/expenses` | expense fields | expense | Expense.Write | validation |
| GET | `/api/expenses/{id}` | - | expense | Expense.Read | not found |
| PUT | `/api/expenses/{id}` | expense fields | expense | Expense.Write | validation |
| DELETE | `/api/expenses/{id}` | - | success | Expense.Delete | locked record |

### Reports

| Method | URL | Request | Response | Yetki | Hata |
|---|---|---|---|---|---|
| GET | `/api/reports/dashboard` | date range | summary cards/charts | Report.Read | forbidden |
| GET | `/api/reports/income-expense` | date/category filters | report rows/totals | Report.Read | invalid date |
| GET | `/api/reports/current-balances` | filters | balances | Report.Read | forbidden |
| GET | `/api/reports/tax` | date range | tax summary | Report.Read | invalid date |

### Users

| Method | URL | Request | Response | Yetki | Hata |
|---|---|---|---|---|---|
| GET | `/api/users` | filters | users | User.Read | forbidden |
| POST | `/api/users/invite` | email, roleId | invitation | User.Manage | email exists |
| PUT | `/api/users/{id}` | profile/status | user | User.Manage | last admin rule |
| DELETE | `/api/users/{id}` | - | success | User.Manage | cannot delete self |

### Roles

| Method | URL | Request | Response | Yetki | Hata |
|---|---|---|---|---|---|
| GET | `/api/roles` | - | roles | Role.Read | forbidden |
| POST | `/api/roles` | name, permissions | role | Role.Manage | duplicate |
| PUT | `/api/roles/{id}` | role fields | role | Role.Manage | system role locked |
| DELETE | `/api/roles/{id}` | - | success | Role.Manage | role in use |

### Subscriptions

| Method | URL | Request | Response | Yetki | Hata |
|---|---|---|---|---|---|
| GET | `/api/subscriptions/current` | - | subscription | CompanyAdmin | not found |
| GET | `/api/subscriptions/plans` | - | plans | Authenticated | none |
| POST | `/api/subscriptions/change-plan` | planId | subscription | CompanyAdmin | limit conflict |

## 9. Is Kurallari

- Her tenant verisi firmaya bagli olmali.
- Kullanici sadece yetkili oldugu firmanin verisini gormeli.
- Request body icindeki `CompanyId` guvenilir kabul edilmemeli; aktif tenant context'ten alinmali.
- Silme islemleri varsayilan olarak soft delete olmali.
- Fatura numarasi firma bazinda benzersiz olmali.
- Fatura numarasi concurrency problemi yasamadan uretilmeli.
- KDV hesaplari kalem bazli yapilmali.
- KDV dahil/haric fiyat mantigi acik secilmeli.
- Yuvarlama kurallari merkezi olmalidir.
- Odeme mutlaka cari hesap ile iliskili olmali.
- Faturaya bagli odeme fatura durumunu otomatik guncellemeli.
- Cari bakiye fatura, odeme, tahsilat ve acilis bakiyesine gore otomatik hesaplanmali.
- Finansal kayitlarda audit log tutulmali.
- Kritik islemler yetki kontrolunden gecmeli.
- Kesilmis/odenmis fatura direkt silinmemeli; iptal/iade akisi olmali.
- Kasa/banka hareketleri odeme/tahsilatla tutarli olmalidir.
- Negatif stok izni firma ayariyla kontrol edilmeli.
- Tarih araliklari ve muhasebe donemi kilitleme MVP sonrasi ama erken tasarimda dusunulmeli.

## 10. Guvenlik ve Yetkilendirme

### JWT Auth

- Access token kisa omurlu olmali.
- Token claim'leri: `sub`, `email`, `userId`, aktif `companyId`, roller/permission versiyonu.
- Token icine hassas veri konmamalidir.

### Refresh Token

- Refresh token database'de hash'lenerek tutulmali.
- Rotation uygulanmali: refresh kullanildiginda yenisi uretilir.
- Reuse detection ile calinmis token riski azaltilmali.

### Role Based Authorization

- Basit roller: Owner, Admin, Accountant, Staff, Viewer.
- Moduller icin permission tabanli policy onerilir.
- Sistem rolleri silinemez/guncellenemez olmali.

### Company/Tenant Isolation

- Tenant middleware aktif firmayi belirlemeli.
- EF Core global query filter ile `CompanyId` filtrelenmeli.
- Application servisleri yine de tenant kontrolu yapmali.
- Cross-tenant id erisimleri 404 veya 403 ile kapanmali.

### Input Validation

- FluentValidation.
- Request DTO'larda whitelist binding.
- Para alanlarinda negatif/scale kontrolleri.
- Tarih, email, IBAN, vergi no format kontrolleri.

### Rate Limit

- Login/register endpointlerinde daha siki limit.
- Export endpointlerinde abuse engeli.
- IP ve user bazli limit kombinasyonu.

### Audit Log

- Fatura, odeme, cari, firma ayari, rol/yetki degisiklikleri loglanmali.
- Eski/yeni degerler JSON olarak saklanabilir.
- Sifre, token, secret audit'e yazilmamalidir.

### Sensitive Data Protection

- Password hash icin guvenli algoritma.
- Connection string ve JWT secret environment/secret manager'da.
- PII maskeleme log politikasi.

### Production Secret Yonetimi

- Development `appsettings.Development.json`.
- Production secret'lari repo disinda.
- CI/CD secret store.
- Key rotation plani.

## 11. Test Plani

### Unit Test

- Domain hesaplamalari.
- InvoiceCalculator.
- CurrentAccount balance rules.
- Payment allocation.

### Integration Test

- DbContext mapping.
- Migration smoke test.
- Repository/service akislari.
- Transaction rollback senaryolari.

### API Test

- CRUD endpointleri.
- Paging/filter/sorting.
- Validation hata response'lari.

### Auth Test

- Register/login/refresh/logout.
- Expired token.
- Revoked refresh token.
- Inactive user.

### Permission Test

- Role bazli erisim.
- Permission bazli erisim.
- Son admini pasif yapma engeli.

### Financial Calculation Test

- KDV dahil/haric.
- Indirim.
- Kismi odeme.
- Fatura iptali.
- Cari bakiye.
- Farkli para birimi icin kur tutarliligi.

### Regression Test

- Kritik muhasebe akislari icin end-to-end senaryolar.
- Bug fix sonrasi otomatik test ekleme zorunlulugu.

## 12. MVP Plani

MVP kapsami:

- Auth: register, login, refresh token, logout.
- Firma olusturma ve firma ayarlari.
- Kullanici ve basit rol yapisi.
- Cari hesap CRUD.
- Musteri/tedarikci temel kayitlari.
- Gelir/gider kaydi.
- Satis/alis faturasi.
- Odeme/tahsilat.
- Basit dashboard.
- Basit raporlama.
- PDF fatura cikisi.
- Audit log temel kayitlari.

MVP disinda birakilabilecekler:

- Gelismis stok/depo yonetimi.
- E-fatura/e-arsiv entegrasyonu.
- Banka entegrasyonlari.
- Otomatik kur cekme.
- Gelismis abonelik/faturalandirma sistemi.
- Mobil uygulama.
- Gelismis bildirim sistemi.
- Muhasebe donemi kilitleme.
- Coklu dil.
- Gelismis workflow/onay mekanizmasi.

## 13. Riskler ve Eksikler

Net riskler:

- Repo tamamen bos; teknik temel, mimari ve urun kararlari henuz yok.
- Muhasebe uygulamasinda basit CRUD yaklasimi hizli patlar; bakiye, fatura, odeme ve audit tutarliligi merkezi tasarlanmali.
- Tenant izolasyonu hatasi veri sizintisina neden olur.
- Fatura numarasi concurrency ile cakisabilir.
- KDV ve yuvarlama hatalari finansal guven kaybettirir.
- Kismi odeme ve cari bakiye kurallari bastan netlesmezse raporlar yanlis olur.
- Soft delete ve audit log olmadan finansal kayitlar izlenemez.
- Yetki modeli basit role bagli kalirsa muhasebe operasyonlarinda esneklik yetmez.
- Production secret'lar yanlis yonetilirse guvenlik acigi olusur.
- Para birimi, kur farki, vade farki, iade/iptal gibi ileri konular MVP'den sonra bile mimariyi etkiler.
- Yasal/vergi uyumlulugu icin Turkiye mevzuati, e-fatura/e-arsiv entegrasyonu ve saklama kurallari ayrica incelenmelidir.
- Database migration disiplini olmazsa finansal veri kaybi riski dogar.
- Testsiz finansal hesaplama kodu canli ortamda ciddi hata uretir.

Eksik dusunulmus olabilecek alanlar:

- Muhasebe donemi kapatma/kilitleme.
- Iade faturasi.
- Kur farki.
- Stok maliyetlendirme.
- Cek/senet.
- Teklif/siparis/irsaliye.
- E-fatura entegrasyonu.
- Dosya saklama politikasi.
- KVKK/GDPR benzeri veri koruma surecleri.

## 14. Gelistirme Sirasi

### Sprint 1: Proje Mimarisi + Auth

- [ ] Solution yapisini olustur.
- [ ] API, Application, Domain, Infrastructure projelerini ekle.
- [ ] Health endpoint ve Swagger ekle.
- [ ] User ve RefreshToken entity'lerini olustur.
- [ ] JWT auth altyapisini kur.
- [ ] Register/login/refresh/logout endpointlerini yaz.
- [ ] Auth unit ve API testlerini ekle.

### Sprint 2: Firma ve Kullanici Yonetimi

- [ ] Company entity ve migration ekle.
- [ ] UserCompanyRole yapisini kur.
- [ ] Tenant middleware yaz.
- [ ] Company CRUD endpointlerini ekle.
- [ ] Role/permission temelini ekle.
- [ ] Kullanici davet veya kullanici ekleme akisini tasarla.
- [ ] Tenant izolasyonu integration testlerini yaz.

### Sprint 3: Cari Hesaplar

- [ ] CurrentAccount entity ve transaction modelini ekle.
- [ ] Musteri/tedarikci entity'lerini ekle.
- [ ] Cari hesap CRUD endpointlerini yaz.
- [ ] Cari ekstre endpointini yaz.
- [ ] Bakiye hesaplama servislerini test et.
- [ ] Soft delete davranisini uygula.

### Sprint 4: Gelir/Gider

- [ ] IncomeRecord, Expense, FinancialCategory entity'lerini ekle.
- [ ] Gelir/gider CRUD endpointlerini yaz.
- [ ] Kategori yonetimini ekle.
- [ ] Dashboard icin temel gelir/gider sorgularini hazirla.
- [ ] Finansal validation testlerini yaz.

### Sprint 5: Fatura

- [ ] Invoice ve InvoiceItem entity'lerini tamamla.
- [ ] InvoiceCalculator domain servisini yaz.
- [ ] Fatura CRUD endpointlerini yaz.
- [ ] Fatura kesme/iptal akisini ekle.
- [ ] KDV ve toplam hesaplama testlerini yaz.
- [ ] Fatura PDF export temelini ekle.

### Sprint 6: Odeme/Tahsilat

- [ ] Payment entity ve servislerini ekle.
- [ ] CashAccount ve BankAccount entity'lerini ekle.
- [ ] Odeme/tahsilat endpointlerini yaz.
- [ ] Kismi odeme ve fatura durum guncellemesini ekle.
- [ ] Cari ve kasa/banka bakiye testlerini yaz.

### Sprint 7: Raporlama

- [ ] Dashboard report endpointini tamamla.
- [ ] Gelir-gider raporu ekle.
- [ ] Cari bakiye raporu ekle.
- [ ] KDV raporu ekle.
- [ ] Excel export servislerini ekle.
- [ ] Rapor permission testlerini yaz.

### Sprint 8: Web Admin Panel

- [ ] Login/register ekranlarini yap.
- [ ] Dashboard ekranini yap.
- [ ] Firma ayarlari ekranini yap.
- [ ] Cari hesap liste/detay ekranlarini yap.
- [ ] Musteri/tedarikci ekranlarini yap.
- [ ] Fatura olusturma ve liste ekranlarini yap.
- [ ] Gelir/gider ekranini yap.
- [ ] Kasa/banka ekranini yap.
- [ ] Rapor ekranlarini yap.
- [ ] Kullanici/rol ekranlarini yap.

### Sprint 9: Test + Production Hazirligi

- [ ] Unit test kapsamlarini tamamla.
- [ ] Integration testleri tamamla.
- [ ] API regression testlerini ekle.
- [ ] Dockerfile ve docker-compose ekle.
- [ ] CI pipeline kur.
- [ ] Serilog ve centralized logging ayarla.
- [ ] Production config ve secret yonetimini netlestir.
- [ ] Migration/deploy runbook hazirla.

## 15. Codex Icin Sonraki Promptlar

### Mimari Duzenleme Promptu

```text
Bu bos Novacare ERP reposunda .NET tabanli Clean Architecture solution yapisini olustur.

Istenen yapi:
- src/NovacareERP.Api
- src/NovacareERP.Application
- src/NovacareERP.Domain
- src/NovacareERP.Infrastructure
- tests/NovacareERP.UnitTests
- tests/NovacareERP.IntegrationTests

ASP.NET Core Web API projesi, Swagger, health endpoint, nullable reference types ve katman referanslarini dogru kur.
Domain hicbir katmana bagimli olmasin.
Build alip sonucu raporla.
```

### Auth Sistemi Promptu

```text
Novacare ERP projesine JWT + refresh token tabanli authentication sistemi ekle.

Istenenler:
- User ve RefreshToken entity'leri
- Register, login, refresh, logout, me endpointleri
- Password hashing
- Refresh token rotation
- Auth DTO ve validation
- Unauthorized/validation hata response standardi
- Unit ve integration testler

Tenant yapisina hazir olacak sekilde tasarla, ancak firma izolasyonunu bir sonraki adima birak.
```

### Database/Entity Promptu

```text
Novacare ERP icin muhasebe domain entity'lerini ve EF Core persistence altyapisini olustur.

Entity'ler:
- Company
- CurrentAccount
- Customer
- Supplier
- Invoice
- InvoiceItem
- Payment
- Expense
- IncomeRecord
- BankAccount
- CashAccount
- Product
- Service
- TaxRate
- Subscription
- AuditLog

Ortak alanlar:
- Id
- CompanyId gereken tablolarda
- CreatedAt/CreatedBy
- UpdatedAt/UpdatedBy
- IsDeleted/DeletedAt/DeletedBy
- RowVersion

EF Core configuration dosyalarini ayir, migration hazirla, indexleri ekle.
```

### Cari Hesap Modulu Promptu

```text
Novacare ERP icin cari hesap modulunu uygula.

Istenenler:
- CurrentAccount CRUD
- Customer/Supplier ile cari hesap iliskisi
- CurrentAccountTransaction modeli
- Cari bakiye hesaplama
- Cari ekstre endpointi
- Company/Tenant izolasyonu
- Soft delete
- Validation
- Unit ve integration testler

Endpointler:
- GET /api/current-accounts
- GET /api/current-accounts/{id}
- POST /api/current-accounts
- PUT /api/current-accounts/{id}
- DELETE /api/current-accounts/{id}
- GET /api/current-accounts/{id}/statement
```

### Fatura Modulu Promptu

```text
Novacare ERP icin fatura modulunu uygula.

Istenenler:
- Sales/Purchase invoice destegi
- Invoice ve InvoiceItem servisleri
- Fatura numarasi uretimi
- KDV, indirim, net/brut toplam hesaplama
- Draft, Issued, PartiallyPaid, Paid, Cancelled durumlari
- Fatura kesme ve iptal akisi
- Cari bakiye etkisi
- PDF export endpointinin temel implementasyonu
- Finansal hesaplama testleri

Endpointler:
- GET /api/invoices
- GET /api/invoices/{id}
- POST /api/invoices
- PUT /api/invoices/{id}
- POST /api/invoices/{id}/issue
- POST /api/invoices/{id}/cancel
- GET /api/invoices/{id}/pdf
```

### Gelir/Gider Modulu Promptu

```text
Novacare ERP icin gelir-gider modulunu gelistir.

Istenenler:
- Expense ve IncomeRecord CRUD
- FinancialCategory yapisi
- KDV alanlari
- Odeme durumu
- Tarih araligi ve kategori filtreleri
- Dashboard raporlarina veri saglayacak query'ler
- Validation ve testler

Manuel kayit ile faturadan olusan kayit ayrimini modelde belirt.
```

### Web Dashboard Promptu

```text
Novacare ERP icin .NET Core web/admin panelinin ilk MVP ekranlarini olustur.

Teknoloji olarak mevcut solution'a uygun sekilde ASP.NET Core MVC veya Blazor Server sec ve gerekcesini kisaca belirt.

Ekranlar:
- Login/Register
- Dashboard
- Firma ayarlari
- Cari hesap listesi/detayi
- Fatura listesi/detayi
- Fatura olusturma
- Gelir/gider
- Kasa/banka
- Raporlar

API client, auth token saklama, loading/error/empty state ve temel validasyonlari ekle.
```

### Test Yazma Promptu

```text
Novacare ERP projesinde backend icin test kapsamini genislet.

Oncelikler:
- Auth flow testleri
- Tenant izolasyonu testleri
- Role/permission testleri
- Fatura KDV/toplam hesaplama testleri
- Kismi odeme testleri
- Cari bakiye testleri
- API validation testleri
- Regression test altyapisi

Testleri calistir, basarisiz olanlari duzelt ve kalan riskleri raporla.
```
