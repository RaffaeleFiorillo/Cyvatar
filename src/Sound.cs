using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace Cyvatar.src
{
    public class Sound
    {
        /// <summary>
        /// The Audio object that will be played.
        /// </summary>
        private SoundEffectInstance _sound;

        /// <summary>
        /// This represents the base volume of this Sound instance.
        /// By changing this value, the Sound's volume is automatically changed.
        /// Volume=0 means the sound is muted.
        /// Volume=1 means the sound is at 100%;
        /// </summary>
        public float Volume
        {
            get => _volume;
            set { _volume = value; _sound?.Volume = value; }
        }
        private float _volume = 1f;

        /// <summary>
        /// This represents the base volume of All Sound instances.
        /// By changing this value, all other Sound volumes are changed.
        /// MasterVolume=0 means all sounds are muted.
        /// MasterVolume=1 means all sounds are at 100%volume;
        /// </summary>
        public static float MasterVolume 
        { 
          get => _masterVolume;
          set { _masterVolume = value; SoundEffect.MasterVolume = value; } 
        }
        private static float _masterVolume = 1f;

        public Sound(string path, bool isLoop=false, float volume=1f)
        {
            string fullPath = Path.Combine("Content", "Audio", path);

            SoundEffect sound = SoundEffect.FromFile(fullPath);
            _sound = sound.CreateInstance();
            _sound.IsLooped = isLoop;

            Volume = volume;
        }

        public void Play(){_sound.Play();}

        public static void MuteAll(){MasterVolume=0;}

        public static void ResumeAll(){MasterVolume=1;}

        public void Stop() {_sound.Stop();}

        public void Pause() {_sound.Pause();}

        public void Resume() {_sound.Resume();}

        public void Dispose()
        {
            _sound.Stop();
            _sound.Dispose();
        }
    }
}
