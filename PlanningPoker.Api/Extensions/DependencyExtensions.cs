using System;
using PlanningPoker.Api.Hubs;

namespace PlanningPoker.Api.Extensions;

public static class DependencyExtensions
{
    public static WebApplication MapHubs(this WebApplication app)
    {
        app.MapHub<PokerHub>("/join")
           .RequireAuthorization();

        return app;
    }
}
