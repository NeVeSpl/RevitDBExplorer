using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal static class VersionChecker
    {
        public async static Task<(bool, string)> CheckIfNewVersionIsAvailable()
        {
            try
            {
                var ver = Assembly.GetExecutingAssembly().GetName().Version;
                var curentVer = ver.ToGitHubTag();

                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://api.github.com");
                httpClient.DefaultRequestHeaders.Add("User-Agent", "RevitDBExplorer");
                var response = await httpClient.GetFromJsonAsync<Rootobject>($"repos/NeVeSpl/RevitDBExplorer/releases/latest");

                if (!response.tag_name.StartsWith(curentVer))
                {
                    await Task.Delay(2000);
                    return (true, response.html_url);
                }
            }
            catch
            {

            }
            return (false, "");
         }
    }


    public class Rootobject
    {
        public string html_url { get; set; }   
        public string tag_name { get; set; }
        public string name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime published_at { get; set; }     
    }
}