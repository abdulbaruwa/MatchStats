using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Akavache;
using MatchStats.Enums;
using MatchStats.Model;
using MatchStats.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ReactiveUI;

namespace MatchStats.Test.ViewModels.MatchScoreViewModelTests
{
    [TestClass]
    public class ActionsViewModelTests
    {

        [TestMethod]
        public void ShouldEnableAceActionCommandWhenAPlayerIsServing()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);

            Assert.IsTrue(fixture.PlayerOneActions.Any(x => x.Name == "AceServe" && x.IsEnabled));
        }
    }
}