namespace Dinah.Core.Tests
{
    class MyGenericBase<T> { }
    class MyGenericBaseTwo<T> : MyGenericBase<T> { }
    class MyConcrete : MyGenericBaseTwo<string> { }

    [TestClass]
    public class TypeExtensionsTests
    {
        [TestMethod]
        public void firstNull() => TypeExtensions.IsGenericOf(null, typeof(string)).ShouldBeFalse();

        [TestMethod]
        public void secondNull() => typeof(string).IsGenericOf(null).ShouldBeFalse();

        [TestMethod]
        [DataRow(typeof(int), typeof(string))]
        public void NoMatch(Type a, Type b) => a.IsGenericOf(b).ShouldBeFalse();

        [TestMethod]
        public void Matches()
        {
            {
                var t = new MyGenericBase<int>();
                t.GetType().IsGenericOf(typeof(MyGenericBase<>));
                t.GetType().IsGenericOf(typeof(MyGenericBaseTwo<>));
            }

            {
                var t = new MyGenericBase<string>();
                t.GetType().IsGenericOf(typeof(MyGenericBase<>));
                t.GetType().IsGenericOf(typeof(MyGenericBaseTwo<>));
            }
        }

        [TestMethod]
        [DataRow(typeof(MyGenericBase<>), typeof(MyGenericBase<>))]
        [DataRow(typeof(MyGenericBaseTwo<>), typeof(MyGenericBase<>))]
        [DataRow(typeof(MyConcrete), typeof(MyGenericBase<>))]
        [DataRow(typeof(MyConcrete), typeof(MyGenericBaseTwo<>))]
        public void Match(Type a, Type b) => a.IsGenericOf(b).ShouldBeTrue();
    }
}
