using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Cyvatar.src.GameScenes;

namespace Cyvatar.src
{
    /// <summary>
    /// This Class is the main core of the Game architecture. It implements the logic that allows switching between GameScenes.
    /// </summary>
    public class GameSceneManager : Game
    {
        /// <summary>
        /// This game uses the MonoGame framework and builds upon it. This property brings some graphic related functionalities from that foundation.
        /// </summary>
        private GraphicsDeviceManager _graphics;

        /// <summary>
        /// This game uses the MonoGame framework and builds upon it. This property brings some rendering related functionalities from that foundation.
        /// </summary>
        public static GraphicsDevice GameGraphicsDevice;

        /// <summary>
        /// A way to obtain the GameScene by knowing the corresponding GameSceneLink.
        /// </summary>
        private Dictionary<GameSceneLink, GameScene> _gameScenes = new();

        /// <summary>
        /// When a GameScene has ended, this class uses the link contained by this property to know to which GameScene it should go next.
        /// </summary>
        public static GameSceneLink NextGameScene = GameSceneLink.LBattleScene;

        /// <summary>
        /// The current GameScene being exposed to the player.
        /// </summary>
        private GameScene _currentGameScene;

        /// <summary>
        /// Total Width of the Screen (Pixels).
        /// </summary>
        public const int ScreenWidth = 1920;

        /// <summary>
        /// Total Height of the Screen (Pixels).
        /// </summary>
        public const int ScreenHeight = 1080;

        /// <summary>
        /// Amount of frames being displayed every second.
        /// </summary>
        public const float FPS = 60f;

        public GameSceneManager()
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
            RegisterGameScene(new BattleScene(GraphicsDevice));

            // set up the initial GameScene (Should be MainMenu)
            ChangeGameScene();

            GraphicManager.Screen = new SpriteBatch(GraphicsDevice);

            base.Initialize();
        }

        private void RegisterGameScene(GameScene gameInterface)
        {
            if (_gameScenes.ContainsKey(gameInterface.Link))
                throw new Exception($"Duplicated interface link: {gameInterface.Link}");

            _gameScenes[gameInterface.Link] = gameInterface;
        }

        public void ChangeGameScene()
        {
            if (!_gameScenes.ContainsKey(NextGameScene))
                throw new Exception($"GameScene not found: {NextGameScene.ToString()}");

            _currentGameScene = _gameScenes[NextGameScene];

            // Try including an assincronous way to load GameScenes when needed.
            _currentGameScene.LoadContent();   // Optional
        }

        protected override void Update(GameTime gameTime)
        {
            // Let the current interface update
            _currentGameScene.Update(gameTime);

            // Check if it wants to switch
            if (_currentGameScene.HasEnded)
                ChangeGameScene();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            GraphicManager.Screen.Begin();
            _currentGameScene.Draw();
            GraphicManager.Screen.End();

            base.Draw(gameTime);
        }

        public static Texture2D LoadImage(string path, Vector2? size = null, float scale = 1.0f)
        {
            return GraphicManager.LoadImage(GameGraphicsDevice, path, size, scale);
        }
    }
}
