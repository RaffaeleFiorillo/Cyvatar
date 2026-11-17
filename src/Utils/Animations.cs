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
    public abstract class Animation
    {
        protected List<Texture2D> _frames = new();
        public int CurrentFrame { get => (int)_currentFrame; }
        protected float _currentFrame;
        protected readonly float _frameSpeed;
        public readonly int FrameCount;
        protected readonly string _framesPath;

        protected Vector2 Coo { get; set; }

        public Animation(
            string framesPath,
            Vector2 initialPosition,
            int framesCount,
            float frameSpeed,
            float imageScale
            )
        {
            _framesPath = framesPath;
            FrameCount = framesCount;
            _frameSpeed = frameSpeed;
            Coo = initialPosition;

            _frames = Enumerable.Range(0, FrameCount).Select(
                i => GameSceneManager.LoadImage(Path.Combine(_framesPath, $"{i}.png"), scale: imageScale)
            ).ToList();
            Reset();
        }

        public virtual void Reset()
        {
            _currentFrame = 0f;
        }

        public abstract void Update(float dt);

        public void Draw()
        {
            GraphicManager.Screen.Draw(_frames[CurrentFrame], Coo, Color.White);
        }
    }

    public class LoopAnimatioon: Animation
    {
        private bool _firstLoop = true;

        public LoopAnimatioon
            (
                string framesPath,
                Vector2 initialPosition,
                int framesCount,
                float frameSpeed = 10,
                float imageScale = 1
            ) : base(framesPath, initialPosition, framesCount, frameSpeed, imageScale)
        {}

        public override void Update(float dt)
        {
            // Avoid displaying the empty frames (0 and 1) after the first loop
            if (!_firstLoop && _currentFrame < 2f)
                _currentFrame = 2f;
            // After frame 2 we consider the animation past the first loop
            else if (_currentFrame > 2f)
                _firstLoop = false;

            // Advance animation withing the boundaries of the existing frames
            _currentFrame = (_currentFrame + _frameSpeed * dt) % _frames.Count;
        }

        public override void Reset()
        {
            base.Reset();
            _firstLoop = true;
        }
    }

    public abstract class SimpleAnimation: Animation
    {
        public bool HasEnded = true;
        protected float LastFrameWaitingTime = 0;
        protected float LastFrameTotalWaitingTime;

        public SimpleAnimation(
            string framesPath,
            Vector2 initialPosition,
            int framesCount,
            float frameSpeed=10,
            float lastFrameWaitingTime=2,
            float imageScale=1
            ) : base(framesPath, initialPosition, framesCount, frameSpeed, imageScale)
        {
            LastFrameTotalWaitingTime = lastFrameWaitingTime;
            Reset();
        }

        public override void Update(float dt)
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

        public override void Reset()
        {
            base.Reset();
            LastFrameWaitingTime = LastFrameTotalWaitingTime;
            HasEnded = false;
        }
    }
}
