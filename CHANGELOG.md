<div align="center" style="margin: 20px 0; padding: 10px; background: #1c1917; border-radius: 10px;">
  <strong>🌐 Language: </strong>
  
  <a href="./CHANGELOG.ru.md" style="color: #F5F752; margin: 0 10px;">
    🇷🇺 Russian
  </a>
  | 
  <span style="color: #0891b2; margin: 0 10px;">
    ✅ 🇺🇸 English (current)
  </span>
</div>

# Updates

## 01.08.2026

### Lizerium Server

- Added multi-domain SEO configuration through `SeoDomains` for running one server on `lizerium.com` and `lizup.ru` without duplicating the project.
- Added dynamic canonical, OpenGraph, VK preview and JSON-LD URL generation based on the configured SEO domain mode.
- Added host-aware `/sitemap.xml` and `/robots.txt` generation: each domain now receives sitemap and robots links for its own host while page canonical URLs can still point to the primary domain.
- Added `appsettings.example.json` documentation for `PrimaryDomain`, `Domains`, `Scheme`, `CanonicalMode`, `OpenGraphImage` and `SiteName`.
- Updated deployment and build documentation with nginx proxy requirements, including forwarded host headers for multi-domain hosting.

## 31.07.2026

### Lizerium Server

- Added a database-backed Lizerium Launcher news feed on `/Home/Launcher` with search, newest/oldest sorting, pagination and localized RU/EN content.
- Added `launcher_news` storage support: automatic table creation, missing-column repair, initial seed news, published/admin queries, save/delete operations and public like counter updates.
- Added the `LauncherNewsDataResponse` model with RU/EN titles and Markdown, YouTube/Rutube/VK links, cover image, gallery, product icon, news type, GitHub link, publication status, sort order and publication date.
- Added a full news reader with Markdown rendering, image galleries, cover/product icons, GitHub links, public likes, localized share/copy flow and navigation controls.
- Added Markdown media support for inline images and embedded videos through `@video(...)`, `@video-vertical(...)` and standalone YouTube/Rutube/VK links.
- Added locale-aware video platform priority: Russian prefers Rutube/VK/YouTube, English prefers YouTube/VK/Rutube, with manual platform switching and blocked-platform messages.
- Added RSS endpoints for launcher news: `/news/rss.xml` and `/rss/news.xml`.
- Added integration/regression coverage for Markdown rendering, launcher scripts and news test data.
- Updated launcher styles, mobile adaptation and localized resource strings for the new news experience.

### Api Lizerium Server

- Added a dedicated admin page for launcher news management with search and compact edit cards.
- Added create/edit/delete/publish/sort controls for news posts, including RU/EN title, Markdown, news type, YouTube/Rutube/VK URLs, GitHub metadata and publication date.
- Added separate upload flows for product icons, cover images, galleries and pasted Markdown images; uploaded files are validated as images and limited to 8 MB.
- Added admin-side previews for product icons, covers, galleries and inline Markdown images, plus client-side paste/upload helpers in `news.js`.
- Added `admin_access.json` and `AdminAccessGuard` for optional trusted-IP access control.
- Added `AccessClosed` view for blocked admin access.
- Added persistent data protection keys and extended admin sessions to 8 hours so long edit sessions do not expire immediately.
- Added bot detection based on user agent and expanded dashboard monitoring with pagination, hourly activity and human/bot counters.

## 30.03.2025

### Lizerium Server

- Moved most folder paths into `appsettings.json`
- Rebuilt the project into an open-source version

### Api Lizerium Server

- Added configuration in `appsettings.json` for local or global translation service (LibreTranslate)

---

## 20.07.2025

- Migrated `black_list` from `database_configuration.json` into `appsettings.json`, renamed to `BlackList`
- Fully redesigned the entire portal UI
- Fixed device overheating issues (especially on mobile) during long sessions in documentation sections (commands list and crafting recipes)
- Improved performance of command input pages:
  - introduced pagination
  - added item search functionality
- Improved mobile version:
  - adjusted fonts
  - optimized layout and block sizes for better readability
- Added links to other Freelancer-related projects (by portal author initiative)
- Implemented DoS protection for the portal

### Additional Improvements

- News system updated:
  - now dynamically updated without server restart
- Added forum system:
  - users can communicate directly on the portal
- Enhanced Q&A system:
  - users can now attach PNG/JPG images to questions
  - author can respond with comments directly under questions
  - rejection reasons are now visible if a request is declined
- Added multilingual support:
  - Russian and English
  - English translations added for international audience

---

## 04.06.2025

- Added documentation page for item crafting and refactored related controllers:
  - Supports `*craft_build*.json` files
  - Renders content in a structured HTML format
  - Ingredients are loaded dynamically on click and unloaded when closed
- Improved mobile optimization across all UI sections
