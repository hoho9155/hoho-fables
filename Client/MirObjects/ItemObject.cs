using Client.MirGraphics;
using Client.MirScenes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using S = ServerPackets;

namespace Client.MirObjects
{
    internal class ItemObject : MapObject
    {
        public override ObjectType Race
        {
            get { return ObjectType.Item; }
        }

        public override bool Blocking
        {
            get { return false; }
        }

        public Size Size;

        public ItemObject(uint objectID) : base(objectID)
        {
        }

        public void Load(S.ObjectItem info)
        {
            Name = info.Name;
            NameColour = info.NameColour;

            BodyLibrary = Libraries.FloorItems;

            CurrentLocation = info.Location;
            MapLocation = info.Location;
            GameScene.Scene.MapControl.AddObject(this);
            DrawFrame = info.Image;

            Size = BodyLibrary.GetTrueSize(DrawFrame);

            DrawY = CurrentLocation.Y;

            // Paul
            if (NameColour == Color.FromArgb(255, 0, 255, 255)) // Cyan (Added)
            {
                LightColour = Color.FromArgb(255, 0, 255, 255);
                Light = 1;
                Effects.Add(new Effect(Libraries.Prguse, 410, 9, 500, (MapObject)this) { Repeat = true, Blend = true, Rate = 0.5f, Start = 100, LightColour = Color.FromArgb(255, 0, 255, 255), Light = 2, Delay = 1500 }); // <-- Add effect frames
                Effects.Add(new Effect(Libraries.Prguse4, 94, 10, 500, (MapObject)this) { Repeat = true, Blend = true, Rate = 0.5f, Start = 100, LightColour = Color.FromArgb(255, 0, 255, 255), Light = 1 }); // <-- Add effect frames
            }
            else if (NameColour == Color.FromArgb(255, 221, 160, 221)) // Violet (Mythical)
            {
                LightColour = Color.Purple;
                Light = 1;
                Effects.Add(new Effect(Libraries.Prguse4, 83, 10, 500, (MapObject)this) { Repeat = true, Blend = true, Rate = 0.7f, Start = 100, LightColour = Color.Purple, Light = 5, Delay = 500 }); // <-- Add effect frames
                Effects.Add(new Effect(Libraries.Prguse4, 134, 10, 500, (MapObject)this) { Repeat = true, Blend = true, Rate = 0.5f, Start = 100, LightColour = Color.Purple, Light = 1 }); // <-- Add effect frames
            }
            else if (NameColour == Color.FromArgb(255, 255, 140, 0)) // Orange (Legendary)
            {
                LightColour = Color.FromArgb(255, 255, 140, 0);
                Light = 1;
                Effects.Add(new Effect(Libraries.Prguse4, 83, 10, 500, (MapObject)this) { Repeat = true, Blend = true, Rate = 0.7f, Start = 100, LightColour = Color.FromArgb(255, 255, 140, 0), Light = 4, Delay = 500 }); // <-- Add effect frames
                Effects.Add(new Effect(Libraries.Prguse4, 114, 10, 500, (MapObject)this) { Repeat = true, Blend = true, Rate = 0.5f, Start = 100, LightColour = Color.FromArgb(255, 255, 140, 0), Light = 1 }); // <-- Add effect frames
            }
            else if (NameColour == Color.FromArgb(255, 50, 205, 50)) // Green (Superior)
            {
                LightColour = Color.FromArgb(255, 50, 205, 50);
                Light = 1;
                Effects.Add(new Effect(Libraries.Prguse4, 38, 7, 1200, (MapObject)this) { Repeat = true, Blend = true, Rate = 0.5f, Start = 100, LightColour = Color.FromArgb(255, 50, 205, 50), Light = 3 }); // <-- Add effect frames
                Effects.Add(new Effect(Libraries.Prguse4, 154, 10, 500, (MapObject)this) { Repeat = true, Blend = true, Rate = 0.5f, Start = 100, LightColour = Color.FromArgb(255, 50, 205, 50), Light = 1 }); // <-- Add effect frames
            }
            else if (NameColour == Color.FromArgb(255, 0, 191, 255)) // Blue (Rare)
            {
                LightColour = Color.Blue;
                Light = 1;
                Effects.Add(new Effect(Libraries.Prguse4, 75, 7, 500, (MapObject)this) { Repeat = true, Blend = true, Rate = 0.5f, Start = 100, LightColour = Color.Blue, Light = 2 }); // <-- Add effect frames
                Effects.Add(new Effect(Libraries.Prguse4, 144, 10, 500, (MapObject)this) { Repeat = true, Blend = true, Rate = 0.5f, Start = 100, LightColour = Color.FromArgb(255, 50, 205, 50), Light = 1 }); // <-- Add effect frames
            }
        }

        public void Load(S.ObjectGold info)
        {
            Name = string.Format("Gold ({0:###,###,###})", info.Gold);

            BodyLibrary = Libraries.FloorItems;

            CurrentLocation = info.Location;
            MapLocation = info.Location;
            GameScene.Scene.MapControl.AddObject(this);

            LightColour = Color.DarkGoldenrod;
            Light = 1;
            if (info.Gold < 100)
                DrawFrame = 112;
            else if (info.Gold < 200)
                DrawFrame = 113;
            else if (info.Gold < 500)
                DrawFrame = 114;
            else if (info.Gold < 1000)
                DrawFrame = 115;
            else
                DrawFrame = 116;

            Size = BodyLibrary.GetTrueSize(DrawFrame);

            DrawY = CurrentLocation.Y;
        }

        public override void Draw()
        {
            DrawBehindEffects(true);
            if (BodyLibrary != null)
                BodyLibrary.Draw(DrawFrame, DrawLocation, DrawColour);
        }

        public override void Process()
        {
            for (int i = 0; i < Effects.Count; i++)
                Effects[i].Process();

            DrawLocation = new Point((CurrentLocation.X - User.Movement.X + MapControl.OffSetX) * MapControl.CellWidth, (CurrentLocation.Y - User.Movement.Y + MapControl.OffSetY) * MapControl.CellHeight);
            DrawLocation.Offset((MapControl.CellWidth - Size.Width) / 2, (MapControl.CellHeight - Size.Height) / 2);
            DrawLocation.Offset(User.OffSetMove);
            DrawLocation.Offset(GlobalDisplayLocationOffset);
            FinalDrawLocation = DrawLocation;

            DisplayRectangle = new Rectangle(DrawLocation, Size);            
        }

        public override bool MouseOver(Point p)
        {
            return MapControl.MapLocation == CurrentLocation;
            // return DisplayRectangle.Contains(p);
        }

        public override void DrawName()
        {
            CreateLabel(Color.Transparent, false, true);

            if (NameLabel == null) return;
            NameLabel.Location = new Point(
                DisplayRectangle.X + (DisplayRectangle.Width - NameLabel.Size.Width) / 2,
                DisplayRectangle.Y + (DisplayRectangle.Height - NameLabel.Size.Height) / 2 - 20);
            NameLabel.Draw();
        }

        public override void DrawBehindEffects(bool effectsEnabled)
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                Effects[i].Draw();
            }
        }

        public override void DrawEffects(bool effectsEnabled)
        {
        }

        public void DrawName(int y)
        {
            CreateLabel(Color.FromArgb(100, 0, 24, 48), true, false);

            NameLabel.Location = new Point(
                DisplayRectangle.X + (DisplayRectangle.Width - NameLabel.Size.Width) / 2,
                DisplayRectangle.Y + y + (DisplayRectangle.Height - NameLabel.Size.Height) / 2 - 20);
            NameLabel.Draw();
        }

        private void CreateLabel(Color backColour, bool border, bool outline)
        {
            NameLabel = null;

            for (int i = 0; i < LabelList.Count; i++)
            {
                if (LabelList[i].Text != Name || LabelList[i].Border != border || LabelList[i].BackColour != backColour || LabelList[i].ForeColour != NameColour || LabelList[i].OutLine != outline) continue;
                NameLabel = LabelList[i];
                break;
            }
            if (NameLabel != null && !NameLabel.IsDisposed) return;

            NameLabel = new MirControls.MirLabel
            {
                AutoSize = true,
                BorderColour = Color.Black,
                BackColour = backColour,
                ForeColour = NameColour,
                OutLine = outline,
                Border = border,
                Text = Regex.Replace(Name, @"\d+$", string.Empty),
            };

            LabelList.Add(NameLabel);
        }
    }
}