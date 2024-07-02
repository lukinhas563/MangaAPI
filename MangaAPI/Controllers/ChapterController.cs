using AutoMapper;
using MangaAPI.Entities;
using MangaAPI.Models.Input;
using MangaAPI.Models.Pagination;
using MangaAPI.Models.View;
using MangaAPI.Persistence;
using MangaAPI.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MangaAPI.Controllers
{
    [Route("api/mangas/{mangaId}/chapters")]
    [ApiController]
    public class ChapterController : ControllerBase
    {
        private readonly MangasContext _context;
        private readonly IMapper _mapper;
        private readonly CreateChapterValidator _validator;

        public ChapterController(MangasContext context, IMapper mapper, CreateChapterValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get(Guid mangaId, int page = 0, int take = 10)
        {
            var chapters = _context.Chapters
                .Where(chapter => chapter.MangaId == mangaId && !chapter.IsDeleted).Skip(page).Take(take).ToList();

            var chaptersViewModel = _mapper.Map<List<ChapterListViewModel>>(chapters);
            var count = chapters.Count();

            var response = new PageList<ChapterListViewModel>(chaptersViewModel, count, page, take);

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Premium,Admin")]
        public IActionResult GetById(Guid mangaId, Guid id)
        {
            var chapter = _context.Chapters
                .Include(chapter => chapter.Pages)
                .SingleOrDefault(chapter => chapter.Id == id && !chapter.IsDeleted);

            if (chapter == null) { return  NotFound(); }

            var chapterViewModel = _mapper.Map<ChapterViewModel>(chapter);

            return Ok(chapterViewModel);
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post(Guid mangaId, ChapterInputModel chapterInput)
        {
            var result = _validator.Validate(chapterInput);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) { return BadRequest(errors); }

            try
            {
                Chapter chapter = new(chapterInput.Title, chapterInput.Number, chapterInput.Release, mangaId);

                _context.Chapters.Add(chapter);
                _context.SaveChanges();

                var chapterViewModel = _mapper.Map<ChapterViewModel>(chapter);

                return CreatedAtAction(nameof(GetById), new { mangaId = mangaId, id = chapter.Id }, chapterViewModel);

            }
            catch (DbUpdateException)
            {
                return BadRequest("The chapter already exists for this manga.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(Guid mangaId, Guid id, ChapterInputModel chapterInput)
        {
            var result = _validator.Validate(chapterInput);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) { return BadRequest(errors); }

            var chapter = _context.Chapters.SingleOrDefault(chapter => chapter.MangaId == mangaId && chapter.Id == id);

            if (chapter == null) { return NotFound(); };

            chapter.Update(chapterInput.Title, chapterInput.Number, chapterInput.Release);

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(Guid mangaId, Guid id)
        {
            var chapter = _context.Chapters.SingleOrDefault(chapter => chapter.MangaId == mangaId && chapter.Id == id);

            if (chapter == null) { return NotFound(); };

            chapter.Delete();

            _context.SaveChanges();

            return NoContent();
        }

    }
}
