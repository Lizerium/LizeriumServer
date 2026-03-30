var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(() => {
    window.addEventListener("load", () => __awaiter(this, void 0, void 0, function* () {
        const utilities = new Utilities();
        const cookies = new Cookies();
        const currentUrl = new URL(document.location.href);
        const pathname = currentUrl.pathname.toLowerCase();
        const partsPath = pathname.split("/");
        switch (partsPath[1]) {
            case "":
                {
                    const auth = new Auth(utilities, cookies);
                    auth.startAuth();
                }
                break;
            case "confirmation":
                {
                    const auth = new Auth(utilities, cookies);
                    auth.startConfirm();
                }
                break;
            case "posts":
                {
                    const posts = new Posts(utilities, cookies);
                    yield posts.startPosts();
                }
                break;
            case "commands":
                {
                    const commands = new Commands(utilities, cookies);
                    yield commands.startCommands();
                }
                break;
            default:
                break;
        }
        const buttonScroll = document.getElementById("scrollButton");
        if (buttonScroll) {
            buttonScroll.addEventListener("click", () => {
                const scrollPosition = window.scrollY;
                const pageHeight = document.documentElement.scrollHeight;
                const windowHeight = window.innerHeight;
                if (scrollPosition > (pageHeight - windowHeight) / 2) {
                    window.scrollTo({ top: 0, behavior: "smooth" });
                }
                else {
                    window.scrollTo({ top: pageHeight, behavior: "smooth" });
                }
            });
        }
        const logout = document.getElementById("logout");
        if (!logout)
            return;
        logout.addEventListener("click", () => __awaiter(this, void 0, void 0, function* () {
            const confirmForm = new ModalForm("Выход из админки", "column");
            const resultConfirm = yield confirmForm.showConfirmForm("Вы уверены что хотите выйти?", "Да", "Нет");
            if (!resultConfirm)
                return;
            document.location.href = "/logout";
        }));
    }));
})();
