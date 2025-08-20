namespace SystemIOExtensionsTests
{
    [TestClass]
    public class MD5
    {
        [TestMethod]
        public void verify_hash()
        {
            var temp = Path.GetTempFileName();
            try
            {
                // base 64 of a txt file with "test"
                var base64 = "dGVzdA==";
                var bytes = Convert.FromBase64String(base64);
                File.WriteAllBytes(temp, bytes);

                var info = new FileInfo(temp);
                var md5 = info.MD5();
                md5.ShouldBe("d41d8cd98f00b204e9800998ecf8427e");
            }
            finally
            {
                File.Delete(temp);
            }
        }
    }
}
