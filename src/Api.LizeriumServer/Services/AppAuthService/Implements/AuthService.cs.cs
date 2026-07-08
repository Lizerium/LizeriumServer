/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 июля 2026 07:48:37
 * Version: 1.0.102
 */

using LizeriumLogging.Accessories.LoggingAccessories;

namespace Api.LizeriumServer.Services.AppAuthService.Implements;

/// <summary>
/// Реализация интерфейса авторизации
/// </summary>
public class AuthService : IAuthService
{
    /// <summary>
    /// Коллекция данных администраторов (secretKey => emailAdmin)
    /// </summary>
    private static Dictionary<string, string> _administrators;

    /// <summary>
    /// Обертка коллекции данных администратора
    /// </summary>
    private static Dictionary<string, string> Administrators => _administrators ??= GenerateAdminData();

    /// <summary>
    /// Метод генерирует коллекцию данных администратора
    /// </summary>
    /// <returns>Коллекция данных администратора</returns>
    private static Dictionary<string, string> GenerateAdminData()
    {
        try
        {
            //получаем из файла конфигурации секцию
            var dataSecretRecords = Program.Configuration.GetSection("admins");

            //обходим дочерние данные
            var administrators = dataSecretRecords.GetChildren().ToDictionary(
                dataSecretRecord => dataSecretRecord.GetValue<string>("secretKey"),
                dataSecretRecord => dataSecretRecord.GetValue<string>("emailAdmin"));

            //отдаем коллекцию данных администраторов
            return administrators;
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем null
            return null;
        }
    }

    /// <summary>
    /// Метод проверяет валидность секретного ключа авторизации
    /// </summary>
    /// <param name="secretKey">Секретный ключ авторизации</param>
    /// <returns>Результат проверки</returns>
    public bool IsValidSecretKey(string secretKey)
    {
        try
        {
            //проверяем входящие данные и наличие секретного ключа в коллекции
            return !string.IsNullOrEmpty(secretKey) && Administrators.ContainsKey(secretKey);
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем секретный ключ не валиден
            return false;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Метод отдаем Email администратора
    /// </summary>
    /// <param name="secretKey">Секретный ключ авторизации</param>
    /// <returns>Email администратора</returns>
    public string GetEmailAdmin(string secretKey)
    {
        try
        {
            //проверяем наличие ключа и отдаем имя
            return Administrators.ContainsKey(secretKey) ? Administrators[secretKey] : null;
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем null
            return null;
        }
    }
}