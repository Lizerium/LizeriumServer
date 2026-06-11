/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 11 июня 2026 13:47:38
 * Version: 1.0.76
 */

using System.Collections.Concurrent;
using System.Net;

using LizeriumLogging.Accessories.LoggingAccessories;

using LizeriumNetSecurity.Services.SecurityService;

using Microsoft.Extensions.Configuration;

namespace LizeriumUtilities.Services.SecurityService.Implements;

public class AppSecurityService : IAppSecurityService
{
    private readonly string _path;
    private readonly ConcurrentDictionary<string, byte> _blackList = new();
    private readonly SemaphoreSlim _lock = new(1, 1);
    private bool _initialized = false;

    private bool _pendingSave = false;
    private int _blockedCount = 0;

    /// <summary>
    /// Конструктор
    /// Ремарка:
    /// Ключ в appsettings.json: "BlackList": "path/to/configSecurity.ini",
    /// </summary>
    /// <param name="config">Конфигурация appsettings.json - путь до файла с ограничениями входа</param>
    public AppSecurityService(IConfiguration config)
    {
        _path = config.GetValue<string>("BlackList");
    }

    public Task<bool> IsBlocked(string ip) =>
        Task.FromResult(_blackList.ContainsKey(ip));

    public async Task EnsureInitializedAsync()
    {
        if (_initialized) return;

        await _lock.WaitAsync();
        try
        {
            if (_initialized) return; // повторная проверка после захвата блокировки

            await ReloadAsync();

            _initialized = true;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task AddIpAsync(string ip)
    {
        // пропускаем все запросы если сервис неправильно настроен (или не настроен вообще)
        if (!EnsureFileExistsOrLog()) return;

        await _lock.WaitAsync();

        try
        {
            if (!IPAddress.TryParse(ip, out _))
                throw new ArgumentException("Invalid IP address format.");
            
            if (_blackList.ContainsKey(ip)) return;

            // запись в память
            if (_blackList.TryAdd(ip, 0))
            {
                _pendingSave = true;
            }
            if (_blockedCount % 100 == 0)
                $"Заблокировано {_blockedCount} IP".LogMessage();
        }
        catch (Exception ex)
        {
            ex.LogException();
        }
        finally 
        { 
            _lock.Release(); 
        }
    }

    public async Task RemoveIpAsync(string ip)
    {
        // пропускаем все запросы если сервис неправильно настроен (или не настроен вообще)
        if(!EnsureFileExistsOrLog()) return;
      
        await _lock.WaitAsync();

        if (!_blackList.ContainsKey(ip)) return;

        // запись в память
        _blackList.TryRemove(ip, out _);
        _pendingSave = true;

        $"{ip} - разблокирован".LogMessage();
    }

    public async Task ReloadAsync()
    {
        var lines = await File.ReadAllLinesAsync(_path);

        // Очистить и заполнить ConcurrentDictionary из файла
        _blackList.Clear();
        foreach (var ip in lines)
        {
            _blackList.TryAdd(ip, 0);
        }
    }

    public async Task FlushAsync()
    {
        if (!_pendingSave) return;

        await _lock.WaitAsync();
        try
        {
            var ips = _blackList.Keys.OrderBy(ip => ip).ToList();
            await File.WriteAllLinesAsync(_path, ips);

            _pendingSave = false;
            "[AppSecurityService] Список IP сохранён на диск".LogMessage();
        }
        finally
        {
            _lock.Release();
        }
    }

    private bool EnsureFileExistsOrLog()
    {
        if (!File.Exists(_path))
        {
            ("Файл конфигурации appsettings.json не имеет ключа \"BlackList\": " +
                "\"path/to/configSecurity.ini\"").LogMessage();
            return false;
        }
        return true;
    }
}
