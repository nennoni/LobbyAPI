namespace LobbyAPI.Models
{
    using System.Net.WebSockets;
    using System.Text;
    using System.Collections.Concurrent;

    public class WebSocketHandler
    {
        private static ConcurrentDictionary<string, List<WebSocket>> lobbies =
            new ConcurrentDictionary<string, List<WebSocket>>();

        public async Task Handle(WebSocket socket, string lobbyCode)
        {
            lobbies.AddOrUpdate(lobbyCode,
                new List<WebSocket> { socket },
                (key, list) => { list.Add(socket); return list; });

            var buffer = new byte[1024 * 4];

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    if (message.Contains("gameover"))
                        await BroadcastToAll(lobbyCode, message);
                    else
                        await BroadcastToLobby(lobbyCode, socket, message);
                }
            }
            finally
            {
                if (lobbies.TryGetValue(lobbyCode, out var list))
                    list.Remove(socket);

                if (socket.State == WebSocketState.Open)
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
            }
        }

        private async Task BroadcastToLobby(string lobbyCode, WebSocket sender, string message)
        {
            if (!lobbies.TryGetValue(lobbyCode, out var sockets)) return;

            var bytes = Encoding.UTF8.GetBytes(message);
            foreach (var socket in sockets.ToList())
            {
                if (socket != sender && socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(bytes),
                        WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        private async Task BroadcastToAll(string lobbyCode, string message)
        {
            if (!lobbies.TryGetValue(lobbyCode, out var sockets)) return;

            var bytes = Encoding.UTF8.GetBytes(message);
            foreach (var socket in sockets.ToList())
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(bytes),
                        WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
    }