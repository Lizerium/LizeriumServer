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
    window.addEventListener("DOMContentLoaded", () => __awaiter(this, void 0, void 0, function* () {
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
            case "news":
                {
                    const news = new NewsAdmin();
                    news.startNews();
                }
                break;
            case "products":
                {
                    const products = new ProductsAdmin();
                    products.startProducts();
                }
                break;
            default:
                break;
        }
        const buttonScroll = document.getElementById("scrollButton");
        if (buttonScroll) {
            const updateScrollButton = () => {
                const scrollPosition = window.scrollY;
                const pageHeight = document.documentElement.scrollHeight;
                const windowHeight = window.innerHeight;
                buttonScroll.classList.toggle("is-up", scrollPosition > (pageHeight - windowHeight) / 2);
            };
            updateScrollButton();
            window.addEventListener("scroll", updateScrollButton);
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
        const sidebarToggle = document.getElementById("sidebarToggle");
        if (sidebarToggle) {
            const isCollapsed = localStorage.getItem("api-sidebar-collapsed") === "true";
            document.body.classList.toggle("sidebar-collapsed", isCollapsed);
            sidebarToggle.addEventListener("click", () => {
                const nextState = !document.body.classList.contains("sidebar-collapsed");
                document.body.classList.toggle("sidebar-collapsed", nextState);
                localStorage.setItem("api-sidebar-collapsed", nextState.toString());
            });
        }
        const fileInputs = document.querySelectorAll("input[type='file']");
        for (let i = 0; i < fileInputs.length; i++) {
            const input = fileInputs[i];
            input.addEventListener("change", () => {
                input.setAttribute("data-file-name", input.files && input.files.length > 0
                    ? input.files[0].name
                    : "Файл не выбран");
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
