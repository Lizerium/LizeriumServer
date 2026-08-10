var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class NewsAdmin {
    constructor() {
        this.newsAssets = null;
        this.newsAssetTarget = null;
        this.newsAssetModal = null;
        this.newsAssetGrid = null;
        this.newsAssetSearch = null;
        this.newsAssetPagination = null;
        this.newsAssetPage = 1;
        this.newsAssetPageSize = 20;
        this.newsAssetPageCount = 0;
        this.newsAssetTotal = 0;
        this.newsAssetQuery = "";
        this.newsAssetSearchTimer = null;
        this.imageLightbox = null;
        this.imageLightboxImage = null;
    }
    startNews() {
        const forms = document.querySelectorAll(".news-editor-card form");
        forms.forEach((form) => {
            form.addEventListener("submit", (event) => {
                event.preventDefault();
                this.submitForm(form, event.submitter);
            });
            this.refreshGalleryPreview(form);
            this.bindExistingImagePreviews(form);
        });
        this.bindMarkdownPasteUploads();
        this.enhanceNewsAssetFields(forms);
        this.ensureNewsAssetModal();
        this.ensureImageLightbox();
    }
    bindMarkdownPasteUploads() {
        document.querySelectorAll("textarea[name='MarkdownRu'], textarea[name='MarkdownEn']").forEach((textarea) => {
            textarea.addEventListener("paste", (event) => __awaiter(this, void 0, void 0, function* () {
                var _a, _b;
                const files = Array.from(((_a = event.clipboardData) === null || _a === void 0 ? void 0 : _a.files) || []);
                const itemFiles = Array.from(((_b = event.clipboardData) === null || _b === void 0 ? void 0 : _b.items) || [])
                    .filter((item) => item.type && item.type.indexOf("image/") === 0)
                    .map((item) => item.getAsFile())
                    .filter((file) => !!file);
                const image = files.concat(itemFiles).find((file) => file.type && file.type.indexOf("image/") === 0);
                if (!image)
                    return;
                event.preventDefault();
                yield this.uploadMarkdownImage(textarea, image);
            }));
        });
    }
    enhanceNewsAssetFields(forms) {
        forms.forEach((form) => {
            form.querySelectorAll("textarea[name='MarkdownRu'], textarea[name='MarkdownEn']").forEach((textarea) => {
                if (textarea.dataset.newsAssetEnhanced === "true")
                    return;
                textarea.dataset.newsAssetEnhanced = "true";
                this.addAssetButton(textarea, "Галерея", () => {
                    this.openNewsAssetModal({ type: "markdown", form, textarea });
                });
            });
            const galleryInput = form.querySelector("textarea[name='ImageGalleryJson']");
            if (galleryInput && galleryInput.dataset.newsAssetEnhanced !== "true") {
                galleryInput.dataset.newsAssetEnhanced = "true";
                galleryInput.addEventListener("input", () => this.refreshGalleryPreview(form));
                this.addAssetButton(galleryInput, "Галерея", () => {
                    this.openNewsAssetModal({ type: "gallery", form, textarea: galleryInput });
                });
            }
            const imageInput = form.querySelector("input[name='ImageUrl']");
            if (imageInput && imageInput.dataset.newsAssetEnhanced !== "true") {
                imageInput.dataset.newsAssetEnhanced = "true";
                this.addAssetButton(imageInput, "Галерея обложек", () => {
                    this.openNewsAssetModal({ type: "cover", form, input: imageInput });
                });
                imageInput.addEventListener("input", () => this.updateImagePreview(form, imageInput.value));
            }
            const iconInput = form.querySelector("input[name='IconUrl']");
            if (iconInput && iconInput.dataset.newsAssetEnhanced !== "true") {
                iconInput.dataset.newsAssetEnhanced = "true";
                this.addAssetButton(iconInput, "Галерея иконок", () => {
                    this.openNewsAssetModal({ type: "icon", form, input: iconInput });
                });
                iconInput.addEventListener("input", () => this.updateIconPreview(form, iconInput.value));
            }
        });
    }
    addAssetButton(field, text, onClick) {
        const holder = document.createElement("div");
        holder.className = "news-asset-field-tools";
        const button = document.createElement("button");
        button.type = "button";
        button.className = "admin-button muted news-asset-picker";
        button.textContent = text;
        button.addEventListener("click", onClick);
        holder.appendChild(button);
        field.parentNode.insertBefore(holder, field.nextSibling);
    }
    bindExistingImagePreviews(form) {
        form.querySelectorAll(".news-icon-preview img, .news-image-preview img, .news-gallery-preview img, .news-inline-upload-preview img").forEach((image) => {
            if (image.dataset.newsLightboxBound === "true")
                return;
            image.dataset.newsLightboxBound = "true";
            image.addEventListener("click", () => this.openImageLightbox(image.src, image.alt || ""));
        });
    }
    uploadMarkdownImage(textarea, image) {
        return __awaiter(this, void 0, void 0, function* () {
            const form = textarea.closest("form");
            const token = form ? form.querySelector("input[name='__RequestVerificationToken']") : null;
            const formData = new FormData();
            formData.append("imageFile", image, image.name || "pasted-image.png");
            if (token) {
                formData.append("__RequestVerificationToken", token.value);
            }
            const marker = "\n\n![uploading image]()\n\n";
            const selectionStart = textarea.selectionStart || 0;
            const selectionEnd = textarea.selectionEnd || selectionStart;
            textarea.setRangeText(marker, selectionStart, selectionEnd, "end");
            const response = yield fetch("/news/upload-image", {
                method: "POST",
                body: formData,
                credentials: "same-origin",
                headers: {
                    "Accept": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                }
            });
            if (!response.ok) {
                textarea.value = textarea.value.replace(marker, "\n\n[image upload failed]\n\n");
                return;
            }
            const result = yield response.json();
            const markdown = result && result.imageUrl
                ? `\n\n![image](${result.imageUrl})\n\n`
                : "\n\n[image upload failed]\n\n";
            textarea.value = textarea.value.replace(marker, markdown);
            if (result && result.imageUrl)
                this.updateInlineImagePreview(textarea, result.previewImageUrl || result.imageUrl, result.imageUrl);
        });
    }
    appendGalleryImage(form, imageUrl) {
        const galleryInput = form ? form.querySelector("textarea[name='ImageGalleryJson']") : null;
        if (!galleryInput || !imageUrl)
            return;
        let urls = [];
        const current = galleryInput.value.trim();
        if (current.length > 0) {
            try {
                const parsed = JSON.parse(current);
                if (Array.isArray(parsed))
                    urls = parsed.filter((url) => typeof url === "string" && url.trim().length > 0);
            }
            catch (_a) {
                urls = current
                    .split(/\r?\n|;|,/)
                    .map((url) => url.trim())
                    .filter((url) => url.length > 0);
            }
        }
        if (!urls.some((url) => url.toLowerCase() === imageUrl.toLowerCase()))
            urls.push(imageUrl);
        galleryInput.value = JSON.stringify(urls);
    }
    submitForm(form, submitter) {
        return __awaiter(this, void 0, void 0, function* () {
            const action = submitter && submitter.getAttribute("formaction")
                ? submitter.getAttribute("formaction")
                : form.getAttribute("action");
            const isDelete = action && action.toLowerCase().indexOf("/news/delete") >= 0;
            if (isDelete && !window.confirm("Удалить эту новость?"))
                return;
            const status = this.ensureStatus(form);
            const buttonText = submitter ? submitter.textContent : "";
            if (submitter) {
                submitter.disabled = true;
                submitter.textContent = isDelete ? "Удаление..." : "Сохранение...";
            }
            status.className = "news-save-status pending";
            status.textContent = isDelete ? "Удаляю..." : "Сохраняю...";
            try {
                const formData = new FormData(form);
                if (submitter && submitter.name && !formData.has(submitter.name)) {
                    formData.append(submitter.name, submitter.value);
                }
                const response = yield fetch(action, {
                    method: "POST",
                    body: formData,
                    credentials: "same-origin",
                    headers: {
                        "Accept": "application/json",
                        "X-Requested-With": "XMLHttpRequest"
                    }
                });
                if (response.status === 401) {
                    throw new Error("Сессия истекла. Нужно войти заново.");
                }
                if (!response.ok) {
                    throw new Error(isDelete ? "Не удалось удалить новость." : "Не удалось сохранить новость.");
                }
                const result = yield response.json();
                if (!result || !result.ok) {
                    throw new Error(isDelete ? "Не удалось удалить новость." : "Не удалось сохранить новость.");
                }
                if (isDelete) {
                    const card = form.closest(".news-editor-card");
                    if (card)
                        card.remove();
                    return;
                }
                this.applySaveResult(form, result);
                status.className = "news-save-status success";
                status.textContent = form.querySelector("input[name='Id']")
                    ? "Сохранено без перезагрузки"
                    : "Создано без перезагрузки. В списке появится после обновления текущей выборки.";
            }
            catch (error) {
                status.className = "news-save-status danger";
                status.textContent = error && error.message ? error.message : "Ошибка сохранения.";
            }
            finally {
                if (submitter) {
                    submitter.disabled = false;
                    submitter.textContent = buttonText;
                }
            }
        });
    }
    applySaveResult(form, result) {
        const idInput = form.querySelector("input[name='Id']");
        const publishedAtUnixInput = form.querySelector("input[name='PublishedAtUnix']");
        const publishedAtLocalInput = form.querySelector("input[name='publishedAtLocal']");
        const iconInput = form.querySelector("input[name='IconUrl']");
        const imageInput = form.querySelector("input[name='ImageUrl']");
        const galleryInput = form.querySelector("textarea[name='ImageGalleryJson']");
        const iconFileInput = form.querySelector("input[name='iconFile']");
        const fileInput = form.querySelector("input[name='imageFile']");
        const galleryFileInput = form.querySelector("input[name='galleryFiles']");
        if (idInput && result.id) {
            idInput.value = result.id;
        }
        if (publishedAtUnixInput && result.publishedAtUnix) {
            publishedAtUnixInput.value = result.publishedAtUnix;
        }
        if (publishedAtLocalInput && result.publishedAtLocal) {
            publishedAtLocalInput.value = result.publishedAtLocal;
        }
        if (iconInput && typeof result.iconUrl === "string") {
            iconInput.value = result.iconUrl;
        }
        if (result.iconPreviewUrl) {
            this.updateIconPreview(form, result.iconPreviewUrl);
        }
        else if (iconInput && iconInput.value.trim().length === 0) {
            const preview = form.querySelector(".news-icon-preview");
            if (preview)
                preview.remove();
        }
        if (imageInput && typeof result.imageUrl === "string") {
            imageInput.value = result.imageUrl;
        }
        if (result.previewImageUrl) {
            this.updateImagePreview(form, result.previewImageUrl);
        }
        else if (imageInput && imageInput.value.trim().length === 0) {
            const preview = form.querySelector(".news-image-preview");
            if (preview)
                preview.remove();
        }
        if (galleryInput && typeof result.imageGalleryJson === "string") {
            galleryInput.value = result.imageGalleryJson;
            this.refreshGalleryPreview(form);
        }
        if (iconFileInput) {
            iconFileInput.value = "";
            iconFileInput.removeAttribute("data-file-name");
        }
        if (fileInput) {
            fileInput.value = "";
            fileInput.removeAttribute("data-file-name");
        }
        if (galleryFileInput) {
            galleryFileInput.value = "";
            galleryFileInput.removeAttribute("data-file-name");
        }
        const removeImage = form.querySelector("input[name='removeImage']");
        if (removeImage)
            removeImage.checked = false;
        const card = form.closest(".news-editor-card");
        if (!card)
            return;
        const titleInput = form.querySelector("input[name='TitleRu']") || form.querySelector("input[name='TitleEn']");
        const summaryTitle = card.querySelector("summary span");
        const summaryStatus = card.querySelector("summary em");
        if (summaryTitle && titleInput && titleInput.value.trim().length > 0) {
            const prefix = idInput && idInput.value ? "#" + idInput.value + " " : "";
            summaryTitle.textContent = prefix + titleInput.value.trim();
        }
        if (summaryStatus) {
            const published = form.querySelector("input[name='IsPublished'][type='checkbox']");
            const isPublished = !!(published && published.checked);
            summaryStatus.className = isPublished ? "published" : "hidden";
            summaryStatus.textContent = isPublished ? "опубликована" : "скрыта";
        }
    }
    updateIconPreview(form, previewImageUrl) {
        let preview = form.querySelector(".news-icon-preview");
        if (!preview) {
            const iconUpload = form.querySelector(".news-icon-upload");
            preview = document.createElement("div");
            preview.className = "news-icon-preview";
            preview.innerHTML = "<span>Иконка продукта</span><img alt=\"\" loading=\"lazy\" onerror=\"this.closest('.news-icon-preview')?.classList.add('broken');\" />";
            if (iconUpload && iconUpload.parentNode) {
                iconUpload.parentNode.insertBefore(preview, iconUpload.nextSibling);
            }
            else {
                form.appendChild(preview);
            }
        }
        const image = preview.querySelector("img");
        if (image) {
            image.src = this.toNewsPreviewUrl(previewImageUrl);
            this.bindNewsImageFallback(image, previewImageUrl);
            if (image.dataset.newsLightboxBound !== "true") {
                image.dataset.newsLightboxBound = "true";
                image.addEventListener("click", () => this.openImageLightbox(image.currentSrc || image.src, image.alt || ""));
            }
        }
        preview.classList.remove("broken");
    }
    updateImagePreview(form, previewImageUrl) {
        let preview = form.querySelector(".news-image-preview");
        if (!preview) {
            const imageUpload = form.querySelector(".news-cover-upload");
            preview = document.createElement("div");
            preview.className = "news-image-preview";
            preview.innerHTML = "<span>Обложка карточки</span><img alt=\"\" loading=\"lazy\" onerror=\"this.closest('.news-image-preview')?.classList.add('broken');\" />";
            if (imageUpload && imageUpload.parentNode) {
                imageUpload.parentNode.insertBefore(preview, imageUpload.nextSibling);
            }
            else {
                form.appendChild(preview);
            }
        }
        const image = preview.querySelector("img");
        if (image) {
            image.src = this.toNewsPreviewUrl(previewImageUrl);
            this.bindNewsImageFallback(image, previewImageUrl);
            if (image.dataset.newsLightboxBound !== "true") {
                image.dataset.newsLightboxBound = "true";
                image.addEventListener("click", () => this.openImageLightbox(image.currentSrc || image.src, image.alt || ""));
            }
        }
        preview.classList.remove("broken");
    }
    updateGalleryPreview(form, imageUrl) {
        if (!form || !imageUrl)
            return;
        this.appendGalleryImage(form, imageUrl);
        this.refreshGalleryPreview(form);
    }
    updateInlineImagePreview(textarea, imageUrl, markdownUrl) {
        const form = textarea ? textarea.closest("form") : null;
        if (!form || !imageUrl)
            return;
        let preview = textarea.parentNode ? textarea.parentNode.querySelector(".news-inline-upload-preview") : null;
        if (!preview) {
            preview = document.createElement("div");
            preview.className = "news-inline-upload-preview";
            preview.innerHTML = "<span>Inserted in Markdown</span><div></div>";
            textarea.parentNode.insertBefore(preview, textarea.nextSibling);
        }
        const list = preview.querySelector("div");
        if (!list)
            return;
        const inlineItem = document.createElement("span");
        inlineItem.className = "news-inline-item";
        const image = document.createElement("img");
        image.src = this.toNewsPreviewUrl(imageUrl);
        image.alt = "";
        image.loading = "lazy";
        this.bindNewsImageFallback(image, imageUrl);
        image.addEventListener("click", () => this.openImageLightbox(image.currentSrc || image.src, image.alt || ""));
        inlineItem.appendChild(image);
        const removeButton = document.createElement("button");
        removeButton.type = "button";
        removeButton.className = "news-inline-remove";
        removeButton.setAttribute("aria-label", "Удалить картинку из Markdown");
        removeButton.title = "Удалить картинку из Markdown";
        removeButton.textContent = "×";
        removeButton.addEventListener("click", () => {
            this.removeInlineMarkdownImage(textarea, markdownUrl || imageUrl, inlineItem);
        });
        inlineItem.appendChild(removeButton);
        list.appendChild(inlineItem);
    }
    removeInlineMarkdownImage(textarea, imageUrl, item) {
        if (!textarea || !imageUrl)
            return;
        const escaped = this.escapeRegExp(imageUrl);
        const markdownImage = new RegExp(`\\n{0,2}!\\[[^\\]]*\\]\\(${escaped}\\)\\n{0,2}`, "g");
        textarea.value = textarea.value.replace(markdownImage, "\n\n");
        if (item)
            item.remove();
    }
    refreshGalleryPreview(form) {
        var _a;
        const galleryInput = form ? form.querySelector("textarea[name='ImageGalleryJson']") : null;
        if (!galleryInput)
            return;
        const urls = this.parseGalleryUrls(galleryInput.value);
        let preview = form.querySelector(".news-gallery-preview");
        if (!preview) {
            const galleryUpload = (_a = form.querySelector("input[name='galleryFiles']")) === null || _a === void 0 ? void 0 : _a.closest(".news-image-upload");
            preview = document.createElement("div");
            preview.className = "news-gallery-preview";
            preview.innerHTML = "<span>Gallery</span><div></div>";
            if (galleryUpload && galleryUpload.parentNode)
                galleryUpload.parentNode.insertBefore(preview, galleryUpload.nextSibling);
            else
                form.appendChild(preview);
        }
        const list = preview.querySelector("div");
        if (!list)
            return;
        list.innerHTML = "";
        if (urls.length === 0) {
            preview.hidden = true;
            return;
        }
        preview.hidden = false;
        urls.forEach((url) => {
            const item = document.createElement("span");
            item.className = "news-gallery-item";
            item.innerHTML = `
                <img src="${this.escapeAttribute(this.toNewsPreviewUrl(url))}" data-fallback-src="${this.escapeAttribute(this.toPublicNewsImageUrl(url))}" alt="" loading="lazy" />
                <button type="button" class="news-gallery-remove" title="Detach image">Detach</button>`;
            const image = item.querySelector("img");
            this.bindNewsImageFallback(image, url);
            image.addEventListener("click", () => this.openImageLightbox(image.currentSrc || image.src, ""));
            item.querySelector("button").addEventListener("click", () => this.detachGalleryImage(form, url));
            list.appendChild(item);
        });
    }
    detachGalleryImage(form, imageUrl) {
        const galleryInput = form ? form.querySelector("textarea[name='ImageGalleryJson']") : null;
        if (!galleryInput)
            return;
        const urls = this.parseGalleryUrls(galleryInput.value)
            .filter((url) => url.toLowerCase() !== imageUrl.toLowerCase());
        galleryInput.value = urls.length === 0 ? "" : JSON.stringify(urls);
        this.refreshGalleryPreview(form);
    }
    parseGalleryUrls(value) {
        const current = (value || "").trim();
        if (!current)
            return [];
        try {
            const parsed = JSON.parse(current);
            if (Array.isArray(parsed))
                return parsed.filter((url) => typeof url === "string" && url.trim().length > 0);
        }
        catch (_a) {
            return current
                .split(/\r?\n|;|,/)
                .map((url) => url.trim())
                .filter((url) => url.length > 0);
        }
        return [];
    }
    ensureNewsAssetModal() {
        if (this.newsAssetModal)
            return;
        this.newsAssetModal = document.createElement("div");
        this.newsAssetModal.className = "news-asset-modal";
        this.newsAssetModal.hidden = true;
        this.newsAssetModal.innerHTML = `
            <div class="news-asset-dialog" role="dialog" aria-modal="true" aria-label="News image gallery">
                <div class="news-asset-head">
                    <div>
                        <span>News images</span>
                        <strong>Select, attach, or delete uploaded files</strong>
                    </div>
                    <button type="button" class="news-asset-close" aria-label="Close">x</button>
                </div>
                <div class="news-asset-search-row">
                    <input class="news-asset-search" type="search" placeholder="Search images by name, folder, or URL..." />
                </div>
                <div class="news-asset-grid"></div>
                <div class="news-asset-pagination"></div>
            </div>`;
        document.body.appendChild(this.newsAssetModal);
        this.newsAssetGrid = this.newsAssetModal.querySelector(".news-asset-grid");
        this.newsAssetSearch = this.newsAssetModal.querySelector(".news-asset-search");
        this.newsAssetPagination = this.newsAssetModal.querySelector(".news-asset-pagination");
        this.newsAssetModal.querySelector(".news-asset-close").addEventListener("click", () => this.closeNewsAssetModal());
        this.newsAssetModal.addEventListener("click", (event) => {
            if (event.target === this.newsAssetModal)
                this.closeNewsAssetModal();
        });
        this.newsAssetSearch.addEventListener("input", () => this.queueNewsAssetSearch());
        document.addEventListener("keydown", (event) => {
            if (event.key === "Escape" && this.newsAssetModal && !this.newsAssetModal.hidden)
                this.closeNewsAssetModal();
        });
    }
    openNewsAssetModal(target) {
        return __awaiter(this, void 0, void 0, function* () {
            this.ensureNewsAssetModal();
            this.newsAssetTarget = target;
            this.newsAssetModal.hidden = false;
            document.body.classList.add("news-asset-modal-open");
            this.newsAssetSearch.value = "";
            this.newsAssetQuery = "";
            this.newsAssetPage = 1;
            yield this.loadNewsAssets();
            this.newsAssetSearch.focus();
        });
    }
    closeNewsAssetModal() {
        if (!this.newsAssetModal)
            return;
        this.newsAssetModal.hidden = true;
        document.body.classList.remove("news-asset-modal-open");
    }
    loadNewsAssets() {
        return __awaiter(this, void 0, void 0, function* () {
            this.setNewsAssetLoading();
            const params = new URLSearchParams({
                page: this.newsAssetPage.toString(),
                pageSize: this.newsAssetPageSize.toString()
            });
            if (this.newsAssetQuery)
                params.set("query", this.newsAssetQuery);
            const response = yield fetch(`/news/assets?${params.toString()}`, {
                headers: {
                    "Accept": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                credentials: "same-origin"
            });
            if (!response.ok) {
                this.newsAssets = [];
                this.newsAssetTotal = 0;
                this.newsAssetPageCount = 0;
                this.renderNewsAssets();
                return;
            }
            const result = yield response.json();
            this.newsAssets = result && Array.isArray(result.assets) ? result.assets : [];
            this.newsAssetPage = result && result.page ? result.page : 1;
            this.newsAssetPageSize = result && result.pageSize ? result.pageSize : this.newsAssetPageSize;
            this.newsAssetTotal = result && result.total ? result.total : 0;
            this.newsAssetPageCount = result && result.pageCount ? result.pageCount : 0;
            this.renderNewsAssets();
        });
    }
    renderNewsAssets() {
        const assets = this.newsAssets || [];
        if (assets.length === 0) {
            this.newsAssetGrid.innerHTML = "<div class=\"news-asset-empty\">No images found</div>";
            this.renderNewsAssetPagination();
            return;
        }
        this.newsAssetGrid.innerHTML = "";
        assets.forEach((asset) => {
            const card = document.createElement("div");
            card.className = "news-asset-card";
            card.innerHTML = `
                <span class="news-asset-thumb"><img src="${this.escapeAttribute(asset.previewUrl || this.toNewsPreviewUrl(asset.url))}" alt="" loading="lazy" /></span>
                <span class="news-asset-meta">
                    <strong>${this.escapeHtml(asset.name || asset.url)}</strong>
                    <small>${this.escapeHtml(asset.group || "/img/news")}</small>
                </span>
                <span class="news-asset-actions">
                    <button type="button" data-action="insert">Insert</button>
                    <button type="button" data-action="gallery">Add</button>
                    <button type="button" data-action="cover">Cover</button>
                    <button type="button" data-action="delete" class="danger">Delete</button>
                </span>`;
            card.querySelector("[data-action='insert']").addEventListener("click", () => this.useNewsAsset(asset, "insert"));
            card.querySelector("[data-action='gallery']").addEventListener("click", () => this.useNewsAsset(asset, "gallery"));
            card.querySelector("[data-action='cover']").addEventListener("click", () => this.useNewsAsset(asset, "cover"));
            card.querySelector("[data-action='delete']").addEventListener("click", () => this.deleteNewsAsset(asset));
            card.querySelector(".news-asset-thumb img").addEventListener("click", () => {
                this.openImageLightbox(asset.previewUrl || this.toNewsPreviewUrl(asset.url), asset.name || "");
            });
            this.newsAssetGrid.appendChild(card);
        });
        this.renderNewsAssetPagination();
    }
    setNewsAssetLoading() {
        this.newsAssetGrid.innerHTML = "<div class=\"news-asset-empty\">Loading images...</div>";
        if (this.newsAssetPagination)
            this.newsAssetPagination.innerHTML = "";
    }
    queueNewsAssetSearch() {
        window.clearTimeout(this.newsAssetSearchTimer);
        this.newsAssetSearchTimer = window.setTimeout(() => __awaiter(this, void 0, void 0, function* () {
            this.newsAssetQuery = (this.newsAssetSearch.value || "").trim();
            this.newsAssetPage = 1;
            yield this.loadNewsAssets();
        }), 220);
    }
    changeNewsAssetPage(nextPage) {
        return __awaiter(this, void 0, void 0, function* () {
            if (nextPage < 1 || (this.newsAssetPageCount > 0 && nextPage > this.newsAssetPageCount))
                return;
            this.newsAssetPage = nextPage;
            yield this.loadNewsAssets();
        });
    }
    renderNewsAssetPagination() {
        if (!this.newsAssetPagination)
            return;
        const from = this.newsAssetTotal === 0 ? 0 : ((this.newsAssetPage - 1) * this.newsAssetPageSize) + 1;
        const to = Math.min(this.newsAssetTotal, this.newsAssetPage * this.newsAssetPageSize);
        this.newsAssetPagination.innerHTML = `
            <span>${from}-${to} of ${this.newsAssetTotal}</span>
            <div>
                <button type="button" data-page="prev"${this.newsAssetPage <= 1 ? " disabled" : ""}>Prev</button>
                <strong>${this.newsAssetPageCount === 0 ? 0 : this.newsAssetPage} / ${this.newsAssetPageCount}</strong>
                <button type="button" data-page="next"${this.newsAssetPage >= this.newsAssetPageCount ? " disabled" : ""}>Next</button>
            </div>`;
        this.newsAssetPagination.querySelector("[data-page='prev']").addEventListener("click", () => this.changeNewsAssetPage(this.newsAssetPage - 1));
        this.newsAssetPagination.querySelector("[data-page='next']").addEventListener("click", () => this.changeNewsAssetPage(this.newsAssetPage + 1));
    }
    useNewsAsset(asset, action) {
        const target = this.newsAssetTarget;
        if (!target || !asset || !asset.url)
            return;
        if (action === "insert")
            action = target.type || "markdown";
        if (action === "markdown" && target.textarea) {
            this.insertMarkdownImage(target.textarea, asset.url);
            this.updateInlineImagePreview(target.textarea, asset.previewUrl || asset.url, asset.url);
        }
        if (action === "gallery" && target.form) {
            this.appendGalleryImage(target.form, asset.url);
            this.refreshGalleryPreview(target.form);
        }
        if (action === "cover" && target.form) {
            const input = target.input || target.form.querySelector("input[name='ImageUrl']");
            if (input) {
                input.value = asset.url;
                input.dispatchEvent(new Event("input", { bubbles: true }));
            }
        }
        if (action === "icon" && target.form) {
            const input = target.input || target.form.querySelector("input[name='IconUrl']");
            if (input) {
                input.value = asset.url;
                input.dispatchEvent(new Event("input", { bubbles: true }));
            }
        }
        this.closeNewsAssetModal();
    }
    insertMarkdownImage(textarea, imageUrl) {
        const markdown = `\n\n![image](${imageUrl})\n\n`;
        const selectionStart = textarea.selectionStart || 0;
        const selectionEnd = textarea.selectionEnd || selectionStart;
        textarea.setRangeText(markdown, selectionStart, selectionEnd, "end");
        textarea.focus();
    }
    deleteNewsAsset(asset) {
        return __awaiter(this, void 0, void 0, function* () {
            if (!asset || !asset.url || !window.confirm("Delete this physical image file?"))
                return;
            const token = this.newsAssetTarget && this.newsAssetTarget.form
                ? this.newsAssetTarget.form.querySelector("input[name='__RequestVerificationToken']")
                : document.querySelector("input[name='__RequestVerificationToken']");
            const formData = new FormData();
            formData.append("url", asset.url);
            if (token)
                formData.append("__RequestVerificationToken", token.value);
            const response = yield fetch("/news/assets/delete", {
                method: "POST",
                body: formData,
                credentials: "same-origin",
                headers: {
                    "Accept": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                }
            });
            if (!response.ok) {
                window.alert("Failed to delete image.");
                return;
            }
            this.newsAssets = (this.newsAssets || []).filter((item) => item.url !== asset.url);
            this.newsAssetTotal = Math.max(0, this.newsAssetTotal - 1);
            if (this.newsAssets.length === 0 && this.newsAssetPage > 1)
                this.newsAssetPage--;
            yield this.loadNewsAssets();
        });
    }
    ensureImageLightbox() {
        if (this.imageLightbox)
            return;
        this.imageLightbox = document.createElement("div");
        this.imageLightbox.className = "news-image-lightbox";
        this.imageLightbox.hidden = true;
        this.imageLightbox.innerHTML = `
            <div class="news-lightbox-dialog" role="dialog" aria-modal="true" aria-label="Image preview">
                <button type="button" class="news-lightbox-close" aria-label="Close">×</button>
                <img alt="" />
            </div>`;
        document.body.appendChild(this.imageLightbox);
        this.imageLightboxImage = this.imageLightbox.querySelector("img");
        this.imageLightbox.querySelector(".news-lightbox-close").addEventListener("click", () => this.closeImageLightbox());
        this.imageLightbox.addEventListener("click", (event) => {
            if (event.target === this.imageLightbox)
                this.closeImageLightbox();
        });
        document.addEventListener("keydown", (event) => {
            if (event.key === "Escape" && this.imageLightbox && !this.imageLightbox.hidden)
                this.closeImageLightbox();
        });
    }
    openImageLightbox(imageUrl, altText) {
        if (!imageUrl)
            return;
        this.ensureImageLightbox();
        this.imageLightboxImage.src = imageUrl;
        this.imageLightboxImage.alt = altText || "";
        this.imageLightbox.hidden = false;
        if (this.newsAssetModal)
            this.newsAssetModal.classList.add("is-behind-lightbox");
        document.body.classList.add("news-image-lightbox-open");
    }
    closeImageLightbox() {
        if (!this.imageLightbox)
            return;
        this.imageLightbox.hidden = true;
        this.imageLightboxImage.removeAttribute("src");
        if (this.newsAssetModal)
            this.newsAssetModal.classList.remove("is-behind-lightbox");
        document.body.classList.remove("news-image-lightbox-open");
    }
    toNewsPreviewUrl(url) {
        if (!url)
            return "";
        if (url.indexOf("/img/news/") === 0)
            return `/news/assets/preview?url=${encodeURIComponent(url)}`;
        return url;
    }
    toPublicNewsImageUrl(url) {
        if (!url || url.indexOf("/img/news/") !== 0)
            return "";
        return `https://lizup.ru${url}`;
    }
    bindNewsImageFallback(image, rawUrl) {
        if (!image)
            return;
        const fallbackUrl = this.toPublicNewsImageUrl(rawUrl || image.getAttribute("src") || "");
        if (fallbackUrl)
            image.dataset.fallbackSrc = fallbackUrl;
        if (image.dataset.newsFallbackBound === "true")
            return;
        image.dataset.newsFallbackBound = "true";
        image.addEventListener("error", () => {
            var _a;
            const fallback = image.dataset.fallbackSrc || "";
            if (fallback && image.src.indexOf(fallback) < 0) {
                image.src = fallback;
                return;
            }
            (_a = image.closest(".news-icon-preview, .news-image-preview")) === null || _a === void 0 ? void 0 : _a.classList.add("broken");
        });
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
    escapeRegExp(value) {
        return String(value).replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
    }
    ensureStatus(form) {
        let status = form.querySelector(".news-save-status");
        if (status)
            return status;
        status = document.createElement("div");
        status.className = "news-save-status";
        const actions = form.querySelector(".news-editor-actions");
        if (actions) {
            actions.prepend(status);
        }
        else {
            form.appendChild(status);
        }
        return status;
    }
}
