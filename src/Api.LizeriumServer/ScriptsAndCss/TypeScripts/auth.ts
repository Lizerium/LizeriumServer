/**
 * Класс авторизации
 */
class Auth {

    /**
     * Экземпляр класса утилит
     */
    private readonly utilities: Utilities;

    /**
     * Экземпляр класса кук
     */
    private readonly cookies: Cookies;

    /**
     * Input секретного ключа
     */
    private readonly inputSecretKey: HTMLInputElement;

    /**
     * Кнопка авторизации
     */
    private readonly btnSignIn: HTMLButtonElement;

    /**
     * Input кода подтверждения
     */
    private readonly inputConfirmRecord: HTMLInputElement;

    /**
     * Кнопка отправки кода подтверждения
     */
    private readonly btnSendConfirm: HTMLButtonElement;

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

        //получаем input секретного ключа
        this.inputSecretKey = document.getElementById("secret_key") as HTMLInputElement;

        //получаем кнопку авторизации
        this.btnSignIn = document.getElementById("sign_in") as HTMLButtonElement;

        //получаем input кода подтверждения
        this.inputConfirmRecord = document.getElementById("confirm_record") as HTMLInputElement;

        //получаем кнопку отправки кода подтверждения
        this.btnSendConfirm = document.getElementById("send_confirm") as HTMLButtonElement;
    }

    /**
     * Метод запускает авторизацию
     */
    startAuth(): void {

        //проверяем контролы
        if (!this.inputSecretKey || !this.btnSignIn) return;

        //привязываем событие на клик по кнопке авторизации
        this.btnSignIn.addEventListener("click", async () => await this.sendSecretKeyAsync());
    }

    /**
     * Метод запускает подтверждение авторизации
     */
    startConfirm(): void {

        //проверяем контролы
        if (!this.inputConfirmRecord || !this.btnSendConfirm) return;

        //привязываем событие на клик по кнопке отправки кода подтверждения
        this.btnSendConfirm.addEventListener("click", async () => await this.sendConfirmCodeAsync());
    }

    /**
     * Метод отправляет секретный ключ авторизации
     */
    private async sendSecretKeyAsync(): Promise<void> {

        //создаем loader
        const loader = new Loader(this.btnSignIn);

        try {

            //закрываем кнопку
            loader.setDisable();

            //создаем объект запроса
            const requestData = {
                "secretKey": this.inputSecretKey.value,
                "recaptchaToken": ""
            };

            //проверяем секретный ключ
            if (this.utilities.isEmpty(requestData.secretKey)) {

                //ставим warning
                this.utilities.setInputWarning(this.inputSecretKey);

                //не продолжаем
                return;
            }

            //создаем экземпляр ajax
            const ajax = new Ajax("auth", this.cookies);

            //отправляем запрос
            const response = await ajax.sendRequest(requestData);

            //если ответ успешный
            if (response === "ok") {

                //редиректим на страницу подтверждения
                document.location.href = "/cabinet" //"/confirmation";

                //не продолжаем
                return;
            }

            // Refresh the page if the browser holds stale protection cookies.
            document.location.reload();

        } finally {

            //открываем кнопку
            loader.setEnable();
        }
    }

    /**
     * Метод отправляет код подтверждения
     */
    private async sendConfirmCodeAsync(): Promise<void> {

        //создаем loader
        const loader = new Loader(this.btnSendConfirm);

        try {

            //закрываем кнопку
            loader.setDisable();

            //создаем объект запроса
            const requestData = {
                "confirmRecord": this.inputConfirmRecord.value,
                "recaptchaToken": ""
            };

            //проверяем секретный ключ
            if (this.utilities.isEmpty(requestData.confirmRecord)) {

                //ставим warning
                this.utilities.setInputWarning(this.inputConfirmRecord);

                //не продолжаем
                return;
            }

            //создаем экземпляр ajax
            const ajax = new Ajax("confirm", this.cookies);

            //отправляем запрос
            const response = await ajax.sendRequest(requestData);

            //если ответ успешный
            if (response === "ok") {

                //редиректим на страницу кабинета
                document.location.href = "/cabinet";

                //не продолжаем
                return;
            }

            // Refresh the page if the browser holds stale protection cookies.
            document.location.reload();

        } finally {

            //открываем кнопку
            loader.setEnable();
        }
    }
}
