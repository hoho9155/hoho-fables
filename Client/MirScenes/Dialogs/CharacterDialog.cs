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
    public sealed class CharacterDialog : MirImageControl
    {
        public MirButton CloseButton, CharacterButton, StatusButton, SkillButton, RuneButton, ElementalButton;
        public MirImageControl CharacterPage, StatusPage, SkillPage, RunePage, HermitPage;

        public MirLabel NameLabel, GuildLabel, LoverLabel;
        public MirLabel ACLabel, MACLabel, DCLabel, MCLabel, SCLabel, HealthLabel, ManaLabel;
        public MirLabel CritRLabel, CritDLabel, LuckLabel, AttkSpdLabel, AccLabel, AgilLabel;
        public MirLabel ExpPLabel, BagWLabel, WearWLabel, HandWLabel, MagicRLabel, PoisonRecLabel, HealthRLabel, ManaRLabel, PoisonResLabel, HolyTLabel, FreezeLabel, PoisonAtkLabel;
        public MirLabel HeadingLabel, StatLabel;
        public MirLabel SkillPageNumber;
        public MirLabel HealthPotion, ManaPotion, AC, MAC, DC, MC, SC, CritRate, CritDamage, AttackSpeed, Accuracy, Agility, Luck;
        public MirLabel Experience, BagWeight, ArmourWeight, HandWeight, MagicResistance, PoisonResistance, HealthRecovery, ManaRecovery, PoisonRecovery, HolyPower, FrostPower, PoisonPower;
        public MirButton NextButton, BackButton;
        Point DisplayLocationWithOffset;

        public MirItemCell[] Grid;
        public MagicButton[] Magics;

        public int StartIndex;

        public CharacterDialog()
        {
            Index = 266;
            Library = Libraries.UI;
            Location = new Point(Settings.ScreenWidth - Libraries.UI.GetTrueSize(Index).Width, 0);
            Movable = true;
            Sort = true;
            BeforeDraw += (o, e) => RefreshInterface();

            PreDrawSkills();

            CharacterPage = new MirImageControl
            {
                Index = 267,
                Parent = this,
                Library = Libraries.UI,
                Location = new Point(5, 42),
            };
            CharacterPage.AfterDraw += (o, e) =>
            {
                if (Libraries.StateItems == null) return;
                ItemInfo RealItem = null;
                if (Grid[(int)EquipmentSlot.Armour].Item != null)
                {
                    if (GameScene.User.WingEffect == 1 || GameScene.User.WingEffect == 2)
                    {
                        int wingOffset = GameScene.User.WingEffect == 1 ? 2 : 4;

                        int genderOffset = MapObject.User.Gender == MirGender.Male ? 0 : 1;

                        Libraries.Prguse2.DrawBlend(1200 + wingOffset + genderOffset, DisplayLocationWithOffset, Color.White, true, 1F);
                    }

                    RealItem = Functions.GetRealItem(Grid[(int)EquipmentSlot.Armour].Item.Info, MapObject.User.Level, MapObject.User.Class, GameScene.ItemInfoList);
                    Libraries.StateItems.Draw(RealItem.Image, DisplayLocationWithOffset, Color.White, true, 1F);

                }
                if (Grid[(int)EquipmentSlot.Weapon].Item != null)
                {
                    RealItem = Functions.GetRealItem(Grid[(int)EquipmentSlot.Weapon].Item.Info, MapObject.User.Level, MapObject.User.Class, GameScene.ItemInfoList);

                    Libraries.StateItems.Draw(RealItem.Image, DisplayLocationWithOffset, Color.White, true, 1F);
                }
                if (Grid[(int)EquipmentSlot.Shield].Item != null)
                {
                    RealItem = Functions.GetRealItem(Grid[(int)EquipmentSlot.Shield].Item.Info, MapObject.User.Level, MapObject.User.Class, GameScene.ItemInfoList);

                    Libraries.StateItems.Draw(RealItem.Image, DisplayLocationWithOffset, Color.White, true, 1F);
                }

                if (Grid[(int)EquipmentSlot.Helmet].Item != null)
                    Libraries.StateItems.Draw(Grid[(int)EquipmentSlot.Helmet].Item.Info.Image, DisplayLocationWithOffset, Color.White, true, 1F);
                else
                {
                    if (Grid[(int)EquipmentSlot.Armour].Item != null && (Grid[(int)EquipmentSlot.Armour].Item.Image >= 4085 && Grid[(int)EquipmentSlot.Armour].Item.Image <= 4168))
                    {

                    }
                    else
                    {
                        int hair = 441 + MapObject.User.Hair + (MapObject.User.Class == MirClass.Assassin ? 20 : 0) + (MapObject.User.Gender == MirGender.Male ? 0 : 40);

                        int offSetX = MapObject.User.Class == MirClass.Assassin ? (MapObject.User.Gender == MirGender.Male ? 6 : 4) : 0;
                        int offSetY = MapObject.User.Class == MirClass.Assassin ? (MapObject.User.Gender == MirGender.Male ? 25 : 18) : 0;

                        Libraries.Prguse.Draw(hair, new Point(DisplayLocationWithOffset.X + offSetX, DisplayLocationWithOffset.Y + offSetY), Color.White, true, 1F);
                    }
                }
            };


            StatusPage = new MirImageControl
            {
                Index = 279,
                Parent = this,
                Library = Libraries.UI,
                Location = new Point(5, 42),
                Visible = false,
            };
            StatusPage.BeforeDraw += (o, e) =>
            {
                ACLabel.Text = string.Format("{0}-{1}", MapObject.User.MinAC, MapObject.User.MaxAC);
                MACLabel.Text = string.Format("{0}-{1}", MapObject.User.MinMAC, MapObject.User.MaxMAC);
                DCLabel.Text = string.Format("{0}-{1}", MapObject.User.MinDC, MapObject.User.MaxDC);
                MCLabel.Text = string.Format("{0}-{1}", MapObject.User.MinMC, MapObject.User.MaxMC);
                SCLabel.Text = string.Format("{0}-{1}", MapObject.User.MinSC, MapObject.User.MaxSC);
                HealthLabel.Text = string.Format("{0}/{1}", MapObject.User.HP, MapObject.User.MaxHP);
                ManaLabel.Text = string.Format("{0}/{1}", MapObject.User.MP, MapObject.User.MaxMP);
                CritRLabel.Text = string.Format("{0}%", MapObject.User.CriticalRate);
                CritDLabel.Text = string.Format("{0}", MapObject.User.CriticalDamage);
                AttkSpdLabel.Text = string.Format("{0}", MapObject.User.ASpeed);
                AccLabel.Text = string.Format("+{0}", MapObject.User.Accuracy);
                AgilLabel.Text = string.Format("+{0}", MapObject.User.Agility);
                LuckLabel.Text = string.Format("{0}", MapObject.User.Luck);

                ExpPLabel.Text = string.Format("{0:0.##%}", MapObject.User.Experience / (double)MapObject.User.MaxExperience);
                BagWLabel.Text = string.Format("{0}/{1}", MapObject.User.CurrentBagWeight, MapObject.User.MaxBagWeight);
                WearWLabel.Text = string.Format("{0}/{1}", MapObject.User.CurrentWearWeight, MapObject.User.MaxWearWeight);
                HandWLabel.Text = string.Format("{0}/{1}", MapObject.User.CurrentHandWeight, MapObject.User.MaxHandWeight);
                MagicRLabel.Text = string.Format("+{0}", MapObject.User.MagicResist);
                PoisonResLabel.Text = string.Format("+{0}", MapObject.User.PoisonResist);
                HealthRLabel.Text = string.Format("+{0}", MapObject.User.HealthRecovery);
                ManaRLabel.Text = string.Format("+{0}", MapObject.User.SpellRecovery);
                PoisonRecLabel.Text = string.Format("+{0}", MapObject.User.PoisonRecovery);
                HolyTLabel.Text = string.Format("+{0}", MapObject.User.Holy);
                FreezeLabel.Text = string.Format("+{0}", MapObject.User.Freezing);
                PoisonAtkLabel.Text = string.Format("+{0}", MapObject.User.PoisonAttack);
            };

            RunePage = new MirImageControl
            {
                Index = 281,
                Parent = this,
                Library = Libraries.UI,
                Location = new Point(5, 42),
                Visible = false,
            };
            HermitPage = new MirImageControl
            {
                Index = 280,
                Parent = this,
                Library = Libraries.UI,
                Location = new Point(5, 42),
                Visible = false,
            };

            SkillPage = new MirImageControl
            {
                Index = 268,
                Parent = this,
                Library = Libraries.UI,
                Location = new Point(5, 42),
                Visible = false
            };

            CharacterButton = new MirButton
            {
                Index = 269,
                Library = Libraries.UI,
                Location = new Point(43, 353),
                Parent = this,
                PressedIndex = 269,
                Size = new Size(47, 35),
                Sound = SoundList.ButtonA,
                Hint = "Equipment"
            };
            CharacterButton.Click += (o, e) => ShowCharacterPage();

            StatusButton = new MirButton
            {
                Library = Libraries.UI,
                Index = 277,
                Location = new Point(247, 353),
                Parent = this,
                PressedIndex = 277,
                Size = new Size(47, 35),
                Sound = SoundList.ButtonA,
                Hint = "Stats && Information"
            };
            StatusButton.Click += (o, e) => ShowStatusPage();

            SkillButton = new MirButton
            {
                Library = Libraries.UI,
                Index = 271,
                Location = new Point(94, 353),
                Parent = this,
                PressedIndex = 271,
                Size = new Size(47, 35),
                Sound = SoundList.ButtonA,
                Hint = "Skills && Magic"
            };
            SkillButton.Click += (o, e) => ShowSkillPage();

            RuneButton = new MirButton
            {
                Library = Libraries.UI,
                Index = 273,
                Location = new Point(145, 353),
                Parent = this,
                PressedIndex = 273,
                Size = new Size(47, 35),
                Sound = SoundList.ButtonA,
                Hint = "Runes"
            };
            RuneButton.Click += (o, e) => ShowRunePage();

            ElementalButton = new MirButton
            {
                Library = Libraries.UI,
                Index = 275,
                Location = new Point(196, 353),
                Parent = this,
                PressedIndex = 275,
                Size = new Size(47, 35),
                Sound = SoundList.ButtonA,
                Hint = "Elemental Attributes"
            };
            ElementalButton.Click += (o, e) => ShowHermitPage();

            CloseButton = new MirButton
            {
                Parent = this,
                Library = Libraries.UI,
                Index = 1,
                HoverIndex = 2,
                PressedIndex = 3,
                Location = new Point(this.Size.Width - 30, 16)
            };
            CloseButton.Click += (o, e) => Hide(); ;

            NameLabel = new MirLabel
            {
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(-120, 15),
                Size = new Size(Libraries.UI.GetTrueSize(CharacterPage.Index).Width, 15),
                NotControl = true,
            };
            GuildLabel = new MirLabel
            {
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = CharacterPage,
                Location = new Point(-120, 0),
                Size = new Size(Libraries.UI.GetTrueSize(CharacterPage.Index).Width, 30),
                NotControl = true,
            };

            Grid = new MirItemCell[Enum.GetNames(typeof(EquipmentSlot)).Length];
            Grid[(int)EquipmentSlot.Shield] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Shield,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(165, 269),
               // Hint = "Shield",
                Border = true,
                BorderColour = Color.Chartreuse
            };
            Grid[(int)EquipmentSlot.Weapon] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Weapon,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(121, 269),
                // Hint = "Weapon"
                Border = true,
                BorderColour = Color.Chartreuse
            };
            Grid[(int)EquipmentSlot.Armour] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Armour,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(7, 81),
                // Hint ="Armour"
                Border = true,
                BorderColour = Color.Chartreuse
            };
            Grid[(int)EquipmentSlot.Helmet] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Helmet,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(7, 41),
                // Hint = "Helmet"
                Border = true,
                BorderColour = Color.Chartreuse
            };
            Grid[(int)EquipmentSlot.Torch] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Torch,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(285, 241),
                //Hint = "Torch / Auras"
                Border = true,
                BorderColour = Color.Chartreuse
            };
            Grid[(int)EquipmentSlot.Necklace] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Necklace,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(285, 41),
                // Hint = "Necklace"
                Border = true,
                BorderColour = Color.Chartreuse
            };
            Grid[(int)EquipmentSlot.BraceletL] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.BraceletL,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(285, 81),
                // Hint = "Left Hand Bracelet / Glove"
                Border = true,
                BorderColour = Color.Chartreuse
            };
            Grid[(int)EquipmentSlot.BraceletR] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.BraceletR,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(285, 121),
                // Hint = "Right Hand Bracelet / Glove"
                Border = true,
                BorderColour = Color.Chartreuse
            };
            Grid[(int)EquipmentSlot.RingL] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.RingL,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(285, 161),
                // Hint = "Left Hand Ring"
                Border = true,
                BorderColour = Color.Chartreuse
            };
            Grid[(int)EquipmentSlot.RingR] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.RingR,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(285, 201),
                // Hint = "Right Hand Ring"
                Border = true,
                BorderColour = Color.Chartreuse
            };
            Grid[(int)EquipmentSlot.Amulet] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Amulet,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(7, 241),
                // Hint = "Amulet / Poison"
                Border = true,
                BorderColour = Color.Chartreuse
            };
            Grid[(int)EquipmentSlot.Boots] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Boots,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(7, 161),
                //Hint = "Boots"
                Border = true,
                BorderColour = Color.Chartreuse
            };
            Grid[(int)EquipmentSlot.Belt] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Belt,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(7, 121),
                // Hint = "Belt"
                Border = true,
                BorderColour = Color.Chartreuse
            };
            Grid[(int)EquipmentSlot.Stone] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Stone,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(7, 201),
                // Hint = "Stone"
                Border = true,
                BorderColour = Color.Chartreuse
            };
            Grid[(int)EquipmentSlot.Mount] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Mount,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(209, 269),
                Visible = true,
                //  Hint = "Mount",
                Border = true,
                BorderColour = Color.Chartreuse
            };

            // STATS I
            HealthPotion = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(10, 20),
                Size = new Size(70, 13),
                NotControl = true,
                Text = "Health Points",
                DrawFormat = TextFormatFlags.Right
            };
            HealthLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(85, 20),
                NotControl = true,
                Text = "0-0",
            };
            ManaPotion = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(10, 38),
                Size = new Size(70, 13),
                NotControl = true,
                Text = "Mana Points",
                DrawFormat = TextFormatFlags.Right
            };
            ManaLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(85, 38),
                NotControl = true,
                Text = "0-0",
            };
            AC = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(10, 56),
                Size = new Size(70, 13),
                NotControl = true,
                Text = "AC",
                DrawFormat = TextFormatFlags.Right,
                Hint = "Armour Charm"
            };
            ACLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(85, 56),
                NotControl = true,
                Text = "0-0",
            };
            MAC = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(10, 74),
                Size = new Size(70, 13),
                NotControl = true,
                Text = "MAC",
                DrawFormat = TextFormatFlags.Right
            };
            MACLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(85, 74),
                NotControl = true,
                Text = "0-0",
            };
            DC = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(10, 92),
                Size = new Size(70, 13),
                NotControl = true,
                Text = "DC",
                DrawFormat = TextFormatFlags.Right
            };
            DCLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(85, 92),
                NotControl = true,
                Text = "0-0"
            };
            MC = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(10, 110),
                Size = new Size(70, 13),
                NotControl = true,
                Text = "MC",
                DrawFormat = TextFormatFlags.Right
            };
            MCLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(85, 110),
                NotControl = true,
                Text = "0/0"
            };
            SC = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(10, 128),
                Size = new Size(70, 13),
                NotControl = true,
                Text = "SC",
                DrawFormat = TextFormatFlags.Right
            };
            SCLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(85, 128),
                NotControl = true,
                Text = "0/0"
            };
            CritRate = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(10, 146),
                Size = new Size(70, 13),
                NotControl = true,
                Text = "Crit Rate",
                DrawFormat = TextFormatFlags.Right
            };
            CritRLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(85, 146),
                NotControl = true
            };
            CritDamage = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(10, 164),
                Size = new Size(70, 13),
                NotControl = true,
                Text = "Crit Damage",
                DrawFormat = TextFormatFlags.Right
            };
            CritDLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(85, 164),
                NotControl = true
            };
            AttackSpeed = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(10, 182),
                Size = new Size(70, 13),
                NotControl = true,
                Text = "Attack Speed",
                DrawFormat = TextFormatFlags.Right
            };
            AttkSpdLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(85, 182),
                NotControl = true
            };
            Accuracy = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(10, 200),
                Size = new Size(70, 13),
                NotControl = true,
                Text = "Accuracy",
                DrawFormat = TextFormatFlags.Right
            };
            AccLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(85, 200),
                NotControl = true
            };
            Agility = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(10, 218),
                Size = new Size(70, 13),
                NotControl = true,
                Text = "Agility",
                DrawFormat = TextFormatFlags.Right
            };
            AgilLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(85, 218),
                NotControl = true
            };
            Luck = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(10, 236),
                Size = new Size(70, 13),
                NotControl = true,
                Text = "Luck",
                DrawFormat = TextFormatFlags.Right
            };
            LuckLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(85, 236),
                NotControl = true
            };


            // STATS II 
            Experience = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(180, 20),
                Size = new Size(85, 13),
                NotControl = true,
                Text = "Experience",
                DrawFormat = TextFormatFlags.Right
            };
            ExpPLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(270, 20),
                NotControl = true,
                Text = "0-0",
            };
            BagWeight = new MirLabel
            {
                Visible = false,
                Parent = StatusPage,
                Location = new Point(180, 38),
                Size = new Size(85, 13),
                NotControl = true,
                Text = "Bag Weight",
                DrawFormat = TextFormatFlags.Right
            };
            BagWLabel = new MirLabel
            {
                Visible = false,
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(270, 38),
                NotControl = true,
                Text = "0-0",
            };
            ArmourWeight = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(180, 56),
                Size = new Size(85, 13),
                NotControl = true,
                Text = "Body Weight",
                DrawFormat = TextFormatFlags.Right
            };
            WearWLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(270, 56),
                NotControl = true,
                Text = "0-0",
            };
            HandWeight = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(180, 74),
                Size = new Size(85, 13),
                NotControl = true,
                Text = "Hand Weight",
                DrawFormat = TextFormatFlags.Right
            };
            HandWLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(270, 74),
                NotControl = true,
                Text = "0-0",
            };
            MagicResistance = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(175, 92),
                Size = new Size(90, 13),
                NotControl = true,
                Text = "Magic Resistance",
                DrawFormat = TextFormatFlags.Right
            };
            MagicRLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(270, 92),
                NotControl = true,
                Text = "0-0"
            };
            PoisonResistance = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(175, 110),
                Size = new Size(90, 13),
                NotControl = true,
                Text = "Poison Resistance",
                DrawFormat = TextFormatFlags.Right
            };
            PoisonResLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(270, 110),
                NotControl = true,
                Text = "0/0"
            };
            HealthRecovery = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(180, 128),
                Size = new Size(85, 13),
                NotControl = true,
                Text = "Health Recovery",
                DrawFormat = TextFormatFlags.Right
            };
            HealthRLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(270, 128),
                NotControl = true,
                Text = "0/0"
            };
            ManaRecovery = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(180, 146),
                Size = new Size(85, 13),
                NotControl = true,
                Text = "Mana Recovery",
                DrawFormat = TextFormatFlags.Right
            };
            ManaRLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(270, 146),
                NotControl = true
            };
            PoisonRecovery = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(180, 164),
                Size = new Size(85, 13),
                NotControl = true,
                Text = "Poison Recovery",
                DrawFormat = TextFormatFlags.Right
            };
            PoisonRecLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(270, 164),
                NotControl = true
            };
            HolyPower = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(180, 182),
                Size = new Size(85, 13),
                NotControl = true,
                Text = "Holy Power",
                DrawFormat = TextFormatFlags.Right
            };
            HolyTLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(270, 182),
                NotControl = true
            };
            FrostPower = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(180, 200),
                Size = new Size(85, 13),
                NotControl = true,
                Text = "Frost Power",
                DrawFormat = TextFormatFlags.Right
            };
            FreezeLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(270, 200),
                NotControl = true
            };
            PoisonPower = new MirLabel
            {
                Parent = StatusPage,
                Location = new Point(180, 218),
                Size = new Size(85, 13),
                NotControl = true,
                Text = "Poison Power",
                DrawFormat = TextFormatFlags.Right
            };
            PoisonAtkLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(270, 218),
                NotControl = true
            };

            Magics = new MagicButton[8];

            for (int i = 0; i < Magics.Length; i++)
            {
                Magics[i] = new MagicButton { Parent = SkillPage, Visible = false, Location = new Point(2, 4 + i * 34) };
            }

            NextButton = new MirButton
            {
                Index = 396,
                Library = Libraries.Prguse,
                Parent = SkillPage,
                PressedIndex = 397,
                Sound = SoundList.ButtonA,
                Location = new Point(Libraries.UI.GetTrueSize(SkillPage.Index).Width - Libraries.Prguse.GetTrueSize(396).Width - 3, Libraries.UI.GetTrueSize(SkillPage.Index).Height - Libraries.Prguse.GetTrueSize(396).Height),
                Hint = "Next Page"
            };
            NextButton.Click += (o, e) =>
            {
                if (StartIndex + 8 >= MapObject.User.Magics.Count) return;

                StartIndex += 8;
                RefreshInterface();

                ClearCoolDowns();
            };

            BackButton = new MirButton
            {
                Index = 398,
                Library = Libraries.Prguse,
                Parent = SkillPage,
                PressedIndex = 399,
                Sound = SoundList.ButtonA,
                Location = new Point(3, Libraries.UI.GetTrueSize(SkillPage.Index).Height - Libraries.Prguse.GetTrueSize(396).Height),
                Hint = "Previous Page"
            };
            BackButton.Click += (o, e) =>
            {
                if (StartIndex - 8 < 0) return;

                StartIndex -= 8;
                RefreshInterface();

                ClearCoolDowns();
            };

            SkillPageNumber = new MirLabel
            {
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Parent = SkillPage,
                Size = new Size(SkillPage.Size.Width - 100, 15),
                Location = new Point(50, Libraries.UI.GetTrueSize(SkillPage.Index).Height - Libraries.Prguse.GetTrueSize(396).Height - 4)
            };
        }

        public void Show()
        {
            if (Visible) return;
            ClearCoolDowns();
            Visible = true;
        }


        public void Hide()
        {
            if (!Visible) return;
            Visible = false;
        }

        public void ShowCharacterPage()
        {
            CharacterPage.Visible = true;
            StatusPage.Visible = false;
            SkillPage.Visible = false;
            HermitPage.Visible = false;
            RunePage.Visible = false;
            CharacterButton.Index = 270;
            StatusButton.Index = 277;
            SkillButton.Index = 271;
            ElementalButton.Index = 275;
            RuneButton.Index = 273;
        }
        private void ShowStatusPage()
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = true;
            SkillPage.Visible = false;
            HermitPage.Visible = false;
            RunePage.Visible = false;
            CharacterButton.Index = 269;
            StatusButton.Index = 278;
            SkillButton.Index = 271;
            ElementalButton.Index = 275;
            RuneButton.Index = 273;
        }
        public void ShowSkillPage()
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = false;
            //StatePage.Visible = false;
            SkillPage.Visible = true;
            HermitPage.Visible = false;
            RunePage.Visible = false;
            CharacterButton.Index = 269;
            StatusButton.Index = 277;
            //StateButton.Index = -1;
            SkillButton.Index = 272;
            StartIndex = 0;
            ElementalButton.Index = 275;
            RuneButton.Index = 273;

            ClearCoolDowns();
        }
        public void ShowRunePage()
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = false;
            //StatePage.Visible = false;
            SkillPage.Visible = false;
            HermitPage.Visible = false;
            RunePage.Visible = true;
            CharacterButton.Index = 269;
            StatusButton.Index = 277;
            //StateButton.Index = -1;
            SkillButton.Index = 271;
            ElementalButton.Index = 275;
            RuneButton.Index = 274;
        }
        public void ShowHermitPage()
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = false;
            //StatePage.Visible = false;
            SkillPage.Visible = false;
            HermitPage.Visible = true;
            RunePage.Visible = false;
            CharacterButton.Index = 269;
            StatusButton.Index = 277;
            //StateButton.Index = -1;
            SkillButton.Index = 271;
            ElementalButton.Index = 276;
            RuneButton.Index = 273;
        }

        private void ClearCoolDowns()
        {
            for (int i = 0; i < Magics.Length; i++)
            {
                Magics[i].CoolDown.Dispose();
            }
        }
        private void RefreshInterface()
        {
            DisplayLocationWithOffset = new Point(DisplayLocation.X + 28, DisplayLocation.Y - 20);
            // int offSet = MapObject.User.Gender == MirGender.Male ? 0 : 1;
            int offSet = MapObject.User.Gender == MirGender.Male ? 0 : 1;

            // Index = 504;// +offSet;
            Index = 266;
            CharacterPage.Index = 425 + offSet;


            NameLabel.Text = MapObject.User.Name;
            GuildLabel.Text = MapObject.User.GuildName + "\r\n" + MapObject.User.GuildRankName;

            for (int i = 0; i < Magics.Length; i++)
            {
                if (i + StartIndex >= MapObject.User.Magics.Count)
                {
                    Magics[i].Visible = false;
                    continue;
                }

                Magics[i].Visible = true;
                Magics[i].Update(MapObject.User.Magics[i + StartIndex]);
                Magics[i].Hint = "";

                string item;
                if (Descriptors.SkillDescriptions.TryGetValue(Magics[i].NameLabel.Text, out item))
                    Magics[i].Hint = string.Format("{0}\r\nLevel: {1}     Experience: {2}\r\n\r\n{3}",
                        Magics[i].NameLabel.Text,
                        Magics[i].LevelLabel.Text,
                        Magics[i].ExpLabel.Text,
                        Descriptors.SkillDescriptions[Magics[i].NameLabel.Text]);
            }
            SkillPageNumber.Text = string.Format("Page: {0} / {1}", (StartIndex / 8) + 1, Math.Ceiling((double)MapObject.User.Magics.Count / 8));
        }
        void PreDrawSkills()
        {
            //  Magics
        }

        public MirItemCell GetCell(ulong id)
        {

            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].Item == null || Grid[i].Item.UniqueID != id) continue;
                return Grid[i];
            }
            return null;
        }
    }
}
