using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;

namespace TheGameOfAzazel
{
    public class ShopScreen : GameScreen

    {
        // Order of items
        // Health, Shield, Light, Speed, fire rate, fire power, luck, start allies, coin magnet

        private class ShopItem
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int InitialCost { get; set; }
            public float CostMultipler { get; set; }
            public Rectangle ItemRectangle { get; set; }
            public Texture2D ItemTexture { get; set; }
            public Upgrade upgrade { get; set; }

            public static int GetCurrentPrice(ShopItem item)
            {
                return (int)Math.Round((item.InitialCost * Math.Pow(item.CostMultipler, item.upgrade.Level - 1)) / 100d, 0, MidpointRounding.AwayFromZero) * 100;
            }
        }
        private BitmapFont _font;
        private SpriteBatch _spriteBatch;
        private readonly List<ShopItem> _ShopItems = new List<ShopItem>();
        private Rectangle _ScreenRectangle;
        private RectangleF _ShopRectangle;
        private RectangleF _ShopItemArea;
        private RectangleF _DescriptionArea;
        private List<Rectangle> _ItemRectangles = new List<Rectangle>();
        private bool _BuyClickReleased = true;
        private int _SelectedItemIndex = -1;
        private SoundEffect _KaChing;
        private Song _MenuMusic;
        private readonly string _PlayText = "PLAY";
        private Size2 _PlayTextSize = new Size2();
        private readonly float _PlayTextScale = 0.6f;
        private Rectangle _PlayRectangle = new Rectangle();
        private bool _PlayHovered = false;
        public ShopScreen()
        {


        }
        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);
            _ScreenRectangle = ScreenManager.GraphicsDevice.Viewport.Bounds;
            _ShopRectangle = _ScreenRectangle.ToRectangleF();
            _ShopRectangle.Inflate(_ShopRectangle.Width * -0.1f, _ShopRectangle.Height * -0.1f);

            List<Rectangle> _ShopRectangleSplits = Helpers.SplitRectangle(_ShopRectangle.ToRectangle(), 1, 2, 0);
            _ShopItemArea = _ShopRectangleSplits[0];

            int Margin = 8;
            _DescriptionArea = _ShopRectangleSplits[1];
            _DescriptionArea.Inflate(-Margin, -Margin);
            _ItemRectangles = Helpers.SplitRectangle(_ShopItemArea.ToRectangle(), 5, 2, Margin);




            base.Initialize();
        }
        public override void LoadContent()
        {
            _font = Game1.Instance.Content.Load<BitmapFont>("Fonts/SourceSansPro-Bold");
            _PlayTextSize = _font.MeasureString(_PlayText) * _PlayTextScale;
            _KaChing = Game1.Instance.Content.Load<SoundEffect>("Audio/Ka-Ching");
            _MenuMusic = Game1.Instance.Content.Load<Song>("Audio/ShopMusic");
            MediaPlayer.Play(_MenuMusic);
            MediaPlayer.IsRepeating = true;

            float TopMargin = 5f;
            _PlayRectangle = new Rectangle((int)(_ShopRectangle.Right - _PlayTextSize.Width), (int)(_ShopRectangle.Bottom + TopMargin), (int)_PlayTextSize.Width, (int)_PlayTextSize.Height);
            _ShopItems.Add(new ShopItem()
            {
                Name = "Health",
                Description = "Increases health.",
                InitialCost = 350,
                CostMultipler = 1.25f,
                ItemRectangle = _ItemRectangles[_ShopItems.Count],
                ItemTexture = Game1.Instance.Content.Load<Texture2D>("ShopItems/Health"),
                upgrade = Upgrades.Health
            });

            _ShopItems.Add(new ShopItem()
            {
                Name = "Shield",
                Description = "Decreases damage from enemies.",
                InitialCost = 350,
                CostMultipler = 1.25f,
                ItemRectangle = _ItemRectangles[_ShopItems.Count],
                ItemTexture = Game1.Instance.Content.Load<Texture2D>("ShopItems/Shield"),
                upgrade = Upgrades.Shield
            });

            _ShopItems.Add(new ShopItem()
            {
                Name = "Light",
                Description = "See for further distances.",
                InitialCost = 350,
                CostMultipler = 1.25f,
                ItemRectangle = _ItemRectangles[_ShopItems.Count],
                ItemTexture = Game1.Instance.Content.Load<Texture2D>("ShopItems/Light"),
                upgrade = Upgrades.Light
            });

            _ShopItems.Add(new ShopItem()
            {
                Name = "Speed",
                Description = "Faster movement.",
                InitialCost = 350,
                CostMultipler = 1.25f,
                ItemRectangle = _ItemRectangles[_ShopItems.Count],
                ItemTexture = Game1.Instance.Content.Load<Texture2D>("ShopItems/Speed"),
                upgrade = Upgrades.Speed
            });
            _ShopItems.Add(new ShopItem()
            {
                Name = "Dash",
                Description = "Increases dash distance.",
                InitialCost = 350,
                CostMultipler = 1.25f,
                ItemRectangle = _ItemRectangles[_ShopItems.Count],
                ItemTexture = Game1.Instance.Content.Load<Texture2D>("ShopItems/Dash"),
                upgrade = Upgrades.Dash
            });
            _ShopItems.Add(new ShopItem()
            {
                Name = "Fire Power",
                Description = "Increases fireball damage.",
                InitialCost = 350,
                CostMultipler = 1.25f,
                ItemRectangle = _ItemRectangles[_ShopItems.Count],
                ItemTexture = Game1.Instance.Content.Load<Texture2D>("ShopItems/FirePower"),
                upgrade = Upgrades.FirePower
            });
            _ShopItems.Add(new ShopItem()
            {
                Name = "Fire Rate",
                Description = "Increases the amount of fireballs shot per second by Azazel.",
                InitialCost = 350,
                CostMultipler = 1.25f,
                ItemRectangle = _ItemRectangles[_ShopItems.Count],
                ItemTexture = Game1.Instance.Content.Load<Texture2D>("ShopItems/FireRate"),
                upgrade = Upgrades.FireRate
            });


            _ShopItems.Add(new ShopItem()
            {
                Name = "Luck",
                Description = "Increases coins dropped from enemies.",
                InitialCost = 350,
                CostMultipler = 1.25f,
                ItemRectangle = _ItemRectangles[_ShopItems.Count],
                ItemTexture = Game1.Instance.Content.Load<Texture2D>("ShopItems/luck"),
                upgrade = Upgrades.Luck
            });

            _ShopItems.Add(new ShopItem()
            {
                Name = "Start Allies",
                Description = "Increases number of starting allies.",
                InitialCost = 350,
                CostMultipler = 1.25f,
                ItemRectangle = _ItemRectangles[_ShopItems.Count],
                ItemTexture = Game1.Instance.Content.Load<Texture2D>("ShopItems/StartAllies"),
                upgrade = Upgrades.StartAllies
            });

            _ShopItems.Add(new ShopItem()
            {
                Name = "Coin Magnet",
                Description = "Increases the radius of the coin magnet.",
                InitialCost = 350,
                CostMultipler = 1.25f,
                ItemRectangle = _ItemRectangles[_ShopItems.Count],
                ItemTexture = Game1.Instance.Content.Load<Texture2D>("ShopItems/CoinMagnet"),
                upgrade = Upgrades.CoinMagnet
            });

            base.LoadContent();
        }


        public override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            _SelectedItemIndex = -1;
            for (int i = 0; i < _ShopItems.Count; i++)
            {
                if (_ShopItems[i].ItemRectangle.Contains(mouseState.Position))
                {
                    _SelectedItemIndex = i;
                }

            }

            _PlayHovered = false;
            if (_PlayRectangle.Contains(mouseState.Position))
            {
                _PlayHovered = true;
            }

            if (_PlayHovered && mouseState.LeftButton == ButtonState.Pressed)
            {
                SaveManager.SaveData();
                Fade(false, 5000.0f, Color.Black, () =>
                {
                    GameplayScreen GS = new GameplayScreen();
                    GS.Fade(true, 5000, Color.Black);
                    ScreenManager.AddScreen(GS);
                    ScreenManager.RemoveScreen(this);
                });
            }

            if (_SelectedItemIndex != -1 && mouseState.LeftButton == ButtonState.Pressed && _BuyClickReleased)
            {

                Buy(_ShopItems[_SelectedItemIndex]);
                _BuyClickReleased = false;
            }

            if (mouseState.LeftButton == ButtonState.Released)
            {
                _BuyClickReleased = true;
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {

            ScreenManager.GraphicsDevice.Clear(new Color(168, 50, 50));
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            string text = "SHOP";
            Size2 TextSize = _font.MeasureString(text);
            int X = _ScreenRectangle.Width / 2;
            int Y = _ScreenRectangle.Height / 20;
            Vector2 DrawLocation = new Vector2(X - TextSize.Width / 2, Y - TextSize.Height / 2);

            _spriteBatch.FillRectangle(_ShopRectangle, new Color(161, 98, 98));
            _spriteBatch.DrawString(_font, "SHOP", DrawLocation, Color.White);

            for (int i = 0; i < _ShopItems.Count; i++)
            {
                bool CanBuy = ShopItem.GetCurrentPrice(_ShopItems[i]) <= SaveManager.GetValueInt("money");
                if (i == _SelectedItemIndex && CanBuy)
                {
                    _spriteBatch.FillRectangle(_ShopItems[i].ItemRectangle, new Color(137, 166, 60));

                }
                else
                {

                    _spriteBatch.FillRectangle(_ShopItems[i].ItemRectangle, new Color(144, 161, 98));

                }

                Rectangle ItemTextureRectangle = _ShopItems[i].ItemRectangle;

                ItemTextureRectangle.Inflate(ItemTextureRectangle.Width / -10, ItemTextureRectangle.Height / -10);
                float SmallerSide = Math.Min(ItemTextureRectangle.Width, ItemTextureRectangle.Height);
                ItemTextureRectangle.Inflate((SmallerSide - ItemTextureRectangle.Width) / 2, (SmallerSide - ItemTextureRectangle.Height) / 2); // Makes width and height equal
                _spriteBatch.Draw(_ShopItems[i].ItemTexture, ItemTextureRectangle, Color.White);

                if (!CanBuy) // Gray out
                {
                    _spriteBatch.FillRectangle(_ShopItems[i].ItemRectangle, new Color(Color.Gray, 0.8f));
                }

                float LeftMargin = 5f;
                _spriteBatch.DrawString(_font, "$" + ShopItem.GetCurrentPrice(_ShopItems[i]).ToString(), new Vector2(_ShopItems[i].ItemRectangle.Left + LeftMargin, _ShopItems[i].ItemRectangle.Top), Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1.0f);

            }

            if (_SelectedItemIndex != -1)
            {
                _spriteBatch.FillRectangle(_DescriptionArea, Color.Gray);

                ShopItem SelectedShopItem = _ShopItems[_SelectedItemIndex];
                string ItemName = SelectedShopItem.Name;
                string ItemDescription = SelectedShopItem.Description;
                float ItemValue = (float)Math.Round(SelectedShopItem.upgrade.Value, 2);
                float NextItemValue = (float)Math.Round(SelectedShopItem.upgrade.InitialValue + SelectedShopItem.upgrade.OffsetterFunction(SelectedShopItem.upgrade.Level + 1), 2);
                float ItemValueReciprocal = (float)Math.Round(1 / SelectedShopItem.upgrade.Value, 2);
                float NextItemValueReciprocal = (float)Math.Round(1 / NextItemValue, 2);

                int Margin = 8;
                Vector2 TitlePosition = new Vector2(_DescriptionArea.Left + Margin, _DescriptionArea.Top + Margin);
                float TitleScale = 0.75f;
                Size2 TitleSize = _font.MeasureString(ItemName) * TitleScale;
                _spriteBatch.DrawString(_font, ItemName + " (level " + (SelectedShopItem.upgrade.Level + 1) + ")", TitlePosition, Color.White, 0.0f, Vector2.Zero, TitleScale, SpriteEffects.None, 1.0f);

                Vector2 DescriptionPosition = new Vector2(TitlePosition.X, TitlePosition.Y + TitleSize.Height + Margin);
                MulticoloredString multicoloredString = new MulticoloredString();
                multicoloredString.AppendString(ItemDescription + " (", Color.White);

                // 6 is the firerate upgrade
                multicoloredString.AppendString((_SelectedItemIndex == 6 ? ItemValueReciprocal : ItemValue).ToString() + SelectedShopItem.upgrade.Unit, Color.Red);
                multicoloredString.AppendString(" >>", Color.White);
                multicoloredString.AppendString(" " + (_SelectedItemIndex == 6 ? NextItemValueReciprocal : NextItemValue).ToString() + SelectedShopItem.upgrade.Unit, Color.Green);
                multicoloredString.AppendString(")", Color.White);
                multicoloredString.Draw(_spriteBatch, _font, new Vector2(DescriptionPosition.X, DescriptionPosition.Y), 0.6f);
            }

            if (_PlayHovered)
            {
                _spriteBatch.DrawString(_font, _PlayText, new Vector2(_PlayRectangle.X, _PlayRectangle.Y), Color.Yellow, 0.0f, Vector2.Zero, _PlayTextScale, SpriteEffects.None, 0.0f);


            }
            else
            {
                _spriteBatch.DrawString(_font, _PlayText, new Vector2(_PlayRectangle.X, _PlayRectangle.Y), Color.White, 0.0f, Vector2.Zero, _PlayTextScale, SpriteEffects.None, 0.0f);

            }

            _spriteBatch.DrawString(_font, "$" + SaveManager.GetValueString("money"), Vector2.Zero, Color.LightGray, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
            _spriteBatch.End();

        }
        private void Buy(ShopItem item)
        {
            int PlayerMoney = SaveManager.GetValueInt("money");
            int ItemCost = ShopItem.GetCurrentPrice(item);
            if (PlayerMoney < ItemCost)
            {
                return;
            }
            _KaChing.Play();
            SaveManager.SetValue("money", PlayerMoney - ItemCost);
            Upgrades.LevelUp(item.upgrade);
        }
    }
}
