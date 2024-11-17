using System;
using System.Collections.Generic;

namespace PlanningPoker.Api.Models;

public class Game
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, Player> Players { get; set; } = new();
    public bool ShowCards { get; set; }
}
