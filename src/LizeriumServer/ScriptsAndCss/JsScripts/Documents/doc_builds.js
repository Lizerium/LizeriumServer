var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
export class DocBuilds {
    constructor(utilities, cookies) {
        this.utilities = utilities;
        this.cookies = cookies;
    }
    start() {
        return __awaiter(this, void 0, void 0, function* () {
            const buttons = document.querySelectorAll(".load-build-component");
            buttons.forEach(button => {
                button.addEventListener("click", () => __awaiter(this, void 0, void 0, function* () {
                    var _a;
                    const targetId = (_a = button.getAttribute("data-target")) === null || _a === void 0 ? void 0 : _a.replace("#", "");
                    const buildName = button.getAttribute("data-name");
                    if (!targetId || !buildName)
                        return;
                    const collapseEl = document.getElementById(targetId);
                    const body = collapseEl === null || collapseEl === void 0 ? void 0 : collapseEl.querySelector(".build-body");
                    if (!collapseEl || !body)
                        return;
                    const isShown = collapseEl.classList.contains("show");
                    if (isShown) {
                        bootstrap.Collapse.getOrCreateInstance(collapseEl).hide();
                        return;
                    }
                    if (!body.hasChildNodes()) {
                        body.innerHTML = `<div class="text-muted">Загрузка...</div>`;
                        try {
                            const response = yield fetch(`/docs/build/${encodeURIComponent(buildName)}`);
                            if (!response.ok) {
                                body.innerHTML = `<div class="text-danger">Ошибка загрузки</div>`;
                                return;
                            }
                            const html = yield response.text();
                            body.innerHTML = html;
                        }
                        catch (err) {
                            body.innerHTML = `<div class="text-danger">Ошибка запроса</div>`;
                            return;
                        }
                    }
                    const onShown = () => {
                        collapseEl.scrollIntoView({ behavior: "smooth", block: "start" });
                        collapseEl.removeEventListener("shown.bs.collapse", onShown);
                    };
                    collapseEl.addEventListener("shown.bs.collapse", onShown);
                    collapseEl.addEventListener("hidden.bs.collapse", () => {
                        body.innerHTML = "";
                    }, { once: true });
                    const bsCollapse = bootstrap.Collapse.getOrCreateInstance(collapseEl);
                    bsCollapse.show();
                }));
            });
        });
    }
}
//# sourceMappingURL=doc_builds.js.map