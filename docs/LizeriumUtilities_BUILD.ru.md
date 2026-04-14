# LizeriumUtilities

- [Назад](BUILDS.ru.md)

## `common_configuration.json`

1. Создайте свой `LizeriumUtilities/common_configuration.json`
2. В `common_configuration.json` занесите свои пути до
   1. `mainDomain` - адрес до домена сервера (нужен при работе почты)
   2. `knownProxies` - массив прокси серверов для работы LizeriumServer при необходимости настроить Docker, Cloudfare и другое

### Пример конфигурации `common_configuration.json`

```json
{
	"appSettings": {
		"mainDomain": "lizup.ru"
	},
	"knownProxies": ["127.0.0.1"]
}
```
