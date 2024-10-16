using Spine38.Unity;
using System.Collections;
using UnityEngine;

namespace PA_SpriteEvo
{
    public class ParallelAnimation : MonoBehaviour
    {
        #region
        public string Animation_Slot0 = "Idle";
        public string Animation_Slot1 = "Interact";
        public string Animation_Slot2 = "Special";
        public string Animation_Slot3;
        public string Animation_Slot4;
        public string Animation_Slot5;
        public string Animation_Slot6;
        public string Animation_Slot7;
        public string Animation_Slot8;
        #endregion
        float duration_a0;
        float duration_a1;
        float duration_a2;
        float duration_a3;
        float duration_a4;
        float duration_a5;
        float duration_a6;
        float duration_a7;
        float duration_a8;

        SkeletonAnimation skeletonAnimation;

        void Start()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            GetAnimationsDuration();
            StartCoroutine(ParallelRoutine());
        }

        private void GetAnimationsDuration()
        {
            duration_a0 = skeletonAnimation.SkeletonDataAsset.GetSkeletonData(false).FindAnimation(Animation_Slot0).Duration;
            duration_a1 = skeletonAnimation.SkeletonDataAsset.GetSkeletonData(false).FindAnimation(Animation_Slot1).Duration;
            duration_a2 = skeletonAnimation.SkeletonDataAsset.GetSkeletonData(false).FindAnimation(Animation_Slot2).Duration;
        }

        IEnumerator ParallelRoutine()
        {
            skeletonAnimation.AnimationState.SetAnimation(0, Animation_Slot0, true);

            while (true)
            {
                yield return new WaitForSeconds(duration_a0);
                skeletonAnimation.AnimationState.SetAnimation(1, Animation_Slot1, false);
                yield return new WaitForSeconds(duration_a1);
                skeletonAnimation.AnimationState.SetAnimation(1, Animation_Slot0, false);

                yield return new WaitForSeconds(duration_a0);
                skeletonAnimation.AnimationState.SetAnimation(1, Animation_Slot2, false);
                yield return new WaitForSeconds(duration_a2);
                skeletonAnimation.AnimationState.SetAnimation(1, Animation_Slot0, false);
            }

        }
    }
}
