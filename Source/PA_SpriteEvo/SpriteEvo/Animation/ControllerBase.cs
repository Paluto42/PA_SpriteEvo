using System.Collections;
using UnityEngine;

namespace SpriteEvo
{
    //多版本兼容 要用哪个版本就声明对应版本的类
    public class ControllerBase<TSkeletonInstance> : ScriptBase where TSkeletonInstance : MonoBehaviour
    {
        //绘制相对深度(高度)
        public float drawDepth = 1;

        #region
        protected virtual TSkeletonInstance SkeletonInstanceInt { get; private set; } //ref
        #endregion

        protected override void Awake()
        {
            SkeletonInstanceInt ??= GetComponent<TSkeletonInstance>();
        }

        protected override void OnEnable()
        {
            if (SkeletonInstanceInt == null) return;
            //StartCoroutine(AnimationQueueHandler());
        }

        public virtual IEnumerator AnimationQueueHandler()
        {
            return null;
        }
    }
}
