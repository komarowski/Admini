using System.Text.Json;

namespace AdminiTests
{
  /// <summary>
  /// Integration tests.
  /// </summary>
  [TestCaseOrderer(
    ordererTypeName: "AdminiTests.PriorityOrderer",
    ordererAssemblyName: "AdminiTests")]
  public class IntegrationTests : IClassFixture<DatabaseFixture>
  {
    private readonly DatabaseFixture dbFixture;

    public IntegrationTests(DatabaseFixture fixture) 
    {
      this.dbFixture = fixture;
    }

    [Fact, TestPriority(1)]
    public async Task GetExistUser()
    {
      // Arrange
      var expected = DatabaseFixture.InitialData[0].User;
      var expectedJsonString = JsonSerializer.Serialize(expected);

      // Act
      var actual = await dbFixture.userService.GetAsync(x => x.Name == expected.Name);
      var actualJsonString = JsonSerializer.Serialize(actual);

      // Assert
      Assert.NotNull(actual);
      Assert.Equal(expectedJsonString, actualJsonString);
    }

    [Fact, TestPriority(2)]
    public async Task GetExistNote()
    {
      // Arrange
      var user = DatabaseFixture.InitialData[1].User;
      var expectedNote = DatabaseFixture.InitialData[1].NoteList[1];
      var expectedJsonString = JsonSerializer.Serialize(expectedNote);

      // Act
      var actualNote = await dbFixture.noteService.GetNoteAsync(user.Name, expectedNote.Code);
      var actualJsonString = JsonSerializer.Serialize(actualNote);

      // Assert
      Assert.NotNull(actualNote);
      Assert.Equal(expectedJsonString, actualJsonString);
    }

    [Fact, TestPriority(3)]
    public async Task GetAllUserIndexNotes()
    {
      // Arrange
      var expectedCount = DatabaseFixture.InitialData.Count - 1;

      // Act
      var notes = await dbFixture.noteService.GetListByQueryAsync(null, null);
      var actualCount = notes.Count;

      // Assert
      Assert.Equal(actualCount, expectedCount);
    }

    [Fact, TestPriority(4)]
    public async Task GetNotesFilteredBySearchQuery()
    {
      // Arrange
      var user = DatabaseFixture.InitialData[2].User;
      var expectedCount = 2;

      // Act
      var notes = await dbFixture.noteService.GetListByQueryAsync(user.Name, "country house", null);
      var actualCount = notes.Count;

      // Assert
      Assert.Equal(actualCount, expectedCount);
    }
  }
}
