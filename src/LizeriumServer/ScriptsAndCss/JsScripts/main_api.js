var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { Cookies } from "./Shared/cookies";
import { Wish } from "./Home/wish";
import { Utilities } from "./Shared/utilities";
import { Documents } from "./Documents/documents";
import { Index } from "./Home/index";
import { Launcher } from "./Home/launcher";
import { DocBuilds } from "./Documents/doc_builds";
import { DocHook } from "./Documents/doc_hook";
import { Wiki } from "./Knowledge/wiki";
import { Game } from "./Home/game";
(() => {
    window.addEventListener("DOMContentLoaded", () => __awaiter(void 0, void 0, void 0, function* () {
        const utilities = new Utilities();
        const cookies = new Cookies();
        const currentUrl = new URL(document.location.href);
        const pathname = currentUrl.pathname.toLowerCase();
        const partsPath = pathname.split("/");
        window.addEventListener('scroll', () => {
        });
        if (partsPath.length == 2) {
        }
        let index = null;
        let wiki = null;
        switch (partsPath[1]) {
            case "":
            case "index":
                index = new Index(utilities, cookies);
                break;
            case "wiki":
                {
                    console.log("load wiki");
                    wiki = new Wiki(utilities, cookies);
                }
                break;
        }
        switch (partsPath[2]) {
            case "":
                {
                    console.log("load index *");
                    index = new Index(utilities, cookies);
                }
                break;
            case "index":
                {
                    index = new Index(utilities, cookies);
                }
                break;
            case "game":
                {
                    const game = new Game();
                    game.start();
                }
                break;
            case "all":
                {
                    const doc = new Documents(utilities, cookies);
                    doc.start();
                }
                break;
            case "wish":
                {
                    const auth = new Wish(utilities, cookies);
                    yield auth.startApp();
                }
                break;
            case "builds":
                {
                    const doc = new DocBuilds(utilities, cookies);
                    doc.start();
                }
                break;
            case "install":
                {
                }
                break;
            case "hook":
                {
                    const doc = new DocHook(utilities, cookies);
                    doc.start();
                }
                break;
            case "launcher":
                {
                    var launcher = new Launcher(utilities, cookies);
                    launcher.start();
                }
                break;
            case "error":
                const Body = document.querySelector('body');
                Body.style.background = '';
                Body.style.backgroundImage = '';
                localStorage.clear();
                break;
            default:
                break;
        }
        yield utilities.stopLoader();
        if (partsPath[1] == "index" || partsPath[1] == "") {
            if (index) {
                yield index.startLoadModdbImage();
            }
            else {
                console.warn("⚠️ Index не был инициализирован — moddb image не загружен");
            }
        }
        if (partsPath[1] == "wiki") {
            if (wiki) {
                console.log("Wiki инициализирован!");
                yield wiki.startApp();
            }
            else {
                console.warn("⚠️ Wiki не был инициализирован — moddb image не загружен");
            }
        }
    }));
})();
//# sourceMappingURL=main_api.js.map