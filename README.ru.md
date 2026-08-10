# Lizerium Server

Русская версия | [English](README.md)

Lizerium Server - серверная часть моей экосистемы Lizerium. Это не отдельный демонстрационный сайт, а рабочий набор ASP.NET Core MVC проектов и библиотек, который обслуживает публичный портал, административную панель, новости, каталог продуктов, документацию и инфраструктурные задачи вокруг Lizerium.

Проект является частью направления [`Lizerium.Software.Structs`](https://github.com/Lizerium/Lizerium.Software.Structs). Для меня это важная связка: здесь лежит серверная реализация, а `Lizerium.Software.Structs` описывает более широкий контекст структуры, инструментов и связанных решений экосистемы.

## Что входит в проект

### Публичный сервер

Проект `src/LizeriumServer` отвечает за пользовательскую часть:

- главная страница портала Lizerium;
- раздел новостей с карточками, фильтрами, видео, Markdown-статьями, галереями и полноэкранным просмотром изображений;
- раздел игр и продуктов с категориями, описаниями, ссылками на загрузку, иконками и фонами;
- документация по Freelancer и внутренним материалам проекта;
- страницы сообщества, поддержки и служебные страницы;
- мультиязычность RU/EN;
- SEO под несколько доменов, включая canonical URL, OpenGraph, `robots.txt` и `sitemap.xml`;
- поддержка внешних директорий для модов, лаунчера, базы знаний и игровых данных.

### Административный API-сервер

Проект `src/Api.LizeriumServer` отвечает за закрытую административную часть:

- вход в админку и защита административных маршрутов;
- управление постами и новостями;
- закрытый предпросмотр новостей до публикации;
- загрузка, поиск, предпросмотр, вставка, прикрепление, открепление и удаление изображений новостей;
- управление типами новостей, обложками, иконками, видео-ссылками, GitHub-ссылками, лайками, датой публикации и порядком;
- управление каталогом продуктов: категории, продукты, источники загрузки, активность, порядок, RU/EN названия и описания;
- галереи изображений для новостей и продуктов;
- управление командами и переводами команд;
- собственная сборка TypeScript, CSS и SCSS через Grunt.

### Общие библиотеки

Решение разделено на несколько проектов:

- `LizeriumDatabase` - слой работы с базами данных и данными приложения;
- `LizeriumEmail` - отправка почты и системных уведомлений;
- `LizeriumLogging` - логирование и диагностика;
- `LizeriumNetSecurity` - сетевые ограничения, фильтрация и защитная логика;
- `LizeriumUtilities` - общие утилиты, конфигурационные помощники и вспомогательные модели;
- `TranslationService` - сервис перевода для мультиязычного контента.

## Фронтенд и сборка

В проекте есть две отдельные клиентские сборки:

- основной сервер собирает TypeScript и SCSS через Webpack/Grunt внутри `src/LizeriumServer`;
- API-сервер собирает TypeScript, CSS и page-level SCSS через Grunt внутри `src/Api.LizeriumServer`.

Исходники клиентской логики лежат в `ScriptsAndCss/TypeScripts`, а сгенерированные JavaScript-файлы попадают в `ScriptsAndCss/JsScripts`. Стили API-страниц постепенно выносятся из Razor в тематические файлы `ScriptsAndCss/CssFiles/pages/*.scss`.

## Документация

Основные документы по сборке:

- [Общий список документов сборки](docs/BUILDS.ru.md)
- [Lizerium Server](docs/LizeriumServer_BUILD.ru.md)
- [Api Lizerium Server](docs/Api.LizeriumServer_BUILD.ru.md)
- [LizeriumDatabase](docs/LizeriumDatabase_BUILD.ru.md)
- [LizeriumEmail](docs/LizeriumEmail_BUILD.ru.md)
- [LizeriumUtilities](docs/LizeriumUtilities_BUILD.ru.md)

## Быстрый запуск для разработки

```powershell
dotnet build LizeriumServer.sln
```

Основной сервер:

```powershell
dotnet run --project src/LizeriumServer
```

API-админка:

```powershell
dotnet run --project src/Api.LizeriumServer
```

Клиентская сборка основного сервера:

```powershell
cd src/LizeriumServer
npx.cmd grunt build
```

Клиентская сборка API-сервера:

```powershell
cd src/Api.LizeriumServer
npx.cmd grunt all
```

## Тесты

В решении есть тестовые проекты для базы данных, основного сервера, интеграционных сценариев, защиты и сервиса перевода.

```powershell
dotnet test
```

## Связанные проекты

- [Lizerium.Software.Structs](https://github.com/Lizerium/Lizerium.Software.Structs)
- [KnowledgeBase](https://github.com/Lizerium/KnowledgeBase)
- [LizeriumModManager](https://github.com/Lizerium/LizeriumModManager)

## История изменений и авторство

- [CHANGELOG.ru.md](CHANGELOG.ru.md)
- [CREDITS.md](CREDITS.md)
- [LICENSE](LICENSE)

Проект развивается как практическая серверная база для Lizerium: публичный портал, админка и инфраструктура должны оставаться связанными, но при этом постепенно приводиться к более чистой и поддерживаемой архитектуре.
