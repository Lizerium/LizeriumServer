import { Cookies } from "../Shared/cookies";
import { Utilities } from "../Shared/utilities";

declare var bootstrap: any;

export class DocBuilds {
    /**
     * Максимальная длинна поста 
     */
    private truncateLength: number;

    /**
     * Флаг что AJAX в процессе
     */
    private ajaxInProgress: boolean;

    /**
     * Экземпляр класса утилит
     */
    private readonly utilities: Utilities;

    /**
     * Экземпляр класса работы с куками
     */
    private readonly cookies: Cookies;

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
        this.bindBuildSearch();

        const buttons = document.querySelectorAll<HTMLButtonElement>(".load-build-component");
        buttons.forEach(button => {
            button.addEventListener("click", async () => {
                const targetId = button.getAttribute("data-target")?.replace("#", "");
                const buildName = button.getAttribute("data-name");
                if (!targetId || !buildName) return;

                const collapseEl = document.getElementById(targetId);
                const body = collapseEl?.querySelector(".build-body");
                if (!collapseEl || !body) return;

                // Если уже раскрыто — свернём
                const isShown = collapseEl.classList.contains("show");
                if (isShown) {
                    bootstrap.Collapse.getOrCreateInstance(collapseEl).hide();
                    return;
                }

                // Load the recipe on first open and drop it on close to keep the page light.
                if (!body.hasChildNodes()) {
                    body.innerHTML = `<div class="text-muted">Загрузка...</div>`;
                    try {
                        const response = await fetch(`/docs/build/${encodeURIComponent(buildName)}`);
                        if (!response.ok) {
                            body.innerHTML = `<div class="text-danger">Ошибка загрузки</div>`;
                            return;
                        }

                        const html = await response.text();
                        body.innerHTML = html;

                    } catch (err) {
                        body.innerHTML = `<div class="text-danger">Ошибка запроса</div>`;
                        return;
                    }
                }

                // Подписываемся на завершение анимации
                const onShown = () => {
                    this.scrollRecipeIntoView(button);
                    collapseEl.removeEventListener("shown.bs.collapse", onShown);
                };
                collapseEl.addEventListener("shown.bs.collapse", onShown);
                // Внутри addEventListener блока кнопки, после получения collapseEl и body
                collapseEl.addEventListener("hidden.bs.collapse", () => {
                    body.innerHTML = ""; // Очищаем содержимое после закрытия
                }, { once: true }); // once: true — чтобы не множились обработчики

                // Теперь вручную запускаем анимацию
                const bsCollapse = bootstrap.Collapse.getOrCreateInstance(collapseEl);
                bsCollapse.show();
            });
        });
    }

    /**
     * Searches across build recipe buttons and mirrors matches into a compact result grid.
     */
    private bindBuildSearch(): void {
        const searchInput = document.querySelector<HTMLInputElement>("#docBuildSearch");
        const clearButton = document.querySelector<HTMLButtonElement>(".doc-build-search-clear");
        const resultsContainer = document.querySelector<HTMLElement>("#docBuildSearchResults");
        const buildButtons = Array.from(document.querySelectorAll<HTMLButtonElement>(".load-build-component"));

        if (!searchInput || !clearButton || !resultsContainer || buildButtons.length === 0) {
            return;
        }

        const emptyText = searchInput.dataset.emptyText || "No items found";
        const resultText = searchInput.dataset.resultText || "result";
        const resultsText = searchInput.dataset.resultsText || "results";

        const render = () => {
            const query = searchInput.value.trim().toLowerCase();
            clearButton.hidden = query.length === 0;

            if (query.length === 0) {
                resultsContainer.hidden = true;
                resultsContainer.innerHTML = "";
                buildButtons.forEach(button => button.closest(".doc-build-item")?.classList.remove("is-search-match"));
                return;
            }

            const matches = buildButtons
                .map(button => {
                    const title = button.dataset.buildTitle || button.textContent?.trim() || "";
                    const category = button.dataset.buildCategory || "";
                    const haystack = `${title} ${category}`.toLowerCase();
                    return { button, title, category, haystack };
                })
                .filter(item => item.haystack.includes(query))
                .slice(0, 8);

            buildButtons.forEach(button => {
                const item = button.closest(".doc-build-item");
                const title = button.dataset.buildTitle || "";
                const category = button.dataset.buildCategory || "";
                item?.classList.toggle("is-search-match", `${title} ${category}`.toLowerCase().includes(query));
            });

            resultsContainer.hidden = false;
            if (matches.length === 0) {
                resultsContainer.innerHTML = `<div class="doc-build-search-empty">${emptyText}</div>`;
                return;
            }

            const label = matches.length === 1 ? resultText : resultsText;
            resultsContainer.innerHTML = `
                <div class="doc-build-search-summary">${matches.length} ${label}</div>
                <div class="doc-build-search-grid">
                    ${matches.map((item, index) => `
                        <button class="doc-build-search-card custom-cursor-hover" type="button" data-result-index="${index}">
                            <span>${this.escapeHtml(item.title)}</span>
                            <small>${this.escapeHtml(item.category)}</small>
                        </button>
                    `).join("")}
                </div>
            `;

            resultsContainer.querySelectorAll<HTMLButtonElement>(".doc-build-search-card").forEach(card => {
                card.addEventListener("click", () => {
                    const index = Number(card.dataset.resultIndex || "0");
                    this.activateSearchResult(matches[index]?.button);
                });
            });
        };

        searchInput.addEventListener("input", render);
        clearButton.addEventListener("click", () => {
            searchInput.value = "";
            render();
            searchInput.focus();
        });
    }

    /**
     * Opens the right tab before scrolling to a recipe matched by global build search.
     */
    private activateSearchResult(button?: HTMLButtonElement): void {
        if (!button) {
            return;
        }

        const paneId = button.dataset.buildPane;
        if (paneId) {
            const tabButton = Array.from(document.querySelectorAll<HTMLButtonElement>(".doc-build-tabs [data-bs-target]"))
                .find(tab => tab.getAttribute("data-bs-target") === `#${paneId}`);
            if (tabButton) {
                bootstrap.Tab.getOrCreateInstance(tabButton).show();
            }
        }

        window.setTimeout(() => {
            button.closest(".doc-build-item")?.scrollIntoView({ behavior: "smooth", block: "center" } as ScrollIntoViewOptions);
            button.closest(".doc-build-item")?.classList.add("is-search-focus");
            window.setTimeout(() => button.closest(".doc-build-item")?.classList.remove("is-search-focus"), 1300);
        }, 80);
    }

    private scrollRecipeIntoView(button: Element): void {
        const item = button.closest(".doc-build-item");
        if (!item) {
            return;
        }

        const footerHeight = document.querySelector("footer")?.getBoundingClientRect().height || 104;
        const rect = item.getBoundingClientRect();
        const targetTop = window.scrollY + rect.top - 96;
        const targetBottom = window.scrollY + rect.bottom - window.innerHeight + footerHeight + 34;
        const target = Math.max(targetTop, targetBottom);

        window.scrollTo({
            top: Math.max(0, target),
            behavior: "smooth"
        });
    }

    private escapeHtml(value: string): string {
        const div = document.createElement("div");
        div.textContent = value;
        return div.innerHTML;
    }
}
