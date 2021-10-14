using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MineHopWebsite.Pages;
using Newtonsoft.Json;

namespace MineHopWebsite
{
    public class Program
    {

        public static List<LeaderElement> leaderboard = new();
        public static List<Forum> Forums = new();
        public static List<Forum> AnnouncementForums = new();
        public static List<Forum> BanApealForums = new();
        public static List<Forum> DiscussionForums = new();
        public static Dictionary<string, Forum> SelctedForums = new();
        public static List<string> Admins = new ();
        public static Dictionary<string, string> TwoFaCodes = new();
        public static Dictionary<string, LoginMaker> LoginMakers = new();

        public static void Main(string[] args)
        {
            LoadAdmins();
            LoadLeaderboard();
            LoadForms();
            
            Thread scoreboardUpdater = new Thread(LeaderBoardChecker.StartLoop);
            scoreboardUpdater.Start();

            CreateHostBuilder(args).Build().Run();
        }

        public static void LoadAdmins()
        {
            if (!File.Exists("Admins.json"))
            {
                File.Create("Admins.json");
            }
            using (StreamReader sr = new StreamReader("Admins.json"))
            {
                string json = sr.ReadToEnd();
                Admins = JsonConvert.DeserializeObject<List<string>>(json);
                sr.Close();
            }
        }
        
        public static void UpdateAdmins()
        {
            if (!File.Exists("Admins.json"))
            {
                File.Create("Admins.json");
            }
            using (StreamWriter sw = new StreamWriter("Admins.json"))
            {
                sw.Write(JsonConvert.SerializeObject(Admins));
                sw.Close();
            }
            LoadAdmins();
        }

        public static void LoadLeaderboard()
        {
            if (!File.Exists("leaderboard.json"))
            {
                File.Create("leaderboard.json");
            }
            using (StreamReader sr = new StreamReader("leaderboard.json"))
            {
                string json = sr.ReadToEnd();
                leaderboard = new List<LeaderElement>();
                leaderboard = JsonConvert.DeserializeObject<List<LeaderElement>>(json);
                sr.Close();
            }
            
            
        }

        public static void UpdateForms()
        {
            if (!File.Exists("Forums.json"))
            {
                File.Create("Forums.json");
            }

            using (StreamWriter sw = new StreamWriter("Forums.json"))
            {
                sw.Write(JsonConvert.SerializeObject(Forums));
                sw.Close();
            }

            LoadForms();
        }

        private static void LoadForms()
        {if (!File.Exists("Forums.json"))
            {
                File.Create("Forums.json");
                return;
            }

            using (StreamReader sr = new StreamReader("Forums.json"))
            {
                string json = sr.ReadToEnd();
                Forums = new List<Forum>();
                Forums = JsonConvert.DeserializeObject<List<Forum>>(json);
                sr.Close();
            }

            AnnouncementForums = new List<Forum>();
            DiscussionForums = new List<Forum>();
            BanApealForums = new List<Forum>();
            
            foreach (Forum forum in Forums)
            {
                switch (forum.Type)
                {
                    case Forum.ForumType.Announcement:
                        AnnouncementForums.Add(forum);
                        break;
                    case Forum.ForumType.Discussion:
                        DiscussionForums.Add(forum);
                        break;
                    case Forum.ForumType.BanAppeal:
                        BanApealForums.Add(forum);
                        break;
                }
            }
        }

        public static string XOR(string str)
        {
            string key = "FUI$H*NF*OEWB$F*OWBG*O#&WKLWVCIFWNF##";
            string output = "";
            int keyIndex = 0;

            for (int i = 0; i < str.Length; i++)
            {
                if (keyIndex == key.Length-1)
                {
                    keyIndex = 0;
                }
                output += (char)((uint)str[i] ^ (uint)key[keyIndex]);
                keyIndex++; 
            }
            
            return output;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
    
    public class LeaderElement{
        public String displayName, mapName, time;

        public LeaderElement(String displayName, String mapName, String time)
        {
            this.mapName = mapName;
            this.displayName = displayName;
            this.time = time;
        }
    }
    
    internal class LeaderBoardChecker
    {
        private const int MsInterval = 10000;

        public static void StartLoop()
        {
            while (true)
            {
                try
                {
                    Program.LoadLeaderboard();
                    Thread.Sleep(MsInterval);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}