using Microsoft.AspNetCore.SignalR;

namespace LobbyAPI.SignalR
{
    public class GameHub : Hub
    {
        public async Task JoinGame(string lobbyCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyCode);
            await Clients.Group(lobbyCode).SendAsync("PlayerJoined", Context.ConnectionId);
        }

        public async Task UpdatePosition(string lobbyCode, string playerId, float x, float y)
        {
            await Clients.OthersInGroup(lobbyCode).SendAsync("UpdatePosition", playerId, x, y);
        }

        public async Task PlayerShoot(string lobbyCode, float x, float y, float dirX, float dirY)
        {
            await Clients.OthersInGroup(lobbyCode).SendAsync("EnemyShoot", x, y, dirX, dirY);
        }

        public async Task UpdateHp(string lobbyCode, int hp, int playerNumber)
        {
            await Clients.OthersInGroup(lobbyCode).SendAsync("UpdateHp", hp, playerNumber);
        }

        public async Task PlayerDied(string lobbyCode, int playerNumber)
        {
            Console.WriteLine($"Player {playerNumber} died in lobby {lobbyCode}");
            await Clients.Group(lobbyCode).SendAsync("GameOver", playerNumber);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Hantera när en spelare kopplar bort sig
            await base.OnDisconnectedAsync(exception);
        }
    }
}
