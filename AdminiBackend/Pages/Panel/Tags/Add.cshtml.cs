using AdminiBackend.Entities;
using AdminiBackend.Services;
using AdminiDomain.Entities;
using AdminiDomain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminiBackend.Pages.Panel.Tags
{
  [Authorize(Roles = "User,Admin")]
  public class AddModel : PageModel
  {
    private readonly TagService tagService;

    public AddModel(TagService tagService)
    {
      this.tagService = tagService;
    }

    [BindProperty]
    public Tag NewTag { get; set; } = default!;

    public IActionResult OnGet()
    {
      NewTag = new Tag
      {
        UserId = AuthService.GetUserID(User.Claims)
      };
      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      await tagService.SaveAsync(NewTag);
      return RedirectToPage("./Index", new { alert = AlertType.Success, text = "Record has been created." });
    }
  }
}
