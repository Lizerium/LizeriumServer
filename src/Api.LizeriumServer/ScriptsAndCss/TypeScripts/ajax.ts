/**
 * Класс Ajax запросов
 */
class Ajax {

    /**
     * URL запроса
     */
    private readonly requestUrl: string;

    /**
     * Экземпляр класса работы с куками
     */
    private readonly cookies: Cookies;

    /**
     * Переменная xhttp запросов
     */
    private readonly xhttp: XMLHttpRequest;

    /**
     * Конструктор
     * @param path - Путь запроса
     * @param cookies - экземпляр класса работы с куками
     */
    constructor(path: string, cookies: Cookies) {

        //присваиваем URL запроса
        this.requestUrl = `/ajax/${path}`;

        //присваиваем экземпляр класса работы с куками
        this.cookies = cookies;

        //инициализируем xhttp
        this.xhttp = new XMLHttpRequest();
    }

    /**
     * Метод отправляет ajax запрос
     * @param dataObject - Объект данных запроса
     */
    async sendRequest(dataObject: object = null): Promise<string> {

        //создаем новый промис
        return new Promise<string>(resolve => {

            try {

                //получаем защитный токен
                const csrfToken = this.cookies.getCookie("CSRF-TOKEN");

                //добавляем параметры асинхронного запроса
                this.xhttp.open("POST", this.requestUrl, true);

                //получение ответа на запрос
                this.xhttp.onreadystatechange = () => {

                    //проверяем готовность ответа
                    if (this.xhttp.readyState !== 4) return;

                    //проверяем успешен или нет ответ
                    if (this.xhttp.status !== 200) {

                        //вызываем промис с пустой строкой
                        resolve("");

                        //не продолжаем
                        return;
                    }

                    //вызываем промис с текстом ответа
                    resolve(this.xhttp.responseText);
                };

                //ошибки запроса
                this.xhttp.onerror = () => {

                    //выводим ошибку на консоль
                    console.error("ERROR: ", "Bad response");

                    //вызываем промис с пустой строкой
                    resolve("");
                };

                //подставляем в заголовки защитный токен
                this.xhttp.setRequestHeader("X-CSRF-TOKEN", csrfToken);

                //отправляем запрос в зависимости задан объект данных или нет
                if (dataObject == null) {

                    //запрос без данных
                    this.xhttp.send();

                } else {

                    //ставим заголовок что это json данные
                    this.xhttp.setRequestHeader("Content-type", "application/json");

                    //запрос с данными
                    this.xhttp.send(JSON.stringify(dataObject));
                }

            } catch (e) {

                //выводим ошибку на консоль
                console.error("ERROR: ", e);

                //вызываем промис с пустой строкой
                resolve("");
            }
        });
    }

    /**
     * Метод загружает файл на сервер
     * @param inputFile - input file
     * @return - url новой аватарки
     */
    async uploadFile(inputFile: HTMLInputElement, idChatbot: number = 0): Promise<string> {

        //проверяем входящие данные
        if (inputFile == null || inputFile.files.length < 1) return "";

        //создаем новый промис
        // ReSharper disable once TsNotResolved
        return new Promise<string>(resolve => {

            try {

                //получаем защитный токен
                const csrfToken = this.cookies.getCookie("CSRF-TOKEN");

                //создаем экземпляр FormData
                const formData = new FormData();

                //подставляем в параметры загружаемый файл
                formData.append("file", inputFile.files[0]);

                //если есть идентификатор чат бота
                if (idChatbot > 0) {

                    //добавляем его в параметры
                    formData.append("idChatbot", `${idChatbot}`);
                }

                //добавляем параметры асинхронного запроса
                this.xhttp.open("POST", this.requestUrl, true);

                //получение ответа на запрос
                this.xhttp.onreadystatechange = () => {

                    //проверяем готовность ответа
                    if (this.xhttp.readyState !== 4) return;

                    //проверяем успешен или нет ответ
                    if (this.xhttp.status !== 200) {

                        //вызываем промис с пустой строкой
                        resolve("");

                        //не продолжаем
                        return;
                    }

                    //вызываем промис с текстом ответа
                    resolve(this.xhttp.responseText);
                };

                //ошибки запроса
                this.xhttp.onerror = () => {

                    //выводим ошибку на консоль
                    console.error("ERROR: ", "Bad response");

                    //вызываем промис с пустой строкой
                    resolve("");
                };

                //подставляем в заголовки защитный токен
                this.xhttp.setRequestHeader("X-CSRF-TOKEN", csrfToken);

                //отправляем запрос с файлом
                this.xhttp.send(formData);

            } catch (e) {

                //выводим ошибку на консоль
                console.error("ERROR: ", e);

                //вызываем промис с пустой строкой
                resolve("");
            }
        });
    }
}