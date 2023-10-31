using AdminiDomain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AdminiDomain.Services
{
  /// <summary>
  /// Service for <see cref="Note"/> entity management.
  /// </summary>
  public class NoteService : GenericService<Note>
  {
    public NoteService(IRepository repository)
      : base(repository)
    {
    }

    /// <summary>
    /// Gets new note code.
    /// </summary>
    /// <returns>Note code.</returns>
    public static string GetNewCode()
      => Guid.NewGuid().ToString().Replace("-", string.Empty);

    /// <summary>
    /// Gets array of <see cref="ImportNote"/> for export.
    /// </summary>
    /// <param name="userId">User id.</param>
    /// <returns>Array of <see cref="ImportNote"/>.</returns>
    public async Task<ImportNote[]> GetExportListAsync(int userId)
    {
      return await repository.GetQuery<Note>()
        .Where(note => note.UserId == userId)
        .Join(
          repository.GetQuery<NoteContent>(),
          note => note.Id,
          noteContent => noteContent.NoteId,
          (note, noteContent) => new ImportNote(note, noteContent))
        .ToArrayAsync();
    }

    /// <summary>
    /// Gets user's note by code.
    /// </summary>
    /// <param name="userName">User name.</param>
    /// <param name="code">Note code.</param>
    /// <returns><see cref="Note"/> or null if not found.</returns>
    public async Task<Note?> GetNoteAsync(string userName, string code)
    {
      return await repository.GetQuery<Note>()
        .Join(repository.GetQuery<User>(),
          note => note.UserId,
          user => user.Id,
          (note, user) => new { note, user })
        .Where(x => x.user.Name == userName && x.note.Code == code)
        .Select(x => x.note)
        .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets array of users index notes (for Admini main page).
    /// <para>The search is performed by tags and note title</para>
    /// <para>If search query is null return all notes.</para>
    /// </summary>
    /// <param name="query">Search query.</param>
    /// <param name="tags">Tag sum.</param>
    /// <returns>Array of <see cref="Note"/>.</returns>
    public async Task<IList<Note>> GetListByQueryAsync(string? query, int? tags)
    {
      var contextQuery = repository.GetQuery<Note>()
        .Join(repository.GetQuery<User>(),
          note => note.UserId,
          user => user.Id,
          (note, user) => new { note, user })
        .Where(x => x.note.Code == Constants.IndexNoteCode && x.user.Name != Constants.AdminUserName);

      if (tags is not null && tags > 0)
      {
        contextQuery = contextQuery.Where(x => (x.note.Tags & tags) == tags);
      }

      contextQuery = string.IsNullOrEmpty(query)
        ? contextQuery.OrderByDescending(x => x.user.Name)
        : contextQuery.Where(x => x.note.Title.ToLower().Contains(query));

      return await contextQuery
        .Select(x => new Note() { Title = x.user.Name, Description = x.note.Description, Code = x.user.Name })
        .ToArrayAsync();
    }

    /// <summary>
    /// Gets array of user's notes.
    /// <para>The search is performed by tags and note title</para>
    /// <para>If search query is null return all user's notes.</para>
    /// </summary>
    /// <param name="userName">User name.</param>
    /// <param name="query">Search query.</param>
    /// <param name="tags">Tag sum.</param>
    /// <returns>Array of <see cref="Note"/>.</returns>
    public async Task<IList<Note>> GetListByQueryAsync(string userName, string? query, int? tags)
    {
      var contextQuery = repository.GetQuery<Note>()
        .Join(repository.GetQuery<User>(),
          note => note.UserId,
          user => user.Id,
          (note, user) => new { note, user })
        .Where(x => x.user.Name == userName && x.note.Code != Constants.IndexNoteCode);

      if (tags is not null && tags > 0)
      {
        contextQuery = contextQuery.Where(x => (x.note.Tags & tags) == tags);
      }

      if (!string.IsNullOrEmpty(query))
      {
        contextQuery = contextQuery.Where(x => x.note.Title.ToLower().Contains(query));
      }

      return await contextQuery
        .OrderByDescending(x => x.note.LastUpdate)
        .Select(x => x.note)
        .ToArrayAsync();
    }

    /// <summary>
    /// Gets array of user's notes for the current state of the map.
    /// <para>The search is performed by tags, note title and coordinates.</para>
    /// <para>If there are no map state parameters return empty array.</para>
    /// </summary>
    /// <param name="userName">User name.</param>
    /// <param name="query">Search query.</param>
    /// <param name="tags">Tag sum.</param>
    /// <param name="zoom">Map zoom.</param>
    /// <param name="lon">Map center longitude.</param>
    /// <param name="lat">Map center latitude.</param>
    /// <returns>Array of <see cref="Note"/>.</returns>
    public async Task<IList<Note>> GetListByQueryAsync(string userName, string? query, int? tags, double? zoom, double? lon, double? lat)
    {
      if (zoom is null || lon is null || lat is null)
      {
        return Array.Empty<Note>();
      }

      var contextQuery = repository.GetQuery<Note>()
        .Join(repository.GetQuery<User>(),
          note => note.UserId,
          user => user.Id,
          (note, user) => new { note, user })
        .Where(x => x.user.Name == userName);

      if (tags is not null && tags > 0)
      {
        contextQuery = contextQuery.Where(x => (x.note.Tags & tags) == tags);
      }

      if (!string.IsNullOrEmpty(query))
      {
        contextQuery = contextQuery.Where(x => x.note.Title.ToLower().Contains(query));
      }

      // Calculates the current boundaries for the map on the client side.
      double deltaLat = 0.0009 * Math.Pow(2, 19 - zoom.Value);
      double deltaLon = deltaLat * 1.5;
      double latTop = Math.Min(lat.Value + deltaLat, 90.0);
      double latBottom = Math.Max(lat.Value - deltaLat, -90.0);
      double longLeft = Math.Max(lon.Value - deltaLon, -180.0);
      double longRight = Math.Min(lon.Value + deltaLon, 180.0);
      return await contextQuery
        .Where(x => x.note.IsMark == true
          && x.note.Latitude >= latBottom 
          && x.note.Latitude <= latTop 
          && x.note.Longitude >= longLeft 
          && x.note.Longitude <= longRight)
        .Select(x => x.note)
        .ToArrayAsync();
    }
  }
}
