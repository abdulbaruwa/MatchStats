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
            blobCache.GetObjectAsync<Match>("CurrentMatch").Subscribe(x => Assert.AreEqual(x.MatchGuid, fixture.CurrMatch.MatchGuid),
                ex => Assert.Fail("Current Match not saved"));
        }

        [TestMethod]
        public void ShouldAddPointForOpponentOnDoubleFaultAction()
        {
            //Arrange
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);

            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            //Act
            fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);

            //Assert
            Assert.IsTrue(fixture.CurrMatch.CurrentGame().PlayerTwoScore.Equals(1), "Point not added for action");
            Assert.IsTrue(fixture.CurrMatch.CurrentGame().PlayerOneScore.Equals(0), "Wrongly updated the score for player");
        }

        [TestMethod]
        public void ShouldAddPointForPlayerOneIfOpponentDoubleFaults()
        {
            //Arrange
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);

            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            //Act
            fixture.PlayerTwoActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);

            //Assert
            Assert.IsTrue(fixture.CurrMatch.CurrentGame().PlayerTwoScore.Equals(0), "Wrongly updated the score for player");
            Assert.IsTrue(fixture.CurrMatch.CurrentGame().PlayerOneScore.Equals(1), "Point not added for action");

        }

        [TestMethod]
        public void ShouldAddPointForPlayerOneForForeHandWinner()
        {
            //Arrange
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);

            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            //Act
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            //Assert
            Assert.IsTrue(fixture.CurrMatch.CurrentGame().PlayerOneScore.Equals(1), "Point not added for action");
            Assert.IsTrue(fixture.CurrMatch.CurrentGame().PlayerTwoScore.Equals(0), "Wrongly updated the score for player");
        }

        [TestMethod]
        public void ShouldFlagAGameAsWonWhenAplayerAcquiresTheRelevantPointsForNormalGameAndNormalDeuce()
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
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);
            //Act
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            //Assert
            Assert.IsNotNull(fixture.CurrMatch.GetGameForKnownSetAndGame(1,1).Winner, "Failed to flag game with a valid winner");
            Assert.AreEqual(fixture.CurrMatch.GetGameForKnownSetAndGame(1,1).Winner.FirstName, fixture.CurrMatch.PlayerOne.FirstName, "Failed to flag game as over");
        }
    
        [TestMethod]
        public void ShouldSetGameStatusAsDeuceAt4040Game()
        {

            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            //Act
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            //Assert
            Assert.IsNull(fixture.CurrMatch.CurrentGame().Winner, "Game is flagged as won when it should be Deuce");
            Assert.IsTrue(fixture.CurrMatch.CurrentGame().GameStatus.Status == Status.Deuce, "Game Status should be flagged as Deuce");
        }

        [TestMethod]
        public void ShouldSetGameStatusAsBreakPointIfAT3040Game()
        {

            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            //Act
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            //Assert
            Assert.IsNull(fixture.CurrMatch.CurrentGame().Winner, "Game is flagged as won when it should be Deuce");
            Assert.IsTrue(fixture.CurrMatch.CurrentGame().GameStatus.Status == Status.BreakPoint, "Game Status should be flagged as Break Point");
            Assert.AreEqual(fixture.CurrMatch.CurrentGame().GameStatus.Player.FirstName, fixture.CurrMatch.PlayerTwo.FirstName, "Game Status should be Break Point for PlayerTwo");
        }

        [TestMethod]
        public void ShouldSetGameStatusAsGamePointIfAT4030Game()
        {

            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            //Act
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            //Assert
            Assert.IsNull(fixture.CurrMatch.CurrentGame().Winner, "Game is flagged as won when it should not be");
            Assert.IsTrue(fixture.CurrMatch.CurrentGame().GameStatus.Status == Status.GamePoint, "Game Status should be flagged as Game Point");
            Assert.AreEqual(fixture.CurrMatch.CurrentGame().GameStatus.Player.FirstName, fixture.CurrMatch.PlayerTwo.FirstName, "Game Status should be Break Point for PlayerTwo");
        }

        [TestMethod]
        public void ShouldSetGameStatusAsGameIfAT5030Game()
        {

            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            //Act
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            //Assert
            Assert.IsNotNull(fixture.CurrMatch.GetGameForKnownSetAndGame(1,1).Winner, "Game Winner should not be Null");
            Assert.IsTrue(fixture.CurrMatch.GetGameForKnownSetAndGame(1,1).GameStatus.Status == Status.GameOver, "Game Status should be flagged as Game Point");
            Assert.AreEqual(fixture.CurrMatch.GetGameForKnownSetAndGame(1,1).GameStatus.Player.FirstName, fixture.CurrMatch.PlayerTwo.FirstName, "Game Status should be IsMatchOver for PlayerTwo");
            Assert.AreEqual(fixture.CurrMatch.GetGameForKnownSetAndGame(1,1).Winner.FirstName, fixture.CurrMatch.PlayerTwo.FirstName, "Game Winner should be PlayerTwo");
        }

        [TestMethod]
        public void ShouldSetGameStatusAsGameIfAt3050Game()
        {

            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            //Act  --> Score is 30-50
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            //Assert
            Assert.IsNotNull(fixture.CurrMatch.GetGameForKnownSetAndGame(1,1).Winner, "Game Winner should not be Null");
            Assert.IsTrue(fixture.CurrMatch.GetGameForKnownSetAndGame(1,1).GameStatus.Status == Status.GameOver, "Game Status should be flagged as Game Point");
            Assert.AreEqual(fixture.CurrMatch.GetGameForKnownSetAndGame(1,1).GameStatus.Player.FirstName, fixture.CurrMatch.PlayerOne.FirstName, "Game Status should be IsMatchOver for PlayerOne");
            Assert.AreEqual(fixture.CurrMatch.GetGameForKnownSetAndGame(1,1).Winner.FirstName, fixture.CurrMatch.PlayerOne.FirstName, "Game Winner should be PlayerOne");
        }

        [TestMethod]
        public void ShouldSetGameStatusAsAdvantageAt15UpAfterDeuce()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            //Act  --> Score is 40-40
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            //Act --> Score is Advantage to PlayerTwo
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            //Assert
            Assert.IsNull(fixture.CurrMatch.CurrentGame().Winner, "Game Winner should be Null");
            Assert.IsTrue(fixture.CurrMatch.CurrentGame().GameStatus.Status == Status.Advantage, "Game Status should be flagged as Advantage Point");
            Assert.AreEqual(fixture.CurrMatch.CurrentGame().GameStatus.Player.FirstName, fixture.CurrMatch.PlayerTwo.FirstName, "Game Status should be Advantage for PlayerTwo");
        }

        [TestMethod]
        public void ShouldSetGameStatusAsGamePointAt4040SuddenDeathDeuce()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SelectedDeuceFormat = DeuceFormat.SuddenDeath;
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            //Act  --> Score is 50-50
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            //Assert --> at Deuce for Sudden Death Deuce format it should be Game Point for any player
            Assert.AreEqual(fixture.CurrMatch.CurrentGame().GameStatus.Status, Status.GamePoint, "Should be Game Point");

            //Act --> A point to any player should be Game  
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            //Assert
            Assert.IsNotNull(fixture.CurrMatch.GetGameForKnownSetAndGame(1,1).Winner, "Game Winner should not be Null");
            Assert.IsTrue(fixture.CurrMatch.GetGameForKnownSetAndGame(1,1).GameStatus.Status == Status.GameOver, "Game Status should be flagged as Game");
            Assert.AreEqual(fixture.CurrMatch.GetGameForKnownSetAndGame(1,1).GameStatus.Player.FirstName, fixture.CurrMatch.PlayerTwo.FirstName, "Game Status should be Game for PlayerTwo");
            Assert.AreEqual(fixture.CurrMatch.GetGameForKnownSetAndGame(1,1).Winner.FirstName, fixture.CurrMatch.PlayerTwo.FirstName, "Game Winner Name should be that of PlayerTwo ");
        }

        [TestMethod]
        public void ShouldSetIsCurrentGameStatusToFalseAndInitializeNewGameWhenIfAt3050Game()
        {

            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            //Act  --> Score is 30-50
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            //Assert
            Assert.IsNull(fixture.CurrMatch.CurrentGame().Winner, "Game Winner should be Null");
            Assert.IsTrue(fixture.CurrMatch.CurrentGame().GameStatus.Status == Status.Neutral, "Game Status should be flagged be Neutral, it is a new Game");
            Assert.IsNotNull(fixture.CurrMatch.GetGameForKnownSetAndGame(1, 2), "The second game should be initialized after the first game is over");
            Assert.IsTrue(fixture.CurrMatch.GetGameForKnownSetAndGame(1, 2).IsCurrentGame, "The Second game should be the current game");
        }

        [TestMethod]
        public void ShouldStartNewSetWhenASetIsWon()
        {

            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            //Act  --> PlayerOne wins 4 games to 1
            AddAGameForPlayerOne(fixture);
            //1-1
            AddAGameForPlayerTwo(fixture);

            //2-1
            AddAGameForPlayerOne(fixture);

            //3-1
            AddAGameForPlayerOne(fixture);

            //Set wont 4-1 to player one
            AddAGameForPlayerOne(fixture);

            //Assert
            Assert.IsNotNull(fixture.CurrMatch.Score.Sets.First().Winner, "Set Winner should not be Null");
            Assert.AreEqual(fixture.CurrMatch.Score.Sets.First().Winner.FullName, fixture.CurrMatch.PlayerOne.FullName, "The Second game should be the current game");
        }

        [TestMethod]
        public void ShouldStartNewSetWhenASetIsWonForLongSetMatch()
        {

            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SelectedSetsFormat = SetsFormat.LongSetSix;
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            //Act  --> PlayerOne wins 4 games to 1
            AddAGameForPlayerOne(fixture);
            //1-1
            AddAGameForPlayerTwo(fixture);

            //2-1
            AddAGameForPlayerOne(fixture);

            //3-1
            AddAGameForPlayerOne(fixture);

            //4-1
            AddAGameForPlayerOne(fixture);

            //5-1
            AddAGameForPlayerOne(fixture);

            //6-1
            AddAGameForPlayerOne(fixture);

            //Assert
            Assert.IsNotNull(fixture.CurrMatch.Score.Sets.First().Winner, "Set Winner should not be Null");
            Assert.AreEqual(fixture.CurrMatch.Score.Sets.First().Winner.FullName, fixture.CurrMatch.PlayerOne.FullName, "The Second game should be the current game");
        }

        [TestMethod]
        public void ShouldStartTieBreakerifGamesAreEven()
        {

            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            AddAGameForPlayerOne(fixture);
            AddAGameForPlayerTwo(fixture);

            AddAGameForPlayerOne(fixture);
            AddAGameForPlayerTwo(fixture);

            AddAGameForPlayerOne(fixture);
            AddAGameForPlayerTwo(fixture);

            AddAGameForPlayerOne(fixture);
            AddAGameForPlayerTwo(fixture);

            Assert.IsNull(fixture.CurrMatch.Score.Sets.First().Winner, "Sets winner should be null");
            Assert.AreEqual(GameType.SevenPointer, fixture.CurrMatch.CurrentGame().GameType,"Game type should be a Seven Point Tiebreaker");

        }

        [TestMethod]
        public void ShouldScoreASevenPointTieBreakToSeven()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            AddAGameForPlayerOne(fixture);
            AddAGameForPlayerTwo(fixture);

            AddAGameForPlayerOne(fixture);
            AddAGameForPlayerTwo(fixture);

            AddAGameForPlayerOne(fixture);
            AddAGameForPlayerTwo(fixture);

            AddAGameForPlayerOne(fixture);
            AddAGameForPlayerTwo(fixture);

            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.AreEqual(1, fixture.CurrMatch.Score.Sets.Count, "Should still be on the first set");
            Assert.IsNull(fixture.CurrMatch.Score.Sets.First().Winner, "Sets winner should be null at 5-2 in a Seven Point Tie Breaker game");
            Assert.IsNotNull(fixture.CurrMatch.CurrentGame(), "The current game should be a Tie Breaker game ");
            Assert.AreEqual(5, fixture.CurrMatch.CurrentGame().PlayerOneScore, "Player one score should be 5");
            Assert.IsNull(fixture.CurrMatch.CurrentGame().Winner, "The current game should not have a winner");
        }

        [TestMethod]
        public void ShouldScoreASevenPointTieBreakToSevenAndSetWinnerWhenAnyPlayerGetsToSeven()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            AddAGameForPlayerOne(fixture);
            AddAGameForPlayerTwo(fixture);

            AddAGameForPlayerOne(fixture);
            AddAGameForPlayerTwo(fixture);

            AddAGameForPlayerOne(fixture);
            AddAGameForPlayerTwo(fixture);

            AddAGameForPlayerOne(fixture);
            AddAGameForPlayerTwo(fixture);

            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.IsNotNull(fixture.CurrMatch.Score.Sets.First().Winner, "Sets winner should not be null at 7-2 in a Seven Point Tie Breaker game");
            Assert.AreEqual(2, fixture.CurrMatch.Score.Sets.Count, "Should still be on the first set");
        }

        [TestMethod]
        public void ShouldScoreThirdSetAsSinglePointersIfFinalSetFormatIsChampionShipTieBreak()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SelectedFinalSet = FinalSetFormats.TenPointChampionShipTieBreak;
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            AddASetForPlayer(fixture,true);
            AddASetForPlayer(fixture,false);

            //Score is 1-1 in sets
            Assert.IsTrue(fixture.CurrMatch.Score.Sets.Count == 3);
            Assert.IsTrue(fixture.CurrMatch.CurrentGame().GameType == GameType.TenPointer, "");
        }

        [TestMethod]
        public void ShouldScoreADecidingTieBreakerAccordingly()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SelectedFinalSet = FinalSetFormats.TenPointChampionShipTieBreak;
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            AddASetForPlayer(fixture, true);
            AddASetForPlayer(fixture, false);

            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.IsTrue(fixture.CurrMatch.CurrentSet().Games.Count == 1, "The TieBreaker set should only have one game");
            Assert.IsTrue(fixture.CurrMatch.CurrentSet().Games.First().PlayerTwoScore == 6);
            Assert.IsTrue(fixture.CurrMatch.CurrentSet().Games.First().PlayerOneScore == 2);
        }

        [TestMethod]
        public void ShouldScoreEndADecidingTieBreakerAfterSetPoints()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SelectedFinalSet = FinalSetFormats.TenPointChampionShipTieBreak;
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            AddASetForPlayer(fixture, true);
            AddASetForPlayer(fixture, false);

            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.IsTrue(fixture.CurrMatch.CurrentSet().Games.Count == 1,  string.Format("The TieBreaker set should only have 1 game, but has {0}",fixture.CurrMatch.CurrentSet().Games.Count));
            Assert.IsTrue(fixture.CurrMatch.CurrentSet().Games.First().PlayerTwoScore == 10);
            Assert.IsTrue(fixture.CurrMatch.CurrentSet().Games.First().PlayerOneScore == 3);
            Assert.IsTrue(fixture.CurrMatch.Score.IsMatchOver, "Game should be flagged as over");
            Assert.IsNotNull(fixture.CurrMatch.Score.Winner, "Winning player should not be null");
        }

        [TestMethod]
        public void ShouldScoreADecidingTieBreakerBeyoundFourPoints()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SelectedDeuceFormat = DeuceFormat.SuddenDeath;
            fixture.NewMatchControlViewModel.SelectedFinalSet = FinalSetFormats.TenPointChampionShipTieBreak;
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            AddASetForPlayer(fixture, true);
            AddASetForPlayer(fixture, false);

            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.AreEqual(fixture.CurrMatch.CurrentGame().PlayerTwoScore, 5, string.Format("Game score should be 4 but it is {0}", fixture.CurrMatch.CurrentGame().PlayerTwoScore));
        }

        [TestMethod]
        public void ShouldSetAMatchIsOverWhenAPlayerOneWinsBy2()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SelectedFinalSet = FinalSetFormats.TenPointChampionShipTieBreak;
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            AddASetForPlayer(fixture, true);
            AddASetForPlayer(fixture, true);

            Assert.IsTrue(fixture.CurrMatch.Score.IsMatchOver,"MatchOver flag should be true");
            Assert.IsNotNull(fixture.CurrMatch.Score.Winner,"Winner should not be null");
            Assert.IsTrue(fixture.CurrMatch.Score.Winner.IsPlayerOne, "Match winner should be player one but it is not");
        }

        [TestMethod]
        public void ShouldChangeServerAfterFirstServeOfTieBreaker()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SelectedFinalSet = FinalSetFormats.TenPointChampionShipTieBreak;
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            AddAGameForPlayerOne(fixture);
            AddAGameForPlayerOne(fixture);
            AddAGameForPlayerOne(fixture);
            AddAGameForPlayerTwo(fixture);
            AddAGameForPlayerTwo(fixture);
            AddAGameForPlayerTwo(fixture);
            AddAGameForPlayerTwo(fixture);
            AddAGameForPlayerOne(fixture);

            bool currentIsPlayerOne = fixture.CurrentServer.IsPlayerOne;
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            Assert.AreNotEqual(fixture.CurrentServer.IsPlayerOne, currentIsPlayerOne); 
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            currentIsPlayerOne = fixture.CurrentServer.IsPlayerOne;
            Assert.AreEqual(fixture.CurrentServer.IsPlayerOne, currentIsPlayerOne); 
        }

        [TestMethod]
        public void ShouldChangeServerAfterFirstServeOfChampionShipTieBreake()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SelectedFinalSet = FinalSetFormats.TenPointChampionShipTieBreak;
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            AddASetForPlayer(fixture, false);
            AddASetForPlayer(fixture, true);

            bool currentIsPlayerOne = fixture.CurrentServer.IsPlayerOne;
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            Assert.AreNotEqual(fixture.CurrentServer.IsPlayerOne, currentIsPlayerOne);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            currentIsPlayerOne = fixture.CurrentServer.IsPlayerOne;
            Assert.AreEqual(fixture.CurrentServer.IsPlayerOne, currentIsPlayerOne);
        }

        [TestMethod]
        public void ShouldSetAMatchIsOverWhenAPlayerTwoWinsBy2()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SelectedFinalSet = FinalSetFormats.TenPointChampionShipTieBreak;
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            AddASetForPlayer(fixture, false);
            AddASetForPlayer(fixture, false);

            Assert.IsTrue(fixture.CurrMatch.Score.IsMatchOver, "MatchOver flag should be true");
            Assert.IsNotNull(fixture.CurrMatch.Score.Winner, "Winner should not be null");
            Assert.IsFalse(fixture.CurrMatch.Score.Winner.IsPlayerOne, "Match winner should be player two but it is not");
        }

        [TestMethod]
        public void ShouldSwitchCurrentServerAtTheEndOfAGame()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SelectedFinalSet = FinalSetFormats.TenPointChampionShipTieBreak;
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            AddAGameForPlayerOne(fixture);

            Assert.AreEqual(fixture.CurrentServer.FullName, fixture.CurrMatch.PlayerOne.FullName, string.Format("The current server should be {0} not {1}", fixture.CurrMatch.PlayerOne.FullName, fixture.CurrMatch.PlayerTwo.FullName));
            Assert.IsTrue(fixture.PlayerOneIsServing);
        }

        [TestMethod]
        public void ShouldStoreDoubleFaultActionAsInMatchStatsList()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SelectedFinalSet = FinalSetFormats.TenPointChampionShipTieBreak;
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);
            
            Assert.IsNotNull(fixture.CurrMatch.MatchStats.FirstOrDefault(x => x.Reason == StatDescription.DoubleFault), "Match stats should have one element in it");
        }

        [TestMethod]
        public void ShouldBeAbleToStoreAndRetrieveActions()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SelectedFinalSet = FinalSetFormats.TenPointChampionShipTieBreak;
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.IsNotNull(fixture.CurrMatch.MatchStats.FirstOrDefault(x => x.Reason == StatDescription.DoubleFault), "Match stats should have one element in it");
            Assert.AreEqual(2, fixture.CurrMatch.MatchStats.Count, "There should be two elements in the MatchStats list");
        }
 
        [TestMethod]
        public void MatchStatShouldStoreGamePointStatWhenAtGamePoint()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SelectedFinalSet = FinalSetFormats.TenPointChampionShipTieBreak;
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.IsNotNull(fixture.CurrMatch.MatchStats.FirstOrDefault(x => x.Reason == StatDescription.DoubleFault), "Match stats should have one element in it");
            Assert.AreEqual(5, fixture.CurrMatch.MatchStats.Count, "There should be two elements in the MatchStats list");
            Assert.IsNotNull(fixture.CurrMatch.MatchStats.Last().Reason == StatDescription.GamePoint);
        }

        [TestMethod]
        public void MatchStatShouldStoreBreakPointStatWhenAtBreakPoint()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SelectedFinalSet = FinalSetFormats.TenPointChampionShipTieBreak;
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.IsNotNull(fixture.CurrMatch.MatchStats.FirstOrDefault(x => x.Reason == StatDescription.DoubleFault), "Match stats should have one element in it");
            Assert.AreEqual(5, fixture.CurrMatch.MatchStats.Count, "There should be two elements in the MatchStats list");
            Assert.IsNotNull(fixture.CurrMatch.MatchStats.Last().Reason == StatDescription.BreakPoint);
        }

        [TestMethod]
        public void ShouldRecordServeInStatsAgainstPlayerServing()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);

            Assert.IsNotNull(fixture.CurrMatch.MatchStats.FirstOrDefault(), "Match stats should have one element in it");
            Assert.IsTrue(fixture.CurrMatch.MatchStats.First().Reason == StatDescription.FirstServeIn);
            Assert.AreEqual(fixture.CurrMatch.MatchStats.First().Player.FullName, fixture.CurrMatch.Score.CurrentServer.FullName);
            Assert.AreEqual(fixture.CurrMatch.MatchStats.First().Server.FullName, fixture.CurrMatch.Score.CurrentServer.FullName);
            Assert.AreEqual(fixture.CurrMatch.MatchStats.First().PointWonLostOrNone, PointWonLostOrNone.NotAPoint);
        }

        [TestMethod]
        public void FirstServeInCommandShouldBeExcutableAfterAPointHasBeenScored()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            fixture.PlayerTwoFirstServeInCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.IsTrue(fixture.PlayerTwoFirstServeInCommand.CanExecute(null));
        }

        [TestMethod]
        public void FirstServeInCommandShouldNotBeExcutableJustAfterAServe()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);

            Assert.IsFalse(fixture.PlayerOneFirstServeInCommand.CanExecute(null));
        }
  
        [TestMethod]
        public void SecondServeInCommandShouldNotBeExcutableByDefault()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            Assert.IsFalse(fixture.PlayerOneSecondServeInCommand.CanExecute(null));
        }

        [TestMethod]
        public void SecondServeInCommandShouldBeBeExcutableIfLastActionIsFirstServe()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeOutCommand.Execute(null);

            Assert.IsTrue(fixture.PlayerOneSecondServeInCommand.CanExecute(null));
        }

        [TestMethod]
        public void FirstServeInAndOutCommandsShouldNotBeExecutableIfCurrentServerIsNotSet()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);

            Assert.IsFalse(fixture.PlayerOneFirstServeInCommand.CanExecute(null));
            Assert.IsFalse(fixture.PlayerOneFirstServeOutCommand.CanExecute(null));
        }


        [TestMethod]
        public void AtGamePointTheFirstServeInAndOutCommandsShouldBeExecutable()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.IsTrue(fixture.PlayerOneFirstServeInCommand.CanExecute(null));
        }

        [TestMethod]
        public void FirstServeInCommandForPlayerTwoShouldBeExcutableAfterAPointHasBeenScoredAndPlayerTwoIsServing()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            fixture.PlayerTwoSecondServeInCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.IsTrue(fixture.PlayerTwoFirstServeInCommand.CanExecute(null));
        }


        [TestMethod]
        public void FirstServeInAndOutForPlayerTwoShouldNotBeExecutableIfCurrentServerIsPlayerOne()
        {
            var fixture = CreateNewMatchFixture();

            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            Assert.IsFalse(fixture.PlayerTwoSecondServeInCommand.CanExecute(null));
            Assert.IsFalse(fixture.PlayerTwoFirstServeInCommand.CanExecute(null));
            Assert.IsFalse(fixture.PlayerTwoFirsrtServeOutCommand.CanExecute(null));
        }

        [TestMethod]
        public void FirstServeInForPlayerTwoShouldAddMatchStatForPlayerTwo()
        {
            var fixture = CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerTwoFirstServeInCommand.Execute(null);
            Assert.IsNotNull(fixture.MatchStatus.FirstOrDefault());
        }

        [TestMethod]
        public void FirstServeOutForPlayerTwoShouldMakeSecondServeExecutable()
        {
            var fixture = CreateNewMatchFixture();
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            fixture.PlayerTwoFirsrtServeOutCommand.Execute(null);

            Assert.IsTrue(fixture.PlayerTwoSecondServeInCommand.CanExecute(null));
        }

        [TestMethod]
        public void ShouldDisablePlayerOneDoubleFaultCommandWhenCurrentServerIsPlayerTwo()
        {
            var fixture = CreateNewMatchFixture();

            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            Assert.IsFalse(fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").IsEnabled, "Double fault action for player one should be false when player two is the current server");
        }

        [TestMethod]
        public void ShouldDisablePlayerTwoDoubleFaultCommandWhenCurrentServerIsPlayerOne()
        {
            var fixture = CreateNewMatchFixture();

            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            Assert.IsFalse(fixture.PlayerTwoActions.First(x => x.Name == "DoubleFault").IsEnabled, "Double fault action for player two should be false when player one is the current server");
        }

        [TestMethod] 
        public void ShouldDisablePlayerTwoCommandOnStartUp()
        {
            var fixture = CreateNewMatchFixture();
            Assert.IsFalse(fixture.PlayerOneActions.Any(x => x.IsEnabled), "All Commands for Player Two should be disabled on start up");
            Assert.IsFalse(fixture.PlayerTwoActions.Any(x => x.IsEnabled), "All Commands for Player Two should be disabled on start up");
        }

        [TestMethod]
        public void ShouldDisablePlayerOneDoubleFaultActionImmediatelyAfterPlayerOneHasDoubleFaulted()
        {
            var fixture = CreateNewMatchFixture();

            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);
            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);

            Assert.IsFalse(fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").IsEnabled, "Double fault Command for Player One should be disabled immediately after it has been called");
        }

        [TestMethod]
        public void ShouldDisablePlayerTwoDoubleFaultActionImmediatelyAfterPlayerTwoHasDoubleFaulted()
        {
            var fixture = CreateNewMatchFixture();

            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);
            fixture.PlayerTwoFirsrtServeOutCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);

            Assert.IsFalse(fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").IsEnabled, "Double fault Command for Player One should be disabled immediately after it has been called");
            
        }

        [TestMethod]
        public void ShouldDisableAllActionsForPlayerOneAfterAPointIsScoredTillAfterAServeIsIn()
        {
            var fixture = CreateNewMatchFixture();
            
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);
            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.IsFalse(fixture.PlayerOneActions.Any(x => x.Name != "DoubleFault" && x.IsEnabled), "Actions for Player one should be disabled till after a successful server by the current server");
        }

        [TestMethod]
        public void ShoudDisableAllActionsForPlayerTwoAfterAPointIsScoredTillAfterAServeIsIn()
        {
            var fixture = CreateNewMatchFixture();

            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);
            fixture.PlayerTwoFirstServeInCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.IsFalse(fixture.PlayerTwoActions.Any(x => x.Name != "DoubleFault" && x.IsEnabled), "Actions for Player two should be disabled till after a successful server by the current server");
        }

        [TestMethod]
        public void ShouldEnableActionCommandsExceptDoubleFaultForBothPlayersOnFirstServeIn()
        {
            var fixture = CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);

            Assert.IsFalse(fixture.PlayerOneActions.Any(x => x.Name != "DoubleFault" && x.IsEnabled == false));
        }

        [TestMethod]
        public void ShouldEnableDisableDoubleFaultCommandWhenSecondServeIsIn()
        {
            var fixture = CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeOutCommand.Execute(null);
            fixture.PlayerOneSecondServeInCommand.Execute(null);

            Assert.IsFalse(fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").IsEnabled, "Player One Double Fault action should be disabled after a second serve has been successful");
            Assert.IsFalse(fixture.PlayerTwoActions.First(x => x.Name == "DoubleFault").IsEnabled, "Player Two Double Fault action should be disable after a second serve has been successful");
        }

        private static MatchScoreViewModel CreateNewMatchFixture()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            return fixture;
        }

        private void AddASetForPlayer(MatchScoreViewModel fixture, bool IsPlayerOne)
        {
            var sets = (int)fixture.CurrMatch.MatchFormat.SetsFormat;
            for (int i = 0; i < sets; i++)
            {
                if (IsPlayerOne)
                {
                    AddAGameForPlayerOne(fixture);
                }
                else
                {
                    AddAGameForPlayerTwo(fixture);
                }

            }
        }

        private void AddAGameForPlayerOne(MatchScoreViewModel fixture)
        {
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
        }        

        private void AddAGameForPlayerTwo(MatchScoreViewModel fixture)
        {
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
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
            fixture.NewMatchControlViewModel.SelectedDeuceFormat = DeuceFormat.Normal;
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
                DeuceFormat = DeuceFormat.SuddenDeath,
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
