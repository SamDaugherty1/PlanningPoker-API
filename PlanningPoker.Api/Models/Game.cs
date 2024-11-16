using System;
using System.Collections.Generic;

namespace PlanningPoker.Api.Models;

public class Game
{
    public Game()
    {
        Id = Guid.NewGuid().ToString();
        Players = new Dictionary<string, Player>();
    }

    public string Id { get; set; }
    public required string Name { get; set; }

    public Dictionary<string, Player> Players { get; set; }
    public bool ShowCards { get; set; }
}
