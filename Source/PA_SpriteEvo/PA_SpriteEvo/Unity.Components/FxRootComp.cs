using UnityEngine;
using Verse;

namespace PA_SpriteEvo.Unity
{
    //控制坐标更新和旋转方向的Root物件
    public class FxRootComp : MonoBehaviour
    {
        #region Inspector
        public Thing User { get; set; }
        //不建议使用一切与Find有关的方法获取它们，而是加上组件后直接用SetChildren设置
        public FxHeadComp FxHeadController { get; set; }
        public FxBodyComp FxBodyController { get; set; }
        public FxExtraComp FxExtraController { get; set; }
        #endregion
        public bool CanDrawNow => Current.ProgramState == ProgramState.Playing;
        GameObject Root => base.gameObject;
        GameObject FxHeadChild => FxHeadController?.gameObject;
        GameObject FxBodyChild => FxBodyController?.gameObject;
        GameObject FxExtraChild => FxExtraController?.gameObject;

        public virtual void DoRotation(Rot4 rot)
        {
            FxHeadController?.DoRotation(rot);
            FxBodyController?.DoRotation(rot);
            FxExtraController?.DoRotation(rot);
        }
        public virtual void DoMove()
        {
            if (Root == null || User == null) return;
            Root.transform.position = User.DrawPos;
        }
        public virtual void Awake()
        {
        }
        public virtual void OnEnable()
        {
        }
        public virtual void Start()
        {
        }
        public virtual void FixedUpdate()
        {
        }
        public virtual void Update()
        {
            if (!CanDrawNow) return;
            if (User == null) return;
            FxHeadChild?.SetActive(true);
            FxBodyChild?.SetActive(true);
            FxExtraChild?.SetActive(true);
            DoMove();
            DoRotation(User.Rotation);
        }
        public virtual void LateUpdate()
        {
        }
        public virtual void OnGUI()
        {
        }
        public virtual void OnDisable()
        {
            FxHeadChild?.SetActive(false);
            FxBodyChild?.SetActive(false);
            FxExtraChild?.SetActive(false);
        }
        public virtual void OnDestory()
        {
        }
    }
}
