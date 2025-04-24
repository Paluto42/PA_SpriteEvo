namespace SpriteEvo
{
    #if DEBUG_BUILD
    public interface IPawnRotate
    {
        void FaceNorth();
        void FaceEast();
        void FaceSouth();
        void FaceWest();
    }
    #endif
}
