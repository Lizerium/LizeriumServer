/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 26 сентября 2026 11:58:44
 * Version: 1.0.188
 */

using LizeriumUtilities.FormatsData.DataBase.Response;

namespace LizeriumServer.Models;

public class GameProductsViewModel
{
    public List<ProductCategoryDataResponse> Categories { get; init; } = new();
}
