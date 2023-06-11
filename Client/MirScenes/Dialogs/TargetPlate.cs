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
    public sealed class TargetPlate : MirImageControl
    {
        public static UserObject User
        {
            get { return MapObject.User; }
            set { MapObject.User = value; }
        }

        public MirImageControl Plate, Avatar, HealthBar;
        public MirLabel TargetName, HealthPercentage, Level;
        public MapObject mapObject;
        public Color NameColour = Color.White;


        public TargetPlate()
        {
            Index = 386;
            Library = Libraries.UI;
            Location = new Point(570, 0);
            PixelDetect = true;

            Avatar = new MirImageControl
            {
                Parent = this,
                Location = new Point(189, 7),
                Library = Libraries.MonIcon,
                Index = 0,
            };
            TargetName = new MirLabel
            {
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Font = new Font(Settings.FontName, 8F),
                Location = new Point(15, 20),
                Parent = this,
                Size = new Size(160, 17),
                ForeColour = Color.White
            };
            HealthPercentage = new MirLabel
            {
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Font = new Font(Settings.FontName, 8F),
                Location = new Point(42, 39),
                Parent = this,
                Size = new Size(136, 13)
            };
            HealthPercentage.BeforeDraw += HealthPercentage_BeforeDraw;
            Level = new MirLabel
            {
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Font = new Font(Settings.FontName, 8F),
                Location = new Point(this.Size.Width - 86, 60),
                Parent = this,
                Size = new Size(33, 13)
            };
        }

        private void HealthPercentage_BeforeDraw(object sender, EventArgs e)
        {
            if (Visible == false) return;
            if (mapObject == null) return;
            if (mapObject.PercentHealth == 0) return;

            //Libraries.UI.Draw(265, new Rectangle(0, 0, (int)(Libraries.UI.GetTrueSize(265).Width * MapObject.TargetObject.PercentHealth / 100F), Libraries.UI.GetTrueSize(265).Height), new Point(this.Location.X + 26, this.Location.Y + 41), Color.White, false);
            Libraries.UI.Draw(265, new Rectangle(0, 0, (int)(Libraries.UI.GetTrueSize(265).Width * mapObject.PercentHealth / 100F), Libraries.UI.GetTrueSize(265).Height), new Point(this.Location.X + 26, this.Location.Y + 41), Color.White, false);
            HealthPercentage.BringToFront();
        }

        public void Process()
        {
            mapObject = (MapObject.TargetObject != null) ? MapObject.TargetObject : (MapObject.MouseObject != null) ? MapObject.MouseObject : null;
            Visible = (mapObject != null && !mapObject.Dead && (mapObject.Race == ObjectType.Monster || mapObject.Race == ObjectType.Player));
            if (mapObject == null) return;

            if (mapObject.Race == ObjectType.Monster || mapObject.Race == ObjectType.Player)
            {

                TargetName.Text = mapObject.Name;
                Level.Text = mapObject.MobLevel.ToString();


                if (mapObject.MobLevel == 0)
                    Index = 388;
                else if (mapObject.MobLevel == 1)
                    Index = 390;
                else if (mapObject.MobLevel == 2)
                    Index = 391;
                else if (mapObject.MobLevel == 3)
                    Index = 392;
                else if (mapObject.MobLevel == 4)
                    Index = 393;
                else if (mapObject.MobLevel == 5)
                    Index = 394;
                else if (mapObject.MobLevel == 6)
                    Index = 395;
                else Index = 388;


                switch (Settings.TargetPlateHPDisplay)
                {
                    case TargetPlateHPDisplay.None:
                        HealthPercentage.Text = string.Empty;
                        break;
                    case TargetPlateHPDisplay.HPRemaining:
                        HealthPercentage.Text = mapObject.CurrentHP.ToString();
                        break;
                    case TargetPlateHPDisplay.HPRemainingAndMaxHP:
                        HealthPercentage.Text = string.Format("{0}/{1}", mapObject.CurrentHP, mapObject.MaximumHP);
                        break;
                    case TargetPlateHPDisplay.PercentageRemaining:
                        HealthPercentage.Text = string.Format("{0}%", mapObject.PercentHealth);
                        break;
                    case TargetPlateHPDisplay.PercentageRemainingAndMaxPercentage:
                        HealthPercentage.Text = string.Format("{0}% / 100%", mapObject.PercentHealth);
                        break;
                    case TargetPlateHPDisplay.HPRemainingAndPercentageRemaining:
                        HealthPercentage.Text = string.Format("{0} ({1}%)", mapObject.CurrentHP, mapObject.PercentHealth);
                        break;
                    case TargetPlateHPDisplay.HPRemainingAndMaxHPAndPercentageRemaining:
                        HealthPercentage.Text = string.Format("{0} / {1} ({2}%)", mapObject.CurrentHP, mapObject.MaximumHP, mapObject.PercentHealth);
                        break;
                    case TargetPlateHPDisplay.HPRemainingAndMaxHPAndPercentageRemainingAndMaxPercentage:
                        HealthPercentage.Text = string.Format("{0} / {1} ({2}% / 100%)", mapObject.CurrentHP, mapObject.MaximumHP, mapObject.PercentHealth);
                        break;
                    default:
                        HealthPercentage.Text = string.Empty;
                        break;
                }
            }
            if (mapObject.Race == ObjectType.Monster)
            {
                MonsterObject monsterObject = mapObject as MonsterObject;
                Avatar.Library = Libraries.MonIcon;
                Avatar.Index = (int)monsterObject.BaseImage;

/*
                switch (mapObject.MobLevel)
                {
                    case 0:
                        Index = 388;
                        break;
                    case 1:
                        Index = 390;
                        break;
                    case 2:
                        Index = 391;
                        break;
                    case 3:
                        Index = 392;
                        break;
                    case 4:
                        Index = 393;
                        break;
                    case 5:
                        Index = 394;
                        break;
                    case 6:
                        Index = 395;
                        break;
                }
*/
            }
            else if (mapObject.Race == ObjectType.Player)
            {
                Avatar.Library = Libraries.UI;
                PlayerObject player = mapObject as PlayerObject;

                switch (player.Class)
                {
                    case MirClass.Warrior:
                        Avatar.Index = (User.Gender == MirGender.Male) ? 121 : 122;
                        break;
                    case MirClass.Wizard:
                        Avatar.Index = (User.Gender == MirGender.Male) ? 123 : 124;
                        break;
                    case MirClass.Taoist:
                        Avatar.Index = (User.Gender == MirGender.Male) ? 125 : 126;
                        break;
                    case MirClass.Assassin:
                        Avatar.Index = (User.Gender == MirGender.Male) ? 127 : 128;
                        break;
                    case MirClass.Archer:
                        break;
                    default:
                        break;
                }
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