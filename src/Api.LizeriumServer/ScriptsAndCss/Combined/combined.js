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
                document.location.href = "/Home/Error";
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
                document.location.href = "/error";
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
                document.location.href = "/Home/Error";
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
