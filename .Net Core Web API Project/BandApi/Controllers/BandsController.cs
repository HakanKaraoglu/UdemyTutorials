using AutoMapper;
using BandApi.DataTransferObjects;
using BandApi.Entities;
using BandApi.Helpers;
using BandApi.QueryModifiers;
using BandApi.Services.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BandApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BandsController : ControllerBase
    {
        private readonly IBandAlbumRepository _bandAlbumRepository;
        private readonly IMapper _mapper;

        public BandsController(IBandAlbumRepository bandAlbumRepository, IMapper mapper)
        {
            _bandAlbumRepository = bandAlbumRepository ?? throw new ArgumentNullException(nameof(bandAlbumRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet(Name = "GetBands")]
        [HttpHead]
        public IActionResult GetBands([FromQuery] QueryParameters queryParameters)
        {
            var bands = _bandAlbumRepository.GetBands(queryParameters);

            var nextPageLink = bands.HasNext ? CreateUri(UriType.NextPage, queryParameters) : null;
            var previousPageLink = bands.HasPrevious ? CreateUri(UriType.PreviousPage, queryParameters) : null;

            var metaData = new
            {
                bands.TotalItems,
                bands.PageSize,
                bands.CurrentPage,
                bands.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("Pagination", JsonConvert.SerializeObject(metaData));

            var bandsDto = _mapper.Map<IEnumerable<BandDto>>(bands);

            return StatusCode(StatusCodes.Status200OK, bandsDto);
        }

        [HttpGet("{bandId}", Name = "GetBand")]
        public IActionResult GetBand(Guid bandId)
        {
            var band = _bandAlbumRepository.GetBand(bandId);

            if (band == null)
                return StatusCode(StatusCodes.Status404NotFound);

            var bandDto = _mapper.Map<BandDto>(band);
            return StatusCode(StatusCodes.Status200OK, bandDto);
        }

        [HttpPost]
        public ActionResult<BandDto> CreateBand(BandCreationDto creationDto)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);

            creationDto.Founded ??= DateTime.Now;

            var band = _mapper.Map<Band>(creationDto);

            _bandAlbumRepository.AddBand(band);
            _bandAlbumRepository.Save();

            var bandDto = _mapper.Map<BandDto>(band);
            return CreatedAtRoute("GetBand", new { bandId = bandDto.Id }, bandDto);

        }

        [HttpDelete("{bandId}")]
        public IActionResult DeleteBand(Guid bandId)
        {
            var bandToDelete = _bandAlbumRepository.GetBand(bandId);

            if (bandToDelete == null)
                return StatusCode(StatusCodes.Status400BadRequest);

            _bandAlbumRepository.DeleteBand(bandToDelete);
            _bandAlbumRepository.Save();

            return StatusCode(StatusCodes.Status204NoContent);
        }

        private string CreateUri(UriType uriType, QueryParameters parameters)
        {
            return uriType switch
            {
                UriType.PreviousPage => Url.Link("GetBands",
                    new
                    {
                        PageNumber = parameters.PageNumber - 1,
                        parameters.PageSize,
                        parameters.BandName,
                        parameters.MainGenre
                    }),
                UriType.NextPage => Url.Link("GetBands",
                    new
                    {
                        PageNumber = parameters.PageNumber + 1,
                        parameters.PageSize,
                        parameters.BandName,
                        parameters.MainGenre
                    }),
                _ => Url.Link("GetBands",
                    new
                    {
                        parameters.PageNumber,
                        parameters.PageSize,
                        parameters.BandName,
                        parameters.MainGenre
                    })
            };
        }
    }
}
