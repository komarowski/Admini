using AdminiBackend.Entities;
using AdminiDomain.Entities;
using AdminiDomain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminiBackend.Pages.Panel.Users
{
  [Authorize(Roles = "Admin")]
  public class CreateModel : PageModel
  {
    private readonly UserService userService;
    private readonly NoteService noteService;

    public CreateModel(UserService userService, NoteService noteService)
    {
      this.userService = userService;
      this.noteService = noteService;
    }

    [BindProperty]
    public User NewUser { get; set; } = default!;

    public IActionResult OnGet()
    {
      NewUser = new User
      {
        Role = UserRoles.User
      };
      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      NewUser.Password = CryptographyService.Encrypt(NewUser.Password);
      var newUser = await userService.SaveAsync(NewUser);
      await noteService.SaveAsync(new Note() { Code = "index", Title = $"{NewUser.Name} main page", UserId = newUser.Id });
      return RedirectToPage("./Index", new { ialert = AlertType.Success, text = "Record has been created." });
    }
  }
}
