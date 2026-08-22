/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 22 августа 2026 15:02:14
 * Version: 1.0.153
 */

namespace Api.LizeriumServer.FormatsData.Stats;

public class MonitorData
{
    public int Id { get; set; }
    public string Date { get; set; }
    public string IP { get; set; }
    public string Lang  { get; set; }
    public string Block  { get; set; }
    public string Agent { get; set; }
    public string Path { get; set; }
    public bool Banned { get; set; }
    public bool IsBot { get; set; }
    public int Count { get; set; }
}

public class MonitorHourlyData
{
    public string Label { get; set; }
    public int Visits { get; set; }
    public int UniqueIps { get; set; }
    public int HumanVisits { get; set; }
    public int BotVisits { get; set; }
    public int HumanUniqueIps { get; set; }
    public int BotUniqueIps { get; set; }
}
