# LizeriumUtilities

## `common_configuration.json`

1. Create your own `LizeriumUtilities/common_configuration.json`
2. In `common_configuration.json`, configure the following:
   1. `mainDomain` — server domain address
      - used for email-related functionality

   2. `knownProxies` — list of proxy servers
      - used when running behind Docker, Cloudflare, or other reverse proxy setups

---

### Example `common_configuration.json`

```json
{
	"appSettings": {
		"mainDomain": "lizup.ru"
	},
	"knownProxies": ["127.0.0.1"]
}
```
