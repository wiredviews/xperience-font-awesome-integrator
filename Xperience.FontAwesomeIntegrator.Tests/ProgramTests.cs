using System;
using Fluent.IO;
using FluentAssertions;
using Xunit;

namespace Xperience.FontAwesomeIntegrator.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void Program_Will_Throw_An_Exception_If_No_CMSPath_Is_Provided()
        {
            Action act = () => Program.Main(new string[] { });

            act.Should().Throw<Exception>().WithMessage($"*{nameof(Program.CMSPath)}*");
        }

        [Fact]
        public void Program_Will_Throw_An_Exception_If_No_FontAwesomePath_Is_Provided()
        {
            Action act = () => Program.Main(new[] { "-cpath", @"C:\test" });

            act.Should().Throw<Exception>().WithMessage($"*{nameof(Program.FontAwesomePath)}*");
        }

        [Fact]
        public void Program_Will_Process_Font_Awesome_Files()
        {
            Program.Main(new[] { "-fpath", @"./SampleFiles/input", "-cpath", @"./SampleFiles/output" });

            var custom = Path.Get(@"./SampleFiles/output/App_Themes/Default").Directories(d => d.FileName == "Custom");

            custom.Should().NotBeNull();

            var webfonts = custom.Directories(d => d.FileName == "webfonts");

            webfonts.Should().NotBeNull();

            webfonts.AllFiles().Should().HaveCount(2);

            var expected = Path.Get(@"./SampleFiles/expected");

            string manifestOutputContent = custom.Files(f => f.FileName == "icon-css-classes.txt").Read().Trim();
            string manifestExpectedContext = expected.Files(f => f.FileName == "icon-css-classes.txt").Read().Trim();

            manifestOutputContent.Should().Be(manifestExpectedContext);

            string styleOutputContent = custom.Files(f => f.FileName == "style.css").Read().Trim();
            string styleExpectedContent = expected.Files(f => f.FileName == "style.css").Read().Trim();

            styleOutputContent.Should().Be(styleExpectedContent);

            var licenseFile = custom.Files(f => f.FileName == "LICENSE.txt");

            licenseFile.Should().NotBeNull();
        }
    }
}
