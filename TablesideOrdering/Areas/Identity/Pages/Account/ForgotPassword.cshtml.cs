// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using MimeKit;
using TablesideOrdering.Models;

namespace TablesideOrdering.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<LoginModel> _logger;

        private readonly Email _email;

        public INotyfService _notyfService { get; }
        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender, INotyfService notyfService, IOptions<Email> email, SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _notyfService = notyfService;
            _email = email.Value;
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var signIn = _signInManager.IsSignedIn(User);
            if (signIn == false)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    _notyfService.Error("Your email doesn't exist!", 5);
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                string LinkURL = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                SendMail(LinkURL);
                _notyfService.Success("The email has been sent!", 5);
                return Page();
            }
            else
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out.");
                _notyfService.Success("The email has been sent!", 5);
                var url = Url.RouteUrl("areas", new { controller = "Account", action = "Login", area = "Identity" });
                if (url != null)
                {
                    return LocalRedirect(url);
                }
                else
                {
                    return RedirectToPage();
                }
            }
        }

        public void SendMail(string LinkURL)
        {
            Email data = new Email();
            data.EmailFrom = _email.EmailFrom;
            data.Password = _email.Password;
            data.Body = "Please reset your password by <a href=" + $"{LinkURL}" + "> click here</a>";

            var email = new MimeMessage();
            {
                email.From.Add(MailboxAddress.Parse(data.EmailFrom));
                email.To.Add(MailboxAddress.Parse(Input.Email));
                email.Subject = "L&L Coffee Recovery Password";
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = data.Body };
            }

            using var smtp = new SmtpClient();
            {
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(data.EmailFrom, data.Password);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }
    }
}
