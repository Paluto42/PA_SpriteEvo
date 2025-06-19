namespace SpriteEvo
{
#if !RELEASE_BUILD
    public interface IFacialControl
    {
        //待机状态
        void IdleState();

        //眼睛
        void Blink();

        //面部
        void PerformAngry();
        void PerformHappy();
        void PerformSad();

    }
#endif
}
