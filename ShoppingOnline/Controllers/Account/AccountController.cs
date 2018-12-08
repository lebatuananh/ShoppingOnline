using System;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using ShoppingOnline.Application.Systems.Users;
using ShoppingOnline.Application.Systems.Users.Dtos;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.Data.Enum;
using ShoppingOnline.Utilities.Constants;
using ShoppingOnline.WebApplication.Controllers.Home;
using ShoppingOnline.WebApplication.Extensions;
using ShoppingOnline.WebApplication.Models.AccountViewModel;
using ShoppingOnline.WebApplication.Services;

namespace ShoppingOnline.WebApplication.Controllers.Account
{
   public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAppUserService _userService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender, ILogger<AccountController> logger,
            IAppUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _userService = userService;
        }

        [Authorize]
        [Route("account.html", Name = "Account")]
        public async Task<IActionResult> Index()
        {
            ViewData["BodyClass"] = "dashboard_page";

            var id = User.GetSpecificClaim("UserId");

            var model = await _userService.GetByIdAsync(id);

            return View(model);
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        [Route("login.html", Name = "Login")]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("login.html", Name = "Login")]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (IsValid(model.Username))
                {
                    var user = await _userManager.FindByEmailAsync(model.Username);
                    result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                }

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("register.html", Name = "Register")]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateRecaptcha]
        [Route("register.html", Name = "Register")]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var error = 0;

            var checkPhone = _userService.CheckPhoneNumber(model.PhoneNumber);

            if (checkPhone == false)
            {
                ModelState.AddModelError(model.PhoneNumber, $"Phone number '{model.PhoneNumber}' is already taken.");
                error++;
            }

            if (!ModelState.IsValid)
            {
                error++;
            }

            //MM/dd/yyy
            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                BirthDay = model.BirthDay,
                Status = Status.Active,
                Avatar = string.Empty,
                Gender = model.Gender,
                Address = model.Address,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded || error > 0)
            {
                AddErrors(result);

                // If we got this far, something failed, redisplay form
                return View(model);
            }

            _logger.LogInformation("User created a new account with password.");
            await _userManager.AddToRoleAsync(user, "Customer");
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);

            await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            //await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User created a new account with password.");

            return RedirectToLocal("/login.html");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);

                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    var isLogin = await _userManager.AddLoginAsync(user, info);

                    if (isLogin.Succeeded)
                    {
                        Response.Cookies.Delete("Default");
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }

                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            var checkPhone = _userService.CheckPhoneNumber(model.PhoneNumber);

            if (checkPhone == false)
            {
                ModelState.AddModelError(model.PhoneNumber, $"Phone number '{model.PhoneNumber}' is already taken.");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }

                var user = new AppUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    BirthDay = DateTime.Parse(model.DOB),
                    Status = Status.Active,
                    Avatar = string.Empty,
                    Gender = model.Gender,
                    Address = model.Address,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Customer");
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);

                        await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                return View("ConfirmEmail");
            }
            return View("Error");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("forgot-password.html", Name = "ForgotPassword")]
        public async Task<IActionResult> ForgotPassword()
        {
            await _signInManager.SignOutAsync();
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("forgot-password.html", Name = "ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    ModelState.AddModelError(model.Email, "Email address: " + model.Email + " does not exist");
                    return View(model);
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var code = await _userManager.GenerateUserTokenAsync(user, SystemConstants.TokenProvider.EmailConfirm, SystemConstants.TokenProvider.Passwordless);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>. <a href='{callbackUrl}'>Link</a> that should expired in 24 hrs");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("forgot-password-confirmation.html", Name = "ForgotPasswordConfirmation")]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("reset-password.html", Name = "ResetPassword")]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("reset-password.html", Name = "ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                // Don't reveal that the user does not exist
                ModelState.AddModelError(model.Email, "Email address: " + model.Email + " does not exist");
                return View(model);
            }

            var isValid = await _userManager.VerifyUserTokenAsync(user, SystemConstants.TokenProvider.EmailConfirm, SystemConstants.TokenProvider.Passwordless, model.Code);
            if (!isValid)
            {
                ModelState.AddModelError(model.Code, "Email has expired");
                return View(model);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            IdentityResult.Failed(new IdentityError { Description = "User names must contain the string 'allowed'" });

            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("reset-password-confirmation.html", Name = "ResetPasswordConfirmation")]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        [Route("update-account.html", Name = "UpdateAccount")]
        public async Task<IActionResult> UpdateAccount()
        {
            var userId = User.GetUserId().ToString();

            var user = await _userService.GetByIdAsync(userId);

            return View(user);
        }

        [HttpPost]
        [Authorize]
        [Route("update-account.html", Name = "UpdateAccount")]
        public async Task<IActionResult> UpdateAccount(AppUserViewModel userVm)
        {
            if (!ModelState.IsValid)
            {
                return View(userVm);
            }
            else
            {
                var error = 0;

                var isValidByPhoneNumber = await _userService.CheckUpdatePhoneNumber(userVm.PhoneNumber, userVm.Id.ToString());

                if (isValidByPhoneNumber == false)
                {
                    ModelState.AddModelError(userVm.PhoneNumber, $"Phone number '{userVm.PhoneNumber}' is already taken.");
                    error++;
                }

                if (!ModelState.IsValid)
                {
                    error++;
                }

                //MM/dd/yyy
                var appUser = await _userManager.FindByIdAsync(userVm.Id.ToString());
                appUser.Email = userVm.Email;
                appUser.FullName = userVm.FullName;
                appUser.PhoneNumber = userVm.PhoneNumber;
                appUser.BirthDay = userVm.BirthDay;
                appUser.Gender = userVm.Gender;
                appUser.Address = userVm.Address;
                appUser.DateModified = DateTime.Now;

                var isUpdate = await _userManager.UpdateAsync(appUser);

                if (!isUpdate.Succeeded || error > 0)
                {
                    AddErrors(isUpdate);
                    return View(userVm);
                }
                return RedirectToLocal("/account.html");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("update-password.html", Name = "UpdatePassword")]
        public IActionResult UpdatePassword()
        {
            return View(new PasswordViewModel());
        }

        [HttpPost]
        [Authorize]
        [Route("update-password.html", Name = "UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(PasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await _userManager.FindByIdAsync(User.GetUserId().ToString());

            var success = await _userManager.ChangePasswordAsync(user, viewModel.CurrentPassword, viewModel.Password);

            if (success.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                await _signInManager.SignOutAsync();

                return RedirectToLocal("/update-password-confirmation.html");
            }

            AddErrors(success);

            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("update-password-confirmation.html", Name = "UpdatePasswordConfirmation")]
        public IActionResult UpdatePasswordConfirmation()
        {
            return View();
        }

        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion Helpers
    }
}