namespace SpriteEvo
{
    #if DEBUG_BUILD
    public interface ITransform
    {
        void PoseSouth();
        void PoseNorth();
        void PoseWest();
        void PoseEast();
        void Move();
    }
    #endif
}
