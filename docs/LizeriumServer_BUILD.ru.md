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
	}
}
```

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
