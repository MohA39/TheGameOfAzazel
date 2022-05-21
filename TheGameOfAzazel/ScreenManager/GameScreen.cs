using Microsoft.Xna.Framework;
using System;

namespace TheGameOfAzazel
{
    public abstract class GameScreen
    {
        public float TotalFadeTime
        {
            get => _TotalFadeTime;
            private set => _TotalFadeTime = value;
        }
        protected float _TotalFadeTime = 600f;

        public float FadingTimeLeft
        {
            get => _FadingTimeLeft;
            private set => _FadingTimeLeft = value;
        }
        protected float _FadingTimeLeft = 0f;


        public bool IsFading
        {
            get => _IsFading;
            private set => _IsFading = value;
        }
        protected bool _IsFading = false;


        public bool FadingInOrOut
        {
            get => _FadingInOrOut;
            private set => _FadingInOrOut = value;
        }
        protected bool _FadingInOrOut = false;

        protected Action _OnFadeComplete = () => { };

        public Color FadeColor
        {
            get => _FadeColor;
            private set => _FadeColor = value;
        }
        protected Color _FadeColor = Color.Black;

        public ScreenManager ScreenManager
        {
            get => _screenManager;
            internal set => _screenManager = value;
        }

        protected ScreenManager _screenManager;
        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
        public virtual void Initialize() { }

        public virtual void Update(GameTime gameTime)
        {
            if (_IsFading)
            {
                _FadingTimeLeft -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            if (_FadingTimeLeft <= 0 && _IsFading)
            {
                _OnFadeComplete();
                _IsFading = false;
            }
        }


        public abstract void Draw(GameTime gameTime);

        public void Fade(bool InOrOut, float Duration, Color? FadeColor = null, Action OnComplete = null)
        {
            if (!FadeColor.HasValue)
            {
                FadeColor = Color.Black;
            }

            if (OnComplete == null)
            {
                _OnFadeComplete = () => { };
            }
            else
            {
                _OnFadeComplete = OnComplete;
            }

            _FadingInOrOut = InOrOut;
            _TotalFadeTime = Duration;
            _FadingTimeLeft = Duration;
            _FadeColor = FadeColor.Value;
            _IsFading = true;

        }
    }
}