var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
export class Index {
    constructor(utilities, cookies) {
        this.utilities = utilities;
        this.cookies = cookies;
    }
    startLoadModdbImage() {
        return __awaiter(this, void 0, void 0, function* () {
            const moddbImg = document.getElementById("moddb-img");
            if (moddbImg && moddbImg.dataset.src) {
                moddbImg.src = moddbImg.dataset.src;
            }
        });
    }
}
//# sourceMappingURL=index.js.map