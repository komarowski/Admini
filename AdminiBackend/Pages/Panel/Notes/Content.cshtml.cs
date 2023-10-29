using AdminiBackend.DTO;
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
  public class ContentModel : PageModel
  {
    private readonly NoteService noteService;
    private readonly NoteContentService noteContentService;
    private readonly NoteFileService noteFileService;

    public ContentModel(NoteService noteService, NoteContentService noteContentService, NoteFileService noteFileService)
    {
      this.noteService = noteService;
      this.noteContentService = noteContentService;
      this.noteFileService = noteFileService;
    }

    [BindProperty]
    public NoteContent NoteContent { get; set; } = default!;
    [BindProperty]
    public string Code { get; set; } = default!;

    public string Title { get; set; } = default!;
    public IList<NoteFileDTO> NoteFiles { get; set; } = default!;

    private async Task SetImageList(int noteId)
    {
      var noteFileList = await noteFileService.GetListAsync(file => file.NoteId == noteId);
      NoteFiles = noteFileList
        .Select(file => 
          new NoteFileDTO() 
          { 
            Name = file.Name,
            FullName = Path.Combine("notes", file.Folder, file.Name) 
          })
        .ToList();
    }

  public async Task<IActionResult> OnGetAsync(int? id)
    {
      if (id is null)
      {
        return RedirectToPage("./Index", new { alert = AlertType.Error, text = "Record Id is null." });
      }
      var note = await noteService.GetAsync(note => note.Id == (int)id && note.UserId == AuthService.GetUserID(User.Claims));
      if (note is null)
      {
        return RedirectToPage("./Index", new { alert = AlertType.Error, text = $"Note record with id={id} not found." });
      }
      Title = note.Title;
      Code = note.Code;
      await SetImageList(note.Id);
      var noteContent = await noteContentService.GetAsync(x => x.NoteId == note.Id);
      if (noteContent is null)
      {
        NoteContent = new NoteContent() { NoteId = note.Id, Content = Constants.DefaultContent };
      }
      else
      {
        NoteContent = noteContent;
      }
      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      var savedContent = await noteContentService.SaveAsync(NoteContent);
      await MarkdownService.ConvertMarkdownToHtmlAsync(savedContent.Content, User.Identity!.Name!, Code);
      return RedirectToPage("./Index", new { alert = AlertType.Success, text = "The note content saved." });
    }
  }
}
