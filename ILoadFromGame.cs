using Unity.Entities;

namespace PathfindingCosts
{
    public interface ILoadFromGame<T> where T : unmanaged, IComponentData
    {
        public void Load(T data);
        public void Set(T data);
    }
}
