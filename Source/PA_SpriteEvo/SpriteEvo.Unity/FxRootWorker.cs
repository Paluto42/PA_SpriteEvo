using Spine41.Unity;
using System.Collections;
using UnityEngine;
using Verse;

namespace SpriteEvo.Unity
{
    //控制坐标更新和旋转方向的Root物件
    public class FxRootWorker : ScriptBase
    {
        //在Unity编辑器里直接使用需要把属性换成字段
        #region Inspector
        public Thing User { get; set; }
        //不建议使用一切与Find有关的方法获取它们，而是加上组件后直接用SetChildren设置
        /*public FxHeadWorker FxHeadController { get; set; }
        public FxBodyWorker FxBodyController { get; set; }
        public FxExtraWorker FxExtraController { get; set; }*/
        #endregion
        public bool CanDrawNow => Current.ProgramState == ProgramState.Playing;
        GameObject Root => base.gameObject;
        public SkeletonAnimation SkeletonAnimation { get; set; }
        /*GameObject FxHeadChild => FxHeadController?.gameObject;
        GameObject FxBodyChild => FxBodyController?.gameObject;
        GameObject FxExtraChild => FxExtraController?.gameObject;*/

        public override void Awake()
        {
        }
        public override void OnEnable()
        {
            /*FxHeadChild?.SetActive(true);
            FxBodyChild?.SetActive(true);
            FxExtraChild?.SetActive(true);*/
        }
        // Start is called before the first frame update
        public override void Start()
        {
        }
        public override void FixedUpdate()
        {
            CheckUserMap();
        }
        // Update is called once per frame
        public override void Update()
        {
            if (!CanDrawNow) return;
            if (User == null) return;
            DoMove();
            DoRot();
        }
        public override void LateUpdate()
        {
        }
        public override void OnGUI()
        {
        }
        public override void OnDisable()
        {
            /*FxHeadChild?.SetActive(false);
            FxBodyChild?.SetActive(false);
            FxExtraChild?.SetActive(false);*/
        }
        public override void OnDestory()
        {
        }
        //检查Pawn是否在地图上 因为切换地图不会触发自动回收 要禁用不在当前地图上的动画实例
        public virtual void CheckUserMap()
        {
            if (Root == null || User == null) return;
            if (User is Pawn P)
            {
                if (P.Map != Find.CurrentMap)
                    Root.SetActive(false);
                else
                    Root.SetActive(true);
            }
        }
        public virtual void DoMove()
        {
            if (Root == null || User == null || !Root.activeSelf) return;
            Root.transform.position = User.DrawPos;
        }
        //0:north 1:east 2:south 3:west 
        public virtual void DoRot() 
        {
            if (Root == null || User == null) return;
            Rot4 rot = User.Rotation;
            switch (rot.AsInt)
            {
                case 0:
                    SkeletonAnimation?.UpdateSkin("North");
                    SkeletonAnimation.DoFlipX(false);
                    break;
                //右 east
                case 1:
                    SkeletonAnimation?.UpdateSkin("East");
                    SkeletonAnimation.DoFlipX(false);
                    break;
                case 2:
                    SkeletonAnimation?.UpdateSkin("South");
                    SkeletonAnimation.DoFlipX(false);
                    break;
                case 3:
                    SkeletonAnimation?.UpdateSkin("East");
                    SkeletonAnimation.DoFlipX(true);
                    break;
                default:
                    Log.Error("ToQuat with Rot = " + rot.AsInt);
                    break;
            }
        }
    }
}
