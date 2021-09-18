using System.Collections.Generic;
using DOTNET_RPG.Dtos.Skill;
using DOTNET_RPG.Dtos.Weapon;
using DOTNET_RPG.Models;

namespace DOTNET_RPG.Dtos.Character
{
    public class GetCharacterDto
    {
        public int Id { get; set; }

        public string Name { get; set; }="Frodo";
        public int HitPoints { get; set; }=100;
        public int Strength { get; set; }=10;
        public int Defense { get; set; }=10;
        public int Intelligence { get; set; }=10;
        public RpgClass RpgClass { get; set; }=RpgClass.Knight;
        public GetWeaponDto Weapon { get; set; }
        public List<GetSkillDto> Skills { get; set; }
        public int Fights { get; set; }
        public int Victories { get; set; }
        public int Defeats { get; set; }
    }
}