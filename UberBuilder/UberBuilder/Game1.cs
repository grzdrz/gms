using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using tainicom.Aether.Physics2D.Samples;
using tainicom.Aether.Physics2D.Samples.DemosTEST;
using tainicom.Aether.Physics2D.Samples.ScreenSystem;
using UberBuilder.GameSystem;

namespace UberBuilder
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
            GameLevel1 gameLevel1 = new GameLevel1();
            //SimpleDemo1 simpleDemo1 = new SimpleDemo1();
            //SimpleDemo2 simpleDemo2 = new SimpleDemo2();
            //SimpleDemo3 simpleDemo3 = new SimpleDemo3();
            //SimpleDemo4 simpleDemo4 = new SimpleDemo4();
            //SimpleDemo5 simpleDemo5 = new SimpleDemo5();
            //SimpleDemo6 simpleDemo6 = new SimpleDemo6();
            //SimpleDemo7 simpleDemo7 = new SimpleDemo7();
            //SimpleDemo8 simpleDemo8 = new SimpleDemo8();
            //SimpleDemo9 simpleDemo9 = new SimpleDemo9();
            //SimpleDemo10 simpleDemo10 = new SimpleDemo10();
            //AdvancedDemo1 advancedDemo1 = new AdvancedDemo1();
            //AdvancedDemo2 advancedDemo2 = new AdvancedDemo2();
            //AdvancedDemo3 advancedDemo3 = new AdvancedDemo3();
            //AdvancedDemo4 advancedDemo4 = new AdvancedDemo4();
            //AdvancedDemo5 advancedDemo5 = new AdvancedDemo5();


            MenuScreen menuScreen = new MenuScreen("Samples");

            menuScreen.AddMenuItem(simpleTest.GetTitle(), EntryType.Screen, simpleTest);
            menuScreen.AddMenuItem(gameLevel1.GetTitle(), EntryType.Screen, gameLevel1);
            //menuScreen.AddMenuItem(simpleDemo1.GetTitle(), EntryType.Screen, simpleDemo1);
            //menuScreen.AddMenuItem(simpleDemo2.GetTitle(), EntryType.Screen, simpleDemo2);
            //menuScreen.AddMenuItem(simpleDemo3.GetTitle(), EntryType.Screen, simpleDemo3);
            //menuScreen.AddMenuItem(simpleDemo4.GetTitle(), EntryType.Screen, simpleDemo4);
            //menuScreen.AddMenuItem(simpleDemo5.GetTitle(), EntryType.Screen, simpleDemo5);
            //menuScreen.AddMenuItem(simpleDemo6.GetTitle(), EntryType.Screen, simpleDemo6);
            //menuScreen.AddMenuItem(simpleDemo7.GetTitle(), EntryType.Screen, simpleDemo7);
            //menuScreen.AddMenuItem(simpleDemo8.GetTitle(), EntryType.Screen, simpleDemo8);
            //menuScreen.AddMenuItem(simpleDemo9.GetTitle(), EntryType.Screen, simpleDemo9);
            //menuScreen.AddMenuItem(simpleDemo10.GetTitle(), EntryType.Screen, simpleDemo10);
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
