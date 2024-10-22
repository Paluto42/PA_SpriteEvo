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

        //不建议使用一切与Find有关的方法获取它们，而是加上组件后直接用SetChildren设置
        private GameObject SouthChild;
        private GameObject NorthChild;
        private GameObject EastChild;
        public void SetChildren(GameObject s, GameObject n, GameObject e)
        {
            this.SouthChild = s;
            this.NorthChild = n;
            this.EastChild = e;
            HaveAllRotNode = true;
        }
        //组件被添加后立刻获取根节点FxRoot组件对象
        void Awake() 
        {
            Comp_FxRoot = transform.parent?.gameObject?.GetComponent<FxRootComp>();
        }
        void OnEnable() 
        {
            CanDrawNow = true;
        }
        void OnDisable() 
        {
            CanDrawNow = false;
        }
        void OnDestory()
        {
            CanDrawNow = false;
        }
        //Once预渲染操作
        void Start()
        {
            CanDrawNow = Current.ProgramState == ProgramState.Playing;
            SouthChild?.SetActive(false);
            NorthChild?.SetActive(false);
            EastChild?.SetActive(false);
        }
        void Update()
        {
            if (!CanDrawNow) return;
            if (!HaveAllRotNode) return;
            if (Caster == null) return;

            switch (Caster.Rotation.AsInt)
            {
                case 0:
                    SouthChild.SetActive(true);
                    NorthChild.SetActive(false);
                    EastChild.SetActive(false);
                    break;
                case 1:
                    EastChild.SetActive(true);
                    SouthChild.SetActive(false);
                    NorthChild.SetActive(false);
                    break;
                case 2:
                    NorthChild.SetActive(true);
                    SouthChild.SetActive(false);
                    EastChild.SetActive(false);
                    break;
                case 3:
                    EastChild.SetActive(true);
                    SouthChild.SetActive(false);
                    NorthChild.SetActive(false);
                    break;
                default:
                    Log.Error("ToQuat with Rot = " + Caster.Rotation.AsInt);
                    break;
            }
        }
        public virtual IEnumerator HeadAnimationController()
        {
            yield return null;
        }
    }
}
