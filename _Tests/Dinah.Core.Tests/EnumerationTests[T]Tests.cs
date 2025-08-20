namespace Enumeration_T_Tests
{
    public abstract class SubClassing : Enumeration<SubClassing>
    {
        // these may be fields or properties
        public static readonly SubClassing Manager = new ManagerType();
        public static SubClassing Servant { get; } = new ServantType();

        private SubClassing(int value, string displayName) : base(value, displayName) { }

        public abstract decimal BonusSize { get; }

        private class ManagerType : SubClassing
        {
            public ManagerType() : base(0, "Manager") { }
            public override decimal BonusSize => 1000m;
        }

        private class ServantType : SubClassing
        {
            public ServantType() : base(1, "Servant") { }
            public override decimal BonusSize => 0m;
        }
    }

    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void Value_is_stored()
            => SubClassing.Manager.Value.ShouldBe(0);
    }

    [TestClass]
    public class GetAll
    {
        [TestMethod]
        public void verify_all_values()
        {
            var all = Enumeration<SubClassing>.GetAll();
            all.Count().ShouldBe(2);
            all.Any(a => a.Value == 0).ShouldBeTrue();
            all.Any(a => a.Value == 1).ShouldBeTrue();
        }
    }

    [TestClass]
    public class FromValue
    {
        [TestMethod]
        public void get_manager()
            => Enumeration<SubClassing>.FromValue(0)
            .ShouldBe(SubClassing.Manager);
    }

    [TestClass]
    public class FromDisplayName
    {
        [TestMethod]
        public void get_manager()
            => Enumeration<SubClassing>.FromDisplayName("Manager")
            .ShouldBe(SubClassing.Manager);
    }
}
