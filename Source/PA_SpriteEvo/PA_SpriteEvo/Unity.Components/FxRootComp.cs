using UnityEngine;
using Verse;

namespace PA_SpriteEvo.Unity
{
    //控制坐标更新和旋转方向的Root物件
    public class FxRootComp : MonoBehaviour
    {
        #region Inspector
        public Thing User;

        #endregion
        public bool CanDrawNow = false;
        public bool HaveAllRotNode = false;
        GameObject Root => base.gameObject;
        //
        //不建议使用一切与Find有关的方法获取它们，而是加上组件后直接用SetChildren设置
        public GameObject SouthChild;
        public GameObject NorthChild;
        public GameObject EastChild;
        public void SetChildren(GameObject s, GameObject n, GameObject e)
        {
            this.SouthChild = s;
            this.NorthChild = n;
            this.EastChild = e;
            HaveAllRotNode = true;
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
            if (User == null) return;

            Root.transform.position = User.DrawPos;

            switch (User.Rotation.AsInt)
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
                    Log.Error("ToQuat with Rot = " + User.Rotation.AsInt);
                    break;
            }
        }
    }
}
