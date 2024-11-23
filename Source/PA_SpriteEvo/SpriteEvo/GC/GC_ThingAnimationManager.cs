using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    //这个GC管理所有Thing的动画运行时实例
    public class GC_ThingAnimationManager : GameComponent
    {
        public static Dictionary<object, GameObject> thingDoc = new();

        public static HashSet<Thing> cachedThings = new();
        public GC_ThingAnimationManager(Game game)
        {
        }
        public override void StartedNewGame() 
        {
            base.StartedNewGame();
        }

        public override void FinalizeInit() { }

        public override void LoadedGame() 
        {
            base.LoadedGame();
        }
        public override void GameComponentTick() { }

        public override void ExposeData() 
        {
            base.ExposeData();
        }



    }
}
