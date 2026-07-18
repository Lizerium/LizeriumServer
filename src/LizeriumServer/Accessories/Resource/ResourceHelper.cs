/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 18 июля 2026 07:48:27
 * Version: 1.0.112
 */

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using System.Reflection;

public class ResourceHelper
{
    private readonly IStringLocalizerFactory _localizerFactory;
    private readonly Assembly _assembly;

    public ResourceHelper(IStringLocalizerFactory localizerFactory, Assembly assembly = null)
    {
        _localizerFactory = localizerFactory;
        _assembly = assembly ?? Assembly.GetExecutingAssembly();
    }

    /// <summary>
    /// Получить все ключи/значения из ресурса View.
    /// resourceName = полное имя view без расширения
    /// Например: "Views.Knowledge.Article"
    /// </summary>
    public Dictionary<string, string> GetAllStrings(string resourceName)
    {
        var localizer = _localizerFactory.Create(resourceName, _assembly.GetName().Name);

        return localizer.GetAllStrings(includeParentCultures: true)
                        .ToDictionary(s => s.Name, s => s.Value);
    }
}