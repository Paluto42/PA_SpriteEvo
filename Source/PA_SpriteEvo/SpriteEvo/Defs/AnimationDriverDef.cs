using System.Collections.Generic;
using Verse;

namespace SpriteEvo
{
#if !RELEASE_BUILD
    public struct PawnStatusParams
    {
        public float currentMood;
        public float currentPain;
    }
    public struct TrackInfo
    {
        public List<TrackSet> trackSets;
    }
    //一个轨道对应的动画队列设置
    public class TrackSet
    {
        public int index; //轨道序号
        public float interval = 0; //队列刷新时长 为0就是不刷新用来设置默认静态轨道
        public List<TrackQueue> queues = new(); //队列
        public class TrackQueue
        {
            public string animation;
            public float delay = 0;
            public bool loop = false;
            public MixBlendInternal blendMode = MixBlendInternal.Replace;
        }
    }
    public class AnimationUpdateWorker
    {
        public class UpdateResult
        {
        }
        public virtual UpdateResult Apply(TrackInfo tinfo)
        {
            return null;
        }
    }
    public class AnimationDriverDef : Def
    {
        public int priority = 0; //0优先级最低
        public List<TrackSet> trackSets = new();
        public List<CastConditioner_Base> castConditions = new();
        //public Type workerClass = typeof(AnimationUpdateWorker);

        public bool Castable(ScriptBase instance)
        {
            foreach (var cc in castConditions)
            {
                if (cc.CastableInternal(instance) == false)
                {
                    return false;
                }
            }
            return true;
        }
        /*private AnimationUpdateWorker workerInt;
        public AnimationUpdateWorker Worker
        {
            get
            {
                workerInt ??= (AnimationUpdateWorker)Activator.CreateInstance(workerClass);
                return workerInt;
            }
        }*/
    }
#endif
}