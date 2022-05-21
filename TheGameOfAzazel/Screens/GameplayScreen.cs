using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Content;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Tweening;
using MonoGame.Extended.ViewportAdapters;
using Penumbra;
using System;
using System.Collections.Generic;
using System.Linq;
using TheGameOfAzazel.Entities;

namespace TheGameOfAzazel
{
    internal class GameplayScreen : GameScreen
    {
        public class AscendingLabel
        {
            public string Text = "";
            public Vector2 Position;
            public Tween tween;
            public float Opacity = 1.0f;
            public Color color;
        }

        public class OFVAllyPointer
        {
            public Vector2 Position;
            public float Rotation;
        }

        public class Round
        {
            public Dictionary<Type, int> Enemies; // Enemy Type, count
            public int MaxSpawnTime; // The maximum time before all enemies are spawned

        }

        public class QueueableAction
        {
            public Action action;
            public float Delay;
        }
        public enum Screen
        {
            Menu,
            Game,
            Pause,
            Shop
        }

        public enum Direction
        {
            Top,
            Bottom,
            Left,
            Right
        }
        private GraphicsDeviceManager _graphics;
        private Game1 _GameInstance;
        private readonly EntityManager _entityManager;
        private SpriteBatch _spriteBatch;
        private readonly List<Type> _EntityDrawOrder = new List<Type>() { typeof(Coin), typeof(Ally), typeof(Enemy), typeof(Bullet), typeof(Azazel), typeof(Explosion) };
        private readonly List<Type> _GUIDrawOrder = new List<Type>() { typeof(TemporaryText) };

        private readonly Tweener _tweener = new Tweener();

        private OrthographicCamera _camera;
        private ViewportAdapter _viewportAdapter;

        public Screen CurrentScreen = Screen.Game;
        private Rectangle _ScreenRectangle;
        private Azazel _player;
        private bool _isDeadFadeStarted = false;
        public static bool DrawingStarted = true;
        private Texture2D _HealthBar;
        private readonly float _HealthBarScale = 3f;
        private Vector2 _HealthBarPosition;

        private Texture2D _SupportBar;
        private readonly float _SupportBarScale = 3f;
        private Vector2 _SupportBarPosition;

        private Texture2D _AllyPointerTexture;
        private readonly float _AllyPointerScale = 0.2f;
        private readonly List<OFVAllyPointer> _OFVAllyPointers = new List<OFVAllyPointer>();

        private TemporaryTextFactory _temporaryTextFactory;
        private BulletFactory _FireballBulletFactory;
        private BulletFactory _MagicballBulletFactory;

        private ExplosionFactory _BasicExplosionFactory;

        private readonly Dictionary<Type, EnemyFactory> _EnemyFactories = new Dictionary<Type, EnemyFactory>();

        private AllyFactory _Level1DemonFactory;
        private readonly Random _RNG = new Random();

        private readonly float _Luck = Upgrades.Luck.Value;
        private readonly float _StartingAllies = Upgrades.StartAllies.Value;
        private readonly float _CoinMagnetRadius = Upgrades.CoinMagnet.Value;

        private readonly Dictionary<string, BitmapFont> _BitmapFonts = new Dictionary<string, BitmapFont>();
        private SoundEffect _CoinCollectedSound;
        private SpriteSheet _CoinSpriteSheet;
        private int _RoundMoney = 0;

        private readonly float _MaxAllyCooldown = 30.0f;
        private float _AllyCooldown = 30.0f;

        private readonly List<AscendingLabel> _ascendingLabels = new List<AscendingLabel>();
        private readonly List<Song> _BackgroundPlaylist = new List<Song>();

        private bool _AllyAttackEnabled = false;
        private readonly float _AllyAttackTargetRadius = 40.0f;
        private Vector2 _AllyAttackTargetPosition;
        private Enemy _TargetEnemy;
        private ParticleEffect _particleEffect;
        private Texture2D _Background;
        private Texture2D _particleTexture;
        private readonly Dictionary<string, Effect> _shaderEffects = new Dictionary<string, Effect>();
        private int _SelectedAttackAllyIndex = 0;

        private List<Ally> _Allies = new List<Ally>();
        private List<Ally> _FollowingAllies = new List<Ally>();
        private List<Bullet> _Bullets = new List<Bullet>();
        private List<Enemy> _Enemies = new List<Enemy>();
        private List<Skeleton> _Skeletons = new List<Skeleton>();
        private List<Dementor> _Dementors = new List<Dementor>();
        private List<Coin> _Coins = new List<Coin>();
        public int CurrentRound = -1;
        private bool _IsInArmageddon = false;
        public int CurrentArmageddonRound = 0;
        private float _CurrentCoinMultiplier = 1;


        private bool _RoundChanged = false;

        private double _RoundStartTime = -1;
        private GameWindow _Window;




        private readonly List<Round> _Rounds = new List<Round>()
        {
            // Round 1
            new Round()
            {
                Enemies = new Dictionary<Type, int>()
                {
                    { typeof(Skeleton), 3 }
                },
                MaxSpawnTime = 10000
            },
            // Round 2
            new Round()
            {
                Enemies = new Dictionary<Type, int>()
                {
                    { typeof(Skeleton), 5 }
                },
                MaxSpawnTime = 10000
            },
            // Round 3
            new Round()
            {
                Enemies = new Dictionary<Type, int>()
                {
                    { typeof(Mage), 2 },
                    { typeof(Skeleton), 6 }
                },
                MaxSpawnTime = 12000
            },
            // Round 4
            new Round()
            {
                Enemies = new Dictionary<Type, int>()
                {
                    { typeof(Mage), 4 },
                    { typeof(Skeleton), 4 }
                },
                MaxSpawnTime = 8000
            },
            // Round 5
            new Round()
            {
                Enemies = new Dictionary<Type, int>()
                {
                    { typeof(Mage), 6 },
                    { typeof(Skeleton), 7 }
                },
                MaxSpawnTime = 5000
            },
            // Round 6
            new Round()
            {
                Enemies = new Dictionary<Type, int>()
                {
                    { typeof(Mage), 4 },
                    { typeof(Skeleton), 4 },
                    { typeof(Dementor), 3 }
                },
                MaxSpawnTime = 10000
            },
            // Round 7
            new Round()
            {
                Enemies = new Dictionary<Type, int>()
                {
                    { typeof(Mage), 5 },
                    { typeof(Skeleton), 5 },
                    { typeof(Dementor), 4 }
                },
                MaxSpawnTime = 12000
            },
            // Round 8
            new Round()
            {
                Enemies = new Dictionary<Type, int>()
                {
                    { typeof(Mage), 6 },
                    { typeof(Skeleton), 6 },
                    { typeof(Dementor), 5 }
                },
                MaxSpawnTime = 5000
            },
            // Round 9
            new Round()
            {
                Enemies = new Dictionary<Type, int>()
                {
                    { typeof(SuicideRat), 3 },
                    { typeof(Mage), 5 },
                    { typeof(Skeleton), 7 },
                    { typeof(Dementor), 5 }
                },
                MaxSpawnTime = 10000
            },
            // Round 10
            new Round()
            {
                Enemies = new Dictionary<Type, int>()
                {
                    { typeof(SuicideRat), 6 },
                    { typeof(Mage), 7 },
                    { typeof(Skeleton), 10 },
                    { typeof(Dementor), 6 }
                },
                MaxSpawnTime = 6000
            }
        };

        private readonly List<QueueableAction> _ActionQueue = new List<QueueableAction>();
        private Direction _DashDirection;

        public GameplayScreen()
        {

            _entityManager = new EntityManager(_EntityDrawOrder, _GUIDrawOrder);
            LightManager.ClearLights();

        }

        public override void Initialize()
        {
            _graphics = ScreenManager.GraphicsManager;
            _GameInstance = Game1.Instance;
            _ScreenRectangle = ScreenManager.GraphicsDevice.Viewport.Bounds;

            base.Initialize();
        }

        public override void LoadContent()
        {
            _Window = _GameInstance.Window;

            _viewportAdapter = new BoxingViewportAdapter(_Window, ScreenManager.GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            CameraManager.Initalize(_viewportAdapter);
            _camera = CameraManager.GetCamera();

            _spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);

            _BackgroundPlaylist.Add(_GameInstance.Content.Load<Song>("Audio/Music1"));
            MediaPlayer.Play(_BackgroundPlaylist.First());
            MediaPlayer.IsRepeating = true;

            SpriteSheet BulletspriteSheet = _GameInstance.Content.Load<SpriteSheet>("SmallRedFire.sf", new JsonContentLoader());
            AnimatedSprite Bulletsprite = new AnimatedSprite(BulletspriteSheet);
            Bulletsprite.Play("fire");

            SoundEffect FireballSoundEffect = _GameInstance.Content.Load<SoundEffect>("Audio/SmallFireball");
            _FireballBulletFactory = new BulletFactory(_entityManager, Bulletsprite, FireballSoundEffect);

            SpriteSheet MagicAttackspriteSheet = _GameInstance.Content.Load<SpriteSheet>("Magicball.sf", new JsonContentLoader());
            AnimatedSprite MagicAttacksprite = new AnimatedSprite(MagicAttackspriteSheet);
            MagicAttacksprite.Play("fire");

            SpriteSheet BasicExplosionspriteSheet = _GameInstance.Content.Load<SpriteSheet>("BasicExplosion.sf", new JsonContentLoader());

            _BasicExplosionFactory = new ExplosionFactory(_entityManager, BasicExplosionspriteSheet, _GameInstance.Content.Load<SoundEffect>("Audio/explosion1"));

            SoundEffect MagicAttackSoundEffect = _GameInstance.Content.Load<SoundEffect>("Audio/MagicAttack");
            _MagicballBulletFactory = new BulletFactory(_entityManager, MagicAttacksprite, MagicAttackSoundEffect, new Color(0, 99, 255));


            SpriteSheet AzazelspriteSheet = _GameInstance.Content.Load<SpriteSheet>("Azazel.sf", new JsonContentLoader());
            AnimatedSprite Azazelsprite = new AnimatedSprite(AzazelspriteSheet);
            Azazelsprite.Play("walk");
            _player = _entityManager.AddEntity(new Azazel(Azazelsprite, _FireballBulletFactory));

            _BitmapFonts.Add("SourceSansPro-SemiBold", _GameInstance.Content.Load<BitmapFont>("Fonts/SourceSansPro-SemiBold"));
            _BitmapFonts.Add("SourceSansPro-Bold", _GameInstance.Content.Load<BitmapFont>("Fonts/SourceSansPro-Bold"));
            _temporaryTextFactory = new TemporaryTextFactory(_entityManager, _BitmapFonts);

            _HealthBar = _GameInstance.Content.Load<Texture2D>("HealthBar");
            _HealthBarPosition = new Vector2((_HealthBar.Width * _HealthBarScale / 2) + 10, ((_HealthBar.Height * _HealthBarScale / 2) + 10));

            _SupportBar = _GameInstance.Content.Load<Texture2D>("SupportBar");
            _SupportBarPosition = _HealthBarPosition.Translate(0, 20 + (_SupportBar.Height * _SupportBarScale / 2));

            _CoinCollectedSound = _GameInstance.Content.Load<SoundEffect>("Audio/CoinCollect");
            _EnemyFactories.Add(typeof(Skeleton), new EnemyFactory<Skeleton>(_entityManager, _GameInstance.Content.Load<SpriteSheet>("Skeleton.sf", new JsonContentLoader()), new Dictionary<string, SoundEffect>() { { "swing", _GameInstance.Content.Load<SoundEffect>("Audio/AxeSwing") }, { "death", _GameInstance.Content.Load<SoundEffect>("Audio/SkeletonDeath") } }));
            _EnemyFactories.Add(typeof(Mage), new EnemyFactory<Mage>(_entityManager, _GameInstance.Content.Load<SpriteSheet>("Mage.sf", new JsonContentLoader()), new Dictionary<string, SoundEffect>() { { "death", _GameInstance.Content.Load<SoundEffect>("Audio/MageDeath") } }, _MagicballBulletFactory));
            _EnemyFactories.Add(typeof(Dementor), new EnemyFactory<Dementor>(_entityManager, _GameInstance.Content.Load<SpriteSheet>("Dementor.sf", new JsonContentLoader())));
            _EnemyFactories.Add(typeof(SuicideRat), new EnemyFactory<SuicideRat>(_entityManager, _GameInstance.Content.Load<SpriteSheet>("SuicideRat.sf", new JsonContentLoader()), new Dictionary<string, SoundEffect>() { { "squeak", _GameInstance.Content.Load<SoundEffect>("Audio/SuicideRatSqueak") }, { "death", _GameInstance.Content.Load<SoundEffect>("Audio/SuicideRatDeath") } }, null, _BasicExplosionFactory));

            _CoinSpriteSheet = _GameInstance.Content.Load<SpriteSheet>("Coin.sf", new JsonContentLoader());
            _AllyPointerTexture = _GameInstance.Content.Load<Texture2D>("AllyPointer");

            _shaderEffects.Add("infinite", _GameInstance.Content.Load<Effect>(@"Shaders/infinite"));
            _Background = _GameInstance.Content.Load<Texture2D>("Background1");

            _Level1DemonFactory = new AllyFactory(_entityManager, _GameInstance.Content, Ally.AllyType.Level1Demon, _FireballBulletFactory);

            for (int i = 0; i < _StartingAllies; i++)
            {
                _Level1DemonFactory.Spawn(_player.Position);
            }

            _particleTexture = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
            _particleTexture.SetData(new[] { Color.White });

            ParticleInit(new TextureRegion2D(_particleTexture));
        }


  

        public override void Update(GameTime gameTime)
        {
            float deltaTime = gameTime.GetElapsedSeconds();
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if (_player.isDead && !_isDeadFadeStarted)
            {
                SaveManager.SetValue("money", SaveManager.GetValueInt("money") + _RoundMoney);
                SaveManager.SaveData();
                float Scale = 0.9f;
                string LabelText = "Game over!";
                Vector2 LabelSize = _temporaryTextFactory.MeasureText(LabelText, "SourceSansPro-Bold", Scale);
                _temporaryTextFactory.Create(LabelText, _ScreenRectangle.Center.ToVector2() - LabelSize / 2, Color.White, 3.5, "SourceSansPro-Bold", Scale);

                Fade(false, 5000.0f, Color.Black, () =>
                {
                    ShopScreen SS = new ShopScreen();
                    SS.Fade(true, 5000, Color.Black);
                    ScreenManager.RemoveScreen(this);
                    ScreenManager.AddScreen(SS);

                });
                _isDeadFadeStarted = true;
            }

            if (_player != null && !_player.IsDestroyed)
            {
                
                _Allies = _entityManager.Entities.Where(e => e is Ally).Cast<Ally>().ToList();
                _FollowingAllies = _Allies.Where(a => a.status == Ally.AllyStatus.Following).ToList();
                _Bullets = _entityManager.Entities.Where(e => e is Bullet).Cast<Bullet>().ToList();
                _Enemies = _entityManager.Entities.Where(e => e is Enemy).Cast<Enemy>().ToList();
                _Coins = _entityManager.Entities.Where(e => e is Coin).Cast<Coin>().ToList();
                _Skeletons = _entityManager.Entities.Where(e => e is Skeleton).Cast<Skeleton>().ToList();
                _Dementors = _entityManager.Entities.Where(e => e is Dementor).Cast<Dementor>().ToList();

                if (CurrentRound == -1) // No round started yet
                {
                    StartRound(1);
                }

                if (_RoundChanged)
                {
                    _RoundStartTime = gameTime.TotalGameTime.TotalMilliseconds;
                    _RoundChanged = false;
                }
                float acceleration = Upgrades.Speed.Value;

                if (!_AllyAttackEnabled)
                {
                    if (!_player.IsDashing)
                    {
                        if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                        {
                            _player.Translate(new Vector2(0, deltaTime * -acceleration));
                            _DashDirection = Direction.Top;
                        }

                        if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                        {
                            _player.Translate(new Vector2(0, deltaTime * acceleration));
                            _DashDirection = Direction.Bottom;
                        }

                        if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                        {
                            _player.Translate(new Vector2(deltaTime * -acceleration, 0));
                            _DashDirection = Direction.Left;
                        }

                        if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                        {
                            _player.Translate(new Vector2(deltaTime * acceleration, 0));
                            _DashDirection = Direction.Right;
                        }
                    }

                    float dashDistance = Upgrades.Dash.Value;
                    if (keyboardState.IsKeyDown(Keys.Space) && !_player.IsDashing)
                    {
                        switch (_DashDirection)
                        {
                            case Direction.Top:
                                _player.Dash(new Vector2(0, -dashDistance));
                                break;
                            case Direction.Bottom:
                                _player.Dash(new Vector2(0, dashDistance));
                                break;
                            case Direction.Left:
                                _player.Dash(new Vector2(-dashDistance, 0));
                                break;
                            case Direction.Right:
                                _player.Dash(new Vector2(dashDistance, 0));
                                break;


                        }

                    }

                    if (keyboardState.IsKeyDown(Keys.Z))
                    {
                        CircleF AttackRadius = new CircleF(_player.Position, 350);
                        List<Enemy> CloseEnemies = _Enemies.Where(x => AttackRadius.Contains(x.Position)).ToList();

                        if (CloseEnemies.Count > 0)
                        {
                            foreach (Ally ally in _FollowingAllies)
                            {
                                ally.Attack(CloseEnemies[_RNG.Next(CloseEnemies.Count)]);
                            }
                        }

                    }
                }
                else
                {
                    if (keyboardState.IsKeyDown(Keys.Q))
                    {
                        if (_SelectedAttackAllyIndex - 1 < 0)
                        {
                            _SelectedAttackAllyIndex = _FollowingAllies.Count - 1;
                        }
                        else
                        {
                            _SelectedAttackAllyIndex -= 1;
                        }
                    }

                    if (keyboardState.IsKeyDown(Keys.E))
                    {
                        if (_SelectedAttackAllyIndex + 1 > _FollowingAllies.Count - 1)
                        {
                            _SelectedAttackAllyIndex = 0;
                        }
                        else
                        {
                            _SelectedAttackAllyIndex += 1;
                        }
                    }
                }
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (!_AllyAttackEnabled)
                    {
                        double Angle = Helpers.AngleFromPoints(_player.Position, _camera.ScreenToWorld(mouseState.Position.ToVector2()));

                        float DirectionX = (float)(1 * Math.Cos(Angle));
                        float DirectionY = (float)(1 * Math.Sin(Angle));

                        _player.Fire(new Vector2(DirectionX, DirectionY), (float)Angle);
                    }
                    else
                    {
                        if (_FollowingAllies.Count > 0)
                        {
                            if (_TargetEnemy == null)
                            {
                                _FollowingAllies[_SelectedAttackAllyIndex].Goto(_AllyAttackTargetPosition);
                            }
                            else
                            {
                                _FollowingAllies[_SelectedAttackAllyIndex].Attack(_TargetEnemy);
                            }
                        }
                        _player.fireCooldown = 0.5f;
                        _AllyAttackEnabled = false;
                    }
                }

                if (mouseState.RightButton == ButtonState.Pressed && !_player.IsDashing)
                {
                    if (_FollowingAllies.Count() != 0)
                    {
                        _AllyAttackEnabled = true;
                    }

                }

                if (_AllyAttackEnabled)
                {
                    _TargetEnemy = null;
                    Vector2 TargetPosition = _camera.ScreenToWorld(mouseState.Position.ToVector2());
                    CircleF TargetCircle = new CircleF(TargetPosition, _AllyAttackTargetRadius * 1.5f);
                    foreach (Enemy enemy in _Enemies)
                    {
                        if (TargetCircle.Contains(enemy.Position))
                        {
                            _TargetEnemy = enemy;
                            TargetPosition = enemy.Position;
                        }
                    }

                    _AllyAttackTargetPosition = TargetPosition;

                }

                _camera.LookAt(_player.Position);

            }

            if (_AllyCooldown > 0)
            {
                _AllyCooldown -= deltaTime;
            }
            else
            {
                _Level1DemonFactory.Spawn(_player.Position);
                _AllyCooldown = _MaxAllyCooldown;
            }

            for (int i = 0; i < _ActionQueue.Count; i++)
            {
                _ActionQueue[i].Delay -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_ActionQueue[i].Delay <= 0)
                {
                    _ActionQueue[i].action();
                    _ActionQueue.RemoveAt(i);
                }
            }

            _entityManager.Update(gameTime);

            CheckNewAllies();
            CheckCollisions();
            AttractCoins();

            _OFVAllyPointers.Clear();

            Rectangle CameraWorldRect = new Rectangle(
                Convert.ToInt32(_camera.Position.X - ((_Window.ClientBounds.Width / 2) / _camera.Zoom)),
                Convert.ToInt32(_camera.Position.Y - ((_Window.ClientBounds.Height / 2) / _camera.Zoom)),
                Convert.ToInt32(_Window.ClientBounds.Width / _camera.Zoom),
                Convert.ToInt32(_Window.ClientBounds.Height / _camera.Zoom));
            foreach (Ally ally in _Allies)
            {
                Vector2 targetPosOnScreen = _camera.WorldToScreen(ally.Position);

                if (_camera.Contains(ally.Position) == ContainmentType.Disjoint && ally.status == Ally.AllyStatus.Idle)
                {
                    Vector2 center = _ScreenRectangle.Center.ToVector2(); 
                    float angle = MathHelper.ToDegrees((float)Math.Atan2(targetPosOnScreen.Y - center.Y, targetPosOnScreen.X - center.X));

                    float coef;
                    if (_ScreenRectangle.Width > _ScreenRectangle.Height)
                    {
                        coef = _ScreenRectangle.Width / _ScreenRectangle.Height;
                    }
                    else
                    {
                        coef = _ScreenRectangle.Height / _ScreenRectangle.Width;
                    }

                    float degreeRange = 360f / (coef + 1);
                    float angle2 = MathHelper.ToDegrees((float)Math.Atan2(_ScreenRectangle.Height - center.Y, _ScreenRectangle.Width - center.X));
                    if (angle < 0)
                    {
                        angle += 360;
                    }

                    int edgeLine;
                    if (angle < angle2)
                    {
                        edgeLine = 0;
                    }
                    else if (angle < 180 - angle2)
                    {
                        edgeLine = 1;
                    }
                    else if (angle < 180 + angle2)
                    {
                        edgeLine = 2;
                    }
                    else if (angle < 360 - angle2)
                    {
                        edgeLine = 3;
                    }
                    else
                    {
                        edgeLine = 0;
                    }

                    float DrawAngle = MathHelper.ToRadians(angle + 180f);

                    Vector2 PointerOffset = Helpers.GetBoundingBox(_AllyPointerTexture.Width * _AllyPointerScale, _AllyPointerTexture.Height * _AllyPointerScale, DrawAngle) / 2;
                    switch (edgeLine)
                    {
                        case 0: // bottom or right
                            PointerOffset *= -1;
                            break;
                        case 1: // bottom or right
                            PointerOffset *= -1;
                            break;
                        case 2: // left
                            PointerOffset *= 1;
                            break;
                        case 3: // top
                            PointerOffset *= 1;
                            break;
                    }

                    _OFVAllyPointers.Add(new OFVAllyPointer
                    {
                        Rotation = DrawAngle,
                        Position = Helpers.intersect(edgeLine, center, targetPosOnScreen, _ScreenRectangle) + PointerOffset 
                    });
                }

            }
            _particleEffect.Position = _AllyAttackTargetPosition;
            _particleEffect.Update(deltaTime);

            _tweener.Update(deltaTime);

            if (_IsFading)
            {
                _FadingTimeLeft -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawingStarted = true;
            BitmapFont font = _BitmapFonts["SourceSansPro-Bold"];
            LightManager.BeginDraw();
            ScreenManager.GraphicsDevice.Clear(Color.White); 
            Effect infinite = _shaderEffects["infinite"];
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, _ScreenRectangle.Width, _ScreenRectangle.Height, 0, 0, 1);
            Matrix uv_transform = GetUVTransform(_Background, new Vector2(0, 0), 1f, _graphics.GraphicsDevice.Viewport);
            infinite.Parameters["view_projection"].SetValue(Matrix.Identity * projection);
            infinite.Parameters["uv_transform"].SetValue(Matrix.Invert(uv_transform));

            _spriteBatch.Begin(effect: infinite, samplerState: SamplerState.LinearWrap, blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(_Background, _graphics.GraphicsDevice.Viewport.Bounds, Color.White);
            _spriteBatch.End();

            // entities
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            _entityManager.Draw(_spriteBatch);
            
            _spriteBatch.End();

            LightManager.Draw(gameTime);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());

            // Ascending labels
            foreach (AscendingLabel label in _ascendingLabels)
            {
                if (label.Opacity <= 0)
                {
                    _ascendingLabels.Remove(label);
                    continue;
                }

                float Scale = 0.4f;
                Vector2 LabelSize = font.MeasureString(label.Text) * Scale;
                Color textColor = label.color * (float)Math.Max(1.0 - label.tween.Completion, 0);
                _spriteBatch.DrawString(font, label.Text, label.Position - LabelSize / 2, textColor, 0.0f, new Vector2(), Scale, SpriteEffects.None, 0.0f);
            }

            // Attack target
            if (_AllyAttackEnabled)
            {
                _spriteBatch.Draw(_particleEffect);
            }


            _spriteBatch.End();

            // GUI
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);
            foreach (OFVAllyPointer allyPointer in _OFVAllyPointers)
            {
                _spriteBatch.Draw(new Sprite(_AllyPointerTexture), allyPointer.Position, allyPointer.Rotation, Vector2.One * _AllyPointerScale);
            }

            Sprite HealthBarSprite = new Sprite(_HealthBar);
            Size2 HealthBarSize = new Size2(_HealthBar.Width * _HealthBarScale, _HealthBar.Height * _HealthBarScale);
            _spriteBatch.FillRectangle(_HealthBarPosition - (Vector2)HealthBarSize / 2, HealthBarSize, Color.Black);
            _spriteBatch.FillRectangle(_HealthBarPosition - (Vector2)HealthBarSize / 2, new Size2(HealthBarSize.Width * (_player.HealthPoints / _player.MaxHealthPoints), HealthBarSize.Height), new Color(130, 63, 63));
            _spriteBatch.Draw(HealthBarSprite, _HealthBarPosition, 0f, Vector2.One * _HealthBarScale);

            Size2 SupportBarSize = new Size2(_SupportBar.Width * _SupportBarScale, _SupportBar.Height * _SupportBarScale);
            _spriteBatch.FillRectangle(_SupportBarPosition - (Vector2)SupportBarSize / 2, SupportBarSize, Color.Black);
            _spriteBatch.FillRectangle(_SupportBarPosition - (Vector2)SupportBarSize / 2, new Size2(SupportBarSize.Width * (_AllyCooldown / _MaxAllyCooldown), SupportBarSize.Height), new Color(26, 76, 102));
            Sprite SupportBarSprite = new Sprite(_SupportBar);
            _spriteBatch.Draw(SupportBarSprite, _SupportBarPosition, 0f, Vector2.One * _SupportBarScale);

            string MoneyCountString = "$" + _RoundMoney;
            float Scale2 = 0.7f;
            Vector2 MoneyCountSize = font.MeasureString(MoneyCountString) * Scale2;
            _spriteBatch.DrawString(font, MoneyCountString, new Vector2(_ScreenRectangle.Width / 2 - MoneyCountSize.X / 2, 0), Color.White, 0.0f, Vector2.Zero, Scale2, SpriteEffects.None, 0.0f);
            if (_IsInArmageddon)
            {
                string CoinMultiplerString = "x" + _CurrentCoinMultiplier + " ARMAGEDDON COIN MULTIPLIER";
                float Scale3 = 0.35f;
                Vector2 CoinMultiplerSize = font.MeasureString(CoinMultiplerString) * Scale3;
                _spriteBatch.DrawString(font, CoinMultiplerString, new Vector2(_ScreenRectangle.Width / 2 - CoinMultiplerSize.X / 2, MoneyCountSize.Y - CoinMultiplerSize.Y / 2), Color.Brown, 0.0f, Vector2.Zero, Scale3, SpriteEffects.None, 0.0f);
            }


            _entityManager.GUIDraw(_spriteBatch);


            _spriteBatch.End();



        }

        public override void UnloadContent()
        {
            _GameInstance.Content.Unload();
            base.UnloadContent();
        }

        private void OnEnemyDeath(object sender, EventArgs e)
        {
            if (_Enemies.Count == 1) // This was the last enemy
            {
                StartRound(CurrentRound + 1);
            }

            if (sender.GetType() == typeof(Skeleton))
            {
                for (int i = 0; i < _Skeletons.Count; i++)
                {

                    _Skeletons[i].ID = i;
                }
                SpawnCoins(GetCoinSpawnCount(12), ((Skeleton)sender).Position);
            }
            if (sender.GetType() == typeof(Mage))
            {
                SpawnCoins(GetCoinSpawnCount(20), ((Mage)sender).Position);
            }
            if (sender.GetType() == typeof(Dementor))
            {
                for (int i = 0; i < _Dementors.Count; i++)
                {

                    _Dementors[i].ID = i;
                }
                SpawnCoins(GetCoinSpawnCount(30), ((Dementor)sender).Position);
            }
            if (sender.GetType() == typeof(SuicideRat))
            {
                for (int i = 0; i < _Dementors.Count; i++)
                {

                    _Dementors[i].ID = i;
                }
                SpawnCoins(GetCoinSpawnCount(2), ((SuicideRat)sender).Position);
            }
        }

        private int GetCoinSpawnCount(int baseCount) // Applies luck to coin count
        {
            return _RNG.Next((int)((baseCount - (baseCount / 2)) * (1 + (_Luck / 100))), (int)((baseCount + (baseCount / 2)) * (1 + (_Luck / 100))));
        }

        private void SpawnCoins(int Count, Vector2 position)
        {
            for (int i = 0; i < Count; i++)
            {
                CircleF spawnCircle = new CircleF(position, _RNG.Next(10, 100));
                float spawnAngle = MathHelper.ToRadians(_RNG.Next(0, 360));
                Point2 spawnPosition = spawnCircle.BoundaryPointAt(spawnAngle);

                Coin coin = new Coin(new AnimatedSprite(_CoinSpriteSheet))
                {
                    Position = spawnPosition
                };
                _entityManager.AddEntity(coin);
            }
        }

        private void AttractCoins()
        {

            for (int i = 0; i < _Coins.Count; i++)
            {
                CircleF MagnetRadius = new CircleF(_player.Position, _CoinMagnetRadius);
                if (MagnetRadius.Contains(_Coins[i].Position))
                {
                    float Distance = 4f;
                    _Coins[i].Position = _Coins[i].Position + (((_player.Position - _Coins[i].Position).NormalizedCopy()) * Distance);
                }

            }
        }
        private void CheckCollisions()
        {
            Dementor[] Dementors = _entityManager.Entities.Where(e => e is Dementor).Cast<Dementor>().ToArray();

            // Bullet hit enemy
            foreach (Enemy enemy in _Enemies)
            {
                Bullet[] BulletsHit = _Bullets.Where(bullet => enemy.Contains(bullet.Position)).ToArray();
                for (int i = BulletsHit.Length - 1; i >= 0; i--)
                {
                    if (BulletsHit[i].Source == _player)
                    {
                        enemy.Damage(BulletsHit[i].Power);
                        BulletsHit[i].Destroy();
                        _Bullets.RemoveAt(i);
                    }

                }
            }

            // Bullet hit ally
            foreach (Ally ally in _Allies)
            {
                for (int i = _Bullets.Count - 1; i >= 0; i--)
                {

                    if (ally.Contains(_Bullets[i].Position) && _Bullets[i].Source != _player)
                    {
                        ally.Kill();
                        _Bullets[i].Destroy();
                        _Bullets.RemoveAt(i);
                        break;
                    }

                }
            }
            // Bullet hit player
            List<Bullet> BulletsHitPlayer = _Bullets.Where(bullet => _player.Contains(bullet.Position)).ToList();
            for (int i = BulletsHitPlayer.Count - 1; i >= 0; i--)
            {

                if (BulletsHitPlayer[i].Source != _player)
                {
                    _player.Damage(BulletsHitPlayer[i].Power);
                    BulletsHitPlayer[i].Destroy();
                    _Bullets.RemoveAt(i);
                }
            }

            int InstancesCreated = 0;
            foreach (Coin coin in _Coins)
            {
                if (_player.Contains(coin.Position))
                {
                    if (InstancesCreated < 1)
                    {

                        _CoinCollectedSound.Play(0.3f, 0.0f, 0.0f);
                        InstancesCreated++;
                    }

                    _RoundMoney += (int)(10 * _CurrentCoinMultiplier);
                    coin.Destroy();
                }
            }

        }

        private void CheckNewAllies()
        {
            CircleF PlayerCircle = new CircleF(_player.Position, 100);

            foreach (Ally Ally in _Allies)
            {
                if (PlayerCircle.Contains(Ally.Position) && Ally.status == Ally.AllyStatus.Idle)
                {
                    Vector2 LabelPosition = Ally.Position.Translate(0, -20);
                    AscendingLabel label = new AscendingLabel { Text = "+10 HP", Opacity = 1, Position = LabelPosition, color = Color.Red };
                    Tween tween = _tweener.TweenTo(label, a => a.Position, LabelPosition.Translate(0, -100), duration: 1.5f).Easing(EasingFunctions.ExponentialIn);

                    _player.SetHealth(_player.HealthPoints + 10);
                    label.tween = tween;
                    _ascendingLabels.Add(label);

                    Ally.Follow(_player);

                }
            }

        }

        private void ParticleInit(TextureRegion2D textureRegion)
        {
            _particleEffect = new ParticleEffect(autoTrigger: false)
            {

                Emitters = new List<ParticleEmitter>
                {
                    new ParticleEmitter(textureRegion, 500, TimeSpan.FromSeconds(0.4),
                        Profile.Ring(_AllyAttackTargetRadius, Profile.CircleRadiation.Out))
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Opacity = new Range<float>(75f, 100f),
                            Speed = new Range<float>(0f, 50f),
                            Quantity = 5,
                            Rotation = new Range<float>(-1f, 1f),
                            Scale = new Range<float>(3.0f, 4.0f)
                        },
                        Modifiers =
                        {
                            new AgeModifier
                            {
                                Interpolators =
                                {
                                    new ColorInterpolator
                                    {
                                        StartValue = HslColor.FromRgb(new Color(122, 43, 45)),
                                        EndValue = new HslColor(0, 0, 0.1f)
                                    }
                                }

                            },

                            new RotationModifier {RotationRate = -2.1f},
                            new LinearGravityModifier {Direction = -Vector2.UnitY, Strength = 30f}
                        }
                    }
                }
            };
        }

        private void StartRound(int Round)
        {
            if (Round > _Rounds.Count)
            {
                // Armageddon round generation
                _IsInArmageddon = true;
                CurrentArmageddonRound++;
                _CurrentCoinMultiplier = 1 + ((float)CurrentArmageddonRound / 10);
                Round GeneratedRound = _Rounds[^1];
                Type[] types = GeneratedRound.Enemies.Keys.ToArray();
                foreach (Type type in types)
                {
                    int Count = GeneratedRound.Enemies[type];
                    int NewCount = (int)Math.Ceiling(Count * 1.1f);
                    GeneratedRound.Enemies[type] = NewCount;
                }
                GeneratedRound.MaxSpawnTime = (int)Math.Floor(GeneratedRound.MaxSpawnTime * 0.95f);
                _Rounds.Add(GeneratedRound);
            }

            Round RoundToStart = _Rounds[Round - 1];
            foreach (Type EnemyType in RoundToStart.Enemies.Keys)
            {
                for (int i = 0; i < RoundToStart.Enemies[EnemyType]; i++)
                {

                    _ActionQueue.Add(new QueueableAction()
                    {
                        action = () =>
                        {
                            SpawnEnemy(EnemyType);
                        },
                        Delay = _RNG.Next(0, RoundToStart.MaxSpawnTime)
                    });
                }

            }


            if (Round != 1)
            {
                float Scale = 0.8f;
                if (!_IsInArmageddon)
                {
                    string LabelText = "Round " + Round.ToString();
                    Vector2 LabelSize = _temporaryTextFactory.MeasureText(LabelText, "SourceSansPro-Bold", Scale);
                    _temporaryTextFactory.Create(LabelText, _ScreenRectangle.Center.ToVector2() - LabelSize / 2, Color.White, 3, "SourceSansPro-Bold", Scale);
                }
                else
                {
                    string LabelText = "Armageddon " + CurrentArmageddonRound.ToString();
                    Vector2 LabelSize = _temporaryTextFactory.MeasureText(LabelText, "SourceSansPro-Bold", Scale);
                    _temporaryTextFactory.Create(LabelText, _ScreenRectangle.Center.ToVector2() - LabelSize / 2, Color.Brown, 3, "SourceSansPro-Bold", Scale);
                }
            }

            CurrentRound = Round;
            _RoundChanged = true;

        }

        private void SpawnEnemy(Type EnemyType)
        {
            Enemy e = _EnemyFactories[EnemyType].Spawn(_player);
            e.Death += OnEnemyDeath;

        }

        private Matrix GetView()
        {
            int width = _ScreenRectangle.Width;
            int height = _ScreenRectangle.Height;
            Vector2 origin = new Vector2(width / 2f, height / 2f);
            Vector2 _xy = _camera.Position;
            return
                Matrix.CreateTranslation(-origin.X, -origin.Y, 0f) *
                Matrix.CreateTranslation(-_xy.X, -_xy.Y, 0f) *
                Matrix.CreateTranslation(origin.X, origin.Y, 0f);
        }


        private Matrix GetUVTransform(Texture2D t, Vector2 offset, float scale, Viewport v)
        {
            return
                Matrix.CreateScale(t.Width, t.Height, 1f) *
                Matrix.CreateScale(scale, scale, 1f) *
                Matrix.CreateTranslation(offset.X, offset.Y, 0f) *
                GetView() *
                Matrix.CreateScale(1f / v.Width, 1f / v.Height, 1f);
        }

    }
}
