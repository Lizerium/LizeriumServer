var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { Ajax } from "../Shared/ajax";
import { ModalForm } from "../Shared/modal_form";
export class Wish {
    constructor(utilities, cookies) {
        this.utilities = utilities;
        this.cookies = cookies;
        this.statusSelect = document.querySelector("#status");
    }
    startApp() {
        return __awaiter(this, void 0, void 0, function* () {
            this.truncateLength = 300;
            const ajax = new Ajax(`getAllLocalizedStrings/Views.Home.Wish`, this.cookies);
            const response = yield ajax.sendRequest();
            this.localizationStrings = JSON.parse(response);
            yield this.loadTablePosts();
            const createPost = document.getElementById("create-post-btn");
            if (createPost) {
                createPost.addEventListener("click", () => __awaiter(this, void 0, void 0, function* () {
                    const confirmForm = new ModalForm(this.localizationStrings["Wish_Modal_Title"], "column");
                    const resultConfirm = yield confirmForm.showConfirmForm("", "", this.localizationStrings["Wish_Modal_Cancel"]);
                    if (!resultConfirm)
                        return;
                    document.location.href = "/create";
                }));
            }
            window.addEventListener("scroll", () => __awaiter(this, void 0, void 0, function* () { return yield this.handleScrollWindow(); }));
            this.statusSelect.addEventListener("change", () => __awaiter(this, void 0, void 0, function* () {
                yield this.loadTablePosts();
            }));
        });
    }
    loadTablePosts() {
        return __awaiter(this, void 0, void 0, function* () {
            this.inputLastUserId = document.getElementById("last_user_id");
            const ajax = new Ajax(`getPosts/0/${this.statusSelect.value}/0`, this.cookies);
            const response = yield ajax.sendRequest();
            if (this.utilities.isEmpty(response))
                return;
            const dataResponse = JSON.parse(response);
            const blockPosts = document.querySelector(".block.posts");
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
    startReadMoreLogic() {
        return __awaiter(this, void 0, void 0, function* () {
            yield this.utilities.startLoader();
            const posts = document.querySelectorAll(".post-content");
            for (let i = 0; i < posts.length; i++) {
                const fullContent = posts[i].querySelector('.full-content');
                const readMoreButton = posts[i].querySelector('.read-more');
                if (fullContent.textContent.length < this.truncateLength) {
                    readMoreButton.style.visibility = "hidden";
                }
                else
                    posts[i].addEventListener('click', () => this.toggleContent(posts[i]));
            }
            yield this.utilities.stopLoader();
        });
    }
    truncateText(text, length) {
        if (text.length > length) {
            return text.substring(0, length) + '...';
        }
        else {
            return text;
        }
    }
    toggleContent(postelement) {
        const truncatedText = postelement.querySelector('.truncated');
        const fullContent = postelement.querySelector('.full-content');
        const readMoreButton = postelement.querySelector('.read-more');
        const fullText = fullContent.textContent;
        truncatedText.textContent = this.truncateText(fullText, this.truncateLength);
        fullContent.style.display = fullContent.style.display === 'none' ? 'inline' : 'none';
        truncatedText.style.display = fullContent.style.display === 'none' ? 'inline' : 'none';
        readMoreButton.textContent = readMoreButton.textContent === this.localizationStrings["Wish_Read_More"] ? this.localizationStrings["Wish_Read_Default"] : this.localizationStrings["Wish_Read_More"];
        postelement.style.height = fullContent.style.display === 'none' ? `${postelement.offsetHeight}px` : 'auto';
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
    appendUsers(posts) {
        if (posts.length < 1) {
            this.ajaxInProgress = true;
            return;
        }
        try {
            const fragment = document.createDocumentFragment();
            for (let i = 0; i < posts.length; i++) {
                if (parseInt(posts[i].Status) < 0)
                    continue;
                const div = document.createElement("div");
                switch (posts[i].Status) {
                    case 1:
                        div.classList.add("status-new");
                        break;
                    case 2:
                        div.classList.add("status-read");
                        break;
                    case 3:
                        div.classList.add("status-job");
                        break;
                    case 4:
                        div.classList.add("status-delete");
                        break;
                    case 5:
                        div.classList.add("status-complete");
                        break;
                }
                const AutorLabel = document.createElement("p");
                AutorLabel.innerText = posts[i].Autor;
                const PostContentDiv = document.createElement("div");
                PostContentDiv.classList.add("post-content");
                const spanTruncated = document.createElement("span");
                spanTruncated.classList.add("truncated");
                spanTruncated.innerText = posts[i].MessageMini;
                const spanFullContent = document.createElement("span");
                spanFullContent.classList.add("full-content");
                spanFullContent.style.display = "none";
                const labelSpanFullContent = document.createElement("p");
                labelSpanFullContent.innerText = posts[i].Message;
                const AReadMore = document.createElement("a");
                AReadMore.classList.add("read-more");
                AReadMore.innerText = this.localizationStrings["Wish_Read_More"];
                PostContentDiv.appendChild(spanTruncated);
                PostContentDiv.appendChild(spanFullContent);
                spanFullContent.appendChild(labelSpanFullContent);
                if (posts[i].Message.length < this.truncateLength) {
                    AReadMore.style.visibility = "hidden";
                }
                else {
                    AReadMore.addEventListener('click', () => this.toggleContent(PostContentDiv));
                    PostContentDiv.appendChild(AReadMore);
                }
                div.appendChild(AutorLabel);
                div.appendChild(PostContentDiv);
                switch (posts[i].Status) {
                    case 1:
                        const labelStatusNew = document.createElement("p");
                        labelStatusNew.classList.add("status-new");
                        const divStatusNew = document.createElement("div");
                        divStatusNew.classList.add("loader");
                        divStatusNew.classList.add("status-new");
                        const statusNewSpans = this.createSpansFromText(this.localizationStrings["Wish_Status_1"]);
                        statusNewSpans.forEach(span => divStatusNew.appendChild(span));
                        labelStatusNew.appendChild(divStatusNew);
                        div.appendChild(labelStatusNew);
                        break;
                    case 2:
                        const labelStatusRead = document.createElement("p");
                        labelStatusRead.classList.add("status-read");
                        const divStatusRead = document.createElement("div");
                        divStatusRead.classList.add("loader");
                        divStatusRead.classList.add("status-read");
                        const statusReadSpans = this.createSpansFromText(this.localizationStrings["Wish_Status_2"]);
                        statusReadSpans.forEach(span => divStatusRead.appendChild(span));
                        labelStatusRead.appendChild(divStatusRead);
                        div.appendChild(labelStatusRead);
                        break;
                    case 3:
                        const labelStatusJob = document.createElement("p");
                        labelStatusJob.classList.add("status-job");
                        const divStatusJob = document.createElement("div");
                        divStatusJob.classList.add("loader");
                        divStatusJob.classList.add("status-job");
                        const statusJobSpans = this.createSpansFromText(this.localizationStrings["Wish_Status_3"]);
                        statusJobSpans.forEach(span => divStatusJob.appendChild(span));
                        labelStatusJob.appendChild(divStatusJob);
                        div.appendChild(labelStatusJob);
                        break;
                    case 4:
                        const labelStatusDelete = document.createElement("p");
                        labelStatusDelete.classList.add("status-delete");
                        const divStatusDelete = document.createElement("div");
                        divStatusDelete.classList.add("loader");
                        divStatusDelete.classList.add("status-delete");
                        const statusDeleteSpans = this.createSpansFromText(this.localizationStrings["Wish_Status_4"]);
                        statusDeleteSpans.forEach(span => divStatusDelete.appendChild(span));
                        labelStatusDelete.appendChild(divStatusDelete);
                        div.appendChild(labelStatusDelete);
                        break;
                    case 5:
                        const labelStatusComplete = document.createElement("div");
                        labelStatusComplete.classList.add("status-complete");
                        const divStatusComplete = document.createElement("div");
                        divStatusComplete.classList.add("loader");
                        divStatusComplete.classList.add("status-complete");
                        const statusCompleteSpans = this.createSpansFromText(this.localizationStrings["Wish_Status_5"]);
                        statusCompleteSpans.forEach(span => divStatusComplete.appendChild(span));
                        labelStatusComplete.style.textAlign = "center";
                        labelStatusComplete.appendChild(divStatusComplete);
                        div.appendChild(labelStatusComplete);
                        break;
                }
                fragment.appendChild(div);
            }
            document.querySelector(".block.posts").appendChild(fragment);
        }
        finally {
            this.ajaxInProgress = false;
        }
    }
    createSpansFromText(text) {
        const spans = [];
        for (const char of text) {
            const span = document.createElement("span");
            span.innerText = char;
            spans.push(span);
        }
        return spans;
    }
}
//# sourceMappingURL=wish.js.map