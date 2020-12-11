using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScwSvc.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ScwSvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private const string Pepper = "scw-";

        private readonly ILogger<ServiceController> _logger;
        private readonly DbSysContext _db;

        public ServiceController(ILogger<ServiceController> logger, DbSysContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> Register([FromBody] AuthenticationModel loginCredentials)
        {
            _logger.LogInformation("Register attempt: user=\"" + loginCredentials.Username + "\"");

            if (await _db.Users.AnyAsync(u => u.Name == loginCredentials.Username).ConfigureAwait(false))
                return BadRequest("User with this name already exists.");

            var newUserId = Guid.NewGuid();
            await _db.Users.AddAsync(new User()
            {
                UserId = newUserId,
                Name = loginCredentials.Username,
                PasswordHash = HashUserPassword(newUserId, loginCredentials.Password),
                Role = UserRole.Common
            }).ConfigureAwait(false);
            await _db.SaveChangesAsync().ConfigureAwait(false);

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
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> Login([FromBody] AuthenticationModel loginCredentials)
        {
#if ENABLE_AD_AUTH
            return await LoginWithAD(loginCredentials);
#elif ENABLE_DB_AUTH
            return await LoginWithDB(loginCredentials);
#elif DEBUG
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(Roles.All.Select(r => new Claim(ClaimTypes.Role, r)).Append(new Claim(ClaimTypes.NameIdentifier, loginCredentials.Username)), CookieAuthenticationDefaults.AuthenticationScheme)),
                new AuthenticationProperties() { IsPersistent = true }).ConfigureAwait(false);
            _logger.LogWarning("Auto-Login with full privileges.");
            return Ok();
#else
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Authentication is disabled.");
#endif
        }

        [HttpPost("login/db")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> LoginWithDB([FromBody] AuthenticationModel loginCredentials)
        {
            _logger.LogInformation("Login attempt: user=\"" + loginCredentials.Username + "\"");

            if (_db.Users.FirstOrDefault(u => u.Name == loginCredentials.Username) is User user)
            {
                var enteredPassword = HashUserPassword(user.UserId, loginCredentials.Password);

                if (enteredPassword.SequenceEqual(user.PasswordHash))
                {
                    var cp = new ClaimsPrincipal(new ClaimsIdentity(
                            new[] { new Claim(ClaimTypes.Role, user.Role.ToString()), new Claim(ClaimTypes.NameIdentifier, user.UserId.ToNameString()), new Claim(ClaimTypes.Name, user.Name) },
                        CookieAuthenticationDefaults.AuthenticationScheme));
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, cp, new AuthenticationProperties() { IsPersistent = true }).ConfigureAwait(false);
                    _logger.LogInformation("Login: user=\"" + loginCredentials.Username + "\"");

                    return Ok();
                }

                _logger.LogInformation("Login fail: user=\"" + loginCredentials.Username + "\"");
                return BadRequest("Incorrect password.");
            }

            return BadRequest("User not found.");
        }

#if ENABLE_AD_AUTH
        [HttpPost("login/ad")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> LoginWithAD([FromBody] AuthenticationModel loginCredentials)
        {
            _logger.LogInformation("Service AUTH: login attempt; user=\"" + loginCredentials.Username + "\"");

            if (!AuthenticateAndAuthorizeWithAD(loginCredentials.Username, loginCredentials.Password, out string error, out ClaimsIdentity identity))
                return BadRequest(error);

            // see https://stackoverflow.com/a/37090696
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity), new AuthenticationProperties() { IsPersistent = true }).ConfigureAwait(false);
            _logger.LogInformation("Service AUTH: login; user=\"" + loginCredentials.Username + "\"");
            return Ok();
        }
#endif

        /// <summary>
        /// Performs logout for logged-in users.
        /// Does not complain if user wasn't logged in when calling.
        /// </summary>
        /// <returns>200 always</returns>
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
            => StatusCode(StatusCodes.Status401Unauthorized, "You are currently not logged in.");

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
            _logger.LogWarning("Service AUTH: user tried accessing forbidden URL; user=\"" + User.FindFirstValue(ClaimTypes.NameIdentifier) + "\"; query=\"" + from + "\"");
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

#if ENABLE_AD_AUTH
        // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-3.0
        private bool AuthenticateAndAuthorizeWithAD(string username, string password, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(true)] out string? error, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ClaimsIdentity? identity)
        {
            identity = null;
            using var user = UserPrincipal.FindByIdentity(CurrentPrincipalContext, IdentityType.SamAccountName, username); // not async!

            if (user == null)
            {
                error = "User does not exist.";
                return false;
            }

            if (user.IsAccountLockedOut())
            {
                error = "User is locked out.";
                return false;
            }

            if (!CurrentPrincipalContext.ValidateCredentials(username, password))
            {
                error = "Invalid credentials.";
                return false;
            }

            error = null;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.SamAccountName, ClaimValueTypes.String)
            };

            var groups = user.GetGroups().Select(g => g.SamAccountName).ToArray(); // not async!

            foreach (var role in AuthorizedGroups.Where(g => groups.Contains(g.Key)).SelectMany(r => r.Value))
                claims.Add(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String));

            identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return true;
        }
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
                    await _db.Users.AddAsync(new User()
                    {
                        UserId = userId,
                        Name = "test" + i,
                        Role = (UserRole)i,
                        PasswordHash = HashUserPassword(userId, "test")
                    }).ConfigureAwait(false);
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

        /// <summary>
        /// Hashes a users's password.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <param name="pass">Password of the user.</param>
        /// <returns>Hashed password of the user.</returns>
        private static byte[] HashUserPassword(Guid userId, in string pass)
        {
            var hasher = SHA256.Create();
            var combination = Encoding.UTF8.GetBytes(Pepper + userId.ToNameString() + pass);
            return hasher.ComputeHash(combination);
        }

        /// <summary>
        /// Compare two memory areas for equal content.
        /// Basically like memcmp().
        /// </summary>
        /// <param name="left">First memory area.</param>
        /// <param name="right">Second memory area.</param>
        /// <returns>Whether or not the two areas contain the same content.</returns>
        private static bool CompareHashes(in ReadOnlySpan<byte> left, in ReadOnlySpan<byte> right)
            => left.SequenceEqual(right);
    }
}
