using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Cyvatar.src.Warriors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src
{
    public class BattleScene : IGameScene
    {
        public string Link => "Battle";

        // Assets
        private Texture2D _background;
        private Texture2D _frontBase;

        // Game objects
        // private BlasterPlayer _player;
        private Player _player;

        // Transition control
        private string? _nextScene = null;

        private readonly GraphicsDevice _graphicsDevice;
        private readonly ContentManager _content;   // Optional if you use Content Pipeline
        public DualSenseController Controller { get; set; } = new DualSenseController();

        public static float dt = 0;

        public BattleScene(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void LoadContent()
        {
            // === Load images (runtime loading style) ===
            _background = GameInterfaceManager.LoadImage(Path.Combine("Battle", "Fields", "GrassField", "Field.png"));
            _frontBase = GameInterfaceManager.LoadImage(Path.Combine("Battle", "Fields", "GrassField", "Base.png"));

            // Create player
            _player = new BlasterPlayer();
        }

        public void Update(GameTime gameTime)
        {
            BattleScene.dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // --- Input ---
            var keyboard = Keyboard.GetState();

            // Example of quitting the battle
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                _nextScene = "OpenWorld";          // Return to open world
                return;
            }

            // Player actions
            _player.Act(Controller.GetPressed());
            _player.Update(dt);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Background
            spriteBatch.Draw(_background, Vector2.Zero, Color.White);

            // Player
            _player.Draw(spriteBatch);

            // Front base (bottom bar)
            int screenHeight = _graphicsDevice.Viewport.Height;
            spriteBatch.Draw(_frontBase, new Vector2(0, screenHeight - _frontBase.Height), Color.White);
        }

        public string? GetNextScene()
        {
            string? next = _nextScene;
            _nextScene = null;     // Reset after reading
            return next;
        }
    }
}
