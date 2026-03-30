/**
 * Класс работы с куками
 */
class Cookies {

    /**
     * Метод получает значение куки по имени
     * @param cookieName - название куки
     * @return - значение куки
     */
    getCookie(cookieName: string): string {

        const name: any = cookieName + "=";

        const decodedCookie = decodeURIComponent(document.cookie);

        const ca = decodedCookie.split(";");

        for (let i = 0; i < ca.length; i++) {

            let c = ca[i];

            while (c.charAt(0) === " ") {

                c = c.substring(1);
            }

            if (c.indexOf(name) === 0) {

                return c.substring(name.length, c.length);
            }
        }

        return "";
    }

    /**
     * Метод устанавливает куку
     * @param name - имя куки
     * @param value - значение куки
     */
    setCookie(name: string, value: string): void {

        //создаем переменную даты
        const date = new Date();

        //устанавливаем дату плюс месяц
        date.setTime(date.getTime() + 30 * 24 * 60 * 60 * 1000);

        //устанавливаем куку
        document.cookie = name + "=" + encodeURIComponent(value) + ";expires=" + date.toUTCString() + ";path=/;secure";
    }

    /**
     * Метод удаляет куку по имени
     * @param name - имя куки
     */
    removeCookie(name: string): void {
        document.cookie = name + "=;Max-Age=-99999999;";
    }
}