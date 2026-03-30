import { Cookies } from "../Shared/cookies";
import { Utilities } from "../Shared/utilities";

export class Index {
    /**
     * Экземпляр класса утилит
     */
    private readonly utilities: Utilities;

    /**
     * Экземпляр класса работы с куками
     */
    private readonly cookies: Cookies;

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
    }

    public async startLoadModdbImage(): Promise<void> {
        const moddbImg = document.getElementById("moddb-img") as HTMLImageElement;
        if (moddbImg && moddbImg.dataset.src) {
            moddbImg.src = moddbImg.dataset.src;
            //console.log("start load moddb stats");
        }
    }
}