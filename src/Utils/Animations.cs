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

        /// <summary>
        /// This must be internally set to true when the animation reaches an end (if not a loop).
        /// </summary>
        public bool HasEnded = false;
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

        public abstract void UpdateCoordinates(Vector2 coo);

        public abstract void Update(float dt);

        public void Draw()
        {
            GraphicManager.Screen.Draw(_frames[CurrentFrame], Coo, Color.White);
        }
    }

    public class LoopAnimation: Animation
    {
        private bool _firstLoop = true;

        public LoopAnimation
            (
                string framesPath,
                Vector2 initialPosition,
                int framesCount,
                float frameSpeed = 10,
                float imageScale = 1
            ) : base(framesPath, initialPosition, framesCount, frameSpeed, imageScale)
        {}

        public override void UpdateCoordinates(Vector2 coo) { }

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
        protected float LastFrameWaitingTime = 0;
        protected readonly float LastFrameTotalWaitingTime;

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

    /// <summary>
    /// This class implements an animation where the frames go from start to end and then go backwards from the end to the start.
    /// It is possible to customize a pausing interval, before the animation goes backwards.
    /// </summary>
    public abstract class BoomerangAnimation : Animation
    {
        protected float LastFrameWaitingTime = 0;
        protected readonly float LastFrameTotalWaitingTime;
        public bool GoingForward = true;
        public bool AtMiddlePoint = false;

        public BoomerangAnimation(
            string framesPath,
            Vector2 initialPosition,
            int framesCount,
            float frameSpeed=10,
            float lastFrameWaitingTime=0,
            float imageScale = 1
        ) : base(framesPath, initialPosition, framesCount, frameSpeed, imageScale)
        {
            LastFrameTotalWaitingTime = lastFrameWaitingTime;
            Reset();
        }

        public override void Update(float dt)
        {
            if (HasEnded)
                return;

            if (GoingForward)
            {
                _currentFrame += _frameSpeed * dt;

                if (_currentFrame >= FrameCount - 1)
                {
                    _currentFrame = FrameCount - 1;
                    GoingForward = false;
                }
                return;
            }

            AtMiddlePoint = true;

            LastFrameWaitingTime -= dt;
            if (LastFrameWaitingTime <= 0)
            {
                AtMiddlePoint = false;
                _currentFrame -= _frameSpeed * dt;

                if (_currentFrame <= 0)
                {
                    _currentFrame = 0;
                    HasEnded = true;
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            GoingForward = true;
            AtMiddlePoint = false;
            LastFrameWaitingTime = LastFrameTotalWaitingTime;
            HasEnded = false;
        }
    }
}
