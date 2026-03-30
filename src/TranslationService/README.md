# Проект-реализация 🐥 разных переводчиков

## Libretranslate

### 3️⃣ Регистрация в DI

> .NET 8 / ASP.NET Core:

```csharp
builder.Services.AddHttpClient<ITranslationService, LibreTranslate>(client =>
{
    client.BaseAddress = new Uri("http://192.168.64.128:5001");
});
```

> Использование:

```csharp
var translated = await translationService.TranslateAsync("Привет", "ru", "en");
```

---
