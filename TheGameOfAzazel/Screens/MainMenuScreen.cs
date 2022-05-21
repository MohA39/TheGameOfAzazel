using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using MonoGame.Extended.BitmapFonts;

namespace TheGameOfAzazel
{
    internal class MainMenuScreen : GameScreen
    {
        protected List<(string, Rectangle)> _menuEntries = new List<(string, Rectangle)>(); // Name, positions
        private bool _PositionsCalculated = false;
        private bool _Selected = false;
        private Point _LastMousePosition;
        private int _selectedEntry = 0;
        private const float _MaxMovementCooldown = 0.5f;
        private float _MenuMovementCooldown = _MaxMovementCooldown;

        public MainMenuScreen()
        {
            _menuEntries.Add(("Play", Rectangle.Empty));
            _menuEntries.Add(("Options (Not implemented)", Rectangle.Empty));
            _menuEntries.Add(("Exit", Rectangle.Empty));
        }

        public override void Initialize()
        {
            MediaPlayer.Play(Game1.Instance.Content.Load<Song>("Audio/MenuMusic1"));
            MediaPlayer.IsRepeating = true;
            base.Initialize();
        }


        protected void OnSelectEntry(int entryIndex)
        {
            if (_Selected)
            {
                return;
            }
            switch (entryIndex)
            {
                case 0:
                    _Selected = true;
                    Fade(false, _TotalFadeTime, Color.Black, () =>
                    {
                        GameplayScreen GS = new GameplayScreen();
                        _screenManager.AddScreen(GS);
                        GS.Fade(true, 5000);
                    });

                    break;

                case 1:

                    break;

                case 2:
                    _screenManager.Game.Exit();
                    break;

            }

        }


        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(new Color(170, 39, 24));
            ScreenManager.SpriteBatch.Begin();

            for (int i = 0; i < _menuEntries.Count; i++)
            {
                Color color;

                if (i == _selectedEntry)
                {
                    color = Color.Yellow;
                }
                else
                {
                    color = Color.White;
                }

                Vector2 origin = new Vector2(0, ScreenManager.Font.LineHeight / 2);
                ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, _menuEntries[i].Item1, _menuEntries[i].Item2.Location.ToVector2(), color, 0, origin, 0.4f, SpriteEffects.None, 0);

            }

            float Scale = 0.35f;
            string Credit = "Game by Mohab Abdalfattah";

            Vector2 CreditSize = ScreenManager.Font.MeasureString(Credit) * Scale;
            Rectangle screenRectangle = ScreenManager.GraphicsDevice.Viewport.Bounds;

            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, Credit, new Vector2(screenRectangle.Width - CreditSize.X, screenRectangle.Height - CreditSize.Y), Color.Black, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if (!_PositionsCalculated)
            {
                CalculateTextRectangle(ref _menuEntries);
                _PositionsCalculated = true;
            }

            if (mouseState.Position != _LastMousePosition)
            {

                for (int i = 0; i < _menuEntries.Count; i++)
                {

                    if (_menuEntries[i].Item2.Contains(mouseState.Position))
                    {
                        _selectedEntry = i;
                    }
                }
            }
            if (keyboardState.IsKeyDown(Keys.Up) && _MenuMovementCooldown <= 0)
            {
                _selectedEntry--;
                if (_selectedEntry < 0)
                {
                    _selectedEntry = _menuEntries.Count - 1;
                }
                _MenuMovementCooldown = _MaxMovementCooldown;
            }

            if (keyboardState.IsKeyDown(Keys.Down) && _MenuMovementCooldown <= 0)
            {
                _selectedEntry++;

                if (_selectedEntry >= _menuEntries.Count)
                {
                    _selectedEntry = 0;
                }

                _MenuMovementCooldown = _MaxMovementCooldown;
            }

            if (keyboardState.IsKeyDown(Keys.Enter) || mouseState.LeftButton == ButtonState.Pressed)
            {
                OnSelectEntry(_selectedEntry);
            }

            _MenuMovementCooldown -= deltaTime;

            _LastMousePosition = mouseState.Position;
            base.Update(gameTime);
        }

        private void CalculateTextRectangle(ref List<(string, Rectangle)> MenuEntries)
        {
            Vector2 position = new Vector2(100, 150);
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                Vector2 TextSize = ScreenManager.Font.MeasureString(MenuEntries[i].Item1);
                MenuEntries[i] = (MenuEntries[i].Item1, new Rectangle(position.ToPoint(), TextSize.ToPoint()));
                position.Y += ScreenManager.Font.LineHeight / 2;
            }
        }

    }
}
