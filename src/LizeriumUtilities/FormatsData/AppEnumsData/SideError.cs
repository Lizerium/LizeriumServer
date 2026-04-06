/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 06 апреля 2026 13:03:28
 * Version: 1.0.8
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