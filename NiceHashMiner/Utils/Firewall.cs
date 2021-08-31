using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace NiceHashMiner
{
    class Firewall
    {
        static Dictionary<string, string> DirSearch(string dir)
        {
            var miners = new Dictionary<string, string>();

            try
            {
                foreach (string f in Directory.GetFiles(dir, "*.exe", SearchOption.AllDirectories))
                {
                    miners.Add(f, Path.GetFileNameWithoutExtension(f));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"DirSearch error: {e.Message}!");
            }
            return miners;
        }

        static void SetFirewallRule(string ruleArgument)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "netsh.exe",
                Arguments = ruleArgument,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            using (var setRule = Process.Start(startInfo))
            {
                setRule.WaitForExit();
            }
        }

        static void AllowFirewallRule(string programFullPath, string name)
        {
            var escapedPath = programFullPath.Contains(' ') ? $"\"{programFullPath}\"" : programFullPath;
            SetFirewallRule($"advfirewall firewall add rule name=mlff_{name} program={escapedPath} protocol=tcp dir=in enable=yes action=allow");
            SetFirewallRule($"advfirewall firewall add rule name=mlff_{name} program={escapedPath} protocol=tcp dir=out enable=yes action=allow");
        }

        static void RemoveFirewallRule(string programFullPath, string name)
        {
            SetFirewallRule($"advfirewall firewall delete rule name=mlff_{name}");
        }

        public static void AddToFirewall()
        {
            Dictionary<string, string> miners = new Dictionary<string, string>();
            SetFirewallRule($"advfirewall firewall delete rule name=mlff");
            // foreach (string binPath in relativePaths)
            {
                var tmpBins = DirSearch("miners");
                foreach (var kmp in tmpBins)
                {
                    if (!kmp.Value.Contains("vc_redist") && !kmp.Value.Contains("switch-radeon-gpu") &&
                        !kmp.Value.Contains("EthMan") && !kmp.Value.Contains("OhGodAnETHlargementPill") &&
                        !kmp.Value.Contains("WinAMDTweak") && !kmp.Value.Contains("devcon") &&
                        !kmp.Value.Contains("Restarter")
                        )
                    {
                        //                        Helpers.ConsolePrint("Firewall", kmp.Key);
                        //                        Helpers.ConsolePrint("Firewall", kmp.Value);
                        miners.Add(Directory.GetCurrentDirectory() + "\\" + kmp.Key, kmp.Value);
                    }
                }
            }

            foreach (var miner in miners)
            {
                RemoveFirewallRule(miner.Key, miner.Value);
                Thread.Sleep(1);
                AllowFirewallRule(miner.Key, miner.Value);
            }

            SetFirewallRule($"advfirewall firewall add rule name=mlff program={Directory.GetCurrentDirectory() + "\\NiceHashMinerLegacy.exe"} protocol=tcp dir=in enable=yes action=allow");
            SetFirewallRule($"advfirewall firewall add rule name=mlff program={Directory.GetCurrentDirectory() + "\\NiceHashMinerLegacy.exe"} protocol=tcp dir=out enable=yes action=allow");
        }
    }
}
