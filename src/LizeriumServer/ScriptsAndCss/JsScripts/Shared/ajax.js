var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
export class Ajax {
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
//# sourceMappingURL=ajax.js.map