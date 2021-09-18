using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DOTNET_RPG.Data;
using DOTNET_RPG.Dtos.Character;
using DOTNET_RPG.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DOTNET_RPG.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext dataContext,IHttpContextAccessor httpContextAccessor)
        {
            _mapper=mapper;
            _dataContext=dataContext;
            _httpContextAccessor=httpContextAccessor;
        }

        private int GetUserId()=>int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        private string GetUserRole()=>_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            ServiceResponse<List<GetCharacterDto>> serviceResponse=new ServiceResponse<List<GetCharacterDto>>(); 
            var character=_mapper.Map<Character>(newCharacter);
            character.User=await _dataContext.Users.FirstOrDefaultAsync(u=>u.Id==GetUserId());

            await _dataContext.Characters.AddAsync(character);
            await _dataContext.SaveChangesAsync();
            serviceResponse.Data=(_dataContext.Characters.Where(c=>c.User.Id==GetUserId()).Select(c=>_mapper.Map<GetCharacterDto>(c))).ToList();             
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
           ServiceResponse<List<GetCharacterDto>> serviceResponse=new ServiceResponse<List<GetCharacterDto>>(); 
           var dbCharacters=GetUserRole().Equals("Admin") ?
           await _dataContext.Characters.ToListAsync():
           await _dataContext.Characters.Where(c=>c.User.Id==GetUserId()).ToListAsync();
           serviceResponse.Data=(dbCharacters.Select(c=>_mapper.Map<GetCharacterDto>(c))).ToList();
           return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            ServiceResponse<GetCharacterDto> serviceResponse=new ServiceResponse<GetCharacterDto>();
            Character dbCharacters=await _dataContext.Characters
            .Include(c=>c.Weapon)
            .Include(c=>c.CharacterSkills).ThenInclude(cs=>cs.Skill)
            .FirstOrDefaultAsync(c=>c.Id==id && c.User.Id==GetUserId());
            serviceResponse.Data=_mapper.Map<GetCharacterDto>(dbCharacters);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter)
        {
            ServiceResponse<GetCharacterDto> serviceResponse=new ServiceResponse<GetCharacterDto>();
             
            try
            {               
                Character character=await _dataContext.Characters.Include(c=>c.User).FirstOrDefaultAsync(x=>x.Id==updateCharacter.Id);
                if (character.User.Id==GetUserId())
                {
                character.Name=updateCharacter.Name;
                character.RpgClass=updateCharacter.RpgClass;
                character.Defense=updateCharacter.Defense;
                character.HitPoints=updateCharacter.HitPoints;
                character.Intelligence=updateCharacter.Intelligence;
                character.Strength=updateCharacter.Strength;
                
                _dataContext.Characters.Update(character);
                await _dataContext.SaveChangesAsync();

                serviceResponse.Data=_mapper.Map<GetCharacterDto>(character);
                }
                else
                {
                    serviceResponse.Success=false;
                    serviceResponse.Message="Character not found";
                }
            }
            catch (System.Exception ex)
            {
                serviceResponse.Success=false;
                serviceResponse.Message=ex.Message;
            }

            

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> serviceResponse=new ServiceResponse<List<GetCharacterDto>>();
             
            try
            {               
               Character character=await _dataContext.Characters
               .FirstOrDefaultAsync(x=>x.Id==id && x.User.Id==GetUserId());
               
               if (character!=null)
               {
                     _dataContext.Remove(character);
                     await _dataContext.SaveChangesAsync();
                     serviceResponse.Data=(_dataContext.Characters.Where(x=>x.User.Id==GetUserId()).Select(c=>_mapper.Map<GetCharacterDto>(c))).ToList();
               }
               else
               {
                   serviceResponse.Success=false;
                   serviceResponse.Message="Character not found";
               }
            }
            catch (System.Exception ex)
            {
                serviceResponse.Success=false;
                serviceResponse.Message=ex.Message;
            }

            return serviceResponse;
        }
    }
}