using Game.Pathfind;

namespace PathfindingCosts
{
    public struct PathfindPedestrianData : ILoadFromGame<Game.Prefabs.PathfindPedestrianData>
    {
        public PathfindCosts? WalkingCost;
        public PathfindCosts? CrosswalkCost;
        public PathfindCosts? UnsafeCrosswalkCost;
        public PathfindCosts? SpawnCost;

        public void Load(Game.Prefabs.PathfindPedestrianData data)
        {
            this.WalkingCost = new(data.m_WalkingCost);
            this.CrosswalkCost = new(data.m_CrosswalkCost);
            this.UnsafeCrosswalkCost = new(data.m_UnsafeCrosswalkCost);
            this.SpawnCost = new(data.m_SpawnCost);
        }

        public void Set(Game.Prefabs.PathfindPedestrianData data)
        {
            data.m_WalkingCost = this.WalkingCost?.ToGameValue() ?? data.m_WalkingCost;
            data.m_CrosswalkCost = this.CrosswalkCost?.ToGameValue() ?? data.m_CrosswalkCost;
            data.m_UnsafeCrosswalkCost = this.UnsafeCrosswalkCost?.ToGameValue() ?? data.m_UnsafeCrosswalkCost;
            data.m_SpawnCost = this.SpawnCost?.ToGameValue() ?? data.m_SpawnCost;
        }
    }
}
