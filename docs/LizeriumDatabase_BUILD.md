# LizeriumDatabase

- [Назад](BUILDS.md)

## `database_configuration.json`

1. Create your own `LizeriumDatabase/database_configuration.json`
2. In `database_configuration.json`, specify the following paths:
   1. `path` — full path to the main database file `application.db`
      - typically generated on first server run with empty tables in the same directory as the server `.exe`

   2. `private_path` — full path to the administrative database `private.db`

   3. `GifPath` — full path to the folder containing GIFs used on the command documentation page

---

### Example `database_configuration.json`

```json
{
	"path": "your_full_path_to_application.db",
	"private_path": "your_full_path_to_private.db",
	"GifPath": "your_full_path_to_folder_gifs"
}
```
