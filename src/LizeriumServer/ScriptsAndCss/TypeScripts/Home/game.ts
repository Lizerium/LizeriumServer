export class Game {
    private readonly modal: HTMLElement | null;
    private readonly modalIcon: HTMLImageElement | null;
    private readonly modalTitle: HTMLElement | null;
    private readonly modalDescription: HTMLElement | null;
    private readonly modalSources: HTMLElement | null;

    constructor() {
        this.modal = document.querySelector("[data-game-download-modal]");
        this.modalIcon = document.querySelector("[data-game-download-icon]");
        this.modalTitle = document.querySelector("[data-game-download-title]");
        this.modalDescription = document.querySelector("[data-game-download-description]");
        this.modalSources = document.querySelector("[data-game-download-sources]");
    }

    public start(): void {
        document.querySelectorAll<HTMLElement>("[data-game-product-title]").forEach((title) => {
            this.setTextWithSoftBreaks(title, title.innerText.trim());
        });

        document.querySelectorAll<HTMLButtonElement>("[data-game-download]").forEach((button) => {
            button.addEventListener("click", () => this.openDownloadModal(button));
        });

        this.initCategoryControls();

        document.querySelectorAll<HTMLElement>("[data-game-download-close]").forEach((closeControl) => {
            closeControl.addEventListener("click", () => this.closeDownloadModal());
        });

        document.addEventListener("keydown", (event) => {
            if (event.key === "Escape")
                this.closeDownloadModal();
        });
    }

    private openDownloadModal(button: HTMLButtonElement): void {
        const product = button.closest<HTMLElement>("[data-game-product]");
        if (!product || !this.modal || !this.modalSources)
            return;

        const title = product.querySelector<HTMLElement>("[data-game-product-title]")?.innerText.trim() ?? "";
        const description = product.querySelector<HTMLElement>("[data-game-product-description]")?.innerText.trim() ?? "";
        const productIcon = button.dataset.productIcon ?? "";

        if (this.modalTitle)
            this.setTextWithSoftBreaks(this.modalTitle, title);

        if (this.modalDescription)
            this.modalDescription.innerText = description;

        if (this.modalIcon) {
            this.modalIcon.src = productIcon;
            this.modalIcon.alt = "";
        }

        this.modalSources.innerHTML = "";
        product.querySelectorAll<HTMLAnchorElement>("[data-game-source]").forEach((source) => {
            const link = document.createElement("a");
            link.className = "lizerium-game-source-link";
            link.href = source.href;
            link.target = "_blank";
            link.rel = "noopener noreferrer";

            const icon = document.createElement("img");
            icon.src = source.dataset.sourceIcon ?? "";
            icon.alt = "";
            icon.loading = "lazy";

            const text = document.createElement("span");
            text.innerText = source.dataset.sourceName ?? source.innerText.trim();

            const arrow = document.createElement("span");
            arrow.innerText = "\u2197";
            arrow.setAttribute("aria-hidden", "true");

            link.append(icon, text, arrow);
            this.modalSources?.append(link);
        });

        this.modal.hidden = false;
        document.body.classList.add("lizerium-modal-open");
    }

    private closeDownloadModal(): void {
        if (!this.modal || this.modal.hidden)
            return;

        this.modal.hidden = true;
        document.body.classList.remove("lizerium-modal-open");
    }

    private initCategoryControls(): void {
        document.querySelectorAll<HTMLElement>("[data-game-category]").forEach((category) => {
            const toggleButton = category.querySelector<HTMLButtonElement>("[data-game-category-toggle]");
            const searchInput = category.querySelector<HTMLInputElement>("[data-game-category-search]");

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

    private setCategoryExpanded(category: HTMLElement, isExpanded: boolean): void {
        const toggleButton = category.querySelector<HTMLButtonElement>("[data-game-category-toggle]");
        const toggleText = toggleButton?.querySelector<HTMLElement>("[data-game-category-toggle-text]");
        const toggleIcon = toggleButton?.querySelector<HTMLElement>("[data-game-category-toggle-icon]");
        const tools = category.querySelector<HTMLElement>("[data-game-category-tools]");
        const searchInput = category.querySelector<HTMLInputElement>("[data-game-category-search]");

        category.classList.toggle("is-expanded", isExpanded);

        if (tools)
            tools.hidden = !isExpanded;

        if (toggleButton) {
            toggleButton.setAttribute("aria-expanded", isExpanded ? "true" : "false");

            if (toggleText)
                toggleText.innerText = isExpanded
                    ? toggleButton.dataset.hideLabel ?? ""
                    : toggleButton.dataset.showLabel ?? "";

            if (toggleIcon)
                toggleIcon.innerText = isExpanded ? "\u2191" : "\u2193";
        }

        if (!isExpanded && searchInput)
            searchInput.value = "";

        this.applyCategoryProductVisibility(category);
    }

    private applyCategoryProductVisibility(category: HTMLElement): void {
        const isExpanded = category.classList.contains("is-expanded");
        const searchInput = category.querySelector<HTMLInputElement>("[data-game-category-search]");
        const query = searchInput?.value.trim().toLocaleLowerCase() ?? "";

        category.querySelectorAll<HTMLElement>("[data-game-product]").forEach((product) => {
            const productIndex = Number(product.dataset.gameProductIndex ?? "0");
            const searchText = (product.dataset.gameProductSearch ?? product.innerText).toLocaleLowerCase();
            const isInsidePreview = productIndex < 4;
            const matchesSearch = query.length === 0 || searchText.includes(query);

            product.hidden = (!isExpanded && !isInsidePreview) || (isExpanded && !matchesSearch);
        });
    }

    private setTextWithSoftBreaks(element: HTMLElement, value: string): void {
        element.replaceChildren();

        const parts = value.split(/(?<=[a-z])(?=[A-Z])/g);
        parts.forEach((part, index) => {
            if (index > 0)
                element.append(document.createElement("wbr"));

            element.append(document.createTextNode(part));
        });
    }
}
