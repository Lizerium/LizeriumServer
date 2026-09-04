/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 04 сентября 2026 08:00:04
 * Version: 1.0.166
 */

class Program
{
    static async Task Main()
    {

        var handler = new HttpClientHandler()
        {
            // Отключаем проверку валидности сертификата
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        var client = new HttpClient(handler);
        string url = "https://192.168.1.12:7176"; // Мой сервер

        int parallelRequests = 100; // сколько запросов параллельно
        int totalBatches = 50; // сколько раз повторить

        for (int batch = 0; batch < totalBatches; batch++)
        {
            var tasks = new List<Task>();
            for (int i = 0; i < parallelRequests; i++)
            {
                tasks.Add(client.GetAsync(url));
            }

            await Task.WhenAll(tasks);
            Console.WriteLine($"Batch {batch + 1} done");
        }

        Console.WriteLine("Load test finished");
    }
}
