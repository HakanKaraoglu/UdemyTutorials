using AutoMapper;
using BandApi.DataTransferObjects;
using BandApi.Entities;
using BandApi.Helpers;
using BandApi.Services.IRepository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BandApi.Controllers
{
    [Route("api/bandscollections")]
    [ApiController]
    public class BandsCollectionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IBandAlbumRepository _bandAlbumRepository;

        public BandsCollectionsController(IMapper mapper, IBandAlbumRepository bandAlbumRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _bandAlbumRepository = bandAlbumRepository ?? throw new ArgumentNullException(nameof(bandAlbumRepository));
        }

        [HttpPost]
        public ActionResult<IEnumerable<Band>> CreateAGroupOfBandsAtAGo(IEnumerable<BandCreationDto> bandCreationDtos)
        {
            var bandsToCreate = _mapper.Map<IEnumerable<Band>>(bandCreationDtos);

            foreach (var band in bandsToCreate)
            {
                _bandAlbumRepository.AddBand(band);
            }

            _bandAlbumRepository.Save();

            var bandsToReturn = _mapper.Map<IEnumerable<BandDto>>(bandsToCreate);

            var ids = string.Join(",", bandsToReturn.Select(x => x.Id));

            return CreatedAtRoute("GetCollectionOfBands", new { ids }, bandsToReturn);
        }

        [HttpGet("({ids})", Name = "GetCollectionOfBands")]
        public ActionResult<IEnumerable<BandDto>> GetCollectionOfBands(
            [FromQuery] [ModelBinder(typeof(CustomModelBinder))]
            IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                ModelState.AddModelError("invalid ids", "ids can not be empty");
                return BadRequest(ModelState);
            }

            var bandIds = ids.ToArray();

            var bands = _bandAlbumRepository.GetBands(bandIds);

            if (bands.Count() != bandIds.Count())
                return NotFound();

            var bandsDto = _mapper.Map<IEnumerable<BandDto>>(bands);

            return Ok(bandsDto);
        }
    }
}
