import { Ajax } from "../Shared/ajax";
import { Cookies } from "../Shared/cookies";
import { LocalizationStrings, Utilities } from "../Shared/utilities";

declare var bootstrap: any;
interface CommandResult {
    urlGif?: string;
    UrlGif?: string;
    CommandNames: string;
    CommandNamesList?: string[];
    Description: string;
    Category: string;
    Translations?: TranslationItem[];
    TitlesCategory?: TitlesCategory[];
}

interface CommandIndexResult {
    anchor?: string;
    category: string;
    firstName: string;
    page: number;
}

interface TranslationItem {
    Locale: string;
    Description: string;
}

interface TitlesCategory {
    ru: string;
    en: string;
}

export class DocHook {
    /**
     * Экземпляр класса утилит
     */
    private readonly utilities: Utilities;

    /**
     * Экземпляр класса работы с куками
     */
    private readonly cookies: Cookies;

    /**
     * Локализованные строки страницы
     */
    private localizationStrings: LocalizationStrings;

    /**
     * Конструктор
     * @param utilities - экземпляр класса утилит
     * @param cookies - экземпляр класса кук
     */
    constructor(utilities: Utilities, cookies: Cookies) {
        //присваиваем экземпляр класса утилит
        this.utilities = utilities;

        //присваиваем экземпляр класса кук
        this.cookies = cookies;
    }

    async start(): Promise<void> {
        //console.log("hook is started");
        // Устанавливаю шрифты для кода
        var code: HTMLCollectionOf<HTMLElement> = document.getElementsByTagName("code");
        for (var i: number = 0; i < code.length; ++i) {
            code[i].style.fontFamily = 'CJK MONO'
        }

        // Устанавливаю настройки поиска
        const commandSearch = document.getElementById("commandSearch") as HTMLInputElement | null;
        const searchResults = document.getElementById("searchResults") as HTMLElement | null;

        //создаем экземпляр Ajax
        const ajax = new Ajax(`getAllLocalizedStrings/Views.Documents.DocHook`, this.cookies);
        //отправляем запрос
        const response = await ajax.sendRequest();
        //фиксируем переводы
        this.localizationStrings = JSON.parse(response);

        if (commandSearch && searchResults) {
            commandSearch.addEventListener("input", async () => {
                const query = commandSearch.value;
                const category = commandSearch.dataset.category;

                if (query.length < 2) {
                    searchResults.innerHTML = "";
                    return;
                }

                try {
                    //создаем экземпляр Ajax
                    const requestUrl = category
                        ? `searchCommands/${encodeURIComponent(query)}?category=${encodeURIComponent(category)}`
                        : `searchCommands/${encodeURIComponent(query)}`;
                    const ajax = new Ajax(requestUrl, this.cookies);
                    //отправляем запрос
                    const response = await ajax.sendRequest();
                    //проверяем ответ
                    if (this.utilities.isEmpty(response)) return;

                    //десериализуем из JSON
                    const dataResponse = JSON.parse(response);

                    //const response = await fetch(`/api/command/search?query=${encodeURIComponent(query)}`);
                    searchResults.innerHTML = "";

                    if (!dataResponse.length) {
                        searchResults.innerHTML = "<p class='search_alert'>" + this.localizationStrings["DocHookItem_Search_Status1"] + "</p>";
                        return;
                    }

                    for (const cmd of dataResponse) {
                        const card = document.createElement("div");
                        const firstName = cmd.CommandNamesList && cmd.CommandNamesList.length > 0
                            ? cmd.CommandNamesList[0]
                            : cmd.CommandNames.split(',')[0].trim(); // fallback
                        await this.ensureCategoryIndex(cmd.Category);


                        const url = this.buildDocUrl(cmd as CommandResult);
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
                } catch (err) {
                    //console.error("Ошибка при поиске команд:", err);
                    searchResults.innerHTML = "<p class='search_alert'>" + this.localizationStrings["DocHookItem_Search_Status2"] + "</p>";
                }
            });
        } else {
            //console.warn("Элемент командного поиска или контейнер результатов не найден.");
        }
    }

    /**
     * Resolves a command result to the exact docs page/anchor using the cached category index.
     */
    private buildDocUrl(cmd: CommandResult): string {
        const key = `cmdIndex:${cmd.Category}`;
        const raw = localStorage.getItem(key);

        if (!raw) {
            // fallback — без индекса
            const anchor = cmd.CommandNames?.[0]?.replace("/", "") ?? "";
            return `/docs/hook/${encodeURIComponent(cmd.Category)}#${encodeURIComponent(anchor)}`;
        }

        const index: Array<{ FirstName: string; Anchor: string; Page: number }> = JSON.parse(raw);

        const commandNameFirst = cmd.CommandNames.split(',')[0].trim();

        var page = 1;
        index.forEach(it => {
            var el = it as unknown as CommandIndexResult;
            if (el.firstName == commandNameFirst) {
                page = el.page;
            }
        });
        var comm = commandNameFirst.replace("/", "");

        if (page > 1) {
            return `/docs/hook/${encodeURIComponent(cmd.Category)}?page=${page}#hook_${encodeURIComponent(comm)}`;
        }
        else return `/docs/hook/${encodeURIComponent(cmd.Category)}#hook_${encodeURIComponent(comm)}`;
    }

    /**
     * Lazily fetches and caches command anchors per category to keep search result links stable.
     */
    private async ensureCategoryIndex(category: string): Promise<void> {
        const key = `cmdIndex:${category}`;
        if (localStorage.getItem(key)) return;

        const res = await fetch(`/docs/hook/${encodeURIComponent(category)}/index`);
        if (!res.ok) return;

        const data = await res.json();
        localStorage.setItem(key, JSON.stringify(data));
    }

    private getLocalizedDescription(cmd: CommandResult): string {
        const locale = this.getCurrentLocale();

        const translations = cmd.Translations as unknown as Record<string, string[]> | undefined;
        if (translations && translations[locale] && translations[locale].length > 0) {
            return translations[locale][0]; // берем первый перевод
        }

        return cmd.Description; // fallback
    }

    private getLocalizedTitleCategory(cmd: CommandResult): string {
        const locale = this.getCurrentLocale();

        const translations = cmd.TitlesCategory as unknown as Record<string, string> | undefined;
        if (translations && translations[locale] && translations[locale].length > 0) {
            return translations[locale]; // берем первый перевод
        }

        return cmd.Category; // fallback
    }

    /**
     * Builds the optional GIF preview cell for command search results.
     */
    private getSearchPreview(cmd: CommandResult): string {
        const gif = cmd.UrlGif || cmd.urlGif;
        if (gif) {
            const src = gif.startsWith("/") ? gif : `/gifs/${gif}`;
            return `<span class="doc-hook-search-preview"><img src="${src}" alt=""></span>`;
        }

        return `<span class="doc-hook-search-preview is-empty" aria-hidden="true"></span>`;
    }

    private getCurrentLocale(): string {
        const lang = this.cookies.getCookie(".AspNetCore.Culture");
        // парсим строку типа "c=en|uic=en"
        const match = /uic=([a-z]{2})/i.exec(lang);
        if (match) {
            return match[1]; // например "en"
        }
        return "ru";
    }
}
