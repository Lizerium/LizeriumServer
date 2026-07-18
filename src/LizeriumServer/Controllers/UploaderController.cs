/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 18 июля 2026 07:48:27
 * Version: 1.0.112
 */

using LizeriumServer.FormatsData.AppUploaderGameData;
using LizeriumServer.Options;

using LizeriumUtilities.Services.DownloadLinksService;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace LizeriumServer.Controllers
{
    [ApiController]
    [Route("uploader")]
    public class UploaderController : Controller
    {
        private IWebHostEnvironment _environment;
        private readonly StoragePathsOptions _storagePaths;
        private readonly ILogger<UploaderController> _logger;
        private readonly DownloadLinksService _downloadLinksService;

        public UploaderController(IWebHostEnvironment environment, 
            ILogger<UploaderController> logger,
            DownloadLinksService downloadLinksService,
            IOptions<StoragePathsOptions> storagePathsOptions)
        {
            _environment = environment;
            _logger = logger;
            _downloadLinksService = downloadLinksService;
            _storagePaths = storagePathsOptions.Value;
        }

        [HttpGet]
        [Route("games")]
        public async Task<IActionResult> GetGames()
        {
            var games = new UploaderGames();
            games.GamesList.Add(new UploaderGame()
            {
                Id = 1,
                Title = "Lizerium Mode",
                Description = "Реинкарнация игры Freelancer по мотивам модов Freelancer Discovery, Orion, Rebirth, Crosswords, Sirius, Nomad и множества других.",
                Urls = new List<string>() { "" }
            });
            games.GamesList.Add(new UploaderGame()
            {
                Id = 1,
                Title = "Lizerium Unity",
                Description = "Собственная реализация космического симулятора вдохновлённая такими проектами как EVE Online, " +
                "No Mans Sky и конечно же Freelancer.",
                Urls = new List<string>() { "" }
            });
            string json = JsonConvert.SerializeObject(games);
            return Content(json);
        }

        [HttpGet("{NameMode}/version")]
        public async Task<IActionResult> GetModeVersion(string NameMode)
        {
            //https://localhost:7176/uploader/LizeriumFreelancerMode/version
            var modsDir = Path.Combine(_storagePaths.ModsRoot);
            //получают папку с модами
            var Mods = Directory.GetDirectories(modsDir);

            bool existMode = Mods.Any(it =>
                {
                    var spl = it.Split('\\').ToList();
                    if(spl.Last().Contains(NameMode))
                        return true;
                    else return false;
                }
            );

            if(!existMode)
                return NotFound();

            var fileName = "version.xml";
            var fileVersionDir = Path.Combine(modsDir, NameMode, fileName);

            // Проверка существования файла
            if (!System.IO.File.Exists(fileVersionDir))
                return NotFound();

            var fs = new FileStream(fileVersionDir, FileMode.Open);
            return File(fs, "application/octet-stream", fileName);
        }

        [HttpGet("{NameMode}/updates")]
        public async Task<IActionResult> GetModeUpdatesAsync(string NameMode)
        {
            //https://localhost:7176/uploader/LizeriumFreelancerMode/version
            var modsDir = Path.Combine(_storagePaths.ModsRoot);
            //получают папку с модами
            var Mods = Directory.GetDirectories(modsDir);

            bool existMode = Mods.Any(it =>
            {
                var spl = it.Split('\\').ToList();
                if (spl.Last().Contains(NameMode))
                    return true;
                else return false;
            }
            );

            if (!existMode)
                return NotFound();

            var fileName = "version.xml";
            var fileVersionDir = Path.Combine(modsDir, NameMode, fileName);

            // Проверка существования файла
            if (!System.IO.File.Exists(fileVersionDir))
                return NotFound();

            var fs = new FileStream(fileVersionDir, FileMode.Open);
            return File(fs, "application/octet-stream", fileName);
        }

        [HttpGet("ping")]
        public async Task<IActionResult> PingAsync()
        {
            try
            {
                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet("mod/{NameMode}/{version}/{fileName}")]
        public async Task<IActionResult> GetModeManifestAsync(string NameMode, string version, string fileName)
        {
            try
            {
                //https://localhost:7176/uploader/mod/LizeriumFreelancerMode/99.3.4/manifest.xml
                var searchPath = string.Empty;

                if (string.IsNullOrEmpty(NameMode))
                    return NotFound();
                if (string.IsNullOrEmpty(version))
                    return NotFound();
                if (string.IsNullOrEmpty(fileName))
                    return NotFound();

                searchPath = Path.Combine(_storagePaths.ModsRoot, NameMode, version);
                if (!Directory.Exists(searchPath)) return NotFound();
                var files = Directory.GetFiles(searchPath);
                bool exist = false;

                foreach (var file in files)
                {
                    if (Path.GetFileName(file).Equals(fileName))
                    {
                        exist = true;
                        break;
                    }
                }

                // Проверка валидности запрашиваемых файлов
                if (!exist) return NotFound();

                var path = Path.Combine(searchPath, fileName); // get manifest.xml
                var fs = new FileStream(path, FileMode.Open);
                return File(fs, "application/octet-stream", fileName);
            }
            catch 
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("download/{NameMode}/{version}/{fileName}")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> GetRootApp(string NameMode, string version, string fileName)
        {
            //https://localhost:7176/uploader/download/LizeriumFreelancerMode/99.3.4/LizeriumFLInstaller-1a.bin.deploy
            var searchPath = string.Empty;

            if (string.IsNullOrEmpty(NameMode))
                return NotFound();
            if (string.IsNullOrEmpty(version))
                return NotFound();
            if (string.IsNullOrEmpty(fileName))
                return NotFound();

            searchPath = Path.Combine(_storagePaths.ModsRoot, NameMode, version);
            var files = Directory.GetFiles(searchPath);
            bool exist = false;

            foreach (var file in files)
            {
                if (Path.GetFileName(file).Equals(fileName))
                {
                    exist = true;
                    break;
                }
            }

            // Проверка валидности запрашиваемых файлов
            if (!exist) return NotFound();

            var path = Path.Combine(searchPath, fileName);
            var fs = new FileStream(path, FileMode.Open);
            return File(fs, "application/octet-stream", fileName);
        }

        [HttpGet]
        [Route("download/{NameMode}/{version}/Redist/{fileName}")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> GetRootFolderApp(string NameMode, string version, string fileName)
        {
            //https://localhost:7176/uploader/download/LizeriumFreelancerMode/99.3.4/LizeriumFLInstaller-1a.bin.deploy
            var searchPath = string.Empty;

            if (string.IsNullOrEmpty(NameMode))
                return NotFound();
            if (string.IsNullOrEmpty(version))
                return NotFound();
            if (string.IsNullOrEmpty(fileName))
                return NotFound();

            searchPath = Path.Combine(_storagePaths.ModsRoot, NameMode, version, "Redist");
            var files = Directory.GetFiles(searchPath);
            bool exist = false;

            foreach (var file in files)
            {
                if (Path.GetFileName(file).Equals(fileName))
                {
                    exist = true;
                    break;
                }
            }

            // Проверка валидности запрашиваемых файлов
            if (!exist) return NotFound();

            var path = Path.Combine(searchPath, fileName);
            var fs = new FileStream(path, FileMode.Open);
            return File(fs, "application/octet-stream", fileName);
        }

        /// <summary>
        /// Качает обновление для мода если у того версия отличается от первоначальной у клиента
        /// </summary>
        /// <param name="NameMode">Имя мода</param>
        /// <param name="version">Версия обновлений (от версии установщика отличается) (1.0.0 -> превратится в 1.0.0.rar) </param>
        [HttpGet]
        [Route("download/u/{NameMode}/{version}/updates/")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> GetUpdateToApp(string NameMode, string version)
        {
            //https://localhost:7176/uploader/download/LizeriumFreelancerMode/99.3.4/LizeriumFLInstaller-1a.bin.deploy
            if (string.IsNullOrEmpty(NameMode) || string.IsNullOrEmpty(version))
                return NotFound();

            var searchPath = Path.Combine(_storagePaths.ModsRoot, NameMode, "updates");
            var fileName = version + ".tar";
            var fullPath = Path.Combine(searchPath, fileName);

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            const int bufferSize = 1024 * 1024 * 4; // 4MB

            var fileInfo = new FileInfo(fullPath);

            Response.ContentType = "application/octet-stream";
            Response.ContentLength = fileInfo.Length;
            Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");

            await using var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.SequentialScan);
            await fs.CopyToAsync(Response.Body, bufferSize);

            return new EmptyResult();
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetVersionAsync(string fileName)
        {
            bool versionXML = fileName.Equals("version.xml", StringComparison.OrdinalIgnoreCase);
            // Проверка валидности запрашиваемых файлов
            if (!versionXML)
            {
                return NotFound();
            }

            var path = Path.Combine(_environment.WebRootPath, fileName);

            // Проверка существования файла
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }

            var fs = new FileStream(path, FileMode.Open);
            return File(fs, "application/octet-stream", fileName);
        }

        [HttpGet]
        [Route("{version}/{fileName}")]
        public async Task<IActionResult> GetRootApp(string version, string fileName)
        {
            var searchPath = string.Empty;

            if (string.IsNullOrEmpty(version))
                return NotFound();
            if (string.IsNullOrEmpty(fileName))
                return NotFound();

            searchPath = Path.Combine(_environment.WebRootPath, version);
            var files = Directory.GetFiles(searchPath);
            bool exist = false;

            foreach (var file in files)
            {
                if (Path.GetFileName(file).Equals(fileName))
                {
                    exist = true;
                    break;
                }
            }

            // Проверка валидности запрашиваемых файлов
            if (!exist) return NotFound();

            var path = Path.Combine(searchPath, fileName);
            var fs = new FileStream(path, FileMode.Open);
            return File(fs, "application/octet-stream", fileName);
        }

        [HttpGet]
        [Route("{version}/{versionApp}/{fileName}")]
        public async Task<IActionResult> GetAppConfigFiles(string version,
            string versionApp, string fileName)
        {
            var searchPath = string.Empty;

            if (string.IsNullOrEmpty(version))
                return NotFound();
            if (string.IsNullOrEmpty(versionApp))
                return NotFound();
            if (string.IsNullOrEmpty(fileName))
                return NotFound();

            searchPath = Path.Combine(_environment.WebRootPath, version, versionApp);
            var files = Directory.GetFiles(searchPath);
            bool exist = false;

            foreach (var file in files)
            {
                if (Path.GetFileName(file).Equals(fileName))
                {
                    exist = true;
                    break;
                }
            }

            // Проверка валидности запрашиваемых файлов
            if (!exist) return NotFound();

            var path = Path.Combine(searchPath, fileName);
            var fs = new FileStream(path, FileMode.Open);
            return File(fs, "application/octet-stream", fileName);
        }

        [HttpGet]
        [Route("{version}/{versionApp}/{versionData}/{fileName}")]
        public async Task<IActionResult> GetAppDataFiles(string version,
            string versionApp, string versionData, string fileName)
        {
            var searchPath = string.Empty;

            if (string.IsNullOrEmpty(version))
                return NotFound();
            if (string.IsNullOrEmpty(versionApp))
                return NotFound();
            if (string.IsNullOrEmpty(versionData))
                return NotFound();
            if (string.IsNullOrEmpty(fileName))
                return NotFound();

            searchPath = Path.Combine(_environment.WebRootPath, version, versionApp, versionData);
            var files = Directory.GetFiles(searchPath);
            bool exist = false;

            foreach (var file in files)
            {
                if (Path.GetFileName(file).Equals(fileName))
                {
                    exist = true;
                    break;
                }
            }

            // Проверка валидности запрашиваемых файлов
            if (!exist) return NotFound();

            var path = Path.Combine(searchPath, fileName);
            var fs = new FileStream(path, FileMode.Open);
            return File(fs, "application/octet-stream", fileName);
        }

        [HttpGet]
        [Route("projects/download/{key}")]
        public async Task<IActionResult> DownloadLauncher(string key)
        {
            var entry = _downloadLinksService.GetLink(key);
            if (entry == null) return NotFound();

            if (entry.Type == "external")
            {
                // лог перехода
                var logPath = Path.Combine(_environment.ContentRootPath, "download_projects.ini");
                var logLine = $"{DateTime.UtcNow:u} | {HttpContext.Connection.RemoteIpAddress} | скачал {key} | {entry.Value}";
                await System.IO.File.AppendAllTextAsync(logPath, logLine + Environment.NewLine);

                // редиректим на внешний сайт
                return Redirect(entry.Value);
            }

            if (entry.Type == "local")
            {
                var searchPath = Path.Combine(_storagePaths.LauncherRoot);
                var filePath = Path.Combine(searchPath, entry.Value);

                if (!System.IO.File.Exists(filePath))
                    return NotFound();

                // лог скачивания
                var logPath = Path.Combine(_environment.ContentRootPath, "download_steam.ini");
                var logLine = $"{DateTime.UtcNow:u} | {HttpContext.Connection.RemoteIpAddress} | скачал {key} |{entry.Value}";
                await System.IO.File.AppendAllTextAsync(logPath, logLine + Environment.NewLine);

                var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(fs, "application/octet-stream", entry.Value);
            }

            return BadRequest("Unknown link type");
        }
    }
}
