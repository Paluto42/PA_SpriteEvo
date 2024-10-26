using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PA_SpriteEvo
{
    public class FacialControllerComp : BaseControllerComp
    {
        #region Inspector
        public GameObject FrontHair { get; set; }
        public GameObject BackHair { get; set; }
        public GameObject Eyebow { get; set; }
        public GameObject LeftEye { get; set; }
        public GameObject RightEye { get; set; }
        public GameObject Mouth { get; set; }
        #endregion
        Spine41.Unity.SkeletonAnimation leftEye_skeletonAnimation;
        Spine41.Unity.SkeletonAnimation rightEye_skeletonAnimation;

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
        public override void OnEnable()
        {
            FrontHair?.SetActive(true);
            BackHair?.SetActive(true);
            Eyebow?.SetActive(true);
            LeftEye?.SetActive(true);
            RightEye?.SetActive(true);
            Mouth?.SetActive(true);
        }
        // Start is called before the first frame update
        public override void Start()
        {
            leftEye_skeletonAnimation = LeftEye?.GetComponent<Spine41.Unity.SkeletonAnimation>();
            rightEye_skeletonAnimation = RightEye?.GetComponent<Spine41.Unity.SkeletonAnimation>();
            StartCoroutine(EyesAnimationCoroutine());
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
