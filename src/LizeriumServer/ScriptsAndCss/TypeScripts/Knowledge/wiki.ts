import { Ajax } from "../Shared/ajax";
import { Cookies } from "../Shared/cookies";
import { LocalizationStrings, Utilities } from "../Shared/utilities";
declare var bootstrap: any;
export class Wiki {

    /**
     * Экземпляр класса утилит
     */
    private readonly utilities: Utilities;

    /**
     * Экземпляр класса работы с куками
     */
    private readonly cookies: Cookies;

    /**
     * Локализованные строки страницы
     */
    private localizationStrings: LocalizationStrings;

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

    /**
     * Метод запускает авторизацию
     */
    async startApp(): Promise<void> {
        await this.utilities.startLoader();

        //создаем экземпляр Ajax
        const ajax = new Ajax(`getAllLocalizedStrings/Views.Knowledge.MarkdownPage`, this.cookies);
        //отправляем запрос
        const response = await ajax.sendRequest();
        //фиксируем переводы
        this.localizationStrings = JSON.parse(response);

        // Ленивая загрузка Prism только когда нужна
        const { default: Prism } = await import("prismjs");
        const { default: Mermaid } = await import("mermaid");
        await import("prismjs/components/prism-typescript");
        await import("prismjs/components/prism-csharp");
        await import("prismjs/components/prism-c");
        await import("prismjs/components/prism-bash");
        await import("prismjs/components/prism-ini");
        await import("prismjs/components/prism-lua");
        await import("prismjs/components/prism-mermaid");

        Mermaid.initialize({
            startOnLoad: false, // сами контролируем рендер
            theme: "dark",   // можно 'forest', 'dark', 'neutral', 'base'
            securityLevel: "strict",          // можешь оставить strict
            flowchart: { htmlLabels: false }, // на всякий случай, если есть flowchart
        });

        // Загружаем языки только при заходе на wiki
        const container = document.querySelector("#prism"); // или .wiki-page, что угодно уникальное
        if (!container) return; // не на wiki — не подсвечиваем

        // Подсветить только внутри wiki-контейнера
        Prism.highlightAllUnder(container as HTMLElement);

        // Подменю
        document.querySelectorAll(".submenu-toggle").forEach(btn => {
            btn.addEventListener("click", () => {
                const submenu = btn.nextElementSibling;
                submenu.classList.toggle("open");
                btn.textContent = btn.textContent.includes("▸")
                    ? btn.textContent.replace("▸", "▾")
                    : btn.textContent.replace("▾", "▸");
            });
        });

        // Мобильный сайдбар
        const sidebar = document.querySelector(".sidebar");
        const toggleBtn = document.createElement("button");
        toggleBtn.className = "menu-toggle";
        toggleBtn.textContent = "☰";
        document.body.appendChild(toggleBtn);

        toggleBtn.addEventListener("click", () => {
            sidebar.classList.toggle("open");
        });
        const currentUrl = window.location.pathname.toLowerCase();
        const links = document.querySelectorAll(".sidebar a");

        links.forEach(link => {
            const href = link.getAttribute("href").toLowerCase();

            if (currentUrl === href || currentUrl.endsWith(href)) {
                link.classList.add("active");

                // раскрываем все родительские подменю
                let parent = link.closest(".submenu");
                while (parent) {
                    parent.classList.add("open"); // раскрываем ul
                    const toggle = parent.previousElementSibling;
                    if (toggle && toggle.classList.contains("submenu-toggle")) {
                        toggle.textContent = toggle.textContent.replace("▸", "▾");
                    }
                    parent = parent.parentElement.closest(".submenu");
                }
            }
        });

        //console.log("prism ok");
        Mermaid.run();

        const toastEl = document.getElementById('copyToast');
        const toast = new bootstrap.Toast(toastEl);

        document.querySelectorAll('.patch-block').forEach(block => {
            block.addEventListener('click', async () => {
                try {
                    await navigator.clipboard.writeText((block as HTMLElement).innerText);
                    (toastEl.querySelector('.toast-body') as HTMLDivElement).innerText = this.localizationStrings["Know_Copy_Msg"] + `: ${(block as HTMLElement).innerText}`;
                    toast.show();
                } catch (err) {
                    console.error('Ошибка копирования:', err);
                }
            });
        });

        // Получаем все чекбоксы категорий
        const categoryCheckboxes = document.querySelectorAll<HTMLInputElement>('.category-checkbox');

        categoryCheckboxes.forEach(cb => {
            cb.addEventListener('change', () => {
                // Список выбранных категорий
                const checkedCats: string[] = Array.from(document.querySelectorAll<HTMLInputElement>('.category-checkbox:checked'))
                    .map(el => el.value);

                // Перебираем все строки таблицы
                const patchRows = document.querySelectorAll<HTMLTableRowElement>('.patch-row');
                patchRows.forEach(row => {
                    const rowCats = row.dataset.categories?.split(' ') || [];
                    // Показываем строку, если хотя бы одна категория выбрана
                    row.style.display = rowCats.some(cat => checkedCats.includes(cat)) ? '' : 'none';
                });
            });
        });

        const selectAll = document.querySelector<HTMLInputElement>('#selectAllCategories');

        // Все категории / снять все
        selectAll?.addEventListener('change', () => {
            const check = selectAll.checked;
            categoryCheckboxes.forEach(cb => cb.checked = check);
            this.updateRows();
        });

        // Индивидуальные категории
        categoryCheckboxes.forEach(cb => {
            cb.addEventListener('change', () => {
                this.updateRows();
            });
        });

        this.updateRows();

        await this.utilities.stopLoader();
    }

    private updateRows() {
        const patchRows = document.querySelectorAll<HTMLTableRowElement>('.patch-row');
        const checkedCats = Array.from(document.querySelectorAll<HTMLInputElement>('.category-checkbox:checked'))
            .map(el => el.value);

        patchRows.forEach(row => {
            const rowCats = row.dataset.categories?.split(',') || [];
            row.style.display = rowCats.some(cat => checkedCats.includes(cat)) ? '' : 'none';
        });

        // Обновляем счетчики по вкладкам
        const tabCounts = document.querySelectorAll<HTMLSpanElement>('.tab-count');

        // Сначала собираем все данные
        const countData = Array.from(tabCounts).map(span => {
            const dllId = span.id.replace('count-', '');
            const tabRows = document.querySelectorAll<HTMLTableRowElement>(`#${dllId} .patch-row`);
            const visibleCount = Array.from(tabRows).filter(r => r.style.display !== 'none').length;
            return { span, count: visibleCount };
        });

        // Находим максимум
        const maxCount = Math.max(...countData.map(d => d.count), 1); // на всякий случай 1, чтобы не делить на 0

        // Потом обновляем DOM разом
        countData.forEach(({ span, count }) => {
            span.textContent = count.toString();

            if (count === 0) {
                span.style.opacity = '0.3'; // визуально полупрозрачное
                return;
            }
            else span.style.opacity = '1.0';

            const intensity = count / maxCount;

            // Цвет и стиль
            const color = `rgba(0, 200, 255, ${0.3 + 0.7 * intensity})`;
            const fontSize = 14 + 4 * intensity;
            const borderWidth = 1 + 2 * intensity;

            span.style.color = color;
            span.style.fontWeight = 'bold';
            span.style.fontSize = `${fontSize}px`;
            span.style.border = `${borderWidth}px solid ${color}`;
            span.style.borderRadius = '4px';
            span.style.padding = '2px 6px';
        });
    }
}