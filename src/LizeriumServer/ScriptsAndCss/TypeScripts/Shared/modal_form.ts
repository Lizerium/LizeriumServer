export class ModalForm {
    /**
     * Контейнер модальной формы
     */
    private readonly modalForm: HTMLDivElement;

    /**
     * Крестик закрытия модальной формы
     */
    private readonly modalClose: HTMLDivElement;

    /**
     * контейнер тела модальной формы
     */
    readonly modalBody: HTMLDivElement;

    /**
     * Resolve
     */
    private resolve: any;

    /**
     * Конструктор
     * @param headerText - заголовок модального окна
     * @param typeFlex - тип flex контейнера (row||column)
     * @param dopClass - дополнительный класс
     */
    constructor(headerText: string, typeFlex: string, dopClass: string = null) {

        //получаем контейнер модальной формы
        this.modalForm = document.getElementById("modal_form") as unknown as HTMLDivElement;

        //получаем контейнер тела модальной формы
        this.modalBody = this.modalForm.querySelector(".modal_body") as unknown as HTMLDivElement;

        //удаляем предыдущие классы у тела модальной формы
        this.modalBody.classList.remove("row", "column", "start");

        //добавляем класс типа flex контейнера
        this.modalBody.classList.add(typeFlex);

        //если есть дополнительный класс
        if (dopClass != null) {

            //добавляем дополнительный класс
            this.modalBody.classList.add(dopClass);
        }

        //получаем блок заголовка хедера
        const headerModel = this.modalForm.querySelector(".modal_header") as unknown as HTMLDivElement;

        //проверяем блок заголовка
        if (headerModel) {

            //подставляем текст заголовка
            headerModel.querySelector("h3").innerHTML = headerText;
        }

        //создаем событие клик в любом месте
        window.addEventListener("click", (event) => {

            //если клик не по подложке модального окна, не продолжаем
            if (event.target !== this.modalForm) return;

            //закрываем модальную форму
            this.hideModal();
        });

        //присваиваем null resolve
        this.resolve = null;
    }

    /**
     * Метод открывает модальную форму
     * @param innerHtml - внутренний HTML для модальной формы
     */
    showModalWithHtml(innerHtml: string): void {

        //если есть внутренний HTML для модальной формы
        if (innerHtml) {

            //подставляем HTML в контейнер тела модальной формы
            this.modalBody.innerHTML = innerHtml;
        }

        //ставим модальной форме display flex
        this.modalForm.style.display = "flex";
    }

    /**
    * Метод открывает модальную форму
    * @param element - HTML элемент
    */
    showModalWithElement(element: HTMLDivElement): void {

        //подставляем HTML элемент в контейнер тела модальной формы
        this.modalBody.appendChild(element);

        //ставим модальной форме display flex
        this.modalForm.style.display = "flex";
    }

    /**
     * Метод заменяет контент модальной формы
     * @param innerHtml - внутренний HTML для модальной формы
     */
    changeContent(innerHtml: string): void {

        //если есть внутренний HTML для модальной формы
        if (innerHtml) {

            //убираем класс
            this.modalBody.classList.remove("start");

            //заменяем HTML в контейнере тела модальной формы
            this.modalBody.innerHTML = innerHtml;
        }
    }

    /**
     * Метод закрывает модальную форму
     */
    hideModal(): void {

        //ставим модальной форме display none
        this.modalForm.style.display = "none";

        //убираем HTML в контейнере тела модальной формы
        this.modalBody.innerHTML = "";

        //если resolve null, не продолжаем
        if (this.resolve == null) return;

        //вызываем промис с false
        this.resolve(false);
    }

    /**
     * Метод открывает форму подтверждения
     * @param textConfirm - текст подтверждения
     * @return - результат подтверждения
     */
    async showConfirmForm(textConfirm: string, yesText: string, noText: string): Promise<boolean> {

        //создаем новый промис
        // ReSharper disable once TsNotResolved
        return new Promise<boolean>(resolve => {

            //присваиваем ссылку на resolve
            this.resolve = resolve;

            //создаем заголовок подтверждения
            const title = document.createElement("h3");
            title.classList.add("title_confirm");
            title.innerText = textConfirm;

            //создаем блок кнопок
            const blockBtn = document.createElement("div");
            blockBtn.classList.add("confirm_btn_container");

            //создаем кнопку нет
            const btnNo = document.createElement("button");
            btnNo.classList.add("btn", "no");
            btnNo.innerText = noText;

            //добавляем обработку клика по кнопке Нет
            btnNo.addEventListener("click", () => {

                //вызываем промис с false
                resolve(false);

                //закрываем модальную форму
                this.hideModal();
            });

            //добавляем кнопки в блок кнопок
            blockBtn.appendChild(btnNo);

            //добавляем заголовок подтверждения и блок кнопок в модальное окно
            this.modalBody.appendChild(title);
            this.modalBody.appendChild(blockBtn);

            //ставим модальной форме display flex
            this.modalForm.style.display = "flex";
        });
    }
}
