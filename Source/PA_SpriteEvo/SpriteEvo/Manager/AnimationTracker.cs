using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
#if DEBUG_BUILD
    public class AnimationTracker //好像根本不用存档
    {
        public GameObject instanceInt;

        public List<AnimationControllerBase<MonoBehaviour, MonoBehaviour>> allAnimationControllers = new();

        public AnimationTracker() 
        {
        }

        public AnimationTracker(GameObject instance)
        {
            this.instanceInt = instance;
            if (instance == null) return;
            var controllers = instanceInt.GetComponents<AnimationControllerBase<MonoBehaviour, MonoBehaviour>>();
            foreach (var controller in controllers) 
            {
                allAnimationControllers.Add(controller);
                Log.Message("AnimationTracker获取到Controller脚本");
            }
        }
    }
    #endif
}
