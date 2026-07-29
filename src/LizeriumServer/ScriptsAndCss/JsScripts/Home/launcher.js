var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
export class Launcher {
    constructor(utilities, cookies) {
        this.utilities = utilities;
        this.cookies = cookies;
    }
    start() {
        return __awaiter(this, void 0, void 0, function* () {
            this.bindNewsCarousels();
            this.bindNewsLightbox();
            this.bindLazyNewsCards();
            this.bindNewsReader();
        });
    }
    bindNewsCarousels() {
        const carousels = document.querySelectorAll("[data-news-carousel]");
        carousels.forEach((carousel) => {
            var _a, _b;
            const slides = Array.from(carousel.querySelectorAll(".launcher-news-image-button"));
            const currentCounter = carousel.querySelector("[data-news-carousel-current]");
            let currentIndex = Math.max(0, slides.findIndex((slide) => slide.classList.contains("active")));
            const showSlide = (nextIndex) => {
                if (slides.length === 0)
                    return;
                currentIndex = (nextIndex + slides.length) % slides.length;
                slides.forEach((slide, index) => {
                    slide.classList.toggle("active", index === currentIndex);
                });
                if (currentCounter)
                    currentCounter.textContent = String(currentIndex + 1);
            };
            (_a = carousel.querySelector("[data-news-carousel-previous]")) === null || _a === void 0 ? void 0 : _a.addEventListener("click", (event) => {
                event.preventDefault();
                event.stopPropagation();
                showSlide(currentIndex - 1);
            });
            (_b = carousel.querySelector("[data-news-carousel-next]")) === null || _b === void 0 ? void 0 : _b.addEventListener("click", (event) => {
                event.preventDefault();
                event.stopPropagation();
                showSlide(currentIndex + 1);
            });
            showSlide(currentIndex);
        });
    }
    bindNewsLightbox() {
        document.querySelectorAll("[data-news-lightbox]").forEach((button) => {
            button.addEventListener("click", () => {
                const imageUrl = button.getAttribute("data-news-lightbox");
                if (!imageUrl)
                    return;
                this.openNewsLightbox(imageUrl, button.getAttribute("data-news-title") || "");
            });
        });
    }
    openNewsLightbox(imageUrl, title) {
        var _a;
        const previous = document.querySelector(".launcher-news-lightbox");
        if (previous)
            previous.remove();
        const lightbox = document.createElement("div");
        lightbox.className = "launcher-news-lightbox";
        lightbox.innerHTML = `
            <button class="launcher-news-lightbox-close" type="button" aria-label="Close">×</button>
            <img src="${this.escapeAttribute(imageUrl)}" alt="${this.escapeAttribute(title)}" />
        `;
        const close = () => lightbox.remove();
        lightbox.addEventListener("click", (event) => {
            if (event.target === lightbox)
                close();
        });
        (_a = lightbox.querySelector(".launcher-news-lightbox-close")) === null || _a === void 0 ? void 0 : _a.addEventListener("click", close);
        document.body.appendChild(lightbox);
    }
    bindLazyNewsCards() {
        const hydrateCard = (card) => {
            card.querySelectorAll("[data-news-card-video-src]").forEach((iframe) => {
                if (!iframe.src)
                    iframe.src = iframe.getAttribute("data-news-card-video-src") || "";
            });
        };
        const cards = Array.from(document.querySelectorAll(".launcher-news-card"));
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
    bindNewsReader() {
        var _a, _b;
        const reader = document.querySelector("[data-news-reader]");
        const feed = document.querySelector("[data-news-reader-feed]");
        if (!reader || !feed)
            return;
        const posts = Array.from(reader.querySelectorAll("[data-news-reader-post]"));
        let activeIndex = 0;
        const visibleRadius = 1;
        let scrollSyncTimeout = 0;
        const hydratePost = (post) => {
            post.querySelectorAll("[data-news-reader-image-src]").forEach((image) => {
                if (!image.src)
                    image.src = image.getAttribute("data-news-reader-image-src") || "";
            });
            post.querySelectorAll("[data-news-video-player]").forEach((player) => {
                this.hydrateNewsVideoPlayer(player);
            });
        };
        const updateVisiblePosts = () => {
            posts.forEach((item, index) => {
                const isVisible = Math.abs(index - activeIndex) <= visibleRadius;
                if (!item.classList.contains("is-unloaded")) {
                    const height = Math.max(item.offsetHeight, 560);
                    item.style.setProperty("--news-post-height", `${height}px`);
                }
                item.classList.toggle("is-unloaded", !isVisible);
            });
        };
        const setActivePost = (post) => {
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
        const syncActivePostFromScroll = () => {
            if (!reader.classList.contains("open"))
                return;
            const feedRect = feed.getBoundingClientRect();
            const feedCenter = feedRect.top + feedRect.height / 2;
            let closestPost = null;
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
        const scrollToPost = (newsId, behavior = "smooth") => {
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
        const open = (newsId) => {
            reader.classList.add("open");
            reader.setAttribute("aria-hidden", "false");
            document.body.classList.add("news-reader-open");
            window.setTimeout(() => scrollToPost(newsId, "auto"), 30);
            window.setTimeout(syncActivePostFromScroll, 120);
        };
        const close = () => {
            reader.classList.remove("open");
            reader.setAttribute("aria-hidden", "true");
            document.body.classList.remove("news-reader-open");
        };
        document.querySelectorAll("[data-news-reader-open]").forEach((button) => {
            button.addEventListener("click", (event) => {
                event.preventDefault();
                event.stopPropagation();
                open(button.getAttribute("data-news-reader-open") || "");
            });
        });
        document.querySelectorAll(".launcher-news-card").forEach((card) => {
            card.addEventListener("click", (event) => {
                const target = event.target;
                if (target === null || target === void 0 ? void 0 : target.closest("a, button, iframe"))
                    return;
                const opener = card.querySelector("[data-news-reader-open]");
                const newsId = opener === null || opener === void 0 ? void 0 : opener.getAttribute("data-news-reader-open");
                if (newsId)
                    open(newsId);
            });
        });
        document.querySelectorAll("[data-news-like]").forEach((button) => {
            const initialNewsId = button.getAttribute("data-news-like") || "";
            if (initialNewsId && localStorage.getItem(`lizerium-news-like-${initialNewsId}`) === "1")
                button.classList.add("liked");
            button.addEventListener("click", () => __awaiter(this, void 0, void 0, function* () {
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
                    const response = yield fetch(`/news/like/${encodeURIComponent(newsId)}`, {
                        method: "POST",
                        credentials: "same-origin",
                        headers: {
                            "Accept": "application/json",
                            "X-Requested-With": "XMLHttpRequest"
                        }
                    });
                    if (!response.ok)
                        throw new Error("Like failed");
                    const result = yield response.json();
                    if (typeof result.likeCount === "number") {
                        document.querySelectorAll(`[data-news-like="${newsId}"]`).forEach((sameNewsButton) => {
                            const count = sameNewsButton.querySelector("[data-news-like-count]");
                            if (count)
                                count.textContent = String(result.likeCount);
                            sameNewsButton.classList.add("liked");
                        });
                    }
                    localStorage.setItem(storageKey, "1");
                    button.classList.add("liked");
                }
                catch (_a) {
                }
                finally {
                    button.classList.remove("pending");
                }
            }));
        });
        reader.querySelectorAll("[data-news-reader-close]").forEach((button) => {
            button.addEventListener("click", close);
        });
        (_a = reader.querySelector("[data-news-reader-previous]")) === null || _a === void 0 ? void 0 : _a.addEventListener("click", () => {
            var _a;
            activeIndex = Math.max(0, activeIndex - 1);
            scrollToPost(((_a = posts[activeIndex]) === null || _a === void 0 ? void 0 : _a.getAttribute("data-news-reader-post")) || "");
        });
        (_b = reader.querySelector("[data-news-reader-next]")) === null || _b === void 0 ? void 0 : _b.addEventListener("click", () => {
            var _a;
            activeIndex = Math.min(posts.length - 1, activeIndex + 1);
            scrollToPost(((_a = posts[activeIndex]) === null || _a === void 0 ? void 0 : _a.getAttribute("data-news-reader-post")) || "");
        });
        if ("IntersectionObserver" in window) {
            const observer = new IntersectionObserver((entries) => {
                var _a;
                const visibleEntries = entries
                    .filter((entry) => entry.isIntersecting)
                    .sort((a, b) => b.intersectionRatio - a.intersectionRatio);
                const visiblePost = (_a = visibleEntries[0]) === null || _a === void 0 ? void 0 : _a.target;
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
        reader.querySelectorAll("[data-news-reader-share]").forEach((button) => {
            button.addEventListener("click", () => __awaiter(this, void 0, void 0, function* () {
                var _a, _b, _c;
                const newsId = button.getAttribute("data-news-reader-share") || "";
                const url = `${window.location.origin}${window.location.pathname}#news-${newsId}`;
                const title = ((_b = (_a = button.closest("[data-news-reader-post]")) === null || _a === void 0 ? void 0 : _a.querySelector("h2")) === null || _b === void 0 ? void 0 : _b.textContent) || document.title;
                if (navigator.share) {
                    try {
                        yield navigator.share({ title, url });
                    }
                    catch (_d) {
                    }
                    return;
                }
                yield ((_c = navigator.clipboard) === null || _c === void 0 ? void 0 : _c.writeText(url));
                button.classList.add("copied");
                window.setTimeout(() => button.classList.remove("copied"), 1200);
            }));
        });
        document.addEventListener("keydown", (event) => {
            if (event.key === "Escape" && reader.classList.contains("open"))
                close();
        });
    }
    hydrateNewsVideoPlayer(player) {
        if (player.getAttribute("data-news-video-ready") === "true")
            return;
        const iframe = player.querySelector("[data-news-video-frame]");
        if (!iframe)
            return;
        const post = player.closest("[data-news-reader-post]");
        const buttons = Array.from((post === null || post === void 0 ? void 0 : post.querySelectorAll("[data-news-video-src]")) || []);
        const setPlatform = (button) => {
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
    escapeAttribute(value) {
        return value
            .replace(/&/g, "&amp;")
            .replace(/"/g, "&quot;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;");
    }
}
//# sourceMappingURL=launcher.js.map