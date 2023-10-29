using AdminiBackend.Entities;
using AdminiDomain.Entities;
using AdminiDomain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminiBackend.Pages.Panel.Users
{
  [Authorize(Roles = "Admin")]
  public class IndexModel : PageModel
  {
    private readonly UserService userService;

    public IndexModel(UserService userService)
    {
      this.userService = userService;
    }

    public ResponseAlert? Alert { get; set; }
    public IList<User> UserList { get; set; } = default!;

    public async Task OnGetAsync(AlertType? alert, string? text)
    {
      UserList = await userService.GetListAsync();
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
      var user = await userService.DeleteAsync((int)id);
      if (user is null)
      {
        return RedirectToPage(new { alert = AlertType.Error, text = $"Record with id={id} not found." });
      }
      FileService.DeleteFolder(user.Name);
      return RedirectToPage(new { alert = AlertType.Success, text = "Record has been deleted." });
    }
  }
}
