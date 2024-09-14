using Newtonsoft.Json;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Forms;
using NiceHashMiner.Stats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NiceHashMiner.Profiles
{
    public static class ProfileData
    {
        public class Profile
        {
            public int ProfileIndex;
            public string ProfileName;
        }
        public static List<Profile> ProfilesList = new List<Profile>();
    }
    public static class Profile
    {
        public static void SaveProfile(string profileName, string selectedProfileName)
        {
            try
            {
                if (File.Exists("Configs\\profiles.json"))
                {
                    string json = File.ReadAllText("Configs\\profiles.json");
                    File.WriteAllText("Configs\\profiles_old.json", json);
                    var profilesList = JsonConvert.DeserializeObject<List<ProfileData.Profile>>(json);
                    if (profilesList != null)
                    {
                        ProfileData.ProfilesList.Clear();
                        int index = 0;
                        foreach (var profile in profilesList)
                        {
                            ProfileData.ProfilesList.Add(profile);
                            index++;
                        }
                        ProfileData.Profile n = new ProfileData.Profile();
                        n.ProfileIndex = index;
                        n.ProfileName = profileName;
                        ProfileData.ProfilesList.Add(n);
                        string _json = JsonConvert.SerializeObject(ProfileData.ProfilesList, Formatting.Indented);
                        File.WriteAllText("Configs\\profiles.json", _json);
                        ConfigManager.GeneralConfig.ProfileIndex = index;
                    }

                    if (Directory.Exists("configs\\profiles"))
                    {
                        Directory.CreateDirectory("configs\\profiles\\" + profileName.Trim() + "\\overclock");
                        DirectoryInfo src_benchmark = new DirectoryInfo(@"configs\\profiles\\" + selectedProfileName.Trim());
                        DirectoryInfo dst_benchmark = new DirectoryInfo(@"configs\\profiles\\" + profileName.Trim());
                        CopyFiles(src_benchmark, dst_benchmark, true, "benchmark_*.json");

                        DirectoryInfo src_overclock = new DirectoryInfo(@"configs\\profiles\\" + selectedProfileName.Trim() + "\\overclock");
                        DirectoryInfo dst_overclock = new DirectoryInfo(@"configs\\profiles\\" + profileName.Trim() + "\\overclock");
                        CopyFiles(src_overclock, dst_overclock, true, "*.gpu");
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("SaveProfile", ex.ToString());
            }
            Form_Main.settings.InitProfiles();
        }

        public static void CheckShedule()
        {
            TimeSpan _From = new TimeSpan();
            TimeSpan _To = new TimeSpan();
            TimeSpan _Add = new TimeSpan(0, 0, 0);//при переходе через 24:00
            if (ConfigManager.GeneralConfig.PowerTarif == 0)
            {
                try
                {
                    _From = TimeSpan.Parse(Form_Main.ZoneSchedule1[0]);
                }
                catch (FormatException ex)
                {
                    Helpers.ConsolePrint("GetKwhPrice", ex.Message);
                }
                try
                {
                    _To = TimeSpan.Parse(Form_Main.ZoneSchedule1[1]);
                }
                catch (FormatException ex)
                {
                    Helpers.ConsolePrint("GetKwhPrice", ex.Message);
                }

                if (_To.TotalMilliseconds - _From.TotalMilliseconds < 0)
                {
                    _Add = new TimeSpan(24, 0, 0);
                    _To = _To.Add(_Add);
                }
                if (DateTime.Now.TimeOfDay.Add(_Add) >= _From && DateTime.Now.TimeOfDay.Add(_Add) < _To)
                {
                    if (!ConfigManager.GeneralConfig.ZoneScheduleUseProfile1)
                    {
                        return;
                    }
                    ApplyProfile(ConfigManager.GeneralConfig.ZoneScheduleProfileIndex1);
                    return;
                }
                if (DateTime.Now.TimeOfDay.IsBetween(_From, _To))//переход через 23:59:59
                {
                    if (!ConfigManager.GeneralConfig.ZoneScheduleUseProfile1)
                    {
                        return;
                    }
                    ApplyProfile(ConfigManager.GeneralConfig.ZoneScheduleProfileIndex1);
                    return;
                }
            }

            if (ConfigManager.GeneralConfig.PowerTarif == 1)
            {
                //1
                try
                {
                    _From = TimeSpan.Parse(Form_Main.ZoneSchedule2[0]);
                }
                catch (FormatException ex)
                {
                    Helpers.ConsolePrint("GetKwhPrice", ex.Message);
                }
                try
                {
                    _To = TimeSpan.Parse(Form_Main.ZoneSchedule2[1]);
                }
                catch (FormatException ex)
                {
                    Helpers.ConsolePrint("GetKwhPrice", ex.Message);
                }

                if (_To.TotalMilliseconds - _From.TotalMilliseconds < 0)
                {
                    _Add = new TimeSpan(24, 0, 0);
                    _To = _To.Add(_Add);
                }
                if (DateTime.Now.TimeOfDay.Add(_Add) >= _From && DateTime.Now.TimeOfDay.Add(_Add) < _To)
                {
                    if (!ConfigManager.GeneralConfig.ZoneScheduleUseProfile1)
                    {
                        return;
                    }
                    ApplyProfile(ConfigManager.GeneralConfig.ZoneScheduleProfileIndex1);
                    return;
                }
                if (DateTime.Now.TimeOfDay.IsBetween(_From, _To))//переход через 23:59:59
                {
                    if (!ConfigManager.GeneralConfig.ZoneScheduleUseProfile1)
                    {
                        return;
                    }
                    ApplyProfile(ConfigManager.GeneralConfig.ZoneScheduleProfileIndex1);
                    return;
                }

                //2
                try
                {
                    _From = TimeSpan.Parse(Form_Main.ZoneSchedule2[3]);
                }
                catch (FormatException ex)
                {
                    Helpers.ConsolePrint("GetKwhPrice", ex.Message);
                }
                try
                {
                    _To = TimeSpan.Parse(Form_Main.ZoneSchedule2[4]);
                }
                catch (FormatException ex)
                {
                    Helpers.ConsolePrint("GetKwhPrice", ex.Message);
                }

                if (_To.TotalMilliseconds - _From.TotalMilliseconds < 0)
                {
                    _Add = new TimeSpan(24, 0, 0);
                    _To = _To.Add(_Add);
                }
                if (DateTime.Now.TimeOfDay.Add(_Add) >= _From && DateTime.Now.TimeOfDay.Add(_Add) < _To)
                {
                    if (!ConfigManager.GeneralConfig.ZoneScheduleUseProfile2)
                    {
                        return;
                    }
                    ApplyProfile(ConfigManager.GeneralConfig.ZoneScheduleProfileIndex2);
                    return;
                }
                if (DateTime.Now.TimeOfDay.IsBetween(_From, _To))//переход через 23:59:59
                {
                    if (!ConfigManager.GeneralConfig.ZoneScheduleUseProfile2)
                    {
                        return;
                    }
                    ApplyProfile(ConfigManager.GeneralConfig.ZoneScheduleProfileIndex2);
                    return;
                }
            }

            if (ConfigManager.GeneralConfig.PowerTarif == 2)
            {
                //1
                try
                {
                    _From = TimeSpan.Parse(Form_Main.ZoneSchedule3[0]);
                }
                catch (FormatException ex)
                {
                    Helpers.ConsolePrint("GetKwhPrice", ex.Message);
                }
                try
                {
                    _To = TimeSpan.Parse(Form_Main.ZoneSchedule3[1]);
                }
                catch (FormatException ex)
                {
                    Helpers.ConsolePrint("GetKwhPrice", ex.Message);
                }
                if (_To.TotalMilliseconds - _From.TotalMilliseconds < 0)
                {
                    _Add = new TimeSpan(24, 0, 0);
                    _To = _To.Add(_Add);
                }
                if (DateTime.Now.TimeOfDay.Add(_Add) >= _From && DateTime.Now.TimeOfDay.Add(_Add) < _To)
                {
                    if (!ConfigManager.GeneralConfig.ZoneScheduleUseProfile1)
                    {
                        return;
                    }
                    ApplyProfile(ConfigManager.GeneralConfig.ZoneScheduleProfileIndex1);
                    return;
                }
                if (DateTime.Now.TimeOfDay.IsBetween(_From, _To))//переход через 23:59:59
                {
                    if (!ConfigManager.GeneralConfig.ZoneScheduleUseProfile1)
                    {
                        return;
                    }
                    ApplyProfile(ConfigManager.GeneralConfig.ZoneScheduleProfileIndex1);
                    return;
                }

                //2
                try
                {
                    _From = TimeSpan.Parse(Form_Main.ZoneSchedule3[3]);
                }
                catch (FormatException ex)
                {
                    Helpers.ConsolePrint("GetKwhPrice", ex.Message);
                }
                try
                {
                    _To = TimeSpan.Parse(Form_Main.ZoneSchedule3[4]);
                }
                catch (FormatException ex)
                {
                    Helpers.ConsolePrint("GetKwhPrice", ex.Message);
                }
                if (_To.TotalMilliseconds - _From.TotalMilliseconds < 0)
                {
                    _Add = new TimeSpan(24, 0, 0);
                    _To = _To.Add(_Add);
                }
                if (DateTime.Now.TimeOfDay.Add(_Add) >= _From && DateTime.Now.TimeOfDay.Add(_Add) < _To)
                {
                    if (!ConfigManager.GeneralConfig.ZoneScheduleUseProfile2)
                    {
                        return;
                    }
                    ApplyProfile(ConfigManager.GeneralConfig.ZoneScheduleProfileIndex2);
                    return;
                }
                if (DateTime.Now.TimeOfDay.IsBetween(_From, _To))//переход через 23:59:59
                {
                    if (!ConfigManager.GeneralConfig.ZoneScheduleUseProfile2)
                    {
                        return;
                    }
                    ApplyProfile(ConfigManager.GeneralConfig.ZoneScheduleProfileIndex2);
                    return;
                }

                //3
                try
                {
                    _From = TimeSpan.Parse(Form_Main.ZoneSchedule3[6]);
                }
                catch (FormatException ex)
                {
                    Helpers.ConsolePrint("GetKwhPrice", ex.Message);
                }
                try
                {
                    _To = TimeSpan.Parse(Form_Main.ZoneSchedule3[7]);
                }
                catch (FormatException ex)
                {
                    Helpers.ConsolePrint("GetKwhPrice", ex.Message);
                }
                if (_To.TotalMilliseconds - _From.TotalMilliseconds < 0)
                {
                    _Add = new TimeSpan(24, 0, 0);
                    _To = _To.Add(_Add);
                }
                if (DateTime.Now.TimeOfDay.Add(_Add) >= _From && DateTime.Now.TimeOfDay.Add(_Add) < _To)
                {
                    if (!ConfigManager.GeneralConfig.ZoneScheduleUseProfile3)
                    {
                        return;
                    }
                    ApplyProfile(ConfigManager.GeneralConfig.ZoneScheduleProfileIndex3);
                    return;
                }
                if (DateTime.Now.TimeOfDay.IsBetween(_From, _To))//переход через 23:59:59
                {
                    if (!ConfigManager.GeneralConfig.ZoneScheduleUseProfile3)
                    {
                        return;
                    }
                    ApplyProfile(ConfigManager.GeneralConfig.ZoneScheduleProfileIndex3);
                    return;
                }

                //4
                try
                {
                    _From = TimeSpan.Parse(Form_Main.ZoneSchedule3[9]);
                }
                catch (FormatException ex)
                {
                    Helpers.ConsolePrint("GetKwhPrice", ex.Message);
                }
                try
                {
                    _To = TimeSpan.Parse(Form_Main.ZoneSchedule3[10]);
                }
                catch (FormatException ex)
                {
                    Helpers.ConsolePrint("GetKwhPrice", ex.Message);
                }
                if (_To.TotalMilliseconds - _From.TotalMilliseconds < 0)
                {
                    _Add = new TimeSpan(24, 0, 0);
                    _To = _To.Add(_Add);
                }
                if (DateTime.Now.TimeOfDay.Add(_Add) >= _From && DateTime.Now.TimeOfDay.Add(_Add) < _To)
                {
                    if (!ConfigManager.GeneralConfig.ZoneScheduleUseProfile4)
                    {
                        return;
                    }
                    ApplyProfile(ConfigManager.GeneralConfig.ZoneScheduleProfileIndex4);
                    return;
                }
                if (DateTime.Now.TimeOfDay.IsBetween(_From, _To))//переход через 23:59:59
                {
                    if (!ConfigManager.GeneralConfig.ZoneScheduleUseProfile4)
                    {
                        return;
                    }
                    ApplyProfile(ConfigManager.GeneralConfig.ZoneScheduleProfileIndex4);
                    return;
                }

                //5
                try
                {
                    _From = TimeSpan.Parse(Form_Main.ZoneSchedule3[12]);
                }
                catch (FormatException ex)
                {
                    Helpers.ConsolePrint("GetKwhPrice", ex.Message);
                }
                try
                {
                    _To = TimeSpan.Parse(Form_Main.ZoneSchedule3[13]);
                }
                catch (FormatException ex)
                {
                    Helpers.ConsolePrint("GetKwhPrice", ex.Message);
                }
                if (_To.TotalMilliseconds - _From.TotalMilliseconds < 0)
                {
                    _Add = new TimeSpan(24, 0, 0);
                    _To = _To.Add(_Add);
                }
                if (DateTime.Now.TimeOfDay.Add(_Add) >= _From && DateTime.Now.TimeOfDay.Add(_Add) < _To)
                {
                    if (!ConfigManager.GeneralConfig.ZoneScheduleUseProfile5)
                    {
                        return;
                    }
                    ApplyProfile(ConfigManager.GeneralConfig.ZoneScheduleProfileIndex5);
                    return;
                }
                if (DateTime.Now.TimeOfDay.IsBetween(_From, _To))//переход через 23:59:59
                {
                    if (!ConfigManager.GeneralConfig.ZoneScheduleUseProfile5)
                    {
                        return;
                    }
                    ApplyProfile(ConfigManager.GeneralConfig.ZoneScheduleProfileIndex5);
                    return;
                }
            }
            ApplyProfile(0);
        }

        public static void ApplyProfile(int profileIndex)
        {
            FormCollection fc = Application.OpenForms;
            foreach (Form frm in fc)
            {
                if (frm.Name == "Form_Settings" || frm.Name == "Form_Benchmark")
                {
                    return;
                }
            }

            try
            {
                if (File.Exists("Configs\\profiles.json"))
                {
                    string json = File.ReadAllText("Configs\\profiles.json");
                    var profilesList = JsonConvert.DeserializeObject<List<ProfileData.Profile>>(json);
                    if (profilesList != null)
                    {
                        if (ConfigManager.GeneralConfig.ProfileName == (profilesList.Find(item => item.ProfileIndex == profileIndex)).ProfileName)
                        {
                            //Helpers.ConsolePrint("ApplyProfile", "Profile not changed");
                        } else
                        {
                            Helpers.ConsolePrint("ApplyProfile", "Profile changed from \"" +
                                ConfigManager.GeneralConfig.ProfileName + "\" to \"" +
                                profilesList.Find(item => item.ProfileIndex == profileIndex).ProfileName + "\"");
                            ConfigManager.GeneralConfig.ProfileName = (profilesList.Find(item => item.ProfileIndex == profileIndex)).ProfileName;
                            ConfigManager.GeneralConfig.ProfileIndex = profileIndex;
                            ConfigManager.GeneralConfigFileCommit();
                            //ConfigManager.CommitBenchmarks();

                            ConfigManager.AfterDeviceQueryInitialization();
                            if (ConfigManager.GeneralConfig.ABEnableOverclock && MSIAfterburner.Initialized)
                            {
                                MSIAfterburner.InitTempFiles();
                            }
                            if (Miner.IsRunningNew)
                            {
                                Helpers.ConsolePrint("ApplyProfile", "Restart mining");
                                NiceHashStats.remoteMiningStop = true;
                                NiceHashStats.remoteMiningStart = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("ApplyProfile", ex.ToString());
            }
        }

        public static bool CheckExistProfile(string profileName)
        {
            try
            {
                if (File.Exists("Configs\\profiles.json"))
                {
                    string json = File.ReadAllText("Configs\\profiles.json");
                    var profilesList = JsonConvert.DeserializeObject<List<ProfileData.Profile>>(json);
                    if (profilesList != null)
                    {
                        return profilesList.Any(item => item.ProfileName.ToLower() == profileName.ToLower());
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("CheckExistProfile", ex.ToString());
            }
            return true;
        }
        public static int GetProfilesCount()
        {
            try
            {
                if (File.Exists("Configs\\profiles.json"))
                {
                    string json = File.ReadAllText("Configs\\profiles.json");
                    var profilesList = JsonConvert.DeserializeObject<List<ProfileData.Profile>>(json);
                    if (profilesList != null)
                    {
                        return profilesList.Count();
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("DelProfile", ex.ToString());
            }
            return 0;
        }
        public static void DelProfile(int _index, string profileName)
        {
            try
            {
                if (File.Exists("Configs\\profiles.json"))
                {
                    string json = File.ReadAllText("Configs\\profiles.json");
                    File.WriteAllText("Configs\\profiles_old.json", json);
                    var profilesList = JsonConvert.DeserializeObject<List<ProfileData.Profile>>(json);
                    if (profilesList != null)
                    {
                        ProfileData.ProfilesList.Clear();
                        int index = 0;
                        foreach (var profile in profilesList)
                        {
                            if (index != _index)
                            {
                                ProfileData.Profile n = new ProfileData.Profile();
                                n.ProfileIndex = index;
                                n.ProfileName = profile.ProfileName;
                                ProfileData.ProfilesList.Add(n);
                                index++;
                            }
                            else
                            {
                                _index = -1;
                            }
                        }
                        string _json = JsonConvert.SerializeObject(ProfileData.ProfilesList, Formatting.Indented);
                        File.WriteAllText("Configs\\profiles.json", _json);

                        ConfigManager.GeneralConfig.ProfileIndex = 0;
                        ConfigManager.GeneralConfig.ProfileName = "default";
                        ConfigManager.GeneralConfigFileCommit();

                        Form_Main.settings.InitProfiles();

                        if (Directory.Exists("configs\\profiles\\" + profileName.Trim()))
                        {
                            Directory.Delete("configs\\profiles\\" + profileName.Trim(), true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("DelProfile", ex.ToString());
            }
        }
        public static void InitProfiles()
        {
            try
            {
                if (!File.Exists("configs\\profiles.json"))
                {
                    ProfileData.Profile n = new ProfileData.Profile();
                    n.ProfileIndex = 0;
                    n.ProfileName = "Default";
                    Profiles.ProfileData.ProfilesList.Add(n);
                    string json = JsonConvert.SerializeObject(ProfileData.ProfilesList, Formatting.Indented);
                    File.WriteAllText("Configs\\profiles.json", json);
                }
                //1st coping files to default profile folder
                if (!Directory.Exists("configs\\profiles"))
                {
                    Directory.CreateDirectory("configs\\profiles\\default\\overclock");
                    DirectoryInfo src_benchmark = new DirectoryInfo(@"configs");
                    DirectoryInfo dst_benchmark = new DirectoryInfo(@"configs\\profiles\\default");
                    CopyFiles(src_benchmark, dst_benchmark, true, "benchmark_*.json");

                    DirectoryInfo src_overclock = new DirectoryInfo(@"configs\\overclock");
                    DirectoryInfo dst_overclock = new DirectoryInfo(@"configs\\profiles\\default\\overclock");
                    CopyFiles(src_overclock, dst_overclock, true, "*.gpu");
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("InitProfiles", ex.ToString());
            }
        }

        public static void CopyFiles(DirectoryInfo source,
                      DirectoryInfo destination,
                      bool overwrite,
                      string searchPattern)
        {
            FileInfo[] files = source.GetFiles(searchPattern);
            foreach (FileInfo file in files)
            {
                file.CopyTo(destination.FullName + "\\" + file.Name, overwrite);
            }
        }
    }
}
