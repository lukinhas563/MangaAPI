using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FluentValidation;
using MangaAPI.Entities;
using MangaAPI.Models.Input;
using MangaAPI.Models.Pagination;
using MangaAPI.Models.View;
using MangaAPI.Persistence;
using MangaAPI.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;

namespace MangaAPI.Controllers
{
    [Route("api/mangas")]
    [ApiController]
    public class MangasController : ControllerBase
    {
        private readonly MangasContext _context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly CreateMangaValidator _validator;

        public MangasController(MangasContext context, IMapper mapper, Cloudinary cloudinary, CreateMangaValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _cloudinary = cloudinary;
            _validator = validator;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll(int pageNumber= 1, int pageSize = 10)
        {
            if (pageNumber < 1) { pageNumber = 1; }

            var mangas = _context.Mangas.Where(manga => !manga.IsDeleted).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var count = mangas.Count;

            var mangasViewModel = _mapper.Map<List<MangaListViewModel>>(mangas);
            var response = new PageList<MangaListViewModel>(mangasViewModel, count, pageNumber, pageSize);

            return Ok(response);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(Guid id, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) { pageNumber=1; }

            var manga = _context.Mangas
                .Include(manga => manga.Chapters)
                .SingleOrDefault(manga => manga.Id == id && !manga.IsDeleted);

            if (manga == null) { return NotFound(); }

            var count = manga.Chapters.Count;
            var paginatedChapters = manga.Chapters.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var chaptersViewModel = _mapper.Map<List<ChapterListViewModel>>(paginatedChapters);
            var pageList = new PageList<ChapterListViewModel>(chaptersViewModel, count, pageNumber, pageSize);

            var mangaViewModel = _mapper.Map<MangaViewModel>(manga);
            mangaViewModel.Chapters = pageList;

            manga.Viewed();
            _context.SaveChanges();

            return Ok(mangaViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromForm] MangaInputModel mangaInput)
        {
            var result = _validator.Validate(mangaInput);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) {  return BadRequest(errors); }

            try
            {
                string coverURL = await UploadFile(mangaInput.Cover);
                string bannerURL = await UploadFile(mangaInput.Banner);

                Mangas manga = new(coverURL, bannerURL, mangaInput.Title, mangaInput.Description, mangaInput.Tags, mangaInput.Release, mangaInput.Status, mangaInput.Authors, mangaInput.Artists);

                _context.Mangas.Add(manga);
                _context.SaveChanges();

                var mangaViewModel = _mapper.Map<MangaViewModel>(manga);

                return CreatedAtAction(nameof(GetById), new { id = manga.Id }, mangaViewModel);
            } 
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpPost("{id}/like")]
        public IActionResult Like(Guid id)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == id && !manga.IsDeleted);

            if (manga == null) { return NotFound(); }

            manga.Liked();
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, MangaInputModel mangaInput)
        {
            var result = _validator.Validate(mangaInput);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) { return BadRequest(errors); }

            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == id);

            if (manga == null) { return NotFound(); }

            try
            {
                string coverURL = await UploadFile(mangaInput.Cover);
                string bannerURL = await UploadFile(mangaInput.Banner);

                manga.Update(coverURL, bannerURL, mangaInput.Title, mangaInput.Description, mangaInput.Tags, mangaInput.Release, mangaInput.Status, mangaInput.Authors, mangaInput.Artists);

                _context.Mangas.Update(manga);
                _context.SaveChanges();

                return NoContent();
            } 
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(Guid id)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == id);

            if (manga == null) { return NotFound(); }

            manga.Delete();
            _context.SaveChanges();

            return NoContent();
        }

        private async Task<string> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File cannot be null or empty.");
            }

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                PublicId = Guid.NewGuid().ToString(),
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.ToString();
            }

            throw new Exception("Error uploading file.");
        }

    }
}
