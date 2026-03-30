import { Ajax } from "../Shared/ajax";
import { Cookies } from "../Shared/cookies";
import { ModalForm } from "../Shared/modal_form";
import { LocalizationStrings, Utilities } from "../Shared/utilities";

/**
 * Класс авторизации
 */
export class Wish {
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
     * Input с крайним полученным идентификатором пользователя
     */
    private inputLastUserId: HTMLInputElement;

    /**
     * Select с выбранным списком сообщений по статусу отсортированным
     */
    private statusSelect: HTMLSelectElement;

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
        this.statusSelect = document.querySelector("#status") as unknown as HTMLSelectElement;
    }

    /**
     * Метод запускает авторизацию
     */
    async startApp(): Promise<void> {
        this.truncateLength = 300;
        //создаем экземпляр Ajax
        const ajax = new Ajax(`getAllLocalizedStrings/Views.Home.Wish`, this.cookies);
        //отправляем запрос
        const response = await ajax.sendRequest();
        //фиксируем переводы
        this.localizationStrings = JSON.parse(response);

        await this.loadTablePosts();

        //получаем ссылку кнопки создания поста
        const createPost = document.getElementById("create-post-btn") as unknown as HTMLLinkElement;
        //console.log("pre add create Post btn");
        //проверяем ссылку выхода
        if (createPost) {
            //console.log("add create Post btn");
            //привязываем метод на клик по ссылке выхода
            createPost.addEventListener("click", async () => {

                //создаем форму подтверждения
                const confirmForm = new ModalForm(this.localizationStrings["Wish_Modal_Title"], "column");

                //открываем форму подтверждения
                const resultConfirm = await confirmForm.showConfirmForm("", "", this.localizationStrings["Wish_Modal_Cancel"]);

                //если нет, не продолжаем
                if (!resultConfirm) return;

                //отправляем на выход
                document.location.href = "/create";
            });
        }

        //добавляем событие на скролл окна
        window.addEventListener("scroll", async () => await this.handleScrollWindow());
        this.statusSelect.addEventListener("change", async () => {
            await this.loadTablePosts()
        });
    }

    private async loadTablePosts(): Promise<void> {
        //получаем input с крайним полученным идентификатором пользователя
        this.inputLastUserId = document.getElementById("last_user_id") as HTMLInputElement;

        //создаем экземпляр Ajax
        const ajax = new Ajax(`getPosts/0/${this.statusSelect.value}/0`, this.cookies);
        //отправляем запрос
        const response = await ajax.sendRequest();
        //console.log(response);
        //проверяем ответ
        if (this.utilities.isEmpty(response)) return;

        //десериализуем из JSON
        const dataResponse = JSON.parse(response);

        const blockPosts = document.querySelector(".block.posts");
        if(blockPosts) {
            // Очищаем содержимое элемента
            blockPosts.innerHTML = '';

            // Или удаляем все дочерние элементы:
            while (blockPosts.firstChild) {
                blockPosts.removeChild(blockPosts.firstChild);
            }
        }

        //console.log("dataResponse.lastUserId.toString() " + dataResponse.lastUserId.toString());
        //подставляем крайний идентификатор пользователя
        this.inputLastUserId.value = dataResponse.lastUserId.toString();

        this.appendUsers(dataResponse.posts);
    }

    private async startReadMoreLogic(): Promise<void> {
        await this.utilities.startLoader();

        const posts = document.querySelectorAll(".post-content") as NodeListOf<HTMLDivElement>;
        //обходим кнопки
        for (let i = 0; i < posts.length; i++) {
            //Полное сообщение
            const fullContent = posts[i].querySelector('.full-content') as HTMLSpanElement;
            //Кнопка открыть остальное
            const readMoreButton = posts[i].querySelector('.read-more') as unknown as HTMLLinkElement;
            if (fullContent.textContent.length < this.truncateLength) {
                readMoreButton.style.visibility = "hidden";
            }// кнопка раскрытия и закрытия блока сообщения
            else posts[i].addEventListener('click', () => this.toggleContent(posts[i]));
        }

        await this.utilities.stopLoader();
    }


    private truncateText(text, length): string {
        if (text.length > length) {
            return text.substring(0, length) + '...';
        } else {
            return text;
        }
    }

    private toggleContent(postelement: HTMLDivElement): void {
        //Обрезанное сообщение
        const truncatedText = postelement.querySelector('.truncated') as HTMLSpanElement;
        //Полное сообщение
        const fullContent = postelement.querySelector('.full-content') as HTMLSpanElement;
        //Кнопка открыть остальное
        const readMoreButton = postelement.querySelector('.read-more') as unknown as HTMLLinkElement;
        const fullText = fullContent.textContent; // Получаем полный текст

        truncatedText.textContent = this.truncateText(fullText, this.truncateLength);

        fullContent.style.display = fullContent.style.display === 'none' ? 'inline' : 'none';
        truncatedText.style.display = fullContent.style.display === 'none' ? 'inline' : 'none';
        readMoreButton.textContent = readMoreButton.textContent === this.localizationStrings["Wish_Read_More"] ? this.localizationStrings["Wish_Read_Default"] : this.localizationStrings["Wish_Read_More"];
        //  Устанавливаем  высоту  post-content  после  показа/скрытия  текста
        postelement.style.height = fullContent.style.display === 'none' ? `${postelement.offsetHeight}px` : 'auto';
    }

   /**
   * Метод обрабатываем скролл окна
   */
    private async handleScrollWindow(): Promise<void> {
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

        //если да конца страницы более 1500 или AJAX в процессе, не продолжаем
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

        //console.log("dataResponse.lastUserId.toString() " + dataResponse.lastUserId.toString());
        //подставляем крайний идентификатор пользователя
        this.inputLastUserId.value = dataResponse.lastUserId.toString();

        //подставляем посты пользователей
        this.appendUsers(dataResponse.posts);
    }

    /**
     * Метод подставляет посты пользователей
     * @param posts - массив постов
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

            //обходим массив постов
            for (let i = 0; i < posts.length; i++) {
                if (parseInt(posts[i].Status) < 0) continue;

                /*
                + <div class="status-job">
                +     <label>Admin</label>*/
                //создаем строку таблицы
                const div = document.createElement("div");

                switch (posts[i].Status) {
                    case 1:
                        div.classList.add("status-new")
                        break;
                    case 2:
                        div.classList.add("status-read")
                        break;
                    case 3:
                        div.classList.add("status-job")
                        break;
                    case 4:
                        div.classList.add("status-delete")
                        break;
                    case 5:
                        div.classList.add("status-complete")
                        break;
                }

                //autor <label>Admin</label>
                const AutorLabel = document.createElement("p");
                AutorLabel.innerText = posts[i].Autor;

                /*
                     <div class="post-content">
                        <span class="truncated">«Побе́да» (юридическое название — ООО «Авиакомпания Победа») — российская бюджетная авиакомпания, дочернее предприятие группы «Аэрофлот — Российские авиалинии», базируется в московском аэропорту Внуково[2]. Основана 16 сентября 2014 года[3] взамен прекратившего полёты из-за санкций Европейского союз...</span>
                        <span class="full-content" style="display: none;">
                            <label>«Побе́да» (юридическое название — ООО «Авиакомпания Победа») — российская бюджетная авиакомпания, дочернее предприятие группы «Аэрофлот — Российские авиалинии», базируется в московском аэропорту Внуково[2]. Основана 16 сентября 2014 года[3] взамен прекратившего полёты из-за санкций Европейского союза «Добролёта». Суточный налёт самолётов авиакомпании в 2017 году был самым высоким в мире — 15—16 часов, что выше, чем у крупнейших мировых лоукостеров — американской Southwest и ирландской Ryanair[4].  Российская авиакомпания «Побе́да» вызывала у пассажиров больше негатива, чем одобрения; стала предметом критики среди пассажиров и СМИ[5][6][7][8][9]. Была фигурантом судебных дел. Верховный суд Российской Федерации признал незаконными габариты ручной клади авиакомпании «Победа». Установленные авиакомпанией «Победа» габариты для ручной клади нарушали права пассажиров[10][11].  Авиакомпания получила от европейских лизингодателей уведомления о расторжении договоров и необходимости вернуть лайнеры[12], которое не было исполнено. 11 апреля 2022 года авиакомпания внесена в чёрный список Евросоюза из-за того, что её самолёты не соответствуют «международным стандартам безопасности» (после запрета на продажу запчастей)[13].</label>
                        </span>
                        <a class="read-more">Читать далее</a>
                    </div>
                */
                const PostContentDiv = document.createElement("div");
                PostContentDiv.classList.add("post-content");
                const spanTruncated = document.createElement("span");
                spanTruncated.classList.add("truncated");
                spanTruncated.innerText = posts[i].MessageMini;
                const spanFullContent = document.createElement("span");
                spanFullContent.classList.add("full-content");
                spanFullContent.style.display = "none";
                const labelSpanFullContent = document.createElement("p");
                labelSpanFullContent.innerText = posts[i].Message;
                const AReadMore = document.createElement("a");
                AReadMore.classList.add("read-more");
                AReadMore.innerText = this.localizationStrings["Wish_Read_More"];
                PostContentDiv.appendChild(spanTruncated);
                PostContentDiv.appendChild(spanFullContent);
                spanFullContent.appendChild(labelSpanFullContent);
                if (posts[i].Message.length < this.truncateLength) {
                    AReadMore.style.visibility = "hidden";
                }// кнопка раскрытия и закрытия блока сообщения
                else {
                    AReadMore.addEventListener('click', () => this.toggleContent(PostContentDiv));
                    PostContentDiv.appendChild(AReadMore);
                }

                div.appendChild(AutorLabel);
                div.appendChild(PostContentDiv);

                switch (posts[i].Status) {
                    /*
                        <div class="loader status-new  hidden">
                            <span>Н</span>
                            <span>o</span>
                            <span>в</span>
                            <span>о</span>
                            <span>е</span>
                        </div>
                    */
                    case 1:
                        const labelStatusNew = document.createElement("p");
                        labelStatusNew.classList.add("status-new");

                        const divStatusNew = document.createElement("div");
                        divStatusNew.classList.add("loader");
                        divStatusNew.classList.add("status-new");

                        const statusNewSpans = this.createSpansFromText(this.localizationStrings["Wish_Status_1"]);
                        statusNewSpans.forEach(span => divStatusNew.appendChild(span));

                        labelStatusNew.appendChild(divStatusNew);
                        div.appendChild(labelStatusNew);
                        break;
                    /*
                        <div class="loader status-read  hidden">
                            <span>П</span>
                            <span>р</span>
                            <span>о</span>
                            <span>ч</span>
                            <span>и</span>
                            <span>т</span>
                            <span>а</span>
                            <span>н</span>
                            <span>о</span>
                        </div>
                    */
                    case 2:
                        const labelStatusRead = document.createElement("p");
                        labelStatusRead.classList.add("status-read");

                        const divStatusRead = document.createElement("div");
                        divStatusRead.classList.add("loader");
                        divStatusRead.classList.add("status-read");

                        const statusReadSpans = this.createSpansFromText(this.localizationStrings["Wish_Status_2"]);
                        statusReadSpans.forEach(span => divStatusRead.appendChild(span));

                        labelStatusRead.appendChild(divStatusRead);

                        div.appendChild(labelStatusRead);
                        break;
                     /*
                     <div class="loader status-job  ">
                            <span>В</span>
                            <span> </span>
                            <span>р</span>
                            <span>а</span>
                            <span>б</span>
                            <span>о</span>
                            <span>т</span>
                            <span>е</span>
                        </div>
                     */
                    case 3:
                        const labelStatusJob = document.createElement("p");
                        labelStatusJob.classList.add("status-job");

                        const divStatusJob = document.createElement("div");
                        divStatusJob.classList.add("loader");
                        divStatusJob.classList.add("status-job");

                        const statusJobSpans = this.createSpansFromText(this.localizationStrings["Wish_Status_3"]);
                        statusJobSpans.forEach(span => divStatusJob.appendChild(span));
                      
                        labelStatusJob.appendChild(divStatusJob);
                        div.appendChild(labelStatusJob);
                        break;
                    /*
                        <div class="loader status-delete  hidden">
                            <span>О</span>
                            <span>т</span>
                            <span>к</span>
                            <span>а</span>
                            <span>з</span>
                            <span>а</span>
                            <span>н</span>
                            <span>о</span>
                        </div>
                    */
                    case 4:
                        const labelStatusDelete = document.createElement("p");
                        labelStatusDelete.classList.add("status-delete");
                        const divStatusDelete = document.createElement("div");
                        divStatusDelete.classList.add("loader");
                        divStatusDelete.classList.add("status-delete");

                        const statusDeleteSpans = this.createSpansFromText(this.localizationStrings["Wish_Status_4"]);
                        statusDeleteSpans.forEach(span => divStatusDelete.appendChild(span));

                        labelStatusDelete.appendChild(divStatusDelete);
                        div.appendChild(labelStatusDelete);
                        break;
                    /*
                        <div class="loader status-complete  hidden">
                            <span>В</span>
                            <span>ы</span>
                            <span>п</span>
                            <span>о</span>
                            <span>л</span>
                            <span>н</span>
                            <span>е</span>
                            <span>н</span>
                            <span>о</span>
                        </div>
                    */
                    case 5:
                        const labelStatusComplete = document.createElement("div");
                        labelStatusComplete.classList.add("status-complete");
                        const divStatusComplete = document.createElement("div");
                        divStatusComplete.classList.add("loader");
                        divStatusComplete.classList.add("status-complete");

                        const statusCompleteSpans = this.createSpansFromText(this.localizationStrings["Wish_Status_5"]);
                        statusCompleteSpans.forEach(span => divStatusComplete.appendChild(span));

                        labelStatusComplete.style.textAlign = "center";
                        labelStatusComplete.appendChild(divStatusComplete);
                        div.appendChild(labelStatusComplete);
                        break;
                }

                //подставляем во фрагмент строку таблицы
                fragment.appendChild(div);
            }

            //подставляем в таблицу пользователей
            document.querySelector(".block.posts").appendChild(fragment);
        } finally {

            //открываем получение через Ajax
            this.ajaxInProgress = false;
        }
    }


    private createSpansFromText(text: string): HTMLSpanElement[] {
        const spans: HTMLSpanElement[] = [];
        for (const char of text) {
            const span = document.createElement("span");
            span.innerText = char;
            spans.push(span);
        }
        return spans;
    }
}