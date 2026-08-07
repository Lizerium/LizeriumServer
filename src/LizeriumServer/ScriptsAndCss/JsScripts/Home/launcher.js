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
    bindNewsReader() {
        var _a, _b;
        const reader = document.querySelector("[data-news-reader]");
        const feed = document.querySelector("[data-news-reader-feed]");
        if (!reader || !feed)
            return;
        const posts = Array.from(reader.querySelectorAll("[data-news-reader-post]"));
        let activeIndex = 0;
        let renderedStart = 0;
        let renderedEnd = -1;
        let suppressScrollSyncUntil = 0;
        const getVisibleHeaderInset = () => {
            if (!window.matchMedia("(max-width: 900px)").matches)
                return 0;
            const header = document.querySelector(".header_icon_lang_block");
            if (!header)
                return 0;
            const rect = header.getBoundingClientRect();
            if (rect.bottom <= 0 || rect.top >= window.innerHeight)
                return 0;
            return Math.max(0, Math.min(rect.bottom, window.innerHeight * 0.42));
        };
        const updateReaderViewportInsets = () => {
            feed.style.setProperty("--news-reader-header-inset", `${getVisibleHeaderInset()}px`);
        };
        const getFeedTopInset = () => {
            updateReaderViewportInsets();
            const paddingTop = Number.parseFloat(window.getComputedStyle(feed).paddingTop || "0");
            return Math.max(16, paddingTop);
        };
        const withReaderTarget = (href, target) => {
            const url = new URL(href, window.location.href);
            url.searchParams.set("reader", target);
            return `${url.pathname}${url.search}${url.hash}`;
        };
        const navigateReaderPage = (target) => {
            const pageLinkSelector = target === "first"
                ? ".launcher-news-pagination a.active + a"
                : ".launcher-news-pagination a:has(+ a.active)";
            const pageLink = document.querySelector(pageLinkSelector);
            if (pageLink === null || pageLink === void 0 ? void 0 : pageLink.href)
                window.location.href = withReaderTarget(pageLink.href, target);
        };
        const releaseProgrammaticScroll = () => {
            suppressScrollSyncUntil = 0;
        };
        const hydratePost = (post, resetVideos = false, preferredPlatform = "") => {
            post.querySelectorAll("[data-news-reader-image-src]").forEach((image) => {
                if (!image.src)
                    image.src = image.getAttribute("data-news-reader-image-src") || "";
            });
            post.querySelectorAll("[data-news-video-player]").forEach((player) => {
                if (resetVideos)
                    player.dataset.newsVideoResetDefault = "true";
                if (preferredPlatform)
                    player.dataset.newsVideoPreferredPlatform = preferredPlatform;
                this.hydrateNewsVideoPlayer(player);
            });
        };
        const setRenderedWindow = (start, end, hydrateVisible = true) => {
            renderedStart = Math.max(0, Math.min(start, posts.length - 1));
            renderedEnd = Math.max(renderedStart, Math.min(end, posts.length - 1));
            posts.forEach((item, index) => {
                if (!item.classList.contains("is-unloaded")) {
                    const height = Math.max(item.offsetHeight, 560);
                    item.style.setProperty("--news-post-height", `${height}px`);
                }
                const isRendered = index >= renderedStart && index <= renderedEnd;
                item.classList.toggle("is-unloaded", !isRendered);
                if (isRendered && hydrateVisible)
                    hydratePost(item);
            });
        };
        const resetRenderedWindow = (centerIndex) => {
            const start = Math.max(0, Math.min(centerIndex, posts.length - 1));
            const end = start;
            setRenderedWindow(start, end, false);
        };
        const ensurePostRendered = (index) => {
            if (index < 0 || index >= posts.length)
                return;
            if (renderedEnd < renderedStart) {
                resetRenderedWindow(index);
                return;
            }
            if (index < renderedStart || index > renderedEnd)
                resetRenderedWindow(index);
            else
                setRenderedWindow(renderedStart, renderedEnd);
        };
        const setActivePost = (post, resetVideos = false, preferredPlatform = "") => {
            const nextIndex = posts.indexOf(post);
            if (nextIndex < 0)
                return;
            activeIndex = nextIndex;
            posts.forEach((item, index) => item.classList.toggle("active", index === activeIndex));
            ensurePostRendered(activeIndex);
            hydratePost(post, resetVideos, preferredPlatform);
        };
        const alignPostToTop = (post, behavior = "auto", attempts = 8) => {
            suppressScrollSyncUntil = Date.now() + 1200;
            const align = (remainingAttempts) => {
                const nextTop = Math.max(0, post.offsetTop - getFeedTopInset());
                feed.scrollTo({ top: nextTop, behavior: remainingAttempts === attempts ? behavior : "auto" });
                if (remainingAttempts <= 0) {
                    window.setTimeout(() => {
                        if (Date.now() >= suppressScrollSyncUntil)
                            suppressScrollSyncUntil = 0;
                    }, 40);
                    return;
                }
                window.requestAnimationFrame(() => align(remainingAttempts - 1));
            };
            window.requestAnimationFrame(() => align(attempts));
        };
        const scrollToPost = (newsId, behavior = "smooth", resetVideos = false, preferredPlatform = "") => {
            const post = posts.find((item) => item.getAttribute("data-news-reader-post") === newsId);
            if (!post)
                return;
            const postIndex = posts.indexOf(post);
            resetRenderedWindow(postIndex);
            setActivePost(post, resetVideos, preferredPlatform);
            alignPostToTop(post, behavior);
        };
        const scrollCurrentPostToBottom = () => {
            const post = posts[activeIndex];
            if (!post)
                return;
            suppressScrollSyncUntil = Date.now() + 500;
            const maxTop = Math.max(0, feed.scrollHeight - feed.clientHeight);
            feed.scrollTo({ top: maxTop, behavior: "smooth" });
        };
        const scrollCurrentPostToTop = () => {
            const post = posts[activeIndex];
            if (!post)
                return;
            suppressScrollSyncUntil = Date.now() + 500;
            feed.scrollTo({ top: Math.max(0, post.offsetTop), behavior: "smooth" });
        };
        const isNearCurrentPostBottom = () => {
            const maxTop = Math.max(0, feed.scrollHeight - feed.clientHeight);
            return feed.scrollTop >= maxTop - 24;
        };
        const isNearCurrentPostTop = () => {
            const post = posts[activeIndex];
            if (!post)
                return true;
            return feed.scrollTop <= Math.max(0, post.offsetTop) + 24;
        };
        const open = (newsId, preferredPlatform = "") => {
            reader.classList.add("open");
            reader.setAttribute("aria-hidden", "false");
            document.body.classList.add("news-reader-open");
            updateReaderViewportInsets();
            const post = posts.find((item) => item.getAttribute("data-news-reader-post") === newsId);
            const postIndex = post ? posts.indexOf(post) : 0;
            feed.scrollTop = 0;
            resetRenderedWindow(postIndex);
            window.setTimeout(() => scrollToPost(newsId, "auto", true, preferredPlatform), 0);
            window.setTimeout(() => scrollToPost(newsId, "auto", false, preferredPlatform), 220);
        };
        const close = () => {
            reader.classList.remove("open");
            reader.setAttribute("aria-hidden", "true");
            document.body.classList.remove("news-reader-open");
        };
        document.querySelectorAll("[data-news-card-video-src]").forEach((button) => {
            button.addEventListener("click", (event) => {
                event.preventDefault();
                event.stopPropagation();
                const card = button.closest(".launcher-news-card");
                const frame = card === null || card === void 0 ? void 0 : card.querySelector("[data-news-card-video-frame]");
                const src = button.getAttribute("data-news-card-video-src") || "";
                const platform = button.getAttribute("data-news-card-video-platform") || "";
                if (!card || !src || !platform)
                    return;
                card.querySelectorAll("[data-news-card-video-src]").forEach((item) => {
                    item.classList.toggle("active", item === button);
                });
                if (frame) {
                    frame.src = src;
                    frame.setAttribute("data-news-reader-video-src", src);
                }
                card.setAttribute("data-news-reader-platform", platform);
                card.querySelectorAll("[data-news-reader-open]").forEach((opener) => {
                    opener.setAttribute("data-news-reader-platform", platform);
                });
            });
        });
        document.querySelectorAll("[data-news-reader-open]").forEach((button) => {
            button.addEventListener("click", (event) => {
                event.preventDefault();
                event.stopPropagation();
                open(button.getAttribute("data-news-reader-open") || "", button.getAttribute("data-news-reader-platform") || "");
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
                    open(newsId, card.getAttribute("data-news-reader-platform") || "");
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
            if (!isNearCurrentPostTop()) {
                scrollCurrentPostToTop();
                return;
            }
            const targetIndex = Math.max(0, activeIndex - 1);
            if (targetIndex === activeIndex) {
                navigateReaderPage("last");
                return;
            }
            const targetPost = posts[targetIndex];
            if (!targetPost)
                return;
            ensurePostRendered(targetIndex);
            scrollToPost(targetPost.getAttribute("data-news-reader-post") || "", "auto", true);
        });
        (_b = reader.querySelector("[data-news-reader-next]")) === null || _b === void 0 ? void 0 : _b.addEventListener("click", () => {
            if (!isNearCurrentPostBottom()) {
                scrollCurrentPostToBottom();
                return;
            }
            const targetIndex = Math.min(posts.length - 1, activeIndex + 1);
            if (targetIndex === activeIndex) {
                navigateReaderPage("first");
                return;
            }
            const targetPost = posts[targetIndex];
            if (!targetPost)
                return;
            ensurePostRendered(targetIndex);
            scrollToPost(targetPost.getAttribute("data-news-reader-post") || "", "auto", true);
        });
        feed.addEventListener("scroll", () => {
            if (Date.now() >= suppressScrollSyncUntil)
                suppressScrollSyncUntil = 0;
        }, { passive: true });
        feed.addEventListener("wheel", releaseProgrammaticScroll, { passive: true });
        feed.addEventListener("touchstart", releaseProgrammaticScroll, { passive: true });
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
                const shareLabel = button.dataset.shareLabel || button.textContent || "";
                const copiedLabel = button.dataset.shareCopiedLabel || shareLabel;
                button.classList.add("copied");
                button.textContent = copiedLabel;
                window.setTimeout(() => {
                    button.classList.remove("copied");
                    button.textContent = shareLabel;
                }, 1200);
            }));
        });
        const hashReaderTarget = window.location.hash === "#reader-first"
            ? "first"
            : window.location.hash === "#reader-last"
                ? "last"
                : "";
        const readerTarget = new URLSearchParams(window.location.search).get("reader") || hashReaderTarget;
        if (readerTarget === "first" && posts[0]) {
            window.setTimeout(() => open(posts[0].getAttribute("data-news-reader-post") || ""), 120);
        }
        else if (readerTarget === "last" && posts[posts.length - 1]) {
            window.setTimeout(() => open(posts[posts.length - 1].getAttribute("data-news-reader-post") || ""), 120);
        }
        window.addEventListener("resize", () => {
            if (!reader.classList.contains("open"))
                return;
            updateReaderViewportInsets();
            alignPostToTop(posts[activeIndex], "auto", 2);
        });
        document.addEventListener("keydown", (event) => {
            if (event.key === "Escape" && reader.classList.contains("open"))
                close();
            if (reader.classList.contains("open")
                && ["ArrowDown", "ArrowUp", "PageDown", "PageUp", "Home", "End", " "].includes(event.key))
                releaseProgrammaticScroll();
        });
    }
    hydrateNewsVideoPlayer(player) {
        const iframe = player.querySelector("[data-news-video-frame]");
        if (!iframe)
            return;
        const isInlineVideo = player.classList.contains("launcher-news-reader-inline-video");
        const buttons = isInlineVideo
            ? []
            : Array.from(player.querySelectorAll("[data-news-video-src]"));
        const status = this.ensureNewsVideoStatus(player);
        const preferredPlatform = player.dataset.newsVideoPreferredPlatform || "";
        const shouldResetToDefault = player.dataset.newsVideoResetDefault === "true"
            || preferredPlatform.length > 0
            || player.getAttribute("data-news-video-ready") !== "true";
        delete player.dataset.newsVideoResetDefault;
        delete player.dataset.newsVideoPreferredPlatform;
        let loadTimer = 0;
        const setStatus = (message, mode = "checking") => {
            status.textContent = message;
            status.dataset.newsVideoStatus = mode;
            status.hidden = mode === "ready";
        };
        const markButtonStatus = (button, mode) => {
            const platform = button.getAttribute("data-news-video-platform") || "video";
            button.dataset.newsVideoAvailability = mode;
            button.setAttribute("aria-disabled", mode === "blocked" ? "true" : "false");
            if (mode === "blocked")
                button.title = this.getVideoPlatformBlockedMessage(platform);
        };
        const clearButtonStatuses = () => {
            buttons.forEach((button) => {
                delete button.dataset.newsVideoAvailability;
                button.setAttribute("aria-disabled", "false");
            });
        };
        const setPlatform = (button) => {
            const src = button.getAttribute("data-news-video-src") || "";
            if (!src)
                return;
            window.clearTimeout(loadTimer);
            buttons.forEach((item) => item.classList.toggle("active", item === button));
            iframe.setAttribute("data-news-reader-video-src", src);
            iframe.src = src;
            iframe.dataset.loaded = "false";
            setStatus("", "ready");
            loadTimer = window.setTimeout(() => {
                if (iframe.dataset.loaded === "true")
                    return;
                markButtonStatus(button, "blocked");
                setStatus(this.getVideoPlatformBlockedMessage(button.getAttribute("data-news-video-platform") || "video"), "blocked");
            }, 4500);
        };
        const activatePlatform = (button) => {
            const src = button.getAttribute("data-news-video-src") || "";
            if (!src)
                return;
            markButtonStatus(button, "available");
            setPlatform(button);
        };
        if (player.getAttribute("data-news-video-ready") !== "true") {
            iframe.addEventListener("load", () => {
                iframe.dataset.loaded = "true";
                window.clearTimeout(loadTimer);
                const activeButton = buttons.find((button) => button.classList.contains("active"));
                if (activeButton)
                    markButtonStatus(activeButton, "available");
                setStatus("", "ready");
            });
            buttons.forEach((button) => {
                button.addEventListener("click", (event) => {
                    event.preventDefault();
                    activatePlatform(button);
                });
            });
        }
        if (shouldResetToDefault)
            clearButtonStatuses();
        const defaultButton = this.getDefaultNewsVideoButton(buttons, preferredPlatform);
        if (defaultButton && shouldResetToDefault)
            activatePlatform(defaultButton);
        else if (!iframe.src)
            iframe.src = iframe.getAttribute("data-news-reader-video-src") || "";
        player.setAttribute("data-news-video-ready", "true");
    }
    getDefaultNewsVideoButton(buttons, preferredPlatform = "") {
        if (preferredPlatform) {
            const preferred = buttons.find((button) => button.getAttribute("data-news-video-platform") === preferredPlatform);
            if (preferred)
                return preferred;
        }
        const culture = this.getCurrentCulture();
        const order = culture === "ru"
            ? ["rutube", "vk", "youtube"]
            : ["youtube", "vk", "rutube"];
        for (const platform of order) {
            const button = buttons.find((item) => item.getAttribute("data-news-video-platform") === platform);
            if (button)
                return button;
        }
        return buttons[0];
    }
    getCurrentCulture() {
        var _a;
        const selectedCulture = (_a = document.querySelector("#cultureForm select[name='culture']")) === null || _a === void 0 ? void 0 : _a.value;
        if (selectedCulture === null || selectedCulture === void 0 ? void 0 : selectedCulture.toLowerCase().startsWith("en"))
            return "en";
        if (selectedCulture === null || selectedCulture === void 0 ? void 0 : selectedCulture.toLowerCase().startsWith("ru"))
            return "ru";
        const htmlLanguage = document.documentElement.lang || "";
        if (htmlLanguage.toLowerCase().startsWith("en"))
            return "en";
        return "ru";
    }
    ensureNewsVideoStatus(player) {
        const current = player.querySelector("[data-news-video-status]");
        if (current)
            return current;
        const status = document.createElement("div");
        status.className = "launcher-news-video-status";
        status.setAttribute("data-news-video-status", "checking");
        status.hidden = true;
        player.appendChild(status);
        return status;
    }
    getVideoPlatformBlockedMessage(platform) {
        var _a;
        const name = this.getVideoPlatformName(platform);
        const template = (_a = document
            .querySelector("[data-news-video-blocked-template]")) === null || _a === void 0 ? void 0 : _a.dataset.newsVideoBlockedTemplate;
        return (template || "{0}").replace("{0}", name);
    }
    getVideoPlatformName(platform) {
        var _a;
        switch (platform) {
            case "youtube":
                return "YouTube";
            case "rutube":
                return "Rutube";
            case "vk":
                return "VK";
            default:
                return ((_a = document
                    .querySelector("[data-news-video-generic-platform]")) === null || _a === void 0 ? void 0 : _a.dataset.newsVideoGenericPlatform) || platform;
        }
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