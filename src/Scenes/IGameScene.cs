using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src
{
    // 1. Interface that every screen must implement
    public interface IGameScene
    {
        string Link { get; }                     // Unique ID (ex: "MainMenu", "OpenWorld")
        void LoadContent();                      // Optional
        void Update(GameTime gameTime);          // Called every frame
        void Draw(SpriteBatch spriteBatch);      // Called every frame
        string GetNextScene();              // Returns next link or null if staying
    }
}
