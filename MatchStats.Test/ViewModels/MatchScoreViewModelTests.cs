using System;
using System.Collections.Generic;
using System.Linq;
using Akavache;
using MatchStats.Enums;
using MatchStats.Model;
using MatchStats.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ReactiveUI;

namespace MatchStats.Test.ViewModels
{
    [TestClass]
    public class MatchScoreViewModelTests
    {

        [TestMethod]
        public void ShouldSaveGivenGameToListOfGames()
        {
            //Arrange
            var blobCache = RegisterComponents();
            var currentListOfMatches = new List<Match>();
            var newListOfMatchesAfterSave = new List<Match>();

            blobCache.GetObjectAsync<List<Match>>("MyMatches").Subscribe(currentListOfMatches.AddRange,
                ex =>
                {
                   //Ignore the exception that the list may not exist. 
                });

            var fixture = BuildAMatchToScore();

            //Act => Send message with Match details
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);     

            //Assert => it has been saved
            blobCache.GetObjectAsync<List<Match>>("MyMatches").Subscribe(newListOfMatchesAfterSave.AddRange,
                ex => {/**Ignore any exceptions**/});
            Assert.IsTrue((currentListOfMatches.Count + 1) == newListOfMatchesAfterSave.Count);
        }

        [TestMethod]
        public void ShouldSaveMatchInCurrentMatchEntity()
        {
            //Arrange
            var blobCache = RegisterComponents();

            var currentListOfMatches = new List<Match>();
            blobCache.GetObjectAsync<List<Match>>("MyMatches").Subscribe(currentListOfMatches.AddRange,
                ex =>
                {
                    //Ignore the exception that the list my not exist. 
                });

            var fixture = BuildAMatchToScore();

            //Act => 
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);

            //Assert => CurrentMatch is now this match
            blobCache.GetObjectAsync<Match>("CurrentMatch").Subscribe(x => Assert.AreEqual(x.MatchGuid, fixture.CurrentMatch.MatchGuid),
                ex => Assert.Fail("Current Match not saved"));
        }

        [TestMethod]
        public void ShouldAddPointForOpponentOnDoubleFaultAction()
        {
            //Arrange
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);

            //Act
            fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);

            //Assert
            Assert.IsTrue(fixture.CurrMatch.Score.Games.First(x => x.IsCurrentGame).PlayerTwoScore.Equals(1), "Point not added for action");
            Assert.IsTrue(fixture.CurrMatch.Score.Games.First(x => x.IsCurrentGame).PlayerOneScore.Equals(0), "Wrongly updated the score for player");
        }

        [TestMethod]
        public void ShouldAddPointForPlayerOneIfOpponentDoubleFaults()
        {
            //Arrange
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);

            //Act
            fixture.PlayerTwoActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);

            //Assert
            Assert.IsTrue(fixture.CurrMatch.Score.Games.First(x => x.IsCurrentGame).PlayerTwoScore.Equals(0), "Wrongly updated the score for player");
            Assert.IsTrue(fixture.CurrMatch.Score.Games.First(x => x.IsCurrentGame).PlayerOneScore.Equals(1), "Point not added for action");

        }

        [TestMethod]
        public void ShouldAddPointForPlayerOneForForeHandWinner()
        {
            //Arrange
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);

            //Act
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            //Assert
            Assert.IsTrue(fixture.CurrMatch.Score.Games.First(x => x.IsCurrentGame).PlayerOneScore.Equals(1), "Point not added for action");
            Assert.IsTrue(fixture.CurrMatch.Score.Games.First(x => x.IsCurrentGame).PlayerTwoScore.Equals(0), "Wrongly updated the score for player");
        }

        [TestMethod]
        public void ShouldFlagAGameAsWonWhenAplayerAcquiresTheRelevantPointsForNormalGameAndNormalDuece()
        {

            //Arrange
            //To win a normal game, player should
            //1. Lead by 2
            //2. Be the first to reach 5 whilst leading by at least 2,
            //(1)15, (2)30, (3)40, (4) 50 (5)
            //(1)15, (2)30, (3)40, (4) 50 (5)
            // 

            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);

            //Act
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            //Assert
            Assert.IsNotNull(fixture.CurrMatch.Score.Games.First(x => x.IsCurrentGame).Winner, "Failed to flag game with a valid winner");
            Assert.AreEqual(fixture.CurrMatch.Score.Games.First(x => x.IsCurrentGame).Winner.FirstName, fixture.CurrMatch.PlayerOne.FirstName, "Failed to flag game as over");
        }
    
        [TestMethod]
        public void ShouldSetGameStatusAsDueceAt4040Game()
        {

            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);

            //Act
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            //Assert
            Assert.IsNull(fixture.CurrMatch.Score.Games.First(x => x.IsCurrentGame).Winner, "Game is flagged as won when it should be Duece");
            Assert.IsTrue(fixture.CurrMatch.Score.Games.First(x => x.IsCurrentGame).GameStatus.Status == Status.Duece, "Game Status should be flagged as Duece");
        }

        [TestMethod]
        public void ShouldSetGameStatusAsBreakPointIfAT3040Game()
        {

            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);

            //Act
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            //Assert
            Assert.IsNull(fixture.CurrMatch.Score.Games.First(x => x.IsCurrentGame).Winner, "Game is flagged as won when it should be Duece");
            Assert.IsTrue(fixture.CurrMatch.Score.Games.First(x => x.IsCurrentGame).GameStatus.Status == Status.BreakPoint, "Game Status should be flagged as Break Point");
            Assert.AreEqual(fixture.CurrMatch.Score.Games.First(x => x.IsCurrentGame).GameStatus.Player.FirstName, fixture.CurrMatch.PlayerTwo.FirstName, "Game Status should be Break Point for PlayerTwo");
        }

        private static TestBlobCache RegisterComponents()
        {
            var blobCache = new TestBlobCache();
            RxApp.MutableResolver.Register(() => new MatchStatsApi(), typeof (IMatchStatsApi));
            RxApp.MutableResolver.RegisterConstant(blobCache, typeof (IBlobCache), "UserAccount");
            return blobCache;
        }

        private static MatchScoreViewModel BuildAMatchToScore()
        {
            var fixture = new MatchScoreViewModel();

            fixture.NewMatchControlViewModel = new NewMatchControlViewModel();
            fixture.NewMatchControlViewModel.PlayerOneFirstName = "William";
            fixture.NewMatchControlViewModel.PlayerOneLastName = "Dof";
            fixture.NewMatchControlViewModel.PlayerTwoFirstName = "Drake";
            fixture.NewMatchControlViewModel.PlayerTwoLastName = "Dufus";
            fixture.NewMatchControlViewModel.SelectedGrade = Grade.Grade3;
            fixture.NewMatchControlViewModel.SelectedAgeGroup = AgeGroup.U14;
            fixture.NewMatchControlViewModel.SelectedDueceFormat = DueceFormat.Normal;
            fixture.NewMatchControlViewModel.SelectedFinalSet = FinalSetFormats.Normal;
            fixture.NewMatchControlViewModel.SelectedPlayerOneRating = Rating.FiveOne;
            fixture.NewMatchControlViewModel.SelectedPlayerTwoRating = Rating.FiveOne;
            fixture.NewMatchControlViewModel.SelectedSetsFormat = SetsFormat.ShortSetToFour;
            fixture.NewMatchControlViewModel.TournamentName = "Sutton Open";
            return fixture;
        }

        private Match BuildTestMatchObject(Guid matchGuid)
        {
            var matchToSave = new Match();
            matchToSave.MatchFormat = new MatchFormat()
            {
                DueceFormat = DueceFormat.SuddenDeath,
                FinalSetType = FinalSetFormats.TenPointChampionShipTieBreak,
                Sets = 3,
                SetsFormat = SetsFormat.ShortSetToFour
            };
            matchToSave.PlayerOne = new Player()
            {
                FirstName = "Raphael",
                SurName = "Nadal",
                Rating = "3.1"
            };

            matchToSave.PlayerTwo = new Player()
            {
                FirstName = "Gael",
                SurName = "Monphis",
                Rating = "3.1"
            };

            matchToSave.Tournament = new Tournament()
            {
                Grade = Grade.Regional,
                TournamentName = "Surrey Open",
                StartDate = DateTime.Now
            };
            matchToSave.MatchGuid = matchGuid;
            return matchToSave;
        }
    }
}