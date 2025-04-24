using System;

namespace SpriteEvo
{
    ///<summary>相当于ThingCompProperties的用法,确定如何对动画实例添加脚本属性</summary>
    public class ScriptProperties
    {
        public Type scriptClass = typeof(ScriptBase);

        public ScriptProperties() 
        {
        }

        public ScriptProperties(Type scriptClass)
        {
            this.scriptClass = scriptClass;
        }
    }
}
