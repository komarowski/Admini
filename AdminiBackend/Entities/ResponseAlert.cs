namespace AdminiBackend.Entities
{
  /// <summary>
  /// Operation completion type.
  /// </summary>
  public enum AlertType
  {
    Info,
    Success,
    Warning,
    Error
  }

  /// <summary>
  /// Operation completion information.
  /// </summary>
  public class ResponseAlert
  {
    public string Title { get; set; }

    public string Text { get; set; }

    public string Color { get; set; }

    public ResponseAlert(string title, string text, string color)
    {
      Title = title;
      Text = text;
      Color = color;
    }

    public static ResponseAlert GetAlert(AlertType alertType, string text) => alertType switch
    {
      AlertType.Success => new ResponseAlert("Success", text, "w3-green"),
      AlertType.Warning => new ResponseAlert("Warning", text, "w3-yellow"),
      AlertType.Error => new ResponseAlert("Error", text, "w3-red"),
      AlertType.Info or _ => new ResponseAlert("Notification", text, "w3-blue"),
    };
  }
}
