var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
export class ModalForm {
    constructor(headerText, typeFlex, dopClass = null) {
        this.modalForm = document.getElementById("modal_form");
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
                const btnNo = document.createElement("button");
                btnNo.classList.add("btn", "no");
                btnNo.innerText = noText;
                btnNo.addEventListener("click", () => {
                    resolve(false);
                    this.hideModal();
                });
                blockBtn.appendChild(btnNo);
                this.modalBody.appendChild(title);
                this.modalBody.appendChild(blockBtn);
                this.modalForm.style.display = "flex";
            });
        });
    }
}
//# sourceMappingURL=modal_form.js.map