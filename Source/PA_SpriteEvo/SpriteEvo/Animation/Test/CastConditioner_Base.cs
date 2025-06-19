namespace SpriteEvo
{
#if !RELEASE_BUILD
    public abstract class CastConditioner_Base
    {
        public bool invert = false;  //如果是true的话，会反着输出结果
        public virtual bool CastableInternal(ScriptBase instance)
        {
            bool res = CastableInternal(instance);
            if (invert) res = !res;
            return res;
        }
        public abstract bool Castable(ScriptBase instance);
    }
#endif
}
