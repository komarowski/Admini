using AdminiBackend.DTO;
using AdminiDomain.Entities;
using AdminiDomain.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace AdminiBackend.Services
{
  /// <summary>
  /// Service for authentication.
  /// </summary>
  public class AuthService
  {
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly UserService userService;

    private const string UserIDClaimType = "UserID";

    public AuthService(UserService userService, IHttpContextAccessor httpContextAccessor)
    {
      this.userService = userService;
      this.httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// User authentication.
    /// </summary>
    /// <param name="form">User login data.</param>
    /// <returns>Authenticated user or null.</returns>
    public async Task<User?> SignIn(LoginDTO form)
    {
      var user = await userService.GetAsync(user => user.Name == form.Name);
      if (user is null || CryptographyService.Decrypt(user.Password) != form.Password)
      {
        return null;
      }
      var role = user.Role.ToString();
      await Authenticate(user.Name, role, user.Id.ToString());
      return user;
    }

    /// <summary>
    /// Sets user's claims to cookie.
    /// </summary>
    /// <param name="userName">User name.</param>
    /// <param name="userRole">User role.</param>
    /// <param name="userId">User id.</param>
    private async Task Authenticate(string userName, string userRole, string userId)
    {
      var claims = new List<Claim>
      {
          new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
          new Claim(ClaimsIdentity.DefaultRoleClaimType, userRole),
          new Claim(UserIDClaimType, userId)
      };
      var cookie = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
      await httpContextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(cookie));
    }

    /// <summary>
    /// Sign Out.
    /// </summary>
    public async Task SignOut()
    {
      await httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Gets claim string value from claims.
    /// </summary>
    /// <param name="claims">User claims.</param>
    /// <param name="claimType">Required claim type.</param>
    /// <returns>Сlaim string value.</returns>
    public static string GetClaim(IEnumerable<Claim> claims, string claimType)
    {
      var claimItem = claims.FirstOrDefault(claim => claim.Type == claimType);
      if (claimItem is null)
      {
        return string.Empty;
      }
      return claimItem.Value;
    }

    /// <summary>
    ///  Gets user role from claims.
    /// </summary>
    /// <param name="claims">User claims.</param>
    /// <returns>User role.</returns>
    public static UserRoles? GetRole(IEnumerable<Claim> claims) 
    {
      if (Enum.TryParse(GetClaim(claims, ClaimsIdentity.DefaultRoleClaimType), out UserRoles userRole))
      {
        return userRole;
      }
      return UserRoles.User; 
    }

    /// <summary>
    /// Gets user id from claims.
    /// </summary>
    /// <param name="claims">User claims.</param>
    /// <returns>User id.</returns>
    public static int GetUserID(IEnumerable<Claim> claims) 
    {
      if (int.TryParse(GetClaim(claims, UserIDClaimType), out int userId))
      {
        return userId;
      }
      return 0;
    }
  }
}
