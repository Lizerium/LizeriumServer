var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { Ajax } from "../Shared/ajax";
export class Wiki {
    constructor(utilities, cookies) {
        this.utilities = utilities;
        this.cookies = cookies;
    }
    startApp() {
        return __awaiter(this, void 0, void 0, function* () {
            yield this.utilities.startLoader();
            const ajax = new Ajax(`getAllLocalizedStrings/Views.Knowledge.MarkdownPage`, this.cookies);
            const response = yield ajax.sendRequest();
            this.localizationStrings = JSON.parse(response);
            const { default: Prism } = yield import("prismjs");
            const { default: Mermaid } = yield import("mermaid");
            yield import("prismjs/components/prism-typescript");
            yield import("prismjs/components/prism-csharp");
            yield import("prismjs/components/prism-c");
            yield import("prismjs/components/prism-bash");
            yield import("prismjs/components/prism-ini");
            yield import("prismjs/components/prism-lua");
            yield import("prismjs/components/prism-mermaid");
            Mermaid.initialize({
                startOnLoad: false,
                theme: "dark",
                securityLevel: "strict",
                flowchart: { htmlLabels: false },
            });
            const container = document.querySelector("#prism");
            if (!container)
                return;
            Prism.highlightAllUnder(container);
            document.querySelectorAll(".submenu-toggle").forEach(btn => {
                btn.addEventListener("click", () => {
                    const submenu = btn.nextElementSibling;
                    submenu.classList.toggle("open");
                    btn.textContent = btn.textContent.includes("▸")
                        ? btn.textContent.replace("▸", "▾")
                        : btn.textContent.replace("▾", "▸");
                });
            });
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
                    let parent = link.closest(".submenu");
                    while (parent) {
                        parent.classList.add("open");
                        const toggle = parent.previousElementSibling;
                        if (toggle && toggle.classList.contains("submenu-toggle")) {
                            toggle.textContent = toggle.textContent.replace("▸", "▾");
                        }
                        parent = parent.parentElement.closest(".submenu");
                    }
                }
            });
            Mermaid.run();
            const toastEl = document.getElementById('copyToast');
            const toast = new bootstrap.Toast(toastEl);
            document.querySelectorAll('.patch-block').forEach(block => {
                block.addEventListener('click', () => __awaiter(this, void 0, void 0, function* () {
                    try {
                        yield navigator.clipboard.writeText(block.innerText);
                        toastEl.querySelector('.toast-body').innerText = this.localizationStrings["Know_Copy_Msg"] + `: ${block.innerText}`;
                        toast.show();
                    }
                    catch (err) {
                        console.error('Ошибка копирования:', err);
                    }
                }));
            });
            const categoryCheckboxes = document.querySelectorAll('.category-checkbox');
            categoryCheckboxes.forEach(cb => {
                cb.addEventListener('change', () => {
                    const checkedCats = Array.from(document.querySelectorAll('.category-checkbox:checked'))
                        .map(el => el.value);
                    const patchRows = document.querySelectorAll('.patch-row');
                    patchRows.forEach(row => {
                        var _a;
                        const rowCats = ((_a = row.dataset.categories) === null || _a === void 0 ? void 0 : _a.split(' ')) || [];
                        row.style.display = rowCats.some(cat => checkedCats.includes(cat)) ? '' : 'none';
                    });
                });
            });
            const selectAll = document.querySelector('#selectAllCategories');
            selectAll === null || selectAll === void 0 ? void 0 : selectAll.addEventListener('change', () => {
                const check = selectAll.checked;
                categoryCheckboxes.forEach(cb => cb.checked = check);
                this.updateRows();
            });
            categoryCheckboxes.forEach(cb => {
                cb.addEventListener('change', () => {
                    this.updateRows();
                });
            });
            this.updateRows();
            yield this.utilities.stopLoader();
        });
    }
    updateRows() {
        const patchRows = document.querySelectorAll('.patch-row');
        const checkedCats = Array.from(document.querySelectorAll('.category-checkbox:checked'))
            .map(el => el.value);
        patchRows.forEach(row => {
            var _a;
            const rowCats = ((_a = row.dataset.categories) === null || _a === void 0 ? void 0 : _a.split(',')) || [];
            row.style.display = rowCats.some(cat => checkedCats.includes(cat)) ? '' : 'none';
        });
        const tabCounts = document.querySelectorAll('.tab-count');
        const countData = Array.from(tabCounts).map(span => {
            const dllId = span.id.replace('count-', '');
            const tabRows = document.querySelectorAll(`#${dllId} .patch-row`);
            const visibleCount = Array.from(tabRows).filter(r => r.style.display !== 'none').length;
            return { span, count: visibleCount };
        });
        const maxCount = Math.max(...countData.map(d => d.count), 1);
        countData.forEach(({ span, count }) => {
            span.textContent = count.toString();
            if (count === 0) {
                span.style.opacity = '0.3';
                return;
            }
            else
                span.style.opacity = '1.0';
            const intensity = count / maxCount;
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
//# sourceMappingURL=wiki.js.map