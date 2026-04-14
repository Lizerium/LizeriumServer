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
