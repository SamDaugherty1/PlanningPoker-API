using System;
using System.Collections.Concurrent;

namespace PlanningPoker.Api.Models;

public class Game
{
    public Game()
    {
        Id = Guid.NewGuid().ToString();
    }
    public string Id { get; set; }
    public string Name { get; set; }

    public readonly ConcurrentDictionary<string, Player> Players = new();
}
