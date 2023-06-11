using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using Microsoft.DirectX.Direct3D;
using Font = System.Drawing.Font;
using C = ClientPackets;
using Effect = Client.MirObjects.Effect;

using Client.MirScenes.Dialogs;
using System.Drawing.Imaging;

namespace Client.MirScenes
{
    public sealed partial class GameScene : MirScene
    {
        public static GameScene Scene;

        public static UserObject User
        {
            get { return MapObject.User; }
            set { MapObject.User = value; }
        }

        public static long MoveTime, AttackTime, NextRunTime, LogTime, LastRunTime;
        public static bool CanMove, CanRun;


        public MapControl MapControl;
        public WeatherEffect WeatherEffect;

        public MainDialog MainDialog;
        public UserPlate UserPlate;
        public GroupPlates GroupPlates;
        public TargetPlate TargetPlate;
        public BottomRightIcons BottomLeftIcons;
        //public ChatDialog ChatDialog;
        public ChatControlBar ChatControl;
        public InventoryDialog InventoryDialog;
        public CharacterDialog CharacterDialog;
        public CraftDialog CraftDialog;
        public StorageDialog StorageDialog;
        public BeltDialog BeltDialog;
        //public SkillBeltDialog SkillBeltDialog;
        public MiniMapDialog MiniMapDialog;
        public InspectDialog InspectDialog;
        public OptionDialog OptionDialog;
        public MenuDialog MenuDialog;
        public NPCDialog NPCDialog;
        public NPCGoodsDialog NPCGoodsDialog;
        public NPCDropDialog NPCDropDialog;
        public NPCAwakeDialog NPCAwakeDialog;
        public HelpDialog HelpDialog;
        public MountDialog MountDialog;
        public FishingDialog FishingDialog;
        public FishingStatusDialog FishingStatusDialog;
        public RefineDialog RefineDialog;

        public GroupDialog GroupDialog;
        public GuildDialog GuildDialog;

        public BigMapDialog BigMapDialog;
        public TrustMerchantDialog TrustMerchantDialog;
        public CharacterDuraPanel CharacterDuraPanel;
        public DuraStatusDialog DuraStatusPanel;
        public TradeDialog TradeDialog;
        public GuestTradeDialog GuestTradeDialog;

        //public SkillBarDialog SkillBarDialog;
        //public List<SkillBarDialog> SkillBarDialogs = new List<SkillBarDialog>();
        public ChatOptionDialog ChatOptionDialog;
        public ChatNoticeDialog ChatNoticeDialog;

        public QuestListDialog QuestListDialog;
        public QuestDiaryDialog QuestLogDialog;
        public QuestTrackingDialog QuestTrackingDialog;
        public QuestDetailDialog QuestDetailDialog;

        public RankingDialog RankingDialog;
        public OnlinePlayersDialog OnlinePlayersDialog; // Paul
        public FieldMapDialog FieldMapDialog; // Paul

        public MailListDialog MailListDialog;
        public MailComposeLetterDialog MailComposeLetterDialog;
        public MailComposeParcelDialog MailComposeParcelDialog;
        public MailReadLetterDialog MailReadLetterDialog;
        public MailReadParcelDialog MailReadParcelDialog;

        public IntelligentCreatureDialog IntelligentCreatureDialog;
        public IntelligentCreatureOptionsDialog IntelligentCreatureOptionsDialog;
        public IntelligentCreatureOptionsGradeDialog IntelligentCreatureOptionsGradeDialog;

        public FriendDialog FriendDialog;
        public MemoDialog MemoDialog;
        public RelationshipDialog RelationshipDialog;
        public MentorDialog MentorDialog;
        public GameShopDialog GameShopDialog;

        public ReportDialog ReportDialog;

        public ItemRentingDialog ItemRentingDialog;
        public ItemRentDialog ItemRentDialog;
        public GuestItemRentingDialog GuestItemRentingDialog;
        public GuestItemRentDialog GuestItemRentDialog;
        public ItemRentalDialog ItemRentalDialog;

        public BuffDialog BuffsDialog;

        //not added yet
        public KeyboardLayoutDialog KeyboardLayoutDialog;

        public static List<ItemInfo> ItemInfoList = new List<ItemInfo>();
        public static List<UserId> UserIdList = new List<UserId>();
        public static List<ChatItem> ChatItemList = new List<ChatItem>();
        public static List<ClientQuestInfo> QuestInfoList = new List<ClientQuestInfo>();
        public static List<GameShopItem> GameShopInfoList = new List<GameShopItem>();
        public static List<ClientRecipeInfo> RecipeInfoList = new List<ClientRecipeInfo>();

        public List<Buff> Buffs = new List<Buff>();

        public static UserItem[] Storage = new UserItem[80];
        public static UserItem[] GuildStorage = new UserItem[112];
        public static UserItem[] Refine = new UserItem[16];
        public static UserItem HoverItem;
        public static MirItemCell SelectedCell;

        public static bool PickedUpGold;
        public MirControl ItemLabel, MailLabel, MemoLabel, GuildBuffLabel;
        public static long UseItemTime, PickUpTime, DropViewTime, TargetDeadTime;
        public static uint Gold, Credit;
        public static long InspectTime;
        public bool ShowReviveMessage;


        public bool NewMail;
        public int NewMailCounter = 0;


        public AttackMode AMode;
        public PetMode PMode;
        public LightSetting Lights;

        public static long NPCTime;
        public static uint NPCID;
        public static float NPCRate;
        public static uint DefaultNPCID;


        public long ToggleTime;
        public static bool Slaying, Thrusting, HalfMoon, CrossHalfMoon, DoubleSlash, TwinDrakeBlade, FlamingSword;
        public static long SpellTime;

        public long PingTime;
        public long NextPing = 10000;

        public MirLabel[] OutputLines = new MirLabel[10];
        public List<OutPutMessage> OutputMessages = new List<OutPutMessage>();

        public long OutputDelay;

        public GameScene()
        {
            MapControl.AutoRun = false;
            MapControl.AutoHit = false;
            Slaying = false;
            Thrusting = false;
            HalfMoon = false;
            CrossHalfMoon = false;
            DoubleSlash = false;
            TwinDrakeBlade = false;
            FlamingSword = false;

            Scene = this;
            BackColour = Color.Transparent;
            MoveTime = CMain.Time;

            KeyDown += GameScene_KeyDown;

            WeatherEffect = new WeatherEffect { Parent = this, Visible = true };
            MainDialog = new MainDialog { Parent = this };
            UserPlate = new UserPlate { Parent = this };
            GroupPlates = new GroupPlates { Parent = this, Visible = true };
            TargetPlate = new TargetPlate { Parent = this };
            ChatControl = new ChatControlBar { Parent = this, };
            BottomLeftIcons = new BottomRightIcons { Parent = this };
            BottomLeftIcons.Show();
            //ChatDialog = new ChatDialog { Parent = this,  };
            InventoryDialog = new InventoryDialog { Parent = this };
            CharacterDialog = new CharacterDialog { Parent = this, Visible = false };
            BeltDialog = new BeltDialog { Parent = this, Visible = false };
            //SkillBeltDialog = new SkillBeltDialog { Parent = this };
            StorageDialog = new StorageDialog { Parent = this, Visible = false };
            CraftDialog = new CraftDialog { Parent = this, Visible = false };
            MiniMapDialog = new MiniMapDialog { Parent = this };
            InspectDialog = new InspectDialog { Parent = this, Visible = false };
            OptionDialog = new OptionDialog { Parent = this, Visible = false };
            MenuDialog = new MenuDialog { Parent = this, Visible = false };
            NPCDialog = new NPCDialog { Parent = this, Visible = false };
            NPCGoodsDialog = new NPCGoodsDialog { Parent = this, Visible = false };
            NPCDropDialog = new NPCDropDialog { Parent = this, Visible = false };
            NPCAwakeDialog = new NPCAwakeDialog { Parent = this, Visible = false };

            HelpDialog = new HelpDialog { Parent = this, Visible = false };

            MountDialog = new MountDialog { Parent = this, Visible = false };
            FishingDialog = new FishingDialog { Parent = this, Visible = false };
            FishingStatusDialog = new FishingStatusDialog { Parent = this, Visible = false };

            GroupDialog = new GroupDialog { Parent = this, Visible = false };
            GuildDialog = new GuildDialog { Parent = this, Visible = false };

            BigMapDialog = new BigMapDialog { Parent = this, Visible = false };
            TrustMerchantDialog = new TrustMerchantDialog { Parent = this, Visible = false };
            CharacterDuraPanel = new CharacterDuraPanel { Parent = this, Visible = false };
            DuraStatusPanel = new DuraStatusDialog { Parent = this, Visible = true };
            TradeDialog = new TradeDialog { Parent = this, Visible = false };
            GuestTradeDialog = new GuestTradeDialog { Parent = this, Visible = false };

            //SkillBarDialog = new SkillBarDialog { Parent = this, Visible = false };
            //SkillBarDialog Bar1 = new SkillBarDialog { Parent = this, Visible = false, BarIndex = 0 };
            // SkillBarDialogs.Add(Bar1);
            // SkillBarDialog Bar2 = new SkillBarDialog { Parent = this, Visible = false, BarIndex = 1 };
            // SkillBarDialogs.Add(Bar2);
            ChatOptionDialog = new ChatOptionDialog { Parent = this, Visible = false };
            ChatNoticeDialog = new ChatNoticeDialog { Parent = this, Visible = false };

            QuestListDialog = new QuestListDialog { Parent = this, Visible = false };
            QuestLogDialog = new QuestDiaryDialog { Parent = this, Visible = false };
            QuestDetailDialog = new QuestDetailDialog { Parent = QuestLogDialog, Visible = false };
            QuestTrackingDialog = new QuestTrackingDialog { Parent = this, Visible = false };

            RankingDialog = new RankingDialog { Parent = this, Visible = false };
            OnlinePlayersDialog = new OnlinePlayersDialog { Parent = this, Visible = false };

            FieldMapDialog = new FieldMapDialog { Parent = this, Visible = false };

            MailListDialog = new MailListDialog { Parent = this, Visible = false };
            MailComposeLetterDialog = new MailComposeLetterDialog { Parent = this, Visible = false };
            MailComposeParcelDialog = new MailComposeParcelDialog { Parent = this, Visible = false };
            MailReadLetterDialog = new MailReadLetterDialog { Parent = this, Visible = false };
            MailReadParcelDialog = new MailReadParcelDialog { Parent = this, Visible = false };

            IntelligentCreatureDialog = new IntelligentCreatureDialog { Parent = this, Visible = false };
            IntelligentCreatureOptionsDialog = new IntelligentCreatureOptionsDialog { Parent = this, Visible = false };
            IntelligentCreatureOptionsGradeDialog = new IntelligentCreatureOptionsGradeDialog { Parent = this, Visible = false };

            RefineDialog = new RefineDialog { Parent = this, Visible = false };
            RelationshipDialog = new RelationshipDialog { Parent = this, Visible = false };
            FriendDialog = new FriendDialog { Parent = this, Visible = false };
            MemoDialog = new MemoDialog { Parent = this, Visible = false };
            MentorDialog = new MentorDialog { Parent = this, Visible = false };
            GameShopDialog = new GameShopDialog { Parent = this, Visible = false };

            ReportDialog = new ReportDialog { Parent = this, Visible = false };

            ItemRentingDialog = new ItemRentingDialog { Parent = this, Visible = false };
            ItemRentDialog = new ItemRentDialog { Parent = this, Visible = false };
            GuestItemRentingDialog = new GuestItemRentingDialog { Parent = this, Visible = false };
            GuestItemRentDialog = new GuestItemRentDialog { Parent = this, Visible = false };
            ItemRentalDialog = new ItemRentalDialog { Parent = this, Visible = false };

            BuffsDialog = new BuffDialog { Parent = this, Visible = true };

            //not added yet
            KeyboardLayoutDialog = new KeyboardLayoutDialog { Parent = this, Visible = false };

            for (int i = 0; i < OutputLines.Length; i++)
                OutputLines[i] = new MirLabel
                {
                    AutoSize = true,
                    BackColour = Color.Transparent,
                    Font = new Font(Settings.FontName, 7F),
                    ForeColour = Color.LimeGreen,
                    Location = new Point(20, 25 + i * 13),
                    OutLine = true,
                };
        }

        public void OutputMessage(string message, OutputMessageType type = OutputMessageType.Normal)
        {
            OutputMessages.Add(new OutPutMessage { Message = message, ExpireTime = CMain.Time + 8000, Type = type });
            if (OutputMessages.Count > 10)
                OutputMessages.RemoveAt(0);
        }


        private void ProcessOuput()
        {
            for (int i = 0; i < OutputMessages.Count; i++)
            {
                if (CMain.Time >= OutputMessages[i].ExpireTime)
                    OutputMessages.RemoveAt(i);
            }

            for (int i = 0; i < OutputLines.Length; i++)
            {
                if (OutputMessages.Count > i)
                {
                    Color color;

                    switch (OutputMessages[i].Type)
                    {

                        case OutputMessageType.MagicOn:
                            color = Color.Aqua;
                            break;
                        case OutputMessageType.MagicOff:
                            color = Color.Red;
                            break;
                        case OutputMessageType.Quest:
                            color = Color.Gold;
                            break;
                        case OutputMessageType.Guild:
                            color = Color.DeepPink;
                            break;
                        default:
                            color = Color.LimeGreen;
                            break;
                    }

                    OutputLines[i].Text = OutputMessages[i].Message;
                    OutputLines[i].ForeColour = color;
                    OutputLines[i].Visible = true;
                    OutputLines[i].OutLine = true;
                }
                else
                {
                    OutputLines[i].Text = string.Empty;
                    OutputLines[i].Visible = false;
                }
            }
        }
        private void GameScene_KeyDown(object sender, KeyEventArgs e)
        {
            //bool skillMode = Settings.SkillMode ? CMain.Tilde : CMain.Ctrl;
            //bool altBind = skillMode ? Settings.SkillSet : !Settings.SkillSet;

            foreach (KeyBind KeyCheck in CMain.InputKeys.Keylist)
            {
                if (KeyCheck.Key == Keys.None)
                    continue;
                if (KeyCheck.Key != e.KeyCode)
                    continue;
                if ((KeyCheck.RequireAlt != 2) && (KeyCheck.RequireAlt != (CMain.Alt ? 1 : 0)))
                    continue;
                if ((KeyCheck.RequireShift != 2) && (KeyCheck.RequireShift != (CMain.Shift ? 1 : 0)))
                    continue;
                if ((KeyCheck.RequireCtrl != 2) && (KeyCheck.RequireCtrl != (CMain.Ctrl ? 1 : 0)))
                    continue;
                if ((KeyCheck.RequireTilde != 2) && (KeyCheck.RequireTilde != (CMain.Tilde ? 1 : 0)))
                    continue;
                //now run the real code
                switch (KeyCheck.function)
                {
                    case KeybindOptions.Bar1Skill1: UseSpell(1); break;
                    case KeybindOptions.Bar1Skill2: UseSpell(2); break;
                    case KeybindOptions.Bar1Skill3: UseSpell(3); break;
                    case KeybindOptions.Bar1Skill4: UseSpell(4); break;
                    case KeybindOptions.Bar1Skill5: UseSpell(5); break;
                    case KeybindOptions.Bar1Skill6: UseSpell(6); break;
                    case KeybindOptions.Bar1Skill7: UseSpell(7); break;
                    case KeybindOptions.Bar1Skill8: UseSpell(8); break;
                    case KeybindOptions.Bar2Skill1: UseSpell(9); break;
                    case KeybindOptions.Bar2Skill2: UseSpell(10); break;
                    case KeybindOptions.Bar2Skill3: UseSpell(11); break;
                    case KeybindOptions.Bar2Skill4: UseSpell(12); break;
                    case KeybindOptions.Bar2Skill5: UseSpell(13); break;
                    case KeybindOptions.Bar2Skill6: UseSpell(14); break;
                    case KeybindOptions.Bar2Skill7: UseSpell(15); break;
                    case KeybindOptions.Bar2Skill8: UseSpell(16); break;
                    case KeybindOptions.Inventory:
                    case KeybindOptions.Inventory2:
                        if (!InventoryDialog.Visible) InventoryDialog.Show();
                        else InventoryDialog.Hide();
                        break;
                    case KeybindOptions.Equipment:
                    case KeybindOptions.Equipment2:
                        if (!CharacterDialog.Visible || !CharacterDialog.CharacterPage.Visible)
                        {
                            CharacterDialog.Show();
                            CharacterDialog.ShowCharacterPage();
                        }
                        else CharacterDialog.Hide();
                        break;
                    case KeybindOptions.Skills:
                    case KeybindOptions.Skills2:
                        if (!CharacterDialog.Visible || !CharacterDialog.SkillPage.Visible)
                        {
                            CharacterDialog.Show();
                            CharacterDialog.ShowSkillPage();
                        }
                        else CharacterDialog.Hide();
                        break;
                    case KeybindOptions.Creature:
                        if (!IntelligentCreatureDialog.Visible) IntelligentCreatureDialog.Show();
                        else IntelligentCreatureDialog.Hide();
                        break;
                    case KeybindOptions.MountWindow:
                        if (!MountDialog.Visible) MountDialog.Show();
                        else MountDialog.Hide();
                        break;

                    case KeybindOptions.GameShop:
                        if (!GameShopDialog.Visible) GameShopDialog.Show();
                        else GameShopDialog.Hide();
                        break;
                    case KeybindOptions.Fishing:

                        if (!FishingDialog.Visible) FishingDialog.Show();
                        else FishingDialog.Hide();

                        //Network.Enqueue(new C.Chat { Message = "@STORAGE" }); // paul

                        //if (!OnlinePlayersDialog.Visible) OnlinePlayersDialog.Show();
                        //else OnlinePlayersDialog.Hide();
                        break;
                    case KeybindOptions.Skillbar:
                        //if (!Settings.SkillBar)
                        //MainDialog.SkillBar2.Show();
                        //else
                        //    foreach (SkillBarDialog Bar in SkillBarDialogs)
                        //        Bar.Hide();
                        break;
                    case KeybindOptions.Mount:
                        if (GameScene.Scene.MountDialog.CanRide())
                            GameScene.Scene.MountDialog.Ride();
                        break;
                    case KeybindOptions.Mentor:
                        if (!MentorDialog.Visible) MentorDialog.Show();
                        else MentorDialog.Hide();
                        break;
                    case KeybindOptions.Relationship:
                        if (!RelationshipDialog.Visible) RelationshipDialog.Show();
                        else RelationshipDialog.Hide();
                        break;
                    case KeybindOptions.Friends:
                        if (!FriendDialog.Visible) FriendDialog.Show();
                        else FriendDialog.Hide();
                        break;
                    case KeybindOptions.Guilds:
                        if (!GuildDialog.Visible) GuildDialog.Show();
                        else
                        {
                            GuildDialog.Hide();
                        }
                        break;

                    case KeybindOptions.Ranking:
                        if (!RankingDialog.Visible) RankingDialog.Show();
                        else RankingDialog.Hide();
                        break;
                    case KeybindOptions.Quests:
                        if (!QuestLogDialog.Visible) QuestLogDialog.Show();
                        else QuestLogDialog.Hide();
                        break;
                    case KeybindOptions.Exit:
                        QuitGame();
                        return;

                    case KeybindOptions.Closeall:
                        InventoryDialog.Hide();
                        CharacterDialog.Hide();
                        OptionDialog.Hide();
                        MenuDialog.Hide();
                        if (NPCDialog.Visible) NPCDialog.Hide();
                        HelpDialog.Hide();
                        KeyboardLayoutDialog.Hide();
                        RankingDialog.Hide();
                        IntelligentCreatureDialog.Hide();
                        IntelligentCreatureOptionsDialog.Hide();
                        IntelligentCreatureOptionsGradeDialog.Hide();
                        MountDialog.Hide();
                        FishingDialog.Hide();
                        FriendDialog.Hide();
                        RelationshipDialog.Hide();
                        MentorDialog.Hide();
                        GameShopDialog.Hide();
                        GroupDialog.Hide();
                        GuildDialog.Hide();
                        InspectDialog.Hide();
                        StorageDialog.Hide();
                        TrustMerchantDialog.Hide();
                        //CharacterDuraPanel.Hide();
                        QuestListDialog.Hide();
                        QuestDetailDialog.Hide();
                        QuestLogDialog.Hide();
                        NPCAwakeDialog.Hide();
                        RefineDialog.Hide();
                        BigMapDialog.Visible = false;
                        FieldMapDialog.Visible = false;
                        if (FishingStatusDialog.bEscExit) FishingStatusDialog.Cancel();
                        MailComposeLetterDialog.Hide();
                        MailComposeParcelDialog.Hide();
                        MailListDialog.Hide();
                        MailReadLetterDialog.Hide();
                        MailReadParcelDialog.Hide();
                        ItemRentalDialog.Visible = false;



                        GameScene.Scene.DisposeItemLabel();
                        break;
                    case KeybindOptions.Options:
                    case KeybindOptions.Options2:
                        if (!OptionDialog.Visible) OptionDialog.Show();
                        else OptionDialog.Hide();
                        break;
                    case KeybindOptions.Group:
                        if (!GroupDialog.Visible) GroupDialog.Show();
                        else GroupDialog.Hide();
                        break;
                    case KeybindOptions.BigMap:
                        BigMapDialog.Toggle();
                        break;
                    case KeybindOptions.FieldMapDialog:
                        if (!FieldMapDialog.Visible) FieldMapDialog.Show();
                        else FieldMapDialog.Hide();
                        break;
                    case KeybindOptions.BeltFlip:
                        BeltDialog.Flip();
                        break;
                    case KeybindOptions.Pickup:
                        if (CMain.Time > PickUpTime)
                        {
                            PickUpTime = CMain.Time + 200;
                            Network.Enqueue(new C.PickUp());
                        }
                        break;
                    case KeybindOptions.Belt1:
                    case KeybindOptions.Belt1Alt:
                        MainDialog.ItemBelt[0].UseItem();
                        break;
                    case KeybindOptions.Belt2:
                    case KeybindOptions.Belt2Alt:
                        MainDialog.ItemBelt[1].UseItem();
                        break;
                    case KeybindOptions.Belt3:
                    case KeybindOptions.Belt3Alt:
                        MainDialog.ItemBelt[2].UseItem();
                        break;
                    case KeybindOptions.Belt4:
                    case KeybindOptions.Belt4Alt:
                        MainDialog.ItemBelt[3].UseItem();
                        break;
                    case KeybindOptions.Belt5:
                    case KeybindOptions.Belt5Alt:
                        MainDialog.ItemBelt[4].UseItem();
                        break;
                    case KeybindOptions.Belt6:
                    case KeybindOptions.Belt6Alt:
                        MainDialog.ItemBelt[5].UseItem();
                        break;
                    case KeybindOptions.Logout:
                        LogOut();
                        break;
                    case KeybindOptions.Minimap:
                        MiniMapDialog.Toggle();
                        break;
                    case KeybindOptions.Trade:
                        Network.Enqueue(new C.TradeRequest());
                        break;
                    case KeybindOptions.Rental:
                        ItemRentalDialog.Toggle();
                        break;
                    case KeybindOptions.ChangePetmode:
                        switch (PMode)
                        {
                            case PetMode.Both:
                                Network.Enqueue(new C.ChangePMode { Mode = PetMode.MoveOnly });
                                return;
                            case PetMode.MoveOnly:
                                Network.Enqueue(new C.ChangePMode { Mode = PetMode.AttackOnly });
                                return;
                            case PetMode.AttackOnly:
                                Network.Enqueue(new C.ChangePMode { Mode = PetMode.None });
                                return;
                            case PetMode.None:
                                Network.Enqueue(new C.ChangePMode { Mode = PetMode.Both });
                                return;
                        }
                        break;
                    case KeybindOptions.PetmodeBoth:
                        Network.Enqueue(new C.ChangePMode { Mode = PetMode.Both });
                        return;
                    case KeybindOptions.PetmodeMoveonly:
                        Network.Enqueue(new C.ChangePMode { Mode = PetMode.MoveOnly });
                        return;
                    case KeybindOptions.PetmodeAttackonly:
                        Network.Enqueue(new C.ChangePMode { Mode = PetMode.AttackOnly });
                        return;
                    case KeybindOptions.PetmodeNone:
                        Network.Enqueue(new C.ChangePMode { Mode = PetMode.None });
                        return;
                    case KeybindOptions.CreatureAutoPickup://semiauto!
                        Network.Enqueue(new C.IntelligentCreaturePickup { MouseMode = false, Location = MapControl.MapLocation });
                        break;
                    case KeybindOptions.CreaturePickup:
                        Network.Enqueue(new C.IntelligentCreaturePickup { MouseMode = true, Location = MapControl.MapLocation });
                        break;
                    case KeybindOptions.ChangeAttackmode:
                        switch (AMode)
                        {
                            case AttackMode.Peace:
                                Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.Group });
                                return;
                            case AttackMode.Group:
                                Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.Guild });
                                return;
                            case AttackMode.Guild:
                                Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.EnemyGuild });
                                return;
                            case AttackMode.EnemyGuild:
                                Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.RedBrown });
                                return;
                            case AttackMode.RedBrown:
                                Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.All });
                                return;
                            case AttackMode.All:
                                Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.Peace });
                                return;
                        }
                        break;
                    case KeybindOptions.AttackmodePeace:
                        Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.Peace });
                        return;
                    case KeybindOptions.AttackmodeGroup:
                        Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.Group });
                        return;
                    case KeybindOptions.AttackmodeGuild:
                        Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.Guild });
                        return;
                    case KeybindOptions.AttackmodeEnemyguild:
                        Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.EnemyGuild });
                        return;
                    case KeybindOptions.AttackmodeRedbrown:
                        Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.RedBrown });
                        return;
                    case KeybindOptions.AttackmodeAll:
                        Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.All });
                        return;

                    case KeybindOptions.Help:
                        if (!HelpDialog.Visible) HelpDialog.Show();
                        else HelpDialog.Hide();
                        break;
                    case KeybindOptions.Autorun:
                        MapControl.AutoRun = !MapControl.AutoRun;
                        break;
                    case KeybindOptions.Cameramode:

                        if (!MainDialog.Visible)
                        {
                            MainDialog.Show();
                            UserPlate.Show();
                            TargetPlate.Show();
                            BottomLeftIcons.Show();
                            //ChatDialog.Show();
                            //BeltDialog.Show();
                            ChatControl.Show();
                            MiniMapDialog.Show();
                            CharacterDuraPanel.Show();
                            DuraStatusPanel.Show();
                            BuffsDialog.Show();
                        }
                        else
                        {
                            MainDialog.Hide();
                            UserPlate.Hide();
                            TargetPlate.Hide();
                            BottomLeftIcons.Hide();
                            //ChatDialog.Hide();
                            BeltDialog.Hide();
                            ChatControl.Hide();
                            MiniMapDialog.Hide();
                            CharacterDuraPanel.Hide();
                            DuraStatusPanel.Hide();
                            BuffsDialog.Hide();
                        }
                        break;
                    case KeybindOptions.DropView:
                        if (CMain.Time > DropViewTime)
                            DropViewTime = CMain.Time + 5000;
                        break;
                    case KeybindOptions.TargetDead:
                        if (CMain.Time > TargetDeadTime)
                            TargetDeadTime = CMain.Time + 5000;
                        break;
                    case KeybindOptions.AddGroupMember:
                        if (MapObject.MouseObject == null) break;
                        if (MapObject.MouseObject.Race != ObjectType.Player) break;

                        GameScene.Scene.GroupDialog.AddMember(MapObject.MouseObject.Name);
                        break;
                }
            }
        }

        public void UseSpell(int key)
        {
            if (User.RidingMount || User.Fishing) return;

            if (!User.HasClassWeapon && User.Weapon >= 0)
            {
                //ChatControl.chatDialog.ReceiveChat("You must be wearing a suitable weapon to perform this skill", ChatType.System);
                ChatControl.chatDialog.ReceiveChat("You must be wearing a suitable weapon to perform this skill", ChatType.System);
                return;
            }

            if (CMain.Time < User.BlizzardStopTime || CMain.Time < User.ReincarnationStopTime) return;

            ClientMagic magic = null;

            for (int i = 0; i < User.Magics.Count; i++)
            {
                if (User.Magics[i].Key != key) continue;
                magic = User.Magics[i];
                break;
            }

            if (magic == null) return;

            switch (magic.Spell)
            {
                case Spell.CounterAttack:
                    if ((CMain.Time < magic.CastTime + magic.Delay) && magic.CastTime != 0)
                    {
                        if (CMain.Time >= OutputDelay)
                        {
                            OutputDelay = CMain.Time + 1000;
                            GameScene.Scene.OutputMessage(string.Format("You cannot cast [{0}] for another [{1}] seconds.", magic.Spell.ToString(), ((magic.CastTime + magic.Delay) - CMain.Time - 1) / 1000 + 1));
                        }

                        return;
                    }
                    magic.CastTime = CMain.Time;
                    break;
            }

            int cost;
            switch (magic.Spell)
            {
                case Spell.Fencing:
                case Spell.FatalSword:
                case Spell.MPEater:
                case Spell.Hemorrhage:
                case Spell.SpiritSword:
                case Spell.Slaying:
                case Spell.Focus:
                case Spell.Meditation:
                    return;
                case Spell.Thrusting:
                    if (CMain.Time < ToggleTime) return;
                    Thrusting = !Thrusting;
                    ChatControl.chatDialog.ReceiveChat(Thrusting ? "Use Thrusting." : "Do not use Thrusting.", ChatType.Hint);
                    ToggleTime = CMain.Time + 1000;
                    Network.Enqueue(new C.SpellToggle { Spell = magic.Spell, CanUse = Thrusting });
                    break;
                case Spell.HalfMoon:
                    if (CMain.Time < ToggleTime) return;
                    HalfMoon = !HalfMoon;
                    ChatControl.chatDialog.ReceiveChat(HalfMoon ? "Use Half Moon." : "Do not use Half Moon.", ChatType.Hint);
                    ToggleTime = CMain.Time + 1000;
                    Network.Enqueue(new C.SpellToggle { Spell = magic.Spell, CanUse = HalfMoon });
                    break;
                case Spell.CrossHalfMoon:
                    if (CMain.Time < ToggleTime) return;
                    CrossHalfMoon = !CrossHalfMoon;
                    ChatControl.chatDialog.ReceiveChat(CrossHalfMoon ? "Use Cross Half Moon." : "Do not use Cross Half Moon.", ChatType.Hint);
                    ToggleTime = CMain.Time + 1000;
                    Network.Enqueue(new C.SpellToggle { Spell = magic.Spell, CanUse = CrossHalfMoon });
                    break;
                case Spell.DoubleSlash:
                    if (CMain.Time < ToggleTime) return;
                    DoubleSlash = !DoubleSlash;
                    ChatControl.chatDialog.ReceiveChat(DoubleSlash ? "Use Double Slash." : "Do not use Double Slash.", ChatType.Hint);
                    ToggleTime = CMain.Time + 1000;
                    Network.Enqueue(new C.SpellToggle { Spell = magic.Spell, CanUse = DoubleSlash });
                    break;
                case Spell.TwinDrakeBlade:
                    if (CMain.Time < ToggleTime) return;
                    ToggleTime = CMain.Time + 500;

                    cost = magic.Level * magic.LevelCost + magic.BaseCost;
                    if (cost > MapObject.User.MP)
                    {
                        Scene.OutputMessage("[Not enough Mana, Your low on MP]", OutputMessageType.MagicOff);
                        return;
                    }
                    TwinDrakeBlade = true;
                    Network.Enqueue(new C.SpellToggle { Spell = magic.Spell, CanUse = true });
                    User.Effects.Add(new Effect(Libraries.Magic2, 210, 6, 500, User));
                    break;
                case Spell.FlamingSword:
                    if (CMain.Time < ToggleTime) return;
                    ToggleTime = CMain.Time + 500;

                    cost = magic.Level * magic.LevelCost + magic.BaseCost;
                    if (cost > MapObject.User.MP)
                    {
                        Scene.OutputMessage("[Not enough Mana, Your low on MP]", OutputMessageType.MagicOff);
                        return;
                    }
                    Network.Enqueue(new C.SpellToggle { Spell = magic.Spell, CanUse = true });
                    break;
                case Spell.CounterAttack:
                    cost = magic.Level * magic.LevelCost + magic.BaseCost;
                    if (cost > MapObject.User.MP)
                    {
                        Scene.OutputMessage("[Not enough Mana, Your low on MP]");
                        return;
                    }

                    SoundManager.PlaySound(20000 + (ushort)Spell.CounterAttack * 10);
                    Network.Enqueue(new C.SpellToggle { Spell = magic.Spell, CanUse = true });
                    break;
                case Spell.MentalState:
                    if (CMain.Time < ToggleTime) return;
                    ToggleTime = CMain.Time + 500;
                    Network.Enqueue(new C.SpellToggle { Spell = magic.Spell, CanUse = true });
                    break;
                default:
                    User.NextMagic = magic;
                    User.NextMagicLocation = MapControl.MapLocation;
                    User.NextMagicObject = MapObject.MouseObject;
                    User.NextMagicDirection = MapControl.MouseDirection();
                    break;
            }

        }

        public void QuitGame()
        {
            if (CMain.Time >= LogTime)
            {
                //If Last Combat < 10 CANCEL
                MirMessageBox messageBox = new MirMessageBox("Do you want to Exit the Game?", MirMessageBoxButtons.YesNo);
                messageBox.YesButton.Click += (o, e) => Program.Form.Close();
                messageBox.Show();
            }
            else
            {
                ChatControl.chatDialog.ReceiveChat("Cannot leave game for " + (LogTime - CMain.Time) / 1000 + " seconds.", ChatType.System);
            }
        }
        public void LogOut()
        {
            if (CMain.Time >= LogTime)
            {
                //If Last Combat < 10 CANCEL
                MirMessageBox messageBox = new MirMessageBox("Do you want to Log out to Character Selection?", MirMessageBoxButtons.YesNo);
                messageBox.YesButton.Click += (o, e) =>
                {
                    Network.Enqueue(new C.LogOut());
                    Enabled = false;
                };
                messageBox.Show();
            }
            else
            {
                ChatControl.chatDialog.ReceiveChat("Cannot leave game for " + (LogTime - CMain.Time) / 1000 + " seconds.", ChatType.System);
            }
        }

        protected internal override void DrawControl()
        {
            if (MapControl != null && !MapControl.IsDisposed)
                MapControl.DrawControl();

            base.DrawControl();


            if (PickedUpGold || (SelectedCell != null && SelectedCell.Item != null))
            {
                int image = PickedUpGold ? 116 : SelectedCell.Item.Image;
                Size imgSize = Libraries.Items.GetTrueSize(image);
                Point p = CMain.MPoint.Add(-imgSize.Width / 2, -imgSize.Height / 2);

                if (p.X + imgSize.Width >= Settings.ScreenWidth)
                    p.X = Settings.ScreenWidth - imgSize.Width;

                if (p.Y + imgSize.Height >= Settings.ScreenHeight)
                    p.Y = Settings.ScreenHeight - imgSize.Height;

                Libraries.Items.Draw(image, p.X, p.Y);
            }

            for (int i = 0; i < OutputLines.Length; i++)
                OutputLines[i].Draw();
        }
        public override void Process()
        {
            if (MapControl == null || User == null)
                return;

            if (CMain.Time >= MoveTime)
            {
                MoveTime += 100; //Move Speed
                CanMove = true;
                MapControl.AnimationCount++;
                MapControl.TextureValid = false;
            }
            else
                CanMove = false;


            if (CMain.Time >= NextPing)
            {
                NextPing = CMain.Time + 60000;
                Network.Enqueue(new C.KeepAlive() { Time = CMain.Time });
            }

            MirItemCell cell = MouseControl as MirItemCell;

            if (cell != null && HoverItem != cell.Item && HoverItem != cell.ShadowItem && SelectedCell != null)
            {
                DisposeItemLabel();
                HoverItem = null;
                CreateItemLabel(cell.Item, false, false, SelectedCell.Item.Count);
            }

            if (ItemLabel != null && !ItemLabel.IsDisposed)
            {
                ItemLabel.BringToFront();

                int x = CMain.MPoint.X + 15, y = CMain.MPoint.Y;
                if (x + ItemLabel.Size.Width > Settings.ScreenWidth)
                    x = Settings.ScreenWidth - ItemLabel.Size.Width;

                if (y + ItemLabel.Size.Height > Settings.ScreenHeight)
                    y = Settings.ScreenHeight - ItemLabel.Size.Height;
                ItemLabel.Location = new Point(x, y);
            }

            if (MailLabel != null && !MailLabel.IsDisposed)
            {
                MailLabel.BringToFront();

                int x = CMain.MPoint.X + 15, y = CMain.MPoint.Y;
                if (x + MailLabel.Size.Width > Settings.ScreenWidth)
                    x = Settings.ScreenWidth - MailLabel.Size.Width;

                if (y + MailLabel.Size.Height > Settings.ScreenHeight)
                    y = Settings.ScreenHeight - MailLabel.Size.Height;
                MailLabel.Location = new Point(x, y);
            }

            if (MemoLabel != null && !MemoLabel.IsDisposed)
            {
                MemoLabel.BringToFront();

                int x = CMain.MPoint.X + 15, y = CMain.MPoint.Y;
                if (x + MemoLabel.Size.Width > Settings.ScreenWidth)
                    x = Settings.ScreenWidth - MemoLabel.Size.Width;

                if (y + MemoLabel.Size.Height > Settings.ScreenHeight)
                    y = Settings.ScreenHeight - MemoLabel.Size.Height;
                MemoLabel.Location = new Point(x, y);
            }

            if (GuildBuffLabel != null && !GuildBuffLabel.IsDisposed)
            {
                GuildBuffLabel.BringToFront();

                int x = CMain.MPoint.X + 15, y = CMain.MPoint.Y;
                if (x + GuildBuffLabel.Size.Width > Settings.ScreenWidth)
                    x = Settings.ScreenWidth - GuildBuffLabel.Size.Width;

                if (y + GuildBuffLabel.Size.Height > Settings.ScreenHeight)
                    y = Settings.ScreenHeight - GuildBuffLabel.Size.Height;
                GuildBuffLabel.Location = new Point(x, y);
            }

            if (!User.Dead) ShowReviveMessage = false;

            if (ShowReviveMessage && CMain.Time > User.DeadTime && User.CurrentAction == MirAction.Dead)
            {
                ShowReviveMessage = false;
                MirMessageBox messageBox = new MirMessageBox("You have died, Do you want to revive in town?", MirMessageBoxButtons.YesNo, false);

                messageBox.YesButton.Click += (o, e) =>
                {
                    if (User.Dead) Network.Enqueue(new C.TownRevive());
                };

                messageBox.AfterDraw += (o, e) =>
                {
                    if (!User.Dead) messageBox.Dispose();
                };

                messageBox.Show();
            }

            BuffsDialog.Process();

            MapControl.Process();
            WeatherEffect.Process();
            MainDialog.Process();
            UserPlate.Process();
            GroupPlates.Process();
            TargetPlate.Process();
            BottomLeftIcons.Process();
            InventoryDialog.Process();
            GameShopDialog.Process();
            MiniMapDialog.Process();
            //foreach (SkillBarDialog Bar in Scene.SkillBarDialogs)
            //    Bar.Process();
            MainDialog.SkillBar1.Process();
            MainDialog.SkillBar2.Process();

            DialogProcess();

            ProcessOuput();
        }

        public void DialogProcess()
        {
            //if(Settings.SkillBar)
            //{
            //    foreach (SkillBarDialog Bar in Scene.SkillBarDialogs)
            //        Bar.Show();
            //}
            //else
            //{
            //    foreach (SkillBarDialog Bar in Scene.SkillBarDialogs)
            //        Bar.Hide();
            //}

            //for (int i = 0; i < Scene.SkillBarDialogs.Count; i++)
            //{
            //    if (i * 2 > Settings.SkillbarLocation.Length) break;
            //    if ((Settings.SkillbarLocation[i, 0] > Settings.Resolution - 100) || (Settings.SkillbarLocation[i, 1] > 700)) continue;//in theory you'd want the y coord to be validated based on resolution, but since client only allows for wider screens and not higher :(
            //    Scene.SkillBarDialogs[i].Location = new Point(Settings.SkillbarLocation[i, 0], Settings.SkillbarLocation[i, 1]);
            //}

            if (Settings.DuraView)
                CharacterDuraPanel.Show();
            else
                CharacterDuraPanel.Hide();
        }

 
        public void AddItem(UserItem item)
        {
            Redraw();

            if (item.Info.StackSize > 1) //Stackable
            {
                for (int i = 0; i < User.Inventory.Length; i++)
                {
                    UserItem temp = User.Inventory[i];
                    if (temp == null || item.Info != temp.Info || temp.Count >= temp.Info.StackSize) continue;

                    if (item.Count + temp.Count <= temp.Info.StackSize)
                    {
                        temp.Count += item.Count;
                        return;
                    }
                    item.Count -= temp.Info.StackSize - temp.Count;
                    temp.Count = temp.Info.StackSize;
                }
            }

            if (item.Info.Type == ItemType.Potion || item.Info.Type == ItemType.Scroll || (item.Info.Type == ItemType.Script && item.Info.Effect == 1))
            {
                for (int i = 0; i < User.BeltIdx - 2; i++)
                {
                    if (User.Inventory[i] != null) continue;
                    User.Inventory[i] = item;
                    return;
                }
            }
            else if (item.Info.Type == ItemType.Amulet)
            {
                for (int i = 4; i < User.BeltIdx; i++)
                {
                    if (User.Inventory[i] != null) continue;
                    User.Inventory[i] = item;
                    return;
                }
            }
            else
            {
                for (int i = User.BeltIdx; i < User.Inventory.Length; i++)
                {
                    if (User.Inventory[i] != null) continue;
                    User.Inventory[i] = item;
                    return;
                }
            }

            for (int i = 0; i < User.Inventory.Length; i++)
            {
                if (User.Inventory[i] != null) continue;
                User.Inventory[i] = item;
                return;
            }
        }
        
        public static void Bind(UserItem item)
        {
            for (int i = 0; i < ItemInfoList.Count; i++)
            {
                if (ItemInfoList[i].Index != item.ItemIndex) continue;

                item.Info = ItemInfoList[i];

                item.SetSlotSize();

                for (int s = 0; s < item.Slots.Length; s++)
                {
                    if (item.Slots[s] == null) continue;

                    Bind(item.Slots[s]);
                }

                return;
            }
        }

        public static void BindQuest(ClientQuestProgress quest)
        {
            for (int i = 0; i < QuestInfoList.Count; i++)
            {
                if (QuestInfoList[i].Index != quest.Id) continue;

                quest.QuestInfo = QuestInfoList[i];

                return;
            }
        }

        public static ItemInfo GetInfo(int index)
        {
            for (int i = 0; i < ItemInfoList.Count; i++)
            {
                ItemInfo info = ItemInfoList[i];
                if (info.Index != index) continue;
                return info;
            }

            return null;
        }

        public string GetUserName(uint id)
        {
            for (int i = 0; i < UserIdList.Count; i++)
            {
                UserId who = UserIdList[i];
                if (id == who.Id)
                    return who.UserName;
            }
            Network.Enqueue(new C.RequestUserName { UserID = id });
            UserIdList.Add(new UserId() { Id = id, UserName = "Unknown" });
            return "";
        }

        public class OutPutMessage
        {
            public string Message;
            public long ExpireTime;
            public OutputMessageType Type;
        }

        #region Disposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Scene = null;
                User = null;

                MoveTime = 0;
                AttackTime = 0;
                NextRunTime = 0;
                LastRunTime = 0;
                CanMove = false;
                CanRun = false;

                MapControl = null;
                MainDialog = null;
                //ChatDialog = null;
                ChatControl = null;
                InventoryDialog = null;
                CharacterDialog = null;
                StorageDialog = null;
                BeltDialog = null;
                MiniMapDialog = null;
                InspectDialog = null;
                OptionDialog = null;
                MenuDialog = null;
                NPCDialog = null;
                QuestDetailDialog = null;
                QuestListDialog = null;
                QuestLogDialog = null;
                QuestTrackingDialog = null;
                GameShopDialog = null;
                MentorDialog = null;

                RelationshipDialog = null;
                CharacterDuraPanel = null;
                DuraStatusPanel = null;

                HoverItem = null;
                SelectedCell = null;
                PickedUpGold = false;

                UseItemTime = 0;
                PickUpTime = 0;
                InspectTime = 0;

                DisposeItemLabel();

                AMode = 0;
                PMode = 0;
                Lights = 0;

                NPCTime = 0;
                NPCID = 0;
                DefaultNPCID = 0;

                for (int i = 0; i < OutputLines.Length; i++)
                    if (OutputLines[i] != null && OutputLines[i].IsDisposed)
                        OutputLines[i].Dispose();

                OutputMessages.Clear();
                OutputMessages = null;
            }

            base.Dispose(disposing);
        }

        #endregion

    }

    public sealed class MapControl : MirControl
    {
        private bool _autoPath;
        public bool AutoPath
        {
            get
            {
                return _autoPath;
            }
            set
            {
                if (_autoPath == value) return;
                _autoPath = value;

                if (!_autoPath)
                    CurrentPath = null;

                if (GameScene.Scene != null)
                    GameScene.Scene.ChatControl.chatDialog.ReceiveChat(value ? "[AutoPath: On]" : "[AutoPath: Off]", ChatType.Hint);
            }
        }

        public PathFinder PathFinder = null;
        public List<Node> CurrentPath = null;

        public static UserObject User
        {
            get { return MapObject.User; }
            set { MapObject.User = value; }
        }

        public static List<MapObject> Objects = new List<MapObject>();

        public const int CellWidth = 48;
        public const int CellHeight = 32;

        public static int OffSetX;
        public static int OffSetY;

        public static int ViewRangeX;
        public static int ViewRangeY;



        public static Point MapLocation
        {
            get { return GameScene.User == null ? Point.Empty : new Point(MouseLocation.X / CellWidth - OffSetX, MouseLocation.Y / CellHeight - OffSetY).Add(GameScene.User.CurrentLocation); }
        }

        public static MouseButtons MapButtons;
        public static Point MouseLocation;
        public static long InputDelay;
        public static long NextAction;

        public CellInfo[,] M2CellInfo;
        public List<Door> Doors = new List<Door>();
        public int Width, Height;

        public string FileName = String.Empty;
        public string Title = String.Empty;
        public ushort MiniMap, BigMap, Music, SetMusic;
        public LightSetting Lights;
        public bool Lightning, Fire;
        public byte MapDarkLight;
        public long LightningTime, FireTime;

        public bool FloorValid, LightsValid;

        private Texture _floorTexture, _lightTexture;
        private Surface _floorSurface, _lightSurface;

        public long OutputDelay;

        private static bool _awakeningAction;
        public static bool AwakeningAction
        {
            get { return _awakeningAction; }
            set
            {
                if (_awakeningAction == value) return;
                _awakeningAction = value;
            }
        }

        private static bool _autoRun;
        public static bool AutoRun
        {
            get { return _autoRun; }
            set
            {
                if (_autoRun == value) return;
                _autoRun = value;
                if (GameScene.Scene != null)
                    GameScene.Scene.ChatControl.chatDialog.ReceiveChat(value ? "[AutoRun: On]" : "[AutoRun: Off]", ChatType.Hint);
            }

        }
        public static bool AutoHit;

        public int AnimationCount;

        public static List<Effect> Effects = new List<Effect>();

        public MapControl()
        {
            MapButtons = MouseButtons.None;

            OffSetX = Settings.ScreenWidth / 2 / CellWidth;
            OffSetY = Settings.ScreenHeight / 2 / CellHeight - 1;

            ViewRangeX = OffSetX + 6;
            ViewRangeY = OffSetY + 6;

            Size = new Size(Settings.ScreenWidth, Settings.ScreenHeight);
            DrawControlTexture = true;
            BackColour = Color.Black;

            MouseDown += OnMouseDown;
            MouseMove += (o, e) => MouseLocation = e.Location;
            Click += OnMouseClick;
        }

        public void LoadMap()
        {
            GameScene.Scene.NPCDialog.Hide();
            Objects.Clear();
            Effects.Clear();
            Doors.Clear();

            if (User != null)
                Objects.Add(User);



            MapObject.MouseObject = null;
            MapObject.TargetObject = null;
            MapObject.MagicObject = null;
            MapReader Map = new MapReader(FileName);
            M2CellInfo = Map.MapCells;
            Width = Map.Width;
            Height = Map.Height;

            try
            {
                if (SetMusic != Music)
                {
                    SoundManager.Device.Dispose();
                    SoundManager.Create();
                    SoundManager.PlayMusic(Music, true);
                }
            }
            catch (Exception)
            {
                // Do nothing. index was not valid.
            }

            SetMusic = Music;
            SoundList.Music = Music;
        }

        public void Process()
        {
            Processdoors();
            User.Process();

            for (int i = Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = Objects[i];
                if (ob == User) continue;
                //  if (ob.ActionFeed.Count > 0 || ob.Effects.Count > 0 || GameScene.CanMove || CMain.Time >= ob.NextMotion)
                ob.Process();
            }

            for (int i = Effects.Count - 1; i >= 0; i--)
                Effects[i].Process();

            if (MapObject.TargetObject != null && MapObject.TargetObject is MonsterObject && MapObject.TargetObject.AI == 64)
                MapObject.TargetObject = null;
            if (MapObject.MagicObject != null && MapObject.MagicObject is MonsterObject && MapObject.MagicObject.AI == 64)
                MapObject.MagicObject = null;

            CheckInput();


            MapObject bestmouseobject = null;
            for (int y = MapLocation.Y + 2; y >= MapLocation.Y - 2; y--)
            {
                if (y >= Height) continue;
                if (y < 0) break;
                for (int x = MapLocation.X + 2; x >= MapLocation.X - 2; x--)
                {
                    if (x >= Width) continue;
                    if (x < 0) break;
                    CellInfo cell = M2CellInfo[x, y];
                    if (cell.CellObjects == null) continue;

                    for (int i = cell.CellObjects.Count - 1; i >= 0; i--)
                    {
                        MapObject ob = cell.CellObjects[i];
                        if (ob == MapObject.User || !ob.MouseOver(CMain.MPoint)) continue;

                        if (MapObject.MouseObject != ob)
                        {
                            if (ob.Dead)
                            {
                                if (!Settings.TargetDead && GameScene.TargetDeadTime <= CMain.Time) continue;

                                bestmouseobject = ob;
                                //continue;
                            }
                            MapObject.MouseObject = ob;
                            Redraw();
                        }
                        if (bestmouseobject != null && MapObject.MouseObject == null)
                        {
                            MapObject.MouseObject = bestmouseobject;
                            Redraw();
                        }
                        return;
                    }
                }
            }


            if (MapObject.MouseObject != null)
            {
                MapObject.MouseObject = null;
                Redraw();
            }
        }

        public static MapObject GetObject(uint targetID)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                MapObject ob = Objects[i];
                if (ob.ObjectID != targetID) continue;
                return ob;
            }
            return null;
        }

        public override void Draw()
        {
            //Do nothing.
        }

        protected override void CreateTexture()
        {
            if (User == null) return;

            if (!FloorValid)
                DrawFloor();


            if (ControlTexture != null && !ControlTexture.Disposed && Size != TextureSize)
                ControlTexture.Dispose();

            if (ControlTexture == null || ControlTexture.Disposed)
            {
                DXManager.ControlList.Add(this);
                ControlTexture = new Texture(DXManager.Device, Size.Width, Size.Height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                ControlTexture.Disposing += ControlTexture_Disposing;
                TextureSize = Size;
            }

            Surface oldSurface = DXManager.CurrentSurface;
            Surface surface = ControlTexture.GetSurfaceLevel(0);
            DXManager.SetSurface(surface);
            DXManager.Device.Clear(ClearFlags.Target, Color.Black, 0, 0);

            DrawBackground();

            if (FloorValid)
                DXManager.Sprite.Draw2D(_floorTexture, Point.Empty, 0F, Point.Empty, Color.White);

            DrawObjects();

            //Render Death, 

            LightSetting setting = Lights == LightSetting.Normal ? GameScene.Scene.Lights : Lights;
            if (setting != LightSetting.Day)
                DrawLights(setting);

            if (Settings.DropView || GameScene.DropViewTime > CMain.Time)
            {
                for (int i = 0; i < Objects.Count; i++)
                {
                    ItemObject ob = Objects[i] as ItemObject;
                    if (ob == null) continue;

                    if (!ob.MouseOver(MouseLocation))
                        ob.DrawName();
                }
            }

            if (MapObject.MouseObject != null && !(MapObject.MouseObject is ItemObject))
                MapObject.MouseObject.DrawName();

            int offSet = 0;
            for (int i = 0; i < Objects.Count; i++)
            {
                ItemObject ob = Objects[i] as ItemObject;
                if (ob == null) continue;

                if (!ob.MouseOver(MouseLocation)) continue;
                ob.DrawName(offSet);
                offSet -= ob.NameLabel.Size.Height + (ob.NameLabel.Border ? 1 : 0);
            }

            if (MapObject.User.MouseOver(MouseLocation))
                MapObject.User.DrawName();




            DXManager.SetSurface(oldSurface);
            surface.Dispose();
            TextureValid = true;
        }
        protected internal override void DrawControl()
        {
            if (!DrawControlTexture)
                return;

            if (!TextureValid)
                CreateTexture();

            if (ControlTexture == null || ControlTexture.Disposed)
                return;

            float oldOpacity = DXManager.Opacity;

            DXManager.SetOpacity(Opacity);
            DXManager.Sprite.Draw2D(ControlTexture, Point.Empty, 0F, Point.Empty, Color.White);
            DXManager.SetOpacity(oldOpacity);

            CleanTime = CMain.Time + Settings.CleanDelay;
        }

        private void DrawFloor()
        {
            if (_floorTexture == null || _floorTexture.Disposed)
            {
                _floorTexture = new Texture(DXManager.Device, Settings.ScreenWidth, Settings.ScreenHeight, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                _floorTexture.Disposing += FloorTexture_Disposing;
                _floorSurface = _floorTexture.GetSurfaceLevel(0);
            }


            Surface oldSurface = DXManager.CurrentSurface;

            DXManager.SetSurface(_floorSurface);
            DXManager.Device.Clear(ClearFlags.Target, Color.Black, 0, 0); //Color.Black

            int index;
            int drawY, drawX;

            for (int y = User.Movement.Y - ViewRangeY; y <= User.Movement.Y + ViewRangeY; y++)
            {
                if (y <= 0 || y % 2 == 1) continue;
                if (y >= Height) break;
                drawY = (y - User.Movement.Y + OffSetY) * CellHeight + User.OffSetMove.Y; //Moving OffSet

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x <= 0 || x % 2 == 1) continue;
                    if (x >= Width) break;
                    drawX = (x - User.Movement.X + OffSetX) * CellWidth - OffSetX + User.OffSetMove.X; //Moving OffSet
                    if ((M2CellInfo[x, y].BackImage == 0) || (M2CellInfo[x, y].BackIndex == -1)) continue;
                    index = (M2CellInfo[x, y].BackImage & 0x1FFFF) - 1;
                    Libraries.MapLibs[M2CellInfo[x, y].BackIndex].Draw(index, drawX, drawY);
                }
            }

            for (int y = User.Movement.Y - ViewRangeY; y <= User.Movement.Y + ViewRangeY + 5; y++)
            {
                if (y <= 0) continue;
                if (y >= Height) break;
                drawY = (y - User.Movement.Y + OffSetY) * CellHeight + User.OffSetMove.Y; //Moving OffSet

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    drawX = (x - User.Movement.X + OffSetX) * CellWidth - OffSetX + User.OffSetMove.X; //Moving OffSet

                    index = M2CellInfo[x, y].MiddleImage - 1;

                    if ((index < 0) || (M2CellInfo[x, y].MiddleIndex == -1)) continue;
                    if (M2CellInfo[x, y].MiddleIndex > 199)
                    {//mir3 mid layer is same level as front layer not real middle + it cant draw index -1 so 2 birds in one stone :p
                        Size s = Libraries.MapLibs[M2CellInfo[x, y].MiddleIndex].GetSize(index);

                        if (s.Width != CellWidth || s.Height != CellHeight) continue;
                    }
                    Libraries.MapLibs[M2CellInfo[x, y].MiddleIndex].Draw(index, drawX, drawY);
                }
            }
            for (int y = User.Movement.Y - ViewRangeY; y <= User.Movement.Y + ViewRangeY + 5; y++)
            {
                if (y <= 0) continue;
                if (y >= Height) break;
                drawY = (y - User.Movement.Y + OffSetY) * CellHeight + User.OffSetMove.Y; //Moving OffSet

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    drawX = (x - User.Movement.X + OffSetX) * CellWidth - OffSetX + User.OffSetMove.X; //Moving OffSet

                    index = (M2CellInfo[x, y].FrontImage & 0x7FFF) - 1;
                    if (index == -1) continue;
                    int fileIndex = M2CellInfo[x, y].FrontIndex;
                    if (fileIndex == -1) continue;
                    Size s = Libraries.MapLibs[fileIndex].GetSize(index);
                    if (fileIndex == 200) continue; //fixes random bad spots on old school 4.map
                    if (M2CellInfo[x, y].DoorIndex > 0)
                    {
                        Door DoorInfo = GetDoor(M2CellInfo[x, y].DoorIndex);
                        if (DoorInfo == null)
                        {
                            DoorInfo = new Door() { index = M2CellInfo[x, y].DoorIndex, DoorState = 0, ImageIndex = 0, LastTick = CMain.Time };
                            Doors.Add(DoorInfo);
                        }
                        else
                        {
                            if (DoorInfo.DoorState != 0)
                            {
                                index += (DoorInfo.ImageIndex + 1) * M2CellInfo[x, y].DoorOffset;//'bad' code if you want to use animation but it's gonna depend on the animation > has to be custom designed for the animtion
                            }
                        }
                    }

                    if (index < 0 || ((s.Width != CellWidth || s.Height != CellHeight) && ((s.Width != CellWidth * 2) || (s.Height != CellHeight * 2)))) continue;
                    Libraries.MapLibs[fileIndex].Draw(index, drawX, drawY);
                }
            }

            DXManager.SetSurface(oldSurface);

            FloorValid = true;
        }

        private void DrawBackground()
        {
            string cleanFilename = FileName.Replace(Settings.MapPath, "");

            if (cleanFilename.StartsWith(@"World\VoidIslands"))
            {
                Libraries.Background.Draw(10, 0, 0); //mountains
            }
            else if (cleanFilename.StartsWith("ID3_013"))
            {
                Libraries.Background.Draw(22, 0, 0); //desert
            }
            else if (cleanFilename.StartsWith(@"World\GreatWall"))
            {
                Libraries.Background.Draw(23, 0, 0); //greatwall
            }
            else if (cleanFilename.StartsWith("ID3_023") || cleanFilename.StartsWith("ID3_025"))
            {
                Libraries.Background.Draw(21, 0, 0); //village entrance
            }
        }

        private void DrawObjects()
        {
            // Draw yellow highlighted marker
            if (Settings.UseHighlightRing && MapObject.MouseObject != null && MapObject.MouseObject.Blocking && !MapObject.MouseObject.Dead && MapObject.MouseObject != MapObject.TargetObject && MapObject.MouseObject.Blend) //Far
            {
                Libraries.Prguse4.DrawBlend(3, new Point(MapObject.MouseObject.DrawLocation.X - 18, MapObject.MouseObject.DrawLocation.Y - 3), Color.White, false, 0.5f); //Paul
            }
            if (Settings.UseHighlightRing && MapObject.TargetObject != null && !MapObject.TargetObject.Dead && MapObject.TargetObject.Blend) //Far
            {
                Libraries.Prguse4.DrawBlend(2, new Point(MapObject.TargetObject.DrawLocation.X - 18, MapObject.TargetObject.DrawLocation.Y - 3), Color.Red, false, 1f); //Paul
            }

            for (int y = User.Movement.Y - ViewRangeY; y <= User.Movement.Y + ViewRangeY + 25; y++)
            {
                if (y <= 0) continue;
                if (y >= Height) break;
                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    M2CellInfo[x, y].DrawDeadObjects();
                }
            }

            for (int y = User.Movement.Y - ViewRangeY; y <= User.Movement.Y + ViewRangeY + 25; y++)
            {
                if (y <= 0) continue;
                if (y >= Height) break;
                int drawY = (y - User.Movement.Y + OffSetY + 1) * CellHeight + User.OffSetMove.Y;

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    int drawX = (x - User.Movement.X + OffSetX) * CellWidth - OffSetX + User.OffSetMove.X;
                    int index;
                    byte animation;
                    bool blend;
                    Size s;
                    #region Draw shanda's tile animation layer
                    index = M2CellInfo[x, y].TileAnimationImage;
                    animation = M2CellInfo[x, y].TileAnimationFrames;
                    if ((index > 0) & (animation > 0))
                    {
                        index--;
                        int animationoffset = M2CellInfo[x, y].TileAnimationOffset ^ 0x2000;
                        index += animationoffset * (AnimationCount % animation);
                        Libraries.MapLibs[190].DrawUp(index, drawX, drawY);
                    }

                    #endregion

                    #region Draw mir3 middle layer
                    if ((M2CellInfo[x, y].MiddleIndex > 199) && (M2CellInfo[x, y].MiddleIndex != -1))
                    {
                        index = M2CellInfo[x, y].MiddleImage - 1;
                        if (index > 0)
                        {
                            animation = M2CellInfo[x, y].MiddleAnimationFrame;
                            blend = false;
                            if ((animation > 0) && (animation < 255))
                            {
                                if ((animation & 0x0f) > 0)
                                {
                                    blend = true;
                                    animation &= 0x0f;
                                }
                                if (animation > 0)
                                {
                                    byte animationTick = M2CellInfo[x, y].MiddleAnimationTick;
                                    index += (AnimationCount % (animation + (animation * animationTick))) / (1 + animationTick);

                                    if (blend && (animation == 10 || animation == 8)) //diamond mines, abyss blends
                                    {
                                        Libraries.MapLibs[M2CellInfo[x, y].MiddleIndex].DrawUpBlend(index, new Point(drawX, drawY));
                                    }
                                    else
                                    {
                                        Libraries.MapLibs[M2CellInfo[x, y].MiddleIndex].DrawUp(index, drawX, drawY);
                                    }
                                }
                            }
                            s = Libraries.MapLibs[M2CellInfo[x, y].MiddleIndex].GetSize(index);
                            if ((s.Width != CellWidth || s.Height != CellHeight) && (s.Width != (CellWidth * 2) || s.Height != (CellHeight * 2)) && !blend)
                            {
                                Libraries.MapLibs[M2CellInfo[x, y].MiddleIndex].DrawUp(index, drawX, drawY);
                            }
                        }
                    }
                    #endregion

                    #region Draw front layer
                    index = (M2CellInfo[x, y].FrontImage & 0x7FFF) - 1;

                    if (index < 0) continue;

                    int fileIndex = M2CellInfo[x, y].FrontIndex;
                    if (fileIndex == -1) continue;
                    animation = M2CellInfo[x, y].FrontAnimationFrame;

                    if ((animation & 0x80) > 0)
                    {
                        blend = true;
                        animation &= 0x7F;
                    }
                    else
                        blend = false;


                    if (animation > 0)
                    {
                        byte animationTick = M2CellInfo[x, y].FrontAnimationTick;
                        index += (AnimationCount % (animation + (animation * animationTick))) / (1 + animationTick);
                    }


                    if (M2CellInfo[x, y].DoorIndex > 0)
                    {
                        Door DoorInfo = GetDoor(M2CellInfo[x, y].DoorIndex);
                        if (DoorInfo == null)
                        {
                            DoorInfo = new Door() { index = M2CellInfo[x, y].DoorIndex, DoorState = 0, ImageIndex = 0, LastTick = CMain.Time };
                            Doors.Add(DoorInfo);
                        }
                        else
                        {
                            if (DoorInfo.DoorState != 0)
                            {
                                index += (DoorInfo.ImageIndex + 1) * M2CellInfo[x, y].DoorOffset;//'bad' code if you want to use animation but it's gonna depend on the animation > has to be custom designed for the animtion
                            }
                        }
                    }

                    s = Libraries.MapLibs[fileIndex].GetSize(index);
                    if (s.Width == CellWidth && s.Height == CellHeight && animation == 0) continue;
                    if ((s.Width == CellWidth * 2) && (s.Height == CellHeight * 2) && (animation == 0)) continue;

                    if (blend)
                    {
                        if ((fileIndex > 99) & (fileIndex < 199))
                            Libraries.MapLibs[fileIndex].DrawBlend(index, new Point(drawX, drawY - (3 * CellHeight)), Color.White, true);
                        else
                            Libraries.MapLibs[fileIndex].DrawBlend(index, new Point(drawX, drawY - s.Height), Color.White, (index >= 2723 && index <= 2732));
                    }
                    else
                        Libraries.MapLibs[fileIndex].Draw(index, drawX, drawY - s.Height);
                    #endregion
                }

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    M2CellInfo[x, y].DrawObjects();
                }
            }

            DXManager.Sprite.Flush();
            //float oldOpacity = DXManager.Opacity;
            //DXManager.SetOpacity(0.4F);

            //MapObject.User.DrawMount();

            //MapObject.User.DrawBody();

            /*
            if ((MapObject.User.Direction == MirDirection.Up) ||
                (MapObject.User.Direction == MirDirection.UpLeft) ||
                (MapObject.User.Direction == MirDirection.UpRight) ||
                (MapObject.User.Direction == MirDirection.Right) ||
                (MapObject.User.Direction == MirDirection.Left))
            {
                MapObject.User.DrawBody();
                MapObject.User.DrawHead();
                MapObject.User.DrawWings();
            }
            else
            {
                MapObject.User.DrawWings();
                MapObject.User.DrawBody();
                MapObject.User.DrawHead();
            }
            */

            //DXManager.SetOpacity(oldOpacity);

            //  if (MapObject.MouseObject != null && !MapObject.MouseObject.Dead && MapObject.MouseObject != MapObject.TargetObject && MapObject.MouseObject.Blend) //Far
            //  {
            //      MapObject.MouseObject.DrawBlend();
            //  }
            /*
            if (MapObject.User.ite)
            {

            }*/

            //  if (MapObject.TargetObject != null)
            //MapObject.TargetObject.DrawBlend();

            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].DrawEffects(Settings.Effect);

                if (Settings.NameView && !(Objects[i] is ItemObject) && !Objects[i].Dead)
                    Objects[i].DrawName();

                Objects[i].DrawChat();
                Objects[i].DrawHealth();
                Objects[i].DrawPoison();

                Objects[i].DrawDamages();
            }


            if (!Settings.Effect) return;

            for (int i = Effects.Count - 1; i >= 0; i--)
                Effects[i].Draw();
        }

        private void DrawLights(LightSetting setting)
        {
            if (DXManager.Lights == null || DXManager.Lights.Count == 0) return;

            if (_lightTexture == null || _lightTexture.Disposed)
            {
                _lightTexture = new Texture(DXManager.Device, Settings.ScreenWidth, Settings.ScreenHeight, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                _lightTexture.Disposing += FloorTexture_Disposing;
                _lightSurface = _lightTexture.GetSurfaceLevel(0);
            }

            Surface oldSurface = DXManager.CurrentSurface;
            DXManager.SetSurface(_lightSurface);

            #region Night Lights
            Color Darkness = Color.Black;
            switch (MapDarkLight)
            {
                case 1:
                    Darkness = Color.FromArgb(255, 20, 20, 20);
                    break;
                case 2:
                    Darkness = Color.LightSlateGray;
                    break;
                case 3:
                    Darkness = Color.SkyBlue;
                    break;
                case 4:
                    Darkness = Color.Goldenrod;
                    break;
                case 5:
                    Darkness = Color.Black;
                    break;
                default:
                    Darkness = Color.FromArgb(255, 50, 50, 50);
                    break;
            }

            Color TimeOfDayLighting()
            {
                switch (setting)
                {
                    case LightSetting.Dawn:
                        return Color.FromArgb(255, 71, 134, 180);
                        break;
                    case LightSetting.Evening:
                        switch (DateTime.Now.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                return Color.FromArgb(255, 68, 68, 183);
                                break;
                            case DayOfWeek.Monday:
                                return Color.FromArgb(255, 170, 50, 200);
                                break;
                            case DayOfWeek.Tuesday:
                                return Color.FromArgb(255, 48, 109, 203);
                                break;
                            case DayOfWeek.Wednesday:
                                return Color.FromArgb(255, 155, 55, 57);
                                break;
                            case DayOfWeek.Thursday:
                                return Color.FromArgb(255, 132, 114, 53);
                                break;
                            case DayOfWeek.Friday:
                                return Color.FromArgb(255, 177, 73, 170);
                                break;
                            case DayOfWeek.Saturday:
                                return Color.FromArgb(255, 71, 134, 180);
                                break;
                            default:
                                return Color.FromArgb(255, 50, 50, 50);
                                break;
                        }
                        break;
                    case LightSetting.Night:
                        return Color.FromArgb(255, 50, 50, 50);
                        break;
                    default:
                        return Color.FromArgb(255, 50, 50, 50);
                        break;
                }
            }

            DXManager.Device.Clear(ClearFlags.Target, setting == LightSetting.Night ? Darkness : TimeOfDayLighting(), 0, 0);
            //DXManager.Device.Clear(ClearFlags.Target, setting == LightSetting.Night ? Darkness : Color.FromArgb(255, 50, 50, 50), 0, 0);


            #endregion

            int light;
            Point p;
            DXManager.SetBlend(true);
            DXManager.Device.RenderState.SourceBlend = Blend.SourceAlpha;

            #region Object Lights (Player/Mob/NPC)
            for (int i = 0; i < Objects.Count; i++)
            {
                MapObject ob = Objects[i];
                if (ob.Light > 0 && (!ob.Dead || ob == MapObject.User || ob.Race == ObjectType.Spell))
                {

                    light = ob.Light;
                    int LightRange = light % 15;
                    if (LightRange >= DXManager.Lights.Count)
                        LightRange = DXManager.Lights.Count - 1;

                    p = ob.DrawLocation;

                    Color lightColour = ob.LightColour;

                    if (ob.Race == ObjectType.Player)
                    {
                        switch (light / 15)
                        {
                            case 0://no light source
                                lightColour = Color.FromArgb(240, 60, 60, 60);
                                break;
                            case 1:
                                lightColour = Color.FromArgb(240, 120, 120, 120);
                                break;
                            case 2://Candle
                                lightColour = Color.FromArgb(240, 180, 180, 180);
                                break;
                            case 3://Torch
                                lightColour = Color.FromArgb(240, 240, 240, 240);
                                break;
                            default://Peddler Torch
                                lightColour = Color.FromArgb(240, 255, 255, 255);
                                break;
                        }
                    }
                    else if (ob.Race == ObjectType.Merchant)
                    {
                        lightColour = Color.FromArgb(240, 120, 120, 120);
                    }

                    if (DXManager.Lights[LightRange] != null && !DXManager.Lights[LightRange].Disposed)
                    {
                        p.Offset(-(DXManager.LightSizes[LightRange].X / 2) - (CellWidth / 2), -(DXManager.LightSizes[LightRange].Y / 2) - (CellHeight / 2) - 5);
                        DXManager.Sprite.Draw2D(DXManager.Lights[LightRange], Point.Empty, 0, p, lightColour);
                    }

                }
                #region Object Effect Lights
                if (!Settings.Effect) continue;
                for (int e = 0; e < ob.Effects.Count; e++)
                {
                    Effect effect = ob.Effects[e];
                    if (!effect.Blend || CMain.Time < effect.Start || (!(effect is Missile) && effect.Light < ob.Light)) continue;

                    light = effect.Light;

                    p = effect.DrawLocation;

                    if (DXManager.Lights[light] != null && !DXManager.Lights[light].Disposed)
                    {
                        p.Offset(-(DXManager.LightSizes[light].X / 2) - (CellWidth / 2), -(DXManager.LightSizes[light].Y / 2) - (CellHeight / 2) - 5);
                        DXManager.Sprite.Draw2D(DXManager.Lights[light], Point.Empty, 0, p, effect.LightColour);
                    }

                }
                #endregion
            }
            #endregion

            #region Map Effect Lights
            if (Settings.Effect)
            {
                for (int e = 0; e < Effects.Count; e++)
                {
                    Effect effect = Effects[e];
                    if (!effect.Blend || CMain.Time < effect.Start) continue;

                    light = effect.Light;
                    if (light == 0) continue;

                    p = effect.DrawLocation;

                    if (DXManager.Lights[light] != null && !DXManager.Lights[light].Disposed)
                    {
                        p.Offset(-(DXManager.LightSizes[light].X / 2) - (CellWidth / 2), -(DXManager.LightSizes[light].Y / 2) - (CellHeight / 2) - 5);
                        DXManager.Sprite.Draw2D(DXManager.Lights[light], Point.Empty, 0, p, Color.White);
                    }
                }
            }
            #endregion

            #region Map Lights
            for (int y = MapObject.User.Movement.Y - ViewRangeY - 24; y <= MapObject.User.Movement.Y + ViewRangeY + 24; y++)
            {
                if (y < 0) continue;
                if (y >= Height) break;
                for (int x = MapObject.User.Movement.X - ViewRangeX - 24; x < MapObject.User.Movement.X + ViewRangeX + 24; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    int imageIndex = (M2CellInfo[x, y].FrontImage & 0x7FFF) - 1;
                    if (M2CellInfo[x, y].Light <= 0 || M2CellInfo[x, y].Light >= 10) continue;
                    if (M2CellInfo[x, y].Light == 0) continue;

                    Color lightIntensity;

                    light = (M2CellInfo[x, y].Light % 10) * 3;

                    switch (M2CellInfo[x, y].Light / 10)
                    {
                        case 1:
                            lightIntensity = Color.FromArgb(255, 255, 255, 255);
                            break;
                        case 2:
                            lightIntensity = Color.FromArgb(255, 120, 180, 255);
                            break;
                        case 3:
                            lightIntensity = Color.FromArgb(255, 255, 180, 120);
                            break;
                        case 4:
                            lightIntensity = Color.FromArgb(255, 22, 160, 5);
                            break;
                        default:
                            lightIntensity = Color.FromArgb(255, 255, 255, 255);
                            break;
                    }

                    int fileIndex = M2CellInfo[x, y].FrontIndex;

                    p = new Point(
                        (x + OffSetX - MapObject.User.Movement.X) * CellWidth + MapObject.User.OffSetMove.X,
                        (y + OffSetY - MapObject.User.Movement.Y) * CellHeight + MapObject.User.OffSetMove.Y + 32);


                    if (M2CellInfo[x, y].FrontAnimationFrame > 0)
                        p.Offset(Libraries.MapLibs[fileIndex].GetOffSet(imageIndex));

                    if (light >= DXManager.Lights.Count)
                        light = DXManager.Lights.Count - 1;

                    if (DXManager.Lights[light] != null && !DXManager.Lights[light].Disposed)
                    {
                        p.Offset(-(DXManager.LightSizes[light].X / 2) - (CellWidth / 2) + 10, -(DXManager.LightSizes[light].Y / 2) - (CellHeight / 2) - 5);
                        DXManager.Sprite.Draw2D(DXManager.Lights[light], Point.Empty, 0, p, lightIntensity);
                    }
                }
            }
            #endregion

            DXManager.SetBlend(false);
            DXManager.SetSurface(oldSurface);

            DXManager.Device.RenderState.SourceBlend = Blend.DestinationColor;
            DXManager.Device.RenderState.DestinationBlend = Blend.BothInvSourceAlpha;

            DXManager.Sprite.Draw2D(_lightTexture, Point.Empty, 0, Point.Empty, Color.White);
            DXManager.Sprite.End();
            DXManager.Sprite.Begin(SpriteFlags.AlphaBlend);
        }

        private static void OnMouseClick(object sender, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            if (me == null) return;

            if (AwakeningAction == true) return;
            switch (me.Button)
            {
                case MouseButtons.Left:
                    {
                        AutoRun = false;
                        if (MapObject.MouseObject == null) return;
                        NPCObject npc = MapObject.MouseObject as NPCObject;
                        if (npc != null)
                        {
                            GameScene.Scene.NPCDialog.Hide();

                            if (CMain.Time <= GameScene.NPCTime && npc.ObjectID == GameScene.NPCID) return;

                            GameScene.NPCTime = CMain.Time + 5000;
                            GameScene.NPCID = npc.ObjectID;
                            Network.Enqueue(new C.CallNPC { ObjectID = npc.ObjectID, Key = "[@Main]" });
                        }

                        MonsterObject mon = MapObject.MouseObject as MonsterObject;
                        if (mon != null && mon.AI == 201)
                        {
                            GameScene.Scene.NPCDialog.Hide();

                            if (CMain.Time <= GameScene.NPCTime) return;

                            GameScene.NPCTime = CMain.Time + 5000;

                            Network.Enqueue(new C.TalkMonsterNPC { ObjectID = mon.ObjectID });
                        }
                    }
                    break;
                case MouseButtons.Right:
                    {
                        AutoRun = false;
                        if (MapObject.MouseObject == null) return;
                        PlayerObject player = MapObject.MouseObject as PlayerObject;
                        if (player == null || player == User || !CMain.Ctrl) return;
                        if (CMain.Time <= GameScene.InspectTime && player.ObjectID == InspectDialog.InspectID) return;

                        GameScene.InspectTime = CMain.Time + 500;
                        InspectDialog.InspectID = player.ObjectID;
                        Network.Enqueue(new C.Inspect { ObjectID = player.ObjectID });
                    }
                    break;
                case MouseButtons.Middle:
                    AutoRun = !AutoRun;
                    break;
            }
        }

        private static void OnMouseDown(object sender, MouseEventArgs e)
        {
            MapButtons |= e.Button;
            GameScene.CanRun = false;

            if (AwakeningAction == true) return;

            // Paul
            if (e.Button == MouseButtons.Right && Settings.RightClickClearsTarget)
                MapObject.TargetObject = null;

            if (e.Button != MouseButtons.Left) return;

            if (GameScene.SelectedCell != null)
            {
                if (GameScene.SelectedCell.GridType != MirGridType.Inventory)
                {
                    GameScene.SelectedCell = null;
                    return;
                }

                MirItemCell cell = GameScene.SelectedCell;
                if (cell.Item.Info.Bind.HasFlag(BindMode.DontDrop))
                {
                    MirMessageBox messageBox = new MirMessageBox(string.Format("You cannot drop {0}", cell.Item.Name), MirMessageBoxButtons.OK);
                    messageBox.Show();
                    GameScene.SelectedCell = null;
                    return;
                }
                if (cell.Item.Count == 1)
                {
                    MirMessageBox messageBox = new MirMessageBox(string.Format("Are you sure you want to drop {0}?", cell.Item.FriendlyName), MirMessageBoxButtons.YesNo);

                    messageBox.YesButton.Click += (o, a) =>
                    {
                        Network.Enqueue(new C.DropItem { UniqueID = cell.Item.UniqueID, Count = 1 });

                        cell.Locked = true;
                    };
                    messageBox.Show();
                }
                else
                {
                    MirAmountBox amountBox = new MirAmountBox("Drop Amount:", cell.Item.Info.Image, cell.Item.Count);

                    amountBox.OKButton.Click += (o, a) =>
                    {
                        if (amountBox.Amount <= 0) return;
                        Network.Enqueue(new C.DropItem
                        {
                            UniqueID = cell.Item.UniqueID,
                            Count = amountBox.Amount
                        });

                        cell.Locked = true;
                    };

                    amountBox.Show();
                }
                GameScene.SelectedCell = null;

                return;
            }
            if (GameScene.PickedUpGold)
            {
                MirAmountBox amountBox = new MirAmountBox("Drop Amount:", 116, GameScene.Gold);

                amountBox.OKButton.Click += (o, a) =>
                {
                    if (amountBox.Amount > 0)
                    {
                        Network.Enqueue(new C.DropGold { Amount = amountBox.Amount });
                    }
                };

                amountBox.Show();
                GameScene.PickedUpGold = false;
            }



            if (MapObject.MouseObject != null && !MapObject.MouseObject.Dead && !(MapObject.MouseObject is ItemObject) &&
                !(MapObject.MouseObject is NPCObject) && !(MapObject.MouseObject is MonsterObject && MapObject.MouseObject.AI == 64)
                 && !(MapObject.MouseObject is MonsterObject && MapObject.MouseObject.AI == 70))
            {
                MapObject.TargetObject = MapObject.MouseObject;
                if (MapObject.MouseObject is MonsterObject && MapObject.MouseObject.AI != 6)
                    MapObject.MagicObject = MapObject.TargetObject;
            }
            else
                MapObject.TargetObject = null;
        }

        private void CheckInput()
        {
            if (AwakeningAction == true) return;

            if ((MouseControl == this) && (MapButtons != MouseButtons.None)) AutoHit = false;//mouse actions stop mining even when frozen!
            if (!CanRideAttack()) AutoHit = false;

            if (CMain.Time < InputDelay || User.Poison.HasFlag(PoisonType.Paralysis) || User.Poison.HasFlag(PoisonType.LRParalysis) || User.Poison.HasFlag(PoisonType.Frozen) || User.Fishing) return;

            if (User.NextMagic != null && !User.RidingMount)
            {
                UseMagic(User.NextMagic);
                return;
            }

            if (CMain.Time < User.BlizzardStopTime || CMain.Time < User.ReincarnationStopTime) return;

            if (MapObject.TargetObject != null && !MapObject.TargetObject.Dead)
            {
                if (((MapObject.TargetObject.Name.EndsWith(")") || MapObject.TargetObject is PlayerObject) && CMain.Shift) ||
                    (!MapObject.TargetObject.Name.EndsWith(")") && MapObject.TargetObject is MonsterObject))
                {

                    GameScene.LogTime = CMain.Time + Globals.LogDelay;

                    if (User.Class == MirClass.Archer && User.HasClassWeapon && !User.RidingMount && !User.Fishing)//ArcherTest - non aggressive targets (player / pets)
                    {
                        if (Functions.InRange(MapObject.TargetObject.CurrentLocation, User.CurrentLocation, Globals.MaxAttackRange))
                        {
                            if (CMain.Time > GameScene.AttackTime)
                            {
                                User.QueuedAction = new QueuedAction { Action = MirAction.AttackRange1, Direction = Functions.DirectionFromPoint(User.CurrentLocation, MapObject.TargetObject.CurrentLocation), Location = User.CurrentLocation, Params = new List<object>() };
                                User.QueuedAction.Params.Add(MapObject.TargetObject != null ? MapObject.TargetObject.ObjectID : (uint)0);
                                User.QueuedAction.Params.Add(MapObject.TargetObject.CurrentLocation);

                                // MapObject.TargetObject = null; //stop constant attack when close up
                            }
                        }
                        else
                        {
                            if (CMain.Time >= OutputDelay)
                            {
                                OutputDelay = CMain.Time + 1000;
                                GameScene.Scene.OutputMessage("Target is too far.");
                            }
                        }
                        //  return;
                    }

                    else if (Functions.InRange(MapObject.TargetObject.CurrentLocation, User.CurrentLocation, 1))
                    {
                        if (CMain.Time > GameScene.AttackTime && CanRideAttack())
                        {
                            User.QueuedAction = new QueuedAction { Action = MirAction.Attack1, Direction = Functions.DirectionFromPoint(User.CurrentLocation, MapObject.TargetObject.CurrentLocation), Location = User.CurrentLocation };
                            return;
                        }
                    }
                }
            }
            if (AutoHit && !User.RidingMount)
            {
                if (CMain.Time > GameScene.AttackTime)
                {
                    User.QueuedAction = new QueuedAction { Action = MirAction.Mine, Direction = User.Direction, Location = User.CurrentLocation };
                    return;
                }
            }


            MirDirection direction;
            if (MouseControl == this)
            {
                direction = MouseDirection();
                if (AutoRun)
                {
                    if (GameScene.CanRun && CanRun(direction) && CMain.Time > GameScene.NextRunTime && User.HP >= 10 && (!User.Sneaking || (User.Sneaking && User.Sprint))) //slow remove
                    {
                        int distance = User.RidingMount || User.Sprint && !User.Sneaking ? 3 : 2;
                        bool fail = false;
                        for (int i = 1; i <= distance; i++)
                        {
                            if (!CheckDoorOpen(Functions.PointMove(User.CurrentLocation, direction, i)))
                                fail = true;
                        }
                        if (!fail)
                        {
                            User.QueuedAction = new QueuedAction { Action = MirAction.Running, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, distance) };
                            return;
                        }
                    }
                    if ((CanWalk(direction)) && (CheckDoorOpen(Functions.PointMove(User.CurrentLocation, direction, 1))))
                    {
                        User.QueuedAction = new QueuedAction { Action = MirAction.Walking, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };
                        return;
                    }
                    if (direction != User.Direction)
                    {
                        User.QueuedAction = new QueuedAction { Action = MirAction.Standing, Direction = direction, Location = User.CurrentLocation };
                        return;
                    }
                    return;
                }

                switch (MapButtons)
                {
                    case MouseButtons.Left:
                        if (MapObject.MouseObject is NPCObject || (MapObject.MouseObject is PlayerObject && MapObject.MouseObject != User)) break;
                        if (MapObject.MouseObject is MonsterObject && MapObject.MouseObject.AI == 70) break;


                        if (CMain.Alt && !User.RidingMount)
                        {
                            User.QueuedAction = new QueuedAction { Action = MirAction.Harvest, Direction = direction, Location = User.CurrentLocation };
                            return;
                        }
                        if (CMain.Shift)
                        {
                            if (CMain.Time > GameScene.AttackTime && CanRideAttack()) //ArcherTest - shift click
                            {
                                MapObject target = null;
                                if (MapObject.MouseObject is MonsterObject || MapObject.MouseObject is PlayerObject) target = MapObject.MouseObject;

                                if (User.Class == MirClass.Archer && User.HasClassWeapon && !User.RidingMount)
                                {
                                    if (target != null)
                                    {
                                        if (!Functions.InRange(MapObject.MouseObject.CurrentLocation, User.CurrentLocation, Globals.MaxAttackRange))
                                        {
                                            if (CMain.Time >= OutputDelay)
                                            {
                                                OutputDelay = CMain.Time + 1000;
                                                GameScene.Scene.OutputMessage("Target is too far.");
                                            }
                                            return;
                                        }
                                    }


                                    User.QueuedAction.Params.Add(target != null ? target.ObjectID : (uint)0);
                                    return;
                                }

                                //stops double slash from being used without empty hand or assassin weapon (otherwise bugs on second swing)
                                if (GameScene.DoubleSlash && (!User.HasClassWeapon && User.Weapon > -1)) return;

                                User.QueuedAction = new QueuedAction { Action = MirAction.Attack1, Direction = direction, Location = User.CurrentLocation };
                            }
                            return;
                        }

                        if (MapObject.MouseObject is MonsterObject && User.Class == MirClass.Archer && MapObject.TargetObject != null && !MapObject.TargetObject.Dead && User.HasClassWeapon && !User.RidingMount) //ArcherTest - range attack
                        {
                            if (Functions.InRange(MapObject.MouseObject.CurrentLocation, User.CurrentLocation, Globals.MaxAttackRange))
                            {
                                if (CMain.Time > GameScene.AttackTime)
                                {
                                    User.QueuedAction = new QueuedAction { Action = MirAction.AttackRange1, Direction = direction, Location = User.CurrentLocation, Params = new List<object>() };
                                    User.QueuedAction.Params.Add(MapObject.TargetObject.ObjectID);
                                    User.QueuedAction.Params.Add(MapObject.TargetObject.CurrentLocation);
                                }
                            }
                            else
                            {
                                if (CMain.Time >= OutputDelay)
                                {
                                    OutputDelay = CMain.Time + 1000;
                                    GameScene.Scene.OutputMessage("Target is too far.");
                                }
                            }
                            return;
                        }

                        if (MapLocation == User.CurrentLocation)
                        {
                            if (CMain.Time > GameScene.PickUpTime)
                            {
                                GameScene.PickUpTime = CMain.Time + 200;
                                Network.Enqueue(new C.PickUp());
                            }
                            return;
                        }

                        //mine
                        if (!ValidPoint(Functions.PointMove(User.CurrentLocation, direction, 1)))
                        {
                            if ((MapObject.User.Equipment[(int)EquipmentSlot.Weapon] != null) && (MapObject.User.Equipment[(int)EquipmentSlot.Weapon].Info.CanMine))
                            {
                                if (direction != User.Direction)
                                {
                                    User.QueuedAction = new QueuedAction { Action = MirAction.Standing, Direction = direction, Location = User.CurrentLocation };
                                    return;
                                }
                                AutoHit = true;
                                return;
                            }
                        }
                        if ((CanWalk(direction)) && (CheckDoorOpen(Functions.PointMove(User.CurrentLocation, direction, 1))))
                        {

                            User.QueuedAction = new QueuedAction { Action = MirAction.Walking, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };
                            return;
                        }
                        if (direction != User.Direction)
                        {
                            User.QueuedAction = new QueuedAction { Action = MirAction.Standing, Direction = direction, Location = User.CurrentLocation };
                            return;
                        }

                        if (CanFish(direction))
                        {
                            User.FishingTime = CMain.Time;
                            Network.Enqueue(new C.FishingCast { CastOut = true });
                            return;
                        }

                        break;
                    case MouseButtons.Right:
                        if (MapObject.MouseObject is PlayerObject && MapObject.MouseObject != User && CMain.Ctrl) break;

                        if (Functions.InRange(MapLocation, User.CurrentLocation, 2))
                        {
                            if (direction != User.Direction)
                            {
                                User.QueuedAction = new QueuedAction { Action = MirAction.Standing, Direction = direction, Location = User.CurrentLocation };
                            }
                            return;
                        }

                        GameScene.CanRun = User.FastRun ? true : GameScene.CanRun;

                        if (GameScene.CanRun && CanRun(direction) && CMain.Time > GameScene.NextRunTime && User.HP >= 10 && (!User.Sneaking || (User.Sneaking && User.Sprint))) //slow removed
                        {
                            int distance = User.RidingMount || User.Sprint && !User.Sneaking ? 3 : 2;
                            bool fail = false;
                            for (int i = 0; i <= distance; i++)
                            {
                                if (!CheckDoorOpen(Functions.PointMove(User.CurrentLocation, direction, i)))
                                    fail = true;
                            }
                            if (!fail)
                            {
                                User.QueuedAction = new QueuedAction { Action = MirAction.Running, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, User.RidingMount || (User.Sprint && !User.Sneaking) ? 3 : 2) };
                                return;
                            }
                        }
                        if ((CanWalk(direction)) && (CheckDoorOpen(Functions.PointMove(User.CurrentLocation, direction, 1))))
                        {
                            User.QueuedAction = new QueuedAction { Action = MirAction.Walking, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };
                            return;
                        }
                        if (direction != User.Direction)
                        {
                            User.QueuedAction = new QueuedAction { Action = MirAction.Standing, Direction = direction, Location = User.CurrentLocation };
                            return;
                        }
                        break;
                }
            }

            if (AutoPath)
            {
                if (CurrentPath == null || CurrentPath.Count == 0)
                {
                    AutoPath = false;
                    return;
                }

                Node currentNode = CurrentPath.SingleOrDefault(x => User.CurrentLocation == x.Location);
                if (currentNode != null)
                {
                    while (true)
                    {
                        Node first = CurrentPath.First();
                        CurrentPath.Remove(first);

                        if (first == currentNode)
                            break;
                    }
                }

                if (CurrentPath.Count > 0)
                {
                    MirDirection dir = Functions.DirectionFromPoint(User.CurrentLocation, CurrentPath.First().Location);
                    Node upcomingStep = CurrentPath.SingleOrDefault(x => Functions.PointMove(User.CurrentLocation, dir, 2) == x.Location);

                    if (!CanWalk(dir))
                    {
                        CurrentPath = PathFinder.FindPath(MapObject.User.CurrentLocation, CurrentPath.Last().Location);
                        return;
                    }

                    if (GameScene.CanRun && CanRun(dir) && CMain.Time > GameScene.NextRunTime && User.HP >= 10 && CurrentPath.Count > 1 && upcomingStep != null)
                    {
                        User.QueuedAction = new QueuedAction { Action = MirAction.Running, Direction = dir, Location = Functions.PointMove(User.CurrentLocation, dir, 2) };
                        return;
                    }
                    if (CanWalk(dir))
                    {
                        User.QueuedAction = new QueuedAction { Action = MirAction.Walking, Direction = dir, Location = Functions.PointMove(User.CurrentLocation, dir, 1) };

                        return;
                    }
                }
            }

            if (MapObject.TargetObject == null || MapObject.TargetObject.Dead) return;
            if (((!MapObject.TargetObject.Name.EndsWith(")") && !(MapObject.TargetObject is PlayerObject)) || !CMain.Shift) &&
                (MapObject.TargetObject.Name.EndsWith(")") || !(MapObject.TargetObject is MonsterObject))) return;
            if (Functions.InRange(MapObject.TargetObject.CurrentLocation, User.CurrentLocation, 1)) return;
            if (User.Class == MirClass.Archer && User.HasClassWeapon && (MapObject.TargetObject is MonsterObject || MapObject.TargetObject is PlayerObject)) return; //ArcherTest - stop walking
            direction = Functions.DirectionFromPoint(User.CurrentLocation, MapObject.TargetObject.CurrentLocation);

            if (!CanWalk(direction)) return;

            User.QueuedAction = new QueuedAction { Action = MirAction.Walking, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };
        }

        private void UseMagic(ClientMagic magic)
        {
            if (CMain.Time < GameScene.SpellTime || User.Poison.HasFlag(PoisonType.Stun))
            {
                User.ClearMagic();
                return;
            }

            if ((CMain.Time <= magic.CastTime + magic.Delay) && magic.CastTime > 0)
            {
                if (CMain.Time >= OutputDelay)
                {
                    OutputDelay = CMain.Time + 1000;
                    GameScene.Scene.OutputMessage(string.Format("You cannot cast [{0}] for another [{1}] seconds.", magic.Spell.ToString(), ((magic.CastTime + magic.Delay) - CMain.Time - 1) / 1000 + 1));
                }

                User.ClearMagic();
                return;
            }

            int cost = magic.Level * magic.LevelCost + magic.BaseCost;

            if (magic.Spell == Spell.Teleport || magic.Spell == Spell.Blink || magic.Spell == Spell.StormEscape)
            {
                for (int i = 0; i < GameScene.Scene.Buffs.Count; i++)
                {
                    if (GameScene.Scene.Buffs[i].Type != BuffType.TemporalFlux) continue;
                    cost += (int)(User.MaxMP * 0.3F);
                }
            }

            if (cost > MapObject.User.MP)
            {
                if (CMain.Time >= OutputDelay)
                {
                    OutputDelay = CMain.Time + 1000;
                    GameScene.Scene.OutputMessage("Not Enough Mana to cast.");
                }
                User.ClearMagic();
                return;
            }

            //bool isTargetSpell = true;

            MapObject target = null;

            //Targeting
            switch (magic.Spell)
            {
                case Spell.FireBall:
                case Spell.GreatFireBall:
                case Spell.ElectricShock:
                case Spell.Poisoning:
                case Spell.ThunderBolt:
                case Spell.FlameDisruptor:
                case Spell.SoulFireBall:
                case Spell.TurnUndead:
                case Spell.FrostCrunch:
                case Spell.Vampirism:
                case Spell.Revelation:
                case Spell.Entrapment:
                case Spell.Hallucination:
                case Spell.DarkBody:
                    if (User.NextMagicObject != null)
                    {
                        if (!User.NextMagicObject.Dead && User.NextMagicObject.Race != ObjectType.Item && User.NextMagicObject.Race != ObjectType.Merchant)
                            target = User.NextMagicObject;
                    }

                    if (target == null) target = MapObject.MagicObject;

                    if (target != null && target.Race == ObjectType.Monster) MapObject.MagicObject = target;
                    break;
                case Spell.StraightShot:
                case Spell.DoubleShot:
                case Spell.ElementalShot:
                case Spell.DelayedExplosion:
                case Spell.BindingShot:
                case Spell.VampireShot:
                case Spell.PoisonShot:
                case Spell.CrippleShot:
                case Spell.NapalmShot:
                case Spell.SummonVampire:
                case Spell.SummonToad:
                case Spell.SummonSnakes:
                    if (!User.HasClassWeapon)
                    {
                        GameScene.Scene.OutputMessage("You must be wearing a bow to perform this skill.");
                        User.ClearMagic();
                        return;
                    }
                    if (User.NextMagicObject != null)
                    {
                        if (!User.NextMagicObject.Dead && User.NextMagicObject.Race != ObjectType.Item && User.NextMagicObject.Race != ObjectType.Merchant)
                            target = User.NextMagicObject;
                    }

                    if (target == null) target = MapObject.MagicObject;

                    if (target != null && target.Race == ObjectType.Monster) MapObject.MagicObject = target;

                    //if(magic.Spell == Spell.ElementalShot)
                    //{
                    //    isTargetSpell = User.HasElements;
                    //}

                    //switch(magic.Spell)
                    //{
                    //    case Spell.SummonVampire:
                    //    case Spell.SummonToad:
                    //    case Spell.SummonSnakes:
                    //        isTargetSpell = false;
                    //        break;
                    //}

                    break;
                case Spell.Purification:
                case Spell.Healing:
                case Spell.UltimateEnhancer:
                case Spell.EnergyShield:
                case Spell.PetEnhancer:
                    if (User.NextMagicObject != null)
                    {
                        if (!User.NextMagicObject.Dead && User.NextMagicObject.Race != ObjectType.Item && User.NextMagicObject.Race != ObjectType.Merchant)
                            target = User.NextMagicObject;
                    }

                    if (target == null) target = User;
                    break;
                case Spell.FireBang:
                case Spell.MassHiding:
                case Spell.FireWall:
                case Spell.TrapHexagon:
                    if (User.NextMagicObject != null)
                    {
                        if (!User.NextMagicObject.Dead && User.NextMagicObject.Race != ObjectType.Item && User.NextMagicObject.Race != ObjectType.Merchant)
                            target = User.NextMagicObject;
                    }
                    break;
                case Spell.PoisonCloud:
                    if (User.NextMagicObject != null)
                    {
                        if (!User.NextMagicObject.Dead && User.NextMagicObject.Race != ObjectType.Item && User.NextMagicObject.Race != ObjectType.Merchant)
                            target = User.NextMagicObject;
                    }
                    break;
                case Spell.Blizzard:
                case Spell.MeteorStrike:
                    if (User.NextMagicObject != null)
                    {
                        if (!User.NextMagicObject.Dead && User.NextMagicObject.Race != ObjectType.Item && User.NextMagicObject.Race != ObjectType.Merchant)
                            target = User.NextMagicObject;
                    }
                    break;
                case Spell.Reincarnation:
                    if (User.NextMagicObject != null)
                    {
                        if (User.NextMagicObject.Dead && User.NextMagicObject.Race == ObjectType.Player)
                            target = User.NextMagicObject;
                    }
                    break;
                case Spell.Trap:
                    if (User.NextMagicObject != null)
                    {
                        if (!User.NextMagicObject.Dead && User.NextMagicObject.Race != ObjectType.Item && User.NextMagicObject.Race != ObjectType.Merchant)
                            target = User.NextMagicObject;
                    }
                    break;
                case Spell.FlashDash:
                    if (User.GetMagic(Spell.FlashDash).Level <= 1 && User.IsDashAttack() == false)
                    {
                        User.ClearMagic();
                        return;
                    }
                    //isTargetSpell = false;
                    break;
                default:
                    //isTargetSpell = false;
                    break;
            }

            MirDirection dir = (target == null || target == User) ? User.NextMagicDirection : Functions.DirectionFromPoint(User.CurrentLocation, target.CurrentLocation);

            Point location = target != null ? target.CurrentLocation : User.NextMagicLocation;

            if (magic.Spell == Spell.FlashDash)
                dir = User.Direction;

            if ((magic.Range != 0) && (!Functions.InRange(User.CurrentLocation, location, magic.Range)))
            {
                if (CMain.Time >= OutputDelay)
                {
                    OutputDelay = CMain.Time + 1000;
                    GameScene.Scene.OutputMessage("Target is too far.");
                }
                User.ClearMagic();
                return;
            }

            GameScene.LogTime = CMain.Time + Globals.LogDelay;

            User.QueuedAction = new QueuedAction { Action = MirAction.Spell, Direction = dir, Location = User.CurrentLocation, Params = new List<object>() };
            User.QueuedAction.Params.Add(magic.Spell);
            User.QueuedAction.Params.Add(target != null ? target.ObjectID : 0);
            User.QueuedAction.Params.Add(location);
            User.QueuedAction.Params.Add(magic.Level);
        }


        public static MirDirection MouseDirection(float ratio = 45F) //22.5 = 16
        {
            Point p = new Point(MouseLocation.X / CellWidth, MouseLocation.Y / CellHeight);
            if (Functions.InRange(new Point(OffSetX, OffSetY), p, 2))
                return Functions.DirectionFromPoint(new Point(OffSetX, OffSetY), p);

            PointF c = new PointF(OffSetX * CellWidth + CellWidth / 2F, OffSetY * CellHeight + CellHeight / 2F);
            PointF a = new PointF(c.X, 0);
            PointF b = MouseLocation;
            float bc = (float)Distance(c, b);
            float ac = bc;
            b.Y -= c.Y;
            c.Y += bc;
            b.Y += bc;
            float ab = (float)Distance(b, a);
            double x = (ac * ac + bc * bc - ab * ab) / (2 * ac * bc);
            double angle = Math.Acos(x);

            angle *= 180 / Math.PI;

            if (MouseLocation.X < c.X) angle = 360 - angle;
            angle += ratio / 2;
            if (angle > 360) angle -= 360;

            return (MirDirection)(angle / ratio);
        }

        //bugged

        public static int Direction16(Point source, Point destination)
        {
            PointF c = new Point(source.X, source.Y);
            PointF a = new PointF(c.X, 0);
            PointF b = new Point(destination.X, destination.Y);
            float bc = (float)Distance(c, b);
            float ac = bc;
            b.Y -= c.Y;
            c.Y += bc;
            b.Y += bc;
            float ab = (float)Distance(b, a);
            double x = (ac * ac + bc * bc - ab * ab) / (2 * ac * bc);
            double angle = Math.Acos(x);

            angle *= 180 / Math.PI;

            if (destination.X < c.X) angle = 360 - angle;
            angle += 11.25F;
            if (angle > 360) angle -= 360;

            return (int)(angle / 22.5F);
        }

        public static double Distance(PointF p1, PointF p2)
        {
            double x = p2.X - p1.X;
            double y = p2.Y - p1.Y;
            return Math.Sqrt(x * x + y * y);
        }

        public bool EmptyCell(Point p)
        {
            if ((M2CellInfo[p.X, p.Y].BackImage & 0x20000000) != 0 || (M2CellInfo[p.X, p.Y].FrontImage & 0x8000) != 0) // + (M2CellInfo[P.X, P.Y].FrontImage & 0x7FFF) != 0)
                return false;

            for (int i = 0; i < Objects.Count; i++)
            {
                MapObject ob = Objects[i];

                if (ob.CurrentLocation == p && ob.Blocking)
                    return false;
            }

            return true;
        }

        private bool CanWalk(MirDirection dir)
        {
            return EmptyCell(Functions.PointMove(User.CurrentLocation, dir, 1)) && !User.InTrapRock;
        }

        private bool CheckDoorOpen(Point p)
        {
            if (M2CellInfo[p.X, p.Y].DoorIndex == 0) return true;
            Door DoorInfo = GetDoor(M2CellInfo[p.X, p.Y].DoorIndex);
            if (DoorInfo == null) return false;//if the door doesnt exist then it isnt even being shown on screen (and cant be open lol)
            if ((DoorInfo.DoorState == 0) || (DoorInfo.DoorState == 3))
            {
                Network.Enqueue(new C.Opendoor() { DoorIndex = DoorInfo.index });
                return false;
            }
            if ((DoorInfo.DoorState == 2) && (DoorInfo.LastTick + 4000 > CMain.Time))
            {
                Network.Enqueue(new C.Opendoor() { DoorIndex = DoorInfo.index });
            }
            return true;
        }


        private bool CanRun(MirDirection dir)
        {
            if (User.InTrapRock) return false;
            if (User.CurrentBagWeight > User.MaxBagWeight) return false;

            if (CanWalk(dir) && EmptyCell(Functions.PointMove(User.CurrentLocation, dir, 2)))
            {
                if (User.RidingMount || User.Sprint && !User.Sneaking)
                {
                    return EmptyCell(Functions.PointMove(User.CurrentLocation, dir, 3));
                }

                return true;
            }

            return false;
        }

        private bool CanRideAttack()
        {
            if (GameScene.User.RidingMount)
            {
                UserItem item = GameScene.User.Equipment[(int)EquipmentSlot.Mount];
                if (item == null || item.Slots.Length < 4 || item.Slots[(int)MountSlot.Bells] == null) return false;
            }

            return true;
        }

        public bool CanFish(MirDirection dir)
        {
            if (!GameScene.User.HasFishingRod || GameScene.User.FishingTime + 1000 > CMain.Time) return false;
            if (GameScene.User.CurrentAction != MirAction.Standing) return false;
            if (GameScene.User.Direction != dir) return false;
            if (GameScene.User.TransformType >= 6 && GameScene.User.TransformType <= 9) return false;

            Point point = Functions.PointMove(User.CurrentLocation, dir, 3);

            if (!M2CellInfo[point.X, point.Y].FishingCell) return false;

            return true;
        }

        public bool CanFly(Point target)
        {
            Point location = User.CurrentLocation;
            while (location != target)
            {
                MirDirection dir = Functions.DirectionFromPoint(location, target);

                location = Functions.PointMove(location, dir, 1);

                if (location.X < 0 || location.Y < 0 || location.X >= GameScene.Scene.MapControl.Width || location.Y >= GameScene.Scene.MapControl.Height) return false;

                if (!GameScene.Scene.MapControl.ValidPoint(location)) return false;
            }

            return true;
        }


        public bool ValidPoint(Point p)
        {
            //GameScene.Scene.ChatDialog.ReceiveChat(string.Format("cell: {0}", (M2CellInfo[p.X, p.Y].BackImage & 0x20000000)), ChatType.Hint);
            return (M2CellInfo[p.X, p.Y].BackImage & 0x20000000) == 0;
        }
        public bool HasTarget(Point p)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                MapObject ob = Objects[i];

                if (ob.CurrentLocation == p && ob.Blocking)
                    return true;
            }
            return false;
        }
        public bool CanHalfMoon(Point p, MirDirection d)
        {
            d = Functions.PreviousDir(d);
            for (int i = 0; i < 4; i++)
            {
                if (HasTarget(Functions.PointMove(p, d, 1))) return true;
                d = Functions.NextDir(d);
            }
            return false;
        }
        public bool CanCrossHalfMoon(Point p)
        {
            MirDirection dir = MirDirection.Up;
            for (int i = 0; i < 8; i++)
            {
                if (HasTarget(Functions.PointMove(p, dir, 1))) return true;
                dir = Functions.NextDir(dir);
            }
            return false;
        }

        private void FloorTexture_Disposing(object sender, EventArgs e)
        {
            FloorValid = false;
            _floorTexture = null;

            if (_floorSurface != null && !_floorSurface.Disposed)
                _floorSurface.Dispose();
            _floorSurface = null;
        }
        #region Disposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Objects.Clear();

                MapButtons = 0;
                MouseLocation = Point.Empty;
                InputDelay = 0;
                NextAction = 0;

                M2CellInfo = null;
                Width = 0;
                Height = 0;

                FileName = String.Empty;
                Title = String.Empty;
                MiniMap = 0;
                BigMap = 0;
                Lights = 0;
                FloorValid = false;
                LightsValid = false;
                MapDarkLight = 0;
                Music = 0;

                if (_floorSurface != null && !_floorSurface.Disposed)
                    _floorSurface.Dispose();


                if (_lightSurface != null && !_lightSurface.Disposed)
                    _lightSurface.Dispose();

                AnimationCount = 0;
                Effects.Clear();
            }

            base.Dispose(disposing);
        }

        #endregion



        public void RemoveObject(MapObject ob)
        {
            M2CellInfo[ob.MapLocation.X, ob.MapLocation.Y].RemoveObject(ob);
        }
        public void AddObject(MapObject ob)
        {
            M2CellInfo[ob.MapLocation.X, ob.MapLocation.Y].AddObject(ob);
        }
        public MapObject FindObject(uint ObjectID, int x, int y)
        {
            return M2CellInfo[x, y].FindObject(ObjectID);
        }
        public void SortObject(MapObject ob)
        {
            M2CellInfo[ob.MapLocation.X, ob.MapLocation.Y].Sort();
        }

        public Door GetDoor(byte Index)
        {
            for (int i = 0; i < Doors.Count; i++)
            {
                if (Doors[i].index == Index)
                    return Doors[i];
            }
            return null;
        }
        public void Processdoors()
        {
            for (int i = 0; i < Doors.Count; i++)
            {
                if ((Doors[i].DoorState == 1) || (Doors[i].DoorState == 3))
                {
                    if (Doors[i].LastTick + 50 < CMain.Time)
                    {
                        Doors[i].LastTick = CMain.Time;
                        Doors[i].ImageIndex++;
                        if (Doors[i].ImageIndex == 1)//change the 1 if you want to actualy animate doors opening/closing
                        {
                            Doors[i].ImageIndex = 0;
                            Doors[i].DoorState = (byte)(++Doors[i].DoorState % 4);
                        }
                        FloorValid = false;
                    }
                }
                if (Doors[i].DoorState == 2)
                {
                    if (Doors[i].LastTick + 5000 < CMain.Time)
                    {
                        Doors[i].LastTick = CMain.Time;
                        Doors[i].DoorState = 3;
                        FloorValid = false;
                    }
                }
            }
        }
        public void OpenDoor(byte Index, bool Closed)
        {
            Door Info = GetDoor(Index);
            if (Info == null) return;
            Info.DoorState = (byte)(Closed ? 3 : Info.DoorState == 2 ? 2 : 1);
            Info.ImageIndex = 0;
            Info.LastTick = CMain.Time;
        }
    }

    public class Buff
    {
        public BuffType Type;
        public string Caster;
        public bool Visible;
        public uint ObjectID;
        public long Expire;
        public int[] Values;
        public bool Infinite;

        public override string ToString()
        {
            string text = string.Empty;

            switch (Type)
            {
                //magic
                case BuffType.TemporalFlux:
                    text = string.Format("[Temporal Flux]\nIncreases cost of next Teleport by: [{0}] MP.\n", (int)(MapObject.User.MaxMP * 0.3F));
                    break;
                case BuffType.Hiding:
                    text = "[Hiding]\nInvisible to many monsters.\n";
                    break;
                case BuffType.Haste:
                    text = string.Format("[Haste]\nIncreases Attack Speed by: {0}.\n", Values[0]);
                    break;
                case BuffType.SwiftFeet:
                    text = string.Format("[Swift Feet]\nIncreases Move Speed by: {0}.\n", Values[0]);
                    break;
                case BuffType.Fury:
                    text = string.Format("[Fury]\nIncreases Attack Speed by: {0}.\n", Values[0]);
                    break;
                case BuffType.LightBody:
                    text = string.Format("[Light Body]\nIncreases Agility by: {0}.\n", Values[0]);
                    break;
                case BuffType.SoulShield:
                    text = string.Format("[Soul Shield]\nIncreases MAC by: 0-{0}.\n", Values[0]);
                    break;
                case BuffType.BlessedArmour:
                    text = string.Format("[Blessed Armour]\nIncreases AC by: 0-{0}.\n", Values[0]);
                    break;
                case BuffType.ProtectionField:
                    text = string.Format("[Protection Field]\nIncreases AC by: 0-{0}.\n", Values[0]);
                    break;
                case BuffType.Rage:
                    text = string.Format("[Rage]\nIncreases DC by: 0-{0}.\n", Values[0]);
                    break;
                case BuffType.ImmortalSkin:
                    text = string.Format("[ImmortalSkin]\nIncreases AC by: 0-{0}.\nDecreases DC by: 0-{1}.\n", Values[0], Values[1]);
                    break;
                case BuffType.CounterAttack:
                    text = string.Format("[Counter Attack]\nIncreases AC/MAC by: {0}-{1}.\n", Values[0], Values[0]);
                    break;
                case BuffType.UltimateEnhancer:
                    if (GameScene.User.Class == MirClass.Wizard || GameScene.User.Class == MirClass.Archer)
                    {
                        text = string.Format("[Ultimate Enhancer]\nIncreases MC by: 0-{0}.\n", Values[0]);
                    }
                    else if (GameScene.User.Class == MirClass.Taoist)
                    {
                        text = string.Format("[Ultimate Enhancer]\nIncreases SC by: 0-{0}.\nIncreases DC by: 0-{1}.\n", Values[0], Values[0]);
                    }
                    else
                    {
                        text = string.Format("[Ultimate Enhancer]\nIncreases DC by: 0-{0}.\n", Values[0]);
                    }
                    break;
                case BuffType.EnergyShield:
                    text = string.Format("[Energy Shield]\n{0}% chance to gain [{1}] HP when attacked\n", Math.Round((1 / (decimal)Values[0]) * 100), Values[1]);
                    break;
                case BuffType.Curse:
                    text = string.Format("[Cursed]\nDecreases DC/MC/SC/ASpeed by: {0}%.\n", Values[0]);
                    break;
                case BuffType.MoonLight:
                    text = "[Moon Light]\nInvisible to players and many\nmonsters when at a distance.\n";
                    break;
                case BuffType.DarkBody:
                    text = "[Dark Body]\nInvisible to many monsters and able to move.\n";
                    break;
                case BuffType.VampireShot:
                    text = string.Format("Vampire Shot\nGives you a vampiric ability\nthat can be released with\ncertain skills.\n", Values[0]);
                    break;
                case BuffType.PoisonShot:
                    text = string.Format("Poison Shot\nGives you a poison ability\nthat can be released with\ncertain skills.\n", Values[0]);
                    break;
                case BuffType.Concentration:
                    text = "Concentrating\nIncreases chance on element extraction.\n";
                    break;
                case BuffType.MentalState:
                    switch (Values[0])
                    {
                        case 0:
                            text = string.Format("Agressive (Full damage)\nCan't shoot over walls.\n", Values[0]);
                            break;
                        case 1:
                            text = string.Format("Trick shot (Minimal damage)\nCan shoot over walls.\n", Values[0]);
                            break;
                        case 2:
                            text = string.Format("Group Mode (Medium damage)\nDon't steal agro.\n", Values[0]);
                            break;
                    }
                    break;
                case BuffType.MagicBooster:
                    text = string.Format("Magic Booster\nIncreases MC by: {0}-{0}.\nIncreases consumption by {1}%.\n", Values[0], Values[1]);
                    break;
                case BuffType.MagicShield:
                    text = string.Format("Magic Shield\nReduces damage by {0}%.\n", (Values[0] + 2) * 10);
                    break;

                //special
                case BuffType.GameMaster:
                    GMOptions options = (GMOptions)Values[0];
                    text = "GameMaster\n";

                    if (options.HasFlag(GMOptions.GameMaster)) text += " -@GameMaster\n";
                    if (options.HasFlag(GMOptions.Superman)) text += "- @SuperMan\n";
                    if (options.HasFlag(GMOptions.Observer)) text += "- @Observer\n";
                    break;
                case BuffType.General:
                    text = string.Format("Mirian Advantage\nExpRate increased by {0}%\n", Values[0]);

                    if (Values.Length > 1)
                        text += string.Format("DropRate increased by {0}%\n", Values[1]);
                    if (Values.Length > 2)
                        text += string.Format("GoldRate increased by {0}%\n", Values[2]);
                    break;
                case BuffType.StarterExp:
                    text = string.Format("Exp Rate\nIncreased by {0}%\n", Values[0]);
                    break;
                case BuffType.Exp:
                    text = string.Format("Exp Rate\nIncreased by {0}%\n", Values[0]);
                    break;
                case BuffType.Gold:
                    text = string.Format("Gold Rate\nIncreased by {0}%\n", Values[0]);
                    break;
                case BuffType.Drop:
                    text = string.Format("Drop Rate\nIncreased by {0}%\n", Values[0]);
                    break;
                case BuffType.BagWeight:
                    text = string.Format("Bag Weight\nIncreases BagWeight by: {0}.\n", Values[0]);
                    break;
                case BuffType.Transform:
                    text = string.Format("Transform\nDisguises your appearance.\n");
                    break;
                case BuffType.RelationshipEXP:
                    text = string.Format("Love is in the Air\nExpRate increased by: {0}%.\n", Values[0]);
                    break;
                case BuffType.Mentee:
                    text = string.Format("In Training\nLearn skill points twice as quick.\nExpRate increased by: {0}%.\n", Values[0]);
                    break;
                case BuffType.Mentor:
                    text = string.Format("Mentorship Empowerment\nDamage to monsters increased by {0}%.\n", Values[0]);
                    break;
                case BuffType.Guild:
                    text = string.Format("Guild Charge\n");
                    text += GameScene.Scene.GuildDialog.ActiveStats;
                    break;
                case BuffType.Rested:
                    text = string.Format("Rested\nIncreases Exp Rate by {0}%\n", Values[0]);
                    break;

                //stats
                case BuffType.Impact:
                    text = string.Format("Impact\nIncreases DC by: 0-{0}.\n", Values[0]);
                    break;
                case BuffType.Magic:
                    text = string.Format("Magic\nIncreases MC by: 0-{0}.\n", Values[0]);
                    break;
                case BuffType.Taoist:
                    text = string.Format("Taoist\nIncreases SC by: 0-{0}.\n", Values[0]);
                    break;
                case BuffType.Storm:
                    text = string.Format("Storm\nIncreases A.Speed by: {0}.\n", Values[0]);
                    break;
                case BuffType.HealthAid:
                    text = string.Format("Health Aid\nIncreases HP by: {0}.\n", Values[0]);
                    break;
                case BuffType.ManaAid:
                    text = string.Format("Mana Aid\nIncreases MP by: {0}.\n", Values[0]);
                    break;
                case BuffType.Defence:
                    text = string.Format("Defence\nIncreases Max AC by: {0}-{0}.\n", Values[0]);
                    break;
                case BuffType.MagicDefence:
                    text = string.Format("Magic Defence\nIncreases Max MAC by: {0}-{0}.\n", Values[0]);
                    break;
                case BuffType.WonderDrug:
                    text = string.Format("Wonder Drug\n");
                    switch (Values[0])
                    {
                        case 0:
                            text += string.Format("Increases Exp Rate by {0}%\n", Values[1]);
                            break;
                        case 1:
                            text += string.Format("Increases Drop Rate by {0}%\n", Values[1]);
                            break;
                        case 2:
                            text += string.Format("Increases HP by: {0}.\n", Values[1]);
                            break;
                        case 3:
                            text += string.Format("Increases MP by: {0}.\n", Values[1]);
                            break;
                        case 4:
                            text += string.Format("Increases Max AC by: {0}-{0}.\n", Values[1]);
                            break;
                        case 5:
                            text += string.Format("Increases Max MAC by: {0}-{0}.\n", Values[1]);
                            break;
                        case 6:
                            text += string.Format("Increases A.Speed by: {0}.\n", Values[1]);
                            break;
                    }
                    break;
                case BuffType.Knapsack:
                    text = string.Format("Knapsack\nIncreases BagWeight by: {0}.\n", Values[0]);
                    break;
            }

            text += string.Format("Expire: {0}", Infinite ? "Never" : Functions.PrintTimeSpanFromSeconds(Math.Round((Expire - CMain.Time) / 1000D)));

            if (Caster.Length > 0) text += string.Format("\nCaster: {0}", Caster);

            return text;
        }

    }
}
