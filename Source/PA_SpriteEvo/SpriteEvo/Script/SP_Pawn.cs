using Spine42;
using Spine42.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpriteEvo
{
    public class ASP_PawnBase : ScriptProperties
    {
        public ASP_PawnBase()
        {
            scriptClass = typeof(AS_PawnBase);
        }
    }

    public class AS_PawnBase : ScriptBase 
    {
        #region Unity
        Color clearColor = Color.clear;

        ISkeletonComponent skeletonComp;
        IAnimationStateComponent animationStateComp;
        #endregion

        int count = 0;

        float blinkMixIn = 0.4f;
        float blinkMixOut = 0.833f;

        public override void OnEnable() 
        {
            skeletonComp ??= GetComponent<ISkeletonComponent>();
            animationStateComp ??= GetComponent<IAnimationStateComponent>();

            var list = skeletonComp.Skeleton.Slots.FindAll(slot => slot.Data.Name.StartsWith("S_") || slot.Data.Name.StartsWith("B_"));
            foreach (var slot in list)
            {
                slot.SetColor(clearColor);
                Debug.Log(slot.Data.Name);
            }

            TrackEntry track0 = animationStateComp.AnimationState.AddAnimation(0, "idle", false, 0);
            track0.Complete += CompleteEventHandler;
        }

        private void CompleteEventHandler(TrackEntry trackEntry)
        {
            count++;
            if (count == 1)
            {
                TrackEntry track1 = animationStateComp.AnimationState.AddAnimation(1, "blink", false, blinkMixIn);
                track1.Complete += CompleteEventHandler;
                return;
            }
            if (count >= 2)
            {
                count = 0;
                TrackEntry track1 = animationStateComp.AnimationState.AddAnimation(0, "idle", false, 0);
                track1.Complete += CompleteEventHandler;
                TrackEntry track2 = animationStateComp.AnimationState.AddAnimation(1, "idle", false, blinkMixOut);
            }
        }
    }
}
