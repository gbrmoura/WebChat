using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using WebChat.Shared.Models;
using System;

namespace WebChat.Client.Services
{
    public class HubService 
    { 
        private NavigationManager _navigationManager;

        public HubService(NavigationManager NavigationManager)
        {
            _navigationManager = NavigationManager;
        }

        // The HubConnection Credentials property is used to specify the access token to use when connecting to the hub.
        public string username = String.Empty;
        public string password = String.Empty;

        // The HubConnection Property is used to connect to the hub.
        public HubConnection? hubConnection;
        public string message = String.Empty;
        public string messages = String.Empty;

        public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;
        public async Task Connect()
        {
            // Create a HubConnection
            hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri($"/chathub?username={username}"))
                .Build();

            // Message received from server
            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var msg = $"{(String.IsNullOrEmpty(user) ? "" : user + ": ")} {message}";
                messages += msg + "\n";
                NotifyDataChanged();
            });

            await hubConnection.StartAsync();
        }

        public async Task SendMessage()
        {
            if (hubConnection != null)
            {
                await hubConnection.SendAsync("SendMessage", username, message);
                message = String.Empty;
            }
        }
        public async ValueTask DisposeAsync()
        {
            if (hubConnection != null)
            {
                await hubConnection.DisposeAsync();
            }
        }

        public event Action OnChange;
        private void NotifyDataChanged() => OnChange?.Invoke();
    }
}