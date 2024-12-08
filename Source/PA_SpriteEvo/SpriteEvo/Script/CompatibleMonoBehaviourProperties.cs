using SpriteEvo.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    ///<summary>相当于ThingCompProperties的用法,确定如何对动画实例添加脚本属性</summary>
    public class CompatibleMonoBehaviourProperties
    {
        public Type scriptClass = typeof(CompatibleMonoBehaviour);

        public CompatibleMonoBehaviourProperties() 
        {
        }
        public CompatibleMonoBehaviourProperties(Type scriptClass)
        {
            this.scriptClass = scriptClass;
        }
        public virtual void DoWorker() {}
    }
}
