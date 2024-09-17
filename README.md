# LogStreamMiddleware

`LogStreamMiddleware`, ASP.NET Core uygulamalarınızda HTTP isteklerini ve yanıtlarını Serilog ve Seq kullanarak loglamanızı sağlayan bir middleware'dir. Ayrıca hassas verileri (TC kimlik numarası, telefon numarası, kredi kartı numarası, parola) maskeler.
Bu middleware, entegrasyonu kolay ve güçlü loglama yetenekleri sunar.

## Özellikler

- Gelen HTTP isteklerini metod ve yol bilgileriyle loglar.
- Çıkan HTTP yanıtlarını durum kodu ile loglar.
- TC kimlik numarası, telefon numarası, kredi kartı numarası ve parola gibi hassas verileri maskeleme
- Serilog ve Seq ile kolay entegrasyon sağlar.
- NuGet üzerinden projenize kolayca eklenebilir.

## Kurulum

NuGet paketi olarak kurmak için aşağıdaki komutu kullanabilirsiniz:

```bash
dotnet add package LogStreamMiddleware --version 1.1.0
```

## Kullanımı
```csharp
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Middleware'i Seq URL ile kullanın
app.UseRequestLogging("http://localhost:5341");

app.MapGet("/", () => "Merhaba Dünya!");

app.Run();
```