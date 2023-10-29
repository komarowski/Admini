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
  public class EditModel : PageModel
  {
    private readonly TagService tagService;

    public EditModel(TagService tagService)
    {
      this.tagService = tagService;
    }

    [BindProperty]
    public Tag EditTag { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
      if (id is null)
      {
        return RedirectToPage("./Index", new { alert = AlertType.Error, text = "Record Id is null." });
      }
      var tag = await tagService.GetAsync(tag => tag.Id == id && tag.UserId == AuthService.GetUserID(User.Claims));
      if (tag is null)
      {
        return RedirectToPage("./Index", new { alert = AlertType.Error, text = $"Record with id={id} not found." });
      }
      EditTag = tag;
      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      await tagService.SaveAsync(EditTag);
      return RedirectToPage("./Index", new { alert = AlertType.Success, text = "Record has been edited." });
    }
  }
}
