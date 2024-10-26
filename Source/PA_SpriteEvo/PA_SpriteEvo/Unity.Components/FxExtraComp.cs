using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PA_SpriteEvo
{
    public class FxExtraComp : BaseControllerComp
    {
        #region Inspector
        public GameObject SouthChild { get; set; }
        public GameObject NorthChild { get; set; }
        public GameObject WestChild { get; set; }
        public GameObject EastChild { get; set; }
        #endregion

        //记得改成集合
        public MonoBehaviour Attachment;
        public virtual MonoBehaviour GetAttachment()
        {
            return null;
        }
        //实现三视角旋转必写
        public virtual void DoRotation(Rot4 rot)
        {
        }
        public virtual IEnumerator ExtraAnimationController()
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
        }
        public override void FixedUpdate()
        {
        }
        // Update is called once per frame
        public override void Update()
        {
        }
        public override void LateUpdate() 
        {
        }
        public override void OnGUI()
        {
        }
        public override void OnDisable()
        {
        }
        public override void OnDestory()
        {
        }
    }
}
