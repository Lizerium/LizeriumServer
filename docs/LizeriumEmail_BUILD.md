# LizeriumEmail

- [Назад](BUILDS.md)

## `email_configuration.json`

1. Create your own `LizeriumEmail/email_configuration.json`
2. In `email_configuration.json`, configure the following:
   1. `smtpSettings` — email configurations for three categories:
      - transactional
      - administrator
      - notification

   2. Define your email addresses:
      - `emailAdmin` — administrator email
      - `emailSupport` — support email

---

### Example `email_configuration.json`

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
