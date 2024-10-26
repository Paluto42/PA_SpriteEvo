using PA_SpriteEvo.Unity;
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
    public abstract class FxBodyComp : MonoBehaviour
    {
        #region Inspector
        public GameObject SouthChild { get; set; }
        public GameObject NorthChild { get; set; }
        public GameObject WestChild { get; set; }
        public GameObject EastChild { get; set; }
        #endregion

        public MonoBehaviour Attachment;
        public virtual MonoBehaviour GetAttachment()
        {
            return null;
        }
        public virtual void DoRotation(Rot4 rot)
        {
        }
        public virtual IEnumerator BodyAnimationController()
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
