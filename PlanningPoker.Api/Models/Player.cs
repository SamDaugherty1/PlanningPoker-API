using System;

namespace PlanningPoker.Api.Models;

public class Player
{
    public Player()
    {
        Id = Guid.NewGuid().ToString();
    }
    internal string Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
    public int? Estimate { get; set; }
    public int? Card { get; set; }
    public bool ViewOnly { get; set; }
}
