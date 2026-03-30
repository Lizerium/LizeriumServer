/**
 * Класс вывода иконки загрузки в кнопках
 */
class Loader {

    /**
     * Кнопка для вывода loader
     */
    private readonly button: HTMLButtonElement;

    /**
     * Тип loader
     */
    private readonly type: string;

    /**
     * HTML кнопки до подстановки loader
     */
    private prevHtml: string;

    /**
    * Изображение loader
    */
    private readonly loader: HTMLImageElement;

    /**
     * Конструктор
     * @param button - кнопка для вывода loader
     * @param type - тип loader
     */
    constructor(button: HTMLButtonElement, type: string = "spinner") {

        //присваиваем кнопку
        this.button = button;

        //пишем тип loader
        this.type = type;

        //ставим пустую строку в HTML кнопки до подстановки loader
        this.prevHtml = "";

        //если тип spinner не продолжаем
        if (type === "spinner") return;

        //создаем новое изображение
        this.loader = new Image();

        //ставим путь изображения
        this.loader.src = "/img/loader.gif";

        //добавляем класс
        this.loader.classList.add("loader");

        //ставим описание изображения
        this.loader.alt = "loader";
    }

    /**
     * Метод скрывает кнопку
     */
    setDisable(): void {

        //закрываем кнопку
        this.button.setAttribute("disabled", "disabled");

        //получаем HTML из кнопки
        this.prevHtml = this.button.innerHTML;

        //смотрим тип loader
        switch (this.type) {

            case "spinner":

                //очищаем HTML в кнопке
                this.button.innerHTML = "";

                //создаем spinner
                const spinner = document.createElement("div");
                spinner.classList.add("spinner");
                spinner.innerHTML = `<div></div><div></div><div></div>`;

                //добавляем spinner в кнопку
                this.button.appendChild(spinner);

                break;
            case "loader":

                //выводим loader
                this.button.innerHTML = this.loader.outerHTML;

                break;
        }
    }

    /**
     * Метод открывает кнопку
     */
    setEnable(): void {

        //открываем кнопку
        this.button.removeAttribute("disabled");

        //возвращаем HTML кнопки до подстановки loader
        this.button.innerHTML = this.prevHtml;
    }
}