using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Cyvatar.src.Weapons.BlasterAttacks
{
    public abstract class Bullet: Damager
    {
        // Current animation frame
        public float CurrentFrame { get; set; } = 0;

        protected Bullet(Vector2 playerCoo, int blasterLevel)
        {
            Coo = new Vector2(playerCoo.X+180 + 30*blasterLevel, playerCoo.Y + 85);
        }

        // Each child must implement these methods
        public abstract Texture2D GetFrame();

        // Draw bullet using MonoGame's SpriteBatch
        public override void Draw()
        {
            GraphicManager.Screen.Draw(GetFrame(), Coo, Color.White);
        }
    }

    public class BoltBullet : Bullet
    {
        public const string Name = "Bolt";
        private static string _framesPath = Path.Combine("Battle", "Weapons", "Blaster", "Bolt");
        private static readonly List<Texture2D> Frames = Enumerable.Range(0, 5).Select(
            i => GameInterfaceManager.LoadImage(Path.Combine(_framesPath, $"{i}.png"), scale: 2.0f)
        ).ToList();
        private static int Speed = 300;      // Speed of the Bolt (pixel/Second)
        private static int FrameSpeed = 25;  // Time between each frame of the Bolt animation (Frame/Second)
        public static int EnergyCost = 30;  // Energy consumed by shooting a Bolt (Jolts)
        public static float CooldownTime = 3f; // Time to wait before the next Bolt can be shot (Seconds)

        public BoltBullet(Vector2 playerCoo, int blasterLevel) : base(playerCoo, blasterLevel) {}

        public override Texture2D GetFrame()
        {
            return Frames[(int)CurrentFrame];
        }

        public override void Update()
        {
            CurrentFrame = (CurrentFrame + FrameSpeed * BattleScene.dt) % Frames.Count;
            Coo = new Vector2(Coo.X + Speed*BattleScene.dt, Coo.Y);
        }
    }

    public class ElectronBullet : Bullet
    {
        public const string Name = "Electron";
        private static string _framesPath = Path.Combine("Battle", "Weapons", "Blaster", "Electron");
        private static readonly List<Texture2D> Frames = Enumerable.Range(0, 6).Select(
            i => GameInterfaceManager.LoadImage(Path.Combine(_framesPath, $"{i}.png"), scale: 2.0f)
        ).ToList();
        private static int Speed = 1000;      // Speed of the Bolt (pixel/Second)
        private static int FrameSpeed = 20;  // Time between each frame of the Bolt animation (Frame/Second)
        public static int EnergyCost = 1;  // Energy consumed by shooting a Bolt (Jolts)
        public static float CooldownTime = 0.06f; // Time to wait before the next Bolt can be shot (Seconds)

        public ElectronBullet(Vector2 playerCoo, int blasterLevel) : base(playerCoo, blasterLevel) { }

        public override Texture2D GetFrame()
        {
            return Frames[(int)CurrentFrame];
        }

        public override void Update()
        {
            CurrentFrame = (CurrentFrame + FrameSpeed * BattleScene.dt) % Frames.Count;
            Coo = new Vector2(Coo.X + Speed * BattleScene.dt, Coo.Y);
        }
    }

    public class QuantumBullet : Bullet
    {
        public const string Name = "Quantum";
        private static string _framesPath = Path.Combine("Battle", "Weapons", "Blaster", "Quantum");
        private static readonly List<Texture2D> Frames = Enumerable.Range(0, 7).Select(
            i => GameInterfaceManager.LoadImage(Path.Combine(_framesPath, $"{i}.png"), scale: 2.0f)
        ).ToList();
        private static int Speed = 300;      // Speed of the Bolt (pixel/Second)
        private static int FrameSpeed = 25;  // Time between each frame of the Bolt animation (Frame/Second)
        public static int EnergyCost = 30;  // Energy consumed by shooting a Bolt (Jolts)
        public static float CooldownTime = 3f; // Time to wait before the next Bolt can be shot (Seconds)

        public QuantumBullet(Vector2 playerCoo, int blasterLevel) : base(playerCoo, blasterLevel) { }

        public override Texture2D GetFrame()
        {
            return Frames[(int)CurrentFrame];
        }

        public override void Update()
        {
            CurrentFrame += FrameSpeed * BattleScene.dt;
            if (CurrentFrame < Frames.Count-1)
                Coo = new Vector2(Coo.X + Speed * BattleScene.dt, Coo.Y);
            else if (Random. Shared.NextDouble() > 0.75f)
            {
                // Quantum bullet can switch between 4 rows where Y = 550, 450, 350, 250
                // 
                int randomRow = Random.Shared.Next(0, 4);
                Coo = new Vector2(Coo.X + Speed * BattleScene.dt, randomRow*150 + 440);
                CurrentFrame = 0;
            }
            else
                CurrentFrame = 0;
        }
    }
}
