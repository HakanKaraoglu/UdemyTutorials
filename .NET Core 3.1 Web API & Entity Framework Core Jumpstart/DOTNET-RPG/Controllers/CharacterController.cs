using Microsoft.AspNetCore.Mvc;
using DOTNET_RPG.Models;
using DOTNET_RPG.Services.CharacterService;
using System.Threading.Tasks;
using DOTNET_RPG.Dtos.Character;
using Microsoft.AspNetCore.Authorization;

namespace DOTNET_RPG.Controllers
{
    [Authorize(Roles ="Player")]
    [ApiController]
    [Route("[controller]")]
    public class CharacterController:ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService=characterService;
        }

        //[AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _characterService.GetAllCharacters());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingle(int id)
        {
            return Ok(await _characterService.GetCharacterById(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddCharacter(AddCharacterDto newCharacter)
        {
            return Ok(await _characterService.AddCharacter(newCharacter));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCharacter(UpdateCharacterDto updateCharacter)
        {
            ServiceResponse<GetCharacterDto> response =await _characterService.UpdateCharacter(updateCharacter);
            if(response.Data==null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
            return Ok(await _characterService.DeleteCharacter(id));
        }
    }
}