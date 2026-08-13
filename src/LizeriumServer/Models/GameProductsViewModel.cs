/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 13 августа 2026 07:11:18
 * Version: 1.0.144
 */

using LizeriumUtilities.FormatsData.DataBase.Response;

namespace LizeriumServer.Models;

public class GameProductsViewModel
{
    public List<ProductCategoryDataResponse> Categories { get; init; } = new();
}
