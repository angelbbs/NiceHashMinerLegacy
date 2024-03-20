using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NiceHashMiner.Stats;
using NiceHashMiner.Utils;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;

namespace NiceHashMiner
{
    public static class Links
    {
        public static string VisitUrl = ("https://www.nicehash.com");
        public static string VisitUrlNew = ("https://github.com/angelbbs/NiceHashMinerLegacy/releases/");
        public static string CheckStatsNew = ("https://nicehash.com/my/miner/");
        public static string StatusNicehash = ("https://status.nicehash.com/");
        public static string NhmHelp = ("https://github.com/angelbbs/NiceHashMinerLegacy/");
        public static string NhmNoDevHelp = ("https://github.com/nicehash/NiceHashMinerLegacy/wiki/Troubleshooting#nosupportdev");
        public static string NhmBtcWalletFaqNew = ("https://www.nicehash.com/support");
        public static string NhmSocketAddress = ("wss://nhmws.nicehash.com/v3/nhml");
        public static string NhmSocketAddressV4 = ("wss://nhmws.nicehash.com/v4/nhm");
        public static string NhmHashpower = ("https://api2.nicehash.com/main/api/v2/hashpower/orderBook?algorithm=");
        public static string NhmSimplemultialgo = ("https://api2.nicehash.com/main/api/v2/public/simplemultialgo/info");
        public static string NhmCurrent = ("https://api2.nicehash.com/main/api/v2/public/stats/global/current");
        public static string Nhm24h = ("https://api2.nicehash.com/main/api/v2/public/stats/global/24h");
        public static string NhmExternal = ("https://api2.nicehash.com/main/api/v2/mining/external/");
        public static string RigDetails = ("https://api2.nicehash.com/main/api/v2/mining/rig2/");
        public static string Balance = ("https://api2.nicehash.com/main/api/v2/accounting/account2/BTC");
        public static string ServerTime = ("https://api2.nicehash.com/api/v2/time");
        public static string ApiFlags = ("https://api2.nicehash.com/api/v2/system/flags");
        public static string GetAPIkey = ("https://www.nicehash.com/my/settings/keys");
        public static string ApiUrl = ("https://api.nicehash.com/api?method=nicehash.service.info");//?
        public static string exchangeRateList = ("https://api2.nicehash.com/main/api/v2/exchangeRate/list/");
        public static string miningStats = ("https://www.nicehash.com/my/mining/stats/");
        public static string githubReleases = ("https://github.com/angelbbs/NiceHashMinerLegacy/releases");
        public static string githubLatestRelease => CheckDNS("https://api.github.com/repos/angelbbs/NiceHashMinerLegacy/releases/latest");
        public static string githubAllReleases => CheckDNS("https://api.github.com/repos/angelbbs/NiceHashMinerLegacy/releases");
        public static string githubDownload => CheckDNS("https://github.com0/angelbbs/NiceHashMinerLegacy/releases/download/Fork_Fix_");
        public static string gitlabReleases = ("https://gitlab.com/angelbbs/NiceHashMinerLegacy/-/releases");
        public static string gitlabRepositoryTags => CheckDNS("https://gitlab.com/api/v4/projects/26404146/repository/tags");
        public static string gitlabLastRelease => CheckDNS("https://gitlab.com/api/v4/projects/26404146/releases/");//?

        internal static bool inuse = false;

        //dns cache
        public static string CheckDNS(string domain, bool forceIP = false)
        {
            //if (domain.Contains("https") || domain.Contains("wss") || domain.Contains("443")) return domain;
            bool resolveError = false;
            string domainName = "";
            string prefix = "";
            string path = "";
            string port = "";

            do
            {
                Thread.Sleep(50);
            } while (inuse);
            inuse = true;


            if (domain.Contains("stratum-proxy"))
            {
                domain = domain.Replace("grincuckatoo32", "stratum").
                    Replace("verushash", "stratum").
                    Replace("nexapow", "stratum").
                    Replace("ironfish", "stratum").
                    Replace("karlsenhash", "stratum").
                    Replace("x16rv2", "stratum");
            }

            if (!domain.Contains("://"))
            {
                domain = "stratum+tcp://" + domain;
            }
            try
            {
                /*
                Uri test = new Uri(domain);
                var r = test.LocalPath;
                var l = test.Host;
                var p = domain.Split(':')[0];
                string doh = p + "://" + internal_get_ip_from_dns(l, "cloudflare-dns.com") + r;
                Helpers.ConsolePrint("CheckDNS", "********" + doh);
                return doh;
                */

                port = ":" + new Uri(domain).Port.ToString();
                if (port.Contains("-") ||
                    (port.Contains("443") && domain.Contains("https")) ||
                    (port.Contains("80") && domain.Equals("http"))
                    )//костыль. Uri("https://...").Port по умолчанию 443
                {
                    port = "";
                }
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("CheckDNS", ex.ToString());
            }
            try
            {
                domainName = new Uri(domain).Host;
                prefix = domain.Split(':')[0] + "://";
                if (new Uri(domain).LocalPath.Length > 1 || new Uri(domain).Query.Length > 1)
                {
                    path = new Uri(domain).LocalPath + new Uri(domain).Query;
                } 

                if (NiceHashSocket.IsIPAddress(domainName))
                {
                    inuse = false;
                    return domain;
                }
                List<string> ResolvedIPsList = new List<string>();
                var heserver = GetHostEntry(domainName);
                if (heserver == null)
                {
                    resolveError = true;
                }
                else
                {
                    
                    foreach (IPAddress curAdd in heserver.AddressList)
                    {
                        if (curAdd.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)//IPv4 only
                        {
                            ResolvedIPsList.Add(curAdd.ToString());
                        }
                    }
                    
                    //resolveError = true;//tesing
                }
                if (File.Exists("configs\\dnscache.json"))
                {
                    if (new System.IO.FileInfo("configs\\dnscache.json").Length == 0)
                    {
                        File.Delete("configs\\dnscache.json");
                    }
                }
                if (!File.Exists("configs\\dnscache.json"))
                {
                    WriteAllBytesWithBackup("configs\\dnscache.json", Properties.Resources.dnscache);
                } else
                {
                    LockManager.GetLock("configs\\dnscache.json", () =>
                    {
                        if (!File.ReadAllText("configs\\dnscache.json")[0].Equals('{'))//
                        {
                            WriteAllBytesWithBackup("configs\\dnscache.json", Properties.Resources.dnscache);
                        }
                    });
                }
                DNSCache file = null;
                try
                {
                    file = JsonConvert.DeserializeObject<DNSCache>(File.ReadAllText("configs\\dnscache.json"), Globals.JsonSettings);
                } catch (Exception)
                {
                    WriteAllBytesWithBackup("configs\\dnscache.json", Properties.Resources.dnscache);
                    file = null;
                    file = JsonConvert.DeserializeObject<DNSCache>(File.ReadAllText("configs\\dnscache.json"), Globals.JsonSettings);
                }
                
                List<IPList> _domains = file.domains;
                var _ipList = new IPList();
                _ipList.domainName = domainName;
                if (_domains.Exists(item => item.domainName == domainName))
                {
                    //Helpers.ConsolePrint("CheckDNS", "******** Exist " + domainName);
                    foreach (var d in _domains)
                    {
                        if (d.domainName.Equals(domainName))
                        {
                            _ipList.IPs = d.IPs;
                        }
                    }

                    if (resolveError || forceIP)
                    {
                        Random random = new Random((int)DateTime.Now.Ticks);
                        _ipList.IPs.RemoveAll(_IP => _IP.Contains(":"));//IPv4 only
                        string ip = _ipList.IPs[random.Next(_ipList.IPs.Count)].ToString();

                        if (resolveError)
                        {
                            Helpers.ConsolePrint("CheckDNS", "******** Return dnscache (" + domainName + "): " + prefix + ip + path);
                        }
                        inuse = false;
                        return prefix + ip + path + port;
                    }
                    foreach (string _ip in ResolvedIPsList)
                    {
                        if (!_ipList.IPs.Contains(_ip))//обновляем
                        {
                            _ipList.Updated = DateTime.Now;
                            _ipList.IPs = ResolvedIPsList;
                            _ipList.domainName = domainName;
                            int index = _domains.IndexOf(_domains.Where(n => n.domainName == domainName).FirstOrDefault());
                            _domains[index] = _ipList;
                        }
                    }
                }
                else
                {
                    if (!resolveError)
                    {
                        var ips = new List<string>();
                        _ipList.Updated = DateTime.Now;
                        _ipList.IPs = ResolvedIPsList;
                        _ipList.domainName = domainName;
                        _domains.Add(_ipList);
                    }
                    else
                    {
                        inuse = false;
                        return domain;
                    }
                }

                var _DNSCache = new DNSCache
                {
                    TimeCached = DateTime.Now,
                    domains = _domains
                };
                var s = JsonConvert.SerializeObject(_DNSCache, Formatting.Indented);
                WriteAllBytesWithBackup("configs\\dnscache.json", StringToByteArrayASCII(s));
            } catch (Exception ex)
            {
                inuse = false;
                Helpers.ConsolePrint("CheckDNS", ex.ToString());
            }
            inuse = false;
            return prefix + domainName + path + port;
        }

        public static byte[] StringToByteArrayASCII(string str)
        {
            byte[] newstr = new byte[str.Length];
            for (int a = 0; a < str.Length; a++)
            {
                newstr[a] = (byte)str[a];
            }
            return newstr;
        }

        public static void WriteAllBytesWithBackup(string FilePath, byte[] contents)
        {
            string path = FilePath;
            var tempPath = FilePath + ".tmp";

            // create the backup name
            var backup = FilePath + ".backup";

            // delete any existing backups
            try
            {
                LockManager.GetLock(backup, () =>
                {
                    if (File.Exists(backup))
                        File.Delete(backup);
                });
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("WriteAllTextWithBackup", ex.ToString());
            }

            // get the bytes
            var data = contents;

            // write the data to a temp file
            try
            {
                LockManager.GetLock(tempPath, () =>
                {
                    var tempFile = File.Create(tempPath, 4096, FileOptions.WriteThrough);
                    tempFile.Write(data, 0, data.Length);
                    tempFile.Flush();
                    tempFile.Close();
                });
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("WriteAllTextWithBackup", ex.ToString());
            }

            //copy file
            try
            {
                LockManager.GetLock(path, () =>
                {
                    if (File.Exists(path)) File.Delete(path);
                    Thread.Sleep(10);
                    File.Copy(tempPath, path);
                });
            }
            catch (Exception ex)
            {
                //Helpers.ConsolePrint("WriteAllTextWithBackup", ex.ToString());
            }
        }
        public static IPHostEntry GetHostEntry(string host)
        {
            IPHostEntry ret = null;
            if (host.ToLower().Contains("daggerautolykos") || host.ToLower().Contains("daggeroctopus") || host.ToLower().Contains("daggerkawpow") ||
                host.ToLower().Contains("daggerkheavyhash") || host.ToLower().Contains("etckheavyhash") || host.ToLower().Contains("autolykoskheavyhash") ||
                host.ToLower().Contains("daggerironfish") || host.ToLower().Contains("etchashironfish") || host.ToLower().Contains("autolykosironfish") ||
                host.ToLower().Contains("octopusironfish"))
            {
                return ret;
            }
            try
            {
                return Dns.GetHostEntry(host);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("Links", "GetHostEntry " + host + ": " + ex.ToString());
            }
            return ret;
        }
        private class DNSCache
        {
            public DateTime TimeCached; //last time
            public List<IPList> domains;
        }
        private class IPList
        {
            public DateTime Updated;
            public string domainName;
            public List<string> IPs;
        }


        [DataContractAttribute]
        internal class cf_response
        {
            [DataMemberAttribute]
            internal int Status;
            [DataMemberAttribute]
            internal cf_question[] Question;
            [DataMemberAttribute]
            internal cf_answer[] Answer;
        }
        [DataContractAttribute]
        internal class cf_question
        {
            [DataMemberAttribute]
            internal string name;
            [DataMemberAttribute]
            internal int type;
        }
        [DataContractAttribute]
        internal class cf_answer
        {
            [DataMemberAttribute]
            internal string name;
            [DataMemberAttribute]
            internal int type;
            [DataMemberAttribute]
            internal int TTL;
            [DataMemberAttribute]
            internal string data;
        }


        //DoH сервера тормозят, глючат и в итоге не имеет смысла их использовать
        private static string internal_get_ip_from_dns(string dns_name, string dns_server_addr)
        {
            try
            {
                string requestUriString = "https://" + dns_server_addr + "/dns-query?name=" + dns_name;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest httpWebRequest = WebRequest.Create(requestUriString) as HttpWebRequest;
                httpWebRequest.Host = "cloudflare-dns.com";
                httpWebRequest.Headers.Set(HttpRequestHeader.CacheControl, "max-age=0, no-cache, no-store");
                httpWebRequest.CachePolicy = (RequestCachePolicy)new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                httpWebRequest.Accept = "application/dns-json";
                httpWebRequest.Timeout = 30000;
                using (StreamReader streamReader = new StreamReader(httpWebRequest.GetResponse().GetResponseStream()))
                {
                    string end = streamReader.ReadToEnd();
                    cf_response cfResponse1 = new cf_response();
                    MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(end));
                    cf_response cfResponse2 = new DataContractJsonSerializer(cfResponse1.GetType()).ReadObject((Stream)memoryStream) as cf_response;
                    memoryStream.Close();
                    if (cfResponse2.Status != 0)
                        throw new Exception("Response status code=" + cfResponse2.Status.ToString());
                    if (cfResponse2.Answer == null || cfResponse2.Answer.Length == 0)
                        throw new Exception("No answer");
                    List<IPAddress> ipAddressList = new List<IPAddress>();
                    foreach (cf_answer cfAnswer in cfResponse2.Answer)
                    {
                        try
                        {
                            IPAddress ipAddress = IPAddress.Parse(cfAnswer.data);
                            ipAddressList.Add(ipAddress);
                        }
                        catch
                        {
                        }
                    }
                    IPAddress[] array = ipAddressList.ToArray();
                    Random random = new Random((int)DateTime.Now.Ticks);
                    string str = array[random.Next(array.Length)].ToString();
                    Helpers.ConsolePrint("dnsoverhttps", "Resolved DNS over HTTPS " + dns_name + " to: " + str);
                    return str;
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("dnsoverhttps", "Failed to get DNS res. for " + dns_name + " error: " + ex.Message);
            }
            return (string)null;
        }
    }

}
