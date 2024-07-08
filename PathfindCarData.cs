using System.Collections.Generic;
using Game.Pathfind;

namespace PathfindingCosts
{
    public class PathfindCarData : ILoadFromGame<Game.Prefabs.PathfindCarData>
    {
        public Dictionary<string, PathfindCosts?> DrivingCost = new();
        public Dictionary<string, PathfindCosts?> TurningCost = new();
        public Dictionary<string, PathfindCosts?> UnsafeTurningCost = new();
        public Dictionary<string, PathfindCosts?> UTurnCost = new();
        public Dictionary<string, PathfindCosts?> UnsafeUTurnCost = new();
        public Dictionary<string, PathfindCosts?> LaneCrossCost = new();
        public Dictionary<string, PathfindCosts?> ParkingCost = new();
        public Dictionary<string, PathfindCosts?> SpawnCost = new();
        public Dictionary<string, PathfindCosts?> ForbiddenCost = new();

        public void Load(string preset, Game.Prefabs.PathfindCarData data)
        {
            this.DrivingCost[preset] = new(data.m_DrivingCost);
            this.TurningCost[preset] = new(data.m_TurningCost);
            this.UnsafeTurningCost[preset] = new(data.m_UnsafeTurningCost);
            this.UTurnCost[preset] = new(data.m_UTurnCost);
            this.UnsafeUTurnCost[preset] = new(data.m_UnsafeUTurnCost);
            this.LaneCrossCost[preset] = new(data.m_LaneCrossCost);
            this.ParkingCost[preset] = new(data.m_ParkingCost);
            this.SpawnCost[preset] = new(data.m_SpawnCost);
            this.ForbiddenCost[preset] = new(data.m_ForbiddenCost);
        }

        public void Set(string preset, ref Game.Prefabs.PathfindCarData data)
        {
            if(this.DrivingCost.ContainsKey(preset))
                data.m_DrivingCost = this.DrivingCost[preset]?.ToGameValue() ?? data.m_DrivingCost;
            if(this.TurningCost.ContainsKey(preset))
                data.m_TurningCost = this.TurningCost[preset]?.ToGameValue() ?? data.m_TurningCost;
            if(this.UnsafeTurningCost.ContainsKey(preset))
                data.m_UnsafeTurningCost = this.UnsafeTurningCost[preset]?.ToGameValue() ?? data.m_UnsafeTurningCost;
            if(this.UTurnCost.ContainsKey(preset))
                data.m_UTurnCost = this.UTurnCost[preset]?.ToGameValue() ?? data.m_UTurnCost;
            if(this.UnsafeUTurnCost.ContainsKey(preset))
                data.m_UnsafeUTurnCost = this.UnsafeUTurnCost[preset]?.ToGameValue() ?? data.m_UnsafeUTurnCost;
            if(this.LaneCrossCost.ContainsKey(preset))
                data.m_LaneCrossCost = this.LaneCrossCost[preset]?.ToGameValue() ?? data.m_LaneCrossCost;
            if(this.ParkingCost.ContainsKey(preset))
                data.m_ParkingCost = this.ParkingCost[preset]?.ToGameValue() ?? data.m_ParkingCost;
            if(this.SpawnCost.ContainsKey(preset))
                data.m_SpawnCost = this.SpawnCost[preset]?.ToGameValue() ?? data.m_SpawnCost;
            if(this.ForbiddenCost.ContainsKey(preset))
                data.m_ForbiddenCost = this.ForbiddenCost[preset]?.ToGameValue() ?? data.m_ForbiddenCost;
        }
    }
}
