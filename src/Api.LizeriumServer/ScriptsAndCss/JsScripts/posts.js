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
        this.statusSelect = document.querySelector("#status");
        this.ajaxInProgress = false;
        this.statusSelect = document.querySelector("#status");
    }
    startPosts() {
        const allSelects = document.querySelectorAll("#status");
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
                select.id = `status`;
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
