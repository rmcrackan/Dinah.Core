#nullable enable
namespace Dinah.Core.Security;

/// <summary>
/// Thrown when authenticated encryption or decryption fails.
/// Messages must never include plaintext, ciphertext, or key material.
/// </summary>
public class SecretProtectionException : Exception
{
	public SecretProtectionException(string message)
		: base(message)
	{
	}

	public SecretProtectionException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
