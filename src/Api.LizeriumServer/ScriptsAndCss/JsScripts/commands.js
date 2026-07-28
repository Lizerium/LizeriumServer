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
