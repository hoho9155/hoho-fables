using Server.MirDatabase;
using System;
using System.Collections.Generic;

namespace Server.MirObjects.Monsters
{
    public class TreasureGoblin : MonsterObject
    {
        private bool _runAway;
        private UInt16 MaximumGoldDrop = 0;
        private UInt16 DroppedGoldCount = 0;

        Random random = new Random();

        List<DropInfo> treasureDrop;
        byte[] indexDropped;

        protected override bool CanAttack
        {
            get
            {
                return base.CanAttack && !_runAway;
            }
        }

        protected internal TreasureGoblin(MonsterInfo info) : base(info)
        {
            if (Info.AI != 255) return;

            _runAway = true;
            MaximumGoldDrop = (ushort)((info.Level + 1) * random.Next(1, 101));
            //treasureDrop = new List<DropInfo>();
            treasureDrop = info.TreasureDiscardDrops;
            indexDropped = new byte[info.TreasureDiscardDrops.Count];
                
        }

        protected override void FindTarget()
        {
            if (_runAway)
                base.FindTarget();
        }

        private void GoldDrop()
        {
            if (!FindNearby(7)) return;
            if (DroppedGoldCount >= MaximumGoldDrop) return; // If all gold has been dropped return.

            double percentageDrop = ((MaximumGoldDrop / 100) * random.Next(1, 10)) < 1 ? random.Next(1, 15) : ((MaximumGoldDrop / 100) * random.Next(1, 4));

            UInt16 _ran = (byte)random.Next(1, (ushort)Math.Round(percentageDrop, MidpointRounding.ToEven)); // Amount of gold to drop.
            if (_ran > MaximumGoldDrop - DroppedGoldCount) _ran = (UInt16)(MaximumGoldDrop - DroppedGoldCount); // Make sure the mob doesn't drop more gold than it's allowed to.
            DroppedGoldCount += _ran; // Add dropped gold to counter.

            ItemObject ob = new ItemObject(this, _ran)
            {
                Owner = null,
                OwnerTime = 0,
            };

            ob.Drop(Settings.DropRange);

            TreasureDrop(); // Once all gold has been dropped it wont throw out items
        }

        public override void MonsterHit()
        {
            byte _ran = (byte)random.Next(1, 9);
            for (int i = 0; i < _ran; i++)
            {
                GoldDrop();
            }
            //TreasureDrop();
        }

        private void TreasureDrop()
        {
            for (int i = 0; i < treasureDrop.Count; i++)
            {
                if (indexDropped[i] == 1) return;
                DropInfo drop = treasureDrop[i];

                int rate = (int)(drop.Chance / (Settings.DropRate));

                if (EXPOwner != null && EXPOwner.ItemDropRateOffset > 0)
                    rate -= (int)(rate * (EXPOwner.ItemDropRateOffset / 100));

                if (rate < 1) rate = 1;

                if (Envir.Random.Next(rate) != 0) continue;

                if (drop.Gold > 0)
                {
                    int lowerGoldRange = (int)(drop.Gold / 2);
                    int upperGoldRange = (int)(drop.Gold + drop.Gold / 2);

                    if (EXPOwner != null && EXPOwner.GoldDropRateOffset > 0)
                        lowerGoldRange += (int)(lowerGoldRange * (EXPOwner.GoldDropRateOffset / 100));

                    if (lowerGoldRange > upperGoldRange) lowerGoldRange = upperGoldRange;

                    int gold = Envir.Random.Next(lowerGoldRange, upperGoldRange);

                    if (gold <= 0) continue;

                    if (!DropGold((uint)gold)) return;
                }
                else
                {
                    UserItem item = Envir.CreateDropItem(drop.Item);

                    if (item == null) continue;

                    if (EXPOwner != null && EXPOwner.Race == ObjectType.Player)
                    {
                        PlayerObject ob = (PlayerObject)EXPOwner;

                        if (ob.CheckGroupQuestItem(item))
                        {
                            continue;
                        }
                    }

                    if (drop.QuestRequired) continue;
                    if (!TreasureDropItem(item))
                    {
                        return;
                    }
                    else
                    {
                        indexDropped[i] = 1;
                       // treasureDrop.RemoveAt(i);
                    }
                }
            }
        }

        private bool TreasureDropItem(UserItem item)
        {
            if (CurrentMap.Info.NoDropMonster)
                return false;

            ItemObject ob = new ItemObject(this, item)
            {
                Owner = EXPOwner,
                OwnerTime = Envir.Time + Settings.Minute,
            };

            if (!item.Info.GlobalDropNotify)
                return ob.Drop(Settings.DropRange);

            foreach (var player in Envir.Players)
            {
                player.ReceiveChat($"{Name} has dropped {item.FriendlyName}.", ChatType.System2);
            }

            return ob.Drop(Settings.DropRange);
        }

        protected override void ProcessTarget()
        {
            if (random.Next(0, 5) == 0) // Random chance to get gold drop.
                GoldDrop();

            if (_runAway)
            {
                if (!CanMove || Target == null) return;

                MirDirection dir = Functions.DirectionFromPoint(Target.CurrentLocation, CurrentLocation);

                if (Walk(dir)) return;

                switch (Envir.Random.Next(2)) //No favour
                {
                    case 0:
                        for (int i = 0; i < 7; i++)
                        {
                            dir = Functions.NextDir(dir);

                            if (Walk(dir))
                                return;
                        }
                        break;
                    default:
                        for (int i = 0; i < 7; i++)
                        {
                            dir = Functions.PreviousDir(dir);

                            if (Walk(dir))
                                return;
                        }
                        break;
                }
            }
            else base.ProcessTarget();
        }
        
    }
}
