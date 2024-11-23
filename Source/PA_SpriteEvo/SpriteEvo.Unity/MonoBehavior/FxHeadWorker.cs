using RimWorld;
using System.Collections;
using UnityEngine;
using Verse;

namespace SpriteEvo.Unity
{
    //切记切记 一旦FxHeadComp组件被添加，就会自动获取上级FxRoot节点
    public class FxHeadWorker : BaseControllComp
    {
        //在Unity编辑器里直接使用需要把属性换成字段
        #region Inspector
        public string version = "3.8";
        public bool IsFilpX = false;
        public GameObject SouthChild { get; set; }
        public GameObject NorthChild { get; set; }
        public GameObject WestChild { get; set; }
        public GameObject EastChild { get; set; }
        #endregion
        public bool CanDrawNow => Current.ProgramState == ProgramState.Playing;
        private FxRootWorker Comp_FxRoot { get; set; }
        private FacialControllWorker SouthControllerComp { get; set; }
        private FacialControllWorker EastControllerComp { get; set; }
        private FacialControllWorker NorthControllerComp { get; set; }
        private FacialControllWorker WestControllerComp { get; set; }
        Pawn User => (Pawn)Comp_FxRoot.User;

        public virtual IEnumerator HeadAnimationController()
        {
            yield return null;
        }
        public virtual void DoBlink() 
        {
        }
        public virtual void DoFilpX(bool IsFlip) 
        {
            //第一种方法全部手动FlipX
            //EastControllerComp?.DoFlipX(IsFlip);
            //第二种方法 直接Transform旋转180 我感觉这个更靠谱
            gameObject.transform.localRotation = IsFlip ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }
        public virtual void DoRotation(Rot4 rot) 
        {   //第二种方法 使用SetAnimation 直接Reload
            //第一种方法 生成3面的物件逐个开关
            switch (rot.AsInt)
            {
                //后
                case 0:
                    NorthChild?.SetActive(true);
                    SouthChild?.SetActive(false);
                    WestChild?.SetActive(false);
                    EastChild?.SetActive(false);
                    break;
                //右
                case 1:
                    EastChild?.SetActive(true);
                    DoFilpX(false);
                    WestChild?.SetActive(false);
                    SouthChild?.SetActive(false);
                    NorthChild?.SetActive(false);
                    break;
                //前
                case 2:
                    SouthChild?.SetActive(true);
                    NorthChild?.SetActive(false);
                    WestChild?.SetActive(false);
                    EastChild?.SetActive(false);
                    break;
                //左
                case 3:
                    if (WestChild == null)
                    {
                        EastChild?.SetActive(true);
                        DoFilpX(true);
                    }
                    else
                    {
                        WestChild?.SetActive(true);
                        EastChild?.SetActive(false);
                    }
                    SouthChild?.SetActive(false);
                    NorthChild?.SetActive(false);
                    break;
                default:
                    Log.Error("ToQuat with Rot = " + rot.AsInt);
                    break;
            }
        }
        //组件被添加后立刻获取根节点FxRoot组件对象
        public override void Awake()
        {
            //Comp_FxRoot = transform.parent?.gameObject?.GetComponent<FxRootComp>();
        }
        public override void OnEnable()
        {
        }
        // Start is called before the first frame update
        public override void Start()
        {
            Comp_FxRoot = transform.parent?.gameObject?.GetComponent<FxRootWorker>();
            SouthControllerComp = SouthChild?.GetComponent<FacialControllWorker>();
            EastControllerComp = EastChild?.GetComponent<FacialControllWorker>();
            NorthControllerComp = NorthChild?.GetComponent<FacialControllWorker>();
            WestControllerComp = WestChild?.GetComponent<FacialControllWorker>();
        }
        public override void FixedUpdate()
        {
        }
        // Update is called once per frame
        public override void Update()
        {
            if (!CanDrawNow) return;
            if (User == null) return;
            DoRotation(User.Rotation);
        }
        public override void LateUpdate()
        {
        }
        public override void OnGUI()
        {
        }
        public override void OnDisable()
        {
            SouthChild?.SetActive(false);
            NorthChild?.SetActive(false);
            WestChild?.SetActive(false);
            EastChild?.SetActive(false);
        }
        public override void OnDestory()
        {
        }
    }
}
