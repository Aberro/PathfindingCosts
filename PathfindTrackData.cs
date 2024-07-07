using Game.Pathfind;

namespace PathfindingCosts
{
    public struct PathfindTrackData : ILoadFromGame<Game.Prefabs.PathfindTrackData>
    {
        public PathfindCosts? DrivingCost;
        public PathfindCosts? TwowayCost;
        public PathfindCosts? SwitchCost;
        public PathfindCosts? DiamondCrossingCost;
        public PathfindCosts? SpawnCost;

        public void Load(Game.Prefabs.PathfindTrackData data)
        {
            this.DrivingCost = new(data.m_DrivingCost);
            this.TwowayCost = new(data.m_TwowayCost);
            this.SwitchCost = new(data.m_SwitchCost);
            this.DiamondCrossingCost = new(data.m_DiamondCrossingCost);
            this.SpawnCost = new(data.m_SpawnCost);
        }

        public void Set(Game.Prefabs.PathfindTrackData data)
        {
            data.m_DrivingCost = this.DrivingCost?.ToGameValue() ?? data.m_DrivingCost;
            data.m_TwowayCost = this.TwowayCost?.ToGameValue() ?? data.m_TwowayCost;
            data.m_SwitchCost = this.SwitchCost?.ToGameValue() ?? data.m_SwitchCost;
            data.m_DiamondCrossingCost = this.DiamondCrossingCost?.ToGameValue() ?? data.m_DiamondCrossingCost;
            data.m_SpawnCost = this.SpawnCost?.ToGameValue() ?? data.m_SpawnCost;
        }
    }
}
