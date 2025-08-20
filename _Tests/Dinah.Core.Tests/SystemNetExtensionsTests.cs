﻿using Shouldly;

namespace SystemNetExtensionsTests
{
    [TestClass]
    public class EnumerateCookies
    {
        [TestMethod]
        public void get_all_in_uri()
        {
            var cookieJar = new CookieContainer();
            var uri1 = new Uri("http://www.example.com");
            var badUri = new Uri("http://www.test.com");

            var cookie1name = "uri1_name1";
            var cookie1value = "uri1 value1";
            var cookie2name = "uri1_name2";
            var cookie2value = "uri1 value2";
            cookieJar.Add(uri1, new Cookie(cookie1name, cookie1value));
            cookieJar.Add(uri1, new Cookie(cookie2name, cookie2value));

            cookieJar.Add(badUri, new Cookie("bad_name", "bad value"));

            Assert.AreEqual(3, cookieJar.Count);
            
            var uri1cookies = cookieJar.EnumerateCookies(uri1).ToList();
            Assert.AreEqual(2, uri1cookies.Count);
            Assert.AreEqual(uri1cookies[0].Name, cookie1name);
            Assert.AreEqual(uri1cookies[0].Value, cookie1value);
            Assert.AreEqual(uri1cookies[1].Name, cookie2name);
            Assert.AreEqual(uri1cookies[1].Value, cookie2value);
        }
    }

    [TestClass]
    public class ReflectOverAllCookies
    {
        [TestMethod]
        public void get_all()
        {
            // ARRANGE
            var cookies = new CookieContainer();

            cookies.Add(new Cookie("name1", "value1", "/", "domain1.com"));
            cookies.Add(new Cookie("name2", "value2", "/", "domain1.com"));
            cookies.Add(new Cookie("name3", "value3", "/", "domain2.com"));

            // ACT
            var hashTable = cookies.ReflectOverAllCookies();

            // ASSERT
            hashTable.Keys.Count.ShouldBe(2);
            var keys = hashTable.Keys.Cast<string>().ToList();
            keys.ShouldBe([".domain1.com", ".domain2.com"], ignoreOrder: true);
            keys.ShouldBe([".domain2.com", ".domain1.com"], ignoreOrder: true);

            var collection1 = cookies.GetCookies(new Uri("http://domain1.com/"));
            collection1.Count.ShouldBe(2);

            var collection1CookieNames = collection1.Cast<Cookie>().Select(c => c.Name).ToList();
            collection1CookieNames.ShouldBe(["name1", "name2"], ignoreOrder: true);

            var cookie1_1 = collection1["name1"];
            cookie1_1.Name.ShouldBe("name1");
            cookie1_1.Value.ShouldBe("value1");
            cookie1_1.Domain.ShouldBe("domain1.com");

            var cookie1_2 = collection1["name2"];
            cookie1_2.Name.ShouldBe("name2");
            cookie1_2.Value.ShouldBe("value2");
            cookie1_2.Domain.ShouldBe("domain1.com");

            var collection2 = cookies.GetCookies(new Uri("http://domain2.com/"));
            collection2.Count.ShouldBe(1);

            var collection2CookieNames = collection2.Cast<Cookie>().Select(c => c.Name).ToList();
            collection2CookieNames.ShouldBe(["name3"], ignoreOrder: true);

            var cookie2_1 = collection2[0];
            cookie2_1.Name.ShouldBe("name3");
            cookie2_1.Value.ShouldBe("value3");
            cookie2_1.Domain.ShouldBe("domain2.com");
        }
    }
}
