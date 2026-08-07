var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { Ajax } from "../Shared/ajax";
export class DocHook {
    constructor(utilities, cookies) {
        this.utilities = utilities;
        this.cookies = cookies;
    }
    start() {
        return __awaiter(this, void 0, void 0, function* () {
            var code = document.getElementsByTagName("code");
            for (var i = 0; i < code.length; ++i) {
                code[i].style.fontFamily = 'CJK MONO';
            }
            const commandSearch = document.getElementById("commandSearch");
            const searchResults = document.getElementById("searchResults");
            const ajax = new Ajax(`getAllLocalizedStrings/Views.Documents.DocHook`, this.cookies);
            const response = yield ajax.sendRequest();
            this.localizationStrings = JSON.parse(response);
            if (commandSearch && searchResults) {
                commandSearch.addEventListener("input", () => __awaiter(this, void 0, void 0, function* () {
                    const query = commandSearch.value;
                    const category = commandSearch.dataset.category;
                    if (query.length < 2) {
                        searchResults.innerHTML = "";
                        return;
                    }
                    try {
                        const requestUrl = category
                            ? `searchCommands/${encodeURIComponent(query)}?category=${encodeURIComponent(category)}`
                            : `searchCommands/${encodeURIComponent(query)}`;
                        const ajax = new Ajax(requestUrl, this.cookies);
                        const response = yield ajax.sendRequest();
                        if (this.utilities.isEmpty(response))
                            return;
                        const dataResponse = JSON.parse(response);
                        searchResults.innerHTML = "";
                        if (!dataResponse.length) {
                            searchResults.innerHTML = "<p class='search_alert'>" + this.localizationStrings["DocHookItem_Search_Status1"] + "</p>";
                            return;
                        }
                        for (const cmd of dataResponse) {
                            const card = document.createElement("div");
                            const firstName = cmd.CommandNamesList && cmd.CommandNamesList.length > 0
                                ? cmd.CommandNamesList[0]
                                : cmd.CommandNames.split(',')[0].trim();
                            yield this.ensureCategoryIndex(cmd.Category);
                            const url = this.buildDocUrl(cmd);
                            card.className = "doc-hook-search-result";
                            card.innerHTML = `
                            <a class="doc-hook-search-card custom-cursor-hover" href="${url}">
                                ${this.getSearchPreview(cmd)}
                                <span class="doc-hook-search-copy">
                                    <strong>${cmd.CommandNames}</strong>
                                    <small>${this.getLocalizedDescription(cmd)}</small>
                                </span>
                                <span class="doc-hook-search-category">${this.getLocalizedTitleCategory(cmd)}</span>
                            </a>`;
                            searchResults.appendChild(card);
                        }
                    }
                    catch (err) {
                        searchResults.innerHTML = "<p class='search_alert'>" + this.localizationStrings["DocHookItem_Search_Status2"] + "</p>";
                    }
                }));
            }
            else {
            }
        });
    }
    buildDocUrl(cmd) {
        var _a, _b, _c;
        const key = `cmdIndex:${cmd.Category}`;
        const raw = localStorage.getItem(key);
        if (!raw) {
            const anchor = (_c = (_b = (_a = cmd.CommandNames) === null || _a === void 0 ? void 0 : _a[0]) === null || _b === void 0 ? void 0 : _b.replace("/", "")) !== null && _c !== void 0 ? _c : "";
            return `/docs/hook/${encodeURIComponent(cmd.Category)}#${encodeURIComponent(anchor)}`;
        }
        const index = JSON.parse(raw);
        const commandNameFirst = cmd.CommandNames.split(',')[0].trim();
        var page = 1;
        index.forEach(it => {
            var el = it;
            if (el.firstName == commandNameFirst) {
                page = el.page;
            }
        });
        var comm = commandNameFirst.replace("/", "");
        if (page > 1) {
            return `/docs/hook/${encodeURIComponent(cmd.Category)}?page=${page}#hook_${encodeURIComponent(comm)}`;
        }
        else
            return `/docs/hook/${encodeURIComponent(cmd.Category)}#hook_${encodeURIComponent(comm)}`;
    }
    ensureCategoryIndex(category) {
        return __awaiter(this, void 0, void 0, function* () {
            const key = `cmdIndex:${category}`;
            if (localStorage.getItem(key))
                return;
            const res = yield fetch(`/docs/hook/${encodeURIComponent(category)}/index`);
            if (!res.ok)
                return;
            const data = yield res.json();
            localStorage.setItem(key, JSON.stringify(data));
        });
    }
    getLocalizedDescription(cmd) {
        const locale = this.getCurrentLocale();
        const translations = cmd.Translations;
        if (translations && translations[locale] && translations[locale].length > 0) {
            return translations[locale][0];
        }
        return cmd.Description;
    }
    getLocalizedTitleCategory(cmd) {
        const locale = this.getCurrentLocale();
        const translations = cmd.TitlesCategory;
        if (translations && translations[locale] && translations[locale].length > 0) {
            return translations[locale];
        }
        return cmd.Category;
    }
    getSearchPreview(cmd) {
        const gif = cmd.UrlGif || cmd.urlGif;
        if (gif) {
            const src = gif.startsWith("/") ? gif : `/gifs/${gif}`;
            return `<span class="doc-hook-search-preview"><img src="${src}" alt=""></span>`;
        }
        return `<span class="doc-hook-search-preview is-empty" aria-hidden="true"></span>`;
    }
    getCurrentLocale() {
        const lang = this.cookies.getCookie(".AspNetCore.Culture");
        const match = /uic=([a-z]{2})/i.exec(lang);
        if (match) {
            return match[1];
        }
        return "ru";
    }
}
//# sourceMappingURL=doc_hook.js.map