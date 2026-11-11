using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src.GameScenes
{
    /// <summary>
    /// The Game can be viewed as a sequence of what is called GameScenes.
    /// Each GameScene manages the current gameloop that the player experiences.
    /// As an example, going from the MainMenu to a NewGame means going from the GameScene that represents the MainMenu to the GameScene that represents the NewGame.
    /// Interface that describes a GameScene. A GameScene is a specific part of the game responsible for ma
    /// </summary>
    public abstract class GameScene
    {
        /// <summary>
        /// Unique ID given to the GameScene to be identified by the GameSceneManager.
        /// </summary>
        public GameSceneLink Link;

        /// <summary>
        /// Flag that informs if the GameScene has ended its cycle or not.
        /// When it ends, the GameSceneManager should go to the next GameScene.
        /// </summary>
        public bool HasEnded = false;

        protected readonly GraphicsDevice _graphicsDevice;

        public GameScene(GraphicsDevice graphicsDevice, GameSceneLink link) 
        {
            _graphicsDevice = graphicsDevice;
            Link = link; 
        }

        public abstract void LoadContent();                      // Optional
        public abstract void Update(GameTime gameTime);          // Called every frame
        public abstract void Draw();      // Called every frame
        public virtual void SetNextScene() 
        {
            HasEnded = true;
        } 
    }
}
