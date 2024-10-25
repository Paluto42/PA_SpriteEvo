using RimWorld;
using System.Collections;
using UnityEngine;
using Verse;

namespace PA_SpriteEvo.Unity
{
    //切记切记 一旦FxHeadComp组件被添加，就会自动获取上级FxRoot节点
    public class FxHeadComp : MonoBehaviour
    {
        #region Inspector
        public bool CanDrawNow = false;

        public string version = "3.8";
        #endregion
        bool HaveAllRotNode = false;

        FxRootComp Comp_FxRoot;
        Pawn Caster => (Pawn)Comp_FxRoot.User;

        public MonoBehaviour Attachment;

        public GameObject SouthChild;
        public GameObject NorthChild;
        public GameObject EastChild;

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
                case 0:
                    SouthChild?.SetActive(true);
                    NorthChild?.SetActive(false);
                    EastChild?.SetActive(false);
                    break;
                case 1:
                    EastChild?.SetActive(true);
                    SouthChild?.SetActive(false);
                    NorthChild?.SetActive(false);
                    break;
                case 2:
                    NorthChild?.SetActive(true);
                    SouthChild?.SetActive(false);
                    EastChild?.SetActive(false);
                    break;
                case 3:
                    EastChild?.SetActive(true);
                    SouthChild?.SetActive(false);
                    NorthChild?.SetActive(false);
                    break;
                default:
                    Log.Error("ToQuat with Rot = " + rot.AsInt);
                    break;
            }
        }
        public virtual void Awake()
        {
            Comp_FxRoot = transform.parent?.gameObject?.GetComponent<FxRootComp>();
        }
        public virtual void OnEnable()
        {
            CanDrawNow = true;
        }
        //Once预渲染操作
        public virtual void Start()
        {
            CanDrawNow = Current.ProgramState == ProgramState.Playing;
        }
        public virtual void FixedUpdate()
        {
        }
        public virtual void Update()
        {
            if (!CanDrawNow) return;
            if (!HaveAllRotNode) return;
            if (Caster == null) return;
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
