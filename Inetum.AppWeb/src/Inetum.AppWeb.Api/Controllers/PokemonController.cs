using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static Inetum.AppWeb.Api.PokeapiResponse;

namespace Inetum.AppWeb.Api.Controllers
{
    [ApiController]
    [Route("pokemon")]
    public class PokemonController : ControllerBase
    {
        private readonly ILogger<PokemonController> _logger;
        private readonly IHttpClientFactory _httpClient;

        public PokemonController(ILogger<PokemonController> logger, IHttpClientFactory httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Root))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPokemonById(int id)
        {
            try
            {
                using var client = _httpClient.CreateClient("pokeapi");

                var response = await client.GetAsync($"pokemon/{id}");

                // 404
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound($"Pokémon with ID {id} not found.");
                }

                var content = await response.Content.ReadAsStringAsync();

                var root = JsonSerializer.Deserialize<Root>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (root == null)
                {
                    return StatusCode(500, "Error deserializing Pokémon data.");
                }

                return Ok(root);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obtaining the Pokémon.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
