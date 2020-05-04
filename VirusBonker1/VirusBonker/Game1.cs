using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using tainicom.Aether.Physics2D.Samples;
using tainicom.Aether.Physics2D.Samples.DemosTEST;
using tainicom.Aether.Physics2D.Samples.ScreenSystem;

namespace VirusBonker
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.Reach;
            _graphics.PreparingDeviceSettings += _graphics_PreparingDeviceSettings;
            _graphics.PreferMultiSampling = true;

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            IsFixedTimeStep = true;

            _graphics.IsFullScreen = false;

            Content.RootDirectory = "Content";

            //new-up components and add to Game.Components
            ScreenManager = new ScreenManager(this);//дерево на GameComponents(аналог SpriteManager из игры с Doge)
            Components.Add(ScreenManager);

            FrameRateCounter frameRateCounter = new FrameRateCounter(ScreenManager);
            frameRateCounter.DrawOrder = 101;
            Components.Add(frameRateCounter);
        }

        void _graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            // unlock the 30 fps limit. 60fps (if possible)
            e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.One;

            // set HiDef Profile if supported
            if (e.GraphicsDeviceInformation.Adapter.IsProfileSupported(GraphicsProfile.HiDef))
                e.GraphicsDeviceInformation.GraphicsProfile = GraphicsProfile.HiDef;
        }

        public ScreenManager ScreenManager { get; set; }

        protected override void Initialize()
        {
            base.Initialize();

            GameLevel simpleTest = new GameLevel();
            //AdvancedDemo1 advancedDemo1 = new AdvancedDemo1();
            //AdvancedDemo2 advancedDemo2 = new AdvancedDemo2();
            //AdvancedDemo3 advancedDemo3 = new AdvancedDemo3();
            //AdvancedDemo4 advancedDemo4 = new AdvancedDemo4();
            //AdvancedDemo5 advancedDemo5 = new AdvancedDemo5();


            MenuScreen menuScreen = new MenuScreen("Samples");

            menuScreen.AddMenuItem(simpleTest.GetTitle(), EntryType.Screen, simpleTest);
            //menuScreen.AddMenuItem(advancedDemo1.GetTitle(), EntryType.Screen, advancedDemo1);
            //menuScreen.AddMenuItem(advancedDemo2.GetTitle(), EntryType.Screen, advancedDemo2);
            //menuScreen.AddMenuItem(advancedDemo3.GetTitle(), EntryType.Screen, advancedDemo3);
            //menuScreen.AddMenuItem(advancedDemo4.GetTitle(), EntryType.Screen, advancedDemo4);
            //menuScreen.AddMenuItem(advancedDemo5.GetTitle(), EntryType.Screen, advancedDemo5);

            menuScreen.AddMenuItem("", EntryType.Separator, null);
            menuScreen.AddMenuItem("Exit", EntryType.ExitItem, null);

            ScreenManager.AddScreen(new BackgroundScreen());
            ScreenManager.AddScreen(menuScreen);
            ScreenManager.AddScreen(new LogoScreen(TimeSpan.FromSeconds(3.0)));
        }
    }
}
