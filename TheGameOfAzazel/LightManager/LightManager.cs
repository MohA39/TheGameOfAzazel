using Microsoft.Xna.Framework;
using Penumbra;

namespace TheGameOfAzazel
{
    public static class LightManager
    {
        private static PenumbraComponent _penumbra;

        public static void Initalize(Game game)
        {
            _penumbra = new PenumbraComponent(game) { AmbientColor = new Color(new Vector3(0.15f)) };
            _penumbra.Initialize();
        }
        public static void AddLight(Light light)
        {
            _penumbra.Lights.Add(light);
        }

        public static void DeleteLight(Light light)
        {

            _penumbra.Lights.Remove(light);

        }
        public static void ClearLights()
        {

            _penumbra.Lights.Clear();
        }
        public static void BeginDraw()
        {
            _penumbra.BeginDraw();
        }

        public static void Draw(GameTime gameTime)
        {
            _penumbra.Draw(gameTime);
        }
    }
}
