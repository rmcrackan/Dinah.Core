namespace EnumerationTests
{
    public abstract class SubClassing : Enumeration
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

        [TestMethod]
        public void DisplayName_is_stored()
            => SubClassing.Manager.DisplayName.ShouldBe("Manager");
    }

    [TestClass]
    public class ToString
    {
        [TestMethod]
        public void outputs_DisplayName()
            => SubClassing.Manager.ToString().ShouldBe("Manager");
    }

    [TestClass]
    public class GetAll_T_
    {
        [TestMethod]
        public void verify_all_values()
        {
            var all = Enumeration.GetAll<SubClassing>();
            all.Count().ShouldBe(2);
            all.Any(a => a.Value == 0).ShouldBeTrue();
            all.Any(a => a.Value == 1).ShouldBeTrue();
        }
    }

    [TestClass]
    public class Equals
    {
        [TestMethod]
        public void instances_are_equal()
        {
            var manager1 = SubClassing.Manager;
            var manager2 = SubClassing.Manager;
            Assert.AreEqual(manager1, manager2);
        }
    }

    [TestClass]
    public class FromValue_T_
    {
        [TestMethod]
        public void get_manager()
            => Enumeration.FromValue<SubClassing>(0)
            .ShouldBe(SubClassing.Manager);
    }

    [TestClass]
    public class FromDisplayName_T_
    {
        [TestMethod]
        public void get_manager()
            => Enumeration.FromDisplayName<SubClassing>("Manager")
            .ShouldBe(SubClassing.Manager);
    }

    [TestClass]
    public class CompareTo
    {
        [TestMethod]
        public void compare_is_equal()
            => SubClassing.Manager
            .CompareTo(SubClassing.Manager)
            .ShouldBe(0);
    }
}
