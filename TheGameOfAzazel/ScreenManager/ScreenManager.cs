using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using MonoGame.Extended.BitmapFonts;

namespace TheGameOfAzazel
{
    public class ScreenManager : DrawableGameComponent
    {
        private readonly List<GameScreen> _screens = new List<GameScreen>();
        private readonly IGraphicsDeviceService _graphicsDeviceService;
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private BitmapFont _font;
        public new Game Game => base.Game;

        public new GraphicsDevice GraphicsDevice => base.GraphicsDevice;

        public SpriteBatch SpriteBatch => _spriteBatch;

        public BitmapFont Font => _font;

        public GraphicsDeviceManager GraphicsManager => _graphicsDeviceManager;

        public ScreenManager(Game game, GraphicsDeviceManager graphicsDeviceManager) : base(game)
        {
            _graphicsDeviceManager = graphicsDeviceManager;
            _graphicsDeviceService = (IGraphicsDeviceService)game.Services.GetService(typeof(IGraphicsDeviceService));

            if (_graphicsDeviceService == null)
            {
                throw new InvalidOperationException("No graphics device service.");
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _font = Game1.Instance.Content.Load<BitmapFont>("Fonts/SourceSansPro-SemiBold");

            foreach (GameScreen screen in _screens)
            {
                screen.LoadContent();
            }
        }
        protected override void UnloadContent()
        {
            foreach (GameScreen screen in _screens)
            {
                screen.UnloadContent();
            }
        }
        public override void Update(GameTime gameTime)
        {

            for (int i = _screens.Count - 1; i >= 0; i--)
            {
                GameScreen screen = _screens[i];
                screen.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in _screens)
            {
                screen.Draw(gameTime);

                // Fade
                if (screen.IsFading)
                {
                    _spriteBatch.Begin();
                    if (screen.FadingInOrOut == false) // In
                    {
                        _spriteBatch.FillRectangle(GraphicsDevice.Viewport.Bounds, new Color(screen.FadeColor, MathF.Min((screen.TotalFadeTime - screen.FadingTimeLeft), screen.TotalFadeTime) / screen.TotalFadeTime));
                    }
                    if (screen.FadingInOrOut == true) // Out
                    {

                        _spriteBatch.FillRectangle(GraphicsDevice.Viewport.Bounds, new Color(screen.FadeColor, MathF.Max(screen.FadingTimeLeft, 0.0f) / screen.TotalFadeTime));
                    }
                    _spriteBatch.End();

                }
            }
        }

        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;
            screen.Initialize();
            _screens.Add(screen);

            if ((_graphicsDeviceService != null) && (_graphicsDeviceService.GraphicsDevice != null))
            {
                screen.LoadContent();
            }
        }
        public void RemoveScreen(GameScreen screen)
        {
            _screens.Remove(screen);

            if ((_graphicsDeviceService != null) && (_graphicsDeviceService.GraphicsDevice != null))
            {
                screen.UnloadContent();
            }
        }
        public GameScreen[] GetScreens()
        {
            return _screens.ToArray();
        }
    }
}
