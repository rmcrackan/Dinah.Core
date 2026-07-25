#nullable enable
namespace Dinah.Core.Security;

/// <summary>
/// Thrown when an OS secret store is required but unavailable or insecure on this environment.
/// Messages must never include secret material.
/// </summary>
public class OsSecretStoreUnavailableException : Exception
{
	public string StoreName { get; }

	public OsSecretStoreUnavailableException(string storeName, string message)
		: base(message)
	{
		StoreName = storeName;
	}

	public OsSecretStoreUnavailableException(string storeName, string message, Exception innerException)
		: base(message, innerException)
	{
		StoreName = storeName;
	}
}
