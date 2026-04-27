using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SQLitePCL;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Infrastructure.Configuration;

namespace Library.Auth.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IGoogleService googleService;
        private readonly IOptions<GoogleConfiguration> googleConfiguration;

        public AuthController(IUserRepository userRepository,
            IGoogleService googleService,
            IOptions<GoogleConfiguration> googleConfiguration)
        {
            this.userRepository = userRepository;
            this.googleService = googleService;
            this.googleConfiguration = googleConfiguration;
        }

        [HttpGet]
        public async Task<IActionResult> SignIn()
        {
            var url = googleService.GetRedirectLink();
            return Redirect(url);
        }

        [HttpGet]
        public async Task<IActionResult> CallBack(string code, CancellationToken ct = default)
        {
            var idToken = await googleService.GetIdToken(code);

            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { googleConfiguration.Value.ClientId }
            });

            var user = await userRepository.GetUserByEmail(payload.Email, ct);
            if (user == null)
            {
                user = new Tekwill.Library.Domain.Entities.User
                {
                    Email = payload.Email
                };
                await userRepository.CreateUser(user, ct);
            }
            //var token = jw
            return Ok();
        }
    }
}
