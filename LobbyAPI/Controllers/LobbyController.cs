using Microsoft.AspNetCore.Mvc;
using LobbyAPI.Models;

namespace LobbyAPI.Controllers
{
    [ApiController]
    [Route("api/lobby")]
    public class LobbyController : ControllerBase
    {
        private readonly LobbyHandler _lobbyHandler;

        public LobbyController(LobbyHandler lobby)
        {
            _lobbyHandler = lobby;
        }

        // Skapar en lobby
        [HttpPost("create")]
        public IActionResult CreateLobby(string playerName)
        {
            Lobby lobby = _lobbyHandler.CreateLobby(playerName);
            return Ok(lobby);
        }

        // Hämtar alla lobbies
        [HttpGet("lobbies")]
        public IActionResult GetLobbies()
        {
            var lobbies = _lobbyHandler.GetLobbies();
            return Ok(lobbies);
        }

        // Hämtar en specifik lobby
        [HttpGet("lobbies/{lobbyCode}")]
        public IActionResult GetLobby(string lobbyCode)
        {
            Lobby lobby = _lobbyHandler.GetLobby(lobbyCode);
            if (lobby == null)
            {
                return NotFound();
            }
            return Ok(lobby);
        }


        // Går med i en lobby
        [HttpPost("join/{lobbyCode}")]
        public IActionResult JoinLobby(string lobbyCode, string playerName)
        {
            Lobby lobby = _lobbyHandler.JoinLobby(lobbyCode, playerName);
            if ( lobby == null){
                return NotFound();
            }
            return Ok(lobby);
        }

        // Startar spelet i en lobby
        [HttpPost("start/{lobbyCode}")]
        public IActionResult StartGame(string lobbyCode)
        {
            Lobby lobby = _lobbyHandler.StartGame(lobbyCode);
            if (lobby == null)
            {
                return NotFound();
            }
            return Ok(lobby);
        }

        // Stänger en lobby
        [HttpDelete("close/{lobbyCode}")]
        public IActionResult CloseLobby(string lobbyCode)
        {
            bool success = _lobbyHandler.CloseLobby(lobbyCode);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
