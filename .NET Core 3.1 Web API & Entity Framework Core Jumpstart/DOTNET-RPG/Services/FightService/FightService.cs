using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DOTNET_RPG.Data;
using DOTNET_RPG.Dtos.Fight;
using DOTNET_RPG.Models;
using Microsoft.EntityFrameworkCore;

namespace DOTNET_RPG.Services.FightService
{
    public class FightService:IFightService 
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public FightService(DataContext dataContext,IMapper mapper)
        {
            _dataContext=dataContext;
            _mapper=mapper;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
           var response =new ServiceResponse<AttackResultDto>(); 
           
           try
            {

                var attacker = await _dataContext.Characters
                    .Include(c => c.Weapon)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                var opponent = await _dataContext.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                int damage = DoWeaponAttack(attacker, opponent);

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated!";

                _dataContext.Characters.Update(opponent);
                await _dataContext.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHP = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch (System.Exception ex)
           {
               response.Success=false;
               response.Message=ex.Message;
           }

           return response;
        }

        private static int DoWeaponAttack(Character attacker, Character opponent)
        {
            var damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
                opponent.HitPoints -= damage;
            return damage;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {
           var response =new ServiceResponse<AttackResultDto>(); 
           
           try
            {

                var attacker = await _dataContext.Characters
                    .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                var opponent = await _dataContext.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                var characterSkill = attacker.CharacterSkills.FirstOrDefault(cs => cs.Skill.Id == request.SkillId);

                if (characterSkill == null)
                {
                    response.Success = false;
                    response.Message = $"{attacker.Name} doesn't know that skill";
                    return response;
                }

                int damage = DoSkillAttack(attacker, opponent, characterSkill);

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated!";

                _dataContext.Characters.Update(opponent);
                await _dataContext.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHP = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch (System.Exception ex)
           {
               response.Success=false;
               response.Message=ex.Message;
           }

           return response;
        }

        private static int DoSkillAttack(Character attacker, Character opponent, CharacterSkill characterSkill)
        {
            var damage = characterSkill.Skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
                opponent.HitPoints -= damage;
            return damage;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            ServiceResponse<FightResultDto> response=new ServiceResponse<FightResultDto>()
            {
                Data=new FightResultDto()
            };

            try
            {
                var characters= 
                    await _dataContext.Characters
                    .Include(c=>c.Weapon)
                    .Include(c=>c.CharacterSkills).ThenInclude(c=>c.Skill)
                    .Where(c=>request.CharacterIds.Contains(c.Id)).ToListAsync();

                var defeated=false;
                while(!defeated)
                {
                    foreach (var attacker in characters)
                    {
                        var opponents=characters.Where(x=>x.Id!=attacker.Id).ToList();
                        var opponent =opponents[new Random().Next(opponents.Count)];

                        int damage=0;
                        var attackUsed=string.Empty;

                        var useWeapon=new Random().Next(2)==0;
                        if (useWeapon)
                        {
                            attackUsed=attacker.Weapon.Name;
                            damage=DoWeaponAttack(attacker,opponent);
                        }
                        else
                        {
                            int randomSkill=new Random().Next(attacker.CharacterSkills.Count);
                            attackUsed=attacker.CharacterSkills[randomSkill].Skill.Name;
                            damage=DoSkillAttack(attacker,opponent,attacker.CharacterSkills[randomSkill]);
                        }

                        response.Data.Log.Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage>=0? damage:0)} damage.");

                        if (opponent.HitPoints<=0)
                        {
                            defeated=true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has been defeated!");
                            response.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left!");
                            break;
                        }
                    }
                }

                characters.ForEach(c=>
                {
                    c.Fights++;
                    c.HitPoints=100;
                });  

                _dataContext.Characters.UpdateRange(characters);
                await _dataContext.SaveChangesAsync();                          
            }
            catch (Exception ex)
            {
                response.Success=false;
                response.Message=ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<List<HighscoreDto>>> GetHighScore()
        {
            List<Character> characters= await _dataContext.Characters
            .Where(c=>c.Fights>0)
            .OrderByDescending(c=>c.Victories)
            .ThenBy(c=>c.Defeats)
            .ToListAsync();

            var response=new ServiceResponse<List<HighscoreDto>>
            {
                Data=characters.Select(c=>_mapper.Map<HighscoreDto>(c)).ToList()
            };
            
            return response;
        }
    }
}