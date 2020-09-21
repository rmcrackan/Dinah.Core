using System.Net.Http;

namespace Dinah.Core.Net.Http
{
	/// <summary>
	/// Only expose actions. Omit properties and access to state
	/// </summary>
	public class SealedHttpClient : HttpClient, IHttpClientActions
	{
		public SealedHttpClient(HttpMessageHandler handler) : base(handler) { }
	}
}
