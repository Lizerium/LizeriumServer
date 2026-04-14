# LizeriumDatabase

- [Назад](BUILDS.ru.md)

## `database_configuration.json`

1. Создайте свой `LizeriumDatabase/database_configuration.json`
2. В `database_configuration.json` занесите свои пути до
   1. `path` - полный путь до файла с базой данных `application.db` - как правило он генерируется при первом запуске с пустыми таблицами в корне рядом с `.exe` сервера
   2. `private_path` - полный путь до админской базы данных `private.db`
   3. `GifPath` - полный путь до папки с гифками для страницы документации команд

### Пример конфигурации `database_configuration.json`

```json
{
	"path": "your_full_path_to_application.db",
	"private_path": "your_full_path_to_private.db",
	"GifPath": "your_full_path_to_folder_gifs"
}
```
