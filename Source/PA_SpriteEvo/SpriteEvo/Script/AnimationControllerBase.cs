namespace SpriteEvo
{
    //多版本兼容 要用哪个版本就声明对应版本的类
    public class AnimationControllerBase<ISkeletonComponent, IAnimationStateComponent> : ScriptBase
    {
        //绘制相对深度(高度)
        public float drawDepth = 1;

        //ref
        #region
        public ISkeletonComponent skeletonInt;
        public IAnimationStateComponent animationStateInt;
        #endregion

        protected override void Awake()
        {
            skeletonInt ??= GetComponent<ISkeletonComponent>();
            animationStateInt ??= GetComponent<IAnimationStateComponent>();
        }

        public virtual void ControllerTick() 
        {
        }
    }
}
