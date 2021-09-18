using System.Threading.Tasks;
using DOTNET_RPG.Dtos.Fight;
using DOTNET_RPG.Services.FightService;
using Microsoft.AspNetCore.Mvc;

namespace DOTNET_RPG.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FightController: ControllerBase
    {

        private readonly IFightService _fightService;

        public FightController(IFightService fightService)
        {
            _fightService=fightService;
        }
        
        [HttpPost("weapon")]
        public async Task<IActionResult> WeaponAttack(WeaponAttackDto request)
        {
            return Ok(await _fightService.WeaponAttack(request));
        }

        [HttpPost("skill")]
        public async Task<IActionResult> Skillttack(SkillAttackDto request)
        {
            return Ok(await _fightService.SkillAttack(request));
        }

        [HttpPost]
        public async Task<IActionResult> Fight(FightRequestDto request)
        {
            return Ok(await _fightService.Fight(request));
        }

        [HttpPost]
        public async Task<IActionResult> GetHighscore()
        {
            return Ok(await _fightService.GetHighScore());
        }
    }
}