using System;
using PlanningPoker.Api.Models;

namespace PlanningPoker.Api.Exceptions;

public class NoActiveGameException : Exception
{
    public NoActiveGameException()
    {
        
    }

    public static void ThrowIfNotActive(Game? game)
    {
        if (game is null)
        {
            throw new NoActiveGameException();
        }
    }
}
