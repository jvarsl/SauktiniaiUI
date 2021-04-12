using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SauktiniaiUI
{
    public enum City
    {
        Vilnius = 6,
        Kaunas = 2,
        Klaipeda = 3,
        Siauliai = 5,
        Panevezys = 4,
        Alytus = 1
    }
    public class Sauktiniai
    {
        public static long GetJsonFileSize(string path)
        {
            bool fileExists = new System.IO.FileInfo(path).Exists;
            return fileExists ? new System.IO.FileInfo(path).Length : 0;
        }
        public static void MakeProperJson(string filePath, City city)
        {
            string text = File.ReadAllText(filePath);

            //forms from many separate json answer one readable json
            text = Regex.Replace(text, @"}]\[{", "},{");
            text = Regex.Replace(text, @"""}", $@""", ""date"":""{DateTime.Now:yyyy-MM-dd}"", ""region"":""{city}"", ""regionNo"":""{(int)city}""}}");
            text = Regex.Replace(text, @"}]\[]", "}]");

            //visually readable format and unicode
            string json = Sauktiniai.FormatJson(text);

            File.WriteAllText(filePath, json, Encoding.Unicode);
        }

        private static string FormatJson(string json)
        {
            try
            {
                dynamic parsedJson = JsonConvert.DeserializeObject(json);
                return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            }
            catch
            {
                return json;
            }
        }

        public static void CmdExecute(string cmdRequest)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine(cmdRequest);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            //Console.WriteLine(cmd.StandardOutput.ReadToEnd());
        }
        public static string CmdRequestString(string filePath, string range, int cityIndex)
        {
            return "C:\\curl -k \"https://sauktiniai.karys.lt/list.php?region="+cityIndex+"\" ^"
                 + "\n-H \"authority: sauktiniai.karys.lt\" ^"
                 + "\n-H \"accept: application/json, text/plain, */*\" ^"
                 + "\n-H \"range-unit: items\" ^"
                 + "\n-H \"dnt: 1\" ^"
                 + "\n-H \"user-agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36\" ^"
                 + "\n-H \"range: " + range + "\" ^"
                 + "\n-H \"sec-fetch-site: same-origin\" ^"
                 + "\n-H \"sec-fetch-mode: cors\" ^"
                 + "\n-H \"sec-fetch-dest: empty\" ^"
                 + "\n-H \"referer: https://sauktiniai.karys.lt/\" ^"
                 + "\n-H \"accept-language: en-US,en;q=0.9\" ^"
                 + "\n  --compressed >> " + filePath;
        }
    }
}
