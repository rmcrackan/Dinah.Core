using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Dinah.Core;
using Dinah.Core.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

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
            dic.Count.Should().Be(1);
            dic["bbb"].Baz.Should().Be("111");
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
            dic.Count.Should().Be(1);
            dic["bbb"].Baz.Should().Be("111");
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
            dic.Count.Should().Be(1);
            dic["bbb"].Baz.Should().Be("222");
        }

    }
}
