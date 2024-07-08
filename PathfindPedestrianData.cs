using System.Collections.Generic;
using Game.Pathfind;

namespace PathfindingCosts
{
    public class PathfindPedestrianData : ILoadFromGame<Game.Prefabs.PathfindPedestrianData>
    {
        public Dictionary<string, PathfindCosts?> WalkingCost = new();
        public Dictionary<string, PathfindCosts?> CrosswalkCost = new();
        public Dictionary<string, PathfindCosts?> UnsafeCrosswalkCost = new();
        public Dictionary<string, PathfindCosts?> SpawnCost = new();

        public void Load(string preset, Game.Prefabs.PathfindPedestrianData data)
        {
            this.WalkingCost[preset] = new(data.m_WalkingCost);
            this.CrosswalkCost[preset] = new(data.m_CrosswalkCost);
            this.UnsafeCrosswalkCost[preset] = new(data.m_UnsafeCrosswalkCost);
            this.SpawnCost[preset] = new(data.m_SpawnCost);
        }

        public void Set(string preset, ref Game.Prefabs.PathfindPedestrianData data)
        {
            if(this.WalkingCost.ContainsKey(preset))
                data.m_WalkingCost = this.WalkingCost[preset]?.ToGameValue() ?? data.m_WalkingCost;
            if(this.CrosswalkCost.ContainsKey(preset))
                data.m_CrosswalkCost = this.CrosswalkCost[preset]?.ToGameValue() ?? data.m_CrosswalkCost;
            if(this.UnsafeCrosswalkCost.ContainsKey(preset))
                data.m_UnsafeCrosswalkCost = this.UnsafeCrosswalkCost[preset]?.ToGameValue() ?? data.m_UnsafeCrosswalkCost;
            if(this.SpawnCost.ContainsKey(preset))
                data.m_SpawnCost = this.SpawnCost[preset]?.ToGameValue() ?? data.m_SpawnCost;
        }
    }
}
