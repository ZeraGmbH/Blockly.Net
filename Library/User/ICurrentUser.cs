using System.Security.Claims;

namespace BlocklyNet.User;

/// <summary>
/// Provide information on the current user.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Report the information in the current user.
    /// </summary>
    ClaimsPrincipal User { get; }

    /// <summary>
    /// Initialze the current user from a externally provided token.
    /// </summary>
    /// <param name="userToken">A token to identify a user.</param>
    void FromToken(string userToken);
}

/// <summary>
/// 
/// </summary>
public static class ICurrentUserExtensions
{
    /// <summary>
    /// Get the user identification of a user instance.
    /// </summary>
    /// <param name="user">Some user instance.</param>
    /// <returns>Value of the name identifier claim.</returns>
    public static string GetUserId(this ICurrentUser user)
        => user?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
}