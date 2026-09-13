/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 13 сентября 2026 06:57:03
 * Version: 1.0.175
 */

using LizeriumUtilities.FormatsData.DataBase.Response;

namespace LizeriumServer.Models;

public class GameProductsViewModel
{
    public List<ProductCategoryDataResponse> Categories { get; init; } = new();
}
