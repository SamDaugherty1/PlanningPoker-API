using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Api.Exceptions;
using PlanningPoker.Api.Models;
using PlanningPoker.Api.Services;
using PlanningPoker.Auth.Models;


namespace PlanningPoker.Api.Hubs;

public class PokerHub : Hub
{
    private readonly EstimationService _estimationService;
    public PokerHub(EstimationService estimationService)
    {
        _estimationService = estimationService;
    }
    

    public override Task OnConnectedAsync()
    {
        // Join game?
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        // Leave game

        return base.OnDisconnectedAsync(exception);
    }
    

    public void JoinGame(string gameId, User user)
    {
        _estimationService.JoinGame(gameId, user);
    }

    public void SubmitEstimate(Estimate estimate)
    {
        
    }

    public void ShowEstimates()
    {
        
    }

    public void ClearEstimates()
    {
        
    }
    
}
