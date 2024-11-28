using Spine38.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteEvo.Unity
{
    //用来测试连播动态立绘的组件,记得删
    public class ParallelAnimation_Spine38 : MonoBehaviour
    {
        #region
        public List<string> Animation_Slots = new List<string>() 
        {
            "Idle",
            "Interact",
            "Special"
        };
        public List<Spine38.Animation> Animation_ss;
        #endregion
        List<float> durations = new List<float>();

        SkeletonAnimation skeletonAnimation;

        public virtual void Start()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            Animation_ss = skeletonAnimation.GetAllAnimations();
            for (int i = 0; i < Animation_ss.Count; i++)
            {
                durations[i] = Animation_ss[i].Duration;
            }
            StartCoroutine(ParallelRoutine());
        }

        private float GetAnimationsDuration(string name)
        {
            Spine38.Animation am = skeletonAnimation.SkeletonDataAsset.GetSkeletonData(false).FindAnimation(name);
            if (am == null)
            {
                return -1f;
            }
            return am.Duration;
        }

        public virtual IEnumerator ParallelRoutine()
        {
            skeletonAnimation.AnimationState.SetAnimation(0, Animation_Slots[0], true);

            while (true)
            {
                yield return new WaitForSeconds(durations[0]);
                skeletonAnimation.AnimationState.SetAnimation(1, Animation_Slots[1], false);
                yield return new WaitForSeconds(durations[1]);
                skeletonAnimation.AnimationState.SetAnimation(1, Animation_Slots[0], false);

                yield return new WaitForSeconds(durations[0]);
                skeletonAnimation.AnimationState.SetAnimation(1, Animation_Slots[2], false);
                yield return new WaitForSeconds(durations[2]);
                skeletonAnimation.AnimationState.SetAnimation(1, Animation_Slots[0], false);
            }

        }
    }
}
