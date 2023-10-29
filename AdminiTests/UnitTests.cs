using AdminiDomain.Entities;
using AdminiDomain.Services;
using System.Text.Json;

namespace AdminiTests
{
  /// <summary>
  /// Unit tests.
  /// </summary>
  public class UnitTests
  {
    #region MarkdownService Tests
    [Fact]
    public void GetFrontMatterAllFieldsThere()
    {
      // Arrange
      var testMarkdown = File.ReadAllText("UnitTestsFiles/markdown1.md");
      var expected = new NoteFrontMatter()
      {
        Code = "testcode",
        Title = "Medeu - high-altitude sports complex",
        Description = "How I visited the highest active open-air stadium.",
        Tags = 11,
        IsMark = true,
        Latitude = 43.15717,
        Longitude = 77.05939,
        LastUpdate = new DateTime(2023, 8, 19, 3, 5, 1)
      };
      var expectedJsonString = JsonSerializer.Serialize(expected);

      // Act
      var actual = MarkdownService.GetFrontMatter<NoteFrontMatter>(testMarkdown);
      var actualJsonString = JsonSerializer.Serialize(actual);

      // Assert
      Assert.NotNull(actual);
      Assert.Equal(expectedJsonString, actualJsonString);
    }

    [Fact]
    public void GetFrontMatterSomeFieldsMissing()
    {
      // Arrange
      var testMarkdown = File.ReadAllText("UnitTestsFiles/markdown2.md");
      var expected = new NoteFrontMatter()
      {
        Code = string.Empty,
        Title = "Medeu - high-altitude sports complex",
        Description = "How I visited the highest active open-air stadium.",
        Tags = 11,
        IsMark = true,
        Latitude = 0,
        Longitude = 77.05939,
        LastUpdate = new DateTime(2023, 8, 19, 3, 5, 1)
      };
      var expectedJsonString = JsonSerializer.Serialize(expected);

      // Act
      var actual = MarkdownService.GetFrontMatter<NoteFrontMatter>(testMarkdown);
      var actualJsonString = JsonSerializer.Serialize(actual);

      // Assert
      Assert.NotNull(actual);
      Assert.Equal(expectedJsonString, actualJsonString);
    }

    [Fact]
    public void GetFrontMatterNoFrontMatter()
    {
      // Arrange
      var testMarkdown = File.ReadAllText("UnitTestsFiles/markdown3.md");

      // Act
      var actual = MarkdownService.GetFrontMatter<NoteFrontMatter>(testMarkdown);

      // Assert
      Assert.Null(actual);
    }

    [Fact]
    public void GetFrontMatterNoFields()
    {
      // Arrange
      var testMarkdown = File.ReadAllText("UnitTestsFiles/markdown4.md");

      // Act
      var actual = MarkdownService.GetFrontMatter<NoteFrontMatter>(testMarkdown);

      // Assert
      Assert.Null(actual);
    }

    [Fact]
    public async Task ConvertMarkdownToHtml()
    {
      // Arrange
      var testMarkdown = File.ReadAllText("UnitTestsFiles/markdown1.md");
      var expected = File.ReadAllText("UnitTestsFiles/index.txt");

      // Act
      await MarkdownService.ConvertMarkdownToHtmlAsync(testMarkdown, "", "");
      var actual = File.ReadAllText("wwwroot/notes/index.txt");

      // Assert
      Assert.Equal(expected, actual);
    }
    #endregion
  }
}