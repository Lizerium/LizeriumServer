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
