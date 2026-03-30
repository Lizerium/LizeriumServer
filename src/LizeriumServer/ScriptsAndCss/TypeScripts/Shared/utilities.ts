/**
 * Класс вспомогательных методов
 */
export class Utilities {

    /**
     * Метод проверяет строку на пустоту
     * @param value - проверяемая строка
     * @return - true:строка пуста, false:строка не пуста
     */
    isEmpty(value: string): boolean {

        //проверяем строку на null
        if (value == null) return true;

        //проверяем строку на пустоту
        if (value === "") return true;

        //отдаем строка не пуста
        return false;
    }

    public async startLoader(): Promise<void> {
        const container = document.getElementById('preloader');
        container.classList.add('show');
        document.getElementById('preloader').style.display = '';
    }

    public async stopLoader(): Promise<void> {
        const container = document.getElementById('preloader');
        container.classList.remove('show');
        document.getElementById('preloader').style.display = 'none';
    }

    /**
     * Метод ставит warning на input
     * @param input
     */
    setInputWarning(input: HTMLInputElement): void {

        //добавляем класс warning
        input.classList.add("warning_input");

        //делаем задержку
        const timeout = setTimeout(() => {

            //убираем класс warning
            input.classList.remove("warning_input");

            //очищаем задержку
            clearTimeout(timeout);

        }, 3000);
    }

    /**
     * Метод проверяет валидность Email
     * @param email - email
     * @return - true:yes, false:no
     */
    isValidEmail(email: string): boolean {

        //проверяем Email на пустоту
        if (this.isEmpty(email)) return false;

        //паттерн проверки валидности Email
        const pattern = /^([a-z0-9_.-])+@[a-z0-9-]+\.([a-z]{2,4}\.)?[a-z]{2,4}$/i;

        //проверяем валидность Email и отдаем результат
        return pattern.test(email);
    }

    /**
    * Метод делает асинхронную задержку
    * @param mlsec - размер задержки в миллисекундах
    */
    async doDelay(mlsec: number): Promise<void> {

        //создаем новый промис
        // ReSharper disable once TsNotResolved
        return new Promise<void>(resolve => {

            //делаем задержку
            setTimeout(() => {

                //оповещаем промис
                resolve(null);

            }, mlsec);
        });
    }

    /**
     * Метод отдаем ширину окна браузера
     * @returns - ширина окна браузера
     */
    get getWidh(): number {

        //отдаем максимальную ширину
        return window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
    }

    /**
     * Метод отдает высоту окна браузера
     * @returns - высота окна браузера
     */
    get getHeight(): number {

        //отдаем максимальную высоту
        return window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
    }

    /**
     * Метод генерирует случайное число в заданном диапазоне
     * @param min - минимальное число
     * @param max - максимальное число
     */
    getRandomInt(min: number, max: number) {

        min = Math.ceil(min);

        max = Math.floor(max);

        return Math.floor(Math.random() * (max - min + 1)) + min;
    }

    /**
     * Метод копирует текст в буфер обмена
     * @param value - текст для копирования в буфер
     * @param element - элемент вызывающий действие копирования
     */
    copyValueToBuffer(value: string, element: any): boolean {

        try {

            //получаем координаты кнопки
            const coord = element.getBoundingClientRect();

            //получаем расстояние сколько проскроллено сверху
            const scrolled = window.pageYOffset || document.documentElement.scrollTop;

            //получаем iOs ли устройство
            const iOsDevice = navigator.userAgent.match(/ipad|iphone/i);

            //создаем textarea
            const textArea = document.createElement("textarea");

            //ставим что textarea только для чтения
            textArea.readOnly = true;

            //ставим textarea отступ сверху на величину скролла + координаты кнопки что бы экран не прыгал
            textArea.style.top = `${Math.round(scrolled + coord.top)}px`;

            //подставляем невидимый класс
            textArea.classList.add("text_copy");

            //подставляем текст в textarea
            textArea.value = value;

            //добавляем textarea в DOM
            document.body.appendChild(textArea);

            //делаем textarea в фокус 
            textArea.focus();

            //проверяем iOs устройство
            if (iOsDevice) {

                const editable = textArea.contentEditable;

                const readOnly = textArea.readOnly;

                textArea.contentEditable = "true";

                textArea.readOnly = false;

                const range = document.createRange();

                range.selectNodeContents(textArea);

                const selection = window.getSelection();

                selection.removeAllRanges();

                selection.addRange(range);

                textArea.setSelectionRange(0, 999999);

                textArea.contentEditable = editable;

                textArea.readOnly = readOnly;

            } else {

                //делаем textarea выбранной
                textArea.select();
            }

            //выполняем команду по копированию в буфер обмена
            const resultCopy = document.execCommand("copy");

            //удаляем textarea
            document.body.removeChild(textArea);

            //отдаем результат копирования
            return resultCopy;

        } catch (e) {

            //выводим ошибку на консоль
            console.error("Error: ", e);

            //возвращаем что данные не скопированы
            return false;
        }
    }

    /**
     * Метод получает текущую дату в строковом значении
     */
    getCurrentDate(): string {

        //получаем текущую дату
        const date = new Date();

        //получаем текущий день
        const nowDay = date.getDate();

        //получаем текущий месяц
        const nowMonth = date.getMonth() + 1;

        //добавка для полного дня с нулем
        let forFullDay = "";

        //проверяем если день менее 10
        if (nowDay < 10) {

            //пишем ноль в добавку
            forFullDay = "0";
        }

        //добавка для полного месяца
        let forFullMonth = "";

        //проверяем если месяц менее 10
        if (nowMonth < 10) {

            //пишем ноль в добавку
            forFullMonth = "0";
        }

        return `${forFullDay}${nowDay}.${forFullMonth}${nowMonth}.${date.getFullYear()}`;
    }

    /**
     * Метод генерирует UIID
     */
    generateUuid() {

        var d = new Date().getTime();

        var d2 = ((typeof performance !== "undefined") && performance.now && (performance.now() * 1000)) || 0;

        // ReSharper disable StringLiteralTypo
        return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, c => {

            var r = Math.random() * 16;

            if (d > 0) {

                r = (d + r) % 16 | 0;
                d = Math.floor(d / 16);

            } else {

                r = (d2 + r) % 16 | 0;
                d2 = Math.floor(d2 / 16);
            }

            return (c === "x" ? r : (r & 0x3 | 0x8)).toString(16);
        });
    }
}

export interface LocalizationStrings {
    [key: string]: string;
}