import { Cookies } from "../Shared/cookies";
import { Utilities } from "../Shared/utilities";

/**
 * Класс авторизации
 */
export class Launcher {
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

    /**
    * Метод запускает авторизацию
    */
    async start(): Promise<void> {
        this.bindNewsCarousels();
        this.bindNewsLightbox();
        this.bindLazyNewsCards();
        this.bindNewsReader();
    }

    private bindNewsCarousels(): void {
        const carousels = document.querySelectorAll<HTMLElement>("[data-news-carousel]");

        carousels.forEach((carousel) => {
            const slides = Array.from(carousel.querySelectorAll<HTMLElement>(".launcher-news-image-button"));
            const currentCounter = carousel.querySelector<HTMLElement>("[data-news-carousel-current]");
            let currentIndex = Math.max(0, slides.findIndex((slide) => slide.classList.contains("active")));

            const showSlide = (nextIndex: number): void => {
                if (slides.length === 0)
                    return;

                currentIndex = (nextIndex + slides.length) % slides.length;

                slides.forEach((slide, index) => {
                    slide.classList.toggle("active", index === currentIndex);
                });

                if (currentCounter)
                    currentCounter.textContent = String(currentIndex + 1);
            };

            carousel.querySelector("[data-news-carousel-previous]")?.addEventListener("click", (event) => {
                event.preventDefault();
                event.stopPropagation();
                showSlide(currentIndex - 1);
            });

            carousel.querySelector("[data-news-carousel-next]")?.addEventListener("click", (event) => {
                event.preventDefault();
                event.stopPropagation();
                showSlide(currentIndex + 1);
            });

            showSlide(currentIndex);
        });
    }

    private bindNewsLightbox(): void {
        document.querySelectorAll<HTMLElement>("[data-news-lightbox]").forEach((button) => {
            button.addEventListener("click", () => {
                const imageUrl = button.getAttribute("data-news-lightbox");
                if (!imageUrl)
                    return;

                this.openNewsLightbox(imageUrl, button.getAttribute("data-news-title") || "");
            });
        });
    }

    private openNewsLightbox(imageUrl: string, title: string): void {
        const previous = document.querySelector(".launcher-news-lightbox");
        if (previous)
            previous.remove();

        const lightbox = document.createElement("div");
        lightbox.className = "launcher-news-lightbox";
        lightbox.innerHTML = `
            <button class="launcher-news-lightbox-close" type="button" aria-label="Close">×</button>
            <img src="${this.escapeAttribute(imageUrl)}" alt="${this.escapeAttribute(title)}" />
        `;

        const close = (): void => lightbox.remove();
        lightbox.addEventListener("click", (event) => {
            if (event.target === lightbox)
                close();
        });

        lightbox.querySelector(".launcher-news-lightbox-close")?.addEventListener("click", close);

        document.body.appendChild(lightbox);
    }

    private bindLazyNewsCards(): void {
        const hydrateCard = (card: Element): void => {
            card.querySelectorAll<HTMLIFrameElement>("[data-news-card-video-src]").forEach((iframe) => {
                if (!iframe.src)
                    iframe.src = iframe.getAttribute("data-news-card-video-src") || "";
            });
        };

        const cards = Array.from(document.querySelectorAll<HTMLElement>(".launcher-news-card"));
        if (cards.length === 0)
            return;

        if (!("IntersectionObserver" in window)) {
            cards.slice(0, 4).forEach(hydrateCard);
            return;
        }

        const observer = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (!entry.isIntersecting)
                    return;

                hydrateCard(entry.target);
                observer.unobserve(entry.target);
            });
        }, { rootMargin: "600px 0px" });

        cards.forEach((card) => observer.observe(card));
    }

    private bindNewsReader(): void {
        const reader = document.querySelector<HTMLElement>("[data-news-reader]");
        const feed = document.querySelector<HTMLElement>("[data-news-reader-feed]");

        if (!reader || !feed)
            return;

        const posts = Array.from(reader.querySelectorAll<HTMLElement>("[data-news-reader-post]"));
        let activeIndex = 0;
        const visibleRadius = 1;
        let scrollSyncTimeout = 0;

        const hydratePost = (post: HTMLElement): void => {
            post.querySelectorAll<HTMLImageElement>("[data-news-reader-image-src]").forEach((image) => {
                if (!image.src)
                    image.src = image.getAttribute("data-news-reader-image-src") || "";
            });

            post.querySelectorAll<HTMLElement>("[data-news-video-player]").forEach((player) => {
                this.hydrateNewsVideoPlayer(player);
            });
        };

        const updateVisiblePosts = (): void => {
            posts.forEach((item, index) => {
                const isVisible = Math.abs(index - activeIndex) <= visibleRadius;
                if (!item.classList.contains("is-unloaded")) {
                    const height = Math.max(item.offsetHeight, 560);
                    item.style.setProperty("--news-post-height", `${height}px`);
                }

                item.classList.toggle("is-unloaded", !isVisible);
            });
        };

        const setActivePost = (post: HTMLElement): void => {
            const nextIndex = posts.indexOf(post);
            if (nextIndex < 0)
                return;

            activeIndex = nextIndex;
            posts.forEach((item, index) => item.classList.toggle("active", index === activeIndex));
            updateVisiblePosts();
            hydratePost(post);

            const previousPost = posts[activeIndex - 1];
            const nextPost = posts[activeIndex + 1];

            if (previousPost)
                hydratePost(previousPost);
            if (nextPost)
                hydratePost(nextPost);
        };

        const syncActivePostFromScroll = (): void => {
            if (!reader.classList.contains("open"))
                return;

            const feedRect = feed.getBoundingClientRect();
            const feedCenter = feedRect.top + feedRect.height / 2;
            let closestPost: HTMLElement | null = null;
            let closestDistance = Number.MAX_VALUE;

            posts.forEach((post) => {
                const rect = post.getBoundingClientRect();
                if (rect.bottom < feedRect.top || rect.top > feedRect.bottom)
                    return;

                const postCenter = rect.top + rect.height / 2;
                const distance = Math.abs(postCenter - feedCenter);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestPost = post;
                }
            });

            if (closestPost)
                setActivePost(closestPost);
        };

        const scrollToPost = (newsId: string, behavior: ScrollBehavior = "smooth"): void => {
            const post = posts.find((item) => item.getAttribute("data-news-reader-post") === newsId);
            if (!post)
                return;

            setActivePost(post);
            window.requestAnimationFrame(() => {
                feed.scrollTo({
                    top: Math.max(0, post.offsetTop - 22),
                    behavior
                });
            });
        };

        const open = (newsId: string): void => {
            reader.classList.add("open");
            reader.setAttribute("aria-hidden", "false");
            document.body.classList.add("news-reader-open");

            window.setTimeout(() => scrollToPost(newsId, "auto"), 30);
            window.setTimeout(syncActivePostFromScroll, 120);
        };

        const close = (): void => {
            reader.classList.remove("open");
            reader.setAttribute("aria-hidden", "true");
            document.body.classList.remove("news-reader-open");
        };

        document.querySelectorAll<HTMLElement>("[data-news-reader-open]").forEach((button) => {
            button.addEventListener("click", (event) => {
                event.preventDefault();
                event.stopPropagation();
                open(button.getAttribute("data-news-reader-open") || "");
            });
        });

        document.querySelectorAll<HTMLElement>(".launcher-news-card").forEach((card) => {
            card.addEventListener("click", (event) => {
                const target = event.target as HTMLElement | null;
                if (target?.closest("a, button, iframe"))
                    return;

                const opener = card.querySelector<HTMLElement>("[data-news-reader-open]");
                const newsId = opener?.getAttribute("data-news-reader-open");
                if (newsId)
                    open(newsId);
            });
        });

        document.querySelectorAll<HTMLElement>("[data-news-like]").forEach((button) => {
            const initialNewsId = button.getAttribute("data-news-like") || "";
            if (initialNewsId && localStorage.getItem(`lizerium-news-like-${initialNewsId}`) === "1")
                button.classList.add("liked");

            button.addEventListener("click", async () => {
                const newsId = button.getAttribute("data-news-like") || "";
                if (!newsId || button.classList.contains("liked"))
                    return;

                const storageKey = `lizerium-news-like-${newsId}`;
                if (localStorage.getItem(storageKey) === "1") {
                    button.classList.add("liked");
                    return;
                }

                button.classList.add("pending");

                try {
                    const response = await fetch(`/news/like/${encodeURIComponent(newsId)}`, {
                        method: "POST",
                        credentials: "same-origin",
                        headers: {
                            "Accept": "application/json",
                            "X-Requested-With": "XMLHttpRequest"
                        }
                    });

                    if (!response.ok)
                        throw new Error("Like failed");

                    const result = await response.json();
                    if (typeof result.likeCount === "number") {
                        document.querySelectorAll<HTMLElement>(`[data-news-like="${newsId}"]`).forEach((sameNewsButton) => {
                            const count = sameNewsButton.querySelector<HTMLElement>("[data-news-like-count]");
                            if (count)
                                count.textContent = String(result.likeCount);

                            sameNewsButton.classList.add("liked");
                        });
                    }

                    localStorage.setItem(storageKey, "1");
                    button.classList.add("liked");
                }
                catch {
                }
                finally {
                    button.classList.remove("pending");
                }
            });
        });

        reader.querySelectorAll("[data-news-reader-close]").forEach((button) => {
            button.addEventListener("click", close);
        });

        reader.querySelector("[data-news-reader-previous]")?.addEventListener("click", () => {
            activeIndex = Math.max(0, activeIndex - 1);
            scrollToPost(posts[activeIndex]?.getAttribute("data-news-reader-post") || "");
        });

        reader.querySelector("[data-news-reader-next]")?.addEventListener("click", () => {
            activeIndex = Math.min(posts.length - 1, activeIndex + 1);
            scrollToPost(posts[activeIndex]?.getAttribute("data-news-reader-post") || "");
        });

        if ("IntersectionObserver" in window) {
            const observer = new IntersectionObserver((entries) => {
                const visibleEntries = entries
                    .filter((entry) => entry.isIntersecting)
                    .sort((a, b) => b.intersectionRatio - a.intersectionRatio);

                const visiblePost = visibleEntries[0]?.target as HTMLElement | undefined;
                if (visiblePost)
                    setActivePost(visiblePost);
            }, {
                root: feed,
                threshold: [0.2, 0.35, 0.55, 0.75]
            });

            posts.forEach((post) => observer.observe(post));
        }

        feed.addEventListener("scroll", () => {
            window.clearTimeout(scrollSyncTimeout);
            scrollSyncTimeout = window.setTimeout(syncActivePostFromScroll, 80);
        }, { passive: true });

        reader.querySelectorAll<HTMLElement>("[data-news-reader-share]").forEach((button) => {
            button.addEventListener("click", async () => {
                const newsId = button.getAttribute("data-news-reader-share") || "";
                const url = `${window.location.origin}${window.location.pathname}#news-${newsId}`;
                const title = button.closest("[data-news-reader-post]")?.querySelector("h2")?.textContent || document.title;

                if (navigator.share) {
                    try {
                        await navigator.share({ title, url });
                    }
                    catch {
                    }
                    return;
                }

                await navigator.clipboard?.writeText(url);
                button.classList.add("copied");
                window.setTimeout(() => button.classList.remove("copied"), 1200);
            });
        });

        document.addEventListener("keydown", (event) => {
            if (event.key === "Escape" && reader.classList.contains("open"))
                close();
        });
    }

    private hydrateNewsVideoPlayer(player: HTMLElement): void {
        if (player.getAttribute("data-news-video-ready") === "true")
            return;

        const iframe = player.querySelector<HTMLIFrameElement>("[data-news-video-frame]");
        if (!iframe)
            return;

        const post = player.closest<HTMLElement>("[data-news-reader-post]");
        const buttons = Array.from(post?.querySelectorAll<HTMLButtonElement>("[data-news-video-src]") || []);
        const setPlatform = (button: HTMLButtonElement): void => {
            const src = button.getAttribute("data-news-video-src") || "";
            if (!src)
                return;

            buttons.forEach((item) => item.classList.toggle("active", item === button));
            iframe.setAttribute("data-news-reader-video-src", src);
            iframe.src = src;
            iframe.dataset.loaded = "false";

            window.setTimeout(() => {
                if (iframe.dataset.loaded === "true")
                    return;

                const currentIndex = buttons.indexOf(button);
                const next = buttons.find((item, index) => index > currentIndex && item.getAttribute("data-news-video-src"));
                if (next)
                    setPlatform(next);
            }, 3200);
        };

        iframe.addEventListener("load", () => {
            iframe.dataset.loaded = "true";
        });

        buttons.forEach((button) => {
            button.addEventListener("click", (event) => {
                event.preventDefault();
                setPlatform(button);
            });
        });

        const activeButton = buttons.find((button) => button.classList.contains("active"));
        if (activeButton)
            setPlatform(activeButton);
        else if (!iframe.src)
            iframe.src = iframe.getAttribute("data-news-reader-video-src") || "";

        player.setAttribute("data-news-video-ready", "true");
    }

    private escapeAttribute(value: string): string {
        return value
            .replace(/&/g, "&amp;")
            .replace(/"/g, "&quot;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;");
    }
}
