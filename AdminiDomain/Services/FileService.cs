using Microsoft.AspNetCore.Http;

namespace AdminiDomain.Services
{
  /// <summary>
  /// Service for file management.
  /// </summary>
  public static class FileService
  {
    /// <summary>
    /// Current working directory of application.
    /// </summary>
    public readonly static string RootFolder = Directory.GetCurrentDirectory();

    /// <summary>
    /// Folder for storing HTML files of notes and supporting files.
    /// </summary>
    private const string TargetNoteFolder = "wwwroot\\notes";

    /// <summary>
    /// Combine an array of strings into path that starts with <see cref="TargetNoteFolder"/>.
    /// </summary>
    /// <param name="paths">Array of path strings.</param>
    /// <returns>Target path.</returns>
    public static string GetTargetPath(params string[] paths) 
      => Path.Combine(TargetNoteFolder, Path.Combine(paths));

    /// <summary>
    /// Gets note linked files from <see cref="TargetNoteFolder"/>.
    /// </summary>
    /// <param name="paths">Array of path strings.</param>
    public static IEnumerable<FileInfo>? GetNoteLinkedFiles(params string[] paths)
    {
      var noteFolderPath = GetTargetPath(paths);
      var noteFolder = new DirectoryInfo(noteFolderPath);
      if (noteFolder.Exists)
      {
        return noteFolder.GetFiles()
          .Where(file => !file.Name.EndsWith(".txt"));
      }
      return null;
    }

    /// <summary>
    /// Deletes note folder.
    /// </summary>
    /// <param name="paths">Array of path strings.</param>
    public static void DeleteFolder(params string[] paths)
    {
      var folderPath = GetTargetPath(paths);
      if (Directory.Exists(folderPath))
      {
        Directory.Delete(folderPath, true);
      }
    }

    /// <summary>
    /// Deletes note linked file.
    /// </summary>
    /// <param name="paths">Array of path strings.</param>
    public static void DeleteNoteFile(params string[] paths)
    {
      var filePath = GetTargetPath(paths);
      if (File.Exists(filePath))
      {
        File.Delete(filePath);
      }
    }

    /// <summary>
    /// Deletes note backup zip files.
    /// </summary>
    public static void DeleteZipFiles()
    {
      var zipFiles = Directory.GetFiles(RootFolder, "notes_*.zip");
      foreach (var zipFile in zipFiles)
      {
        File.Delete(zipFile);
      }
    }

    /// <summary>
    /// Saves file sent with the HttpRequest.
    /// </summary>
    /// <param name="file">File sent with the HttpRequest.</param>
    /// <param name="path">File path.</param>>
    public static async Task SaveFileAsync(IFormFile file, string path)
    {
      using Stream fileStream = new FileStream(path, FileMode.Create);
      await file.CopyToAsync(fileStream);
    }
  }
}
