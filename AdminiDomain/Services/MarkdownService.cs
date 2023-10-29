using AdminiDomain.Entities;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Microsoft.AspNetCore.Http;
using System.IO.Compression;
using System.Text;
using YamlDotNet.Serialization;

namespace AdminiDomain.Services
{
  /// <summary>
  /// Service for markdown files management.
  /// </summary>
  public static class MarkdownService
  {
    /// <summary>
    /// Deserializing YAML metadata from a markdown file.
    /// </summary>
    private static readonly IDeserializer YamlDeserializer = new DeserializerBuilder()
      .IgnoreUnmatchedProperties()
      .Build();

    /// <summary>
    /// Serializing YAML metadata for markdown file.
    /// </summary>
    private static readonly ISerializer YamlSerializer = new SerializerBuilder()
      .Build();

    /// <summary>
    /// Converting markdown to HTML.
    /// </summary>
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
      .UseAdvancedExtensions()
      .UseYamlFrontMatter()
      .Build();

    /// <summary>
    /// Get markdown file metadata.
    /// </summary>
    /// <typeparam name="T">Metadata format.</typeparam>
    /// <param name="markdown">Markdown text.</param>
    /// <returns>Metadata or null if it could not be retrieved.</returns>
    public static T? GetFrontMatter<T>(this string markdown)
    {
      var document = Markdown.Parse(markdown, Pipeline);
      var yamlBlock = document
        .Descendants<YamlFrontMatterBlock>()
        .FirstOrDefault();

      if (yamlBlock is null || yamlBlock.Lines.Count == 0)
        return default;

      var yaml = yamlBlock
        .Lines
        .Lines
        .OrderByDescending(x => x.Line)
        .Select(x => $"{x}\n")
        .ToList()
        .Select(x => x.Replace("---", string.Empty))
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Aggregate((s, agg) => agg + s);

      return YamlDeserializer.Deserialize<T>(yaml);
    }

    /// <summary>
    /// Adds file to a zip archive.
    /// </summary>
    /// <param name="zipArchive">Zip archive.</param>
    /// <param name="bytes">File bytes.</param>
    /// <param name="name">File name.</param>
    private static async Task CreateArchiveEntry(ZipArchive zipArchive, byte[] bytes, string name)
    {
      var zipEntry = zipArchive.CreateEntry(name, CompressionLevel.Optimal);
      using var zipStream = zipEntry.Open();
      await zipStream.WriteAsync(bytes);
    }

    /// <summary>
    /// Gets <see cref="NoteFrontMatter"/> from <see cref="Note"/>.
    /// </summary>
    /// <param name="note"><see cref="Note"/> instance.</param>
    /// <returns><see cref="NoteFrontMatter"/> instance.</returns>
    private static NoteFrontMatter GetNoteFrontMatter(Note note) => new()
    {
      Title = note.Title,
      Code = note.Code,
      Description = note.Description,
      Tags = note.Tags,
      IsMark = note.IsMark,
      Latitude = note.Latitude,
      Longitude = note.Longitude,
      LastUpdate = note.LastUpdate,
    };

    /// <summary>
    /// Creates a zip archive of user notes for export.
    /// </summary>
    /// <param name="userName">User name.</param>
    /// <param name="exportList">Array of <see cref="ImportNote"/> for export.</param>
    /// <returns>Zip archive path.</returns>
    public static async Task<string> ExportAsync(string userName, ImportNote[] exportList)
    {
      var zipName = $"notes_{DateTime.Now:yyyy'_'MM'_'dd'_'HH'_'mm'_'ss}.zip";
      using var fileStream = new FileStream(zipName, FileMode.CreateNew);
      using var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true, Encoding.UTF8);
      foreach (var item in exportList)
      {
        var noteString = YamlSerializer.Serialize(GetNoteFrontMatter(item.Note));
        if (noteString is null)
          continue;

        var archiveFolder = item.Note.Code + "/";
        noteString = $"---\n{noteString}---\n\n{item.Content.Content}";
        var mdBytes = Encoding.UTF8.GetBytes(noteString);
        var fileName = Path.Combine(archiveFolder, "index.md");
        await CreateArchiveEntry(archive, mdBytes, fileName);

        var linkedFiles = FileService.GetNoteLinkedFiles(userName, item.Note.Code);
        if (linkedFiles is null)
          continue;

        foreach (var linkedFile in linkedFiles)
        {
          var bytes = await File.ReadAllBytesAsync(linkedFile.FullName);
          fileName = Path.Combine(archiveFolder, linkedFile.Name);
          await CreateArchiveEntry(archive, bytes, fileName);
        }
      }
      return Path.Combine(FileService.RootFolder, zipName);
    }

    /// <summary>
    /// Gets <see cref="ImportNote"/> from zip archive folder.
    /// </summary>
    /// <param name="noteEntries">Zip archive folder entries.</param>
    /// <param name="userName">User name.</param>
    /// <param name="userId">User id.</param>
    /// <returns><see cref="ImportNote"/> or null if it could not be retrieved.</returns>
    private static async Task<ImportNote?> ImportNoteEntries(IEnumerable<ZipArchiveEntry> noteEntries, string userName, int userId)
    {
      var markdownEntry = noteEntries.FirstOrDefault(entry => entry.Name.EndsWith(".md"));
      if (markdownEntry is null)
      {
        return null;
      }

      using var reader = new StreamReader(markdownEntry.Open(), Encoding.UTF8);
      var markdown = await reader.ReadToEndAsync();
      var markdownFrontMatter = GetFrontMatter<NoteFrontMatter>(markdown);
      if (markdownFrontMatter is null)
      {
        return null;
      }

      var noteCode = string.IsNullOrEmpty(markdownFrontMatter.Code) ? NoteService.GetNewCode() : markdownFrontMatter.Code;
      var note = new Note()
      {
        Title = markdownFrontMatter.Title,
        Code = noteCode,
        Description = markdownFrontMatter.Description,
        Tags = markdownFrontMatter.Tags,
        UserId = userId,
        IsMark = markdownFrontMatter.IsMark,
        Longitude = markdownFrontMatter.Longitude,
        Latitude = markdownFrontMatter.Latitude,
        LastUpdate = markdownFrontMatter.LastUpdate,
      };
      var noteContent = new NoteContent()
      {
        Content = markdown[(markdown.LastIndexOf("---") + 3)..].Trim()
      };

      await ConvertMarkdownToHtmlAsync(markdown, userName, noteCode);

      var files = new List<NoteFile>();
      var linkedEntries = noteEntries.Where(entry => !entry.Name.EndsWith(".md"));
      foreach (var linkedEntry in linkedEntries)
      {
        var targetPath = FileService.GetTargetPath(userName, noteCode, linkedEntry.Name);
        linkedEntry.ExtractToFile(targetPath, true);
        files.Add(new NoteFile() { Name = linkedEntry.Name, Folder = Path.Combine(userName, noteCode) });
      }

      return new ImportNote(note, noteContent, files);
    }

    /// <summary>
    /// Gets list of <see cref="ImportNote"/> for importing user notes.
    /// </summary>
    /// <param name="zipFile">Zip archive.</param>
    /// <param name="userName">User name.</param>
    /// <param name="userId">User id.</param>
    /// <returns>List of <see cref="ImportNote"/>.</returns>
    public static async Task<List<ImportNote>> ImportAsync(IFormFile zipFile, string userName, int userId)
    {
      var importList = new List<ImportNote>();
      using var stream = zipFile.OpenReadStream();
      using var archive = new ZipArchive(stream);
      {
        var folders = archive.Entries
          .Select(entry => Path.GetDirectoryName(entry.FullName))
          .Where(name => name is not null)
          .Distinct();

        if (folders is null)
        {
          return importList;
        }

        foreach (var folder in folders)
        {
          var noteEntries = archive.Entries
          .Where(entry => entry.FullName.StartsWith(folder!) && !string.IsNullOrEmpty(entry.Name));

          var note = await ImportNoteEntries(noteEntries, userName, userId);
          if (note is not null)
          {
            importList.Add(note);
          }
        }
      }
      return importList;
    }

    /// <summary>
    /// Converting markdown to html and saving html file.
    /// </summary>
    /// <param name="markdown">Markdown text.</param>
    /// <param name="userName">User name.</param>
    /// <param name="noteCode">Note code.</param>
    public async static Task ConvertMarkdownToHtmlAsync(string markdown, string userName, string noteCode)
    {
      var html = Markdown.ToHtml(markdown, Pipeline);
      var targetFolderPath = FileService.GetTargetPath(userName, noteCode);
      // If you save it as an HTML file, the RedirectToPage in Razor Pages does not work. 
      var targetFilePath = FileService.GetTargetPath(userName, noteCode, "index.txt");
      if (!Directory.Exists(targetFolderPath))
      {
        Directory.CreateDirectory(targetFolderPath);
      }
      await File.WriteAllTextAsync(targetFilePath, html, Encoding.UTF8);
    }
  }
}
