# LizeriumServer

- [Назад](BUILDS.ru.md)

## `appsettings.json`

1. Создайте свой `LizeriumServer/appsettings.json`
2. В `appsettings.json` занесите свои пути до
   1. `KnowledgeBase` - Базы знаний, взять её можно отсюда - https://github.com/Lizerium/KnowledgeBase
   2. `LauncherRoot` - До папки с загрузчиком
   3. `ModsRoot` - До папки с модами | играми
   4. `GameServerConfigs` - эта папка содержит в себе папку `BUILDS` - в ней рецепты для крафта, такие файлы делает мой генератор из проекта `https://github.com/Lizerium/LizeriumModManager`
      1. Внутри этой папки ваши
         1. craft_builda.json
         2. craft_builde.json
         3. craft_buildl.json
         4. craft_buildw.json
   5. `BlackList` - До файла с запрещёнными IP адресами
   6. `GoogleReCaptcha` - ключи для GoogleReCaptcha под ваш сайт
   7. `SeoDomains` - публичные домены и режим генерации SEO для одного экземпляра сервера

### Пример конфигурации `appsettings.json`

```json
{
	"Logging": {
		"LogLevel": {
			"Default": "Information",
			"Microsoft.AspNetCore": "Warning"
		}
	},
	"AllowedHosts": "*",
	"BlackList": "full_path_to_file_46722929.ini",
	"GoogleReCaptcha": {
		"SiteKey": "your_SiteKey",
		"SecretKey": "your_SecretKey",
		"Version": "v3"
	},
	"StoragePaths": {
		"ModsRoot": "full_path_to_PROJECTS_UPDATES",
		"LauncherRoot": "full_path_to_LAUNCHER_RELEASE",
		"KnowledgeBase": "full_path_to_KnowledgeBase",
		"GameServerConfigs": "full_path_to_GameServerConfigs"
	},
	"SeoDomains": {
		"PrimaryDomain": "lizerium.com",
		"Domains": ["lizerium.com", "lizup.ru"],
		"Scheme": "https",
		"CanonicalMode": "RequestHost",
		"OpenGraphImage": "/img/Main.png",
		"SiteName": "Lizerium"
	}
}
```

## SEO для нескольких доменов

`LizeriumServer` может обслуживать несколько публичных доменов из одного развёрнутого проекта. Приложение читает host текущего запроса и через `SeoDomains` генерирует SEO-данные под нужный домен.

От текущего домена генерируются:

- canonical URL
- OpenGraph URL
- OpenGraph и VK preview image URL
- JSON-LD breadcrumb URL
- `/robots.txt`
- `/sitemap.xml`
- CORS origins для настроенных публичных доменов

### Параметры `SeoDomains`

```json
{
	"SeoDomains": {
		"PrimaryDomain": "lizerium.com",
		"Domains": ["lizerium.com", "lizup.ru"],
		"Scheme": "https",
		"CanonicalMode": "RequestHost",
		"OpenGraphImage": "/img/Main.png",
		"SiteName": "Lizerium"
	}
}
```

- `PrimaryDomain` - главный домен, который используется как fallback и как canonical-домен при режиме `PrimaryDomain`.
- `Domains` - все публичные домены, которые могут вести на этот же сервер.
- `Scheme` - публичная схема для абсолютных ссылок, обычно `https`.
- `CanonicalMode` - стратегия canonical URL:
  - `RequestHost` означает, что каждый настроенный домен генерирует SEO-ссылки сам на себя. Это подходит, если нужно индексировать и `lizerium.com`, и `lizup.ru`.
  - `PrimaryDomain` означает, что все домены в canonical указывают на `PrimaryDomain`. Это подходит, если нужно собрать SEO-вес на одном основном домене.
- `OpenGraphImage` - путь к картинке для предпросмотра в соцсетях. Относительный путь разворачивается в абсолютный URL текущего домена.
- `SiteName` - название сайта для social metadata.

### Рекомендуемая настройка доменов

Одного экземпляра приложения достаточно. Не нужно дублировать проект под каждый домен.

1. Направьте DNS-записи всех доменов на один сервер.
2. Добавьте все домены в nginx, например `lizerium.com`, `www.lizerium.com` и `lizup.ru`.
3. Проксируйте все домены в один и тот же ASP.NET Core процесс.
4. Выпустите HTTPS-сертификаты для всех публичных доменов.
5. Оставьте включённым `UseForwardedHeaders`, чтобы приложение получало корректные внешние scheme и host от reverse proxy.
6. Обновите реальный production `appsettings.json`, добавив `SeoDomains`.

Примерная схема nginx:

```nginx
server {
    listen 443 ssl http2;
    server_name lizerium.com www.lizerium.com lizup.ru;

    location / {
        proxy_pass http://127.0.0.1:7176;
        proxy_set_header Host $host;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Host $host;
    }
}
```

После деплоя проверьте оба домена отдельно:

```bash
curl -I https://lizerium.com/
curl https://lizerium.com/robots.txt
curl https://lizerium.com/sitemap.xml
curl -I https://lizup.ru/
curl https://lizup.ru/robots.txt
curl https://lizup.ru/sitemap.xml
```

При `CanonicalMode: "RequestHost"` файл `lizerium.com/sitemap.xml` должен содержать `https://lizerium.com/...`, а `lizup.ru/sitemap.xml` должен содержать `https://lizup.ru/...`.

> [!IMPORTANT]
> Боевые конфиги намеренно не попадают в deploy-пакет. Скрипты исключают `appsettings*.json`, `downloads.json`, `dev_mode.json`, базы данных, логи и контентные папки. Реальный конфиг на сервере нужно обновить до рестарта сервиса.

### Пример конфигураций `craft_builda.json`, `craft_builde.json`, `craft_buildl.json`, `craft_buildw.json`

```json
{
	"nameFile": "builda",
	"translationsNameCategory": {
		"ru": "Амуниция и сложные компоненты",
		"en": "Ammunition and complex components"
	},
	"total": "1",
	"date": "2026-06-30T10:02:06.5304022+03:00",
	"components": [
		{
			"nickname": "nuclear_missile_bomber_ammo",
			"translationsNameComponent": {
				"ru": "Ядерная торпеда",
				"en": "Nuclear torpedo"
			},
			"count": "1",
			"components": [
				{
					"nickname": "commodity_nomad_generator",
					"translationsNameComponent": {
						"ru": "Генератор Странников",
						"en": "Wanderers generator"
					},
					"count": "10",
					"components": []
				}
			]
		}
	]
}
```

## `dev_mode.json`

1. Создайте свой `LizeriumServer/dev_mode.json`
2. В `dev_mode.json` занесите свои пути до
   1. `DevelopMode` - включает режим разработки, это когда сайт висит в сети глобальной - вы лично можете взаимодействовать с ним по доверенному списку `UpdaterWhiteList` а другие видят баннер о его недоступности
   2. `UpdaterState` - разрешает работать загрузчикам обновлений
   3. `UpdaterDevMode` - ограничивает работу всем загрузчикам обновлений пользователей кроме доверенного списка `UpdaterWhiteList`

### Пример конфигурации `dev_mode.json`

```json
{
	"DevelopMode": false,
	"UpdaterState": true,
	"UpdaterDevMode": true,
	"UpdaterWhiteList": ["your_global_ip_address", "::1"]
}
```

## `downloads.json`

1. Создайте свой `LizeriumServer/downloads.json`
2. В `downloads.json` занесите свои пути и ключи до того, что вы объявили или объявите в [Game.cshtml](../src/LizeriumServer/Views/Home/Game.cshtml)

### Пример конфигурации `downloads.json`

```json
{
	"steam": {
		"type": "local",
		"value": "LizeriumSteam_1.0.6_Win_7_10_11_x86_x64_install_ru_en.exe"
	},
	"lizerium_game": {
		"type": "external",
		"value": "https://disk.yandex.ru/d/KucilXa6eUx-Yw"
	},
	"freelancer_game": {
		"type": "external",
		"value": "https://disk.yandex.ru/d/tg8lbbVrJ1kNCg"
	}
}
```
