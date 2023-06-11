using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using C = ClientPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class GroupDialog : MirImageControl
    {
        public static bool AllowGroup;
        public static List<string> GroupList = new List<string>();
        public static List<GroupPlates.GroupMember> GroupMemberData = new List<GroupPlates.GroupMember>();

        public MirButton SwitchButton, CloseButton, AddButton;
        public MirLabel[] GroupMembers, GroupClass, GroupLevels;
        public MirButton[] PMMember, RemoveMember, PromoteMember;
        public MirImageControl Crown;

        public static UserObject User
        {
            get { return MapObject.User; }
            set { MapObject.User = value; }
        }

        public GroupDialog()
        {
            Index = 282;
            Library = Libraries.UI;
            Movable = true;
            Sort = true;
            Location = Center;

            GroupMembers = new MirLabel[Globals.MaxGroup];
            GroupClass = new MirLabel[Globals.MaxGroup];
            GroupLevels = new MirLabel[Globals.MaxGroup];
            PMMember = new MirButton[Globals.MaxGroup];
            RemoveMember = new MirButton[Globals.MaxGroup];
            PromoteMember = new MirButton[Globals.MaxGroup];

            for (int i = 0; i < GroupMembers.Length; i++)
            {
                GroupMembers[i] = new MirLabel
                {
                    AutoSize = true,
                    Location = new Point(30, 72 + (i * 17)),
                    Parent = this,
                    NotControl = true
                };
                GroupClass[i] = new MirLabel
                {
                    Parent = this,
                    DrawFormat = TextFormatFlags.HorizontalCenter,
                    Location = new Point(166, 72 + (i * 17)),
                    Size = new Size(53, 17 + (i * 17)),
                    NotControl = true
                };
                GroupLevels[i] = new MirLabel
                {
                    Parent = this,
                    DrawFormat = TextFormatFlags.HorizontalCenter,
                    Location = new Point(220, 72 + (i * 17)),
                    Size = new Size(39, 17 + (i * 17)),
                    NotControl = true
                };
                PMMember[i] = new MirButton
                {
                    Parent = this,
                    Index = 307,
                    HoverIndex = 308,
                    PressedIndex = 309,
                    Library = Libraries.UI,
                    Location = new Point(263, 72 + (i * 17)),
                    Visible = false,
                    Hint = "Message user"
                };
                PMMember[i].Click += PMMember_Click;

                RemoveMember[i] = new MirButton
                {
                    Parent = this,
                    Index = 310,
                    HoverIndex = 311,
                    PressedIndex = 312,
                    Library = Libraries.UI,
                    Location = new Point(283, 72 + (i * 17)),
                    Visible = false,
                    Hint = "Remove user"
                };
                RemoveMember[i].Click += RemoveMember_Click;
                PromoteMember[i] = new MirButton
                {
                    Parent = this,
                    Index = 313,
                    HoverIndex = 314,
                    PressedIndex = 315,
                    Library = Libraries.UI,
                    Location = new Point(303, 72 + (i * 17)),
                    Visible = false,
                    Hint = "Promote to group leader"
                };
            }
            Crown = new MirImageControl
            {
                Parent = this,
                Index = 306,
                Library = Libraries.UI,
                Location = new Point(10, 69),
                Visible = false
            };

            CloseButton = new MirButton
            {
                Parent = this,
                Library = Libraries.UI,
                Index = 1,
                HoverIndex = 2,
                PressedIndex = 3,
                Size = Libraries.UI.GetTrueSize(1),
                Location = new Point(this.Size.Width - 30, 16),
                Sound = SoundList.ButtonA
            };
            CloseButton.Click += (o, e) => Hide();

            SwitchButton = new MirButton
            {
                HoverIndex = 296,
                Index = 295,
                Location = new Point(11, 21),
                Library = Libraries.UI,
                Parent = this,
                PressedIndex = 116,
                Sound = SoundList.ButtonA,
            };
            SwitchButton.Click += (o, e) => Network.Enqueue(new C.SwitchGroup { AllowGroup = !AllowGroup });
            AddButton = new MirButton
            {
                Index = 421,
                HoverIndex = 422,
                PressedIndex = 423,
                Location = new Point(23, Libraries.UI.GetTrueSize(this.Index).Height - 35),
                Library = Libraries.UI,
                Parent = this,
                Sound = SoundList.ButtonA,
            };
            AddButton.Click += (o, e) => AddMember();
            //DelButton = new MirButton
            //{
            //    HoverIndex = 301,
            //    Index = 300,
            //    Location = new Point(291, Libraries.UI.GetTrueSize(this.Index).Height - 23),
            //    Library = Libraries.UI,
            //    Parent = this,
            //    PressedIndex = 302,
            //    Sound = SoundList.ButtonA,
            //};
            //DelButton.Click += (o, e) => DelMember();

            BeforeDraw += GroupPanel_BeforeDraw;

            GroupList.Clear();
        }

        private void RemoveMember_Click(object sender, EventArgs e)
        {
            int i = Array.IndexOf(RemoveMember, sender);
            MirMessageBox messageBox = new MirMessageBox( string.Format("Are you sure you want to kick [{0}] from the group?", GroupMemberData[i].Name), MirMessageBoxButtons.YesNo);

            messageBox.YesButton.Click += (o1, a) =>
            {
                Network.Enqueue(new C.DelMember { Name = GroupMemberData[i].Name });
                Hide();
            };
            messageBox.Show();
        }

        private void PMMember_Click(object sender, EventArgs e)
        {
            int i = Array.IndexOf(PMMember, sender);

            GameScene.Scene.ChatControl.chatDialog.ChatTextBox.BringToFront();
            GameScene.Scene.ChatControl.chatDialog.ChatTextBox.SetFocus();
            GameScene.Scene.ChatControl.chatDialog.ChatTextBox.Text = string.Format("/{0} ", GroupMemberData[i].Name);
            GameScene.Scene.ChatControl.chatDialog.ChatTextBox.Visible = true;
            GameScene.Scene.ChatControl.chatDialog.ChatTextBox.TextBox.SelectionLength = 0;
            GameScene.Scene.ChatControl.chatDialog.ChatTextBox.TextBox.SelectionStart = GameScene.Scene.ChatControl.chatDialog.ChatTextBox.Text.Length;
        }

        private void GroupPanel_BeforeDraw(object sender, EventArgs e)
        {
            //if (GroupList.Count == 0)
            //    DelButton.Visible = false;

            Crown.Visible = (GroupList.Count > 0) ? true : false;

            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {
                AddButton.Visible = false;
                //DelButton.Visible = false;
            }
            else
            {
                AddButton.Visible = true;
                //DelButton.Visible = true;
            }

            if (AllowGroup)
            {
                SwitchButton.Index = 294;
                SwitchButton.HoverIndex = 296;
                SwitchButton.PressedIndex = 295;
                SwitchButton.Hint = "Enabled";
            }
            else
            {
                SwitchButton.Index = 295;
                SwitchButton.HoverIndex = 296;
                SwitchButton.PressedIndex = 294;
                SwitchButton.Hint = "Disabled";
            }

            List<GroupPlates.GroupMember> TempGroupMembers = new List<GroupPlates.GroupMember>();
            for (int i = 0; i < GroupList.Count; i++)
            {
                int tempMember = GroupMemberData.FindIndex(x => x.Name == GroupList[i]);
                if (tempMember != null && tempMember >= 0)
                    TempGroupMembers.Add(GroupMemberData[tempMember]);
                else
                    TempGroupMembers.Add(new GroupPlates.GroupMember { Name = GroupList[i] });
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
                        TempGroupMembers[j].Level = player.Level;

                        if (!TempGroupMembers[j].Initialized)
                        {
                            TempGroupMembers[j].Gender = player.Gender;
                            TempGroupMembers[j].Class = player.Class;

                            TempGroupMembers[j].Initialized = true;
                        }
                    }
                }

            GroupMemberData = TempGroupMembers;

            //foreach (var item in PMMember) item.Visible = false;
            //foreach (var item in RemoveMember) item.Visible = false;
            //foreach (var item in PromoteMember) item.Visible = false;
            foreach (var item in GroupMembers) item.Text = string.Empty;
            foreach (var item in GroupClass) item.Text = string.Empty;
            foreach (var item in GroupLevels) item.Text = string.Empty;

            for (int i = GroupList.Count; i < Globals.MaxGroup; i++)
            {
                PMMember[i].Visible = false;
                RemoveMember[i].Visible = false;
                PromoteMember[i].Visible = false;
            }
                        
            for (int i = 0; i < GroupMembers.Length; i++)
            {
                GroupMembers[i].Text = i >= GroupList.Count ? string.Empty : GroupMemberData[i].Name;
                GroupClass[i].Text = i >= GroupList.Count ? string.Empty : ((GroupMemberData[i].Initialized) ? GroupMemberData[i].Class.ToString() : "-");
                GroupLevels[i].Text = i >= GroupList.Count ? string.Empty : ((GroupMemberData[i].Initialized) ? GroupMemberData[i].Level.ToString() : "-");


                if (GroupMembers[i].Text == User.Name || GroupMembers[i].Text == string.Empty) continue;
                PMMember[i].Visible = true;

                if (GroupMembers[0].Text != User.Name || GroupMembers[i].Text == string.Empty) continue;
                RemoveMember[i].Visible = PromoteMember[i].Visible = true;
            }

        }

        public void AddMember(string name)
        {
            if (GroupList.Count >= Globals.MaxGroup)
            {
                GameScene.Scene.ChatControl.chatDialog.ReceiveChat("Your group already has the maximum number of members.", ChatType.System);
                return;
            }
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {
                GameScene.Scene.ChatControl.chatDialog.ReceiveChat("You are not the leader of your group.", ChatType.System);
                return;
            }

            Network.Enqueue(new C.AddMember { Name = name });
        }

        private void AddMember()
        {
            if (GroupList.Count >= Globals.MaxGroup)
            {
                GameScene.Scene.ChatControl.chatDialog.ReceiveChat("Your group already has the maximum number of members.", ChatType.System);
                return;
            }
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {

                GameScene.Scene.ChatControl.chatDialog.ReceiveChat("You are not the leader of your group.", ChatType.System);
                return;
            }

            MirInputBox inputBox = new MirInputBox("Please enter the name of the person you wish to group.");

            inputBox.OKButton.Click += (o, e) =>
            {
                Network.Enqueue(new C.AddMember { Name = inputBox.InputTextBox.Text });
                inputBox.Dispose();
            };
            inputBox.Show();
        }
        private void DelMember()
        {
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {

                GameScene.Scene.ChatControl.chatDialog.ReceiveChat("You are not the leader of your group.", ChatType.System);
                return;
            }

            MirInputBox inputBox = new MirInputBox("Please enter the name of the person you wish to group.");

            inputBox.OKButton.Click += (o, e) =>
            {
                Network.Enqueue(new C.DelMember { Name = inputBox.InputTextBox.Text });
                inputBox.Dispose();
            };
            inputBox.Show();
        }

        public void Hide()
        {
            if (!Visible) return;
            Visible = false;
        }
        public void Show()
        {
            if (Visible) return;
            Visible = true;
        }
    }
}