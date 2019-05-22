using System;
using System.Configuration;
using System.Collections.Specialized;

namespace TeslaMergeWin
{ 
    public class TeslaMergeConfig
    {
        public string DashCamFolder { get; set; }

        public string ffmpegBinaryFolder { get; set; }

        public string TriggerFile { get; set; }

        public string CombinedFileArguments { get; set; }
        public string PreviewFileArguments { get; set; }

        public string TimelapseFileArguments { get; set; }

        public string PushOverApplication { get; set; }
        public string PushOverUser { get; set; }

        public bool DeleteOriginals { get; set; }

        public string DateFormat { get; set; }

        public static TeslaMergeConfig GetConfig()
        {
            var configSection = (NameValueCollection)ConfigurationManager.GetSection("TeslaMergeWin");

            TeslaMergeConfig results = new TeslaMergeConfig();

            results.DashCamFolder = getConfigValue(configSection, "DashCamFolder");
            results.ffmpegBinaryFolder = getConfigValue(configSection, "ffmpegBinaryFolder");
            results.TriggerFile = getConfigValue(configSection, "TriggerFile");
            results.CombinedFileArguments = getConfigValue(configSection, "CombinedFileArguments");
            results.PreviewFileArguments = getConfigValue(configSection, "PreviewFileArguments");
            results.TimelapseFileArguments = getConfigValue(configSection, "TimelapseFileArguments");
            results.DeleteOriginals = bool.Parse(getConfigValue(configSection, "DeleteOriginals"));
            results.DateFormat = getConfigValue(configSection, "DateFormat");
            results.PushOverApplication = getConfigValue(configSection, "PushOverApplication", false);
            results.PushOverUser = getConfigValue(configSection, "PushOverUser", false);

            return results;
        }
        private static string getConfigValue(NameValueCollection configSection, string id, bool required=true)
        {
            if (null != configSection[id])
            {
                return configSection[id];
            }
            else if (required)
            {
                throw new IndexOutOfRangeException($"Expected required app.config key: {id} that was not found.");
            }
            else
            {
                return string.Empty;
            }
        }

    }


}
