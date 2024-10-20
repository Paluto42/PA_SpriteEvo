using UnityEngine;
using Verse;

namespace PA_SpriteEvo
{
    public class UniqueID_Thing : MonoBehaviour
    {
        #region Inspector
        public Thing UID { get; set; }

        #endregion

        void Start() 
        {
        }
        //在这里直接更新位置
        void Update() 
        {
            if (UID == null) return;
            //UID.DrawPos;
        }
    }
}
