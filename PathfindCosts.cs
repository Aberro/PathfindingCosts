namespace PathfindingCosts;

public class PathfindCosts
{
    public float Time;
    public float Behaviour;
    public float Money;
    public float Comfort;

    public PathfindCosts(float time, float behaviour, float money, float comfort)
    {
        this.Time = time;
        this.Behaviour = behaviour;
        this.Money = money;
        this.Comfort = comfort;
    }
    public PathfindCosts()
    {
        this.Time = 0;
        this.Behaviour = 0;
        this.Money = 0;
        this.Comfort = 0;
    }
    public PathfindCosts(Game.Pathfind.PathfindCosts costs)
    {
        this.Time = costs.m_Value.x;
        this.Behaviour = costs.m_Value.y;
        this.Money = costs.m_Value.z;
        this.Comfort = costs.m_Value.w;
    }
    public Game.Pathfind.PathfindCosts ToGameValue() => new Game.Pathfind.PathfindCosts(this.Time, this.Behaviour, this.Money, this.Comfort);
}
