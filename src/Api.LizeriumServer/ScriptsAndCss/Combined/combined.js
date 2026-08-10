var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class Ajax {
    constructor(path, cookies) {
        this.requestUrl = `/ajax/${path}`;
        this.cookies = cookies;
        this.xhttp = new XMLHttpRequest();
    }
    sendRequest() {
        return __awaiter(this, arguments, void 0, function* (dataObject = null) {
            return new Promise(resolve => {
                try {
                    const csrfToken = this.cookies.getCookie("CSRF-TOKEN");
                    this.xhttp.open("POST", this.requestUrl, true);
                    this.xhttp.onreadystatechange = () => {
                        if (this.xhttp.readyState !== 4)
                            return;
                        if (this.xhttp.status !== 200) {
                            resolve("");
                            return;
                        }
                        resolve(this.xhttp.responseText);
                    };
                    this.xhttp.onerror = () => {
                        console.error("ERROR: ", "Bad response");
                        resolve("");
                    };
                    this.xhttp.setRequestHeader("X-CSRF-TOKEN", csrfToken);
                    if (dataObject == null) {
                        this.xhttp.send();
                    }
                    else {
                        this.xhttp.setRequestHeader("Content-type", "application/json");
                        this.xhttp.send(JSON.stringify(dataObject));
                    }
                }
                catch (e) {
                    console.error("ERROR: ", e);
                    resolve("");
                }
            });
        });
    }
    uploadFile(inputFile_1) {
        return __awaiter(this, arguments, void 0, function* (inputFile, idChatbot = 0) {
            if (inputFile == null || inputFile.files.length < 1)
                return "";
            return new Promise(resolve => {
                try {
                    const csrfToken = this.cookies.getCookie("CSRF-TOKEN");
                    const formData = new FormData();
                    formData.append("file", inputFile.files[0]);
                    if (idChatbot > 0) {
                        formData.append("idChatbot", `${idChatbot}`);
                    }
                    this.xhttp.open("POST", this.requestUrl, true);
                    this.xhttp.onreadystatechange = () => {
                        if (this.xhttp.readyState !== 4)
                            return;
                        if (this.xhttp.status !== 200) {
                            resolve("");
                            return;
                        }
                        resolve(this.xhttp.responseText);
                    };
                    this.xhttp.onerror = () => {
                        console.error("ERROR: ", "Bad response");
                        resolve("");
                    };
                    this.xhttp.setRequestHeader("X-CSRF-TOKEN", csrfToken);
                    this.xhttp.send(formData);
                }
                catch (e) {
                    console.error("ERROR: ", e);
                    resolve("");
                }
            });
        });
    }
}

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class Auth {
    constructor(utilities, cookies) {
        this.utilities = utilities;
        this.cookies = cookies;
        this.inputSecretKey = document.getElementById("secret_key");
        this.btnSignIn = document.getElementById("sign_in");
        this.inputConfirmRecord = document.getElementById("confirm_record");
        this.btnSendConfirm = document.getElementById("send_confirm");
    }
    startAuth() {
        if (!this.inputSecretKey || !this.btnSignIn)
            return;
        this.btnSignIn.addEventListener("click", () => __awaiter(this, void 0, void 0, function* () { return yield this.sendSecretKeyAsync(); }));
    }
    startConfirm() {
        if (!this.inputConfirmRecord || !this.btnSendConfirm)
            return;
        this.btnSendConfirm.addEventListener("click", () => __awaiter(this, void 0, void 0, function* () { return yield this.sendConfirmCodeAsync(); }));
    }
    sendSecretKeyAsync() {
        return __awaiter(this, void 0, void 0, function* () {
            const loader = new Loader(this.btnSignIn);
            try {
                loader.setDisable();
                const requestData = {
                    "secretKey": this.inputSecretKey.value,
                    "recaptchaToken": ""
                };
                if (this.utilities.isEmpty(requestData.secretKey)) {
                    this.utilities.setInputWarning(this.inputSecretKey);
                    return;
                }
                const ajax = new Ajax("auth", this.cookies);
                const response = yield ajax.sendRequest(requestData);
                if (response === "ok") {
                    document.location.href = "/cabinet";
                    return;
                }
                document.location.reload();
            }
            finally {
                loader.setEnable();
            }
        });
    }
    sendConfirmCodeAsync() {
        return __awaiter(this, void 0, void 0, function* () {
            const loader = new Loader(this.btnSendConfirm);
            try {
                loader.setDisable();
                const requestData = {
                    "confirmRecord": this.inputConfirmRecord.value,
                    "recaptchaToken": ""
                };
                if (this.utilities.isEmpty(requestData.confirmRecord)) {
                    this.utilities.setInputWarning(this.inputConfirmRecord);
                    return;
                }
                const ajax = new Ajax("confirm", this.cookies);
                const response = yield ajax.sendRequest(requestData);
                if (response === "ok") {
                    document.location.href = "/cabinet";
                    return;
                }
                document.location.reload();
            }
            finally {
                loader.setEnable();
            }
        });
    }
}

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class Commands {
    constructor(utilities, cookies) {
        this.utilities = utilities;
        this.cookies = cookies;
        this.inputLastUserId = document.getElementById("last_user_id");
        this.statusSelect = document.querySelector("#status");
        this.categorySelect = document.querySelector("#categories");
        this.ajaxInProgress = false;
        this.statusSelect = document.querySelector("#status");
    }
    startCommands() {
        this.bindFilters();
        this.bindCreateCommandModal();
        const allSelects = document.querySelectorAll("#changeCommand");
        for (let i = 0; i < allSelects.length; i++) {
            allSelects[i].addEventListener('click', () => __awaiter(this, void 0, void 0, function* () { return yield this.updateChangeAsync(allSelects[i]); }));
        }
        var buttonDelete = document.querySelectorAll("#deleteCommand");
        for (let i = 0; i < buttonDelete.length; i++) {
            buttonDelete[i].addEventListener('click', () => __awaiter(this, void 0, void 0, function* () { return yield this.deleteAsync(buttonDelete[i]); }));
        }
    }
    bindFilters() {
        const filterForm = document.querySelector(".admin-toolbar");
        if (!filterForm || !this.statusSelect || !this.categorySelect)
            return;
        this.statusSelect.addEventListener("change", () => filterForm.submit());
        this.categorySelect.addEventListener("change", () => filterForm.submit());
    }
    bindCreateCommandModal() {
        const openButton = document.getElementById("openCreateCommandModal");
        const template = document.getElementById("createCommandTemplate");
        if (!openButton || !template)
            return;
        openButton.addEventListener("click", () => {
            const modal = new ModalForm("Новая команда", "column", "command-create-modal");
            modal.showModalWithHtml(template.innerHTML);
            const buttonCreate = document.querySelector("#createCommand");
            if (!buttonCreate)
                return;
            buttonCreate.addEventListener("click", () => __awaiter(this, void 0, void 0, function* () { return yield this.createCommandAsync(); }));
        });
    }
    createCommandAsync() {
        return __awaiter(this, void 0, void 0, function* () {
            const newCategory = document.querySelector("#newCategory");
            const newName = document.querySelector("#newName");
            const newExampleInput = document.querySelector("#newExampleInput");
            const newDescription = document.querySelector("#newDescription");
            const newGif = document.querySelector("#newGif");
            const newLikes = document.querySelector("#newLikes");
            const newStatus = document.querySelector("#newStatus");
            if (!newCategory || !newName || !newExampleInput || !newDescription || !newGif || !newLikes || !newStatus)
                return;
            const dataRequest = {
                "newCategory": newCategory.value,
                "newName": newName.value,
                "newDescription": newDescription.value,
                "newGif": newGif.value,
                "newExampleInput": newExampleInput.value,
                "newLikes": parseInt(newLikes.value || "0"),
                "newStatus": parseInt(newStatus.value),
            };
            const ajax = new Ajax("saveCommand", this.cookies);
            const response = yield ajax.sendRequest(dataRequest);
            if (response !== "ok") {
                document.location.reload();
                return;
            }
            document.location.href = "/Commands";
        });
    }
    updateChangeAsync(button) {
        return __awaiter(this, void 0, void 0, function* () {
            const id = parseInt(button.getAttribute("IdC"));
            var Category = document.getElementById(id + "_Category");
            var CommandNames = document.getElementById(id + "_CommandNames");
            var ExampleInput = document.getElementById(id + "_ExampleInput");
            var Description = document.getElementById(id + "_Description");
            var UrlGif = document.getElementById(id + "_UrlGif");
            var CountLike = document.getElementById(id + "_CountLike");
            var Status = document.getElementById(id + "_status");
            console.log(Category.value);
            console.log(CommandNames.value);
            console.log(ExampleInput.value);
            console.log(Description.value);
            console.log(UrlGif.textContent);
            console.log(parseInt(CountLike.value));
            console.log(parseInt(Status.value));
            const dataRequest = {
                "Id": id,
                "newCategory": Category.value,
                "newName": CommandNames.value,
                "newDescription": Description.value,
                "newGif": UrlGif.textContent,
                "newExampleInput": ExampleInput.value,
                "newLikes": parseInt(CountLike.value),
                "newStatus": parseInt(Status.value),
            };
            const ajax = new Ajax(`updateCommand`, this.cookies);
            const response = yield ajax.sendRequest(dataRequest);
            console.log(response);
        });
    }
    deleteAsync(button) {
        return __awaiter(this, void 0, void 0, function* () {
            const id = parseInt(button.getAttribute("IdC"));
            var Category = document.getElementById(id + "_Category");
            var CommandNames = document.getElementById(id + "_CommandNames");
            var ExampleInput = document.getElementById(id + "_ExampleInput");
            var Description = document.getElementById(id + "_Description");
            var UrlGif = document.getElementById(id + "_UrlGif");
            var CountLike = document.getElementById(id + "_CountLike");
            var Status = document.getElementById(id + "_status");
            console.log(Category.value);
            console.log(CommandNames.value);
            console.log(ExampleInput.value);
            console.log(Description.value);
            console.log(UrlGif.textContent);
            console.log(parseInt(CountLike.value));
            console.log(parseInt(Status.value));
            const dataRequest = {
                "Id": id,
                "newCategory": Category.value,
                "newName": CommandNames.value,
                "newDescription": Description.value,
                "newGif": UrlGif.textContent,
                "newExampleInput": ExampleInput.value,
                "newLikes": parseInt(CountLike.value),
                "newStatus": parseInt(Status.value),
            };
            const ajax = new Ajax(`deleteCommand`, this.cookies);
            const response = yield ajax.sendRequest(dataRequest);
            console.log(response);
        });
    }
}

class Cookies {
    getCookie(cookieName) {
        const name = cookieName + "=";
        const decodedCookie = decodeURIComponent(document.cookie);
        const ca = decodedCookie.split(";");
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) === " ") {
                c = c.substring(1);
            }
            if (c.indexOf(name) === 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    }
    setCookie(name, value) {
        const date = new Date();
        date.setTime(date.getTime() + 30 * 24 * 60 * 60 * 1000);
        document.cookie = name + "=" + encodeURIComponent(value) + ";expires=" + date.toUTCString() + ";path=/;secure";
    }
    removeCookie(name) {
        document.cookie = name + "=;Max-Age=-99999999;";
    }
}

class Loader {
    constructor(button, type = "spinner") {
        this.button = button;
        this.type = type;
        this.prevHtml = "";
        if (type === "spinner")
            return;
        this.loader = new Image();
        this.loader.src = "/img/loader.gif";
        this.loader.classList.add("loader");
        this.loader.alt = "loader";
    }
    setDisable() {
        this.button.setAttribute("disabled", "disabled");
        this.prevHtml = this.button.innerHTML;
        switch (this.type) {
            case "spinner":
                this.button.innerHTML = "";
                const spinner = document.createElement("div");
                spinner.classList.add("spinner");
                spinner.innerHTML = `<div></div><div></div><div></div>`;
                this.button.appendChild(spinner);
                break;
            case "loader":
                this.button.innerHTML = this.loader.outerHTML;
                break;
        }
    }
    setEnable() {
        this.button.removeAttribute("disabled");
        this.button.innerHTML = this.prevHtml;
    }
}

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(() => {
    window.addEventListener("DOMContentLoaded", () => __awaiter(this, void 0, void 0, function* () {
        const utilities = new Utilities();
        const cookies = new Cookies();
        const currentUrl = new URL(document.location.href);
        const pathname = currentUrl.pathname.toLowerCase();
        const partsPath = pathname.split("/");
        switch (partsPath[1]) {
            case "":
                {
                    const auth = new Auth(utilities, cookies);
                    auth.startAuth();
                }
                break;
            case "confirmation":
                {
                    const auth = new Auth(utilities, cookies);
                    auth.startConfirm();
                }
                break;
            case "posts":
                {
                    const posts = new Posts(utilities, cookies);
                    yield posts.startPosts();
                }
                break;
            case "commands":
                {
                    const commands = new Commands(utilities, cookies);
                    yield commands.startCommands();
                }
                break;
            case "news":
                {
                    const news = new NewsAdmin();
                    news.startNews();
                }
                break;
            case "products":
                {
                    const products = new ProductsAdmin();
                    products.startProducts();
                }
                break;
            default:
                break;
        }
        const buttonScroll = document.getElementById("scrollButton");
        if (buttonScroll) {
            const updateScrollButton = () => {
                const scrollPosition = window.scrollY;
                const pageHeight = document.documentElement.scrollHeight;
                const windowHeight = window.innerHeight;
                buttonScroll.classList.toggle("is-up", scrollPosition > (pageHeight - windowHeight) / 2);
            };
            updateScrollButton();
            window.addEventListener("scroll", updateScrollButton);
            buttonScroll.addEventListener("click", () => {
                const scrollPosition = window.scrollY;
                const pageHeight = document.documentElement.scrollHeight;
                const windowHeight = window.innerHeight;
                if (scrollPosition > (pageHeight - windowHeight) / 2) {
                    window.scrollTo({ top: 0, behavior: "smooth" });
                }
                else {
                    window.scrollTo({ top: pageHeight, behavior: "smooth" });
                }
            });
        }
        const sidebarToggle = document.getElementById("sidebarToggle");
        if (sidebarToggle) {
            const isCollapsed = localStorage.getItem("api-sidebar-collapsed") === "true";
            document.body.classList.toggle("sidebar-collapsed", isCollapsed);
            sidebarToggle.addEventListener("click", () => {
                const nextState = !document.body.classList.contains("sidebar-collapsed");
                document.body.classList.toggle("sidebar-collapsed", nextState);
                localStorage.setItem("api-sidebar-collapsed", nextState.toString());
            });
        }
        const fileInputs = document.querySelectorAll("input[type='file']");
        for (let i = 0; i < fileInputs.length; i++) {
            const input = fileInputs[i];
            input.addEventListener("change", () => {
                input.setAttribute("data-file-name", input.files && input.files.length > 0
                    ? input.files[0].name
                    : "Файл не выбран");
            });
        }
        const logout = document.getElementById("logout");
        if (!logout)
            return;
        logout.addEventListener("click", () => __awaiter(this, void 0, void 0, function* () {
            const confirmForm = new ModalForm("Выход из админки", "column");
            const resultConfirm = yield confirmForm.showConfirmForm("Вы уверены что хотите выйти?", "Да", "Нет");
            if (!resultConfirm)
                return;
            document.location.href = "/logout";
        }));
    }));
})();

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class ModalForm {
    constructor(headerText, typeFlex, dopClass = null) {
        this.modalForm = document.getElementById("modal_form");
        this.modalClose = this.modalForm.querySelector(".close_model");
        this.modalBody = this.modalForm.querySelector(".modal_body");
        this.modalBody.classList.remove("row", "column", "start");
        this.modalBody.classList.add(typeFlex);
        if (dopClass != null) {
            this.modalBody.classList.add(dopClass);
        }
        const headerModel = this.modalForm.querySelector(".modal_header");
        if (headerModel) {
            headerModel.querySelector("h3").innerHTML = headerText;
        }
        this.modalClose.addEventListener("click", () => {
            this.hideModal();
        });
        window.addEventListener("click", (event) => {
            if (event.target !== this.modalForm)
                return;
            this.hideModal();
        });
        this.resolve = null;
    }
    showModalWithHtml(innerHtml) {
        if (innerHtml) {
            this.modalBody.innerHTML = innerHtml;
        }
        this.modalForm.style.display = "flex";
    }
    showModalWithElement(element) {
        this.modalBody.appendChild(element);
        this.modalForm.style.display = "flex";
    }
    changeContent(innerHtml) {
        if (innerHtml) {
            this.modalBody.classList.remove("start");
            this.modalBody.innerHTML = innerHtml;
        }
    }
    hideModal() {
        this.modalForm.style.display = "none";
        this.modalBody.innerHTML = "";
        if (this.resolve == null)
            return;
        this.resolve(false);
    }
    showConfirmForm(textConfirm, yesText, noText) {
        return __awaiter(this, void 0, void 0, function* () {
            return new Promise(resolve => {
                this.resolve = resolve;
                const title = document.createElement("h3");
                title.classList.add("title_confirm");
                title.innerText = textConfirm;
                const blockBtn = document.createElement("div");
                blockBtn.classList.add("confirm_btn_container");
                const btnYes = document.createElement("button");
                btnYes.classList.add("btn", "yes");
                btnYes.innerText = yesText;
                btnYes.addEventListener("click", () => {
                    resolve(true);
                    this.hideModal();
                });
                const btnNo = document.createElement("button");
                btnNo.classList.add("btn", "no");
                btnNo.innerText = noText;
                btnNo.addEventListener("click", () => {
                    resolve(false);
                    this.hideModal();
                });
                blockBtn.appendChild(btnYes);
                blockBtn.appendChild(btnNo);
                this.modalBody.appendChild(title);
                this.modalBody.appendChild(blockBtn);
                this.modalForm.style.display = "flex";
            });
        });
    }
}

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
        this.repairNewsImageSources(document);
        forms.forEach((form) => {
            form.addEventListener("submit", (event) => {
                event.preventDefault();
                this.submitForm(form, event.submitter);
            });
        });
        this.bindMarkdownPasteUploads();
        this.enhanceNewsAssetFields(forms);
        forms.forEach((form) => {
            this.hydrateImagePreviews(form);
            this.bindExistingImagePreviews(form);
        });
        this.ensureNewsAssetModal();
        this.ensureImageLightbox();
    }
    repairNewsImageSources(root) {
        const scope = root || document;
        scope.querySelectorAll("img").forEach((image) => {
            const rawSource = image.dataset.rawNewsUrl
                || image.getAttribute("src")
                || image.dataset.newsPreviewLightbox
                || image.dataset.fallbackSrc
                || "";
            const publicSource = this.toPublicNewsImageUrl(rawSource);
            if (!publicSource)
                return;
            image.dataset.rawNewsUrl = rawSource;
            image.dataset.fallbackSrc = publicSource;
            image.src = publicSource;
            if (image.dataset.newsPreviewLightbox)
                image.dataset.newsPreviewLightbox = publicSource;
        });
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
            this.bindNewsImageFallback(image, image.dataset.rawNewsUrl || image.getAttribute("src") || "");
            if (image.dataset.newsLightboxBound === "true")
                return;
            image.dataset.newsLightboxBound = "true";
            image.addEventListener("click", () => this.openImageLightbox(image.currentSrc || image.src, image.alt || ""));
        });
    }
    hydrateImagePreviews(form) {
        if (!form)
            return;
        const iconInput = form.querySelector("input[name='IconUrl']");
        const imageInput = form.querySelector("input[name='ImageUrl']");
        if (iconInput && iconInput.value.trim().length > 0)
            this.updateIconPreview(form, iconInput.value.trim(), iconInput.value.trim());
        if (imageInput && imageInput.value.trim().length > 0)
            this.updateImagePreview(form, imageInput.value.trim(), imageInput.value.trim());
        this.refreshGalleryPreview(form);
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
            this.updateImagePreview(form, result.previewImageUrl, imageInput ? imageInput.value : result.imageUrl);
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
    updateIconPreview(form, previewImageUrl, rawImageUrl) {
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
            image.dataset.rawNewsUrl = rawImageUrl || previewImageUrl || "";
            this.bindNewsImageFallback(image, rawImageUrl || previewImageUrl);
            image.src = this.toNewsDisplayUrl(rawImageUrl || previewImageUrl);
            if (image.dataset.newsLightboxBound !== "true") {
                image.dataset.newsLightboxBound = "true";
                image.addEventListener("click", () => this.openImageLightbox(image.currentSrc || image.src, image.alt || ""));
            }
        }
        preview.classList.remove("broken");
    }
    updateImagePreview(form, previewImageUrl, rawImageUrl) {
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
            image.dataset.rawNewsUrl = rawImageUrl || previewImageUrl || "";
            this.bindNewsImageFallback(image, rawImageUrl || previewImageUrl);
            image.src = this.toNewsDisplayUrl(rawImageUrl || previewImageUrl);
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
        image.src = this.toNewsDisplayUrl(imageUrl);
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
                <img src="${this.escapeAttribute(this.toNewsDisplayUrl(url))}" data-fallback-src="${this.escapeAttribute(this.toPublicNewsImageUrl(url))}" alt="" loading="lazy" />
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
            const previewUrl = this.toNewsDisplayUrl(asset.previewUrl || asset.url)
                || this.toNewsPreviewUrl(asset.url)
                || asset.url
                || "";
            const fallbackUrl = this.toPublicNewsImageUrl(asset.url || asset.previewUrl || "");
            const card = document.createElement("div");
            card.className = "news-asset-card";
            card.innerHTML = `
                <span class="news-asset-thumb"><img src="${this.escapeAttribute(previewUrl)}" data-raw-news-url="${this.escapeAttribute(asset.url || "")}" data-fallback-src="${this.escapeAttribute(fallbackUrl)}" alt="" loading="lazy" /></span>
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
            const image = card.querySelector(".news-asset-thumb img");
            this.bindNewsImageFallback(image, asset.url || asset.previewUrl || "");
            image.addEventListener("click", () => this.openImageLightbox(image.currentSrc || image.src, asset.name || ""));
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
    toNewsDisplayUrl(url) {
        if (!url)
            return "";
        const publicUrl = this.toPublicNewsImageUrl(url);
        return publicUrl || url;
    }
    toPublicNewsImageUrl(url) {
        if (!url)
            return "";
        const raw = String(url).trim();
        if (!raw)
            return "";
        if (raw.toLowerCase().indexOf("/img/news/") === 0)
            return `https://lizup.ru${raw}`;
        try {
            const parsed = new URL(raw, window.location.origin);
            if (parsed.pathname.toLowerCase().indexOf("/img/news/") === 0)
                return `https://lizup.ru${parsed.pathname}${parsed.search || ""}`;
        }
        catch (_a) {
            return "";
        }
        return "";
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

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class Posts {
    constructor(utilities, cookies) {
        this.utilities = utilities;
        this.cookies = cookies;
        this.inputLastUserId = document.getElementById("last_user_id");
        this.statusSelect = document.querySelector("#postsStatus");
        this.ajaxInProgress = false;
        this.statusSelect = document.querySelector("#postsStatus");
    }
    startPosts() {
        const allSelects = document.querySelectorAll(".post-status-select");
        for (let i = 0; i < allSelects.length; i++) {
            allSelects[i].addEventListener('change', () => __awaiter(this, void 0, void 0, function* () { return yield this.updateSelectStatusAsync(allSelects[i]); }));
        }
        window.addEventListener("scroll", () => __awaiter(this, void 0, void 0, function* () { return yield this.handleScrollWindow(); }));
        this.statusSelect.addEventListener("change", () => __awaiter(this, void 0, void 0, function* () {
            yield this.loadTablePosts();
        }));
    }
    updateSelectStatusAsync(button) {
        return __awaiter(this, void 0, void 0, function* () {
            const idUser = parseInt(button.getAttribute("user"));
            const selectedStatusId = parseInt(button.value);
            console.log('Измененный статус:', idUser);
            const ajax = new Ajax(`updateStatusPost/${idUser}/${selectedStatusId}`, this.cookies);
            const response = yield ajax.sendRequest();
            console.log(response);
        });
    }
    handleScrollWindow() {
        return __awaiter(this, void 0, void 0, function* () {
            this.inputLastUserId = document.getElementById("last_user_id");
            const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
            const scrollHeight = Math.max(document.body.scrollHeight, document.documentElement.scrollHeight, document.body.offsetHeight, document.documentElement.offsetHeight, document.body.clientHeight, document.documentElement.clientHeight);
            if (scrollHeight - scrollTop > 1500 || this.ajaxInProgress)
                return;
            this.ajaxInProgress = true;
            const statusValue = parseInt(this.statusSelect.value);
            const ajax = new Ajax(`getPosts/${this.inputLastUserId.value}/${statusValue}/1`, this.cookies);
            const response = yield ajax.sendRequest();
            if (this.utilities.isEmpty(response))
                return;
            const dataResponse = JSON.parse(response);
            this.inputLastUserId.value = dataResponse.lastUserId.toString();
            this.appendUsers(dataResponse.posts);
        });
    }
    loadTablePosts() {
        return __awaiter(this, void 0, void 0, function* () {
            this.inputLastUserId = document.getElementById("last_user_id");
            const ajax = new Ajax(`getPosts/0/${this.statusSelect.value}/0`, this.cookies);
            const response = yield ajax.sendRequest();
            console.log(response);
            if (this.utilities.isEmpty(response))
                return;
            const dataResponse = JSON.parse(response);
            const blockPosts = document.querySelector("tbody");
            if (blockPosts) {
                blockPosts.innerHTML = '';
                while (blockPosts.firstChild) {
                    blockPosts.removeChild(blockPosts.firstChild);
                }
            }
            this.inputLastUserId.value = dataResponse.lastUserId.toString();
            this.appendUsers(dataResponse.posts);
        });
    }
    appendUsers(posts) {
        if (posts.length < 1) {
            this.ajaxInProgress = true;
            return;
        }
        try {
            const fragment = document.createDocumentFragment();
            for (let i = 0; i < posts.length; i++) {
                const tr = document.createElement("tr");
                const tdIdUser = document.createElement("td");
                tdIdUser.innerText = posts[i].Id;
                const tdUserData = document.createElement("td");
                const div1 = document.createElement("div");
                div1.classList.add("user_in_table");
                const divImg = document.createElement("div");
                const img = document.createElement("img");
                img.src = "img/my-account-icon.png";
                img.alt = posts[i].Autor;
                const divSpan = document.createElement("div");
                const span = document.createElement("span");
                span.innerText = posts[i].Autor;
                divSpan.appendChild(span);
                divImg.appendChild(img);
                div1.appendChild(divImg);
                div1.appendChild(divSpan);
                tdUserData.appendChild(div1);
                const tdMsgUser = document.createElement("td");
                tdMsgUser.innerHTML = posts[i].Message;
                const tdRegAuth = document.createElement("td");
                tdRegAuth.innerHTML = `<div class="times"><span>Регистрация</span><span>${posts[i].DateTimeUnixString}</span></div>`;
                const selectTd = document.createElement("td");
                const select = document.createElement("select");
                select.setAttribute("user", posts[i].Id);
                select.classList.add("post-status-select");
                const options = [
                    { value: "-1", text: "Обработка" },
                    { value: "1", text: "Новое" },
                    { value: "2", text: "Прочитано" },
                    { value: "3", text: "В работе" },
                    { value: "4", text: "Отказано" },
                    { value: "5", text: "Выполнено" }
                ];
                for (const option of options) {
                    const optionElement = document.createElement("option");
                    optionElement.value = option.value;
                    optionElement.text = option.text;
                    select.appendChild(optionElement);
                }
                select.value = posts[i].Status.toString();
                selectTd.appendChild(select);
                select.addEventListener('change', () => __awaiter(this, void 0, void 0, function* () { return yield this.updateSelectStatusAsync(select); }));
                tr.appendChild(tdIdUser);
                tr.appendChild(tdUserData);
                tr.appendChild(tdRegAuth);
                tr.appendChild(tdMsgUser);
                tr.appendChild(selectTd);
                fragment.appendChild(tr);
            }
            document.querySelector("table > tbody").appendChild(fragment);
        }
        finally {
            this.ajaxInProgress = false;
        }
    }
}

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
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
    enhanceSaving(page) {
        page.querySelectorAll(".products-admin-form").forEach((form) => {
            if (form.dataset.productsAjaxSave === "true")
                return;
            form.dataset.productsAjaxSave = "true";
            form.addEventListener("submit", (event) => this.submitForm(event, form));
        });
    }
    submitForm(event, form) {
        return __awaiter(this, void 0, void 0, function* () {
            event.preventDefault();
            if (typeof form.reportValidity === "function" && !form.reportValidity())
                return;
            const submitButton = form.querySelector("button[type='submit']");
            const originalText = submitButton ? submitButton.textContent : "";
            const isNewRecord = !this.getFormId(form);
            this.setSubmitState(submitButton, true, "Saving...");
            try {
                const response = yield fetch(form.action, {
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
                const result = yield response.json();
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
            }
            catch (error) {
                console.error(error);
                form.classList.add("is-save-error");
                this.setSubmitState(submitButton, false, "Error");
                window.setTimeout(() => {
                    form.classList.remove("is-save-error");
                    this.setSubmitState(submitButton, false, originalText);
                }, 1600);
            }
        });
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
    openAssetModal(input) {
        return __awaiter(this, void 0, void 0, function* () {
            this.ensureAssetModal();
            this.activeInput = input;
            this.modal.hidden = false;
            document.body.classList.add("products-asset-modal-open");
            this.modalSearch.value = "";
            this.modalGrid.innerHTML = "<div class=\"products-asset-empty\">Загрузка...</div>";
            if (!this.assets)
                yield this.loadAssets();
            this.renderAssets();
            this.modalSearch.focus();
        });
    }
    closeAssetModal() {
        if (!this.modal)
            return;
        this.modal.hidden = true;
        document.body.classList.remove("products-asset-modal-open");
    }
    loadAssets() {
        return __awaiter(this, void 0, void 0, function* () {
            const response = yield fetch("/products/assets", {
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
            const result = yield response.json();
            this.assets = result && Array.isArray(result.assets) ? result.assets : [];
        });
    }
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
    uploadImage(input, uploadInput) {
        return __awaiter(this, void 0, void 0, function* () {
            const file = uploadInput.files && uploadInput.files[0];
            if (!file)
                return;
            const form = input.closest("form") || document.querySelector("form");
            const token = form ? form.querySelector("input[name='__RequestVerificationToken']") : null;
            const formData = new FormData();
            formData.append("imageFile", file, file.name);
            if (token)
                formData.append("__RequestVerificationToken", token.value);
            const response = yield fetch("/products/assets/upload", {
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
            const result = yield response.json();
            if (result && result.url) {
                input.value = result.url;
                input.dispatchEvent(new Event("input", { bubbles: true }));
                this.assets = null;
            }
        });
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

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class Utilities {
    isEmpty(value) {
        if (value == null)
            return true;
        if (value === "")
            return true;
        return false;
    }
    setInputWarning(input) {
        input.classList.add("warning_input");
        const timeout = setTimeout(() => {
            input.classList.remove("warning_input");
            clearTimeout(timeout);
        }, 3000);
    }
    isValidEmail(email) {
        if (this.isEmpty(email))
            return false;
        const pattern = /^([a-z0-9_.-])+@[a-z0-9-]+\.([a-z]{2,4}\.)?[a-z]{2,4}$/i;
        return pattern.test(email);
    }
    doDelay(mlsec) {
        return __awaiter(this, void 0, void 0, function* () {
            return new Promise(resolve => {
                setTimeout(() => {
                    resolve(null);
                }, mlsec);
            });
        });
    }
    get getWidh() {
        return window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
    }
    get getHeight() {
        return window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
    }
    getRandomInt(min, max) {
        min = Math.ceil(min);
        max = Math.floor(max);
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }
    copyValueToBuffer(value, element) {
        try {
            const coord = element.getBoundingClientRect();
            const scrolled = window.pageYOffset || document.documentElement.scrollTop;
            const iOsDevice = navigator.userAgent.match(/ipad|iphone/i);
            const textArea = document.createElement("textarea");
            textArea.readOnly = true;
            textArea.style.top = `${Math.round(scrolled + coord.top)}px`;
            textArea.classList.add("text_copy");
            textArea.value = value;
            document.body.appendChild(textArea);
            textArea.focus();
            if (iOsDevice) {
                const editable = textArea.contentEditable;
                const readOnly = textArea.readOnly;
                textArea.contentEditable = "true";
                textArea.readOnly = false;
                const range = document.createRange();
                range.selectNodeContents(textArea);
                const selection = window.getSelection();
                selection.removeAllRanges();
                selection.addRange(range);
                textArea.setSelectionRange(0, 999999);
                textArea.contentEditable = editable;
                textArea.readOnly = readOnly;
            }
            else {
                textArea.select();
            }
            const resultCopy = document.execCommand("copy");
            document.body.removeChild(textArea);
            return resultCopy;
        }
        catch (e) {
            console.error("Error: ", e);
            return false;
        }
    }
    getCurrentDate() {
        const date = new Date();
        const nowDay = date.getDate();
        const nowMonth = date.getMonth() + 1;
        let forFullDay = "";
        if (nowDay < 10) {
            forFullDay = "0";
        }
        let forFullMonth = "";
        if (nowMonth < 10) {
            forFullMonth = "0";
        }
        return `${forFullDay}${nowDay}.${forFullMonth}${nowMonth}.${date.getFullYear()}`;
    }
    generateUuid() {
        var d = new Date().getTime();
        var d2 = ((typeof performance !== "undefined") && performance.now && (performance.now() * 1000)) || 0;
        return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, c => {
            var r = Math.random() * 16;
            if (d > 0) {
                r = (d + r) % 16 | 0;
                d = Math.floor(d / 16);
            }
            else {
                r = (d2 + r) % 16 | 0;
                d2 = Math.floor(d2 / 16);
            }
            return (c === "x" ? r : (r & 0x3 | 0x8)).toString(16);
        });
    }
}
