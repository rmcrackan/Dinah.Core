namespace NSubstituteExamples
{
    [TestClass]
    public class scratchPad
    {
        public interface IRandomNumberGenerator
        {
			int Seed { get; set; }
            int Next();
            int Next(int minValue, int maxValue);
        }

        [TestMethod]
        public void constant_return_method()
        {
            var mockRng = Substitute.For<IRandomNumberGenerator>();
            mockRng.Next().Returns(3);
            mockRng.Next().ShouldBe(3);
        }

        [TestMethod]
        public void constant_return_get()
        {
            var seed = -12345;
            var mockRng = Substitute.For<IRandomNumberGenerator>();
            mockRng.Seed.Returns(seed);
            mockRng.Seed.ShouldBe(seed);
        }

        [TestMethod]
        public void conditional_return()
        {
            var rng = Substitute.For<IRandomNumberGenerator>();
            rng.Next(995, 1005).Returns(1001);

            var undefined = rng.Next(1, 2);
            undefined.ShouldBe(0);

            var _1001 = rng.Next(995, 1005);
            _1001.ShouldBe(1001);
        }

        [TestMethod]
        public void return_sequentially()
        {
            var rng = Substitute.For<IRandomNumberGenerator>();
			var seed1 = 98765;
			var seed2 = -12345;
            rng.Seed.Returns(seed1, seed2);

            rng.Seed.ShouldBe(seed1);
            rng.Seed.ShouldBe(seed2);
        }
    }
}
