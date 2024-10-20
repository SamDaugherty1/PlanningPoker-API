using System;

namespace PlanningPoker.Api.Models;

public class Player
{
    public Player()
    {
        Id = Guid.NewGuid().ToString();
    }
    internal string Id { get; set; }
    public string Name { get; set; }
    public int? Estimate { get; set; }
}
