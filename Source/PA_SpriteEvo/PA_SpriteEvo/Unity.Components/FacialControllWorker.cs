using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    //绑在头部的面部控件
    public class FacialControllWorker : BaseControllWorker
    {
        //在Unity编辑器里直接使用需要把属性换成字段
        #region Inspector
        public Rot4 rot;
        public GameObject FrontHair { get; set; }
        public GameObject BackHair { get; set; }
        public GameObject Eyebow { get; set; }
        public GameObject LeftEye { get; set; }
        public GameObject RightEye { get; set; }
        public GameObject Mouth { get; set; }
        #endregion
        private float ScaleX { get; set; } = 1f;

        Spine41.Unity.SkeletonAnimation head_skeletonAnimation;
        Spine41.Unity.SkeletonAnimation frontHair_skeletonAnimation;
        Spine41.Unity.SkeletonAnimation backHair_skeletonAnimation;
        Spine41.Unity.SkeletonAnimation eyeBow_skeletonAnimation;
        Spine41.Unity.SkeletonAnimation leftEye_skeletonAnimation;
        Spine41.Unity.SkeletonAnimation rightEye_skeletonAnimation;
        Spine41.Unity.SkeletonAnimation mouth_skeletonAnimation;

        //这个方法只能给左右脸用！
        public void DoFlipX(bool IsFlip) 
        {
            ScaleX = IsFlip ? -1f : 1f;
            head_skeletonAnimation?.skeleton.SetScaleX(ScaleX);
            frontHair_skeletonAnimation?.skeleton.SetScaleX(ScaleX);
            backHair_skeletonAnimation?.skeleton.SetScaleX(ScaleX);
            eyeBow_skeletonAnimation?.skeleton.SetScaleX(ScaleX);
            leftEye_skeletonAnimation?.skeleton.SetScaleX(ScaleX);
            rightEye_skeletonAnimation?.skeleton.SetScaleX(ScaleX);
            mouth_skeletonAnimation?.skeleton.SetScaleX(ScaleX);
        }
        public virtual IEnumerator EyesAnimationCoroutine()
        {
            while (true) 
            {
                yield return new WaitForSeconds(1);
                leftEye_skeletonAnimation?.AnimationState.SetAnimation(0, "Blink", false);
                rightEye_skeletonAnimation?.AnimationState.SetAnimation(0, "Blink", false);

                yield return new WaitForSeconds(1);
                leftEye_skeletonAnimation?.AnimationState.SetAnimation(0, "Blink", false);

                yield return new WaitForSeconds(1);
                rightEye_skeletonAnimation?.AnimationState.SetAnimation(0, "Blink", false);
            }
        }
        public override void Awake()
        {
            head_skeletonAnimation = gameObject.GetComponent<Spine41.Unity.SkeletonAnimation>();
        }
        public override void OnEnable()
        {
            FrontHair?.SetActive(true);
            BackHair?.SetActive(true);
            Eyebow?.SetActive(true);
            LeftEye?.SetActive(true);
            RightEye?.SetActive(true);
            Mouth?.SetActive(true);
            StartCoroutine(EyesAnimationCoroutine());
        }
        // Start is called before the first frame update
        public override void Start()
        {
            frontHair_skeletonAnimation = FrontHair?.GetComponent<Spine41.Unity.SkeletonAnimation>();
            backHair_skeletonAnimation = BackHair?.GetComponent<Spine41.Unity.SkeletonAnimation>();
            eyeBow_skeletonAnimation = Eyebow?.GetComponent<Spine41.Unity.SkeletonAnimation>();
            leftEye_skeletonAnimation = LeftEye?.GetComponent<Spine41.Unity.SkeletonAnimation>();
            rightEye_skeletonAnimation = RightEye?.GetComponent<Spine41.Unity.SkeletonAnimation>();
            mouth_skeletonAnimation = Mouth?.GetComponent<Spine41.Unity.SkeletonAnimation>();
            //StartCoroutine(EyesAnimationCoroutine());
        }
        // Update is called once per frame
        public override void Update()
        {
        }
        public override void OnDisable()
        {
            StopAllCoroutines();
            FrontHair?.SetActive(false);
            BackHair?.SetActive(false);
            Eyebow?.SetActive(false);
            LeftEye?.SetActive(false);
            RightEye?.SetActive(false);
            Mouth?.SetActive(false);
        }

    }
}
