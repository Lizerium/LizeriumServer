export class Game {
    constructor() {
        this.modal = document.querySelector("[data-game-download-modal]");
        this.modalIcon = document.querySelector("[data-game-download-icon]");
        this.modalTitle = document.querySelector("[data-game-download-title]");
        this.modalDescription = document.querySelector("[data-game-download-description]");
        this.modalSources = document.querySelector("[data-game-download-sources]");
    }
    start() {
        document.querySelectorAll("[data-game-product-title]").forEach((title) => {
            this.setTextWithSoftBreaks(title, title.innerText.trim());
        });
        document.querySelectorAll("[data-game-download]").forEach((button) => {
            button.addEventListener("click", () => this.openDownloadModal(button));
        });
        this.initCategoryControls();
        document.querySelectorAll("[data-game-download-close]").forEach((closeControl) => {
            closeControl.addEventListener("click", () => this.closeDownloadModal());
        });
        document.addEventListener("keydown", (event) => {
            if (event.key === "Escape")
                this.closeDownloadModal();
        });
    }
    openDownloadModal(button) {
        var _a, _b, _c, _d, _e;
        const product = button.closest("[data-game-product]");
        if (!product || !this.modal || !this.modalSources)
            return;
        const title = (_b = (_a = product.querySelector("[data-game-product-title]")) === null || _a === void 0 ? void 0 : _a.innerText.trim()) !== null && _b !== void 0 ? _b : "";
        const description = (_d = (_c = product.querySelector("[data-game-product-description]")) === null || _c === void 0 ? void 0 : _c.innerText.trim()) !== null && _d !== void 0 ? _d : "";
        const productIcon = (_e = button.dataset.productIcon) !== null && _e !== void 0 ? _e : "";
        if (this.modalTitle)
            this.setTextWithSoftBreaks(this.modalTitle, title);
        if (this.modalDescription)
            this.modalDescription.innerText = description;
        if (this.modalIcon) {
            this.modalIcon.src = productIcon;
            this.modalIcon.alt = "";
        }
        this.modalSources.innerHTML = "";
        product.querySelectorAll("[data-game-source]").forEach((source) => {
            var _a, _b, _c;
            const link = document.createElement("a");
            link.className = "lizerium-game-source-link";
            link.href = source.href;
            link.target = "_blank";
            link.rel = "noopener noreferrer";
            const icon = document.createElement("img");
            icon.src = (_a = source.dataset.sourceIcon) !== null && _a !== void 0 ? _a : "";
            icon.alt = "";
            icon.loading = "lazy";
            const text = document.createElement("span");
            text.innerText = (_b = source.dataset.sourceName) !== null && _b !== void 0 ? _b : source.innerText.trim();
            const arrow = document.createElement("span");
            arrow.innerText = "\u2197";
            arrow.setAttribute("aria-hidden", "true");
            link.append(icon, text, arrow);
            (_c = this.modalSources) === null || _c === void 0 ? void 0 : _c.append(link);
        });
        this.modal.hidden = false;
        document.body.classList.add("lizerium-modal-open");
    }
    closeDownloadModal() {
        if (!this.modal || this.modal.hidden)
            return;
        this.modal.hidden = true;
        document.body.classList.remove("lizerium-modal-open");
    }
    initCategoryControls() {
        document.querySelectorAll("[data-game-category]").forEach((category) => {
            const toggleButton = category.querySelector("[data-game-category-toggle]");
            const searchInput = category.querySelector("[data-game-category-search]");
            if (toggleButton) {
                toggleButton.addEventListener("click", () => {
                    const isExpanded = toggleButton.getAttribute("aria-expanded") === "true";
                    this.setCategoryExpanded(category, !isExpanded);
                });
            }
            if (searchInput) {
                searchInput.addEventListener("input", () => this.applyCategoryProductVisibility(category));
            }
        });
    }
    setCategoryExpanded(category, isExpanded) {
        var _a, _b;
        const toggleButton = category.querySelector("[data-game-category-toggle]");
        const toggleText = toggleButton === null || toggleButton === void 0 ? void 0 : toggleButton.querySelector("[data-game-category-toggle-text]");
        const toggleIcon = toggleButton === null || toggleButton === void 0 ? void 0 : toggleButton.querySelector("[data-game-category-toggle-icon]");
        const tools = category.querySelector("[data-game-category-tools]");
        const searchInput = category.querySelector("[data-game-category-search]");
        category.classList.toggle("is-expanded", isExpanded);
        if (tools)
            tools.hidden = !isExpanded;
        if (toggleButton) {
            toggleButton.setAttribute("aria-expanded", isExpanded ? "true" : "false");
            if (toggleText)
                toggleText.innerText = isExpanded
                    ? (_a = toggleButton.dataset.hideLabel) !== null && _a !== void 0 ? _a : ""
                    : (_b = toggleButton.dataset.showLabel) !== null && _b !== void 0 ? _b : "";
            if (toggleIcon)
                toggleIcon.innerText = isExpanded ? "\u2191" : "\u2193";
        }
        if (!isExpanded && searchInput)
            searchInput.value = "";
        this.applyCategoryProductVisibility(category);
    }
    applyCategoryProductVisibility(category) {
        var _a;
        const isExpanded = category.classList.contains("is-expanded");
        const searchInput = category.querySelector("[data-game-category-search]");
        const query = (_a = searchInput === null || searchInput === void 0 ? void 0 : searchInput.value.trim().toLocaleLowerCase()) !== null && _a !== void 0 ? _a : "";
        category.querySelectorAll("[data-game-product]").forEach((product) => {
            var _a, _b;
            const productIndex = Number((_a = product.dataset.gameProductIndex) !== null && _a !== void 0 ? _a : "0");
            const searchText = ((_b = product.dataset.gameProductSearch) !== null && _b !== void 0 ? _b : product.innerText).toLocaleLowerCase();
            const isInsidePreview = productIndex < 4;
            const matchesSearch = query.length === 0 || searchText.includes(query);
            product.hidden = (!isExpanded && !isInsidePreview) || (isExpanded && !matchesSearch);
        });
    }
    setTextWithSoftBreaks(element, value) {
        element.replaceChildren();
        const parts = value.split(/(?<=[a-z])(?=[A-Z])/g);
        parts.forEach((part, index) => {
            if (index > 0)
                element.append(document.createElement("wbr"));
            element.append(document.createTextNode(part));
        });
    }
}
//# sourceMappingURL=game.js.map