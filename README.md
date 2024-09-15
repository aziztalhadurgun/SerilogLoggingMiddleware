# LogStreamMiddleware

`LogStreamMiddleware`, ASP.NET Core uygulamalarınızda HTTP isteklerini ve yanıtlarını Serilog ve Seq kullanarak loglamanızı sağlayan bir middleware'dir. Bu middleware, entegrasyonu kolay ve güçlü loglama yetenekleri sunar.

## Özellikler

- Gelen HTTP isteklerini metod ve yol bilgileriyle loglar.
- Çıkan HTTP yanıtlarını durum kodu ile loglar.
- Serilog ve Seq ile kolay entegrasyon sağlar.
- NuGet üzerinden projenize kolayca eklenebilir.

## Kurulum

`LogStreamMiddleware` paketini NuGet Package Manager Console veya doğrudan proje dosyanıza ekleyerek yükleyebilirsiniz.

### NuGet Package Manager Console Kullanarak

```bash
Install-Package LogStreamMiddleware
