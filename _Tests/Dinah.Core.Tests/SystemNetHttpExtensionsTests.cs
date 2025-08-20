namespace SystemNetHttpExtensionsTests
{
    [TestClass]
    public class AddContent
    {
        [TestMethod]
        public void string_test()
        {
            var request = getEmptyMessage();

            var input = "my string";
            var content = new StringContent(input);

            request.AddContent(content);

            Assert.AreEqual(request.Content.Headers.ContentType.CharSet, "utf-8");
            Assert.AreEqual(request.Content.Headers.ContentType.MediaType, "text/plain");

            test_content(request, input);
        }

        [TestMethod]
        public void dictionary_test()
        {
            var request = getEmptyMessage();

            var dic = new Dictionary<string, string> { ["name1"] = "value 1", ["name2"] = "\"'&<>" };
            request.AddContent(dic);

            Assert.AreEqual(request.Content.Headers.ContentType.CharSet, null);
            Assert.AreEqual(request.Content.Headers.ContentType.MediaType, "application/x-www-form-urlencoded");

            test_content(request, "name1=value+1&name2=%22%27%26%3C%3E");
        }

        [TestMethod]
        public void json_test()
        {
            var request = getEmptyMessage();

            var jsonStr = "{\"name1\":\"value 1\"}";
            var json = JObject.Parse(jsonStr);
            request.AddContent(json);

            request.Content.Headers.ContentType.CharSet.ShouldBe("utf-8");
            request.Content.Headers.ContentType.MediaType.ShouldBe("application/json");

            test_content(request, JObject.Parse(jsonStr).ToString(Newtonsoft.Json.Formatting.Indented));
        }

        HttpRequestMessage getEmptyMessage()
        {
            var request = new HttpRequestMessage();
            Assert.AreEqual(request.Content, null);

            return request;
        }

        void test_content(HttpRequestMessage request, string expectedMessage)
        {
            var contentString = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.AreEqual(expectedMessage, contentString);
        }
    }

    [TestClass]
    public class ParseCookie
    {
        [TestMethod]
        public void null_param_throws()
            => Assert.ThrowsException<ArgumentNullException>(() => SystemNetHttpExtensions.ParseCookie(null));

        [TestMethod]
        public void test_cookie()
        {
            var cookie = SystemNetHttpExtensions.ParseCookie("session-id=139-1488065-0277455; Domain=.amazon.com; Expires=Thu, 30-Jun-2039 19:07:14 GMT; Path=/");
            cookie.Name.ShouldBe("session-id");
            cookie.Value.ShouldBe("139-1488065-0277455");
            cookie.Domain.ShouldBe(".amazon.com");
            cookie.Path.ShouldBe("/");
            cookie.Secure.ShouldBeFalse();
            cookie.Expires.ShouldBe(DateTime.Parse("Thu, 30-Jun-2039 19:07:14 GMT"));
        }
    }

	[TestClass]
	public class ReadAsJObjectAsync
	{
		[TestMethod]
		public async Task valid_FormUrlEncodedContent()
		{
			var message = new HttpResponseMessage
			{
				Content = new FormUrlEncodedContent(new Dictionary<string, string>
				{
					["k1"] = "v1",
					["k2"] = "!@#$%^&*()<>-=_:'\"\\\n"
				}),
				StatusCode = System.Net.HttpStatusCode.OK
			};

			var str = await message.Content.ReadAsStringAsync();
			str.ShouldBe("k1=v1&k2=%21%40%23%24%25%5E%26%2A%28%29%3C%3E-%3D_%3A%27%22%5C%0A");

			var jObj = await message.Content.ReadAsJObjectAsync();
			var json = jObj.ToString(Newtonsoft.Json.Formatting.Indented);
			var expected = @"
{
  ""k1"": ""v1"",
  ""k2"": ""!@#$%^&*()<>-=_:'\""\\\n""
}
		".Trim();

			json.ShouldBe(expected);
		}

		[TestMethod]
		public async Task not_supported_HttpContent_type()
		{
			var message = new HttpResponseMessage
			{
				Content = new StreamContent(new MemoryStream()),
				StatusCode = System.Net.HttpStatusCode.OK
			};

			await Assert.ThrowsExceptionAsync<JsonReaderException>(() => message.Content.ReadAsJObjectAsync());
		}

		[TestMethod]
		public async Task invalid_json()
		{
			var message = new HttpResponseMessage
			{
				Content = new StringContent("{\"a\""),
				StatusCode = System.Net.HttpStatusCode.OK
			};
			await Assert.ThrowsExceptionAsync<JsonReaderException>(() => message.Content.ReadAsJObjectAsync());
		}

		[TestMethod]
		public async Task valid_json()
		{
			var message = new HttpResponseMessage
			{
				Content = new StringContent("{'a':1}"),
				StatusCode = System.Net.HttpStatusCode.OK
			};
			var jObj = await message.Content.ReadAsJObjectAsync();
			jObj.ToString(Newtonsoft.Json.Formatting.None).ShouldBe("{\"a\":1}");
		}
	}

	[TestClass]
    public class DownloadFileAsync_ISealedHttpClient
    {
        [TestMethod]
        public async Task null_params_throw()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => SystemNetHttpExtensions.DownloadFileAsync((IHttpClientActions)null, "url", "file"));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => SystemNetHttpExtensions.DownloadFileAsync(Substitute.For<IHttpClientActions>(), null, "file"));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => SystemNetHttpExtensions.DownloadFileAsync(Substitute.For<IHttpClientActions>(), "url", null));
        }

        [TestMethod]
        public async Task blank_params_throw()
        {
            var mock = Substitute.For<IHttpClientActions>();
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => SystemNetHttpExtensions.DownloadFileAsync(mock, "", "file"));
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => SystemNetHttpExtensions.DownloadFileAsync(mock, "   ", "file"));
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => SystemNetHttpExtensions.DownloadFileAsync(mock, "url", ""));
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => SystemNetHttpExtensions.DownloadFileAsync(mock, "url", "   "));
        }
    }

    [TestClass]
    public class DownloadFileAsync_HttpClient
    {
        [TestMethod]
        public async Task null_params_throw()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => SystemNetHttpExtensions.DownloadFileAsync((HttpClient)null, "url", "file"));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => SystemNetHttpExtensions.DownloadFileAsync(Substitute.For<HttpClient>(), null, "file"));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => SystemNetHttpExtensions.DownloadFileAsync(Substitute.For<HttpClient>(), "url", null));
        }

        [TestMethod]
        public async Task blank_params_throw()
        {
            var mock = Substitute.For<HttpClient>();
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => SystemNetHttpExtensions.DownloadFileAsync(mock, "", "file"));
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => SystemNetHttpExtensions.DownloadFileAsync(mock, "   ", "file"));
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => SystemNetHttpExtensions.DownloadFileAsync(mock, "url", ""));
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => SystemNetHttpExtensions.DownloadFileAsync(mock, "url", "   "));
        }
	}
}
