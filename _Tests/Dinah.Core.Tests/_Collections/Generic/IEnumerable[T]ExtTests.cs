namespace IEnumerable_T_ExtTests
{
    [TestClass]
    public class ToDictionarySafe
    {
        record Foo(string Bar, string Baz);

        [TestMethod]
        public void first_wins_default()
        {
            var list = new List<Foo>
            {
                new("bbb", "111"),
                new("bbb", "222"),
            };

            var dic = list.ToDictionarySafe(x => x.Bar);
            dic.Count.ShouldBe(1);
            dic["bbb"].Baz.ShouldBe("111");
        }

        [TestMethod]
        public void first_wins_explicit()
        {
            var list = new List<Foo>
            {
                new("bbb", "111"),
                new("bbb", "222"),
            };

            var dic = list.ToDictionarySafe(x => x.Bar, WinnerEnum.FirstInWins);
            dic.Count.ShouldBe(1);
            dic["bbb"].Baz.ShouldBe("111");
        }

        [TestMethod]
        public void last_wins()
        {
            var list = new List<Foo>
            {
                new("bbb", "111"),
                new("bbb", "222"),
            };

            var dic = list.ToDictionarySafe(x => x.Bar, WinnerEnum.LastInWins);
            dic.Count.ShouldBe(1);
            dic["bbb"].Baz.ShouldBe("222");
        }

    }
}
