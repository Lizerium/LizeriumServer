/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 31 июля 2026 16:48:21
 * Version: 1.0.127
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