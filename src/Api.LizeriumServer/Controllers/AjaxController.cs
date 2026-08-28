/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 28 августа 2026 07:12:37
 * Version: 1.0.160
 */

using System;
using System.Text.Json;

using Api.LizeriumServer.FormatsData.AppAdminData;
using Api.LizeriumServer.Services.AdminAccess;
using Api.LizeriumServer.Services.AppAuthService;
using Api.LizeriumServer.Services.AppAuthService.Implements;

using LizeriumDatabase.Accessories.DataBaseAccessories;
using LizeriumDatabase.Services.AppDataBaseService;

using LizeriumLogging.Accessories.LoggingAccessories;

using LizeriumNetSecurity.Services.SecurityService;

using LizeriumUtilities.Accessories.JsonAccessories;
using LizeriumUtilities.Accessories.SessionAccessories;
using LizeriumUtilities.FormatsData.AppRequestData.Admin.AjaxData;
using LizeriumUtilities.FormatsData.DataBase.Requests;

using Microsoft.AspNetCore.Mvc;

using TranslationService;

namespace Api.LizeriumServer.Controllers;

/// <summary>
/// Контроллер Ajax запросов с сайта
/// </summary>
[Route("[controller]/[action]")]
public class AjaxController : Controller
{
    private IDataBaseService appDb { get; set; }
    private IConfiguration configuration { get; set; }
    private IAppSecurityService securityService { get; set; }
    private ITranslationService translationService { get; set; }

    public AjaxController(IDataBaseService dataBaseService, IConfiguration configuration, 
        IAppSecurityService appSecurityService, ITranslationService translationService)
    {
        appDb = dataBaseService;
        this.configuration = configuration;
        securityService = appSecurityService;
        this.translationService = translationService;
    }

    /// <summary>
    /// Метод получает секретный ключ авторизации
    /// </summary>
    /// <param name="requestAuth">Объект запроса авторизации</param>
    /// <returns>Результат действия</returns>
    [HttpPost]
    [Consumes("application/json")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Auth([FromBody] RequestAuth requestAuth)
    {
        try
        {
            if (!AdminAccessGuard.IsAllowed(HttpContext))
                return StatusCode(404);

            //проверяем блокировку
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
                return StatusCode(403);

            //проверяем валидность входящих данных
            if (!ModelState.IsValid) return BadRequest("failed");

            //создаем экземпляр интерфейса авторизации
            IAuthService authService = new AuthService();

            //если секретный ключ авторизации не валиден, отдаем BadRequest
            if (!authService.IsValidSecretKey(requestAuth.SecretKey)) return BadRequest("bad secret key");

            //создаем объект данных сессии администратора
            var adminSession = new AdminSession
            {
                EmailAdmin = authService.GetEmailAdmin(requestAuth.SecretKey),
                OnceCode = 2606971, //CryptoRandom.GetRandomInt(1000000, 9999999),
                SentOnceCode = true,
                IsConfirmed = true,
            };

            //ставим сессию администратора
            HttpContext.Session.SetSession("admin", adminSession);
            await HttpContext.Session.CommitAsync();

            //если debug
            if (!Program.SettingsApp.IsRelease)
            {
                //выводим код подтверждения на консоль
                Console.WriteLine(adminSession.OnceCode);
            }
            else
            {
                ////инициализируем экземпляр интерфейса отправки Email
                //IEmailService emailService = new EmailService();

                ////отправляем Email администратору с кодом подтверждения авторизации
                //await emailService.SendEmailAsync(new EmailData
                //{
                //    EmailType = TypeEmail.ConfitmationCodeAdminAuth,
                //    Recipient = adminSession.EmailAdmin,
                //    SubjectEmail = "Разовый код авторизации администратора",
                //    Message = $"Код подтверждения авторизации: {adminSession.OnceCode}"
                //});
            }

            //отдаем ok
            return Ok("ok");
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем BadRequest
            return BadRequest("failed");
        }
    }

    /// <summary>
    /// Загрузка гифки для команды
    /// </summary>
    /// <param name="file"></param>
    [HttpPost]
    public async Task<IActionResult> UploadGifCommand([FromForm] IFormFile file, string id)
    {
        //проверяем блокировку
        var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
        if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
            return StatusCode(403);

        var gifPaths = DatabaseExtensions.Configuration.GetValue<string>("GifPath");

        // Проверка, был ли загружен файл
        if (file == null || file.Length == 0 || !file.FileName.EndsWith(".gif"))
        {
            return BadRequest("Файл не был загружен или это не GIF-файл.");
        }

        // Проверяем, является ли файл GIF
        if (!file.ContentType.Equals("image/gif", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Неверный тип файла. Допустимо только GIF.");
        }

        // Проверяем размер файла
        if (file.Length > 50 * 1024 * 1024) // 50 МБ
        {
            return BadRequest("Размер файла превышает допустимый лимит (50 МБ).");
        }

        // Создаем путь для сохранения файла
        string fileName = Path.GetFileName(file.FileName);

        // Создаем папку для сохранения, если она еще не существует
        if (!Directory.Exists(gifPaths))
        {
            Directory.CreateDirectory(gifPaths);
        }

        // Сохраняем файл на сервер
        string filePath = Path.Combine(gifPaths, fileName);
        using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        // сохраняем файл в бд
        await appDb.SaveGifCommandAsync(fileName, id);


        // Возвращаем результат
        return Ok("Файл GIF успешно загружен.");
    }

    /// <summary>
    /// Загрузка сформированного списка команд на сервер через админку в формате JSON
    /// </summary>
    /// <param name="file">JSON</param>
    [HttpPost]
    public async Task<IActionResult> UploadCommandsJson([FromForm] IFormFile file)
    {
        //проверяем блокировку
        var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
        if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
            return StatusCode(403);

        // Проверка, был ли загружен файл
        if (file == null || file.Length == 0 || !file.FileName.EndsWith(".json"))
        {
            return BadRequest("Файл не был загружен или это не JSON-файл.");
        }

        // Чтение содержимого JSON-файла
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            string jsonContent = await reader.ReadToEndAsync();

            // Десериализация JSON
            try
            {
                var jsonData = JsonSerializer.Deserialize<CommandsFileRequest>(jsonContent);

                //Оброботка и сохранение в БД
                await appDb.SaveCommandsFromJsonAsync(jsonData);

                return Ok("JSON-файл загружен и обработан успешно.");
            }
            catch (JsonException ex)
            {
                return BadRequest($"Ошибка при десериализации JSON: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Загрузка сформированного списка категорий команд на сервер через админку в формате JSON
    /// </summary>
    /// <param name="file">JSON</param>
    [HttpPost]
    public async Task<IActionResult> UploadCategoriesCommandsJson([FromForm] IFormFile file)
    {
        //проверяем блокировку
        var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
        if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
            return StatusCode(403);

        // Проверка, был ли загружен файл
        if (file == null || file.Length == 0 || !file.FileName.EndsWith(".json"))
        {
            return BadRequest("Файл не был загружен или это не JSON-файл.");
        }

        // Чтение содержимого JSON-файла
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            string jsonContent = await reader.ReadToEndAsync();

            // Десериализация JSON
            try
            {
                var jsonData = JsonSerializer.Deserialize<CommandsFileRequest>(jsonContent);

                //Оброботка и сохранение в БД
                await appDb.SaveCategoriesCommandsFromJsonAsync(jsonData);

                return Ok("JSON-файл загружен и обработан успешно.");
            }
            catch (JsonException ex)
            {
                return BadRequest($"Ошибка при десериализации JSON: {ex.Message}");
            }
        }
    }


    /// <summary>
    /// Перевод всех команд на другой язык и фиксации в таблице переводов
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("/Admin/TranslatedAllCommands")]
    public async Task<IActionResult> TranslatedAllCommands(string langFrom = "ru", string langTo = "en")
    {
        try
        {
            //проверяем блокировку
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
                return StatusCode(403);

            // проверяем валидность входящих данных
            if (!ModelState.IsValid) return BadRequest("failed");

            // получаем объект сессии администратора
            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");

            // если сессии нет или администратор не авторизован, отдаем BadRequest
            if (adminSession is not { IsAuth: true }) return BadRequest("need authorization");

            if(!await translationService.CheckConnectionAsync())
            {
                return BadRequest("failed connection to translater");
            }

            // получить команды без перевода на en с оригианльным описанием ru в главной таблице
            var getCommandsMissingTranslate = await appDb.GetCommandsMissingTranslationAsync(langTo);

            foreach (var cmd in getCommandsMissingTranslate)
            {
                var translatedText = await translationService.TranslateAsync(cmd.Description, "ru", "en");
                cmd.Description = translatedText;
            }

            var result = await appDb.SaveAllCommandsTranslationsAsync(getCommandsMissingTranslate);

            // отдаем успешный ответ
            return Ok(result);
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем BadRequest
            return BadRequest("failed");
        }
    }

    /// <summary>
    /// Метод сохраняет команду
    /// </summary>
    /// <param name="requestCommand">Объект данных о команде</param>
    /// <returns>Результат действия</returns>
    [HttpPost]
    [Consumes("application/json")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCommand([FromBody] RequestSaveCommand requestCommand)
    {
        try
        {
            //проверяем блокировку
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
                return StatusCode(403);

            // проверяем валидность входящих данных
            if (!ModelState.IsValid) return BadRequest("failed");

            // получаем объект сессии администратора
            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");

            // если сессии нет или администратор не авторизован, отдаем BadRequest
            if (adminSession is not { IsAuth: true }) return BadRequest("need authorization");

            // сохраняю команду
            var dataCommand = await appDb.AddCommandAsync(new CreateCommandViewRequest()
            {
                Category = requestCommand.Category,
                CommandNames = requestCommand.CommandNames,
                ExampleInput = requestCommand.ExampleInput,
                CountLike = requestCommand.CountLike,
                Description = requestCommand.Description,
                Status = requestCommand.Status,
                UrlGif = requestCommand.UrlGif
            });

            // проверяем данные пользователей
            if (dataCommand == false) return BadRequest("error update status");

            // отдаем успешный ответ
            return Ok("ok");
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем BadRequest
            return BadRequest("failed");
        }
    }

    /// <summary>
    /// Метод удаляет команду и её переводы
    /// </summary>
    /// <param name="requestCommand">Объект данных о команде</param>
    /// <returns>Результат действия</returns>
    [HttpPost]
    [Consumes("application/json")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCommand([FromBody] RequestSaveCommand requestCommand)
    {
        try
        {
            //проверяем блокировку
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
                return StatusCode(403);

            // проверяем валидность входящих данных
            if (!ModelState.IsValid) return BadRequest("failed");

            // получаем объект сессии администратора
            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");

            // если сессии нет или администратор не авторизован, отдаем BadRequest
            if (adminSession is not { IsAuth: true }) return BadRequest("need authorization");

            // сохраняю команду
            var dataCommand = await appDb.DeleteCommandAndTranslationsAsync(new CreateCommandViewRequest()
            {
                Id = requestCommand.Id,
                Category = requestCommand.Category,
                CommandNames = requestCommand.CommandNames,
                ExampleInput = requestCommand.ExampleInput,
                CountLike = requestCommand.CountLike,
                Description = requestCommand.Description,
                Status = requestCommand.Status,
                UrlGif = requestCommand.UrlGif
            });

            // проверяем данные пользователей
            if (dataCommand == false) return BadRequest("error delete status");

            // отдаем успешный ответ
            return Ok("ok");
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем BadRequest
            return BadRequest("failed");
        }
    }

    /// <summary>
    /// Метод сохраняет команду
    /// </summary>
    /// <param name="requestCommand">Объект данных о команде</param>
    /// <returns>Результат действия</returns>
    [HttpPost]
    [Consumes("application/json")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCommand([FromBody] RequestSaveCommand requestCommand)
    {
        try
        {
            //проверяем блокировку
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
                return StatusCode(403);

            // проверяем валидность входящих данных
            if (!ModelState.IsValid) return BadRequest("failed");

            // получаем объект сессии администратора
            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");

            // если сессии нет или администратор не авторизован, отдаем BadRequest
            if (adminSession is not { IsAuth: true }) return BadRequest("need authorization");

            var msg = $"{requestCommand.Id} change...";
            msg.LogMessage();

            // сохраняю команду
            var dataCommand = await appDb.ChangeCommandAsync(new CreateCommandViewRequest()
            {
                Id = requestCommand.Id,
                Category = requestCommand.Category,
                CommandNames = requestCommand.CommandNames,
                ExampleInput = requestCommand.ExampleInput,
                CountLike = requestCommand.CountLike,
                Description = requestCommand.Description,
                Status = requestCommand.Status,
                UrlGif = requestCommand.UrlGif
            });

            // проверяем данные пользователей
            if (dataCommand == false) return BadRequest("error update status");

            var msgOk = $"{requestCommand.Id} change OK...";
            msgOk.LogMessage();

            // отдаем успешный ответ
            return Ok("ok");
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем BadRequest
            return BadRequest("failed");
        }
    }

    /// <summary>
    /// Метод получает запись подтверждения авторизации
    /// </summary>
    /// <param name="requestConfirm">Объект запроса подтверждения авторизации</param>
    [HttpPost]
    [Consumes("application/json")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm([FromBody] RequestConfirm requestConfirm)
    {
        try
        {
            //проверяем блокировку
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
                return StatusCode(403);

            //проверяем валидность входящих данных
            if (!ModelState.IsValid) return BadRequest("failed");

            //получаем объект сессии администратора
            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");

            //проверяем объект сессии и сверяем разовые коды
            if (adminSession == null || requestConfirm.OnceCode < 1 || adminSession.OnceCode != requestConfirm.OnceCode)
            {
                //удаляем сессию администратора
                HttpContext.Session.Clear();

                //отдаем BadRequest
                return BadRequest("failed");
            }

            //ставим в сессии что разовый код подтвержден
            adminSession.IsConfirmed = true;

            //ставим сессию администратора
            HttpContext.Session.SetSession("admin", adminSession);

            //логируем вход администратора
            $"Авторизация администратора: {adminSession.EmailAdmin}".LogMessage();

            //отдаем ok
            return Ok("ok");
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем BadRequest
            return BadRequest("failed");
        }
    }

    /// <summary>
    /// Метод обновляет статус заявки пользователя
    /// </summary>
    /// <param name="lastUserId">Идентификатор пользователя</param>
    /// <param name="lastUserId">Идентификатор статуса</param>
    [HttpPost]
    [Route("{lastUserId:long}/{status:int}")]
    [Produces("application/json")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatusPost(long lastUserId, int status)
    {
        try
        {
            //проверяем блокировку
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
                return StatusCode(403);

            //получаем объект сессии администратора
            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");

            //если сессии нет или администратор не авторизован, отдаем BadRequest
            if (adminSession is not { IsAuth: true }) return BadRequest("need authorization");

            //получаем данные пользователей
            var dataUsers = await appDb.UpdateStatusPostAsync(lastUserId, status);

            //проверяем данные пользователей
            if (dataUsers == false) return BadRequest("error update status");

            //отдаем данные пользователей
            return dataUsers.SuccessResponse();
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем BadRequest
            return BadRequest("failed");
        }
    }

    /// <summary>
    /// Метод отдает данные постов пользователей в админку
    /// </summary>
    /// <param name="lastUserId">Идентификатор крайнего пользователя</param>
    /// <returns>Результат действия</returns>
    [HttpPost]
    [Route("{lastUserId:long}/{status:int}/{scroll:int}")]
    [Produces("application/json")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetPosts(long lastUserId, int status, int scroll)
    {
        try
        {
            //проверяем блокировку
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
                return StatusCode(403);

            //получаем объект сессии администратора
            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");

            //если сессии нет или администратор не авторизован, отдаем BadRequest
            if (adminSession is not { IsAuth: true }) return BadRequest("need authorization");

            //получаем данные постов пользователей
            var dataUsers = await appDb.GetAllPostsAsync((int)lastUserId, status, (scroll == 0) ? false : true);

            //проверяем данные пользователей
            if (dataUsers == null) return BadRequest("error get users");

            if (dataUsers.Posts.Count <= 0) dataUsers.LastUserId = 0;
            else dataUsers.LastUserId = dataUsers.Posts[^1].Id;

            //отдаем данные пользователей
            return dataUsers.SuccessResponse();
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем BadRequest
            return BadRequest("failed");
        }
    }
}
