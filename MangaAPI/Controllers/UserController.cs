using AutoMapper;
using MangaAPI.Entities;
using MangaAPI.Models.Input;
using MangaAPI.Models.View;
using MangaAPI.Persistence;
using MangaAPI.Services;
using MangaAPI.Utils;
using MangaAPI.Validators;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MangaAPI.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MangasContext _context;
        private readonly IMapper _mapper;
        private readonly TokenService _tokenService;
        private readonly CreateUserValidator _validator;

        public UserController(MangasContext context, IMapper mapper, TokenService tokenService, CreateUserValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
            _validator = validator;
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Invalid username or password.");
            }

            var user = _context.Users.SingleOrDefault(user => user.Username == username && !user.isDeleted);

            if (user == null) { return NotFound(); }

            Cryptography hash = new Cryptography();

            if (hash.Verify(user.Password, password))
            {
                string token = _tokenService.Generate(user);

                return Ok(token);
            } 
            else
            {
                return BadRequest("Invalid username or password.");
            }
        }

        [HttpPost("register")]
        public IActionResult Register(UserInputModel userInput)
        {
            var result = _validator.Validate(userInput);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) { return BadRequest(errors); }

            try
            {
                Cryptography hash = new Cryptography();

                User user = new User(userInput.Name, userInput.LastName, userInput.Username, userInput.Email, hash.Hash(userInput.Password));
                _context.Users.Add(user);
                _context.SaveChanges();

                var viewModel = _mapper.Map<UserViewModel>(user);

                return Ok(viewModel);

            } 
            catch (DbUpdateException)
            {
                return BadRequest("Invalid email or username.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, string passwordInput, UserInputModel userInput)
        {
            var result = _validator.Validate(userInput);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) { return BadRequest(errors); }

            var user = _context.Users.SingleOrDefault(user => user.Id == id && !user.isDeleted);

            if (user == null) { return NotFound(); };

            Cryptography hash = new Cryptography();
            if (hash.Verify(passwordInput, user.Password))
            {
                user.Update(userInput.Name, userInput.LastName, userInput.Username, userInput.Email, hash.Hash(userInput.Password));

                _context.SaveChanges();

                return NoContent();
            } 
            else
            {
                return BadRequest("Invalid email or username.");
            }




        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var user = _context.Users.SingleOrDefault(user => user.Id == id);

            if (user == null) { return NotFound(); }

            user.Delete();

            _context.SaveChanges();

            return NoContent();

        }
    }
}
