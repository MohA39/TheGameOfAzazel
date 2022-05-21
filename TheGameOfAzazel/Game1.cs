using Microsoft.Xna.Framework;

namespace TheGameOfAzazel
{
    public class Game1 : Game
    {
        public static Game1 Instance;
        private readonly GraphicsDeviceManager _graphics;
        private ScreenManager _screenManager;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            Instance = this;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            /*
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = true;*/

            _graphics.ApplyChanges();

            LightManager.Initalize(this);
            _screenManager = new ScreenManager(this, _graphics);

            _screenManager.Initialize();
            _screenManager.AddScreen(new MainMenuScreen());
            SaveManager.Initalize();
            Upgrades.Initalize();

            Components.Add(_screenManager);
            base.Initialize();
        }

        protected override void LoadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            _screenManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!GameplayScreen.DrawingStarted)
            {
                return;
            }
            base.Draw(gameTime);
        }
    }
}
