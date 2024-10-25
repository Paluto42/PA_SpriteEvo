using UnityEngine;
using Verse;

namespace PA_SpriteEvo.Unity
{
    //控制坐标更新和旋转方向的Root物件
    public class FxRootComp : MonoBehaviour
    {
        #region Inspector
        public Thing User { get; set; }

        #endregion
        public bool CanDrawNow = false;
        GameObject Root => base.gameObject;
        //
        //不建议使用一切与Find有关的方法获取它们，而是加上组件后直接用SetChildren设置
        public FxHeadComp FxHeadChild { get; set; }
        public FxBodyComp FxBodyChild { get; set; }
        public FxExtraComp FxExtraChild { get; set; }

        //public GameObject SouthChild;
        //public GameObject NorthChild;
        //public GameObject EastChild;
        public void SetChildren(FxHeadComp h, FxBodyComp b, FxExtraComp e = null)
        {
            this.FxHeadChild = h;
            this.FxBodyChild = b;
            this.FxExtraChild = e;
        }
        public virtual void DoAllRotation(Rot4 rot)
        {
            FxHeadChild.DoRotation(rot);
            FxBodyChild?.DoRotation(rot);
            FxExtraChild.DoRotation(rot);
        }
        public virtual void DoTransform()
        {
            if (Root == null || User == null) return;
            Root.transform.position = User.DrawPos;
        }
        public virtual void Awake()
        {
        }
        public virtual void OnEnable()
        {
            CanDrawNow = true;
        }
        public virtual void Start()
        {
            CanDrawNow = Current.ProgramState == ProgramState.Playing;
            FxHeadChild?.gameObject?.SetActive(true);
            FxBodyChild?.gameObject?.SetActive(true);
            FxExtraChild?.gameObject?.SetActive(true);
        }
        public virtual void FixedUpdate()
        {
        }
        public virtual void Update()
        {
            if (!CanDrawNow) return;
            if (User == null) return;

            DoTransform();
            DoAllRotation(User.Rotation);
        }
        public virtual void LateUpdate()
        {
        }
        public virtual void OnGUI()
        {
        }
        public virtual void OnDisable()
        {
            CanDrawNow = false;
        }
        public virtual void OnDestory()
        {
            CanDrawNow = false;
        }
    }
}
