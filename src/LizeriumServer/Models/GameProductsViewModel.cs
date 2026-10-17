/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 17 октября 2026 07:43:37
 * Version: 1.0.208
 */

using LizeriumUtilities.FormatsData.DataBase.Response;

namespace LizeriumServer.Models;

public class GameProductsViewModel
{
    public List<ProductCategoryDataResponse> Categories { get; init; } = new();
}
