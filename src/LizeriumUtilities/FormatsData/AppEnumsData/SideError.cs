/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 25 мая 2026 11:33:09
 * Version: 1.0.59
 */

namespace LizeriumUtilities.FormatsData.AppEnumsData;

/// <summary>
/// Перечисление на чьей стороне возникла ошибка
/// </summary>
public enum SideError
{
    UserSide = 1, //ошибка на стороне пользователя
    ServerSide = 2 //ошибка на стороне сервера
}