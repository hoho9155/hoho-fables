using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirScenes.Dialogs;
using Client.MirSounds;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using C = ClientPackets;
using Effect = Client.MirObjects.Effect;
using Font = System.Drawing.Font;
using S = ServerPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class ChatDialog : MirImageControl
    {
        public List<ChatHistory> FullHistory = new List<ChatHistory>();
        public List<ChatHistory> History = new List<ChatHistory>();
        public List<MirLabel> ChatLines = new List<MirLabel>();

        public MirButton HomeButton, UpButton, EndButton, DownButton, PositionBar;
        public MirImageControl CountBar;
        public MirTextBox ChatTextBox;
        public Font ChatFont = new Font(Settings.FontName, 8F);
        public string LastPM = string.Empty;
       // public MirLabel ChatTextBoxWin10Label;

        public int StartIndex, LineCount = 10, WindowSize;
        public string ChatPrefix = "";

        public bool Transparent;

        public ChatDialog()
        {
            Index = 2204;
            Library = Libraries.Prguse;
            Location = new Point(100, Settings.ScreenHeight - 97 - 100);
            //PixelDetect = true;
            NotControl = false;
            

            KeyPress += ChatPanel_KeyPress;
            KeyDown += ChatPanel_KeyDown;
            MouseWheel += ChatPanel_MouseWheel;
            

            ChatTextBox = new MirTextBox
            {
                BackColour = Color.DarkGray,
                ForeColour = Color.Black,
                Parent = this,
                Size = new Size(403, 13),
                //Size = new Size(Settings.Resolution != 800 ? 627 : 403, 13),
                Location = new Point(1, 128),
                MaxLength = Globals.MaxChatLength,
                Visible = false,
                Font = ChatFont,
                NotControl = false,
                Sort = true
            };
            //ChatTextBoxWin10Label = new MirLabel
            //{
            //    Parent = this,
            //    Location = ChatTextBox.Location,
            //    Size = ChatTextBox.Size,
            //    Visible = false
            //};
            //ChatTextBoxWin10Label.Click += ChatTextBoxWin10Label_Click;
            ChatTextBox.TextBox.KeyPress += ChatTextBox_KeyPress;
            ChatTextBox.TextBox.KeyDown += ChatTextBox_KeyDown;
            ChatTextBox.TextBox.KeyUp += ChatTextBox_KeyUp;
            ChatTextBox.TextBox.TextChanged += ChatTextBox_TextChanged;
            ChatTextBox.BringToFront();
            HomeButton = new MirButton
            {
                Index = 2018,
                HoverIndex = 2019,
                Library = Libraries.Prguse,
                //Location = new Point(Settings.Resolution != 800 ? 618 : 394, 1),
                Location = new Point(394, 1),
                Parent = this,
                PressedIndex = 2020,
                Sound = SoundList.ButtonA,
                NotControl = false
            };
            HomeButton.Click += (o, e) =>
            {
                if (StartIndex == 0) return;
                StartIndex = 0;
                Update();
            };


            UpButton = new MirButton
            {
                Index = 2021,
                HoverIndex = 2022,
                Library = Libraries.Prguse,
                //Location = new Point(Settings.Resolution != 800 ? 618 : 394, 9),
                Location = new Point(394, 9),
                Parent = this,
                PressedIndex = 2023,
                Sound = SoundList.ButtonA,
                NotControl = false
            };
            UpButton.Click += (o, e) =>
            {
                if (StartIndex == 0) return;
                StartIndex--;
                Update();
            };


            EndButton = new MirButton
            {
                Index = 2027,
                HoverIndex = 2028,
                Library = Libraries.Prguse,
                //Location = new Point(Settings.Resolution != 800 ? 618 : 394, 45),
                Location = new Point(394, 120), //45
                Parent = this,
                PressedIndex = 2029,
                Sound = SoundList.ButtonA,
            };
            EndButton.Click += (o, e) =>
            {
                if (StartIndex == History.Count - 1) return;
                StartIndex = History.Count - 1;
                Update();
            };

            DownButton = new MirButton
            {
                Index = 2024,
                HoverIndex = 2025,
                Library = Libraries.Prguse,
                //Location = new Point(Settings.Resolution != 800 ? 618 : 394, 39),
                Location = new Point(394, 114), //prev 39 = -6
                Parent = this,
                PressedIndex = 2026,
                Sound = SoundList.ButtonA,
            };
            DownButton.Click += (o, e) =>
            {
                if (StartIndex == History.Count - 1) return;
                StartIndex++;
                Update();
            };



            CountBar = new MirImageControl
            {
                Index = 2012,
                Library = Libraries.Prguse,
                //Location = new Point(Settings.Resolution != 800 ? 622 : 398, 16),
                Location = new Point(398, 16),
                Parent = this,
            };

            PositionBar = new MirButton
            {
                Index = 2015,
                HoverIndex = 2016,
                Library = Libraries.Prguse,
                //Location = new Point(Settings.Resolution != 800 ? 619 : 395, 16),
                Location = new Point(395, 16),
                Parent = this,
                PressedIndex = 2017,
                Movable = true,
                Sound = SoundList.None,
            };
            PositionBar.OnMoving += PositionBar_OnMoving;
        }

        private void ChatTextBoxWin10Label_Click(object sender, EventArgs e)
        {
            ChatTextBox.SetFocus();
        }

        private void ChatTextBox_TextChanged(object sender, EventArgs e)
        {
            //ChatTextBoxWin10Label.Text = ChatTextBox.Text;
        }

        public void Show()
        {
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }

        private void ChatTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // MessageBox.Show(String.Format("X: {0}, Y:{1}", ChatTextBox.Location.X, ChatTextBox.Location.Y));
            switch (e.KeyChar)
            {
                case (char)Keys.Enter:
                    e.Handled = true;
                    if (!string.IsNullOrEmpty(ChatTextBox.Text))
                    {
                        string msg = ChatTextBox.Text;

                        if (msg.ToUpper() == "@LEVELEFFECT")
                        {
                            Settings.LevelEffect = !Settings.LevelEffect;
                        }

                        if (msg.ToUpper() == "@TARGETDEAD")
                        {
                            Settings.TargetDead = !Settings.TargetDead;
                        }

                        if (msg.ToUpper() == "@WEATHER MIST") GameScene.Scene.WeatherEffect.weatherType = WeatherType.Mist;

                        if (msg.ToUpper() == "@WEATHER SNOW") GameScene.Scene.WeatherEffect.weatherType = WeatherType.Snow;

                        if (msg.ToUpper() == "@WEATHER NONE" | msg.ToUpper() == "@WEATHER OFF") GameScene.Scene.WeatherEffect.weatherType = WeatherType.None;


                        if (msg.ToUpper().StartsWith("@TARGETPLATEHPDISPLAY"))
                        {
                            try
                            {
                                Settings.TargetPlateHPDisplay = (TargetPlateHPDisplay)Convert.ToByte(msg.Split(' ')[1]);
                            }
                            catch (Exception)
                            {
                                
                            }
                        }

                        Network.Enqueue(new C.Chat
                        {
                            Message = msg
                        });

                        if (ChatTextBox.Text[0] == '/')
                        {
                            string[] parts = ChatTextBox.Text.Split(' ');
                            if (parts.Length > 0)
                                LastPM = parts[0];
                        }
                    }
                    ChatTextBox.Visible = /*ChatTextBoxWin10Label.Visible = */false;
                    ChatTextBox.Text = string.Empty;
                    break;
                case (char)Keys.Escape:
                    e.Handled = true;
                    ChatTextBox.Visible = /*ChatTextBoxWin10Label.Visible =*/ false;
                    ChatTextBox.Text = /*ChatTextBoxWin10Label.Text =*/ string.Empty;
                    break;
            }
        }

        void PositionBar_OnMoving(object sender, MouseEventArgs e)
        {
            //int x = Settings.Resolution != 800 ? 619 : 395;
            int x = 395;
            int y = PositionBar.Location.Y;
            if (y >= 10 + CountBar.Size.Height - PositionBar.Size.Height) y = 10 + CountBar.Size.Height - PositionBar.Size.Height;
            if (y < 10) y = 10;

            int h = CountBar.Size.Height - PositionBar.Size.Height;
            h = (int)((y - 10) / (h / (float)(History.Count - 1)));

            if (h != StartIndex)
            {
                StartIndex = h;
                Update();
            }

            PositionBar.Location = new Point(x, y);
        }

        public void ReceiveChat(string text, ChatType type)
        {
            Color foreColour, backColour;

            switch (type)
            {
                case ChatType.Hint:
                    backColour = Color.White;
                    foreColour = Color.DarkGreen;
                    break;
                case ChatType.Announcement:
                    backColour = Color.Blue;
                    foreColour = Color.White;
                    GameScene.Scene.ChatNoticeDialog.ShowNotice(text);
                    break;
                case ChatType.Shout:
                    backColour = Color.Yellow;
                    foreColour = Color.Black;
                    break;
                case ChatType.Shout2:
                    backColour = Color.Green;
                    foreColour = Color.White;
                    break;
                case ChatType.Shout3:
                    backColour = Color.Purple;
                    foreColour = Color.White;
                    break;
                case ChatType.System:
                    backColour = Color.Red;
                    foreColour = Color.White;
                    break;
                case ChatType.System2:
                    backColour = Color.DarkRed;
                    foreColour = Color.White;
                    break;
                case ChatType.Group:
                    backColour = Color.White;
                    foreColour = Color.Brown;
                    break;
                case ChatType.WhisperOut:
                    foreColour = Color.CornflowerBlue;
                    backColour = Color.White;
                    break;
                case ChatType.WhisperIn:
                    foreColour = Color.DarkBlue;
                    backColour = Color.White;
                    break;
                case ChatType.Guild:
                    backColour = Color.White;
                    foreColour = Color.Green;
                    break;
                case ChatType.LevelUp:
                    backColour = Color.FromArgb(255, 225, 185, 250);
                    foreColour = Color.Blue;
                    break;
                case ChatType.Relationship:
                    backColour = Color.Transparent;
                    foreColour = Color.HotPink;
                    break;
                case ChatType.Mentor:
                    backColour = Color.White;
                    foreColour = Color.Purple;
                    break;
                default:
                    backColour = Color.White;
                    foreColour = Color.Black;
                    break;
            }

            //int chatWidth = Settings.Resolution != 800 ? 614 : 390;
            int chatWidth = 390;
            List<string> chat = new List<string>();

            int index = 0;
            for (int i = 1; i < text.Length; i++)
                if (TextRenderer.MeasureText(CMain.Graphics, text.Substring(index, i - index), ChatFont).Width > chatWidth)
                {
                    chat.Add(text.Substring(index, i - index - 1));
                    index = i - 1;
                }
            chat.Add(text.Substring(index, text.Length - index));

            if (StartIndex == History.Count - LineCount)
                StartIndex += chat.Count;

            for (int i = 0; i < chat.Count; i++)
                FullHistory.Add(new ChatHistory { Text = chat[i], BackColour = backColour, ForeColour = foreColour, Type = type });

            Update();
        }

        public void Update()
        {
            ChatTextBox.BringToFront();
            ChatTextBox.TextBox.Invalidate(); //paul
            History = new List<ChatHistory>();

            for (int i = 0; i < FullHistory.Count; i++)
            {
                switch (FullHistory[i].Type)
                {
                    case ChatType.Normal:
                        if (Settings.FilterNormalChat) continue;
                        break;
                    case ChatType.WhisperIn:
                    case ChatType.WhisperOut:
                        if (Settings.FilterWhisperChat) continue;
                        break;
                    case ChatType.Shout:
                        if (Settings.FilterShoutChat) continue;
                        break;
                    case ChatType.System:
                    case ChatType.System2:
                        if (Settings.FilterSystemChat) continue;
                        break;
                    case ChatType.Group:
                        if (Settings.FilterGroupChat) continue;
                        break;
                    case ChatType.Guild:
                        if (Settings.FilterGuildChat) continue;
                        break;
                }

                History.Add(FullHistory[i]);
            }

            for (int i = 0; i < ChatLines.Count; i++)
                ChatLines[i].Dispose();

            ChatLines.Clear();

            if (StartIndex >= History.Count) StartIndex = History.Count - 1;
            if (StartIndex < 0) StartIndex = 0;

            if (History.Count > 1)
            {
                int h = CountBar.Size.Height - PositionBar.Size.Height;
                h = (int)((h / (float)(History.Count - 1)) * StartIndex);
                //PositionBar.Location = new Point(Settings.Resolution != 800 ? 619 : 395, 16 + h);
                PositionBar.Location = new Point(395, 16 + h);
            }

            int y = 1;

            for (int i = StartIndex; i < History.Count; i++)
            {
                MirLabel temp = new MirLabel
                {
                    AutoSize = true,
                    BackColour = History[i].BackColour,
                    ForeColour = History[i].ForeColour,
                    Location = new Point(1, y),
                    OutLine = false,
                    Parent = this,
                    Text = History[i].Text,
                    Font = ChatFont,
                };
                temp.MouseWheel += ChatPanel_MouseWheel;
                ChatLines.Add(temp);

                temp.Click += (o, e) =>
                {
                    MirLabel l = o as MirLabel;
                    if (l == null) return;

                    string[] parts = l.Text.Split(':', ' ');
                    if (parts.Length == 0) return;

                    string name = Regex.Replace(parts[0], "[^A-Za-z0-9]", "");

                    ChatTextBox.BringToFront();
                    ChatTextBox.SetFocus();
                    ChatTextBox.Text = string.Format("/{0} ", name);
                    ChatTextBox.Visible = true;
                    ChatTextBox.TextBox.SelectionLength = 0;
                    ChatTextBox.TextBox.SelectionStart = ChatTextBox.Text.Length;
                };
                
                y += 13;
                if (i - StartIndex == LineCount - 1) break;
            }
        }

        private void ChatPanel_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (StartIndex == 0) return;
                    StartIndex--;
                    break;
                case Keys.Home:
                    if (StartIndex == 0) return;
                    StartIndex = 0;
                    break;
                case Keys.Down:
                    if (StartIndex == History.Count - 1) return;
                    StartIndex++;
                    break;
                case Keys.End:
                    if (StartIndex == History.Count - 1) return;
                    StartIndex = History.Count - 1;
                    break;
                case Keys.PageUp:
                    if (StartIndex == 0) return;
                    StartIndex -= LineCount;
                    break;
                case Keys.PageDown:
                    if (StartIndex == History.Count - 1) return;
                    StartIndex += LineCount;
                    break;
                default:
                    return;
            }
            Update();
            e.Handled = true;
        }
        private void ChatPanel_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '@':
                case '!':
                case ' ':
                case (char)Keys.Enter:
                    ChatTextBox.SetFocus();
                    if (e.KeyChar == '!') ChatTextBox.Text = "!";
                    if (e.KeyChar == '@') ChatTextBox.Text = "@";
                    if (ChatPrefix != "") ChatTextBox.Text = ChatPrefix;

                    ChatTextBox.Visible = /*ChatTextBoxWin10Label.Visible = */true;
                    ChatTextBox.TextBox.SelectionLength = 0;
                    ChatTextBox.TextBox.SelectionStart = ChatTextBox.Text.Length;
                    e.Handled = true;
                    break;
                case '/':
                    ChatTextBox.SetFocus();
                    ChatTextBox.Text = LastPM + " ";
                    ChatTextBox.Visible =/* ChatTextBoxWin10Label.Visible = */true;
                    ChatTextBox.TextBox.SelectionLength = 0;
                    ChatTextBox.TextBox.SelectionStart = ChatTextBox.Text.Length;
                    e.Handled = true;
                    break;
            }
        }
        private void ChatPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            int count = e.Delta / SystemInformation.MouseWheelScrollDelta;

            if (StartIndex == 0 && count >= 0) return;
            if (StartIndex == History.Count - 1 && count <= 0) return;

            StartIndex -= count;
            Update();
        }
        private void ChatTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            CMain.Shift = e.Shift;
            CMain.Alt = e.Alt;
            CMain.Ctrl = e.Control;

            switch (e.KeyCode)
            {
                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                case Keys.Tab:
                    CMain.CMain_KeyUp(sender, e);
                    break;

            }
        }
        private void ChatTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            CMain.Shift = e.Shift;
            CMain.Alt = e.Alt;
            CMain.Ctrl = e.Control;

            switch (e.KeyCode)
            {
                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                case Keys.Tab:
                    CMain.CMain_KeyDown(sender, e);
                    break;

            }
        }


        public void ChangeSize()
        {
            if (++WindowSize >= 3) WindowSize = 0;

            int y = DisplayRectangle.Height;
            switch (WindowSize)
            {
                case 0:
                    LineCount = 11;
                    Index = Settings.Resolution != 800 ? 2221 : 2201;
                    CountBar.Index = 2012;
                    DownButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 39);
                    EndButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 45);
                    ChatTextBox.Location = new Point(1, 104);
                    break;
                case 1:
                    LineCount = 11;
                    Index = Settings.Resolution != 800 ? 2224 : 2204;
                    CountBar.Index = 2013;
                    DownButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 39 + 48);
                    EndButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 45 + 48);
                    ChatTextBox.Location = new Point(1, 54);
                    break;
                case 2:
                    LineCount = 11;
                    Index = Settings.Resolution != 800 ? 2227 : 2207;
                    CountBar.Index = 2014;
                    DownButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 39 + 96);
                    EndButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 45 + 96);
                    ChatTextBox.Location = new Point(1, 54);
                    break;
            }

            Location = new Point(Location.X, y - Size.Height);

            UpdateBackground();

            Update();
        }

        public void UpdateBackground()
        {
            int offset = Transparent ? 1 : 0;

            switch (WindowSize)
            {
                case 0:
                    Index = 2207;
                    break;
                case 1:
                    Index = 2207;
                    break;
                case 2:
                    Index = 2207;
                    break;
            }

            Index -= offset;
        }

        public class ChatHistory
        {
            public string Text;
            public Color ForeColour, BackColour;
            public ChatType Type;
        }
    }
    public sealed class ChatControlBar : MirImageControl
    {
        public ChatDialog chatDialog;
        public MirButton SizeButton, SettingsButton, NormalButton, ShoutButton, WhisperButton, LoverButton, MentorButton, GroupButton, GuildButton, ReportButton, TradeButton;
       

        public ChatControlBar()
        {
            //Index = Settings.Resolution != 800 ? 2034 : 2035;
            //Index = 261;
            Index = 263;
            Library = Libraries.UI;
            Movable = true;
            Location = new Point(500, Settings.ScreenHeight - 280);
            DrawBorder();
            //Size = new Size(405, 200);
            //Location = new Point(GameScene.Scene.MainDialog.Location.X + 230, Settings.ScreenHeight - 112);

            chatDialog = new ChatDialog
            {
                Parent = this,
                Location = new Point(0, 15),
                NotControl = false
            };

            #region sizing
            //SizeButton = new MirButton
            //{
            //    Index = 252,
            //    HoverIndex = 253,
            //    PressedIndex = 254,
            //    Library = Libraries.UI,
            //    Parent = this,
            //    //Location = new Point(Settings.Resolution != 800 ? 574 : 350, 1),
            //    Location = new Point(350, 1),
            //    Visible = true,
            //    Sound = SoundList.ButtonA,
            //    Hint = "Size"
            //};
            //SizeButton.Click += (o, e) =>
            //{
            //    //this.chatDialog.ChangeSize();
            //    //Location = new Point(Location.X, this.chatDialog.DisplayRectangle.Top - Size.Height);
            //    //if (GameScene.Scene.BeltDialog.Index == 1932)
            //    //    GameScene.Scene.BeltDialog.Location = new Point(GameScene.Scene.MainDialog.Location.X + 230, Location.Y - GameScene.Scene.BeltDialog.Size.Height);
            //};
            #endregion

            SettingsButton = new MirButton
            {
                Index = 258,
                HoverIndex = 259,
                PressedIndex = 260,
                Library = Libraries.UI,
                Parent = this,
                //Location = new Point(Settings.Resolution != 800 ? 596 : 372, 1),
                Location = new Point(387, 1),
                Sound = SoundList.ButtonA,
                Hint = "Chat Settings"
            };
            SettingsButton.Click += (o, e) =>
            {
                /*
                    if (GameScene.Scene.ChatOptionDialog.Visible)
                        GameScene.Scene.ChatOptionDialog.Hide();
                    else
                        GameScene.Scene.ChatOptionDialog.Show();
                */
                // this.chatDialog.ChangeSize();
                // Location = new Point(Location.X, this.chatDialog.DisplayRectangle.Top - Size.Height);
                // this.chatDialog.Transparent = !this.chatDialog.Transparent;
                // GameScene.Scene.ChatDialog.UpdateBackground();
            };

            NormalButton = new MirButton
            {
                Index = 231,
                HoverIndex = 232,
                PressedIndex = 233,
                Library = Libraries.UI,
                Parent = this,
                Location = new Point(12, 1),
                Sound = SoundList.ButtonA,
                Hint = "All"
            };
            NormalButton.Click += (o, e) =>
            {
                ToggleChatFilter("All");
            };

            ShoutButton = new MirButton
            {
                Index = 234,
                HoverIndex = 235,
                PressedIndex = 236,
                Library = Libraries.UI,
                Parent = this,
                Location = new Point(34, 1),
                Sound = SoundList.ButtonA,
                Hint = "Shout"
            };
            ShoutButton.Click += (o, e) =>
            {
                ToggleChatFilter("Shout");
            };

            WhisperButton = new MirButton
            {
                Index = 237,
                HoverIndex = 238,
                PressedIndex = 239,
                Library = Libraries.UI,
                Parent = this,
                Location = new Point(56, 1),
                Sound = SoundList.ButtonA,
                Hint = "Whisper"
            };
            WhisperButton.Click += (o, e) =>
            {
                ToggleChatFilter("Whisper");
            };

            LoverButton = new MirButton
            {
                Index = 240,
                HoverIndex = 241,
                PressedIndex = 242,
                Library = Libraries.UI,
                Parent = this,
                Location = new Point(78, 1),
                Sound = SoundList.ButtonA,
                Hint = "Lover"
            };
            LoverButton.Click += (o, e) =>
            {
                ToggleChatFilter("Lover");
            };

            MentorButton = new MirButton
            {
                Index = 243,
                HoverIndex = 244,
                PressedIndex = 245,
                Library = Libraries.UI,
                Parent = this,
                Location = new Point(100, 1),
                Sound = SoundList.ButtonA,
                Hint = "Mentor"
            };
            MentorButton.Click += (o, e) =>
            {
                ToggleChatFilter("Mentor");
            };

            GroupButton = new MirButton
            {
                Index = 246,
                HoverIndex = 247,
                PressedIndex = 248,
                Library = Libraries.UI,
                Parent = this,
                Location = new Point(122, 1),
                Sound = SoundList.ButtonA,
                Hint = "Group"
            };
            GroupButton.Click += (o, e) =>
            {
                ToggleChatFilter("Group");
            };

            GuildButton = new MirButton
            {
                Index = 249,
                HoverIndex = 250,
                PressedIndex = 251,
                Library = Libraries.UI,
                Parent = this,
                Location = new Point(144, 1),
                Sound = SoundList.ButtonA,
                Hint = "Guild"
            };
            GuildButton.Click += (o, e) =>
            {
                Settings.ShowGuildChat = !Settings.ShowGuildChat;
                ToggleChatFilter("Guild");
            };

            TradeButton = new MirButton
            {
                Index = 2004,
                HoverIndex = 2005,
                PressedIndex = 2006,
                Library = Libraries.Prguse,
                Location = new Point(166, 1),
                Parent = this,
                Sound = SoundList.ButtonC,
                Hint = "Trade (" + CMain.InputKeys.GetKey(KeybindOptions.Trade) + ")",
                Visible = false
            };
            TradeButton.Click += (o, e) => Network.Enqueue(new C.TradeRequest());

            ReportButton = new MirButton
            {
                Index = 2063,
                HoverIndex = 2064,
                PressedIndex = 2065,
                Library = Libraries.Prguse,
                Parent = this,
                //Location = new Point(Settings.Resolution != 800 ? 552 : 328, 1),
                Location = new Point(328, 1),
                Sound = SoundList.ButtonA,
                Hint = "Report",
                Visible = false
            };
            ReportButton.Click += (o, e) =>
            {
                GameScene.Scene.ReportDialog.Visible = !GameScene.Scene.ReportDialog.Visible;
            };

            ToggleChatFilter("All");
        }

        private void ChatControlBar_BeforeDraw(object sender, EventArgs e)
        {
            
        }

        public void ToggleChatFilter(string chatFilter)
        {
            NormalButton.Index = 231;
            NormalButton.HoverIndex = 232;
            ShoutButton.Index = 234;
            ShoutButton.HoverIndex = 235;
            WhisperButton.Index = 237;
            WhisperButton.HoverIndex = 238;
            LoverButton.Index = 240;
            LoverButton.HoverIndex = 241;
            MentorButton.Index = 243;
            MentorButton.HoverIndex = 244;
            GroupButton.Index = 246;
            GroupButton.HoverIndex = 247;
            GuildButton.Index = 249;
            GuildButton.HoverIndex = 250;

            this.chatDialog.ChatPrefix = "";

            switch (chatFilter)
            {
                case "All":
                    NormalButton.Index = 233;
                    NormalButton.HoverIndex = 233;
                    this.chatDialog.ChatPrefix = "";
                    break;
                case "Shout":
                    ShoutButton.Index = 236;
                    ShoutButton.HoverIndex = 236;
                    this.chatDialog.ChatPrefix = "!";
                    break;
                case "Whisper":
                    WhisperButton.Index = 239;
                    WhisperButton.HoverIndex = 239;
                    this.chatDialog.ChatPrefix = "/";
                    break;
                case "Group":
                    GroupButton.Index = 248;
                    GroupButton.HoverIndex = 248;
                    this.chatDialog.ChatPrefix = "!!";
                    break;
                case "Guild":
                    GuildButton.Index = 251;
                    GuildButton.HoverIndex = 251;
                    this.chatDialog.ChatPrefix = "!~";
                    break;
                case "Lover":
                    LoverButton.Index = 242;
                    LoverButton.HoverIndex = 242;
                    this.chatDialog.ChatPrefix = ":)";
                    break;
                case "Mentor":
                    MentorButton.Index = 245;
                    MentorButton.HoverIndex = 245;
                    this.chatDialog.ChatPrefix = "!#";
                    break;
            }
        }

        public void Show()
        {
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }
    }
}
