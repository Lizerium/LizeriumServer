/**
 * Класс управления постами
 */

class Posts {

    /**
     * Экземпляр класса утилит
     */
    private readonly utilities: Utilities;

    /**
     * Экземпляр класса кук
     */
    private readonly cookies: Cookies;

    /**
     * Input с крайним полученным идентификатором пользователя
     */
    private inputLastUserId: HTMLInputElement;

    /**
     * Select с выбранным списком сообщений по статусу отсортированным
     */
    private readonly statusSelect: HTMLSelectElement;

    /**
     * Флаг что AJAX в процессе
     */
    private ajaxInProgress: boolean;

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

        //получаем input с крайним полученным идентификатором пользователя
        this.inputLastUserId = document.getElementById("last_user_id") as HTMLInputElement;

        this.statusSelect = document.querySelector("#status") as HTMLSelectElement;

        //ставим что ajax не в процессе
        this.ajaxInProgress = false;

        this.statusSelect = document.querySelector("#status") as HTMLSelectElement;
    }

    /**
     * Метод запускает управление пользователями
     */
    startPosts(): void {
        //получаем все кнопки сохранения
        const allSelects = document.querySelectorAll("#status") as NodeListOf<HTMLSelectElement>;

        //обходим все боксы с выбором статуса
        for (let i = 0; i < allSelects.length; i++) {
            // привязываем метод на select
            allSelects[i].addEventListener('change', async () => await this.updateSelectStatusAsync(allSelects[i]));
        }

        //добавляем событие на скролл окна
        window.addEventListener("scroll", async () => await this.handleScrollWindow());

        this.statusSelect.addEventListener("change", async () => {
            await this.loadTablePosts()
        });
    }

    /**
      * Метод сохраняет коэффициенты сетки
      * @param button - кнопка
      */
    private async updateSelectStatusAsync(button: HTMLSelectElement): Promise<void>
    {
        const idUser = parseInt(button.getAttribute("user"));

        // Получаем значение выбранного статуса
        const selectedStatusId: number = parseInt(button.value);

        // Выводим в консоль выбранный статус для проверки
        console.log('Измененный статус:', idUser);

        const ajax = new Ajax(`updateStatusPost/${idUser}/${selectedStatusId}`, this.cookies);

        //отправляем запрос
        const response = await ajax.sendRequest();

        console.log(response);
    }

    /**
     * Метод обрабатываем скролл окна
     */
    private async handleScrollWindow(): Promise<void>
    {
        //получаем input с крайним полученным идентификатором пользователя
        this.inputLastUserId = document.getElementById("last_user_id") as HTMLInputElement;

        //получаем величину прокрутки окна
        const scrollTop = window.pageYOffset || document.documentElement.scrollTop;

        //получаем высоту документа
        const scrollHeight = Math.max(
            document.body.scrollHeight, document.documentElement.scrollHeight,
            document.body.offsetHeight, document.documentElement.offsetHeight,
            document.body.clientHeight, document.documentElement.clientHeight
        );

        //если да конца страницы более 1500 или AJAX  в процессе, не продолжаем
        if (scrollHeight - scrollTop > 1500 || this.ajaxInProgress) return;

        //ставим что ajax в процессе
        this.ajaxInProgress = true;

        const statusValue = parseInt(this.statusSelect.value);
        //создаем экземпляр Ajax
        const ajax = new Ajax(`getPosts/${this.inputLastUserId.value}/${statusValue}/1`, this.cookies);

        //отправляем запрос
        const response = await ajax.sendRequest();

        //проверяем ответ
        if (this.utilities.isEmpty(response)) return;

        //десериализуем из JSON
        const dataResponse = JSON.parse(response);

        //подставляем крайний идентификатор пользователя
        this.inputLastUserId.value = dataResponse.lastUserId.toString();

        //подставляем посты пользователей
        this.appendUsers(dataResponse.posts);
    }

    private async loadTablePosts(): Promise<void> {
        //получаем input с крайним полученным идентификатором пользователя
        this.inputLastUserId = document.getElementById("last_user_id") as HTMLInputElement;

        //создаем экземпляр Ajax
        const ajax = new Ajax(`getPosts/0/${this.statusSelect.value}/0`, this.cookies);
        //отправляем запрос
        const response = await ajax.sendRequest();
        console.log(response);
        //проверяем ответ
        if (this.utilities.isEmpty(response)) return;

        //десериализуем из JSON
        const dataResponse = JSON.parse(response);

        const blockPosts = document.querySelector("tbody");
        if (blockPosts) {
            // Очищаем содержимое элемента
            blockPosts.innerHTML = '';

            // Или удаляем все дочерние элементы:
            while (blockPosts.firstChild) {
                blockPosts.removeChild(blockPosts.firstChild);
            }
        }

        //подставляем крайний идентификатор пользователя
        this.inputLastUserId.value = dataResponse.lastUserId.toString();

        this.appendUsers(dataResponse.posts);
    }

    /**
     * Метод подставляет посты пользователей
     * @param users - массив пользователей
     */
    private appendUsers(posts: any): void {

        //если в массиве нет данных не продолжаем
        if (posts.length < 1) {

            //закрываем получение через Ajax
            this.ajaxInProgress = true;

            //не продолжаем
            return;
        }

        try {

            //создаем фрагмент
            const fragment = document.createDocumentFragment();

            //обходим массив пользователей
            for (let i = 0; i < posts.length; i++)
            {
                //создаем строку таблицы
                const tr = document.createElement("tr");

                //создаем столбец с идентификатором пользователя
                const tdIdUser = document.createElement("td");
                tdIdUser.innerText = posts[i].Id;

                //создаем столбец с данными пользователя
                const tdUserData = document.createElement("td");
                const div1 = document.createElement("div");
                div1.classList.add("user_in_table");
                const divImg = document.createElement("div");
                const img = document.createElement("img");
                img.src = "img/my-account-icon.png";
                img.alt = posts[i].Autor;
                const divSpan = document.createElement("div");
                const span = document.createElement("span");
                span.innerText = posts[i].Autor;

                divSpan.appendChild(span);
                divImg.appendChild(img);
                div1.appendChild(divImg);
                div1.appendChild(divSpan);
                tdUserData.appendChild(div1);

                //создаем столбец с Message пользователя
                const tdMsgUser = document.createElement("td");
                tdMsgUser.innerHTML = posts[i].Message;

                //создаем столбец с данными о регистрации/авторизации
                const tdRegAuth = document.createElement("td");
                tdRegAuth.innerHTML = `<div class="times"><span>Регистрация</span><span>${posts[i].DateTimeUnixString}</span></div>`;

                const selectTd = document.createElement("td");
                const select = document.createElement("select");
                select.setAttribute("user", posts[i].Id);
                select.id = `status`; //  Уникальный  id  для  select

                //  Добавление  option  в  select
                const options = [
                    { value: "-1", text: "Обработка" },
                    { value: "1", text: "Новое" },
                    { value: "2", text: "Прочитано" },
                    { value: "3", text: "В работе" },
                    { value: "4", text: "Отказано" },
                    { value: "5", text: "Выполнено" }
                ];

                for (const option of options) {
                    const optionElement = document.createElement("option");
                    optionElement.value = option.value;
                    optionElement.text = option.text;
                    select.appendChild(optionElement);
                }

                //  Установите  начальное  значение  select
                select.value = posts[i].Status.toString();
                selectTd.appendChild(select);
                select.addEventListener('change', async () => await this.updateSelectStatusAsync(select));

                /*
                   <td>
                        <select id="status" asp-for="@postData.Status" user="@postData.Id">
                            <option value="-1">Обработка</option>
                            <option value="1">Новое</option>
                            <option value="2">Прочитано</option>
                            <option value="3">В работе</option>
                            <option value="4">Отказано</option>
                            <option value="5">Выполнено</option>
                        </select>
                    </td>
                */

                //подставляем столбцы в строку
                tr.appendChild(tdIdUser);
                tr.appendChild(tdUserData);
                tr.appendChild(tdRegAuth);
                tr.appendChild(tdMsgUser);
                tr.appendChild(selectTd);

                //подставляем во фрагмент строку таблицы
                fragment.appendChild(tr);
            }

            //подставляем в таблицу пользователей
            document.querySelector("table > tbody").appendChild(fragment);

        } finally {

            //открываем получение через Ajax
            this.ajaxInProgress = false;
        }
    }
}