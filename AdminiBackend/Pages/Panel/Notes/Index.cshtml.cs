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
  public class IndexModel : PageModel
  {
    private readonly ILogger<ErrorModel> logger;
    private readonly NoteService noteService;
    private readonly NoteContentService noteContentService;
    private readonly NoteFileService noteFileService;

    public IndexModel(NoteService noteService, 
      NoteFileService noteFileService,
      NoteContentService noteContentService,
      ILogger<ErrorModel> logger)
    {
      this.noteService = noteService;
      this.noteContentService = noteContentService;
      this.noteFileService = noteFileService;
      this.logger = logger;
    }

    [BindProperty]
    public IFormFile UploadZip { get; set; } = default!;

    public ResponseAlert? Alert { get; set; }
    public IList<Note> NoteList { get;set; } = default!;

    public async Task OnGetAsync(AlertType? alert, string? text)
    {
      NoteList = await noteService.GetListAsync(note => note.UserId == AuthService.GetUserID(User.Claims));
      if (alert is not null && text is not null)
      {
        this.Alert = ResponseAlert.GetAlert((AlertType)alert, text);
      }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int? id)
    {
      if (id is null)
      {
        return RedirectToPage(new { alert = AlertType.Error, text = "Record Id is null." });
      }
      var deleteNote = await noteService.DeleteAsync((int)id);
      if (deleteNote is null)
      {
        return RedirectToPage(new { alert = AlertType.Error, text = $"Record with id={id} not found." });
      }
      FileService.DeleteFolder(User.Identity!.Name!, deleteNote.Code);
      return RedirectToPage(new { alert = AlertType.Success, text = "Record has been deleted." });
    }

    public async Task<IActionResult> OnPostUploadAsync()
    {
      if (UploadZip is not null)
      {
        var userName = User.Identity!.Name!;
        try
        {
          var importList = await MarkdownService.ImportAsync(UploadZip, userName, AuthService.GetUserID(User.Claims));
          foreach (var item in importList)
          {
            var importNote = await noteService.GetAsync(note => note.Code == item.Note.Code && note.UserId == item.Note.UserId);
            if (importNote is null)
            {
              importNote = item.Note;
            }
            else
            {
              importNote.Title = item.Note.Title;
              importNote.Description = item.Note.Description;
              importNote.IsMark = item.Note.IsMark;
              importNote.Longitude = item.Note.Longitude;
              importNote.Latitude = item.Note.Latitude;
              importNote.LastUpdate = item.Note.LastUpdate;
              importNote.Tags = item.Note.Tags;
            }
            var note = await noteService.SaveAsync(importNote);

            var importNoteContent = await noteContentService.GetAsync(content => content.NoteId == note.Id);
            if (importNoteContent is null)
            {
              importNoteContent = item.Content;
              importNoteContent.NoteId = note.Id;
            }
            else
            {
              importNoteContent.Content = item.Content.Content;
            }
            await noteContentService.SaveAsync(item.Content);

            foreach (var file in item.Files)
            {
              var existNoteFile = await noteFileService.GetAsync(noteFile => noteFile.Name == file.Name && noteFile.NoteId == note.Id);
              if (existNoteFile is null)
              {
                file.NoteId = note.Id;
                await noteFileService.SaveAsync(file);
              }
            }
          }
        }
        catch (Exception ex)
        {
          logger.LogError(ex, "Error while import '{userName}'", userName);
          return RedirectToPage(new { alert = AlertType.Error, text = "An error occurred while importing." });
        }
        
        return RedirectToPage(new { alert = AlertType.Success, text = "Notes has been uploaded." });
      }
      return RedirectToPage(new { alert = AlertType.Warning, text = "Files to import not selected." });
    }

    public async Task<IActionResult> OnPostDownloadAsync()
    {
      FileService.DeleteZipFiles();
      var exportList = await noteService.GetExportListAsync(AuthService.GetUserID(User.Claims));
      if (exportList.Length > 0)
      {
        var zipPath = await MarkdownService.ExportAsync(User.Identity!.Name!, exportList);
        return PhysicalFile(zipPath, "application/zip");
      }
      return RedirectToPage(new { alert = AlertType.Warning, text = "Notes not found." });
    }
  }
}
