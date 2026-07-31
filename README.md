<h1 align="center">🌐 Lizerium Server</h1>

<p align="center">
  <b>Modular ASP.NET Core MVC server for game portals, admin panels, documentation, dynamic content, and infrastructure services.</b>
</p>

<p align="center">
  <img src="https://shields.dvurechensky.pro/badge/Platform-Windows%20%7C%20Linux-0078D6?style=for-the-badge" />
  <img src="https://shields.dvurechensky.pro/badge/Backend-ASP.NET%20Core%20MVC-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://shields.dvurechensky.pro/badge/Framework-.NET%206.0-5C2D91?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://shields.dvurechensky.pro/badge/ORM-EF%20Core-6A1B9A?style=for-the-badge" />
  <img src="https://shields.dvurechensky.pro/badge/Architecture-Modular-1565C0?style=for-the-badge" />
  <img src="https://shields.dvurechensky.pro/badge/Status-Open%20Source-00C853?style=for-the-badge" />
</p>

<div align="center" style="margin: 20px 0; padding: 10px; background: #1c1917; border-radius: 10px;">
  <strong>🌐 Language: </strong>
  
  <a href="./README.ru.md" style="color: #F5F752; margin: 0 10px;">
    🇷🇺 Russian
  </a>
  | 
  <span style="color: #0891b2; margin: 0 10px;">
    ✅ 🇺🇸 English (current)
  </span>
</div>

---

> [!NOTE]
> This project is part of the **Lizerium** ecosystem and belongs to the following direction:
>
> - [`Lizerium.Software.Structs`](https://github.com/Lizerium/Lizerium.Software.Structs)
>
> If you are looking for related engineering and supporting tools, start there.

# 📖 About the Project

**Lizerium Server** is a modular server platform designed as the core for a game portal, administrative panel, and supporting infrastructure.

Originally created for the **Lizerium** ecosystem, the project evolved into a broader server platform combining:

- public web portal
- administrative backend
- database integration
- email infrastructure
- internal services
- documentation system
- dynamic content delivery
- network security and filtering
- multilingual content support

The project targets scenarios where you need not just a website, but a **central server node** capable of handling both user-facing and internal administrative processes.

---

- [📖 About the Project](#-about-the-project)
- [✨ Features](#-features)
  - [🌍 Public Layer](#-public-layer)
  - [🛠 Admin Layer](#-admin-layer)
  - [⚙️ Infrastructure Features](#️-infrastructure-features)
- [🧱 Architecture](#-architecture)
- [📦 Solution Structure](#-solution-structure)
  - [`LizeriumServer`](#lizeriumserver)
  - [`Api.LizeriumServer`](#apilizeriumserver)
  - [`LizeriumDatabase`](#lizeriumdatabase)
  - [`LizeriumEmail`](#lizeriumemail)
  - [`LizeriumLogging`](#lizeriumlogging)
  - [`LizeriumNetSecurity`](#lizeriumnetsecurity)
  - [`LizeriumUtilities`](#lizeriumutilities)
  - [`TranslationService`](#translationservice)
- [🚀 Quick Start](#-quick-start)
  - [Documents](#documents)
  - [Clone repository](#clone-repository)
  - [Build solution](#build-solution)
  - [Run main server](#run-main-server)
  - [Run admin backend](#run-admin-backend)
- [🧪 Testing](#-testing)
  - [Test projects](#test-projects)
    - [Run tests](#run-tests)
- [🧰 Scripts](#-scripts)
  - [Examples](#examples)
- [🔗 Related Projects](#-related-projects)
- [📜 Changelog](#-changelog)
- [⚖️ License](#️-license)
- [💬 Notes](#-notes)

---

# ✨ Features

## 🌍 Public Layer

- Project landing page
- Informational and service pages
- News and publication system
- Domain-aware SEO for multiple public domains
- User request display
- Freelancer (2003) documentation
- Crafting recipes visualization
- Multilingual support:
  - Russian
  - English

---

## 🛠 Admin Layer

- Separate administrative backend
- Admin authentication
- Dynamic command loading from JSON
- Internal data management
- Support for local or external translation services

---

## ⚙️ Infrastructure Features

- Modular architecture
- Separation into independent service libraries
- External configuration support
- Centralized logging
- Email service
- Database abstraction layer
- Utility modules
- Reverse proxy / trusted proxy support
- Dynamic `robots.txt`, `sitemap.xml`, canonical and OpenGraph URLs from configuration
- DoS protection
- Support for external directories (mods, launcher, game data)

---

# 🧱 Architecture

The project is divided into independent components, each responsible for a specific domain of server logic.

---

# 📦 Solution Structure

## `LizeriumServer`

Main server project.

Responsible for:

- public portal
- routing
- user-facing pages
- game data
- documentation
- service integration

---

## `Api.LizeriumServer`

Administrative backend.

Responsible for:

- internal panel
- configuration data
- admin workflows
- dynamic content management

---

## `LizeriumDatabase`

Database module.

Used for:

- primary database
- private database
- project data storage

---

## `LizeriumEmail`

Email module.

Used for:

- notifications
- system emails
- administrative communication

---

## `LizeriumLogging`

Logging module.

Used for:

- centralized logging
- diagnostics
- server maintenance

---

## `LizeriumNetSecurity`

Network and security layer.

Used for:

- filtering
- protection mechanisms
- network infrastructure handling

---

## `LizeriumUtilities`

Utility module.

Used for:

- helper logic
- shared extensions
- configuration helpers

---

## `TranslationService`

Text translation service.

Used for:

- multilingual content
- integration with local or external translators

---

# 🚀 Quick Start

## Documents

- [BUILDS](docs/BUILDS.md)

## Clone repository

```bash
git clone https://github.com/Lizerium/LizeriumServer.git
cd LizeriumServer
```

## Build solution

```bash
dotnet build LizeriumServer.sln
```

## Run main server

```bash
dotnet run --project src/LizeriumServer
```

## Run admin backend

```bash
dotnet run --project src/Api.LizeriumServer
```

> [!IMPORTANT]
> Configuration files and infrastructure dependencies must be prepared before running.

📄 Full setup documentation is available in the [`docs`](docs) folder.

---

# 🧪 Testing

The project includes both unit and integration tests.

## Test projects

- `Lizerium.DDoS.Tests`
- `LizeriumDatabase.Tests`
- `LizeriumServer.Tests`
- `LizeriumServer.IntegrationTests`
- `TranslationService.Tests`

### Run tests

```bash
dotnet test
```

---

# 🧰 Scripts

The project includes helper scripts for processing and preparing game data.

## Examples

- payload translation
- crafting JSON translation
- reconfiguration utilities

📄 More details: [`scripts`](scripts)

---

# 🔗 Related Projects

- [https://github.com/Lizerium/KnowledgeBase](https://github.com/Lizerium/KnowledgeBase)
- [https://github.com/Lizerium/LizeriumModManager](https://github.com/Lizerium/LizeriumModManager)

---

# 📜 Changelog

See: [`CHANGELOG`](CHANGELOG.md)

---

# ⚖️ License

Distributed under the license specified in [`LICENSE`](LICENSE)

---

# 💬 Notes

This project is not just a website, but the result of a long engineering evolution toward a modular server platform.

It retains practical use while being gradually transformed into a cleaner, more open, and extensible architecture.
