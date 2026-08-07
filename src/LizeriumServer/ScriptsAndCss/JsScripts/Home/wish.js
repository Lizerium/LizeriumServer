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
                createPost.addEventListener("click", () => this.openCreatePostModal());
            }
            document.querySelectorAll("[data-wish-modal-close]").forEach(closeControl => {
                closeControl.addEventListener("click", () => this.closeCreatePostModal());
            });
            document.addEventListener("keydown", event => {
                if (event.key === "Escape") {
                    this.closeCreatePostModal();
                }
            });
            window.addEventListener("scroll", () => __awaiter(this, void 0, void 0, function* () { return yield this.handleScrollWindow(); }));
            this.statusSelect.addEventListener("change", () => __awaiter(this, void 0, void 0, function* () {
                yield this.loadTablePosts();
            }));
        });
    }
    openCreatePostModal() {
        const modal = document.getElementById("modal_form");
        if (!modal)
            return;
        modal.style.display = "flex";
        window.requestAnimationFrame(() => modal.classList.add("is-open"));
        document.body.classList.add("lizerium-modal-open");
        const firstInput = modal.querySelector("input[name='autor']");
        if (firstInput) {
            window.setTimeout(() => firstInput.focus(), 80);
        }
    }
    closeCreatePostModal() {
        const modal = document.getElementById("modal_form");
        if (!modal)
            return;
        modal.classList.remove("is-open");
        document.body.classList.remove("lizerium-modal-open");
        window.setTimeout(() => {
            if (!modal.classList.contains("is-open")) {
                modal.style.display = "none";
            }
        }, 200);
    }
    loadTablePosts() {
        return __awaiter(this, void 0, void 0, function* () {
            this.inputLastUserId = document.getElementById("last_user_id");
            const ajax = new Ajax(`getPosts/0/${this.statusSelect.value}/0`, this.cookies);
            const response = yield ajax.sendRequest();
            if (this.utilities.isEmpty(response))
                return;
            const dataResponse = JSON.parse(response);
            const blockPosts = document.querySelector(".lizerium-wish-feed");
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
                const div = document.createElement("article");
                div.classList.add("lizerium-wish-card");
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
                AutorLabel.classList.add("lizerium-wish-author");
                AutorLabel.innerText = posts[i].Autor;
                const avatar = document.createElement("span");
                avatar.classList.add("lizerium-wish-avatar");
                avatar.setAttribute("aria-hidden", "true");
                avatar.style.setProperty("--wish-avatar-seed", `${(posts[i].Id % 6) + 1}`);
                const PostContentDiv = document.createElement("div");
                PostContentDiv.classList.add("post-content", "lizerium-wish-message");
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
                const cardHeader = document.createElement("div");
                cardHeader.classList.add("lizerium-wish-card-head");
                cardHeader.appendChild(AutorLabel);
                const cardDate = document.createElement("time");
                cardDate.classList.add("lizerium-wish-date");
                cardDate.innerText = posts[i].DateTimeUnixString ? posts[i].DateTimeUnixString : "";
                cardHeader.appendChild(cardDate);
                const cardBody = document.createElement("div");
                cardBody.classList.add("lizerium-wish-card-body");
                cardBody.appendChild(cardHeader);
                cardBody.appendChild(PostContentDiv);
                const statusSlot = document.createElement("div");
                statusSlot.classList.add("lizerium-wish-status-row");
                div.appendChild(avatar);
                div.appendChild(cardBody);
                switch (posts[i].Status) {
                    case 1:
                        statusSlot.appendChild(this.createStatusBadge(this.localizationStrings["Wish_Status_1"], "status-new"));
                        break;
                    case 2:
                        statusSlot.appendChild(this.createStatusBadge(this.localizationStrings["Wish_Status_2"], "status-read"));
                        break;
                    case 3:
                        statusSlot.appendChild(this.createStatusBadge(this.localizationStrings["Wish_Status_3"], "status-job"));
                        break;
                    case 4:
                        statusSlot.appendChild(this.createStatusBadge(this.localizationStrings["Wish_Status_4"], "status-delete"));
                        break;
                    case 5:
                        statusSlot.appendChild(this.createStatusBadge(this.localizationStrings["Wish_Status_5"], "status-complete"));
                        break;
                }
                cardBody.appendChild(statusSlot);
                fragment.appendChild(div);
            }
            document.querySelector(".lizerium-wish-feed").appendChild(fragment);
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
    createStatusBadge(text, statusClass) {
        const badge = document.createElement("span");
        badge.classList.add("lizerium-wish-status-badge", statusClass);
        badge.innerText = text;
        return badge;
    }
}
//# sourceMappingURL=wish.js.map