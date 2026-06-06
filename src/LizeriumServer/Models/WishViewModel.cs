/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 06 июня 2026 09:06:57
 * Version: 1.0.71
 */

using LizeriumUtilities.FormatsData.AppUserData;
using LizeriumUtilities.FormatsData.DataBase.Response;

namespace LizeriumServer.Models;

/// <summary>
/// Модель Home[Index]
/// </summary>
public class WishViewModel
{
    /// <summary>
    /// Точка входа
    /// </summary>
    /// <param name="posts">Список постов</param>
    public WishViewModel(List<PostDataResponse> posts)
    {
        Posts = new DataPosts();
        Posts.Posts = new List<PostDataResponse>();
        if (posts == null || posts.Count == 0) return;
        Posts.Posts.AddRange(posts);
        Posts.LastUserId = Posts.Posts[^1].Id;
    }

    /// <summary>
    /// Список постов
    /// </summary>
    public DataPosts Posts { get; init; }
}
