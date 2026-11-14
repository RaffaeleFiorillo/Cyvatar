using Cyvatar.src.GameScenes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src.Utils.Animations
{
    public abstract class SimpleAnimation
    {
        protected List<Texture2D> _frames = new();
        protected float _currentFrame = 0;
        protected readonly float _frameSpeed;
        public readonly int FrameCount;
        protected readonly string _framesPath;

        public bool HasEnded = true;
        protected float LastFrameWaitingTime = 0;
        protected float LastFrameTotalWaitingTime;

        protected Vector2 Coo { get; set; }
        public int CurrentFrame { get => (int)_currentFrame; }

        public SimpleAnimation(
            string framesPath,
            Vector2 initialPosition,
            int framesCount,
            float frameSpeed=10,
            float lastFrameWaitingTime=2,
            float imageScale=1
            )
        {
            _framesPath = framesPath;
            FrameCount = framesCount;
            _frameSpeed = frameSpeed;
            Coo = initialPosition;
            LastFrameTotalWaitingTime = lastFrameWaitingTime;

            _frames = Enumerable.Range(0, FrameCount).Select(
                i => GameSceneManager.LoadImage(Path.Combine(_framesPath, $"{i}.png"), scale: imageScale)
            ).ToList();

            Reset();
        }

        public abstract void UpdateCoordinates(Vector2 position);

        public virtual void Update(float dt)
        {
            if (_currentFrame == FrameCount - 1)
            {
                LastFrameWaitingTime -= dt;
                // Only when the last frame has been displayed for a certain amount of time, the Animation is considered finished
                if (LastFrameWaitingTime <= 0)
                    HasEnded = true;
            }
            // If animation is ongoing, Advance animation
            else if (LastFrameWaitingTime == LastFrameTotalWaitingTime)
            {
                _currentFrame = _currentFrame + _frameSpeed * dt;
                if (_currentFrame > FrameCount - 1)
                    _currentFrame = FrameCount - 1;
            }
        }

        public virtual void Draw()
        {
            GraphicManager.Screen.Draw(_frames[CurrentFrame], Coo, Color.White);
        }

        public virtual void Reset()
        {
            _currentFrame = 0f;
            LastFrameWaitingTime = LastFrameTotalWaitingTime;
            HasEnded = false;
        }
    }
}
