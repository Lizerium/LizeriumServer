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
        var newCategory = document.querySelector("#newCategory");
        var newName = document.querySelector("#newName");
        var newExampleInput = document.querySelector("#newExampleInput");
        var newDescription = document.querySelector("#newDescription");
        var newGif = document.querySelector("#newGif");
        var newLikes = document.querySelector("#newLikes");
        var buttonCreate = document.querySelector("#createCommand");
        var newStatus = document.querySelector("#newStatus");
        buttonCreate.addEventListener('click', () => __awaiter(this, void 0, void 0, function* () {
            console.log("Create Command Start");
            const selectedStatusId = parseInt(newStatus.value);
            console.log(newCategory.value);
            console.log(newName.value);
            console.log(newDescription.value);
            console.log(newGif.value);
            console.log(newExampleInput.value);
            console.log(parseInt(newLikes.value));
            console.log(selectedStatusId);
            const dataRequest = {
                "newCategory": newCategory.value,
                "newName": newName.value,
                "newDescription": newDescription.value,
                "newGif": newGif.value,
                "newExampleInput": newExampleInput.value,
                "newLikes": parseInt(newLikes.value),
                "newStatus": selectedStatusId,
            };
            const ajax = new Ajax("saveCommand", this.cookies);
            const response = yield ajax.sendRequest(dataRequest);
            if (response !== "ok") {
                document.location.href = "/Home/Error";
                return;
            }
            else {
                document.location.href = "/Commands";
            }
        }));
        const allSelects = document.querySelectorAll("#changeCommand");
        for (let i = 0; i < allSelects.length; i++) {
            allSelects[i].addEventListener('click', () => __awaiter(this, void 0, void 0, function* () { return yield this.updateChangeAsync(allSelects[i]); }));
        }
        var buttonDelete = document.querySelectorAll("#deleteCommand");
        for (let i = 0; i < buttonDelete.length; i++) {
            buttonDelete[i].addEventListener('click', () => __awaiter(this, void 0, void 0, function* () { return yield this.deleteAsync(buttonDelete[i]); }));
        }
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
