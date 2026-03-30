import { Cookies } from "../Shared/cookies";
import { Utilities } from "../Shared/utilities";

declare var bootstrap: any;

export class DocBuilds {
    /**
     * Максимальная длинна поста 
     */
    private truncateLength: number;

    /**
     * Флаг что AJAX в процессе
     */
    private ajaxInProgress: boolean;

    /**
     * Экземпляр класса утилит
     */
    private readonly utilities: Utilities;

    /**
     * Экземпляр класса работы с куками
     */
    private readonly cookies: Cookies;

    /**
     * Конструктор
     * @param utilities - экземпляр класса утилит
     * @param cookies - экземпляр класса кук
     */
    constructor(utilities: Utilities, cookies: Cookies) {
        //присваиваем экземпляр класса утилит
        this.utilities = utilities;

        //присваиваем экземпляр класса кук
        this.cookies = cookies;
    }

    async start(): Promise<void> {
        const buttons = document.querySelectorAll(".load-build-component");
        buttons.forEach(button => {
            button.addEventListener("click", async () => {
                const targetId = button.getAttribute("data-target")?.replace("#", "");
                const buildName = button.getAttribute("data-name");
                if (!targetId || !buildName) return;

                const collapseEl = document.getElementById(targetId);
                const body = collapseEl?.querySelector(".build-body");
                if (!collapseEl || !body) return;

                // Если уже раскрыто — свернём
                const isShown = collapseEl.classList.contains("show");
                if (isShown) {
                    bootstrap.Collapse.getOrCreateInstance(collapseEl).hide();
                    return;
                }

                // Если ещё не загружено — загружаем
                if (!body.hasChildNodes()) {
                    body.innerHTML = `<div class="text-muted">Загрузка...</div>`;
                    try {
                        const response = await fetch(`/docs/build/${encodeURIComponent(buildName)}`);
                        if (!response.ok) {
                            body.innerHTML = `<div class="text-danger">Ошибка загрузки</div>`;
                            return;
                        }

                        const html = await response.text();
                        body.innerHTML = html;

                    } catch (err) {
                        body.innerHTML = `<div class="text-danger">Ошибка запроса</div>`;
                        return;
                    }
                }

                // Подписываемся на завершение анимации
                const onShown = () => {
                    collapseEl.scrollIntoView({ behavior: "smooth", block: "start" } as ScrollIntoViewOptions);
                    collapseEl.removeEventListener("shown.bs.collapse", onShown);
                };
                collapseEl.addEventListener("shown.bs.collapse", onShown);
                // Внутри addEventListener блока кнопки, после получения collapseEl и body
                collapseEl.addEventListener("hidden.bs.collapse", () => {
                    body.innerHTML = ""; // Очищаем содержимое после закрытия
                }, { once: true }); // once: true — чтобы не множились обработчики

                // Теперь вручную запускаем анимацию
                const bsCollapse = bootstrap.Collapse.getOrCreateInstance(collapseEl);
                bsCollapse.show();
            });
        });
    }
}