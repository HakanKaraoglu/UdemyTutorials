using System.Threading.Tasks;
using DOTNET_RPG.Dtos.Character;
using DOTNET_RPG.Dtos.Weapon;
using DOTNET_RPG.Models;

namespace DOTNET_RPG.Services.WeaponService
{
    public interface IWeaponService
    {
       Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon);
    }
}