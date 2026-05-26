namespace LobbyAPI.Models
{
    public class LobbyHandler
    {
        private Dictionary<string, Lobby> lobbies = new Dictionary<string, Lobby>();
        public string GenerateLobbyCode()
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const string digits = "0123456789";
            var random = new Random();

            string lobbyCode = "";

            for (int i = 0; i < 3; i++)
            {
                 lobbyCode += letters[random.Next(letters.Length)];
            }
            for (int i = 0; i < 3; i++)
            {
                lobbyCode += digits[random.Next(digits.Length)];
            }

            return lobbyCode;
        }

        public Lobby CreateLobby(string playerName)
        {
            string lobbyCode = GenerateLobbyCode();
            Lobby lobby = new Lobby
            {
                lobbyCode = lobbyCode,
                player1 = playerName,
                player2 = null,
                isActive = true,
                gameStarted = false,
                createdAt = DateTime.Now
            };
            lobbies[lobbyCode] = lobby;
            return lobby;
        }

        public List<Lobby> GetLobbies()
        {
            return lobbies.Values.Where(l => l.isActive).ToList();
        }

        public Lobby GetLobby(string lobbyCode)
        {
            lobbies.TryGetValue(lobbyCode, out Lobby lobby);
            Console.WriteLine($"GetLobby: {lobbyCode}, player2: {lobby?.player2}");
            return lobby;
        }

        public Lobby JoinLobby(string lobbyCode, string playerName)
        {
            if (lobbies.TryGetValue(lobbyCode, out Lobby lobby))
            {
                if (lobby.isActive && lobby.player2 == null)
                {
                    lobby.player2 = playerName;
                    return lobby;
                }
            }
            return null;
        }

        public Lobby StartGame(string lobbyCode)
        {
            if (lobbies.TryGetValue(lobbyCode, out Lobby lobby))
            {
                if (lobby.isActive && lobby.player1 != null && lobby.player2 != null)
                {
                    lobby.gameStarted = true;
                    return lobby;
                }
            }
            return null;
        }

        public bool CloseLobby(string lobbyCode)
        {
            if (lobbies.TryGetValue(lobbyCode, out Lobby lobby))
            {
                lobby.isActive = false;
                return true;
            }
            return false;
        }
    }
}

   