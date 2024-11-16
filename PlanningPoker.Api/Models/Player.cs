using System;

namespace PlanningPoker.Api.Models;

public class Player
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public int? Card { get; set; }
    public bool ViewOnly { get; set; }
}
