using System.Threading.Tasks;
using DOTNET_RPG.Dtos.Character;
using DOTNET_RPG.Dtos.CharacterSkill;
using DOTNET_RPG.Models;

namespace DOTNET_RPG.Services.CharacterSkillService
{
    public interface ICharacterSkillService
    {
       Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill);
    }
}