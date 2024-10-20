using System;
using PlanningPoker.Auth.Models;

namespace PlanningPoker.Auth.Services;

public interface ITokenService
{
    public string GenerateToken(User user);
}
