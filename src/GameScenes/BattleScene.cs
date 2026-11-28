using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading.Tasks;
using Cyvatar.src.Battle;
using System.Diagnostics;

namespace Cyvatar.src.GameScenes
{
    public class BattleScene : GameScene
    {
        public static readonly SpriteFont HpFont = GraphicManager.LoadFont("HPFont");
        public static BattleField Field = new BattleField();

        // Transition control
        private GameSceneLink _nextScene;

        // private readonly ContentManager _content;   // Optional if you use Content Pipeline

        public DualSenseController Controller { get; set; } = new DualSenseController();

        public BattleScene(GraphicsDevice graphicsDevice) : base(graphicsDevice, GameSceneLink.LBattleScene) { }

        public override void LoadContent()
        {
            Field.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // This Delta Time value will be used by all entities that this GameScene manages
            dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Debug.WriteLine(dt);

            // When this happens, there should be a "Are you sure you want to try quitting the battle?"
            var keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                _nextScene = GameSceneLink.LOpenWorld;  // Return to open world
                return;
            }

            Field.Update(Controller.GetPressed());
        }

        public override void Draw()
        {
            Field.Draw();
        }

        public override void SetNextScene()
        {
            base.SetNextScene();
            GameSceneLink next = _nextScene;
        }
    }
}
