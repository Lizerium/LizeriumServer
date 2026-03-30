class Loader {
    constructor(button, type = "spinner") {
        this.button = button;
        this.type = type;
        this.prevHtml = "";
        if (type === "spinner")
            return;
        this.loader = new Image();
        this.loader.src = "/img/loader.gif";
        this.loader.classList.add("loader");
        this.loader.alt = "loader";
    }
    setDisable() {
        this.button.setAttribute("disabled", "disabled");
        this.prevHtml = this.button.innerHTML;
        switch (this.type) {
            case "spinner":
                this.button.innerHTML = "";
                const spinner = document.createElement("div");
                spinner.classList.add("spinner");
                spinner.innerHTML = `<div></div><div></div><div></div>`;
                this.button.appendChild(spinner);
                break;
            case "loader":
                this.button.innerHTML = this.loader.outerHTML;
                break;
        }
    }
    setEnable() {
        this.button.removeAttribute("disabled");
        this.button.innerHTML = this.prevHtml;
    }
}
