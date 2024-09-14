using NiceHashMiner.Configs.Data;

namespace NiceHashMiner.Configs.ConfigJsonFile
{
    public class DeviceBenchmarkConfigFile : ConfigFile<DeviceBenchmarkConfig>
    {
        private const string BenchmarkPrefix = "benchmark_";

        private static string GetName(string deviceUuid, string old = "")
        {
            // make device name
            var invalid = new[] { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };
            var fileName = BenchmarkPrefix + deviceUuid.Replace(' ', '_');
            foreach (var c in invalid)
            {
                fileName = fileName.Replace(c.ToString(), string.Empty);
            }
            const string extension = ".json";
            return "configs\\profiles\\" + ConfigManager.GeneralConfig.ProfileName.Trim() + "\\" +
                fileName + old + extension;
        }

        public DeviceBenchmarkConfigFile(string deviceUuid)
            : base("configs\\profiles\\" + ConfigManager.GeneralConfig.ProfileName.Trim() + "\\",
                  GetName(deviceUuid), GetName(deviceUuid, "_OLD"))
            //: base(Folders.Config, GetName(deviceUuid), GetName(deviceUuid, "_OLD"))
        { }
    }
}
