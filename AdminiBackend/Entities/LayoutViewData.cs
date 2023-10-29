namespace AdminiBackend.Entities
{
  /// <summary>
  /// Admin panel sidebar current page.
  /// </summary>
  public enum AdminPanelPage
  {
    /// <summary>Panel/Notes/Index page.</summary>
    NotesPage,
    /// <summary>Panel/Tags/Index page.</summary>
    TagsPage,
    /// <summary>Panel/Users/Index page.</summary>
    UsersPage
  }

  /// <summary>
  /// View data information for Pages/Shared/_Layout.cshtml.
  /// </summary>
  public class LayoutViewData
  {
    public string Title { get; set; }

    public AdminPanelPage? CurrentPage { get; set; }

    public string[]? Styles { get; set; }

    public string[]? Scripts { get; set; }

    public ResponseAlert? Alert { get; set; }

    public LayoutViewData(string title)
    {
      Title = title;
    }

    public LayoutViewData(string title, AdminPanelPage page, ResponseAlert? alert = null)
    {
      Title = title;
      CurrentPage = page;
      Alert = alert;
    }

    public LayoutViewData(string title, AdminPanelPage page, string[]? styles, string[]? scripts, ResponseAlert? alert = null)
    {
      Title = title;
      CurrentPage = page;
      Styles = styles;
      Scripts = scripts;
      Alert = alert;
    }
  }
}
