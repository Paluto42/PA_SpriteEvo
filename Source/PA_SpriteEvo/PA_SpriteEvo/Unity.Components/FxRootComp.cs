using UnityEngine;
using Verse;

namespace PA_SpriteEvo.Unity
{
    //控制坐标更新的Root物件
    public class FxRootComp : MonoBehaviour
    {
        #region Inspector
        public Thing User;

        #endregion
        public bool CanDrawNow = false;
        GameObject Root => base.gameObject;
        //
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
            if (Current.ProgramState == ProgramState.Playing) 
            {
                CanDrawNow = true;
            }
        }
        void Update() 
        {
            if (!CanDrawNow) return;
            if (User == null) return;
            Root.transform.position = User.DrawPos;
        }
    }
}
