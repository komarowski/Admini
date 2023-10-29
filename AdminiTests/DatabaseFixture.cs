using AdminiDomain.Entities;
using AdminiDomain.Services;
using AdminiInfrastructure;
using Microsoft.EntityFrameworkCore;

namespace AdminiTests
{
  /// <summary>
  /// A shared object instance for accessing the test database in integration tests.
  /// </summary>
  public class DatabaseFixture
  {
    private readonly RepositorySqlServer repositorySqlServer;
    public readonly NoteService noteService;
    public readonly UserService userService;
    public readonly TagService tagService;

    public DatabaseFixture()
    {
      var context = GetInitializedDbContext(true);
      repositorySqlServer = new RepositorySqlServer(context);
      noteService = new NoteService(repositorySqlServer);
      userService = new UserService(repositorySqlServer);
      tagService = new TagService(repositorySqlServer);
    }

    /// <summary>
    /// Initializing DbContext for test database.
    /// </summary>
    /// <param name="createNewDb">Create new database.</param>
    /// <returns>Instance of <see cref="AdminiContext"/>.</returns>
    private static AdminiContext GetInitializedDbContext(bool createNewDb)
    {
      var options = new DbContextOptionsBuilder<AdminiContext>()
               .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AdminiTestDb;Trusted_Connection=True;MultipleActiveResultSets=true")
               .Options;

      var context = new AdminiContext(options);
      if (createNewDb)
      {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
      }

      foreach (var item in InitialData)
      {
        var user = context.Users.Add(item.User);
        context.SaveChanges();
        foreach (var note in item.NoteList)
        {
          note.UserId = user.Entity.Id;
          context.Notes.Add(note);
          context.SaveChanges();
        }
        foreach (var tag in item.TagList)
        {
          tag.UserId = user.Entity.Id;
          context.Tags.Add(tag);
          context.SaveChanges();
        }
      }
      context.SaveChanges();
      return context;
    }

    /// <summary>
    /// Initial data for test database.
    /// </summary>
    public static readonly List<UserData> InitialData = new()
    {
      new UserData()
      {
        User = new()
        {
          Name = Constants.AdminUserName,
          Role = UserRoles.Admin
        },
        NoteList = new List<Note>()
        {
          new Note()
          {
            Code = Constants.IndexNoteCode,
            Title = "Index page"
          }
        },
        TagList = new List<Tag>()
        {
          new Tag()
          {
            Title = "Personal",
            Number = 1,
          },
          new Tag()
          {
            Title = "Company",
            Number = 2,
          }
        }
      },
      new UserData()
      {
        User = new()
        {
          Name = "digitalnomad",
          Role = UserRoles.User
        },
        NoteList = new List<Note>()
        {
          new Note()
          {
            Code = Constants.IndexNoteCode,
            Title = "My travel blog",
            Tags = 1
          },
          new Note()
          {
            Code = "061ad2e6735042189712bd6983a98499",
            Title = "Top 10 places in Stockholm",
            Tags = 1
          },
          new Note()
          {
            Code = "7e10f0b7f55b446d83e6f16cd7f9cc61",
            Title = "Travel to Machu Picchu",
            Tags = 2,
            IsMark = true,
            Longitude = -13.16301,
            Latitude = -72.54499,
          }
        },
        TagList = new List<Tag>()
        {
          new Tag()
          {
            Title = "topplaces",
            Number = 1,
          },
          new Tag()
          {
            Title = "mustvisit",
            Number = 2,
          }
        }
      },
      new UserData()
      {
        User = new()
        {
          Name = "besthouses",
          Role = UserRoles.User
        },
        NoteList = new List<Note>()
        {
          new Note()
          {
            Code = Constants.IndexNoteCode,
            Title = "Advertisements for the sale of houses",
            Tags = 2
          },
          new Note()
          {
            Code = "e01dc75e5ce845ea869253830f914432",
            Title = "Apartment 1",
            Tags = 1,
            IsMark = true,
            Longitude = 44.32104,
            Latitude = 23.77589,
          },
          new Note()
          {
            Code = "4e6c46c18abb441a9141e388fa71d86a",
            Title = "Country house 1",
            Tags = 2,
            IsMark = true,
            Longitude = 44.31653,
            Latitude = 23.78652,
          },
          new Note()
          {
            Code = "a1e4d0663dc94cbc89fd8586e05fde17",
            Title = "Country house 2",
            Tags = 4,
            IsMark = true,
            Longitude = 44.31878,
            Latitude = 23.79673
          }
        },
        TagList = new List<Tag>()
        {
          new Tag()
          {
            Title = "<30000$",
            Number = 1,
          },
          new Tag()
          {
            Title = "30000$-50000$",
            Number = 2,
          },
          new Tag()
          {
            Title = ">50000$",
            Number = 4,
          }
        }
      }
    };
  }

  /// <summary>
  /// Helper class for initializing initial data.
  /// </summary>
  public class UserData
  {
    public User User { get; set; }

    public List<Note> NoteList { get; set; }

    public List<Tag> TagList { get; set; }
  }
}
