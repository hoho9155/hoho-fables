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
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Effect = Client.MirObjects.Effect;
using C = ClientPackets;
using S = ServerPackets;
using Font = System.Drawing.Font;

namespace Client.MirScenes.Dialogs
{
    public sealed class FieldMapDialog : MirImageControl
    {
        private MirButton CloseButton;
        private MirLabel MapNameLabel, InfoLabel, CoordsLabel, MapIconLabel;
        private MirControl mapIcon;
        private MapControl CurrentMap;
        private Font font = new Font(Settings.FontName, 9F);
        private float mapScale = 1f;
        private Point firstPoint, LastPoint, DrawPoint;
        private bool MouseIsDown = false; // Track if the mouse button is being held
        MirImageControl FieldMapImage;
        List<Settings.MinimapData> MapDataIcons;
        private MirButton[] MapButtons;
        private MirButton[] MapItems;
        string ViewingMapName;

        public static UserObject User
        {
            get { return MapObject.User; }
            set { MapObject.User = value; }
        }

        public FieldMapDialog()
        {
            if (Libraries.MiniMap == null) return;



            Index = 0;
            Library = Libraries.UI;
            Location = Center;
            Movable = true;
            MouseWheel += MapImage_MouseWheel;

            CloseButton = new MirButton
            {
                Parent = this,
                Library = Libraries.UI,
                Index = 1,
                HoverIndex = 2,
                PressedIndex = 3,
                Size = Libraries.UI.GetTrueSize(1),
                Location = new Point(this.Size.Width - 30, 16)
            };
            CloseButton.Click += (o, e) => { Hide(); };
            InfoLabel = new MirLabel
            {
                Parent = this,
                Text = "DP: " + DrawPoint.ToString(),
                Location = new Point(0, -100000),
                Font = font,
                Size = new Size(300, 20),
                ForeColour = Color.White
            };
            MapNameLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(20, 472),
                Font = font,
                Size = new Size(300, 20),
                ForeColour = Color.White
            };
            MapIconLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(0, 0),
                Font = font,
                Size = new Size(200, 20),
                ForeColour = Color.White
            };
            CoordsLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(670, 472),
                Font = font,
                Size = new Size(300, 20),
                ForeColour = Color.White,
                Visible = false
            };
            FieldMapImage = new MirImageControl
            {
                Parent = this,

                // Movable = true,
                Fixed = true,
                Index = 0,
                Library = Libraries.MiniMap,
                Size = new Size(784, 416),
                ViewRectangle = new Rectangle(0, 0, 784, 416),
                Location = new Point(8, 48)
            };
            FieldMapImage.MouseMove += MapImage_MouseMove;
            FieldMapImage.MouseDown += MapImage_MouseDown;
            FieldMapImage.MouseUp += MapImage_MouseUp;
            FieldMapImage.BeforeDraw += MapImage_DrawBefore;
            FieldMapImage.AfterDraw += (o, e) => MapImage_DrawAfter();
            FieldMapImage.MouseLeave += MapImage_MouseLeave;
        }

        private void MapImage_MouseLeave(object sender, EventArgs e)
        {
            CoordsLabel.Visible = false;
            MapImage_MouseUp(null, null);
        }
        private void MapImage_MouseWheel(object sender, MouseEventArgs e)
        {
            // TODO: Add zoom functionality
        }
        private void MapImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (FieldMapImage.Index != CurrentMap.MiniMap) return;
                UInt16 ex = (ushort)((e.X + DrawPoint.X - this.Location.X - FieldMapImage.Location.X) / 1.5f);
                UInt16 way = (ushort)(e.Y + DrawPoint.Y - this.Location.Y - FieldMapImage.Location.Y);
                Network.Enqueue(new C.Chat { Message = string.Format("@MOVE {0} {1}", ex, way) });
            }
            else
            {
                firstPoint = e.Location;
                MouseIsDown = true;
            }
        }
        private void MapImage_MouseUp(object sender, MouseEventArgs e)
        {
            LastPoint = DrawPoint;
            MouseIsDown = false;
        }
        private void MapImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (FieldMapImage.IsMouseOver(CMain.MPoint))
            {
                UInt16
                    X = (ushort)((e.X + DrawPoint.X - this.Location.X - FieldMapImage.Location.X) / 1.5f),
                    Y = (ushort)(e.Y + DrawPoint.Y - this.Location.Y - FieldMapImage.Location.Y);

                CoordsLabel.Visible = (X > (ushort)(Libraries.MiniMap.GetTrueSize(FieldMapImage.Index).Width / 1.5f) || Y > Libraries.MiniMap.GetTrueSize(FieldMapImage.Index).Height) ? false : true;
                CoordsLabel.Text = string.Format("Cursor: ({0}:{1})", X, Y);
            }
            if (MouseIsDown)
            {
                //// Get the difference between the two points
                int xDiff = (firstPoint.X - this.Location.X) - (e.Location.X);
                int yDiff = (firstPoint.Y - this.Location.Y) - (e.Location.Y);

                // Set the new point
                int x = this.Location.X + xDiff;
                int y = this.Location.Y + yDiff;

                DrawPoint = new Point(
                    x + LastPoint.X < 0 ? 0 : x + LastPoint.X > Libraries.MiniMap.GetSize(FieldMapImage.Index).Width - FieldMapImage.ViewRectangle.Width ? Libraries.MiniMap.GetSize(FieldMapImage.Index).Width - FieldMapImage.ViewRectangle.Width : x + LastPoint.X,
                    y + LastPoint.Y < 0 ? 0 : y + LastPoint.Y > Libraries.MiniMap.GetSize(FieldMapImage.Index).Height - FieldMapImage.ViewRectangle.Height ? Libraries.MiniMap.GetSize(FieldMapImage.Index).Height - FieldMapImage.ViewRectangle.Height : y + LastPoint.Y);

                DrawPoint.X = Libraries.MiniMap.GetTrueSize(FieldMapImage.Index).Width < FieldMapImage.ViewRectangle.Width ? 0 : DrawPoint.X;
                DrawPoint.Y = Libraries.MiniMap.GetTrueSize(FieldMapImage.Index).Height < FieldMapImage.ViewRectangle.Height ? 0 : DrawPoint.Y;

                if (Libraries.MiniMap.GetSize(FieldMapImage.Index).Width < FieldMapImage.ViewRectangle.Width) DrawPoint.X = 0;
                if (Libraries.MiniMap.GetSize(FieldMapImage.Index).Height < FieldMapImage.ViewRectangle.Height) DrawPoint.Y = 0;

                // Keep updating this so the map doesn't lag when dragging.
                InfoLabel.Text = "DP: " + DrawPoint.ToString() + " " + Libraries.MiniMap.GetSize(FieldMapImage.Index).ToString();
                // FieldMapImage.Redraw();

            }
        }
        private void MapImage_DrawBefore(object sender, EventArgs e)
        {
            CurrentMap = GameScene.Scene.MapControl;
            MapNameLabel.Text = (CurrentMap.MiniMap == FieldMapImage.Index) ? string.Format("{0} ({1}:{2})", CurrentMap.Title, User.CurrentLocation.X, User.CurrentLocation.Y) : ViewingMapName;

            Rectangle MapBoundary = new Rectangle(DrawPoint.X, DrawPoint.Y, FieldMapImage.ViewRectangle.Width, FieldMapImage.ViewRectangle.Height);

            FieldMapImage.ViewRectangle = new Rectangle(DrawPoint.X, DrawPoint.Y, FieldMapImage.ViewRectangle.Width, FieldMapImage.ViewRectangle.Height);
        }
        private void MapImage_DrawAfter()
        {
            Color colour;

            if (CurrentMap == null)
                return;

            for (int i = 0; i < MapButtons.Length; i++)
            {
                if (ButtonInView(new Point((ushort)(
                    MapDataIcons[i].X * 1.5f /*Scale*/) - DrawPoint.X + FieldMapImage.Location.X,
                    (ushort)(MapDataIcons[i].Y * 1) - DrawPoint.Y + FieldMapImage.Location.Y), Libraries.UI.GetTrueSize(MapButtons[i].Index)))
                {
                    MapButtons[i].Visible = true;
                    MapButtons[i].Location = new Point(
                        (ushort)(MapDataIcons[i].X * 1.5f /*Scale*/) - DrawPoint.X + FieldMapImage.Location.X - (MapButtons[i].Size.Width / 2),
                        (ushort)(MapDataIcons[i].Y * 1) - DrawPoint.Y + FieldMapImage.Location.Y - (MapButtons[i].Size.Height / 2));
                }
                else
                {
                    MapButtons[i].Visible = false;
                }

            }

            if (FieldMapImage.Index == CurrentMap.MiniMap)
            {

                for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
                {
                    MapObject ob = MapControl.Objects[i];

                    if (ob.Race == ObjectType.Item || ob.Dead || ob.Race == ObjectType.Spell) continue;
                    float x = (ob.CurrentLocation.X * 1.5f /*Scale*/) - DrawPoint.X + this.Location.X + FieldMapImage.Location.X;
                    float y = (ob.CurrentLocation.Y * 1 /*Scale*/) - DrawPoint.Y + this.Location.Y + FieldMapImage.Location.Y;

                    if (!InView(ob.CurrentLocation))
                        continue;


                    if (ob.Race == ObjectType.Merchant)
                        Libraries.UI.Draw(416, (int)(x - 3), (int)(y - 3));
                    else if (ob.AI == 6 || ob.AI == 58 || ob.AI == 57)
                        Libraries.UI.Draw(419, (int)(x - 3), (int)(y - 3));
                    else if ((GroupDialog.GroupList.Contains(ob.Name) && MapObject.User != ob))
                        Libraries.UI.Draw(415, (int)(x - 3), (int)(y - 3));
                    else if (!ob.Name.EndsWith(string.Format("({0})", MapObject.User.Name)) && ob.Name.EndsWith(")"))
                        Libraries.UI.Draw(414, new Point((int)(x - 3), (int)(y - 3)), Color.MediumPurple);
                    else if (ob.Name.EndsWith(string.Format("({0})", MapObject.User.Name)))
                        Libraries.UI.Draw(414, new Point((int)(x - 3), (int)(y - 3)), Color.Purple);
                    else if (ob.Race == ObjectType.Player)
                        Libraries.UI.Draw(413, (int)(x - 3), (int)(y - 3));
                    else if (ob.AI == 56 || (ob.AI >= 1 && ob.AI <= 3)) // Trainer, Passive, Trees
                        Libraries.UI.Draw(414, new Point((int)(x - 3), (int)(y - 3)), Color.Orange);
                    else
                        Libraries.UI.Draw(418, (int)(x - 3), (int)(y - 3));
                }
            }


            if (MapItems != null)
                foreach (var item in MapItems)
                {
                    if (item != null)
                        item.Dispose();
                }
            MapItems = new MirButton[MapControl.Objects.Count];
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject item = MapControl.Objects[i];
                if (item == null || item.Race != ObjectType.Item || item.NameColour == Color.FromArgb(255,255,255,0)) continue;


                float x = (item.MapLocation.X * 1.5f /*Scale*/) - DrawPoint.X + this.Location.X + FieldMapImage.Location.X;
                float y = (item.MapLocation.Y * 1 /*Scale*/) - DrawPoint.Y + this.Location.Y + FieldMapImage.Location.Y;

                MapItems[i] = new MirButton
                {
                    Library = Libraries.UI,
                    Index = 420,
                    HoverIndex = 420,
                    PressedIndex = 420,
                    Parent = this,
                    Size = Libraries.UI.GetTrueSize(420),
                    Location = new Point((int)(x - 3), (int)(y - 3)),
                    Hint = item.Name,
                    Tag = i.ToString(),
                    ForeColour = item.NameColour

                };
            }
            for (int i = 0; i < MapItems.Length; i++)
            {
                if (MapItems[i] == null) continue;

                MapObject item = MapControl.Objects[i];
                if (item == null) continue;

                if (item.Race != ObjectType.Item) continue;
                float x = (item.MapLocation.X * 1.5f /*Scale*/) - DrawPoint.X + this.Location.X + FieldMapImage.Location.X;
                float y = (item.MapLocation.Y * 1 /*Scale*/) - DrawPoint.Y + this.Location.Y + FieldMapImage.Location.Y;


                MapItems[i].Location = new Point(
                    (ushort)(item.MapLocation.X * 1.5f /*Scale*/) - DrawPoint.X + FieldMapImage.Location.X - (7), // -7 should be image ((width - 1) / 2)
                    (ushort)(item.MapLocation.Y * 1) - DrawPoint.Y + FieldMapImage.Location.Y - (7)); // -7 should be image ((width - 1) / 2)
            }
        }

        private bool InView(Point objectPoint)
        {
            if ((objectPoint.X * 1.5f /*Scale*/) - DrawPoint.X + this.Location.X + FieldMapImage.Location.X > this.Location.X + FieldMapImage.Location.X + FieldMapImage.ViewRectangle.Width || // Clip Left
                (objectPoint.X * 1.5f /*Scale*/) - DrawPoint.X + this.Location.X + FieldMapImage.Location.X < this.Location.X + FieldMapImage.Location.X) // Clip Right
                return false;

            if (objectPoint.Y - DrawPoint.Y + this.Location.Y + FieldMapImage.Location.Y > this.Location.Y + FieldMapImage.Location.Y + FieldMapImage.ViewRectangle.Height || // Clip Bottom
                objectPoint.Y - DrawPoint.Y + this.Location.Y + FieldMapImage.Location.Y < this.Location.Y + FieldMapImage.Location.Y) // Clip Top
                return false;

            return true;
        }
        private bool ButtonInView(Point objectPoint, Size objectSize)
        {
            if (objectPoint.X >= FieldMapImage.Location.X + FieldMapImage.ViewRectangle.Width /*- (objectSize.Width / 2)*/ || // Clip Right
                objectPoint.X <= FieldMapImage.Location.X) // Clip Left
                return false;

            if (objectPoint.Y >= FieldMapImage.Location.Y + FieldMapImage.ViewRectangle.Height /*- (objectSize.Height / 2) */|| // Clip Bottom
                objectPoint.Y <= FieldMapImage.Location.Y) // Clip Top
                return false;

            return true;
        }

        void UpdateMapDataIcons(string Map)
        {
            if (Map == null || Map == string.Empty) return;
            MapDataIcons = new List<Settings.MinimapData>();
            MapDataIcons = Settings.MiniMapDataList.FindAll(s => s.Map == Map);


            //---- New Code
            MapItems = new MirButton[255];
            MapButtons = new MirButton[MapDataIcons.Count];
            for (int i = 0; i < MapButtons.Length; i++)
            {
                MapButtons[i] = new MirButton
                {
                    Library = Libraries.UI,
                    Index = MapDataIcons[i].Index,
                    HoverIndex = MapDataIcons[i].Index,
                    PressedIndex = MapDataIcons[i].Index,
                    Parent = this,
                    Size = Libraries.UI.GetTrueSize(Index),
                    Location = new Point(0, 0),
                    Hint = MapDataIcons[i].Name,
                    Tag = i.ToString()
                };
                MapButtons[i].Click += (o, e) =>
               {
                   MirButton btn = (MirButton)o;

                   //MirMessageBox.Show(MapDataIcons[Convert.ToUInt16(btn.Tag)].Name);
                   //MirMessageBox.Show(btn.Location.ToString());
                   //ChangeMap(Convert.ToUInt16(btn.Tag)].)
                   ChangeMap(MapDataIcons[Convert.ToUInt16(btn.Tag)].LinkMap);
               };
            }
        }

        private void ChangeMap(string Map)
        {
            if (!Map.StartsWith(".")) return;
            if (MapButtons != null)
                foreach (var item in MapButtons)
                {
                    item.Dispose();
                }
            if (MapItems != null)
                foreach (var item in MapItems)
                {
                    if (item != null)
                        item.Dispose();
                }


            UpdateMapDataIcons(Map);
            FieldMapImage.Index = Settings.MapDataList.Find(s => s.Map == Map).MinimapNumber;
            ViewingMapName = Settings.MapDataList.Find(s => s.Map == Map).Name;

            DrawPoint = new Point(0, 0);
            firstPoint = new Point(0, 0);
            LastPoint = new Point(0, 0);
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
            CurrentMap = GameScene.Scene.MapControl;
            ChangeMap(CurrentMap.FileName);

            Visible = true;
        }


    }


}