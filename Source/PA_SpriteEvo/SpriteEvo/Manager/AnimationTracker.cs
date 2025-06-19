using UnityEngine;

namespace SpriteEvo
{
    public class AnimationTracker //好像根本不用存档
    {
        public GameObject instanceInt;

        public ControllerBase<MonoBehaviour>[] controllers;

        public AnimationTracker(GameObject instance)
        {
            this.instanceInt = instance;
            if (instance == null) return;
            controllers = instanceInt.GetComponents<ControllerBase<MonoBehaviour>>();
        }
    }
}
