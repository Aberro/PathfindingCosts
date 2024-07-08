using Unity.Entities;

namespace PathfindingCosts
{
    public interface ILoadFromGame<T> where T : unmanaged, IComponentData
    {
        public void Load(string preset, T data);
        public void Set(string preset, ref T data);
    }
}
