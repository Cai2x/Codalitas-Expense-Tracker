using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Authentication;
using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Controllers
{
    public class AccountController : ControllerBase<AccountController>
    {
        private readonly SessionManager _sessionManager;
        private readonly SignInManager _signInManager;
        private readonly TokenValidationParametersFactory _tokenValidationParametersFactory;
        private readonly TokenProviderOptionsFactory _tokenProviderOptionsFactory;
        private readonly IConfiguration _appConfiguration;
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="localizer">The localizer.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="tokenValidationParametersFactory">The token validation parameters factory.</param>
        /// <param name="tokenProviderOptionsFactory">The token provider options factory.</param>
        public AccountController(
                            SignInManager signInManager,
                            IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory loggerFactory,
                            IConfiguration configuration,
                            IMapper mapper,
                            IUserService userService,
                            TokenValidationParametersFactory tokenValidationParametersFactory,
                            TokenProviderOptionsFactory tokenProviderOptionsFactory) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._sessionManager = new SessionManager(this._session);
            this._signInManager = signInManager;
            this._tokenProviderOptionsFactory = tokenProviderOptionsFactory;
            this._tokenValidationParametersFactory = tokenValidationParametersFactory;
            this._appConfiguration = configuration;
            this._userService = userService;
        }

        /// <summary>
        /// Login Method
        /// </summary>
        /// <returns>Created response view</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            TempData["returnUrl"] = System.Net.WebUtility.UrlDecode(HttpContext.Request.Query["ReturnUrl"]);
            this._sessionManager.Clear();
            this._session.SetString("SessionId", System.Guid.NewGuid().ToString());
            return this.View();
        }

        /// <summary>
        /// Authenticate user and signs the user in when successful.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns> Created response view </returns>
        /// 
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            User user = null;

            this._session.SetString("HasSession", "Exist");

            var loginResult = _userService.AuthenticateUser(model.Username, model.Password, ref user);
            if (loginResult == LoginResult.Success)
            {
                // 認証OK
                await this._signInManager.SignInAsync(user);
                this._session.SetString("UserName", user.FirstName);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // 認証NG
                TempData["ErrorMessage"] = "Incorrect Username or Password";
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetUserDetails(int id)
        {
            try
            {
                var expense = _userService.RetrieveUser(id);
                return Ok(expense);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> EditAsync(EditProfileModel model)
        {
            try
            {
                _userService.UpdateUser(model);
                var user = _userService.GetUserPass(model.UserId);
                var login = new LoginViewModel
                {
                    Username = user.Username,
                    Password = user.Password,
                };

                User resetUser = null;
                this._session.SetString("HasSession", "Exist");

                var resetClaim = _userService.AuthenticateUser(login.Username,login.Password,ref resetUser);
                if (resetClaim == LoginResult.Success)
                {
                    await _signInManager.SignInAsync(resetUser);
                    this._session.SetString("UserName", resetUser.FirstName);
                    return RedirectToAction("Index", "Settings");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error during profile update. Please try again.";
                    return RedirectToAction("Index", "Settings");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(UserViewModel model)
        {
            try
            {
                _userService.AddUser(model);
                TempData["SuccessMessage"] = "Registration successful! Please log In.";

                return RedirectToAction("Login", "Account");
            }
            catch(InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch(Exception ex)
            {
                //TempData["ErrorMessage"] = Resources.Messages.Errors.ServerError;
                TempData["ErrorMessage"] = ex.Message;
                throw new InvalidDataException(ex.Message);
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ChangePassword(ForgotPasswordModel model)
        {
            try
            {
                var userId = int.Parse(UserId);
                if (userId == 0)
                {
                    TempData["ErrorMessage"] = Resources.Messages.Errors.UserNotFound;
                    return View();
                }

                var changepass = _userService.ChangePassword(userId, model.OldPassword, model.NewPassword);

                if (changepass)
                {
                    TempData["SuccessMessage"] = "Password Changed Successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Change Password Failed";
                }
                

                return RedirectToAction("Index", "Settings");
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.Errors.ServerError;
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["ErrorMessage"] = "Email cannot be empty.";
                return View("ForgotPassword");
            }
            try
            {
                var forgotpass = await _userService.SendPasswordResetEmailAsync(email);

                if (forgotpass)
                {
                    TempData["SuccessMessage"] = "Reset link sent successfully!.";
                }
                //return Ok(forgotpass);
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                //return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.Errors.ServerError;

                //return BadRequest(ex.Message);

            }

            return View("ForgotPassword");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ResetPassword(string newPassword, string token)
        {
            try
            {
                var forgotpass = _userService.ResetPassword(newPassword, token);

                if (forgotpass)
                {
                    TempData["SuccessMessage"] = "Password changed successfully.";

                    return RedirectToAction("Login", "Account");
                }

                TempData["ErrorMessage"] = "Reset Password Token has expired";
                return RedirectToAction("ForgotPassword", "Account");

            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.Errors.ServerError;
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Sign Out current account and return login view.
        /// </summary>
        /// <returns>Created response view</returns>
        [AllowAnonymous]
        public async Task<IActionResult> SignOutUser()
        {
            await this._signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
