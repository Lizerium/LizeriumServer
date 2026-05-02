/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 02 мая 2026 19:36:20
 * Version: 1.0.37
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
    public int Count { get; set; }
}
