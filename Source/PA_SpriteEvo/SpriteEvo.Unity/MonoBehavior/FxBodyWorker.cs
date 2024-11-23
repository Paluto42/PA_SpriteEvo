using SpriteEvo.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SpriteEvo.Unity
{
    //绑在身上的身体控件
    public class FxBodyWorker : BaseControllComp
    {
        //在Unity编辑器里直接使用需要把属性换成字段
        #region Inspector
        public GameObject SouthChild { get; set; }
        public GameObject NorthChild { get; set; }
        public GameObject WestChild { get; set; }
        public GameObject EastChild { get; set; }
        #endregion
        public bool CanDrawNow => Current.ProgramState == ProgramState.Playing;
        private FxRootWorker Comp_FxRoot { get; set; }
        Pawn User => (Pawn)Comp_FxRoot.User;

        public virtual void DoFilpX(bool IsFlip)
        {
            gameObject.transform.localRotation = IsFlip ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
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
        public virtual IEnumerator BodyAnimationController()
        {
            yield return null;
        }
        public override void Awake()
        {
        }
        public override void OnEnable()
        {
        }
        // Start is called before the first frame update
        public override void Start()
        {
            Comp_FxRoot = transform.parent?.gameObject?.GetComponent<FxRootWorker>();
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
