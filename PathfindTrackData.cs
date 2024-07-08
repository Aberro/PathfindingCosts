using System.Collections.Generic;
using Game.Pathfind;

namespace PathfindingCosts
{
    public class PathfindTrackData : ILoadFromGame<Game.Prefabs.PathfindTrackData>
    {
        public Dictionary<string, PathfindCosts?> DrivingCost = new();
        public Dictionary<string, PathfindCosts?> TwowayCost = new();
        public Dictionary<string, PathfindCosts?> SwitchCost = new();
        public Dictionary<string, PathfindCosts?> DiamondCrossingCost = new();
        public Dictionary<string, PathfindCosts?> SpawnCost = new();

        public void Load(string preset, Game.Prefabs.PathfindTrackData data)
        {
            this.DrivingCost[preset] = new(data.m_DrivingCost);
            this.TwowayCost[preset] = new(data.m_TwowayCost);
            this.SwitchCost[preset] = new(data.m_SwitchCost);
            this.DiamondCrossingCost[preset] = new(data.m_DiamondCrossingCost);
            this.SpawnCost[preset] = new(data.m_SpawnCost);
        }

        public void Set(string preset, ref Game.Prefabs.PathfindTrackData data)
        {
            if(this.DrivingCost.ContainsKey(preset))
                data.m_DrivingCost = this.DrivingCost[preset]?.ToGameValue() ?? data.m_DrivingCost;
            if(this.TwowayCost.ContainsKey(preset))
                data.m_TwowayCost = this.TwowayCost[preset]?.ToGameValue() ?? data.m_TwowayCost;
            if(this.SwitchCost.ContainsKey(preset))
                data.m_SwitchCost = this.SwitchCost[preset]?.ToGameValue() ?? data.m_SwitchCost;
            if(this.DiamondCrossingCost.ContainsKey(preset))
                data.m_DiamondCrossingCost = this.DiamondCrossingCost[preset]?.ToGameValue() ?? data.m_DiamondCrossingCost;
            if(this.SpawnCost.ContainsKey(preset))
                data.m_SpawnCost = this.SpawnCost[preset]?.ToGameValue() ?? data.m_SpawnCost;
        }
    }
}
