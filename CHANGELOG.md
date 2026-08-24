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

## 24.08.2026

### Lizerium Server

- Added canonical public news URLs in the `/news/{id}/{slug}.html` format while keeping the existing modal reader experience on the launcher news page.
- Added direct opening of a news item through canonical news URLs so shared links and sitemap entries render the selected article immediately.
- Added published launcher news entries to `sitemap.xml` and protected sitemap rebuild support through `/internal/sitemap/rebuild`.
- Fixed sitemap generation for debug and hosted environments where `WebRootPath` can be empty by falling back to a writable `wwwroot` path.
- Fixed the global page-transition handler so `Read full` news links open the modal instead of forcing a full page navigation.
- Fixed Knowledge Base SEO canonicalization: language-less `/wiki/KnowledgeBase/...` URLs now permanently redirect to the selected language URL, explicit `/ru/` and `/en/` article URLs keep their own language content, and article pages emit canonical plus `hreflang` alternates from a single valid HTML `<head>`.
- Kept news-card video iframes lazy: card iframe `src` is set only after selecting a video platform.

### Api Lizerium Server

- Added automatic public `sitemap.xml` rebuild after news save/delete through the configured public site endpoint.
- Added a manual `Rebuild sitemap.xml` action to the admin news page.
- Added protected API-to-public sitemap rebuild configuration through `appSettings:sitemapRebuildToken` and `appSettings:publicSiteBaseUrl`.

### Tests and Quality

- Added regression tests for canonical news URLs, sitemap news entries, sitemap rebuild authorization and modal-reader/page-transition behavior.
- Added regression tests for Knowledge Base redirects, canonical URLs and RU/EN alternate links.

## 08.08.2026

### Lizerium Server

- Enabled `asp-append-version` for Knowledge Base, documentation and support page CSS files so user browsers receive updated styles after deployment without manual `Ctrl+F5`.
- Fixed video-source visual states in news: only the selected platform is highlighted now, instead of all available YouTube/VK/Rutube buttons looking active at once.
- Fixed video-platform switching on news cards: selecting YouTube/VK/Rutube now updates the active button and passes the selected platform into the full news reader, including cards that use a poster instead of an embedded iframe.
- News cards now render the first meaningful Markdown block of the article with inline formatting, skipping covers, images, videos and headings that belong to the full reader.
- Added fullscreen lightbox viewing for Markdown images in the public news reader and grouped consecutive article images into an adaptive grid for narrow/mobile screenshots.
- Rebuilt `global.min.css` and `app.min.js` to ship the news-card style and behavior fixes.

### Api Lizerium Server

- Fixed broken news image previews on the production API admin domain by serving local `/img/news/...` files through protected `/news/assets/preview` and `/news/image/{fileName}` routes.
- Added a protected news image gallery in the admin news editor: search uploaded files, insert an image into Markdown, add it to `ImageGalleryJson`, set it as the cover image, or delete the physical file.
- Added detach controls for already attached gallery images so an image can be removed from a news record without deleting the uploaded file.
- Added a closed admin-only news preview route at `/news/preview/{id}` protected by `AdminAccessGuard`, IP block checks and an authorized `AdminSession`.
- The preview can open hidden/unpublished news, so drafts can be saved without publishing and reviewed before appearing on the public site.
- Split the admin news preview into two states: the public news-list card and the full article reader view.
- The admin preview card now uses the same Markdown excerpt logic as the public news card.
- Fixed the admin news preview layout so article centering no longer leaks into the preview, image URLs can fall back to the public `lizerium.com` domain, and video-news cards show a YouTube/Rutube poster when no dedicated cover image exists.
- Added a `Preview` button to each saved news item in the admin news list; it opens the closed preview page in a new tab.
- Added a note to the create-news form explaining that preview becomes available after the first save and drafts should be kept unpublished.
- Added server-side pagination and debounced search to the admin news image gallery so large upload folders no longer freeze the browser when the picker opens.
- Fixed admin news image rendering on the API preview page by routing cover, icon, gallery and Markdown images through the protected news asset preview endpoint with safer production path resolution.
- Fixed inline Markdown images in the API news preview so `![image](...)` renders even when it appears inside a text paragraph.
- Matched the API news preview with the public reader: article images can be opened fullscreen and adjacent inline images can sit side by side.
- Moved API news preview styles out of Razor into a page-level SCSS file and added SCSS compilation to the API Grunt pipeline.

### Database and Tests

- Added `GetAdminLauncherNewsByIdAsync`, which loads a news item by id without the `IsPublished` filter for admin-only scenarios.
- Added a regression test confirming that hidden news can be loaded for admin preview.

## 07.08.2026

### Lizerium Server

- Added a dedicated `Community` page with links to the main Lizerium channels: VK, Discord, YouTube, Rutube, VK Video and game server monitoring.
- Reworked the home page into a portal overview that surfaces the key areas immediately: news, games, documentation, server features, tools, support, community and external Freelancer projects.
- Reworked global site navigation with a desktop top menu, updated sidebar and mobile navigation, new icons, active states, a language combobox and animated page transitions.
- Rebuilt the `Games` section on top of a database-backed product catalog: launcher, games, builds and utilities are now grouped by categories and download sources.
- Added a tools and utilities section for Freelancer (2003): LizeriumFindChanges, LizeriumDataToolkit, LizeriumVSCodeColorPicker, LizeriumAccauntManager, Lizerium.Restarter.Server, Lizerium.RDL.Converter, Lizerium.Localization.Toolkit, Lizerium.BINI.Converter, CompilerInfocardsUI, LizeriumFLHook, LizeriumUTFtoXML, Lizerium.UTF.Editor and Freelancer.Reverse.Runtime.
- Added a download-source modal on the games page, expandable long categories, in-category search and soft wrapping for long product names.
- Extended the Lizerium Steam news page with filters by video platform, GitHub links and news type; refreshed the hero, cards, reader modal and pagination links so filters are preserved.
- News posts can now represent GitHub project publications, videos, full update articles, images, galleries and mixed media announcements.
- Reworked the support page with a new wish/request UI, custom creation modal, request cards, statuses, publication dates and ReCaptcha validation only on request creation.
- Fixed wish/request submission redirect: users now return to the support page instead of the home page.
- Reworked documentation pages: the documentation index, installation page, server command pages and crafting pages now use the new layout, hero blocks, cards, back navigation and responsive styling.
- Added search to the craftable-items table, including item/category search, quick jump to a found recipe, result highlighting and improved scrolling to opened recipes.
- Added global command search and category-local command search to server command documentation; category cards now show command counts.
- Added lightbox viewing for command GIF examples.
- Fixed links and the table of contents in the Freelancer (2003) Knowledge Base: Russian anchor links now map to generated heading ids, while relative and absolute KnowledgeBase links normalize to `/wiki/KnowledgeBase/...`.
- Added support for an external Knowledge Base root through `StoragePathsOptions.KnowledgeBase`.
- Updated RU/EN localization for the home page, games, news, community, support, documentation, footer, navigation and page transitions.
- Added public `wwwroot/img` assets for brand, navigation, social links and the home/game/community/documentation pages.
- Improved maintenance mode handling for empty request paths and empty updater-dev-mode whitelists.
- Switched JS/CSS minimization in Webpack to non-parallel mode for more stable builds in constrained environments.

### Api Lizerium Server

- Added a `Products` section to the admin panel.
- Added management for product categories, products and download sources: create, edit, delete, sort order, RU/EN names and descriptions, active flags, icons and backgrounds.
- Added AJAX saving for product catalog forms with quick `Saving/Saved/Error` feedback and automatic reload after creating a new record.
- Added collapsible category, product and inline-form panels for easier work with large catalogs.
- Added an image library for the product catalog: search existing `/img` files, preview images, select an existing image and upload new files.
- Added protected `/products/assets`, `/products/assets/preview` and `/products/assets/upload` endpoints; uploads are limited to 8 MB and support `webp`, `png`, `jpg`, `jpeg`, `gif` and `svg`.
- Extended the news admin page with a selector for existing news types so RU/EN publication types can be reused without manual entry.
- Added `Products` to the admin sidebar.
- Renamed Lizerium Launcher wording to Lizerium Steam across admin and public surfaces.

### Database and Contracts

- Added `product_categories`, `products` and `product_download_links` tables with cascade deletion for nested products and links.
- Added `ProductCategoryDataResponse`, `ProductDataResponse` and `ProductDownloadLinkDataResponse` DTOs.
- Made `DataBaseService` partial and moved initial product catalog seeding into `DataBaseService.ProductsSeed.cs`.
- Added methods for loading the public and admin product catalog, saving and deleting categories, products and download sources.
- Added initial seed data for launcher, available downloads and tools.
- Added an optional `category` filter to public command search; existing calls without the filter remain compatible.
- Updated the launcher-news model wording to Lizerium Steam and fixed Russian XML comments.

### Tests and Quality

- Added product catalog tests to verify public queries return only active categories, products and links in the correct order.
- Added regression tests for the community page and layout assets.
- Added coverage for `IReCaptchaService` registration.
- Expanded Markdown rendering tests for front matter, first-H1 replacement, KnowledgeBase link normalization and Russian table-of-contents links.

### Upgrade Notes

- The new product tables are created automatically through `CREATE TABLE IF NOT EXISTS`; for databases other than the current SQLite scenario, SQL compatibility should be checked separately.
- Initial product seed data is inserted only when the product categories table is empty.
- Product image upload and preview require a valid `appSettings:portalImagesPath` or accessible `LizeriumServer/wwwroot/img` directory.
- The public `/Home/Game` page now expects `GameProductsViewModel`.
- `ReCaptcha` is registered through `AddReCaptcha(...)` instead of only `Configure<ReCaptchaSettings>`.

## 01.08.2026

### Lizerium Server

- Added multi-domain SEO configuration through `SeoDomains` for running one server on `lizerium.com` and `lizup.ru` without duplicating the project.
- Added dynamic canonical, OpenGraph, VK preview and JSON-LD URL generation based on the configured SEO domain mode.
- Added host-aware `/sitemap.xml` and `/robots.txt` generation: each domain now receives sitemap and robots links for its own host while page canonical URLs can still point to the primary domain.
- Added `appsettings.example.json` documentation for `PrimaryDomain`, `Domains`, `Scheme`, `CanonicalMode`, `OpenGraphImage` and `SiteName`.
- Updated deployment and build documentation with nginx proxy requirements, including forwarded host headers for multi-domain hosting.

## 31.07.2026

### Lizerium Server

- Added a database-backed Lizerium Steam news feed on `/Home/Launcher` with search, newest/oldest sorting, pagination and localized RU/EN content.
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
