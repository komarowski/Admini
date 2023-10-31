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
  public class FileModel : PageModel
  {
    private readonly NoteService noteService;
    private readonly NoteFileService noteFileService;

    public FileModel(NoteService noteService, NoteFileService noteFileService)
    {
      this.noteService = noteService;
      this.noteFileService = noteFileService;
    }

    [BindProperty]
    public IList<IFormFile>? UploadFiles { get; set; }
    [BindProperty]
    public int NoteId { get; set; } = default!;
    [BindProperty]
    public string Code { get; set; } = default!;

    public ResponseAlert? Alert { get; set; }
    public IList<NoteFile> FileList { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id, AlertType? alert, string? text)
    {
      if (id is null)
      {
        return RedirectToPage("./Index", new { alert = AlertType.Error, text = "Note Id is null." });
      }
      var note = await noteService.GetAsync(note => note.Id == (int)id && note.UserId == AuthService.GetUserID(User.Claims));
      if (note is null)
      {
        return RedirectToPage("./Index", new { alert = AlertType.Error, text = $"Note record with id={id} not found." });
      }
      if (alert is not null && text is not null)
      {
        this.Alert = ResponseAlert.GetAlert((AlertType)alert, text);
      }
      NoteId = note.Id;
      Code = note.Code;
      FileList = await noteFileService.GetListAsync(file => file.NoteId == note.Id);
      return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int? id)
    {
      if (id is null)
      {
        return RedirectToPage(new { id = NoteId, alert = AlertType.Error, text = "Note file Id is null." });
      }
      var deleteFileNote = await noteFileService.DeleteAsync((int)id);
      if (deleteFileNote is null)
      {
        return RedirectToPage(new { id = NoteId, alert = AlertType.Error, text = $"Note file with id={id} not found." });
      }
      FileService.DeleteNoteFile(deleteFileNote.Folder, deleteFileNote.Name);
      return RedirectToPage(new { id = NoteId, alert = AlertType.Success, text = "Note file has been deleted." });
    }

    public async Task<IActionResult> OnPostUploadAsync()
    {
      if (UploadFiles is not null)
      {
        foreach (var file in UploadFiles)
        {
          var targetFolder = Path.Combine(User.Identity!.Name!, Code);
          await noteFileService.SaveAsync(new NoteFile() { Name = file.FileName, NoteId = NoteId, Folder = targetFolder });
          var targetPath = FileService.GetTargetPath(targetFolder, file.FileName);
          await FileService.SaveFileAsync(file, targetPath);
        }
        return RedirectToPage(new { id = NoteId, alert = AlertType.Success, text = "Files has been uploaded." });
      }
      return RedirectToPage(new { id = NoteId, alert = AlertType.Warning, text = "Files to import not selected." });
    }
  }
}
