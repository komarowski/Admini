using AdminiBackend.Entities;
using AdminiBackend.Services;
using AdminiDomain.Entities;
using AdminiDomain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminiBackend.Pages.Panel.Notes
{
  [Authorize(Roles = "User,Admin")]
  public class EditModel : PageModel
  {
    private readonly NoteService noteService;
    private readonly TagService tagService;

    public EditModel(NoteService noteService, TagService tagService)
    {
      this.noteService = noteService;
      this.tagService = tagService;
    }

    [BindProperty]
    public Note EditNote { get; set; } = default!;

    public IList<Tag> TagList { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
      if (id is null)
      {
        return RedirectToPage("./Index", new { alert = AlertType.Error, text = "Record Id is null." });
      }
      var userId = AuthService.GetUserID(User.Claims);
      var note = await noteService.GetAsync(note => note.Id == (int)id && note.UserId == userId);
      if (note is null)
      {
        return RedirectToPage("./Index", new { alert = AlertType.Error, text = $"Record with id={id} not found." });
      }
      TagList = note.Code == "index" 
        ? await tagService.GetListAsync("admini") 
        : await tagService.GetListAsync(tag => tag.UserId == userId);
      EditNote = note;
      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      EditNote.Code = EditNote.Code.ToLower();
      await noteService.SaveAsync(EditNote);
      return RedirectToPage("./Index", new { alert = AlertType.Success, text = "Record has been edited." });
    }
  }
}
