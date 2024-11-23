using Spine41.Unity;
using System.Collections;
using UnityEngine;

namespace SpriteEvo.Unity
{
    //测试连播动态立绘用
    public class ParallelAnimation_Spine41 : MonoBehaviour
    {
        #region Inspector
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

        Spine41.Unity.SkeletonAnimation skeletonAnimation;

        void Start()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            StartCoroutine(Routine());
        }

        //协程
        public virtual IEnumerator Routine()
        {
            skeletonAnimation.AnimationState.SetAnimation(0, Animation_0, true);

            while (true)
            {
                yield return new WaitForSeconds(Random.Range(0.5f, 3f));
                skeletonAnimation.AnimationState.SetAnimation(1, Animation_1, false);

                yield return new WaitForSeconds(Random.Range(0.5f, 3f));
                skeletonAnimation.AnimationState.SetAnimation(1, Animation_2, false);
            }

        }
    }
}
