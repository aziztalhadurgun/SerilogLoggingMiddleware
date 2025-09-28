# ADWorks.Logging

`ADWorks.Logging`, ASP.NET Core uygulamalarınızda HTTP isteklerini ve yanıtlarını Serilog ve Seq kullanarak loglamanızı sağlayan bir middleware'dir. Ayrıca hassas verileri (TC kimlik numarası, telefon numarası, kredi kartı numarası, parola) maskeler.
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
dotnet add package ADWorks.Logging --version 1.0.0
```

## Kullanımı
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilogLogging(builder.Configuration);

var app = builder.Build();

app.UseCorrelationId();
app.UseRequestResponseLogging();

app.MapGet("/", () => "Merhaba Dünya!");

app.Run();
```

## Yapılandırma

`appsettings.json` dosyanıza aşağıdaki gibi bir yapı ekleyebilirsiniz:

```json
"Logging": {
  "Handlers": {
    "Console": { "Enabled": true },
    "File": { "Enabled": true, "Path": "logs/app.log" },
    "Seq": { "Enabled": true, "ServerUrl": "http://localhost:5341", "ApiKey": "" },
    "Database": {
      "Enabled": false,
      "Type": "MSSQL",
      "ConnectionString": "",
      "TableName": "Logs"
    }
  }
}
```