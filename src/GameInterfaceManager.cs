using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Cyvatar.src
{
    // 2. The Manager (this will replace Game1)
    public class GameInterfaceManager : Game
    {
        private GraphicsDeviceManager _graphics;

        private Dictionary<string, IGameScene> _interfaces = new();
        private IGameScene _currentInterface;

        public static GraphicsDevice GameGraphicsDevice;
        public const int ScreenWidth = 1920;
        public const int ScreenHeight = 1080;
        public const float FPS = 60f;

        public GameInterfaceManager()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            // Window setup (size, title, fps...) goes here
            Window.Title = "Cyvatar - Beta";
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / FPS);
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            GameGraphicsDevice = GraphicsDevice;
            // Register all interfaces
            RegisterScene(new BattleScene(GraphicsDevice));

            // Start with the main menu
            ChangeInterface("Battle");

            GraphicManager.Screen = new SpriteBatch(GraphicsDevice);

            base.Initialize();
        }

        private void RegisterScene(IGameScene gameInterface)
        {
            if (_interfaces.ContainsKey(gameInterface.Link))
                throw new Exception($"Duplicated interface link: {gameInterface.Link}");

            _interfaces[gameInterface.Link] = gameInterface;
        }

        public void ChangeInterface(string link)
        {
            if (!_interfaces.ContainsKey(link))
                throw new Exception($"Interface not found: {link}");

            _currentInterface = _interfaces[link];
            _currentInterface.LoadContent();   // Optional
        }

        protected override void Update(GameTime gameTime)
        {
            // Let the current interface update
            _currentInterface.Update(gameTime);

            // Check if it wants to switch
            string next = _currentInterface.GetNextScene();
            if (next != null)
            {
                ChangeInterface(next);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            GraphicManager.Screen.Begin();
            _currentInterface.Draw(GraphicManager.Screen);
            GraphicManager.Screen.End();

            base.Draw(gameTime);
        }

        public static Texture2D LoadImage(string path, Vector2? size = null, float scale = 1.0f)
        {
            return GraphicManager.LoadImage(GameGraphicsDevice, path, size, scale);
        }
    }
}
