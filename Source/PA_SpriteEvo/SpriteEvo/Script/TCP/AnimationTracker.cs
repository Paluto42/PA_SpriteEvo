using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
#if DEBUG_BUILD
    public class AnimationTracker //好像根本不用存档
    {
        public GameObject instanceInt;

        public List<AnimationController_Base> allAnimationControllers = new();

        public AnimationTracker() 
        {
        }

        public AnimationTracker(GameObject instance)
        {
            this.instanceInt = instance;
            if (instance == null) return;
            var controllers = instanceInt.GetComponents<AnimationController_Base>();
            foreach (var controller in controllers) 
            {
                allAnimationControllers.Add(controller);
                Log.Message("AnimationTracker获取到Controller脚本");
            }
        }

        public virtual void Tick() 
        {
            foreach (var controller in allAnimationControllers) 
            {
                controller?.ControllerTick();
            }
        }
    }
    #endif
}
