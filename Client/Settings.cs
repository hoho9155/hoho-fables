﻿using System.IO;
using System;
using Client.MirSounds;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace Client
{
    class Settings
    {
        public const long CleanDelay = 600000;
        public static int ScreenWidth = 800, ScreenHeight = 600;
        private static InIReader Reader = new InIReader(@".\Mir2Config.ini");

        public static  bool InitializeLibraries = true;
        private static bool _useTestConfig;
        public static bool UseTestConfig
        {
            get
            {
                return _useTestConfig;
            }
            set 
            {
                if (value == true)
                {
                    Reader = new InIReader(@".\Mir2Test.ini");
                }
                _useTestConfig = value;
            }
        }

        static bool IsWindows10()
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

            string productName = (string)reg.GetValue("ProductName");

            return productName.StartsWith("Windows 10");
        }

        private const string MinimapDataListFile = @".\mmdata.list";
        public static List<MinimapData> MiniMapDataList;
        public class MinimapData
        {
            public string Map, Name, LinkMap;
            public UInt16 Index, X, Y;
        }
        public static List<MapData> MapDataList;
        public class MapData
        {
            public string Map, Name;
            public UInt16 MinimapNumber;
        }

        public const string DataPath = @".\Data\",
                            MapPath = @".\Map\",
                            SoundPath = @".\Sound\",
                            ExtraDataPath = @".\Data\Extra\",
                            ShadersPath = @".\Data\Shaders\",
                            MonsterPath = @".\Data\Monster\",
                            GatePath = @".\Data\Gate\",
                            FlagPath = @".\Data\Flag\",
                            NPCPath = @".\Data\NPC\",
                            CArmourPath = @".\Data\CArmour\",
                            CShieldsPath = @".\Data\CShields\",
                            CWeaponPath = @".\Data\CWeapon\",
							CWeaponEffectPath = @".\Data\CWeaponEffect\",
							CHairPath = @".\Data\CHair\",
                            AArmourPath = @".\Data\AArmour\",
                            AWeaponPath = @".\Data\AWeapon\",
                            AHairPath = @".\Data\AHair\",
                            ARArmourPath = @".\Data\ARArmour\",
                            ARWeaponPath = @".\Data\ARWeapon\",
                            ARHairPath = @".\Data\ARHair\",
                            CHumEffectPath = @".\Data\CHumEffect\",
                            AHumEffectPath = @".\Data\AHumEffect\",
                            ARHumEffectPath = @".\Data\ARHumEffect\",
                            MountPath = @".\Data\Mount\",
                            FishingPath = @".\Data\Fishing\",
                            PetsPath = @".\Data\Pet\",
                            TransformPath = @".\Data\Transform\",
                            TransformMountsPath = @".\Data\TransformRide2\",
                            TransformEffectPath = @".\Data\TransformEffect\",
                            TransformWeaponEffectPath = @".\Data\TransformWeaponEffect\";

        //Logs
        public static bool LogErrors = true;
        public static bool LogChat = true;
        public static int RemainingErrorLogs = 100;

        //Graphics
        public static bool FullScreen = true, TopMost = true;
        public static string FontName = "Segoe UI"; //"MS Sans Serif"
        public static bool FPSCap = true;
        public static int MaxFPS = 45;
        public static int Resolution = 1920;
        public static bool BorderlessWindow = false;
        public static bool DebugMode = false;
        public static byte MiniMapRadarSize = 2;
        public static bool UseHighlightRing = true;

        //Network
        public static bool UseConfig = true;
        public static string IPAddress = "";
        public static int Port = 7000;
        public const int TimeOut = 5000;

        //Sound
        public static int SoundOverLap = 3;
        private static byte _volume = 100;
        public static byte Volume
        {
            get { return _volume; }
            set
            {
                if (_volume == value) return;

                _volume = (byte) (value > 100 ? 100 : value);

                if (_volume == 0)
                    SoundManager.Vol = -10000;
                else 
                    SoundManager.Vol = (int)(-3000 + (3000 * (_volume / 100M)));
            }
        }

        private static byte _musicVolume = 100;
        public static byte MusicVolume
        {
            get { return _musicVolume; }
            set
            {
                if (_musicVolume == value) return;

                _musicVolume = (byte)(value > 100 ? 100 : value);

                if (_musicVolume == 0)
                    SoundManager.MusicVol = -10000;
                else
                    SoundManager.MusicVol = (int)(-3000 + (3000 * (_musicVolume / 100M)));
            }
        }

        //Game
        public static string AccountID = "",
                             Password = "";

        public static bool
            SkillMode = false,
            SkillBar = true,
            //SkillSet = true,
            Effect = true,
            LevelEffect = true,
            DropView = true,
            NameView = true,
            HPView = true,
            TransparentChat = false,
            DuraView = false,
            DisplayDamage = true,
            TargetDead = false,
            ExpandedBuffWindow = true,
            RightClickClearsTarget = true,
            ShowSecondSkillBar = false,
            Windows10LabelTextbar = false;

        public static TargetPlateHPDisplay TargetPlateHPDisplay = TargetPlateHPDisplay.PercentageRemaining;

        public static int[,] SkillbarLocation = new int[2, 2] { { 0, 0 }, { 216, 0 }  };

        //Quests
        public static int[] TrackedQuests = new int[10];

        //Chat
        public static bool
            ShowNormalChat = true,
            ShowYellChat = true,
            ShowWhisperChat = true,
            ShowLoverChat = true,
            ShowMentorChat = true,
            ShowGroupChat = true,
            ShowGuildChat = true;

        //Filters
        public static bool
            FilterNormalChat = false,
            FilterWhisperChat = false,
            FilterShoutChat = false,
            FilterSystemChat = false,
            FilterLoverChat = false,
            FilterMentorChat = false,
            FilterGroupChat = false,
            FilterGuildChat = false;


        //AutoPatcher

        public static bool P_Patcher = false;
        public static string P_Host = @"http://last-wrongHOST-mir.com/patch/";
        public static string P_PatchFileName = @"PList.gz";
        public static bool P_NeedLogin = false;
        public static string P_Login = string.Empty;
        public static string P_Password = string.Empty;
        public static string P_ServerName = string.Empty;
        public static string P_BrowserAddress = "https://launcher.mironline.co.uk/web/";
        public static string P_Client = Application.StartupPath + "\\";
        public static bool P_AutoStart = false;

        public static void LoadMinimapData()
        {
            if (File.Exists(MinimapDataListFile))
            {
                MiniMapDataList = new List<MinimapData>();
                MapDataList = new List<MapData>();
                string[] lines = File.ReadAllLines(MinimapDataListFile);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("#")) continue; // Skip comments
                    if (lines[i].Length == 0) continue; // Skip comments
                    //if (lines[i].Count(f => f == '|') != 5) continue; // Skip lines without the appropriate breaks

                    string[] line = lines[i].Split('|');

                    if (line.Length == 3)
                        MapDataList.Add(new MapData
                        {
                            Map = line[0],
                            MinimapNumber = Convert.ToUInt16(line[1]),
                            Name = line[2]
                        });
                    else
                        MiniMapDataList.Add(new MinimapData
                        {
                            Map = @line[0].Replace(@"\\", @"\"),
                            Name = line[1],
                            Index = Convert.ToUInt16(line[2]),
                            X = Convert.ToUInt16(line[3]),
                            Y = Convert.ToUInt16(line[4]),
                            LinkMap = @line[5].Replace(@"\\", @"\")
                        });

                }
            }
        }
        public static void Load()
        {
            if (!Directory.Exists(DataPath)) Directory.CreateDirectory(DataPath);
            if (!Directory.Exists(MapPath)) Directory.CreateDirectory(MapPath);
            if (!Directory.Exists(SoundPath)) Directory.CreateDirectory(SoundPath);

            //Graphics
            FullScreen = Reader.ReadBoolean("Graphics", "FullScreen", FullScreen);
            TopMost = Reader.ReadBoolean("Graphics", "AlwaysOnTop", TopMost);
            FPSCap = Reader.ReadBoolean("Graphics", "FPSCap", FPSCap);
            Resolution = Reader.ReadInt32("Graphics", "Resolution", Resolution);
            BorderlessWindow = Reader.ReadBoolean("Graphics", "BorderlessWindow", BorderlessWindow);
            MiniMapRadarSize = Reader.ReadByte("Graphics", "MiniMapRadarSize", MiniMapRadarSize);
            DebugMode = Reader.ReadBoolean("Graphics", "DebugMode", DebugMode);
            UseHighlightRing = Reader.ReadBoolean("Graphics", "UseHighlightRing", UseHighlightRing);            

            //Network
            UseConfig = Reader.ReadBoolean("Network", "UseConfig", UseConfig);
            if (UseConfig)
            {
                IPAddress = Reader.ReadString("Network", "IPAddress", IPAddress);
                Port = Reader.ReadInt32("Network", "Port", Port);
            }

            //Logs
            LogErrors = Reader.ReadBoolean("Logs", "LogErrors", LogErrors);
            LogChat = Reader.ReadBoolean("Logs", "LogChat", LogChat);

            //Sound
            Volume = Reader.ReadByte("Sound", "Volume", Volume);
            SoundOverLap = Reader.ReadInt32("Sound", "SoundOverLap", SoundOverLap);
            MusicVolume = Reader.ReadByte("Sound", "Music", MusicVolume);

            //Game
            AccountID = Reader.ReadString("Game", "AccountID", AccountID);
            Password = Reader.ReadString("Game", "Password", Password);

            SkillMode = Reader.ReadBoolean("Game", "SkillMode", SkillMode);
            SkillBar = Reader.ReadBoolean("Game", "SkillBar", SkillBar);
            //SkillSet = Reader.ReadBoolean("Game", "SkillSet", SkillSet);
            Effect = Reader.ReadBoolean("Game", "Effect", Effect);
            LevelEffect = Reader.ReadBoolean("Game", "LevelEffect", Effect);
            DropView = Reader.ReadBoolean("Game", "DropView", DropView);
            NameView = Reader.ReadBoolean("Game", "NameView", NameView);
            HPView = Reader.ReadBoolean("Game", "HPMPView", HPView);
            FontName = Reader.ReadString("Game", "FontName", FontName);
            TransparentChat = Reader.ReadBoolean("Game", "TransparentChat", TransparentChat);
            DisplayDamage = Reader.ReadBoolean("Game", "DisplayDamage", DisplayDamage);
            TargetDead = Reader.ReadBoolean("Game", "TargetDead", TargetDead);
            ExpandedBuffWindow = Reader.ReadBoolean("Game", "ExpandedBuffWindow", ExpandedBuffWindow);
            DuraView = Reader.ReadBoolean("Game", "DuraWindow", DuraView);
            RightClickClearsTarget = Reader.ReadBoolean("Game", "RightClickClearsTarget", RightClickClearsTarget);
            ShowSecondSkillBar = Reader.ReadBoolean("Game", "ShowSecondSkillBar", ShowSecondSkillBar);
            Windows10LabelTextbar = Reader.ReadBoolean("Game", "Windows10LabelTextbar", Windows10LabelTextbar);
            TargetPlateHPDisplay = (TargetPlateHPDisplay)Reader.ReadByte("Game", "TargetPlateHPDisplay", (byte)TargetPlateHPDisplay);

            for (int i = 0; i < SkillbarLocation.Length / 2; i++)
            {
                SkillbarLocation[i, 0] = Reader.ReadInt32("Game", "Skillbar" + i.ToString() + "X", SkillbarLocation[i, 0]);
                SkillbarLocation[i, 1] = Reader.ReadInt32("Game", "Skillbar" + i.ToString() + "Y", SkillbarLocation[i, 1]);
            }

            //Chat
            ShowNormalChat = Reader.ReadBoolean("Chat", "ShowNormalChat", ShowNormalChat);
            ShowYellChat = Reader.ReadBoolean("Chat", "ShowYellChat", ShowYellChat);
            ShowWhisperChat = Reader.ReadBoolean("Chat", "ShowWhisperChat", ShowWhisperChat);
            ShowLoverChat = Reader.ReadBoolean("Chat", "ShowLoverChat", ShowLoverChat);
            ShowMentorChat = Reader.ReadBoolean("Chat", "ShowMentorChat", ShowMentorChat);
            ShowGroupChat = Reader.ReadBoolean("Chat", "ShowGroupChat", ShowGroupChat);
            ShowGuildChat = Reader.ReadBoolean("Chat", "ShowGuildChat", ShowGuildChat);

            //Filters
            FilterNormalChat = Reader.ReadBoolean("Filter", "FilterNormalChat", FilterNormalChat);
            FilterWhisperChat = Reader.ReadBoolean("Filter", "FilterWhisperChat", FilterWhisperChat);
            FilterShoutChat = Reader.ReadBoolean("Filter", "FilterShoutChat", FilterShoutChat);
            FilterSystemChat = Reader.ReadBoolean("Filter", "FilterSystemChat", FilterSystemChat);
            FilterLoverChat = Reader.ReadBoolean("Filter", "FilterLoverChat", FilterLoverChat);
            FilterMentorChat = Reader.ReadBoolean("Filter", "FilterMentorChat", FilterMentorChat);
            FilterGroupChat = Reader.ReadBoolean("Filter", "FilterGroupChat", FilterGroupChat);
            FilterGuildChat = Reader.ReadBoolean("Filter", "FilterGuildChat", FilterGuildChat);

            //AutoPatcher
            P_Patcher = Reader.ReadBoolean("Launcher", "False", P_Patcher);
            P_Host = Reader.ReadString("Launcher", "Host", P_Host);
            P_PatchFileName = Reader.ReadString("Launcher", "PatchFile", P_PatchFileName);
            P_NeedLogin = Reader.ReadBoolean("Launcher", "NeedLogin", P_NeedLogin);
            P_Login = Reader.ReadString("Launcher", "Login", P_Login);
            P_Password = Reader.ReadString("Launcher", "Password", P_Password);
            P_AutoStart = Reader.ReadBoolean("Launcher", "AutoStart", P_AutoStart);
            P_ServerName = Reader.ReadString("Launcher", "ServerName", P_ServerName);
            P_BrowserAddress = Reader.ReadString("Launcher", "Browser", P_BrowserAddress);

            if (!P_Host.EndsWith("/")) P_Host += "/";
            if (P_Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase)) P_Host = P_Host.Insert(0, "http://");
            if (P_BrowserAddress.StartsWith("www.", StringComparison.OrdinalIgnoreCase)) P_BrowserAddress = P_BrowserAddress.Insert(0, "http://");
        }

        public static void Save()
        {
            //Graphics
            Reader.Write("Graphics", "FullScreen", FullScreen);
            Reader.Write("Graphics", "AlwaysOnTop", TopMost);
            Reader.Write("Graphics", "FPSCap", FPSCap);
            Reader.Write("Graphics", "Resolution", Resolution);
            Reader.Write("Graphics", "BorderlessWindow", BorderlessWindow);
            Reader.Write("Graphics", "MiniMapRadarSize", MiniMapRadarSize);
            Reader.Write("Graphics", "DebugMode", DebugMode);
            Reader.Write("Graphics", "UseHighlightRing", UseHighlightRing);

            //Sound
            Reader.Write("Sound", "Volume", Volume);
            Reader.Write("Sound", "Music", MusicVolume);

            //Game
            Reader.Write("Game", "AccountID", AccountID);
            Reader.Write("Game", "Password", Password);
            Reader.Write("Game", "SkillMode", SkillMode);
            Reader.Write("Game", "SkillBar", SkillBar);
            //Reader.Write("Game", "SkillSet", SkillSet);
            Reader.Write("Game", "Effect", Effect);
            Reader.Write("Game", "LevelEffect", LevelEffect);
            Reader.Write("Game", "DropView", DropView);
            Reader.Write("Game", "NameView", NameView);
            Reader.Write("Game", "HPMPView", HPView);
            Reader.Write("Game", "FontName", FontName);
            Reader.Write("Game", "TransparentChat", TransparentChat);
            Reader.Write("Game", "DisplayDamage", DisplayDamage);
            Reader.Write("Game", "TargetDead", TargetDead);
            Reader.Write("Game", "ExpandedBuffWindow", ExpandedBuffWindow);
            Reader.Write("Game", "DuraWindow", DuraView);
            Reader.Write("Game", "RightClickClearsTarget", RightClickClearsTarget);
            Reader.Write("Game", "ShowSecondSkillBar", ShowSecondSkillBar);
            Reader.Write("Game", "Windows10LabelTextbar", Windows10LabelTextbar);
            Reader.Write("Game", "TargetPlateHPDisplay", (byte)TargetPlateHPDisplay);

            //for (int i = 0; i < SkillbarLocation.Length / 2; i++)
            //{
            //    Reader.Write("Game", "Skillbar" + i.ToString() + "X", SkillbarLocation[i, 0]);
            //    Reader.Write("Game", "Skillbar" + i.ToString() + "Y", SkillbarLocation[i, 1]);
            //}

            //Chat
            Reader.Write("Chat", "ShowNormalChat", ShowNormalChat);
            Reader.Write("Chat", "ShowYellChat", ShowYellChat);
            Reader.Write("Chat", "ShowWhisperChat", ShowWhisperChat);
            Reader.Write("Chat", "ShowLoverChat", ShowLoverChat);
            Reader.Write("Chat", "ShowMentorChat", ShowMentorChat);
            Reader.Write("Chat", "ShowGroupChat", ShowGroupChat);
            Reader.Write("Chat", "ShowGuildChat", ShowGuildChat);

            //Filters
            Reader.Write("Filter", "FilterNormalChat", FilterNormalChat);
            Reader.Write("Filter", "FilterWhisperChat", FilterWhisperChat);
            Reader.Write("Filter", "FilterShoutChat", FilterShoutChat);
            Reader.Write("Filter", "FilterSystemChat", FilterSystemChat);
            Reader.Write("Filter", "FilterLoverChat", FilterLoverChat);
            Reader.Write("Filter", "FilterMentorChat", FilterMentorChat);
            Reader.Write("Filter", "FilterGroupChat", FilterGroupChat);
            Reader.Write("Filter", "FilterGuildChat", FilterGuildChat);

            //AutoPatcher
            Reader.Write("Launcher", "Enabled", P_Patcher);
            Reader.Write("Launcher", "Host", P_Host);
            Reader.Write("Launcher", "PatchFile", P_PatchFileName);
            Reader.Write("Launcher", "NeedLogin", P_NeedLogin);
            Reader.Write("Launcher", "Login", P_Login);
            Reader.Write("Launcher", "Password", P_Password);
            Reader.Write("Launcher", "ServerName", P_ServerName);
            Reader.Write("Launcher", "Browser", P_BrowserAddress);
            Reader.Write("Launcher", "AutoStart", P_AutoStart);
        }

        public static void LoadTrackedQuests(string Charname)
        {
            //Quests
            for (int i = 0; i < TrackedQuests.Length; i++)
            {
                TrackedQuests[i] = Reader.ReadInt32("Q-" + Charname, "Quest-" + i.ToString(), -1);
            }
        }

        public static void SaveTrackedQuests(string Charname)
        {
            //Quests
            for (int i = 0; i < TrackedQuests.Length; i++)
            {
                Reader.Write("Q-" + Charname, "Quest-" + i.ToString(), TrackedQuests[i]);
            }
        }
    }
}
