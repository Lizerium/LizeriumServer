/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 09 августа 2026 15:52:37
 * Version: 1.0.135
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
            NameRu = "Р›Р°СѓРЅС‡РµСЂ",
            NameEn = "Launcher",
            DescriptionRu = "РћР±РЅРѕРІР»РµРЅРёСЏ Рё Р·Р°РіСЂСѓР·С‡РёРє РїСЂРѕРµРєС‚РѕРІ Lizerium",
            DescriptionEn = "Updates and downloader for Lizerium projects",
            IconUrl = "/img/pages/game/cat_launcher.webp",
            BackgroundUrl = "/img/pages/home/ecosystem-launcher-bg.webp",
            SortOrder = 10,
            IsActive = true,
            Products = new List<ProductDataResponse>
            {
                new()
                {
                    TitleRu = "Р—Р°РіСЂСѓР·С‡РёРє Р›РёР·РµСЂРёСѓРј (Lizerium Steam)",
                    TitleEn = "Lizerium uploader (Lizerium Steam)",
                    DescriptionRu = "РџСЂРµРґРЅР°Р·РЅР°С‡РµРЅ РґР»СЏ СЃРєР°С‡РёРІР°РЅРёСЏ РѕР±РЅРѕРІР»РµРЅРёР№ РґР»СЏ РґРѕСЃС‚СѓРїРЅС‹С… РёРіСЂ",
                    DescriptionEn = "Downloads updates for available games",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 10,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "РЎ РџРѕСЂС‚Р°Р»Р°",
                            NameEn = "Portal",
                            Url = "/uploader/projects/download/steam",
                            IconUrl = "/img/Main.webp",
                            SortOrder = 10,
                            IsActive = true
                        },
                        new()
                        {
                            NameRu = "РСЃС…РѕРґРЅС‹Р№ РєРѕРґ",
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
            NameRu = "Р§С‚Рѕ РґРѕСЃС‚СѓРїРЅРѕ РґР»СЏ СЃРєР°С‡РёРІР°РЅРёСЏ",
            NameEn = "Available downloads",
            DescriptionRu = "РРіСЂС‹ Рё СЃР±РѕСЂРєРё, РґРѕСЃС‚СѓРїРЅС‹Рµ С‡РµСЂРµР· РїРѕСЂС‚Р°Р»",
            DescriptionEn = "Games and builds available through the portal",
            IconUrl = "/img/pages/game/cat_games.webp",
            BackgroundUrl = "/img/pages/home/ecosystem-projects-bg.webp",
            SortOrder = 20,
            IsActive = true,
            Products = new List<ProductDataResponse>
            {
                new()
                {
                    TitleRu = "РРіСЂР° Lizerium",
                    TitleEn = "Lizerium game",
                    DescriptionRu = "РЎР°РјРѕСЃС‚РѕСЏС‚РµР»СЊРЅР°СЏ РёРіСЂР° РЅР° Р±Р°Р·Рµ Freelancer (2003)",
                    DescriptionEn = "Standalone game based on Freelancer (2003)",
                    IconUrl = "/img/pages/game/lizerium-game.webp",
                    SortOrder = 10,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "РЇРЅРґРµРєСЃ Р”РёСЃРє",
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
                    TitleRu = "РРіСЂР° Freelancer (2003)",
                    TitleEn = "Freelancer (2003)",
                    DescriptionRu = "РћС„РёС†РёР°Р»СЊРЅР°СЏ РёРіСЂР° РѕС‚ Digital Anvil Рё Microsoft",
                    DescriptionEn = "Official game by Digital Anvil and Microsoft",
                    IconUrl = "/img/pages/game/freelancer-game.webp",
                    SortOrder = 20,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "РЇРЅРґРµРєСЃ Р”РёСЃРє",
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
            NameRu = "РРЅСЃС‚СЂСѓРјРµРЅС‚С‹ Рё РІСЃРїРѕРјРѕРіР°С‚РµР»СЊРЅС‹Рµ РїСЂРѕРіСЂР°РјРјС‹",
            NameEn = "Tools and utilities",
            DescriptionRu = "РЈС‚РёР»РёС‚С‹ РґР»СЏ СЃСЂР°РІРЅРµРЅРёСЏ С„Р°Р№Р»РѕРІ, РїРѕРґРіРѕС‚РѕРІРєРё РѕР±РЅРѕРІР»РµРЅРёР№ Рё СЂР°Р±РѕС‚С‹ СЃ РґР°РЅРЅС‹РјРё Freelancer",
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
                    DescriptionRu = "РРЅСЃС‚СЂСѓРјРµРЅС‚ РґР»СЏ СЃСЂР°РІРЅРµРЅРёСЏ РґРІСѓС… РІРµСЂСЃРёР№ С„Р°Р№Р»РѕРІРѕР№ СЃС‚СЂСѓРєС‚СѓСЂС‹ Рё РїРѕРґРіРѕС‚РѕРІРєРё РїР°РїРєРё РѕР±РЅРѕРІР»РµРЅРёСЏ",
                    DescriptionEn = "Compares two file-structure versions and prepares an update folder",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 10,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "РћС‚РєСЂС‹С‚СЊ СЂРµР»РёР·С‹",
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
                    DescriptionRu = "Р§С‚РµРЅРёРµ, СЂР°Р·Р±РѕСЂ Рё СЃРµСЂРёР°Р»РёР·Р°С†РёСЏ РґР°РЅРЅС‹С… РёР· РёРіСЂ Freelancer",
                    DescriptionEn = "Reads, parses, and serializes Freelancer game data",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 20,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "РћС‚РєСЂС‹С‚СЊ СЂРµР»РёР·С‹",
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
                    DescriptionRu = "Р Р°СЃС€РёСЂРµРЅРёРµ РґР»СЏ VS Code, РїРѕР·РІРѕР»СЏСЋС‰РµРµ РѕРїСЂРµРґРµР»СЏС‚СЊ С†РІРµС‚Р° РІ СЃС‚СЂРѕРєР°С… С„Р°Р№Р»Р° СЃ РїРѕРјРѕС‰СЊСЋ РєРѕРЅСЃС‚СЂСѓРєС†РёР№ РІРёРґР° `color = R, G, B` (Р·РЅР°С‡РµРЅРёСЏ РѕС‚ 0 РґРѕ 255)",
                    DescriptionEn = "An extension for VSCode that allows you to define colors in file lines with the `color = R, G, B` (0-255) constructs",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 30,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "РћС‚РєСЂС‹С‚СЊ СЂРµР»РёР·С‹",
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
                    DescriptionRu = "РЎРѕРІСЂРµРјРµРЅРЅС‹Р№, РїРµСЂРµСЂР°Р±РѕС‚Р°РЅРЅС‹Р№ РјРµРЅРµРґР¶РµСЂ СѓС‡РµС‚РЅС‹С… Р·Р°РїРёСЃРµР№ РёРіСЂРѕРєРѕРІ РґР»СЏ СЃРµСЂРІРµСЂР° Freelancer, Р°РґР°РїС‚РёСЂРѕРІР°РЅРЅС‹Р№ РґР»СЏ РєСЂСѓРїРЅС‹С… РёРіСЂРѕРІС‹С… СЃР±РѕСЂРѕРє Рё РїРѕРґРґРµСЂР¶РёРІР°СЋС‰РёР№ РєРёСЂРёР»Р»РёС†Сѓ, Р°СЃРёРЅС…СЂРѕРЅРЅСѓСЋ РѕР±СЂР°Р±РѕС‚РєСѓ Рё СЂР°СЃС€РёСЂСЏРµРјСѓСЋ Р°СЂС…РёС‚РµРєС‚СѓСЂСѓ",
                    DescriptionEn = "A modern, reimagined player account manager for Freelancer Server, redesigned for large game builds, Cyrillic support, asynchronous processing, and an extensible architecture",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 40,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "РћС‚РєСЂС‹С‚СЊ СЂРµР»РёР·С‹",
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
                    DescriptionRu = "РђРІС‚РѕРјР°С‚РёС‡РµСЃРєРёР№ РјРѕРЅРёС‚РѕСЂРёРЅРі, РїРµСЂРµР·Р°РїСѓСЃРє Рё СѓРґР°Р»РµРЅРЅРѕРµ СѓРїСЂР°РІР»РµРЅРёРµ С‡РµСЂРµР· API РґР»СЏ РёРіСЂРѕРІС‹С… СЃРµСЂРІРµСЂРѕРІ Freelancer",
                    DescriptionEn = "Automatic monitoring, restart and remote API control for Freelancer game servers",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 50,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "РћС‚РєСЂС‹С‚СЊ СЂРµР»РёР·С‹",
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
                    DescriptionRu = "Р РµР°Р»РёР·Р°С†РёСЏ frc.exe (Freelancer Resource Compiler) РЅР° СЏР·С‹РєРµ C#. РџСЂРµРѕР±СЂР°Р·СѓРµС‚ RDL (XML) РІ С‚РµРєСЃС‚РѕРІС‹Р№ С„РѕСЂРјР°С‚ FRC, РёСЃРїРѕР»СЊР·СѓРµРјС‹Р№ РІРѕ РІСЂРµРјСЏ РІС‹РїРѕР»РЅРµРЅРёСЏ",
                    DescriptionEn = "C# implementation of frc.exe (Freelancer Resource Compiler). Converts RDL (XML) into FRC runtime text format",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 60,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "РћС‚РєСЂС‹С‚СЊ СЂРµР»РёР·С‹",
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
                    DescriptionRu = "Lizerium.Localization.Toolkit вЂ” СЌС‚Рѕ РёРЅСЃС‚СЂСѓРјРµРЅС‚ РґР»СЏ РѕСЂРіР°РЅРёР·Р°С†РёРё РїСЂРѕС†РµСЃСЃР° Р»РѕРєР°Р»РёР·Р°С†РёРё РІ .NET-РїСЂРѕРµРєС‚Р°С…, РіРґРµ РїРµСЂРµРІРѕРґС‹ С…СЂР°РЅСЏС‚СЃСЏ РІ С„Р°Р№Р»Р°С… .resx. РћРЅ РѕР±СЉРµРґРёРЅСЏРµС‚ РІ СЃРµР±Рµ Р·Р°РіСЂСѓР·РєСѓ СЂРµСЃСѓСЂСЃРѕРІ РІРѕ РІСЂРµРјСЏ РІС‹РїРѕР»РЅРµРЅРёСЏ, РіРµРЅРµСЂР°С‚РѕСЂ РєРѕРґР° РЅР° Р±Р°Р·Рµ Roslyn, РґРёР°РіРЅРѕСЃС‚РёРєСѓ СЃ РїРѕРјРѕС‰СЊСЋ Р°РЅР°Р»РёР·Р°С‚РѕСЂР°, СЃСЂРµРґСЃС‚РІР° Р°РІС‚РѕРјР°С‚РёС‡РµСЃРєРѕРіРѕ РёСЃРїСЂР°РІР»РµРЅРёСЏ РєРѕРґР° РІ Visual Studio Рё РѕС‚РґРµР»СЊРЅС‹Р№ СЂРµРґР°РєС‚РѕСЂ РЅР° Р±Р°Р·Рµ WPF",
                    DescriptionEn = "Lizerium.Localization.Toolkit is a .NET localization workflow for projects that store translations in .resx files. It combines runtime loading, a Roslyn source generator, analyzer diagnostics, Visual Studio code fixes, and a standalone WPF editor",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 70,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "РћС‚РєСЂС‹С‚СЊ СЂРµР»РёР·С‹",
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
                    DescriptionRu = "Р РµР°Р»РёР·Р°С†РёСЏ РЅР° C# Рё JavaScript РґР»СЏ РїСЂРµРѕР±СЂР°Р·РѕРІР°РЅРёСЏ Р±РёРЅР°СЂРЅС‹С… INI-С„Р°Р№Р»РѕРІ С„РѕСЂРјР°С‚Р° BINI (РёР· РёРіСЂС‹ Freelancer) РІ СЂРµРґР°РєС‚РёСЂСѓРµРјС‹Рµ С‚РµРєСЃС‚РѕРІС‹Рµ INI-С„Р°Р№Р»С‹ Рё РѕР±СЂР°С‚РЅРѕ",
                    DescriptionEn = "C#, JavaScript implementation for converting Freelancer BINI binary INI files into editable text INI files and back",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 80,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "Р”РµРјРѕРЅСЃС‚СЂР°С†РёСЏ",
                            NameEn = "Demo",
                            Url = "https://lizerium.github.io/Lizerium.BINI.Converter/",
                            IconUrl = "/img/Main.webp",
                            SortOrder = 10,
                            IsActive = true
                        },
                        new()
                        {
                            NameRu = "РћС‚РєСЂС‹С‚СЊ СЂРµР»РёР·С‹",
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
                    DescriptionRu = "Freelancer (2003): РџР°РЅРµР»СЊ СѓРїСЂР°РІР»РµРЅРёСЏ РёРЅС„РѕСЂРјР°С†РёРѕРЅРЅС‹РјРё РєР°СЂС‚РѕС‡РєР°РјРё РґР»СЏ Lizerium (СѓРЅРёРІРµСЂСЃР°Р»СЊРЅР°СЏ)",
                    DescriptionEn = "Freelancer (2003) Information Card Control Panel for Lizerium (Universal)",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 90,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "РћС‚РєСЂС‹С‚СЊ СЂРµР»РёР·С‹",
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
                    DescriptionRu = "FLHook Рё РЅР°Р±РѕСЂ РїР»Р°РіРёРЅРѕРІ РґР»СЏ СЃРµСЂРІРµСЂР° Freelancer (2003 Рі.)",
                    DescriptionEn = "FLHook and a collection of plugins for the Freelancer 2003 Server",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 100,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "РћС‚РєСЂС‹С‚СЊ СЂРµР»РёР·С‹",
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
                    DescriptionRu = "РљРѕРЅРІРµСЂС‚РµСЂ UTF РІ XML РґР»СЏ С„Р°Р№Р»РѕРІ РёРіСЂС‹ Freelancer (2003)",
                    DescriptionEn = "UTF to XML Converter for Freelancer (2003) game data",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 110,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "РћС‚РєСЂС‹С‚СЊ СЂРµР»РёР·С‹",
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
                    DescriptionRu = "Р РµРґР°РєС‚РѕСЂ Рё РёРЅСЃС‚СЂСѓРјРµРЅС‚ Р°РЅР°Р»РёР·Р° РіРµРѕРјРµС‚СЂРёРё Рё СЂРµСЃСѓСЂСЃРѕРІ (UTF / CMP / 3DB) РґР»СЏ Freelancer",
                    DescriptionEn = "Editor and analysis tool for geometry and resources (UTF / CMP / 3DB) for Freelancer",
                    IconUrl = "/img/Main.webp",
                    SortOrder = 120,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "РћС‚РєСЂС‹С‚СЊ СЂРµР»РёР·С‹",
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
                    DescriptionRu = "РЇ СЂР°Р·СЂР°Р±Р°С‚С‹РІР°СЋ СЃР»РѕР№ РІСЂРµРјРµРЅРё РІС‹РїРѕР»РЅРµРЅРёСЏ (runtime layer) РїРѕРІРµСЂС… РёРіСЂС‹ Freelancer (2003 РіРѕРґР°), РїРµСЂРµСЃРѕР±РёСЂР°СЏ Рё РїРѕРґРјРµРЅСЏСЏ СЃРёСЃС‚РµРјРЅС‹Рµ DLL, С‡С‚РѕР±С‹ Р±РµР·РѕРїР°СЃРЅРѕ РёСЃСЃР»РµРґРѕРІР°С‚СЊ, СЂР°СЃС€РёСЂСЏС‚СЊ Рё РґРѕРєСѓРјРµРЅС‚РёСЂРѕРІР°С‚СЊ РїРѕРІРµРґРµРЅРёРµ РѕСЂРёРіРёРЅР°Р»СЊРЅРѕРіРѕ РґРІРёР¶РєР°",
                    DescriptionEn = "I'm building a runtime layer on top of Freelancer (2003), rebuilding and overriding system DLLs to safely explore, extend, and document the behavior of the original engine",
                    IconUrl = "/img/social/binnexus.webp",
                    SortOrder = 130,
                    IsActive = true,
                    DownloadLinks = new List<ProductDownloadLinkDataResponse>
                    {
                        new()
                        {
                            NameRu = "Р”РµРјРѕРЅСЃС‚СЂР°С†РёСЏ",
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
