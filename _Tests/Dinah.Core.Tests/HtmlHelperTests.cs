﻿namespace HtmlHelperTests
{
    [TestClass]
    public class GetInputs
    {
        [TestMethod]
        public void null_param_throws()
            => Assert.ThrowsException<ArgumentNullException>(() => HtmlHelper.GetInputs(null));

        string basicHidden => "<input type='hidden' name='foo' value='bar' />";
        string basicCb => "<input type='checkbox' name='cbFoo' value='cbBar' />";

        [TestMethod]
        public void empty()
            => HtmlHelper.GetInputs("").Count.Should().Be(0);

        [TestMethod]
        public void no_match()
            => HtmlHelper.GetInputs("<p></p>").Count.Should().Be(0);

        [TestMethod]
        public void match_hidden()
        {
            var inputs = HtmlHelper.GetInputs(basicHidden);
            inputs.Count.Should().Be(1);
            inputs.ContainsKey("foo").Should().BeTrue();
            inputs["foo"].Should().Be("bar");
        }

        [TestMethod]
        public void match_checkbox()
        {
            var inputs = HtmlHelper.GetInputs(basicCb);
            inputs.Count.Should().Be(1);
            inputs.ContainsKey("cbFoo").Should().BeTrue();
            inputs["cbFoo"].Should().Be("cbBar");
        }

        [TestMethod]
        public void match_hidden_and_checkbox()
        {
            var both = basicHidden + basicCb;

            var inputs = HtmlHelper.GetInputs(both);

            inputs.Count.Should().Be(2);

            inputs.ContainsKey("foo").Should().BeTrue();
            inputs["foo"].Should().Be("bar");

            inputs.ContainsKey("cbFoo").Should().BeTrue();
            inputs["cbFoo"].Should().Be("cbBar");
        }

        [TestMethod]
        public void find_1_top_level()
        {
            var inputs = HtmlHelper.GetInputs(basicHidden);
            inputs.Count.Should().Be(1);
            inputs.ContainsKey("foo").Should().BeTrue();
            inputs["foo"].Should().Be("bar");
        }

        [TestMethod]
        public void find_1_nest_1()
        {
            var html = $"<p>{basicHidden}</p>";
            var inputs = HtmlHelper.GetInputs(html);
            inputs.Count.Should().Be(1);
            inputs.ContainsKey("foo").Should().BeTrue();
            inputs["foo"].Should().Be("bar");
        }

        [TestMethod]
        public void find_1_nest_2()
        {
            var html = $"<p><p>{basicHidden}</p></p>";
            var inputs = HtmlHelper.GetInputs(html);
            inputs.Count.Should().Be(1);
            inputs.ContainsKey("foo").Should().BeTrue();
            inputs["foo"].Should().Be("bar");
        }

        [TestMethod]
        public void find_1_nest_3()
        {
            var html = $"<p><p><p>{basicHidden}</p></p></p>";
            var inputs = HtmlHelper.GetInputs(html);
            inputs.Count.Should().Be(1);
            inputs.ContainsKey("foo").Should().BeTrue();
            inputs["foo"].Should().Be("bar");
        }

        [TestMethod]
        public void find_1_invalid_nest1()
        {
            var html = $"<p><p><p>{basicHidden}</p></p>";
            var inputs = HtmlHelper.GetInputs(html);
            inputs.Count.Should().Be(1);
            inputs.ContainsKey("foo").Should().BeTrue();
            inputs["foo"].Should().Be("bar");
        }

        [TestMethod]
        public void find_1_invalid_nest2()
        {
            var html = $"<p><p>{basicHidden}</p></p></p>";
            var inputs = HtmlHelper.GetInputs(html);
            inputs.Count.Should().Be(1);
            inputs.ContainsKey("foo").Should().BeTrue();
            inputs["foo"].Should().Be("bar");
        }

        [TestMethod]
        public void dont_capture_null()
        {
            var html = @"<input type='hidden' value='foo' />";
            var inputs = HtmlHelper.GetInputs(html);
            inputs.Count.Should().Be(0);
        }

        [TestMethod]
        public void dont_capture_empty()
        {
            var html = @"<input type='hidden' name='' value='foo' />";
            var inputs = HtmlHelper.GetInputs(html);
            inputs.Count.Should().Be(0);
        }

        [TestMethod]
        public void dont_capture_blank()
        {
            var html = @"<input type='hidden' name='   ' value='foo' />";
            var inputs = HtmlHelper.GetInputs(html);
            inputs.Count.Should().Be(0);
        }

        [TestMethod]
        public void find_many()
        {
            var html = @"
<p>
  <p>
    <input type='hidden' name='h1' value='hv1' />
    <input type='hidden' name='h2' value='hv2' />
  </p>
</p>
<p>
  <input type='checkbox' name='cb1' value='cbv1' />
  <input type='checkbox' name='cb2' value='cbv2' />
</p>
  <input type='email' name='e1' value='a@b.xyz' />

  don't capture null: <input type='hidden' value='foo' />
  don't capture empty: <input type='hidden' name='' value='foo' />
";

            var inputs = HtmlHelper.GetInputs(html);

            inputs.Count.Should().Be(5);

            inputs.ContainsKey("h1").Should().BeTrue();
            inputs["h1"].Should().Be("hv1");

            inputs.ContainsKey("h2").Should().BeTrue();
            inputs["h2"].Should().Be("hv2");

            inputs.ContainsKey("cb1").Should().BeTrue();
            inputs["cb1"].Should().Be("cbv1");

            inputs.ContainsKey("cb2").Should().BeTrue();
            inputs["cb2"].Should().Be("cbv2");

            inputs.ContainsKey("e1").Should().BeTrue();
            inputs["e1"].Should().Be("a@b.xyz");
        }
    }

    [TestClass]
    public class GetLinks
    {
        [TestMethod]
        public void _1_link()
        {
            var url = "http://example.com/a?b=c";
            var html = $@"<body><p><a href='{url}'></p></body>";
            var links = HtmlHelper.GetLinks(html);
            links.Count.Should().Be(1);
            links[0].Should().Be(url);
        }

        [TestMethod]
        public void _2_links()
        {
            var url1 = "http://example.com/a?b=c";
            var url2 = "#";
            var html = $@"<body><p><a href='{url1}'></p><p><a href='{url2}' class='foo'></p></body>";
            var links = HtmlHelper.GetLinks(html);
            links.Count.Should().Be(2);
            links[0].Should().Be(url1);
            links[1].Should().Be(url2);
        }

        [TestMethod]
        public void link_with_class()
        {
            var fooUrl = "http://example.com/a?b=c";
            var barUrl = "#";
            var html = $@"<body><p><a href='{fooUrl}' class='foo'></p><p><a href='{barUrl}' class='bar'></p></body>";
            var links = HtmlHelper.GetLinks(html, "foo");
            links.Count.Should().Be(1);
            links[0].Should().Be(fooUrl);
        }
    }

    [TestClass]
    public class GetDivCount
    {
        [TestMethod]
        public void _0_divs()
        {
            var html = "<body><p></p></body>";
            HtmlHelper.GetDivCount(html).Should().Be(0);
        }
        [TestMethod]
        public void _1_div()
        {
            var html = "<body><p><div /></p></body>";
            HtmlHelper.GetDivCount(html).Should().Be(1);
        }
        [TestMethod]
        public void _1_div_0_matches()
        {
            var html = "<body><p><div id='foo' /></p></body>";
            HtmlHelper.GetDivCount(html, "bar").Should().Be(0);
        }
        [TestMethod]
        public void _3_divs_2_matches()
        {
            var html = "<body><p><div id='foo' /><div id='bar' /><div id='bar' /></p></body>";
            HtmlHelper.GetDivCount(html, "bar").Should().Be(2);
        }
    }

    [TestClass]
    public class GetElements
    {
        const string SAMPLE = @"
    <div class='a-row a-spacing-small'>
      <fieldset class='a-spacing-small'>
        
          <div data-a-input-name='otpDeviceContext' class='a-radio auth-TOTP'><label><input type='radio' name='otpDeviceContext' value='aAbBcC=, TOTP' checked/><i class='a-icon a-icon-radio'></i><span class='a-label a-radio-label'>
            Enter the OTP from the authenticator app
          </span></label></div>
        
          <div data-a-input-name='otpDeviceContext' class='a-radio auth-SMS'><label><input type='radio' name='otpDeviceContext' value='dDeEfE=, SMS'/><i class='a-icon a-icon-radio'></i><span class='a-label a-radio-label'>
            Send an SMS to my number ending with 123
          </span></label></div>
        
          <div data-a-input-name='otpDeviceContext' class='a-radio auth-VOICE'><label><input type='radio' name='otpDeviceContext' value='dDeEfE=, VOICE'/><i class='a-icon a-icon-radio'></i><span class='a-label a-radio-label'>
            Call me on my number ending with 123
          </span></label></div>
        
      </fieldset>
    </div>
";
        [TestMethod]
		public void parse_sample()
		{
            var otpDivs = HtmlHelper.GetElements(SAMPLE, "div", "data-a-input-name", "otpDeviceContext");
            otpDivs.Count.Should().Be(3);

            {
                var otp = otpDivs[0];

                var inputNode = otp.SelectSingleNode(".//input");
                var name = inputNode.Attributes["name"]?.Value;
                name.Should().Be("otpDeviceContext");
                var value = inputNode.Attributes["value"]?.Value;
                value.Should().Be("aAbBcC=, TOTP");

                var span = otp.SelectSingleNode(".//span");
                var text = span?.InnerText.Trim();
                text.Should().Be("Enter the OTP from the authenticator app");
            }

            {
                var otp = otpDivs[1];

                var inputNode = otp.SelectSingleNode(".//input");
                var name = inputNode.Attributes["name"]?.Value;
                name.Should().Be("otpDeviceContext");
                var value = inputNode.Attributes["value"]?.Value;
                value.Should().Be("dDeEfE=, SMS");

                var span = otp.SelectSingleNode(".//span");
                var text = span?.InnerText.Trim();
                text.Should().Be("Send an SMS to my number ending with 123");
            }

            {
                var otp = otpDivs[2];

                var inputNode = otp.SelectSingleNode(".//input");
                var name = inputNode.Attributes["name"]?.Value;
                name.Should().Be("otpDeviceContext");
                var value = inputNode.Attributes["value"]?.Value;
                value.Should().Be("dDeEfE=, VOICE");

                var span = otp.SelectSingleNode(".//span");
                var text = span?.InnerText.Trim();
                text.Should().Be("Call me on my number ending with 123");
            }
        }

        [TestMethod]
		public void get_by_tag()
		{
            var title = "My Title";
            var html = $"<head><title foo='bar'>{title}</title></head>";
            var nodes = HtmlHelper.GetElements(html, "title");
            nodes.Count.Should().Be(1);
            nodes[0].InnerText.Should().Be(title);
		}
    }
}
