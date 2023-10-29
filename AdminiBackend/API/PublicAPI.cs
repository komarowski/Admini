using AdminiBackend.DTO;
using AdminiDomain.Entities;
using AdminiDomain.Services;

namespace AdminiBackend.API
{
  /// <summary>
  /// Public api endpoints.
  /// </summary>
  public class PublicAPI
  {
    /// <summary>
    /// Gets client user name.
    /// </summary>
    /// <param name="name">User name or null.</param>
    /// <returns>Client user name.</returns>
    private static string GetUserName(string? name)
      => string.IsNullOrEmpty(name) ? Constants.AdminUserName : name;

    /// <summary>
    /// Register public api endpoints.
    /// </summary>
    /// <param name="app">WebApplication instance.</param>
    public static void Register(WebApplication app)
    {
      app.MapGet("/api/users", async (UserService userService, string? user) =>
      {
        var userDb = await userService.GetAsync(u => u.Name == GetUserName(user));
        if (userDb is null)
        {
          return null;
        }
        return new UserDTO(userDb);
      });

      app.MapGet("/api/tags", async (TagService tagService, string? user) =>
      {
        var tagArray = await tagService.GetListAsync(GetUserName(user));
        return tagArray.Select(tag => new TagDTO() { Number = tag.Number, Title = tag.Title });
      });

      app.MapGet("/api/note", async (NoteService noteService, TagService tagService, string? user, string? code) =>
      {
        if (string.IsNullOrEmpty(code))
        {
          return null;
        }
        var userName = GetUserName(user);
        var note = await noteService.GetNoteAsync(userName, code);
        if (note is null)
        {
          return null;
        }
        if (note.Code == Constants.IndexNoteCode)
        {
          userName = Constants.AdminUserName;
        }
        var tagString = await tagService.GetTagsString(note.Tags, userName);
        return new NoteDTO(note, tagString);
      });

      app.MapGet("/api/notes", async (NoteService noteService, string? user, string? query, int? tags) =>
      {
        var noteArray = (string.IsNullOrEmpty(user) || user == Constants.AdminUserName)
          ? await noteService.GetListByQueryAsync(query, tags) 
          : await noteService.GetListByQueryAsync(user, query, tags);
        return noteArray.Select(note => new NoteDTO(note)).ToArray();
      });

      app.MapGet("/api/mapnotes", async (NoteService noteService, string? user, string? query, int? tags, double? zoom, double? lon, double? lat) =>
      {
        if (string.IsNullOrEmpty(user))
        {
          return Array.Empty<NoteDTO>();
        }
        var noteArray = await noteService.GetListByQueryAsync(user, query, tags, zoom, lon, lat);
        return noteArray.Select(note => new NoteDTO(note)).ToArray();
      });
    }
  }
}
