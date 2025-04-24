using Spine42.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SpriteEvo
{
    public class AnimationController_Base : ScriptBase
    {
        //绘制相对深度(高度)
        public float drawDepth = 1;

        //ref
        #region
        public ISkeletonComponent skeletonInt;
        public IAnimationStateComponent animationStateInt;
        #endregion

        protected override void Awake()
        {
            skeletonInt ??= GetComponent<ISkeletonComponent>();
            animationStateInt ??= GetComponent<IAnimationStateComponent>();
        }

        public virtual void ControllerTick() 
        {
        }
    }
}
