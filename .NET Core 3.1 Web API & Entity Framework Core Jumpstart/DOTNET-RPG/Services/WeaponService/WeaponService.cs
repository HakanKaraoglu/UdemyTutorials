using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DOTNET_RPG.Data;
using DOTNET_RPG.Dtos.Character;
using DOTNET_RPG.Dtos.Weapon;
using DOTNET_RPG.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DOTNET_RPG.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public WeaponService(IMapper mapper, DataContext dataContext,IHttpContextAccessor httpContextAccessor)
        {
            _mapper=mapper;
            _dataContext=dataContext;
            _httpContextAccessor=httpContextAccessor;
        }
        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
        {
            ServiceResponse<GetCharacterDto> response=new ServiceResponse<GetCharacterDto>();
            try
            {
                 var character=await _dataContext.Characters.FirstOrDefaultAsync(x=>x.Id==newWeapon.CharacterId &&
                 x.User.Id==int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));

                 if (character==null)
                 {
                     response.Success=false;
                     response.Message="Character not found.";
                     return response;
                 }

                 var weapon=new Weapon
                 {
                     Name=newWeapon.Name,
                     Damage=newWeapon.Damage,
                     Character=character
                 };

                 await _dataContext.Weapons.AddAsync(weapon);
                 await _dataContext.SaveChangesAsync();

                 response.Data=_mapper.Map<GetCharacterDto>(character);
            }
            catch (System.Exception ex)
            {
               response.Success=false;
               response.Message=ex.Message;
            }
            return response;
        }
    }
}