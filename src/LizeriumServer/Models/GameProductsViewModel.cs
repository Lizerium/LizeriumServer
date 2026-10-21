/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 21 октября 2026 10:10:52
 * Version: 1.0.212
 */

using LizeriumUtilities.FormatsData.DataBase.Response;

namespace LizeriumServer.Models;

public class GameProductsViewModel
{
    public List<ProductCategoryDataResponse> Categories { get; init; } = new();
}
