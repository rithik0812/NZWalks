using Microsoft.AspNetCore.Mvc;
using NZWalksAPI.Repository;

namespace NZWalksAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private IUserRepository userRepository;
        private ITokenHandler tokenHandler;

        public AuthController(IUserRepository userRepository, ITokenHandler tokenHandler)
        {
            this.userRepository = userRepository;
            this.tokenHandler = tokenHandler;
        }
        
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(Models.DTO.LoginRequest loginRequest)
        {
            // validate username and password 
            if (ValidateLogin(loginRequest) == false)
            {
                return BadRequest(ModelState);
            }

            // check if user if authenticated (does the username and password exist in DB ? )

            var AuthenticatedUser = await userRepository.AuthenticateAsync(loginRequest.UserName, loginRequest.Password);

            if (AuthenticatedUser != null ) 
            {
                // GENERATE Jwt token
                var token = await tokenHandler.CreateTokenAsync(AuthenticatedUser);
                return Ok(token);

            }

            return BadRequest("Username or Password if incorrect");

        }

        #region Private Methods

        // validation of username and password of login request : 
        private bool ValidateLogin(Models.DTO.LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.UserName))
            {
                ModelState.AddModelError(nameof(loginRequest.UserName), $"{nameof(loginRequest.UserName)} cannot be null/ empty / whitespace");
            }

            if (string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                ModelState.AddModelError(nameof(loginRequest.Password), $"{nameof(loginRequest.Password)} cannot be null/ empty / whitespace");
            }

            // if any of the validation checks fail then return false
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            // if the all the validation chacks pass then return true
            return true;

        }

        #endregion


    }
}
