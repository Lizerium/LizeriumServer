/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 11 апреля 2026 13:54:51
 * Version: 1.0.13
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
