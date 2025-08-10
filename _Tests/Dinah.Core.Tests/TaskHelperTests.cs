namespace Dinah.Core.Tests
{
    [TestClass]
    public class WhenAll
    {
        private async Task<int> Task1Async() => await Task.FromResult(1);
        private async Task<int> Task2Async() => await Task.FromResult(2);
        private async Task<int> Task3Async() => await Task.FromResult(3);

        [TestMethod]
        public async Task mult_returns()
        {
            var (result1, result2, result3) = await TaskHelper.WhenAll(Task1Async(), Task2Async(), Task3Async());
            Assert.AreEqual(1, result1);
            Assert.AreEqual(2, result2);
            Assert.AreEqual(3, result3);
        }

        [TestMethod]
        public async Task ducktyping()
        {
            var (result1, result2, result3) = await (
                Task1Async(),
                Task2Async(),
                Task3Async());
            Assert.AreEqual(1, result1);
            Assert.AreEqual(2, result2);
            Assert.AreEqual(3, result3);
        }
    }
}
