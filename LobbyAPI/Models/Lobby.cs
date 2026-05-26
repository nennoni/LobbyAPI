namespace LobbyAPI.Models
{
    public class Lobby
    {
        public string lobbyCode { get; set; }
        public string player1 { get; set; }
        public string player2 { get; set; }
        public bool isActive { get; set; }
        public bool gameStarted { get; set; }
        public DateTime createdAt { get; set; }
    }
}
