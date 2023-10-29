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
  public class CreateModel : PageModel
  {
    private readonly NoteService noteService;
    private readonly TagService tagService;

    public CreateModel(NoteService noteService, TagService tagService)
    {
      this.noteService = noteService;
      this.tagService = tagService;
    }

    [BindProperty]
    public Note NewNote { get; set; } = default!;
    public IList<Tag> TagList { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
      var userId = AuthService.GetUserID(User.Claims);
      NewNote = new Note() 
      {  
        Code = NoteService.GetNewCode(),
        LastUpdate = DateTime.Now, 
        UserId = userId 
      };
      TagList = await tagService.GetListAsync(tag => tag.UserId == userId);
      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      NewNote.Code = NewNote.Code.ToLower();
      await noteService.SaveAsync(NewNote);
      return RedirectToPage("./Content", new { id = NewNote.Id, alert = AlertType.Success, text = "Record has been created." });
    }
  }
}
