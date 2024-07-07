using Game.Pathfind;

namespace PathfindingCosts
{
    public struct PathfindCarData : ILoadFromGame<Game.Prefabs.PathfindCarData>
    {
        public PathfindCosts? DrivingCost;
        public PathfindCosts? TurningCost;
        public PathfindCosts? UnsafeTurningCost;
        public PathfindCosts? UTurnCost;
        public PathfindCosts? UnsafeUTurnCost;
        public PathfindCosts? LaneCrossCost;
        public PathfindCosts? ParkingCost;
        public PathfindCosts? SpawnCost;
        public PathfindCosts? ForbiddenCost;

        public void Load(Game.Prefabs.PathfindCarData data)
        {
            this.DrivingCost = new(data.m_DrivingCost);
            this.TurningCost = new(data.m_TurningCost);
            this.UnsafeTurningCost = new(data.m_UnsafeTurningCost);
            this.UTurnCost = new(data.m_UTurnCost);
            this.UnsafeUTurnCost = new(data.m_UnsafeUTurnCost);
            this.LaneCrossCost = new(data.m_LaneCrossCost);
            this.ParkingCost = new(data.m_ParkingCost);
            this.SpawnCost = new(data.m_SpawnCost);
            this.ForbiddenCost = new(data.m_ForbiddenCost);
        }

        public void Set(Game.Prefabs.PathfindCarData data)
        {
            data.m_DrivingCost = this.DrivingCost?.ToGameValue() ?? data.m_DrivingCost;
            data.m_TurningCost = this.TurningCost?.ToGameValue() ?? data.m_TurningCost;
            data.m_UnsafeTurningCost = this.UnsafeTurningCost?.ToGameValue() ?? data.m_UnsafeTurningCost;
            data.m_UTurnCost = this.UTurnCost?.ToGameValue() ?? data.m_UTurnCost;
            data.m_UnsafeUTurnCost = this.UnsafeUTurnCost?.ToGameValue() ?? data.m_UnsafeUTurnCost;
            data.m_LaneCrossCost = this.LaneCrossCost?.ToGameValue() ?? data.m_LaneCrossCost;
            data.m_ParkingCost = this.ParkingCost?.ToGameValue() ?? data.m_ParkingCost;
            data.m_SpawnCost = this.SpawnCost?.ToGameValue() ?? data.m_SpawnCost;
            data.m_ForbiddenCost = this.ForbiddenCost?.ToGameValue() ?? data.m_ForbiddenCost;
        }
    }
}
