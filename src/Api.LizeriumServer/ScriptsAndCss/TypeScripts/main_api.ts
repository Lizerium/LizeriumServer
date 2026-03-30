(() => {
    //по загрузке окна
    window.addEventListener("load", async () => {

        //создаем экземпляр класса утилит
        const utilities = new Utilities();

        //создаем экземпляр класса кук
        const cookies = new Cookies();

        //получаем текущий URL
        const currentUrl = new URL(document.location.href);

        //получаем путь из URL
        const pathname = currentUrl.pathname.toLowerCase();

        //разбиваем пути URL на части
        const partsPath = pathname.split("/");

        //смотрим путь
        switch (partsPath[1]) {
            case "": //страница авторизации
                {
                    //создаем экземпляр класса авторизации
                    const auth = new Auth(utilities, cookies);

                    //запускаем авторизацию
                    auth.startAuth();
                }
                break;
            case "confirmation": //страница подтверждения
                {
                    //создаем экземпляр класса авторизации
                    const auth = new Auth(utilities, cookies);

                    //запускаем подтверждение
                    auth.startConfirm();
                }
                break;
            case "posts":
                {
                    //создаем экземпляр класса управления пользователями
                    const posts = new Posts(utilities, cookies);

                    //запускаем управление пользователями
                    await posts.startPosts();
                }
                break;
            case "commands":
                {
                    //создаем экземпляр класса управления пользователями
                    const commands = new Commands(utilities, cookies);

                    //запускаем управление пользователями
                    await commands.startCommands();
                }
                break;
            default:
                break;
        }

        const buttonScroll = document.getElementById("scrollButton") as HTMLButtonElement;
        //проверяем ссылку выхода
        if (buttonScroll) {
            buttonScroll.addEventListener("click", () => {
                const scrollPosition = window.scrollY;
                const pageHeight = document.documentElement.scrollHeight;
                const windowHeight = window.innerHeight;

                // Если пользователь ниже середины страницы — скроллим вверх
                if (scrollPosition > (pageHeight - windowHeight) / 2) {
                    window.scrollTo({ top: 0, behavior: "smooth" });
                } else {
                    // Иначе — вниз
                    window.scrollTo({ top: pageHeight, behavior: "smooth" });
                }
            });
        }

        //получаем ссылку выхода
        const logout = document.getElementById("logout") as unknown as HTMLLinkElement;

        //проверяем ссылку выхода
        if (!logout) return;

        //привязываем метод на клик по ссылке выхода
        logout.addEventListener("click", async () => {

            //создаем форму подтверждения
            const confirmForm = new ModalForm("Выход из админки", "column");

            //открываем форму подтверждения
            const resultConfirm = await confirmForm.showConfirmForm("Вы уверены что хотите выйти?", "Да", "Нет");

            //если нет, не продолжаем
            if (!resultConfirm) return;

            //отправляем на выход
            document.location.href = "/logout";
        });
    });

})();
