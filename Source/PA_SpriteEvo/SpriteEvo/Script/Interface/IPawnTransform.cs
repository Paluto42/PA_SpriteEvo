namespace SpriteEvo
{
#if !RELEASE_BUILD
    public interface IPawnTransform
    {
        void FaceNorth();
        void FaceEast();
        void FaceSouth();
        void FaceWest();
        void UpdatePosition();
        void UpdateRotation();
    }
#endif
}
