# payment-gateway

Üye iş yerlerinin (merchant) müşterilerinden ödeme alabilmesini sağlayan bir **ödeme hizmet sağlayıcısı (PSP – Payment Service Provider)** altyapısı.

## Proje ne yapıyor?

Bir e-ticaret sitesi ya da uygulama, kart ödemesi almak istediğinde bankalarla tek tek entegre olmak yerine bir ödeme sağlayıcısına bağlanır. Ödeme sağlayıcısı arada durur. İş yerlerini kaydeder ve yönetir, gelen ödeme isteklerini doğrular, tutarları doğru para birimiyle işler ve her işlemin durumunu takip eder.

Bu proje böyle bir sistemin çekirdeğini sıfırdan inşa ediyor. Hedef, gerçek bir ödeme sisteminin ihtiyaç duyduğu kuralları (para hesaplarında hata yapmamak, geçersiz durum geçişlerine izin vermemek, her kuralı testle güvence altına almak) en baştan doğru kurmak.

Şu ana kadar sistemin iki temel kavramı hazır:

- **Para (`Money`):** Sistemde dolaşan her tutar, para birimiyle birlikte tek bir nesne olarak taşınır. Böylece TL ile doları yanlışlıkla toplamak ya da kuruş kaybetmek mümkün olmaz.
- **Üye iş yeri (`Merchant`):** Ödeme alan firmayı temsil eder. Bir iş yeri aktif olabilir, askıya alınabilir ya da silinebilir. Bu geçişler belirli kurallara bağlıdır.

> **Durum:** Erken aşama, aktif geliştiriliyor.

## Tasarım yaklaşımı

**Katmanlı mimari (Clean Architecture).** İş kuralları (domain), veritabanı, web ve dış servislerden tamamen bağımsız tutulur. Böylece ödeme mantığı, altyapı değişse bile etkilenmez ve kolayca test edilir.

**Kurallar modelin içinde.** Bir iş yerinin durumu dışarıdan doğrudan değiştirilemez. `Suspend()`, `Activate()`, `Delete()` gibi metotlar kullanılır ve kurala aykırı her istek hata fırlatır. Geçersiz bir nesne hiçbir zaman oluşamaz.

**Para asla ondalıklı sayıyla tutulmaz.** Tutarlar en küçük birim (kuruş, cent) cinsinden tam sayı olarak saklanır. `10,50 TL` sistemde `1050` olarak tutulur. Bu, ödeme sistemlerinde yuvarlama hatalarını önlemenin standart yoludur.

**Önce test.** Domain katmanındaki her kural birim testleriyle güvence altında.

## Teknik detaylar

### Teknolojiler

| Alan        | Kullanılan                               |
|-------------|------------------------------------------|
| Platform    | .NET 10                                  |
| API         | ASP.NET Core Web API                     |
| Dokümantasyon | OpenAPI + Swagger UI (Swashbuckle)     |
| Test        | xUnit, coverlet                          |

### Proje yapısı

```
PSP.slnx
├── PSP.API              → Web API, sistemin dışarıya açılan kapısı
├── PSP.APPLICATION      → Use case'ler (iş akışları)
├── PSP.DOMAIN           → Entity'ler, value object'ler, iş kuralları
├── PSP.INFRASTRUCTURE   → Veritabanı ve dış servis entegrasyonları
└── PSP.DOMAIN.TEST      → Domain birim testleri
```

Bağımlılıklar içe doğrudur. `PSP.DOMAIN` hiçbir projeye bağımlı değildir:

```
API ──► APPLICATION ──► DOMAIN
INFRASTRUCTURE ──► APPLICATION, DOMAIN
```

### `Money`

Değiştirilemez bir value object (`sealed record`).

- `Amount` (`long`): Tutar, en küçük para birimi cinsinden.
- `Currency` (`string`): 3 harfli, büyük harf ISO 4217 kodu (`TRY`, `USD`, `EUR`). Geçersiz kodlar `ArgumentException` fırlatır.
- `Add(Money)`: Toplamı **yeni** bir nesne olarak döner. Farklı para birimlerinde `ArgumentException`, taşmada `OverflowException` fırlatır.
- Eşitlik değer bazlıdır: `new Money(100, "TRY") == new Money(100, "TRY")`.

```csharp
var urun  = new Money(1050, "TRY");
var kargo = new Money(2500, "TRY");
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

## Çalıştırma

Gereksinim: [.NET 10 SDK](https://dotnet.microsoft.com/download)

```bash
git clone https://github.com/enesskilicc/payment-gateway.git
```

```bash
cd payment-gateway
```

```bash
dotnet run --project PSP.API
```

API varsayılan olarak `http://localhost:5183` adresinde açılır (HTTPS profili: `https://localhost:7054`).

| Adres                                   | Açıklama              |
|-----------------------------------------|-----------------------|
| `GET /api/health`                       | Servis durumu         |
| `/swagger`                              | Swagger UI            |
| `/openapi/v1.json`                      | OpenAPI dokümanı      |

### Testler

```bash
dotnet test PSP.slnx
```

25 birim testi; para birimi doğrulaması, toplama, taşma, eşitlik, iş yeri oluşturma kuralları ve tüm durum geçişlerini kapsar.
