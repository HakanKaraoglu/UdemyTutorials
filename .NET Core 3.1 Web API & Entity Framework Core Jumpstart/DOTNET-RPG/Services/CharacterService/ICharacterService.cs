using System.Collections.Generic;
using System.Threading.Tasks;
using DOTNET_RPG.Dtos.Character;
using DOTNET_RPG.Models;

namespace DOTNET_RPG.Services.CharacterService
{
    public interface ICharacterService
    {
         Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters();

         Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id);

         Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter);

        Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter);

        Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id);
    }
}