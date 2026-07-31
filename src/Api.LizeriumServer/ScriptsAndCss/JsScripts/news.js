class NewsAdmin {
    startNews() {
        const forms = document.querySelectorAll(".news-editor-card form");

        forms.forEach((form) => {
            form.addEventListener("submit", (event) => {
                event.preventDefault();
                this.submitForm(form, event.submitter);
            });
        });

        this.bindMarkdownPasteUploads();
    }

    bindMarkdownPasteUploads() {
        document.querySelectorAll("textarea[name='MarkdownRu'], textarea[name='MarkdownEn']").forEach((textarea) => {
            textarea.addEventListener("paste", async (event) => {
                const files = Array.from(event.clipboardData?.files || []);
                const itemFiles = Array.from(event.clipboardData?.items || [])
                    .filter((item) => item.type && item.type.indexOf("image/") === 0)
                    .map((item) => item.getAsFile())
                    .filter((file) => !!file);
                const image = files.concat(itemFiles).find((file) => file.type && file.type.indexOf("image/") === 0);
                if (!image)
                    return;

                event.preventDefault();
                await this.uploadMarkdownImage(textarea, image);
            });
        });
    }

    async uploadMarkdownImage(textarea, image) {
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

        const response = await fetch("/news/upload-image", {
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

        const result = await response.json();
        const markdown = result && result.imageUrl
            ? `\n\n![image](${result.imageUrl})\n\n`
            : "\n\n[image upload failed]\n\n";

        textarea.value = textarea.value.replace(marker, markdown);

        if (result && result.imageUrl)
            this.updateInlineImagePreview(textarea, result.previewImageUrl || result.imageUrl);
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
            catch {
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

    updateGalleryPreview(form, imageUrl) {
        if (!form || !imageUrl)
            return;

        let preview = form.querySelector(".news-gallery-preview");
        if (!preview) {
            const galleryUpload = form.querySelector("input[name='galleryFiles']")?.closest(".news-image-upload");
            preview = document.createElement("div");
            preview.className = "news-gallery-preview";
            preview.innerHTML = "<span>Галерея</span><div></div>";
            if (galleryUpload && galleryUpload.parentNode)
                galleryUpload.parentNode.insertBefore(preview, galleryUpload.nextSibling);
            else
                form.appendChild(preview);
        }

        const list = preview.querySelector("div");
        if (!list)
            return;

        const image = document.createElement("img");
        image.src = imageUrl;
        image.alt = "";
        image.loading = "lazy";
        list.appendChild(image);
    }

    updateInlineImagePreview(textarea, imageUrl) {
        const form = textarea ? textarea.closest("form") : null;
        if (!form || !imageUrl)
            return;

        let preview = textarea.parentNode ? textarea.parentNode.querySelector(".news-inline-upload-preview") : null;
        if (!preview) {
            preview = document.createElement("div");
            preview.className = "news-inline-upload-preview";
            preview.innerHTML = "<span>Вставлено в Markdown</span><div></div>";
            textarea.parentNode.insertBefore(preview, textarea.nextSibling);
        }

        const list = preview.querySelector("div");
        if (!list)
            return;

        const image = document.createElement("img");
        image.src = imageUrl;
        image.alt = "";
        image.loading = "lazy";
        list.appendChild(image);
    }

    async submitForm(form, submitter) {
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

            const response = await fetch(action, {
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

            const result = await response.json();
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
            image.src = previewImageUrl;
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
            image.src = previewImageUrl;
        }

        preview.classList.remove("broken");
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
