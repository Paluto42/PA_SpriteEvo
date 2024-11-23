using UnityEngine;
using Verse;

namespace SpriteEvo.Unity
{
    //控制坐标更新和旋转方向的Root物件
    public class FxRootWorker : BaseControllComp
    {
        //在Unity编辑器里直接使用需要把属性换成字段
        #region Inspector
        public Thing User { get; set; }
        //不建议使用一切与Find有关的方法获取它们，而是加上组件后直接用SetChildren设置
        public FxHeadWorker FxHeadController { get; set; }
        public FxBodyWorker FxBodyController { get; set; }
        public FxExtraWorker FxExtraController { get; set; }
        #endregion
        public bool CanDrawNow => Current.ProgramState == ProgramState.Playing;
        GameObject Root => base.gameObject;
        GameObject FxHeadChild => FxHeadController?.gameObject;
        GameObject FxBodyChild => FxBodyController?.gameObject;
        GameObject FxExtraChild => FxExtraController?.gameObject;

        /*public virtual void DoRotation(Rot4 rot)
        {
            FxHeadController?.DoRotation(rot);
            FxBodyController?.DoRotation(rot);
            FxExtraController?.DoRotation(rot);
        }*/
        public virtual void DoMove()
        {
            if (Root == null || User == null) return;
            Root.transform.position = User.DrawPos;
        }
        public override void Awake()
        {
        }
        public override void OnEnable()
        {
            FxHeadChild?.SetActive(true);
            FxBodyChild?.SetActive(true);
            FxExtraChild?.SetActive(true);
        }
        // Start is called before the first frame update
        public override void Start()
        {
        }
        public override void FixedUpdate()
        {
        }
        // Update is called once per frame
        public override void Update()
        {
            if (!CanDrawNow) return;
            if (User == null) return;
            DoMove();
        }
        public override void LateUpdate()
        {
        }
        public override void OnGUI()
        {
        }
        public override void OnDisable()
        {
            FxHeadChild?.SetActive(false);
            FxBodyChild?.SetActive(false);
            FxExtraChild?.SetActive(false);
        }
        public override void OnDestory()
        {
        }
    }
}
