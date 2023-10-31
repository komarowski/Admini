using AdminiBackend.DTO;
using AdminiBackend.Entities;
using AdminiBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminiBackend.Pages.Panel
{
    public class LoginModel : PageModel
  {
    private readonly AuthService authenticationService;
    private readonly ILogger<LoginModel> logger;

    public LoginModel(AuthService authenticationService, ILogger<LoginModel> logger)
    {
      this.authenticationService = authenticationService;
      this.logger = logger;
    }

    [BindProperty]
    public LoginDTO LoginForm { get; set; } = default!;

    public ResponseAlert? Alert { get; set; }

    public async Task<IActionResult> OnGetAsync(AlertType? alert, string? text, string? action)
    {
      if (action == "signout")
      {
        await authenticationService.SignOut();
        return RedirectToPage("/Panel/Login");
      }
      if (alert is not null && text is not null)
      {
        this.Alert = ResponseAlert.GetAlert((AlertType)alert, text);
      }
      return Page();
    }

    public async Task<IActionResult> OnPostLoginAsync()
    {
      var user = await authenticationService.SignIn(LoginForm);
      if (user is null)
      {
        return RedirectToPage(new { alert = AlertType.Error, text = $"User not found." });
      }
      logger.LogWarning("Sign in '{0}' user", user.Name);
      return RedirectToPage("/Panel/Notes/Index");
    }
  }
}
