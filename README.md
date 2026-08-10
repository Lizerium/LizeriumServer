# Lizerium Server

English version | [Русская версия](README.ru.md)

Lizerium Server is the server-side foundation of my Lizerium ecosystem. It is not a standalone showcase website; it is a working set of ASP.NET Core MVC projects and libraries that power the public portal, admin panel, news system, product catalog, documentation, and supporting infrastructure around Lizerium.

This project is part of [`Lizerium.Software.Structs`](https://github.com/Lizerium/Lizerium.Software.Structs). That link matters: this repository contains the server implementation, while `Lizerium.Software.Structs` describes the broader structure, tools, and related ecosystem work.

## What is included

### Public server

`src/LizeriumServer` handles the public-facing part:

- Lizerium portal home page;
- news section with cards, filters, videos, Markdown articles, galleries, and fullscreen image viewing;
- games and products section with categories, descriptions, download links, icons, and backgrounds;
- Freelancer documentation and internal project knowledge pages;
- community, support, and service pages;
- RU/EN localization;
- multi-domain SEO with canonical URLs, OpenGraph, `robots.txt`, and `sitemap.xml`;
- external roots for mods, launcher files, knowledge base content, and game data.

### Administrative API server

`src/Api.LizeriumServer` handles the closed admin area:

- admin login and protected admin routes;
- post and news management;
- closed news preview before publication;
- upload, search, preview, insert, attach, detach, and delete flows for news images;
- management for news types, covers, icons, video links, GitHub links, likes, publication dates, and ordering;
- product catalog management: categories, products, download sources, active flags, sort order, RU/EN names and descriptions;
- image libraries for news and products;
- command and command-translation management;
- its own TypeScript, CSS, and SCSS build pipeline through Grunt.

### Shared libraries

The solution is split into several projects:

- `LizeriumDatabase` - database and application data layer;
- `LizeriumEmail` - email and system notifications;
- `LizeriumLogging` - logging and diagnostics;
- `LizeriumNetSecurity` - network limits, filtering, and protection logic;
- `LizeriumUtilities` - shared utilities, configuration helpers, and common models;
- `TranslationService` - translation service for multilingual content.

## Frontend and build pipeline

The repository has two separate frontend build flows:

- the main server builds TypeScript and SCSS through Webpack/Grunt inside `src/LizeriumServer`;
- the API server builds TypeScript, CSS, and page-level SCSS through Grunt inside `src/Api.LizeriumServer`.

Client source code lives in `ScriptsAndCss/TypeScripts`, and generated JavaScript is emitted into `ScriptsAndCss/JsScripts`. API page styles are being moved out of Razor views into dedicated files under `ScriptsAndCss/CssFiles/pages/*.scss`.

## Documentation

Build documentation:

- [Build document index](docs/BUILDS.md)
- [Lizerium Server](docs/LizeriumServer_BUILD.md)
- [Api Lizerium Server](docs/Api.LizeriumServer_BUILD.md)
- [LizeriumDatabase](docs/LizeriumDatabase_BUILD.md)
- [LizeriumEmail](docs/LizeriumEmail_BUILD.md)
- [LizeriumUtilities](docs/LizeriumUtilities_BUILD.md)

## Development quick start

```powershell
dotnet build LizeriumServer.sln
```

Main server:

```powershell
dotnet run --project src/LizeriumServer
```

API admin server:

```powershell
dotnet run --project src/Api.LizeriumServer
```

Main server frontend build:

```powershell
cd src/LizeriumServer
npx.cmd grunt build
```

API server frontend build:

```powershell
cd src/Api.LizeriumServer
npx.cmd grunt all
```

## Tests

The solution includes tests for the database layer, public server, integration flows, network protection, and translation service.

```powershell
dotnet test
```

## Related projects

- [Lizerium.Software.Structs](https://github.com/Lizerium/Lizerium.Software.Structs)
- [KnowledgeBase](https://github.com/Lizerium/KnowledgeBase)
- [LizeriumModManager](https://github.com/Lizerium/LizeriumModManager)

## Changelog and credits

- [CHANGELOG.md](CHANGELOG.md)
- [CREDITS.md](CREDITS.md)
- [LICENSE](LICENSE)

The project is developed as a practical server base for Lizerium: the public portal, admin area, and infrastructure should stay connected while gradually moving toward a cleaner and more maintainable architecture.
