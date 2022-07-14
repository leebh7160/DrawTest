using System.Windows;

namespace DrawTest2.Drawers
{
    public interface IShape
    {
        void Create(Point e);
        void Update(Point e);
        void Finish();
    }
}
