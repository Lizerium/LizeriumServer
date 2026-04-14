# Api.LizeriumServer

## `app_configuration.json`

1. Create your own `Api.LizeriumServer/app_configuration.json`
2. In `app_configuration.json`, define the following:
   1. `appHost` — address of the admin server
   2. `landingSecret` — secret key for the landing page (currently unused)
   3. `admins` — administrator credentials:
      - `secretKey`
      - `emailAdmin`

---

### Example `app_configuration.json`

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

---

## Command List Import

- Commands are imported into the admin panel from a JSON file
- They are then dynamically loaded and displayed to users in the portal

---

### Example `CommandsAll.json`

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
					"desc": "Sets visibility of death messages. Options: all, system, self, none.",
					"ex": "",
					"status": 1
				}
			]
		}
	]
}
```
