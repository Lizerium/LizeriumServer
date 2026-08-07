class ProductsAdmin {
    constructor() {
        this.assets = null;
        this.activeInput = null;
        this.modal = null;
        this.modalGrid = null;
        this.modalSearch = null;
    }

    startProducts() {
        const page = document.querySelector(".products-admin-page");
        if (!page)
            return;

        this.enhanceCollapsing(page);
        this.enhanceSaving(page);
        this.enhanceImageFields(page);
        this.ensureAssetModal();
    }

    // Adds collapsible sections without changing the server-rendered form structure.
    enhanceCollapsing(page) {
        const createPanel = page.querySelector(".products-admin-create");
        if (createPanel) {
            const title = createPanel.querySelector("h3");
            if (title)
                this.addToggle(title, createPanel, "Развернуть создание категории", true);
        }

        page.querySelectorAll(".products-admin-category").forEach((category) => {
            const head = category.querySelector(".products-admin-category-head");
            if (!head)
                return;

            this.addToggle(head, category, "Развернуть категорию", true);
        });

        page.querySelectorAll(".products-admin-product").forEach((product) => {
            const head = product.querySelector(".products-admin-product-head");
            if (!head)
                return;

            this.addToggle(head, product, "Развернуть продукт", true);
        });

        page.querySelectorAll(".products-admin-products > .products-admin-form").forEach((form) => {
            this.addInlineFormToggle(form, "Добавить продукт");
        });

        page.querySelectorAll(".products-admin-links > .products-admin-form:not(.products-admin-link)").forEach((form) => {
            this.addInlineFormToggle(form, "Добавить источник");
        });
    }

    addToggle(head, target, label, collapsed) {
        if (head.querySelector(".products-admin-toggle"))
            return;

        const button = document.createElement("button");
        button.type = "button";
        button.className = "products-admin-toggle";
        button.setAttribute("aria-label", label);
        button.innerHTML = "<span></span>";

        const setState = (isCollapsed) => {
            target.classList.toggle("is-collapsed", isCollapsed);
            button.setAttribute("aria-expanded", (!isCollapsed).toString());
        };

        button.addEventListener("click", (event) => {
            event.preventDefault();
            setState(!target.classList.contains("is-collapsed"));
        });

        head.appendChild(button);
        setState(collapsed);
    }

    addInlineFormToggle(form, label) {
        if (form.dataset.productsInlineToggle === "true")
            return;

        form.dataset.productsInlineToggle = "true";
        form.classList.add("products-admin-inline-form", "is-collapsed");

        const button = document.createElement("button");
        button.type = "button";
        button.className = "products-admin-inline-toggle";
        button.textContent = label;
        button.setAttribute("aria-expanded", "false");
        button.addEventListener("click", () => {
            const collapsed = form.classList.toggle("is-collapsed");
            button.setAttribute("aria-expanded", (!collapsed).toString());
        });

        form.parentNode.insertBefore(button, form);
    }

    // Converts product forms to AJAX saves; non-JS fallback still uses normal POST redirects.
    enhanceSaving(page) {
        page.querySelectorAll(".products-admin-form").forEach((form) => {
            if (form.dataset.productsAjaxSave === "true")
                return;

            form.dataset.productsAjaxSave = "true";
            form.addEventListener("submit", (event) => this.submitForm(event, form));
        });
    }

    async submitForm(event, form) {
        event.preventDefault();

        if (typeof form.reportValidity === "function" && !form.reportValidity())
            return;

        const submitButton = form.querySelector("button[type='submit']");
        const originalText = submitButton ? submitButton.textContent : "";
        const isNewRecord = !this.getFormId(form);

        this.setSubmitState(submitButton, true, "Saving...");

        try {
            const response = await fetch(form.action, {
                method: form.method || "POST",
                body: new FormData(form),
                credentials: "same-origin",
                headers: {
                    "Accept": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                }
            });

            if (!response.ok)
                throw new Error(`HTTP ${response.status}`);

            const result = await response.json();
            if (!result || result.ok !== true)
                throw new Error(result && result.message ? result.message : "Save failed");

            this.setSubmitState(submitButton, false, isNewRecord ? "Created" : "Saved");
            form.classList.add("is-saved");

            if (isNewRecord) {
                window.setTimeout(() => window.location.reload(), 450);
                return;
            }

            window.setTimeout(() => {
                form.classList.remove("is-saved");
                this.setSubmitState(submitButton, false, originalText);
            }, 1100);
        } catch (error) {
            console.error(error);
            form.classList.add("is-save-error");
            this.setSubmitState(submitButton, false, "Error");

            window.setTimeout(() => {
                form.classList.remove("is-save-error");
                this.setSubmitState(submitButton, false, originalText);
            }, 1600);
        }
    }

    getFormId(form) {
        const input = form.querySelector("input[name='Id']");
        return input && input.value ? input.value : "";
    }

    setSubmitState(button, disabled, text) {
        if (!button)
            return;

        button.disabled = disabled;
        if (text)
            button.textContent = text;
    }

    // Adds image library/upload controls beside IconUrl and BackgroundUrl inputs.
    enhanceImageFields(page) {
        const inputs = page.querySelectorAll("input[name='IconUrl'], input[name='BackgroundUrl']");
        inputs.forEach((input) => {
            if (input.dataset.productsImageEnhanced === "true")
                return;

            input.dataset.productsImageEnhanced = "true";
            const label = input.closest("label");
            if (label)
                label.classList.add("products-image-field");

            const tools = document.createElement("div");
            tools.className = "products-image-tools";

            const libraryButton = document.createElement("button");
            libraryButton.type = "button";
            libraryButton.className = "products-image-action";
            libraryButton.textContent = "Библиотека";
            libraryButton.addEventListener("click", () => this.openAssetModal(input));

            const uploadLabel = document.createElement("label");
            uploadLabel.className = "products-image-upload";
            uploadLabel.innerHTML = "<input type=\"file\" accept=\"image/*,.svg\" /><span>Загрузить</span>";
            const uploadInput = uploadLabel.querySelector("input");
            uploadInput.addEventListener("change", () => this.uploadImage(input, uploadInput));

            const preview = document.createElement("span");
            preview.className = "products-image-preview";

            tools.appendChild(libraryButton);
            tools.appendChild(uploadLabel);
            tools.appendChild(preview);
            input.parentNode.insertBefore(tools, input.nextSibling);

            const updatePreview = () => this.updatePreview(input, preview);
            input.addEventListener("input", updatePreview);
            updatePreview();
        });
    }

    // Shared asset picker modal. Assets are fetched only when the modal is first opened.
    ensureAssetModal() {
        if (this.modal)
            return;

        this.modal = document.createElement("div");
        this.modal.className = "products-asset-modal";
        this.modal.hidden = true;
        this.modal.innerHTML = `
            <div class="products-asset-dialog" role="dialog" aria-modal="true" aria-label="Выбор изображения">
                <div class="products-asset-head">
                    <div>
                        <span>Библиотека изображений</span>
                        <strong>Выберите существующий файл</strong>
                    </div>
                    <button type="button" class="products-asset-close" aria-label="Закрыть">×</button>
                </div>
                <input class="products-asset-search" type="search" placeholder="Найти изображение..." />
                <div class="products-asset-grid"></div>
            </div>`;

        document.body.appendChild(this.modal);
        this.modalGrid = this.modal.querySelector(".products-asset-grid");
        this.modalSearch = this.modal.querySelector(".products-asset-search");

        this.modal.querySelector(".products-asset-close").addEventListener("click", () => this.closeAssetModal());
        this.modal.addEventListener("click", (event) => {
            if (event.target === this.modal)
                this.closeAssetModal();
        });
        this.modalSearch.addEventListener("input", () => this.renderAssets());
        document.addEventListener("keydown", (event) => {
            if (event.key === "Escape" && this.modal && !this.modal.hidden)
                this.closeAssetModal();
        });
    }

    async openAssetModal(input) {
        this.ensureAssetModal();
        this.activeInput = input;
        this.modal.hidden = false;
        document.body.classList.add("products-asset-modal-open");
        this.modalSearch.value = "";
        this.modalGrid.innerHTML = "<div class=\"products-asset-empty\">Загрузка...</div>";

        if (!this.assets)
            await this.loadAssets();

        this.renderAssets();
        this.modalSearch.focus();
    }

    closeAssetModal() {
        if (!this.modal)
            return;

        this.modal.hidden = true;
        document.body.classList.remove("products-asset-modal-open");
    }

    async loadAssets() {
        const response = await fetch("/products/assets", {
            headers: {
                "Accept": "application/json",
                "X-Requested-With": "XMLHttpRequest"
            },
            credentials: "same-origin"
        });

        if (!response.ok) {
            this.assets = [];
            return;
        }

        const result = await response.json();
        this.assets = result && Array.isArray(result.assets) ? result.assets : [];
    }

    // Renders the cached image list and applies client-side search inside the modal.
    renderAssets() {
        const query = (this.modalSearch.value || "").trim().toLowerCase();
        const assets = (this.assets || []).filter((asset) => {
            const haystack = `${asset.name || ""} ${asset.group || ""} ${asset.url || ""}`.toLowerCase();
            return !query || haystack.indexOf(query) >= 0;
        });

        if (assets.length === 0) {
            this.modalGrid.innerHTML = "<div class=\"products-asset-empty\">Изображения не найдены</div>";
            return;
        }

        this.modalGrid.innerHTML = "";
        assets.forEach((asset) => {
            const previewUrl = asset.previewUrl || this.toPreviewUrl(asset.url);
            const button = document.createElement("button");
            button.type = "button";
            button.className = "products-asset-card";
            button.innerHTML = `
                <span class="products-asset-thumb"><img src="${this.escapeAttribute(previewUrl)}" alt="" loading="lazy" /></span>
                <span class="products-asset-meta">
                    <strong>${this.escapeHtml(asset.name || asset.url)}</strong>
                    <small>${this.escapeHtml(asset.group || "/img")}</small>
                </span>`;

            button.addEventListener("click", () => {
                if (this.activeInput) {
                    this.activeInput.value = asset.url;
                    this.activeInput.dispatchEvent(new Event("input", { bubbles: true }));
                }

                this.closeAssetModal();
            });

            this.modalGrid.appendChild(button);
        });
    }

    // Uploads a new product image, then writes the returned /img/... URL into the active input.
    async uploadImage(input, uploadInput) {
        const file = uploadInput.files && uploadInput.files[0];
        if (!file)
            return;

        const form = input.closest("form") || document.querySelector("form");
        const token = form ? form.querySelector("input[name='__RequestVerificationToken']") : null;
        const formData = new FormData();
        formData.append("imageFile", file, file.name);

        if (token)
            formData.append("__RequestVerificationToken", token.value);

        const response = await fetch("/products/assets/upload", {
            method: "POST",
            body: formData,
            credentials: "same-origin",
            headers: {
                "Accept": "application/json",
                "X-Requested-With": "XMLHttpRequest"
            }
        });

        uploadInput.value = "";

        if (!response.ok) {
            window.alert("Не удалось загрузить изображение.");
            return;
        }

        const result = await response.json();
        if (result && result.url) {
            input.value = result.url;
            input.dispatchEvent(new Event("input", { bubbles: true }));
            this.assets = null;
        }
    }

    updatePreview(input, preview) {
        const url = (input.value || "").trim();
        if (!url) {
            preview.innerHTML = "";
            preview.hidden = true;
            return;
        }

        preview.hidden = false;
        preview.innerHTML = `<img src="${this.escapeAttribute(this.toPreviewUrl(url))}" alt="" loading="lazy" />`;
    }

    toPreviewUrl(url) {
        if (!url)
            return "";

        if (url.indexOf("/img/") === 0)
            return `/products/assets/preview?url=${encodeURIComponent(url)}`;

        return url;
    }

    escapeHtml(value) {
        return String(value)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

    escapeAttribute(value) {
        return this.escapeHtml(value);
    }
}
