/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 14 августа 2026 08:37:41
 * Version: 1.0.145
 */

using LizeriumUtilities.FormatsData.DataBase.Response;

namespace LizeriumDatabase.Services.AppDataBaseService.Implements;

public partial class DataBaseService
{
    private async Task SeedDefaultProductsCatalogAsync()
    {
        // Default catalog is a first-run bootstrap. Runtime edits should happen through /products admin.
        var launcher = new ProductCategoryDataResponse
        {
            Key = "launcher",
            NameRu = "Лаунчер",
            NameEn = "Launcher",
            DescriptionRu = "Обновления и загрузчик проектов Lizerium",
            DescriptionEn = "Updates and downloader for Lizerium projects",
            IconUrl = "/img/pages/game/cat_launcher.webp",
            BackgroundUrl = "/img/pages/home/ecosystem-launcher-bg.webp",
            SortOrder = 10,
            IsActive = true,
            Products = new List<ProductDataResponse>
            {
                new()
                {
                    TitleRu = "Загрузчик Лизериум (Lizerium Steam)",
                    TitleEn = "Lizerium uploader (Lizerium Steam)",
                    DescriptionRu = "Предназначен для скачивания обновлений для доступных игр",
                    DescriptionEn = "Downloads updates for available games",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 10,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "С Портала",
                            NameEn = "Portal",
                            Url = "/uploader/projects/download/steam",
                            IconUrl = "/img/Main.webp",
                            SortOrder = 10,
                            IsActive = true
                        },
                        new()
                        {
                            NameRu = "Исходный код",
                            NameEn = "Source code",
                            Url = "https://github.com/Lizerium/LizeriumSteam",
                            IconUrl = "/img/social/github.webp",
                            SortOrder = 20,
                            IsActive = true
                        },
                        new()
                        {
                            NameRu = "MEGA",
                            NameEn = "MEGA",
                            Url = "https://mega.nz/file/K9t0wJDI#3eFXg38amgsr0f2b42lBVk3AU0SNY480aHMBNx8a5C3A",
                            IconUrl = "/img/social/mega.webp",
                            SortOrder = 30,
                            IsActive = true
                        }
                    }
                }
            }
        };

        var downloads = new ProductCategoryDataResponse
        {
            Key = "downloads",
            NameRu = "Что доступно для скачивания",
            NameEn = "Available downloads",
            DescriptionRu = "Игры и сборки, доступные через портал",
            DescriptionEn = "Games and builds available through the portal",
            IconUrl = "/img/pages/game/cat_games.webp",
            BackgroundUrl = "/img/pages/home/ecosystem-projects-bg.webp",
            SortOrder = 20,
            IsActive = true,
            Products = new List<ProductDataResponse>
            {
                new()
                {
                    TitleRu = "Игра Lizerium",
                    TitleEn = "Lizerium game",
                    DescriptionRu = "Самостоятельная игра на базе Freelancer (2003)",
                    DescriptionEn = "Standalone game based on Freelancer (2003)",
                    IconUrl = "/img/pages/game/lizerium-game.webp",
                    SortOrder = 10,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "Яндекс Диск",
                            NameEn = "Yandex Disk",
                            Url = "/uploader/projects/download/lizerium_game",
                            IconUrl = "/img/social/yandex_disk.webp",
                            SortOrder = 10,
                            IsActive = true
                        },
                        new()
                        {
                            NameRu = "MEGA",
                            NameEn = "MEGA",
                            Url = "https://mega.nz/file/Wo0zhSDQ#qM2Nrngt_FYabX7mb26g_gjEhBy6BCJ0MPIzWC2_Rek",
                            IconUrl = "/img/social/mega.webp",
                            SortOrder = 20,
                            IsActive = true
                        }
                    }
                },
                new()
                {
                    TitleRu = "Игра Freelancer (2003)",
                    TitleEn = "Freelancer (2003)",
                    DescriptionRu = "Официальная игра от Digital Anvil и Microsoft",
                    DescriptionEn = "Official game by Digital Anvil and Microsoft",
                    IconUrl = "/img/pages/game/freelancer-game.webp",
                    SortOrder = 20,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "Яндекс Диск",
                            NameEn = "Yandex Disk",
                            Url = "/uploader/projects/download/freelancer_game",
                            IconUrl = "/img/social/yandex_disk.webp",
                            SortOrder = 10,
                            IsActive = true
                        },
                        new()
                        {
                            NameRu = "MEGA",
                            NameEn = "MEGA",
                            Url = "https://mega.nz/file/2sMnFQKb#0Av5_dSCLjZ4tBHHfQX42dZIumu9wwzDzwT86b_a4k0",
                            IconUrl = "/img/social/mega.webp",
                            SortOrder = 20,
                            IsActive = true
                        }
                    }
                }
            }
        };

        var tools = new ProductCategoryDataResponse
        {
            Key = "tools",
            NameRu = "Инструменты и вспомогательные программы",
            NameEn = "Tools and utilities",
            DescriptionRu = "Утилиты для сравнения файлов, подготовки обновлений и работы с данными Freelancer",
            DescriptionEn = "Utilities for file comparison, update preparation, and Freelancer data work",
            IconUrl = "/img/pages/game/cat_tools.webp",
            BackgroundUrl = "/img/pages/home/feature-card-info-bg.webp",
            SortOrder = 30,
            IsActive = true,
            Products = new List<ProductDataResponse>
            {
                new()
                {
                    TitleRu = "LizeriumFindChanges",
                    TitleEn = "LizeriumFindChanges",
                    DescriptionRu = "Инструмент для сравнения двух версий файловой структуры и подготовки папки обновления",
                    DescriptionEn = "Compares two file-structure versions and prepares an update folder",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 10,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "Открыть релизы",
                            NameEn = "Open releases",
                            Url = "https://github.com/Lizerium/LizeriumFindChanges/releases",
                            IconUrl = "/img/social/github.webp",
                            SortOrder = 10,
                            IsActive = true
                        }
                    }
                },
                new()
                {
                    TitleRu = "LizeriumDataToolkit",
                    TitleEn = "LizeriumDataToolkit",
                    DescriptionRu = "Чтение, разбор и сериализация данных из игр Freelancer",
                    DescriptionEn = "Reads, parses, and serializes Freelancer game data",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 20,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "Открыть релизы",
                            NameEn = "Open releases",
                            Url = "https://github.com/Lizerium/LizeriumDataToolkit/releases",
                            IconUrl = "/img/social/github.webp",
                            SortOrder = 10,
                            IsActive = true
                        }
                    }
                }, new()
                {
                    TitleRu = "LizeriumVSCodeColorPicker",
                    TitleEn = "LizeriumVSCodeColorPicker",
                    DescriptionRu = "Расширение для VS Code, позволяющее определять цвета в строках файла с помощью конструкций вида `color = R, G, B` (значения от 0 до 255)",
                    DescriptionEn = "An extension for VSCode that allows you to define colors in file lines with the `color = R, G, B` (0-255) constructs",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 30,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "Открыть релизы",
                            NameEn = "Open releases",
                            Url = "https://github.com/Lizerium/LizeriumVSCodeColorPicker/releases",
                            IconUrl = "/img/social/github.webp",
                            SortOrder = 10,
                            IsActive = true
                        }
                    }
                }, new()
                {
                    TitleRu = "LizeriumAccauntManager",
                    TitleEn = "LizeriumAccauntManager",
                    DescriptionRu = "Современный, переработанный менеджер учетных записей игроков для сервера Freelancer, адаптированный для крупных игровых сборок и поддерживающий кириллицу, асинхронную обработку и расширяемую архитектуру",
                    DescriptionEn = "A modern, reimagined player account manager for Freelancer Server, redesigned for large game builds, Cyrillic support, asynchronous processing, and an extensible architecture",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 40,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "Открыть релизы",
                            NameEn = "Open releases",
                            Url = "https://github.com/Lizerium/LizeriumAccauntManager/releases",
                            IconUrl = "/img/social/github.webp",
                            SortOrder = 10,
                            IsActive = true
                        }
                    }
                }, new()
                {
                    TitleRu = "Lizerium.Restarter.Server",
                    TitleEn = "Lizerium.Restarter.Server",
                    DescriptionRu = "Автоматический мониторинг, перезапуск и удаленное управление через API для игровых серверов Freelancer",
                    DescriptionEn = "Automatic monitoring, restart and remote API control for Freelancer game servers",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 50,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "Открыть релизы",
                            NameEn = "Open releases",
                            Url = "https://github.com/Lizerium/Lizerium.Restarter.Server/releases",
                            IconUrl = "/img/social/github.webp",
                            SortOrder = 10,
                            IsActive = true
                        }
                    }
                }, new()
                {
                    TitleRu = "Lizerium.RDL.Converter",
                    TitleEn = "Lizerium.RDL.Converter",
                    DescriptionRu = "Реализация frc.exe (Freelancer Resource Compiler) на языке C#. Преобразует RDL (XML) в текстовый формат FRC, используемый во время выполнения",
                    DescriptionEn = "C# implementation of frc.exe (Freelancer Resource Compiler). Converts RDL (XML) into FRC runtime text format",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 60,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "Открыть релизы",
                            NameEn = "Open releases",
                            Url = "https://github.com/Lizerium/Lizerium.RDL.Converter/releases",
                            IconUrl = "/img/social/github.webp",
                            SortOrder = 10,
                            IsActive = true
                        }
                    }
                }, new()
                {
                    TitleRu = "Lizerium.Localization.Toolkit",
                    TitleEn = "Lizerium.Localization.Toolkit",
                    DescriptionRu = "Lizerium.Localization.Toolkit — это инструмент для организации процесса локализации в .NET-проектах, где переводы хранятся в файлах .resx. Он объединяет в себе загрузку ресурсов во время выполнения, генератор кода на базе Roslyn, диагностику с помощью анализатора, средства автоматического исправления кода в Visual Studio и отдельный редактор на базе WPF",
                    DescriptionEn = "Lizerium.Localization.Toolkit is a .NET localization workflow for projects that store translations in .resx files. It combines runtime loading, a Roslyn source generator, analyzer diagnostics, Visual Studio code fixes, and a standalone WPF editor",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 70,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "Открыть релизы",
                            NameEn = "Open releases",
                            Url = "https://github.com/Lizerium/Lizerium.Localization.Toolkit/releases",
                            IconUrl = "/img/social/github.webp",
                            SortOrder = 10,
                            IsActive = true
                        }
                    }
                }, new()
                {
                    TitleRu = "Lizerium.BINI.Converter",
                    TitleEn = "Lizerium.BINI.Converter",
                    DescriptionRu = "Реализация на C# и JavaScript для преобразования бинарных INI-файлов формата BINI (из игры Freelancer) в редактируемые текстовые INI-файлы и обратно",
                    DescriptionEn = "C#, JavaScript implementation for converting Freelancer BINI binary INI files into editable text INI files and back",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 80,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "Демонстрация",
                            NameEn = "Demo",
                            Url = "https://lizerium.github.io/Lizerium.BINI.Converter/",
                            IconUrl = "/img/Main.webp",
                            SortOrder = 10,
                            IsActive = true
                        },
                        new()
                        {
                            NameRu = "Открыть релизы",
                            NameEn = "Open releases",
                            Url = "https://github.com/Lizerium/Lizerium.BINI.Converter/releases",
                            IconUrl = "/img/social/github.webp",
                            SortOrder = 20,
                            IsActive = true
                        }
                    }
                }, new()
                {
                    TitleRu = "CompilerInfocardsUI",
                    TitleEn = "CompilerInfocardsUI",
                    DescriptionRu = "Freelancer (2003): Панель управления информационными карточками для Lizerium (универсальная)",
                    DescriptionEn = "Freelancer (2003) Information Card Control Panel for Lizerium (Universal)",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 90,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "Открыть релизы",
                            NameEn = "Open releases",
                            Url = "https://github.com/Lizerium/CompilerInfocardsUI/releases",
                            IconUrl = "/img/social/github.webp",
                            SortOrder = 10,
                            IsActive = true
                        }
                    }
                }, new()
                {
                    TitleRu = "LizeriumFLHook",
                    TitleEn = "LizeriumFLHook",
                    DescriptionRu = "FLHook и набор плагинов для сервера Freelancer (2003 г.)",
                    DescriptionEn = "FLHook and a collection of plugins for the Freelancer 2003 Server",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 100,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "Открыть релизы",
                            NameEn = "Open releases",
                            Url = "https://github.com/Lizerium/LizeriumFLHook/releases",
                            IconUrl = "/img/social/github.webp",
                            SortOrder = 10,
                            IsActive = true
                        }
                    }
                }, new()
                {
                    TitleRu = "LizeriumUTFtoXML",
                    TitleEn = "LizeriumUTFtoXML",
                    DescriptionRu = "Конвертер UTF в XML для файлов игры Freelancer (2003)",
                    DescriptionEn = "UTF to XML Converter for Freelancer (2003) game data",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 110,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "Открыть релизы",
                            NameEn = "Open releases",
                            Url = "https://github.com/Lizerium/LizeriumUTFtoXML/releases",
                            IconUrl = "/img/social/github.webp",
                            SortOrder = 10,
                            IsActive = true
                        }
                    }
                }, new()
                {
                    TitleRu = "Lizerium.UTF.Editor",
                    TitleEn = "Lizerium.UTF.Editor",
                    DescriptionRu = "Редактор и инструмент анализа геометрии и ресурсов (UTF / CMP / 3DB) для Freelancer",
                    DescriptionEn = "Editor and analysis tool for geometry and resources (UTF / CMP / 3DB) for Freelancer",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 120,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "Открыть релизы",
                            NameEn = "Open releases",
                            Url = "https://github.com/Lizerium/Lizerium.UTF.Editor/releases",
                            IconUrl = "/img/social/github.webp",
                            SortOrder = 10,
                            IsActive = true
                        }
                    }
                }, new()
                {
                    TitleRu = "Freelancer.Reverse.Runtime",
                    TitleEn = "Freelancer.Reverse.Runtime",
                    DescriptionRu = "Я разрабатываю слой времени выполнения (runtime layer) поверх игры Freelancer (2003 года), пересобирая и подменяя системные DLL, чтобы безопасно исследовать, расширять и документировать поведение оригинального движка",
                    DescriptionEn = "I'm building a runtime layer on top of Freelancer (2003), rebuilding and overriding system DLLs to safely explore, extend, and document the behavior of the original engine",
                    IconUrl = "/img/social/binnexus.webp",
                    SortOrder = 130,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "Демонстрация",
                            NameEn = "Demo",
                            Url = "https://dvurechensky.github.io/Freelancer.Reverse.Runtime/",
                            IconUrl = "/img/social/binnexus.webp",
                            SortOrder = 10,
                            IsActive = true
                        }
                    }
                }
            }
        };

        await ProductCategories.AddRangeAsync(launcher, downloads, tools);
        await SaveChangesAsync();
    }

}
