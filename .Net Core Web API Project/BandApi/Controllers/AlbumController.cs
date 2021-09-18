using AutoMapper;
using BandApi.DataTransferObjects;
using BandApi.Entities;
using BandApi.Services.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BandApi.Controllers
{
    [Route("api/bands/{bandId}/albums")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IBandAlbumRepository _bandAlbumRepository;

        public AlbumController(IMapper mapper, IBandAlbumRepository bandAlbumRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _bandAlbumRepository = bandAlbumRepository ?? throw new ArgumentNullException(nameof(bandAlbumRepository));
        }

        [HttpGet]
        public ActionResult<IEnumerable<AlbumDto>> GetAlbumsForBand(Guid bandId)
        {

            if (!_bandAlbumRepository.BandExists(bandId))
                return StatusCode(StatusCodes.Status404NotFound);

            var albumsByGivenBand = _bandAlbumRepository.GetAlbumsForABand(bandId);
            return StatusCode(StatusCodes.Status200OK, _mapper.Map<IEnumerable<AlbumDto>>(albumsByGivenBand));
        }

        [HttpGet("{albumId}", Name = "GetAlbumForBand")]
        public ActionResult<AlbumDto> GetAlbumForBand(Guid bandId, Guid albumId)
        {
            if (!_bandAlbumRepository.BandExists(bandId))
                return StatusCode(StatusCodes.Status404NotFound);

            var albumForBand = _bandAlbumRepository.GetAlbum(bandId, albumId);

            if (albumForBand == null)
                return StatusCode(StatusCodes.Status404NotFound);

            return StatusCode(StatusCodes.Status200OK, _mapper.Map<AlbumDto>(albumForBand));

        }

        [HttpPost]
        public ActionResult<AlbumDto> PostAlbum(Guid bandId, AlbumCreationDto creationDto)
        {
            if (!_bandAlbumRepository.BandExists(bandId))
                return StatusCode(StatusCodes.Status404NotFound, new { error = "Band does not exist " });

            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);

            var album = _mapper.Map<Album>(creationDto);

            _bandAlbumRepository.AddAlbum(bandId, album);
            _bandAlbumRepository.Save();

            var albumDto = _mapper.Map<AlbumDto>(album);
            return CreatedAtRoute("GetAlbumForBand", new { bandId, albumId = albumDto.BandId }, albumDto);
        }

        [HttpPut("{albumId}")]
        public ActionResult UpdateAlbum(Guid albumId, Guid bandId, AlbumForUpdatingDto albumToUpdateDto)
        {
            if (!_bandAlbumRepository.BandExists(bandId))
                return StatusCode(StatusCodes.Status404NotFound, new { error = "band doesn't exist" });

            var albumFromRepo = _bandAlbumRepository.GetAlbum(bandId, albumId);

            if (albumFromRepo == null)
            {
                var albumToCreate = _mapper.Map<Album>(albumToUpdateDto);
                albumToCreate.Id = albumId;

                _bandAlbumRepository.AddAlbum(bandId, albumToCreate);
                _bandAlbumRepository.Save();

                var albumToReturn = _mapper.Map<AlbumDto>(albumToCreate);

                return CreatedAtRoute("GetAlbumForBand", new { albumId, bandId }, albumToReturn);

            }

            _mapper.Map(albumToUpdateDto, albumFromRepo);

            _bandAlbumRepository.Save();

            return StatusCode(StatusCodes.Status204NoContent);
        }

        [HttpDelete("{albumId}")]
        public IActionResult DeleteAlbum(Guid albumId, Guid bandId)
        {
            if (!_bandAlbumRepository.BandExists(bandId))
                return StatusCode(StatusCodes.Status404NotFound);
            var albumToDelete = _bandAlbumRepository.GetAlbum(bandId, albumId);

            if (albumToDelete == null)
                return StatusCode(StatusCodes.Status404NotFound);

            _bandAlbumRepository.DeleteAlbum(albumToDelete);
            _bandAlbumRepository.Save();

            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
