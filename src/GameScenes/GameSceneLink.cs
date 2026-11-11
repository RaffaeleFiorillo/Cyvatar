using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src.GameScenes
{
    /// <summary>
    /// The GameSceneManager uses this Links to know which GameScene to display next.
    /// </summary>
    public enum GameSceneLink
    {
        /// <summary>
        /// Link for the first GameScene, which is responsible for displaying the MainMenu.
        /// </summary>
        LMainMenu,

        /// <summary>
        /// Link to a GameScene where the player faces one or more foes in a battle.
        /// </summary>
        LBattleScene,

        /// <summary>
        /// Link for the GameScene where the player is free to roam around the WorldMap persuing his adventure.
        /// </summary>
        LOpenWorld,
    }
}
