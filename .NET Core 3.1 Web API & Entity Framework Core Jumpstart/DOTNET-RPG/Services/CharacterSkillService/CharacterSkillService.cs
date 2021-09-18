using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DOTNET_RPG.Data;
using DOTNET_RPG.Dtos.Character;
using DOTNET_RPG.Dtos.CharacterSkill;
using DOTNET_RPG.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DOTNET_RPG.Services.CharacterSkillService
{
    public class CharacterSkillService : ICharacterSkillService
    {
         private readonly IMapper _mapper;
        private readonly DataContext _dataContext;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterSkillService(IMapper mapper, DataContext dataContext,IHttpContextAccessor httpContextAccessor)
        {
            _mapper=mapper;
            _dataContext=dataContext;
            _httpContextAccessor=httpContextAccessor;
        }
        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            ServiceResponse<GetCharacterDto> response=new ServiceResponse<GetCharacterDto>();
            try
            {
                 var character=await _dataContext.Characters
                 .Include(x=>x.Weapon)
                 .Include(x=>x.CharacterSkills).ThenInclude(cs=>cs.Skill)
                 .FirstOrDefaultAsync(x=>x.Id==newCharacterSkill.CharacterId &&
                 x.User.Id==int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));

                 if (character==null)
                 {
                     response.Success=false;
                     response.Message="Character not found.";
                     return response;
                 }

                 var skill=await _dataContext.Skills.FirstOrDefaultAsync(s=>s.Id==newCharacterSkill.SkillId);

                 if (skill==null)
                 {
                     response.Success=false;
                     response.Message="Skill not found";
                     return response;
                 }

                 var characterSkill=new CharacterSkill()
                 {
                    Character=character,
                    Skill=skill
                 };

                 await _dataContext.CharacterSkills.AddAsync(characterSkill);
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