using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using Microsoft.DirectX.Direct3D;
using Font = System.Drawing.Font;
using S = ServerPackets;
using C = ClientPackets;
using Effect = Client.MirObjects.Effect;

using Client.MirScenes.Dialogs;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Client.MirScenes.Dialogs
{
    public sealed class GroupPlates : MirImageControl
    {
        public MirImageControl[] Plates, Avatars;
        public MirControl[] HealthBar;
        public MirLabel[] Names, HealthPercentage;
        public List<GroupMember> GroupMembers = new List<GroupMember>();

        public static UserObject User
        {
            get { return MapObject.User; }
            set { MapObject.User = value; }
        }

        public GroupPlates()
        {
            Movable = true;
            Location = new Point(0, 100);

            Plates = new MirImageControl[20];
            Avatars = new MirImageControl[20];
            Names = new MirLabel[20];
            HealthBar = new MirControl[20];
            HealthPercentage = new MirLabel[20];
            for (int i = 0; i < 20; i++)
            {
                Plates[i] = new MirImageControl
                {
                    Parent = this,
                    Index = 283,
                    Library = Libraries.UI,
                    Location = new Point(0, i * Libraries.UI.GetTrueSize(283).Height),
                    Visible = false,
                    NotControl = true
                };
                Avatars[i] = new MirImageControl
                {
                    Parent = Plates[i],
                    Location = new Point(0, 0),
                    Visible = false,
                    Library = Libraries.UI,
                    NotControl = true,
                    Index = 293
                };
                Names[i] = new MirLabel
                {
                    Parent = Plates[i],
                    Location = new Point(65, 12),
                    AutoSize = true,
                    Visible = false,
                    NotControl = true
                };
                HealthBar[i] = new MirControl
                {
                    Parent = Plates[i],
                    Location = new Point(65, 32),
                    Visible = false,
                    NotControl = true
                };
                HealthBar[i].BeforeDraw += HealthBar_BeforeDraw;
                HealthPercentage[i] = new MirLabel
                {
                    Parent = HealthBar[i],
                    Location = new Point(0, 0),
                    Size = new Size(97, 15),
                    Visible = false,
                    NotControl = true,
                    DrawFormat = TextFormatFlags.HorizontalCenter,
                };
            }
        }

        private void HealthBar_BeforeDraw(object sender, EventArgs e)
        {
            if (GroupMembers == null || GroupMembers.Count == 0) return;

            int i = Array.IndexOf(HealthBar, sender);
            Libraries.UI.Draw(285, new Rectangle(0, 0, (int)(Libraries.UI.GetTrueSize(285).Width * GroupMembers[i].HealthPercentage / 100F), Libraries.UI.GetTrueSize(285).Height), new Point(Plates[i].Location.X + 65, this.Location.Y + Plates[i].Location.Y + (34 * (i * this.Size.Height + 1))), Color.White, false);
        }

        public void Show()
        {
            this.Visible = true;
        }
        public void Hide()
        {
            this.Visible = false;
        }
        public void Toggle()
        {
            this.Visible = !this.Visible;
        }

        public void Process()
        {
            foreach (var item in Plates) item.Visible = false;
            foreach (var item in Names) item.Visible = false;
            foreach (var item in HealthBar) item.Visible = false;
            foreach (var item in HealthPercentage) item.Visible = false;
            foreach (var item in GroupMembers) { item.HealthPercentage = 0; item.IsInRange = false; }

            if (GroupDialog.GroupList.Count == 0) return;

            List<GroupMember> TempGroupMembers = new List<GroupMember>();
            for (int i = 0; i < GroupDialog.GroupList.Count; i++)
            {
                if (GroupDialog.GroupList[i] == User.Name) continue;

                int tempMember = GroupMembers.FindIndex(x => x.Name == GroupDialog.GroupList[i]);
                if (tempMember != null && tempMember >= 0)
                    TempGroupMembers.Add(GroupMembers[tempMember]);
                else
                    TempGroupMembers.Add(new GroupMember { Name = GroupDialog.GroupList[i] });
            }

            for (int i = 0; i < MapControl.Objects.Count; i++)
                for (int j = 0; j < TempGroupMembers.Count; j++)
                {
                    MapObject ob = MapControl.Objects[i];
                    if (ob.Race == ObjectType.Player && ob.Name == TempGroupMembers[j].Name)
                    {
                        PlayerObject player = ob as PlayerObject;
                        TempGroupMembers[j].HealthPercentage = player.PercentHealth;
                        TempGroupMembers[j].Guild = player.GuildName;
                        TempGroupMembers[j].IsDead = player.Dead;
                        TempGroupMembers[j].IsInRange = true;

                        TempGroupMembers[j].Level = player.Level; // Doesn't update on user level change
                        TempGroupMembers[j].Gender = player.Gender; // Not working for females
                        TempGroupMembers[j].Class = player.Class;

                        TempGroupMembers[j].Initialized = true;
                    }
                }

            GroupMembers.Clear();
            GroupMembers = TempGroupMembers;

            for (int i = 0; i < GroupMembers.Count; i++)
            {
                Plates[i].Index = (GroupDialog.GroupList[0] == GroupMembers[i].Name) ? 284 : 283;
                Plates[i].Visible = Names[i].Visible = HealthBar[i].Visible = HealthPercentage[i].Visible = Avatars[i].Visible = true;
                Names[i].Text = GroupMembers[i].Name + ((GroupMembers[i].Level > 0) ? "   Lv." + GroupMembers[i].Level : string.Empty);
                HealthPercentage[i].Text = (GroupMembers[i].IsDead && GroupMembers[i].IsInRange) ? "Dead" : (GroupMembers[i].HealthPercentage == 0) ? "Out of range" : GroupMembers[i].HealthPercentage + "%";

                if (GroupMembers[i].Initialized)
                    switch (GroupMembers[i].Class)
                    {
                        case MirClass.Warrior:
                            Avatars[i].Index = (GroupMembers[i].Gender == MirGender.Male) ? 287 : 288;
                            break;
                        case MirClass.Wizard:
                            Avatars[i].Index = (GroupMembers[i].Gender == MirGender.Male) ? 289 : 290;
                            break;
                        case MirClass.Taoist:
                            Avatars[i].Index = (GroupMembers[i].Gender == MirGender.Male) ? 291 : 292;
                            break;
                        default:
                            Avatars[i].Index = 293;
                            break;
                    }
                if (GroupMembers[i].IsDead && GroupMembers[i].IsInRange) Avatars[i].Index = 286;
            }
        }

        public class GroupMember
        {
            public string
                Name = string.Empty,
                Guild = string.Empty;

            public byte
                HealthPercentage = 0;

            public int
                Level = -1;

            public bool
                IsDead = false,
                IsInRange = false;

            public bool Initialized = false; // When the user is in range we can get their gender, class for their avatar, once Initialized = true we do not need to keep checking or assigning values.
            public MirGender Gender = MirGender.Male;
            public MirClass Class = MirClass.Warrior;
        }
    }
    

    // For TheLord
    //public sealed class GroupPlates : MirImageControl
    //{
    //    public MirImageControl[] Plates;
    //    public MirControl[] HealthBar;
    //    public MirLabel[] Names, HealthPercentage;
    //    public List<GroupMember> GroupMembers = new List<GroupMember>();

    //    public static UserObject User
    //    {
    //        get { return MapObject.User; }
    //        set { MapObject.User = value; }
    //    }

    //    public GroupPlates()
    //    {
    //        Movable = true;
    //        Location = new Point(0, 100);

    //        Plates = new MirImageControl[20];
    //        Names = new MirLabel[20];
    //        HealthBar = new MirControl[20];
    //        HealthPercentage = new MirLabel[20];
    //        for (int i = 0; i < 20; i++)
    //        {
    //            Plates[i] = new MirImageControl
    //            {
    //                Parent = this,
    //                Index = 237, // <--- Change This
    //                Library = Libraries.Prguse,
    //                Location = new Point(0, i * Libraries.Prguse.GetTrueSize(237).Height), // <--- Change This (237) to match Index above
    //                Visible = false,
    //                NotControl = true
    //            };
    //            Names[i] = new MirLabel
    //            {
    //                Parent = Plates[i],
    //                Location = new Point(0, 0),
    //                AutoSize = true,
    //                Visible = false,
    //                NotControl = true
    //            };
    //            HealthBar[i] = new MirControl
    //            {
    //                Parent = Plates[i],
    //                Location = new Point(0, 13),
    //                Visible = false,
    //                NotControl = true
    //            };
    //            HealthBar[i].BeforeDraw += HealthBar_BeforeDraw;
    //            HealthPercentage[i] = new MirLabel
    //            {
    //                Parent = HealthBar[i],
    //                Location = new Point(0, 0),
    //                Size = new Size(97, 15),
    //                Visible = false,
    //                NotControl = true,
    //                DrawFormat = TextFormatFlags.HorizontalCenter,
    //            };
    //        }
    //    }

    //    private void HealthBar_BeforeDraw(object sender, EventArgs e)
    //    {
    //        if (GroupMembers == null || GroupMembers.Count == 0) return;

    //        int i = Array.IndexOf(HealthBar, sender);
    //        Libraries.Prguse.Draw(1951 /* <--- Change this */, new Rectangle(0, 0, (int)(Libraries.Prguse.GetTrueSize(1951 /* <--- Change this*/).Width - 6 * GroupMembers[i].HealthPercentage / 100F), Libraries.Prguse.GetTrueSize(1951 /* <--- Change this*/).Height), new Point(3, this.Location.Y + Plates[i].Location.Y + (13 * (i * this.Size.Height + 1))), Color.White, false);
    //    }

    //    public void Show()
    //    {
    //        this.Visible = true;
    //    }
    //    public void Hide()
    //    {
    //        this.Visible = false;
    //    }
    //    public void Toggle()
    //    {
    //        this.Visible = !this.Visible;
    //    }

    //    public void Process()
    //    {
    //        foreach (var item in Plates) item.Visible = false;
    //        foreach (var item in Names) item.Visible = false;
    //        foreach (var item in HealthBar) item.Visible = false;
    //        foreach (var item in HealthPercentage) item.Visible = false;

    //        if (GroupDialog.GroupList.Count == 0) return;

    //        List<GroupMember> TempGroupMembers = new List<GroupMember>();
    //        for (int i = 0; i < GroupDialog.GroupList.Count; i++)
    //        {
    //            if (GroupDialog.GroupList[i] == User.Name) continue;

    //            int tempMember = GroupMembers.FindIndex(x => x.Name == GroupDialog.GroupList[i]);
    //            if (tempMember != null && tempMember >= 0)
    //                TempGroupMembers.Add(GroupMembers[tempMember]);
    //            else
    //                TempGroupMembers.Add(new GroupMember { Name = GroupDialog.GroupList[i] });
    //        }

    //        for (int i = 0; i < MapControl.Objects.Count; i++)
    //            for (int j = 0; j < TempGroupMembers.Count; j++)
    //            {
    //                MapObject ob = MapControl.Objects[i];
    //                if (ob.Race == ObjectType.Player && ob.Name == TempGroupMembers[j].Name)
    //                {
    //                    PlayerObject player = ob as PlayerObject;
    //                    TempGroupMembers[j].HealthPercentage = player.PercentHealth;
    //                    TempGroupMembers[j].Guild = player.GuildName;
    //                }
    //            }

    //        GroupMembers.Clear();
    //        GroupMembers = TempGroupMembers;

    //        for (int i = 0; i < GroupMembers.Count; i++)
    //        {
    //            Plates[i].Visible = Names[i].Visible = HealthBar[i].Visible = HealthPercentage[i].Visible = true;
    //            Names[i].Text = GroupMembers[i].Name;
    //            HealthPercentage[i].Text = GroupMembers[i].HealthPercentage + "%";
    //        }
    //    }

    //    public class GroupMember
    //    {
    //        public string
    //            Name = string.Empty,
    //            Guild = string.Empty;

    //        public byte
    //            HealthPercentage = 0;            
    //    }
    //}
}