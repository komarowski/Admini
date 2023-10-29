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
  public class IndexModel : PageModel
  {
    private readonly TagService tagService;

    public IndexModel(TagService tagService)
    {
      this.tagService = tagService;
    }

    public ResponseAlert? Alert { get; set; }

    public IList<Tag> TagList { get; set; } = default!;

    public async Task OnGetAsync(AlertType? alert, string? text)
    {
      TagList = await tagService.GetListAsync(tag => tag.UserId == AuthService.GetUserID(User.Claims));
      if (alert is not null && text is not null)
      {
        this.Alert = ResponseAlert.GetAlert((AlertType)alert, text);
      }
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
      if (id is null)
      {
        return RedirectToPage(new { alert = AlertType.Error, text = "Record Id is null." });
      }
      var tag = await tagService.DeleteAsync((int)id);
      if (tag is null)
      {
        return RedirectToPage(new { alert = AlertType.Error, text = $"Record with id={id} not found." });
      }
      return RedirectToPage(new { alert = AlertType.Success, text = "Record has been deleted." });
    }
  }
}
