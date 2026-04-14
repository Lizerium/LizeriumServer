# LizeriumEmail

## `email_configuration.json`

1. Создайте свой `LizeriumEmail/email_configuration.json`
2. В `email_configuration.json` занесите свои пути до
   1. `smtpSettings` - свои почты под три категории
   2. Свои почты - админская - `emailAdmin` и поддержка `emailSupport`

### Пример конфигурации `email_configuration.json`

```json
{
	"smtpSettings": [
		{
			"type": "transactional",
			"smtpHost": "host",
			"smtpPort": 587,
			"email": "mail",
			"password": "password"
		},
		{
			"type": "administrator",
			"smtpHost": "host",
			"smtpPort": 587,
			"email": "mail",
			"password": "password"
		},
		{
			"type": "notification",
			"smtpHost": "host",
			"smtpPort": 587,
			"email": "mail",
			"password": "password"
		}
	],
	"displayedName": "Lizerium",
	"emailAdmin": "mail",
	"emailSupport": "mail"
}
```
