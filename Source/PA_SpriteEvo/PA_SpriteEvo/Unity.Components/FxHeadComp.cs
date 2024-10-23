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
        }
        void Update()
        {
            if (!CanDrawNow) return;
            if (!HaveAllRotNode) return;
            if (Caster == null) return;
        }
        public virtual IEnumerator HeadAnimationController()
        {
            yield return null;
        }
    }
}
