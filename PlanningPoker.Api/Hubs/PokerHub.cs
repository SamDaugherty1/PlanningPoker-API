using System;
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
        
    }
    

    public override Task OnConnectedAsync()
    {
        

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    public void JoinGame(User user)
    {
        
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
