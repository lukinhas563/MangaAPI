using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MangaAPI.Entities;
using MangaAPI.Models.Input;
using MangaAPI.Models.View;
using MangaAPI.Persistence;
using MangaAPI.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MangaAPI.Controllers
{
    [Route("api/chapters/{chapterId}/pages")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly MangasContext _context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly CreatePageValidation _validator;

        public PageController(MangasContext context, IMapper mapper, Cloudinary cloudinary, CreatePageValidation validator)
        {
            _context = context;
            _mapper = mapper;
            _cloudinary = cloudinary;
            _validator = validator;
        }

        [HttpGet]
        [Authorize(Roles = "Premium,Admin")]
        public IActionResult GetAll(Guid chapterId)
        {
            var pages = _context.Pages.Where(page => page.ChapterId == chapterId && !page.IsDeleted);
            var pageViewModel = _mapper.Map<List<PageViewModel>>(pages);

            return Ok(pageViewModel);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Premium,Admin")]
        public IActionResult GetById(Guid chapterId, Guid id)
        {
            var page = _context.Pages.SingleOrDefault(page => page.ChapterId == chapterId && page.Id == id && !page.IsDeleted);

            if (page == null) { return NotFound(); }

            var pageViewModel = _mapper.Map<PageViewModel>(page);

            return Ok(pageViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(Guid chapterId, [FromForm] PageInputModel pageInput)
        {
            var result = _validator.Validate(pageInput);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) { return BadRequest(errors); }

            try
            {
                string pageURL = await UploadFile(pageInput.Url);

                PageManga page = new(pageInput.Order, pageURL, chapterId);

                _context.Pages.Add(page);
                _context.SaveChanges();

                var pageViewModel = _mapper.Map<PageViewModel>(page);

                return CreatedAtAction(nameof(GetById), new { chapterId = chapterId, id = page.Id }, pageViewModel);
            } 
            catch (DbUpdateException)
            {
                return BadRequest("The page already exists for this manga.");
            } 
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid chapterId, Guid id, [FromForm] PageInputModel pageInput)
        {
            var result = _validator.Validate(pageInput);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) { return BadRequest(errors); }

            var page = _context.Pages.SingleOrDefault(page => page.ChapterId == chapterId && page.Id == id);

            if (page == null) { return NotFound(); };

            try
            {
                var pageURL = await UploadFile(pageInput.Url);

                page.Update(pageInput.Order, pageURL);

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
        public IActionResult Delete(Guid chapterId, Guid id)
        {
            var page = _context.Pages.SingleOrDefault(page => page.ChapterId == chapterId && page.Id == id);

            if (page == null) { return NotFound(); }

            page.Delete();
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

            throw new Exception("Erro upload file.");
        }
    }
}
