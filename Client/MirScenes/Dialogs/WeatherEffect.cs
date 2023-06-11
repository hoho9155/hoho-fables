using Client.MirControls;
using Client.MirGraphics;
using Client.MirSounds;
using System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.MirScenes.Dialogs
{
    public sealed class WeatherEffect : MirImageControl
    {
        MirImageControl[] Mist;
        MirAnimatedControl Snow;
        long NextMoveTime;
        bool Enabled = false;
        public WeatherType weatherType = WeatherType.None;
        UInt16
            SnowStartIndex = 319,
            SnowEndIndex = 378;


        public WeatherEffect()
        {
            Size = new Size(1920, 1080);
            Location = new Point(0, 0);
            NotControl = true;

            NextMoveTime = CMain.Time + 20;

            Mist = new MirImageControl[2];
            Mist[0] = new MirImageControl
            {
                Parent = this,
                Library = Libraries.UI,
                Index = 318,
                Location = new Point(0, 0),
                NotControl = true,
                Visible = false,
                Opacity = 0f
            };
            Mist[1] = new MirImageControl
            {
                Parent = this,
                Library = Libraries.UI,
                Index = 318,
                Location = new Point(1920, 0),
                NotControl = true,
                Visible = false,
                Opacity = 0f
            };

            Snow = new MirAnimatedControl
            {
                Parent = this,
                Library = Libraries.UI,
                Index = SnowStartIndex,
                AnimationCount = 60,                
                Location = new Point(0, 0),
                Animated = true,
                Loop  =true,
                AnimationDelay = 100,
                NotControl = true,
                Visible = false,
                Opacity = 0f
            };
        }
        public void Process()
        {
            if (CMain.Time < NextMoveTime) return;
            if (GameScene.Scene.MapControl.MapDarkLight == 5) weatherType = WeatherType.None;
            //if (Enabled == false) return;
            switch (weatherType)
            {
                case WeatherType.None:
                    break;
                case WeatherType.Mist:
                    NextMoveTime = CMain.Time + 20;
                    if (!Mist[0].Visible) Mist[0].Visible = Mist[1].Visible = true;

                    if (Mist[0].Opacity < 0.5f) Mist[0].Opacity = Mist[1].Opacity = Mist[0].Opacity + 0.001f;
                    if (Mist[0].Location.X == -1920)
                    {
                        Mist[0].Location = new Point(0, 0);
                        Mist[1].Location = new Point(1920, 0);
                    }
                    Mist[0].Location = new Point(Mist[0].Location.X - 1, 0);
                    Mist[1].Location = new Point(Mist[1].Location.X - 1, 0);
                    break;
                case WeatherType.Fog:
                    break;
                case WeatherType.Rain:
                    break;
                case WeatherType.Snow:
                    //NextMoveTime = CMain.Time + 80;
                                        
                    if (!Snow.Visible) Snow.Visible = true;
                    if (Snow.Opacity != 0.3f) Snow.Opacity = 0.3f;
                    //if (Snow.Opacity < 0.5f) Snow.Opacity += 0.001f;
                    break;
                case WeatherType.SandStorm:
                    break;
                default:
                    break;
            }

            // Fade Out Code
            if (weatherType != WeatherType.Mist && Mist[0].Opacity > 0f) Mist[0].Opacity = Mist[1].Opacity = Mist[0].Opacity - 0.05f;
            if (weatherType != WeatherType.Mist && Mist[0].Opacity == 0f && Mist[0].Visible != false) Mist[0].Visible = Mist[1].Visible = false;

            if (weatherType != WeatherType.Snow && Snow.Opacity > 0f) Snow.Opacity -= 0.05f;
            if (weatherType != WeatherType.Snow && Snow.Opacity == 0f && Snow.Visible != false) Snow.Visible = false;
        }
        public void Toggle()
        {
            Mist[0].Visible = Mist[1].Visible = !Enabled;
            Enabled = !Enabled;
        }
    }
}