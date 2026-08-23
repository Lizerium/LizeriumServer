import { Cookies } from "./Shared/cookies";
import { Wish } from "./Home/wish";
import { ModalForm } from "./Shared/modal_form";
import { Utilities } from "./Shared/utilities";
import { Documents } from "./Documents/documents";
import { Index } from "./Home/index";
import { Launcher } from "./Home/launcher";
import { DocBuilds } from "./Documents/doc_builds";
import { DocHook } from "./Documents/doc_hook";
import { Wiki } from "./Knowledge/wiki";
import { Game } from "./Home/game";

(() => {
    //по загрузке окна
    window.addEventListener("DOMContentLoaded", async () => {
        //создаем экземпляр класса утилит
        const utilities = new Utilities();

        //создаем экземпляр класса кук
        const cookies = new Cookies();

        //получаем текущий URL
        const currentUrl = new URL(document.location.href);

        //получаем путь из URL
        const pathname = currentUrl.pathname.toLowerCase();

        //разбиваем пути URL на части
        const partsPath = pathname.split("/");

        /*  Добавляем  класс  "scroll-hidden"  к  кнопкам  меню  при  скроллинге  */
        window.addEventListener('scroll', () => {

        });

        if (partsPath.length == 2) {
            //console.log("load index **");
        }
        //console.log(partsPath[2]);

        let index = null;
        let wiki = null;
        if (partsPath[1] === "news") {
            var launcher = new Launcher(utilities, cookies);
            launcher.start();
        }
        switch (partsPath[1]) {
            case "":
            case "index":
                index = new Index(utilities, cookies);
                break;
            case "wiki": //страница документации базы знаний
                {
                    console.log("load wiki");
                    wiki = new Wiki(utilities, cookies);
                }
                break;
        }

        //смотрим путь
        switch (partsPath[2]) {
            case "": //страница авторизации
                {
                    console.log("load index *")
                    index = new Index(utilities, cookies);
                }
                break;
            case "index": //страница авторизации
                {
                    //console.log("load index");
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
                    //console.log("load documents");
                }
                break;
            case "wish":
                {
                    //console.log("load wish");
                    const auth = new Wish(utilities, cookies);
                    //запускаем авторизацию
                    await auth.startApp();
                }
                break;
            case "builds":
                {
                    const doc = new DocBuilds(utilities, cookies);
                    doc.start();
                    //console.log("load builds page docs");
                }
                break;
            case "install":
                {
                    //console.log("load Install page docs");
                }
                break;
            case "hook":
                {
                    //console.log("load Hook page docs");
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
                const Body = document.querySelector('body') as HTMLBodyElement;
                Body.style.background = '';
                Body.style.backgroundImage = '';
                localStorage.clear();
                //console.log("error page");
                break;
            default:
                break;
        }

        //console.log("end load");
        await utilities.stopLoader();
        if (partsPath[1] == "index" || partsPath[1] == "")
        {
            if (index) {
                await index.startLoadModdbImage();
            } else {
                console.warn("⚠️ Index не был инициализирован — moddb image не загружен");
            }
        }
        if (partsPath[1] == "wiki")
        {
            if (wiki) {
                console.log("Wiki инициализирован!");
                await wiki.startApp();
            } else {
                console.warn("⚠️ Wiki не был инициализирован — moddb image не загружен");
            }
        }
    });
})();
