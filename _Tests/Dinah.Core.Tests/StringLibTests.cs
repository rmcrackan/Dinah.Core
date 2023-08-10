namespace StringLibTests
{
    [TestClass]
    public class ExtractFirstNumber
    {
        [TestMethod]

        [DataRow(null, 0)]
        [DataRow("", 0)]
        [DataRow("   ", 0)]
        [DataRow("0", 0)]

        // letters only
        [DataRow("XYZ", 0)]

        // positive
        [DataRow("2", 2f)]
        [DataRow("0.3,4.7,8.6", 0.3f)]
        [DataRow("0.6, 3.5", 0.6f)]
        [DataRow("5-8", 5f)]
        [DataRow("5.", 5f)]
        [DataRow("0.5", 0.5f)]
        [DataRow("zzz1.12.zzz3.4", 1.12f)]

        // negative
        [DataRow("-2", -2f)]
        [DataRow("-0.3,4.7,8.6", -0.3f)]
        [DataRow("-0.6, 3.5", -0.6f)]
        [DataRow("-5-8", -5f)]
        [DataRow("-5.", -5f)]
        [DataRow("-0.5", -0.5f)]
        [DataRow("zzz-1.12.zzz3.4", -1.12f)]

        // no leading 0 means dot isn't recognized as a decimal point
        [DataRow(".5", 5f)]
        [DataRow("-.5", 5f)]
        [DataRow(".-5", -5f)]
        [DataRow("X.5", 5f)]
        [DataRow("X-.5", 5f)]
        [DataRow("-X.5", 5f)]
        [DataRow("X.-5", -5f)]
        public void match(string input, float result) => StringLib.ExtractFirstNumber(input).Should().Be(result);
	}
}
