/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 02 мая 2026 19:36:20
 * Version: 1.0.37
 */

namespace LizeriumUtilities.FormatsData.AppUserData;

/// <summary>
/// Объект данных о ключе API пользователя
/// </summary>
public class UserApiKeyData
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public long IdUser { get; init; }

    /// <summary>
    /// Ключ API пользователя
    /// </summary>
    public string ApiKey { get; init; }

    /// <summary>
    /// Флаг валидности данных
    /// </summary>
    public bool ValidData => IdUser > 0 && !string.IsNullOrEmpty(ApiKey);
}
