var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
export class DocBuilds {
    constructor(utilities, cookies) {
        this.utilities = utilities;
        this.cookies = cookies;
    }
    start() {
        return __awaiter(this, void 0, void 0, function* () {
            this.bindBuildSearch();
            const buttons = document.querySelectorAll(".load-build-component");
            buttons.forEach(button => {
                button.addEventListener("click", () => __awaiter(this, void 0, void 0, function* () {
                    var _a;
                    const targetId = (_a = button.getAttribute("data-target")) === null || _a === void 0 ? void 0 : _a.replace("#", "");
                    const buildName = button.getAttribute("data-name");
                    if (!targetId || !buildName)
                        return;
                    const collapseEl = document.getElementById(targetId);
                    const body = collapseEl === null || collapseEl === void 0 ? void 0 : collapseEl.querySelector(".build-body");
                    if (!collapseEl || !body)
                        return;
                    const isShown = collapseEl.classList.contains("show");
                    if (isShown) {
                        bootstrap.Collapse.getOrCreateInstance(collapseEl).hide();
                        return;
                    }
                    if (!body.hasChildNodes()) {
                        body.innerHTML = `<div class="text-muted">Загрузка...</div>`;
                        try {
                            const response = yield fetch(`/docs/build/${encodeURIComponent(buildName)}`);
                            if (!response.ok) {
                                body.innerHTML = `<div class="text-danger">Ошибка загрузки</div>`;
                                return;
                            }
                            const html = yield response.text();
                            body.innerHTML = html;
                        }
                        catch (err) {
                            body.innerHTML = `<div class="text-danger">Ошибка запроса</div>`;
                            return;
                        }
                    }
                    const onShown = () => {
                        this.scrollRecipeIntoView(button);
                        collapseEl.removeEventListener("shown.bs.collapse", onShown);
                    };
                    collapseEl.addEventListener("shown.bs.collapse", onShown);
                    collapseEl.addEventListener("hidden.bs.collapse", () => {
                        body.innerHTML = "";
                    }, { once: true });
                    const bsCollapse = bootstrap.Collapse.getOrCreateInstance(collapseEl);
                    bsCollapse.show();
                }));
            });
        });
    }
    bindBuildSearch() {
        const searchInput = document.querySelector("#docBuildSearch");
        const clearButton = document.querySelector(".doc-build-search-clear");
        const resultsContainer = document.querySelector("#docBuildSearchResults");
        const buildButtons = Array.from(document.querySelectorAll(".load-build-component"));
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
                buildButtons.forEach(button => { var _a; return (_a = button.closest(".doc-build-item")) === null || _a === void 0 ? void 0 : _a.classList.remove("is-search-match"); });
                return;
            }
            const matches = buildButtons
                .map(button => {
                var _a;
                const title = button.dataset.buildTitle || ((_a = button.textContent) === null || _a === void 0 ? void 0 : _a.trim()) || "";
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
                item === null || item === void 0 ? void 0 : item.classList.toggle("is-search-match", `${title} ${category}`.toLowerCase().includes(query));
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
            resultsContainer.querySelectorAll(".doc-build-search-card").forEach(card => {
                card.addEventListener("click", () => {
                    var _a;
                    const index = Number(card.dataset.resultIndex || "0");
                    this.activateSearchResult((_a = matches[index]) === null || _a === void 0 ? void 0 : _a.button);
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
    activateSearchResult(button) {
        if (!button) {
            return;
        }
        const paneId = button.dataset.buildPane;
        if (paneId) {
            const tabButton = Array.from(document.querySelectorAll(".doc-build-tabs [data-bs-target]"))
                .find(tab => tab.getAttribute("data-bs-target") === `#${paneId}`);
            if (tabButton) {
                bootstrap.Tab.getOrCreateInstance(tabButton).show();
            }
        }
        window.setTimeout(() => {
            var _a, _b;
            (_a = button.closest(".doc-build-item")) === null || _a === void 0 ? void 0 : _a.scrollIntoView({ behavior: "smooth", block: "center" });
            (_b = button.closest(".doc-build-item")) === null || _b === void 0 ? void 0 : _b.classList.add("is-search-focus");
            window.setTimeout(() => { var _a; return (_a = button.closest(".doc-build-item")) === null || _a === void 0 ? void 0 : _a.classList.remove("is-search-focus"); }, 1300);
        }, 80);
    }
    scrollRecipeIntoView(button) {
        var _a;
        const item = button.closest(".doc-build-item");
        if (!item) {
            return;
        }
        const footerHeight = ((_a = document.querySelector("footer")) === null || _a === void 0 ? void 0 : _a.getBoundingClientRect().height) || 104;
        const rect = item.getBoundingClientRect();
        const targetTop = window.scrollY + rect.top - 96;
        const targetBottom = window.scrollY + rect.bottom - window.innerHeight + footerHeight + 34;
        const target = Math.max(targetTop, targetBottom);
        window.scrollTo({
            top: Math.max(0, target),
            behavior: "smooth"
        });
    }
    escapeHtml(value) {
        const div = document.createElement("div");
        div.textContent = value;
        return div.innerHTML;
    }
}
//# sourceMappingURL=doc_builds.js.map