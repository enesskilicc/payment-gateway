# payment-gateway

Üye iş yerlerinin (merchant) müşterilerinden ödeme alabilmesini sağlayan bir **ödeme hizmet sağlayıcısı (PSP – Payment Service Provider)** altyapısı.

## Proje ne yapıyor?

Bir e-ticaret sitesi ya da uygulama, kart ödemesi almak istediğinde bankalarla tek tek entegre olmak yerine bir ödeme sağlayıcısına bağlanır. Ödeme sağlayıcısı arada durur. İş yerlerini kaydeder ve yönetir, gelen ödeme isteklerini doğrular, tutarları doğru para birimiyle işler ve her işlemin durumunu takip eder.

Bu proje böyle bir sistemin çekirdeğini sıfırdan inşa ediyor. Hedef, gerçek bir ödeme sisteminin ihtiyaç duyduğu kuralları (para hesaplarında hata yapmamak, geçersiz durum geçişlerine izin vermemek, her kuralı testle güvence altına almak) en baştan doğru kurmak.

> Bu bir **sandbox** projesidir. Gerçek para hareketi ve gerçek kart verisi yoktur; banka gibi dış sistemler simüle edilir.

Şu ana kadar hazır olanlar:

- **Para (`Money`):** Sistemde dolaşan her tutar, para birimiyle birlikte tek bir nesne olarak taşınır. TL ile doları yanlışlıkla toplamak ya da kuruş kaybetmek mümkün olmaz.
- **Üye iş yeri (`Merchant`):** Ödeme alan firmayı temsil eder. Aktif, askıda ya da silinmiş olabilir.
- **API anahtarı (`ApiKey`):** İş yerinin sisteme kimliğini kanıtladığı anahtar. Veritabanında yalnızca hash'i tutulur, gerektiğinde iptal edilebilir.
- **Ödeme (`Payment`):** Kart ödemesinin yaşam döngüsü: bloke (authorize), tahsilat (capture), iptal (void), iade (refund). Kısmi tahsilat ve kısmi iade desteklenir.
- **Kalıcılık:** PostgreSQL + EF Core ile veritabanı şeması ve migration'lar.

> **Durum:** Erken aşama, aktif geliştiriliyor.

## Tasarım yaklaşımı

**Katmanlı mimari (Clean Architecture).** İş kuralları (domain), veritabanı, web ve dış servislerden tamamen bağımsız tutulur. Böylece ödeme mantığı, altyapı değişse bile etkilenmez ve kolayca test edilir.

**Kurallar modelin içinde.** Nesnelerin durumu dışarıdan doğrudan değiştirilemez (tüm setter'lar `private`). `Authorize()`, `Capture()`, `Refund()` gibi metotlar kullanılır ve kurala aykırı her istek hata fırlatır. Geçersiz bir nesne hiçbir zaman oluşamaz.

**Durum makinesi (state machine).** `Payment` ve `Merchant` yalnızca izin verilen durum geçişlerini kabul eder. Bilinmeyen her geçiş reddedilir.

**Para asla ondalıklı sayıyla tutulmaz.** Tutarlar en küçük birim (kuruş, cent) cinsinden tam sayı olarak saklanır. `10,50 TL` sistemde `1050` olarak tutulur. Bu, ödeme sistemlerinde yuvarlama hatalarını önlemenin standart yoludur.

**Hata türleri anlamlı.** Girdi yanlışsa `ArgumentException`, nesnenin o anki durumu işleme izin vermiyorsa `InvalidOperationException` fırlatılır.

**Sırlar koda girmez.** Veritabanı bağlantı bilgisi `appsettings.json`'da değil, geliştirmede `user-secrets`'ta tutulur.

**Önce test.** Domain katmanındaki her kural birim testleriyle güvence altında.

## Teknik detaylar

### Teknolojiler

| Alan          | Kullanılan                                          |
|---------------|-----------------------------------------------------|
| Platform      | .NET 10                                             |
| API           | ASP.NET Core Web API                                |
| Veritabanı    | PostgreSQL 17 (Docker)                              |
| ORM           | EF Core + Npgsql, EFCore.NamingConventions (snake_case) |
| Dokümantasyon | OpenAPI + Swagger UI (Swashbuckle)                  |
| Test          | xUnit, coverlet                                     |

### Proje yapısı

```
PSP.slnx
├── PSP.API              → Web API, sistemin dışarıya açılan kapısı
├── PSP.APPLICATION      → Use case'ler (iş akışları)
├── PSP.DOMAIN           → Entity'ler, value object'ler, iş kuralları
├── PSP.INFRASTRUCTURE   → Veritabanı (EF Core, migration'lar) ve dış servisler
└── PSP.DOMAIN.TEST      → Domain birim testleri
```

Bağımlılıklar içe doğrudur. `PSP.DOMAIN` hiçbir projeye bağımlı değildir:

```
API ──► APPLICATION ──► DOMAIN
INFRASTRUCTURE ──► APPLICATION, DOMAIN
API ──► INFRASTRUCTURE   (yalnızca DI kaydı için: AddInfrastructure)
```

### `Money`

Değiştirilemez bir value object (`sealed record`).

- `Amount` (`long`): Tutar, en küçük para birimi cinsinden.
- `Currency` (`string`): 3 harfli, büyük harf ISO 4217 kodu (`TRY`, `USD`, `EUR`). Geçersiz kodlar `ArgumentException` fırlatır.
- `Add(Money)` / `Subtract(Money)`: Sonucu **yeni** bir nesne olarak döner. Farklı para birimlerinde `ArgumentException`, taşmada `OverflowException` fırlatır.
- `IsGreaterThan(Money)`: Karşılaştırma. Farklı para birimlerinde `ArgumentException` fırlatır (100 USD ile 50 TL sessizce karşılaştırılmaz).
- Eşitlik değer bazlıdır: `new Money(100, "TRY") == new Money(100, "TRY")`.

```csharp
var urun   = new Money(1050, "TRY");
var kargo  = new Money(2500, "TRY");
var toplam = urun.Add(kargo);
```

### `Merchant`

| Alan        | Tip              | Kural                                 |
|-------------|------------------|---------------------------------------|
| `Id`        | `Guid`           | Otomatik atanır                       |
| `Name`      | `string`         | Zorunlu, en fazla 100 karakter        |
| `Status`    | `MerchantStatus` | `Active` olarak başlar                |
| `CreatedAt` | `DateTimeOffset` | UTC oluşturulma zamanı                |

Durum geçişleri:

```
            Suspend()
  Active ─────────────► Suspended
         ◄─────────────     │
    │      Activate()       │
    │                       │
    └────── Delete() ───────┴──► Deleted (son durum)
```

Kurala aykırı geçişler (örneğin aktif bir iş yerini tekrar aktifleştirmek ya da silinmiş bir iş yerini geri açmak) `InvalidOperationException` fırlatır.

### `ApiKey`

- Anahtar kriptografik olarak rastgele üretilir ve URL güvenli Base64 (`Base64Url`) ile kodlanır; `+`, `/`, `=` içermez.
- Düz anahtar yalnızca oluşturulduğu anda bir kez döner. Veritabanında **sadece hash'i** (`key_hash`, benzersiz index) ve tanıma amaçlı kısa ön eki tutulur.
- `Verify(candidateKey)`: Boş anahtar ya da iptal edilmiş anahtar için her zaman `false` döner (fail-secure).
- `Revoke()`: Anahtarı iptal eder ve `RevokedAt` zamanını kaydeder. Zaten iptal edilmiş bir anahtarda `InvalidOperationException` fırlatır.

### `Payment`

Bir kart ödemesi. Para önce bloke edilir, sonra tahsil edilir.

| Alan             | Tip              | Açıklama                                   |
|------------------|------------------|--------------------------------------------|
| `Id`             | `Guid`           | Ödemenin sistemdeki kimliği                |
| `MerchantId`     | `Guid`           | Ödemeyi alan iş yeri                       |
| `Amount`         | `Money`          | Bloke edilen tutar, sıfırdan büyük olmalı  |
| `CapturedAmount` | `Money`          | Gerçekte tahsil edilen tutar (0'dan başlar) |
| `RefundedAmount` | `Money`          | Şimdiye kadar iade edilen toplam (0'dan başlar) |
| `Status`         | `PaymentStatus`  | `Pending` olarak başlar                    |
| `CreatedAt`      | `DateTimeOffset` | UTC oluşturulma zamanı                     |

İşlemler:

| Metot              | İzin verilen durum                 | Sonuç                                   |
|--------------------|------------------------------------|-----------------------------------------|
| `Authorize()`      | `Pending`                          | `Authorized` (para bloke edildi)        |
| `Fail()`           | `Pending`                          | `Failed` (banka reddetti)               |
| `Void()`           | `Authorized`                       | `Voided` (bloke kaldırıldı)             |
| `Capture(tutar)`   | `Authorized`                       | `Captured` (bloke tutarın tamamı ya da bir kısmı tahsil edildi) |
| `Refund(tutar)`    | `Captured`, `PartiallyRefunded`    | `PartiallyRefunded` ya da tamamı iade edildiyse `Refunded` |

```
Pending ──Authorize()──► Authorized ──Capture(tutar)──► Captured ──Refund(kısmi)──► PartiallyRefunded
   │                         │                              │                            │
 Fail()                    Void()                     Refund(tamamı)              Refund(kalanı)
   ▼                         ▼                              ▼                            ▼
 Failed                   Voided                         Refunded ◄──────────────────────┘
```

Kurallar:

- Tek seferlik capture: tahsilat bloke tutarı aşamaz, kısmi olabilir, ikinci kez yapılamaz.
- Toplam iade, tahsil edilen tutarı aşamaz. Kalan iade edilebilir tutar = `CapturedAmount − RefundedAmount`.
- Tutar hatalıysa `ArgumentException`, durum uygun değilse `InvalidOperationException`.

```csharp
var payment = new Payment(merchantId, new Money(100000, "TRY")); // 1000 TL bloke isteği
payment.Authorize();
payment.Capture(new Money(80000, "TRY"));  // 800 TL tahsil edildi
payment.Refund(new Money(30000, "TRY"));   // PartiallyRefunded, kalan 500 TL
payment.Refund(new Money(50000, "TRY"));   // Refunded
```

### Veritabanı

- PostgreSQL, tablo ve kolon isimleri `snake_case`: `merchants`, `api_keys`, `payments`.
- Tutarlar `bigint`, para birimi `varchar(3)`, durumlar okunabilir metin olarak (`"Captured"`) saklanır.
- `Money` alanları `OwnsOne` ile aynı tabloda kolonlara açılır (`amount`/`currency`, `captured_amount`/`captured_currency`, `refunded_amount`/`refunded_currency`).
- `api_keys` ve `payments`, `merchants`'a foreign key ile bağlıdır (`ON DELETE RESTRICT`); finansal kayıtlar zincirleme silinmez.
- Entity eşlemeleri `PSP.INFRASTRUCTURE/Persistence/Configurations` altında `IEntityTypeConfiguration` sınıflarıdır.

## Çalıştırma

Gereksinimler: [.NET 10 SDK](https://dotnet.microsoft.com/download), [Docker](https://www.docker.com/), `dotnet-ef` aracı (`dotnet tool install --global dotnet-ef`).

**1. Projeyi indir**

```bash
git clone https://github.com/enesskilicc/payment-gateway.git
cd payment-gateway
```

**2. PostgreSQL'i başlat**

```bash
docker run -d --name paymentgateway-postgres \
  -e POSTGRES_USER=paymentgateway \
  -e POSTGRES_PASSWORD=paymentgateway_dev \
  -e POSTGRES_DB=paymentgateway \
  -p 5433:5432 \
  -v paymentgateway-pgdata:/var/lib/postgresql/data \
  postgres:17
```

> Host portu `5433`'tür; bilgisayarda kurulu bir PostgreSQL varsa `5432`'yi o kullanıyor olabilir. Şifre yalnızca yerel geliştirme içindir.

**3. Bağlantı bilgisini user-secrets'a ekle**

```bash
dotnet user-secrets init --project PSP.API
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5433;Database=paymentgateway;Username=paymentgateway;Password=paymentgateway_dev" --project PSP.API
```

Bağlantı bilgisi yoksa uygulama açılışta hata vererek durur (fail-fast).

**4. Veritabanı şemasını oluştur**

```bash
dotnet ef database update --project PSP.INFRASTRUCTURE --startup-project PSP.API
```

**5. API'yi çalıştır**

```bash
dotnet run --project PSP.API
```

API varsayılan olarak `http://localhost:5183` adresinde açılır (HTTPS profili: `https://localhost:7054`).

| Adres                | Açıklama           |
|----------------------|--------------------|
| `GET /api/health`    | Servis durumu      |
| `/swagger`           | Swagger UI         |
| `/openapi/v1.json`   | OpenAPI dokümanı   |

### Yeni migration eklemek

```bash
dotnet ef migrations add <MigrationAdi> --project PSP.INFRASTRUCTURE --startup-project PSP.API
dotnet ef database update --project PSP.INFRASTRUCTURE --startup-project PSP.API
```

### Testler

```bash
dotnet test PSP.slnx
```

Domain birim testleri; `Money` (doğrulama, toplama, çıkarma, karşılaştırma, taşma, eşitlik), `Merchant` durum geçişleri, `ApiKey` (üretim, doğrulama, iptal, URL güvenliği) ve `Payment` (tüm durum geçişleri, kısmi capture ve kısmi/tam iade) kurallarını kapsar.

## Yol haritası

- [x] Domain: `Money`, `Merchant`, `ApiKey`, `Payment`
- [x] PostgreSQL + EF Core, ilk migration'lar
- [ ] Application katmanı ve ilk ödeme uçları (oluştur, sorgula)
- [ ] API anahtarı ile kimlik doğrulama
- [ ] Idempotency (aynı istek iki kez gelirse ödeme iki kez oluşmaz)
- [ ] Banka simülatörü: timeout, hata ve "belirsiz durum" senaryoları
