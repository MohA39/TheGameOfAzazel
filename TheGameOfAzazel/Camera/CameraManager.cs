using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace TheGameOfAzazel
{
    public static class CameraManager
    {
        private static OrthographicCamera _camera;

        public static void Initalize(ViewportAdapter viewportAdapter)
        {
            _camera = new OrthographicCamera(viewportAdapter);
        }

        public static OrthographicCamera GetCamera()
        {
            return _camera;
        }
    }
}
