using AdminiBackend.Entities;
using AdminiDomain.Entities;
using AdminiDomain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminiBackend.Pages.Panel.Users
{
  [Authorize(Roles = "Admin")]
  public class EditModel : PageModel
  {
    private readonly UserService userService;

    public EditModel(UserService userService)
    {
      this.userService = userService;
    }

    [BindProperty]
    public User EditUser { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
      if (id is null)
      {
        return RedirectToPage("./Index", new { alert = AlertType.Error, text = "Record Id is null." });
      }
      var user = await userService.GetAsync(user => user.Id == id);
      if (user is null)
      {
        return RedirectToPage("./Index", new { alert = AlertType.Error, text = $"Record with id={id} not found." });
      }
      EditUser = user;
      EditUser.Password = CryptographyService.Decrypt(EditUser.Password);
      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      EditUser.Password = CryptographyService.Encrypt(EditUser.Password);
      await userService.SaveAsync(EditUser);
      return RedirectToPage("./Index", new { alert = AlertType.Success, text = "Record has been edited." });
    }
  }
}
