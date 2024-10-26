using RimWorld;
using System.Collections;
using UnityEngine;
using Verse;

namespace PA_SpriteEvo.Unity
{
    //切记切记 一旦FxHeadComp组件被添加，就会自动获取上级FxRoot节点
    public class FxHeadComp : BaseControllerComp
    {
        //在Unity编辑器里直接使用需要把属性换成字段
        #region Inspector
        public GameObject SouthChild { get; set; }
        public GameObject NorthChild { get; set; }
        public GameObject WestChild { get; set; }
        public GameObject EastChild { get; set; }

        public string version = "3.8";
        #endregion
        public bool CanDrawNow => Current.ProgramState == ProgramState.Playing;
        FxRootComp Comp_FxRoot { get; set; }
        Pawn User => (Pawn)Comp_FxRoot.User;

        public MonoBehaviour Attachment;

        //组件被添加后立刻获取根节点FxRoot组件对象
        public virtual MonoBehaviour GetAttachment() 
        {
            return null;
        }
        public virtual IEnumerator HeadAnimationController()
        {
            yield return null;
        }
        public virtual void DoRotation(Rot4 rot) 
        {
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
                    WestChild?.SetActive(true);
                    EastChild?.SetActive(false);
                    SouthChild?.SetActive(false);
                    NorthChild?.SetActive(false);
                    break;
                default:
                    Log.Error("ToQuat with Rot = " + rot.AsInt);
                    break;
            }
        }
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
            Comp_FxRoot = transform.parent?.gameObject?.GetComponent<FxRootComp>();
        }
        public override void FixedUpdate()
        {
        }
        // Update is called once per frame
        public override void Update()
        {
            if (!CanDrawNow) return;
            if (User == null) return;
            //DoRotation(User.Rotation);
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
