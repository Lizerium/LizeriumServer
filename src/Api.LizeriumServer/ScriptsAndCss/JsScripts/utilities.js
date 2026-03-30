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
