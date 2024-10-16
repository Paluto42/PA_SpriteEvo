using Spine38.Unity;
using System.Collections;
using UnityEngine;

namespace PA_SpriteEvo
{
    public class ParallelAnimation : MonoBehaviour
    {
        #region
        public string Animation_0 = "Idle";
        public string Animation_1 = "Interact";
        public string Animation_2 = "Special";
        public string Animation_3;
        public string Animation_4;
        public string Animation_5;
        public string Animation_6;
        public string Animation_7;
        public string Animation_8;
        #endregion
        float timer_a0;
        float timer_a1;
        float timer_a2;
        float timer_a3;
        float timer_a4;
        float timer_a5;
        float timer_a6;
        float timer_a7;
        float timer_a8;

        SkeletonAnimation skeletonAnimation;

        void Start()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            GetAnimationsDuration();
            StartCoroutine(ParallelRoutine());
        }

        private void GetAnimationsDuration()
        {
            timer_a0 = skeletonAnimation.SkeletonDataAsset.GetSkeletonData(false).FindAnimation(Animation_0).Duration;
            timer_a1 = skeletonAnimation.SkeletonDataAsset.GetSkeletonData(false).FindAnimation(Animation_1).Duration;
            timer_a2 = skeletonAnimation.SkeletonDataAsset.GetSkeletonData(false).FindAnimation(Animation_2).Duration;
        }

        IEnumerator ParallelRoutine()
        {
            skeletonAnimation.AnimationState.SetAnimation(0, Animation_0, true);

            while (true)
            {
                yield return new WaitForSeconds(timer_a0);
                skeletonAnimation.AnimationState.SetAnimation(1, Animation_1, false);
                yield return new WaitForSeconds(timer_a1);
                skeletonAnimation.AnimationState.SetAnimation(1, Animation_0, false);

                yield return new WaitForSeconds(timer_a0);
                skeletonAnimation.AnimationState.SetAnimation(1, Animation_2, false);
                yield return new WaitForSeconds(timer_a2);
                skeletonAnimation.AnimationState.SetAnimation(1, Animation_0, false);
            }

        }
    }
}
