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
    public class FxExtraComp : MonoBehaviour
    {
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
        public virtual void Awake()
        {
        }
        public virtual void OnEnable()
        {
        }
        public virtual void Start()
        {
        }
        public virtual void FixedUpdate()
        {
        }
        public virtual void Update()
        {
        }
        public virtual void LateUpdate() 
        {
        }
        public virtual void OnGUI()
        {
        }
        public virtual void OnDisable()
        {
        }
        public virtual void OnDestory()
        {
        }
    }
}
