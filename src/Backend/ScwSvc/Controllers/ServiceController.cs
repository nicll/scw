using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScwSvc.Repositories;
using ScwSvc.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static ScwSvc.Utils.Authentication;

namespace ScwSvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly ILogger<ServiceController> _logger;
        private readonly DbSysContext _db;

        public ServiceController(ILogger<ServiceController> logger, DbSysContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> Register([FromBody] AuthenticationModel loginCredentials)
        {
            _logger.LogInformation("Register attempt: user=\"" + loginCredentials.Username + "\"");

            if (await _db.IsUsernameAssigned(loginCredentials.Username))
                return BadRequest("User with this name already exists.");

            var newUserId = Guid.NewGuid();
            await _db.AddUser(new User()
            {
                UserId = newUserId,
                Name = loginCredentials.Username,
                PasswordHash = HashUserPassword(newUserId, loginCredentials.Password),
                Role = UserRole.Common
            });
            await _db.SaveChangesAsync().ConfigureAwait(false);

            var cp = new ClaimsPrincipal(new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.Role, nameof(UserRole.Common)), new Claim(ClaimTypes.NameIdentifier, newUserId.ToNameString()) },
                CookieAuthenticationDefaults.AuthenticationScheme));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, cp, new AuthenticationProperties() { IsPersistent = true }).ConfigureAwait(false);

            _logger.LogInformation("Register: user=\"" + loginCredentials.Username + "\"");
            return Ok();
        }

        /// <summary>
        /// Performs redimentary login using the specified credentials.
        /// Grants a session cookie if successful.
        /// Only works if feature enabled during compilation or when running in debug mode.
        /// </summary>
        /// <param name="loginCredentials">Credentials the user wants to login with</param>
        /// <returns>200 and cookie if successful; 400 on validation fail; 503 if disabled</returns>
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> Login([FromBody] AuthenticationModel loginCredentials)
        {
#if ENABLE_DB_AUTH
            return await LoginWithDB(loginCredentials);
#else
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Authentication is disabled.");
#endif
        }

        [HttpPost(nameof(Login) + "/db")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> LoginWithDB([FromBody] AuthenticationModel loginCredentials)
        {
            _logger.LogInformation("Login attempt: user=\"" + loginCredentials.Username + "\"");

            var user = await _db.GetUserByName(loginCredentials.Username);

            if (user is null)
                return BadRequest("User not found.");

            var enteredPassword = HashUserPassword(user.UserId, loginCredentials.Password);

            if (CompareHashes(enteredPassword, user.PasswordHash))
            {
                var cp = new ClaimsPrincipal(new ClaimsIdentity(
                        new[] { new Claim(ClaimTypes.Role, user.Role.ToString()), new Claim(ClaimTypes.NameIdentifier, user.UserId.ToNameString()) },
                    CookieAuthenticationDefaults.AuthenticationScheme));
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, cp, new AuthenticationProperties() { IsPersistent = true }).ConfigureAwait(false);
                _logger.LogInformation("Login: user=\"" + loginCredentials.Username + "\"");

                return Ok();
            }

            _logger.LogInformation("Login fail: user=\"" + loginCredentials.Username + "\"");
            return BadRequest("Incorrect password.");
        }

        /// <summary>
        /// Performs logout for logged-in users.
        /// Does not complain if user wasn't logged in when calling.
        /// </summary>
        /// <returns>200 always</returns>
        [HttpGet("[action]")] // ToDo: temporary fix for frontend
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async ValueTask<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Users get redirected to this URL when calling a privileged function without being logged in.
        /// Only tells users that they need to log in.
        /// </summary>
        /// <param name="from">The privileged URL that the user attempted to access</param>
        /// <returns>401 always</returns>
        [HttpGet("[action]")]
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<string> Unauthenticated([FromQuery] string from)
        {
            _logger.LogWarning("Unauthenticated: unauthenticated user tried accessing URL; remote=\"" + HttpContext.Connection.RemoteIpAddress + "\"; query=\"" + from + "\"");
            return StatusCode(StatusCodes.Status401Unauthorized, "You are currently not logged in.");
        }

        /// <summary>
        /// Users get redirected to this URL when calling a privileged function without the necessary rights.
        /// Logs attempt and tells user they don't have the needed rights.
        /// </summary>
        /// <param name="from">The privileged URL that the user attempted to access</param>
        /// <returns>403 always</returns>
        [HttpGet("[action]")]
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<string> Unauthorized([FromQuery] string from)
        {
            _logger.LogWarning("Unauthorized: user tried accessing forbidden URL; user=\"" + User.FindFirstValue(ClaimTypes.NameIdentifier) + "\"; query=\"" + from + "\"");
            return StatusCode(StatusCodes.Status403Forbidden, "You are not allowed to access this URL.");
        }

        /// <summary>
        /// Gets the roles of the currently logged-in user.
        /// </summary>
        /// <returns>200 always</returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string[]> MyRoles()
            => Ok(User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray());

#if DEBUG
        /// <summary>
        /// Gets the roles of the currently logged-in user.
        /// </summary>
        /// <returns>200 always</returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string[]> MyClaims()
            => Ok(User.Claims.Select(c => c.Value).ToArray());
#endif

#if DEBUG
        // demo users for testing
        [HttpPost("createDemoUsers")]
        public async ValueTask<IActionResult> CreateDemoUsers()
        {
            try
            {
                for (int i = 0; i < 3; ++i)
                {
                    var userId = Guid.NewGuid();
                    await _db.AddUser(new User()
                    {
                        UserId = userId,
                        Name = "test" + i,
                        Role = (UserRole)i,
                        PasswordHash = HashUserPassword(userId, "test")
                    });
                }

                await _db.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
#endif
    }
}
