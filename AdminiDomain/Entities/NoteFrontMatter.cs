using YamlDotNet.Serialization;

namespace AdminiDomain.Entities
{
  /// <summary>
  /// Metadata in Markdown files to store <see cref="Note" /> fields.
  /// </summary>
  public class NoteFrontMatter
  {
    [YamlMember(Alias = "code")]
    public string Code { get; set; } = string.Empty;

    [YamlMember(Alias = "title")]
    public string Title { get; set; } = string.Empty;

    [YamlMember(Alias = "description")]
    public string? Description { get; set; }

    [YamlMember(Alias = "tags")]
    public int Tags { get; set; }

    [YamlMember(Alias = "ismark")]
    public bool IsMark { get; set; }

    [YamlMember(Alias = "latitude")]
    public double Latitude { get; set; }

    [YamlMember(Alias = "longitude")]
    public double Longitude { get; set; }

    [YamlMember(Alias = "lastupdate")]
    public DateTime? LastUpdate { get; set; }
  }
}
