# Api.LizeriumServer

- [Назад](BUILDS.ru.md)

## `app_configuration.json`

1. Создайте свой `Api.LizeriumServer/app_configuration.json`
2. В `app_configuration.json` занесите свои пути до
   1. `appHost` - адрес до сервера с админкой
   2. `landingSecret` - секретный ключ для лендинга (не используется)
   3. `admins` - `секретный ключ` и `email` администратора при входе

### Пример конфигурации `app_configuration.json`

```json
{
	"appSettings": {
		"appHost": "localhost:5036",
		"landingSecret": "your_secret_key"
	},
	"admins": [
		{
			"secretKey": "your_secret_key",
			"emailAdmin": "your_email"
		}
	]
}
```

## Импорт в проект списка команд проходит из json в адиминке

- Сами команды динамически подгружаются пользователю в портале

### Пример `CommandsAll.json`

```json
{
	"count": 1,
	"categories": [
		{
			"name": "HOOK",
			"title": [{ "ru": "Общие" }, { "en": "General" }],
			"url_repo": "https://github.com/Lizerium",
			"version": "9.0.0"
		}
	],
	"data": [
		{
			"name": "HOOK",
			"commands": [
				{
					"name": "/set diemsg [visibility]",
					"desc": "Устанавливает видимость сообщений о смерти. Параметры: all, system, self, none.",
					"ex": "",
					"status": 1
				}
			]
		}
	]
}
```
