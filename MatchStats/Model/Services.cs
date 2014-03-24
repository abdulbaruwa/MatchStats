using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using Windows.UI.Xaml.Media.Imaging;
using Akavache;
using MatchStats.Enums;
using ReactiveUI;

namespace MatchStats.Model
{
    public interface IImagesApi
    {
        Task<string> BrowseImage();
        Task<StorageItemThumbnail> BrowseImageThumbnail();
        void SaveDefaultPlayerImage(StorageItemThumbnail imageThumbnail);
    }


    public class ImagesApi : IImagesApi
    {
        public async Task<string> BrowseImage()
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.PicturesView);
                return file.Path;
            }
            return string.Empty;
        }

        public async Task<StorageItemThumbnail> BrowseImageThumbnail()
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var filestream = await FileIO.ReadBufferAsync(file);
                var newfile = await ApplicationData.Current.LocalFolder.CreateFileAsync("DefaultPlayerImage.bmp", CreationCollisionOption.ReplaceExisting);
                using (DataReader dataReader = DataReader.FromBuffer(filestream))
                {
                    var buffer = new byte[dataReader.UnconsumedBufferLength];
                    dataReader.ReadBytes(buffer);
                    await FileIO.WriteBytesAsync(newfile, buffer);    
                }
                var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.PicturesView);
                return thumbnail;
            }

            return null;
        }

        public void SaveDefaultPlayerImage(StorageItemThumbnail imageThumbnail)
        {
            var blobCache = RxApp.MutableResolver.GetService<IBlobCache>("UserAccount");
            blobCache.InsertObject("DefaultPlayerImage", imageThumbnail);
        }
    }

    public class FakeImagesApi : IImagesApi
    {
        public async Task<string> BrowseImage()
        {
            //return "ms-appx:///Assets/male_silhouette.png";
            return "C:\\Users\\abdul_000\\Pictures\\AdemolasSweden 080.jpg";
            //return "Assets/AdemolasSweden 080.jpg";
        }

        public Task<StorageItemThumbnail> BrowseImageThumbnail()
        {
            throw new NotImplementedException();
        }

        public void SaveDefaultPlayerImage(StorageItemThumbnail imageThumbnail)
        {
            throw new NotImplementedException();
        }
    }

    public interface IMatchStatsApi
    {
        void SaveMatch(Match match);
        IObservable<List<Match>> FetchMatchStats();
        IObservable<Match> ExecuteActionCommand(ICommand command);
        IObservable<Match> GetCurrentMatch();
        Match ApplyGameRules(Match currentMatch);
        void SaveMatchStats(List<Match> matchStats);
    }

    public class MatchStatsApi : IMatchStatsApi
    {
        private readonly IBlobCache _blobCache;
        private readonly ILogger _logger;
        public MatchStatsApi(IBlobCache blocCache = null)
        {
            _blobCache = blocCache ?? RxApp.DependencyResolver.GetService<IBlobCache>("UserAccount");
            _logger = RxApp.DependencyResolver.GetService<ILogger>();
        }

        public void SaveMatchStats(List<Match> matchStats)
        {
            _blobCache.InsertObject("MyMatchStats", matchStats);
        }

        public async void SaveMatch(Match match)
        {
            var existingMatches = new List<Match>();
            IEnumerable<Match> matchesFromStorage = null;
            try
            {
               matchesFromStorage = await _blobCache.GetObjectAsync<List<Match>>("MyMatchStats");
            }
            catch (KeyNotFoundException e)
            {
                _logger.Write("Object not found in Cache", LogLevel.Error);
                _logger.Write(e.ToString(), LogLevel.Error);
            }

            if (matchesFromStorage != null)existingMatches.AddRange(matchesFromStorage); 
            var existingMatch = existingMatches.FirstOrDefault(x => x.MatchGuid == match.MatchGuid);
            if (existingMatch != null)
            {
                existingMatches.Remove(existingMatch);
            }

            existingMatches.Add(match);
            var newVals = new List<Match>(existingMatches);

            try
            {
                await _blobCache.InsertObject("MyMatchStats", newVals).Concat(_blobCache.InsertObject("CurrentMatch", match));
            }
            catch (Exception e)
            {
                //Log and continue - likely exception may be a sqllite lock.
                _logger.Write("Error saving Match info", LogLevel.Error);
                _logger.Write(e.ToString(), LogLevel.Error);
            }
        }

        public IObservable<List<Match>> FetchMatchStats()
        {
            IObservable<List<Match>> observableRes = _blobCache.GetObjectAsync<List<Match>>("MyMatchStats");
              return observableRes;
        }

        public IObservable<Match> ExecuteActionCommand(ICommand command)
        {
            throw new NotImplementedException();
        }

        public IObservable<Match> GetCurrentMatch()
        {
            IObservable<Match> currentMatchObservable = _blobCache.GetObjectAsync<Match>("CurrentMatch");
            return currentMatchObservable;
        }

        public Match ApplyGameRules(Match currentMatch)
        {
            //currentMatch
            //Is current game over?
            //Func<Game, bool> 
            var currentSet = currentMatch.Sets.FirstOrDefault(x => x.IsCurrentSet);
            if (currentSet == null) return currentMatch;

            Game currentGame = currentSet.Games.FirstOrDefault(x => x.IsCurrentGame);
            if (currentGame == null) return currentMatch;

            //PlayerTwo Breakpoint or GamePoint
            CheckBreakPointForPlayerOneRule(currentMatch, ref currentGame);

            //Player One BreakPoint or GamePoint
            CheckBreakPointForPlayerTwoRule(currentMatch, ref currentGame);

            //Advantage
            CheckAdvantageRule(currentMatch, ref currentGame);
            
            //Game over rules
            CheckGameOverRule(currentMatch, ref currentGame);

            //Game over Short Tie Breaker to seven
            CheckTieBreakerGameOverRule(currentMatch, ref currentGame);

            //Deuce
            CheckDeuceRule(currentMatch, ref currentGame);

            //Deuce Sudden Death
            CheckGamePointOnSuddenDeathDeuceRule(currentMatch, ref currentGame);

            //Switch Server on ChampionShip TieBreak
            CheckAndSwitchCurrentServerOnChampionShipTieBreakPoints(currentMatch, ref currentGame);

            //Is game over 
            CheckGameIsOverAndInitializeNewGameIfNeedBe(currentMatch);

            CheckIsMatchPoint(currentMatch);

            //Is Set Over
            CheckSetIsOverAndInitializeNewSetIfNeedBe(currentMatch);

            //Update MatchSituation with latest valid MatchSituation we can find one.
            var latestMatchSituation = currentMatch.MatchStats.LastOrDefault(x => x.MatchSituations.Count > 0);
            if (latestMatchSituation != null && latestMatchSituation.MatchSituations.Count > 0)
            {
                currentGame.LastMatchSituation = currentMatch.MatchStats.Last(x => x.MatchSituations.Count > 0).MatchSituations.Last();
                if (currentGame.LastMatchSituation != null ) currentGame.Points.Last().MatchSituationAfter = currentGame.LastMatchSituation;
            }

            return currentMatch;
        }

        private void CheckAndSwitchCurrentServerOnChampionShipTieBreakPoints(Match currentMatch, ref Game currentGame)
        {
            if (currentMatch.MatchFormat.FinalSetType == FinalSetFormats.TenPointChampionShipTieBreak 
                && currentGame.GameType == GameType.TenPointer )
            {
                //We should switch if the total score is an odd number
                if ((currentGame.PlayerOneScore + currentGame.PlayerTwoScore)%2 == 1)
                {
                    SwitchCurrentServer(currentMatch);
                }
            }
        }

        private bool CheckIsMatchPoint(Match currentMatch)
        {
            var gamesCount = (int)currentMatch.MatchFormat.SetsFormat;
            if (currentMatch.Sets.Any())
            {
                var groupedSetWiners = (from n in currentMatch.Sets
                    where n.Winner != null
                    group n by n.Winner.IsPlayerOne
                    into setWinners
                    select new {WinnerIsPlayerOne = setWinners.Key, setCount = setWinners.Count()}).ToList();

                var currentSetGames = currentMatch.CurrentSet().Games;
                if (currentSetGames.Any())
                {
                    var groupedWinner = (from n in currentSetGames
                        where n.Winner != null
                        group n by n.Winner.IsPlayerOne
                        into winloss
                        select new {WinnerIsPlayerOne = winloss.Key, gameCount = winloss.Count()}).ToList();

                    //Get games won by each player
                    var playerOneGamesCount = 0;
                    if (groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne) != null)
                        playerOneGamesCount = groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne).gameCount;
                    var playerTwoGamesCount = 0;
                    if (groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne == false) != null)
                        playerTwoGamesCount = groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne == false).gameCount;

                    var currentGame = currentMatch.CurrentGame();
                    if (groupedSetWiners.Count(x => x.setCount == 1) == 2)
                    {
                        //Both players have won a set
                        //Check in the current set any player is one game away and is on set point
                        if (currentMatch.MatchFormat.FinalSetType == FinalSetFormats.TenPointChampionShipTieBreak)
                        {
                            var championShipTieBreakerPoints = (int)currentMatch.CurrentGame().GameType;
                            //Note it could be 9-9, in which case we add MatchPointSituation for both players.
                            if (currentGame.PlayerOneScore == championShipTieBreakerPoints - 1)
                            {
                                currentMatch.MatchStats.Last().MatchSituations.Add(new MatchSituation()
                                {
                                    GameId = currentGame.GameId,
                                    Id = Guid.NewGuid().ToString(),
                                    MatchSituationType = MatchSituationType.MatchPoint,
                                    Player = currentMatch.PlayerOne,
                                    SetId = currentMatch.CurrentSet().SetId
                                });
                            }
                            if (currentGame.PlayerTwoScore == championShipTieBreakerPoints - 1)
                            {
                                currentMatch.MatchStats.Last().MatchSituations.Add(new MatchSituation()
                                {
                                    GameId = currentGame.GameId,
                                    Id = Guid.NewGuid().ToString(),
                                    MatchSituationType = MatchSituationType.MatchPoint,
                                    Player = currentMatch.PlayerTwo,
                                    SetId = currentMatch.CurrentSet().SetId
                                });
                            }
                        }
                        else if (playerOneGamesCount >= gamesCount - 1 || playerTwoGamesCount >= gamesCount - 1)
                        {
                            if (playerOneGamesCount.DiffValueWith(playerTwoGamesCount) >= 1) //Lead by a game and on poossibly the last game
                            {
                                var leaderIsPlayerOne = playerOneGamesCount > playerTwoGamesCount;
                                var lastSituation = currentMatch.MatchStats.Last().MatchSituations.LastOrDefault();
                                if(lastSituation != null)
                                {
                                    if ((lastSituation.MatchSituationType == MatchSituationType.GamePoint || lastSituation.MatchSituationType == MatchSituationType.BreakPoint)
                                                && lastSituation.Player.IsPlayerOne == leaderIsPlayerOne)
                                    {
                                        currentMatch.MatchStats.Last().MatchSituations.Add(new MatchSituation()
                                        {
                                            GameId = currentGame.GameId,
                                            Id = Guid.NewGuid().ToString(),
                                            MatchSituationType = MatchSituationType.MatchPoint,
                                            Player = leaderIsPlayerOne ? currentMatch.PlayerOne : currentMatch.PlayerTwo,
                                            SetId = currentMatch.CurrentSet().SetId
                                        });
                                    }
                                }

                            }
                        }
                    }
                    else if (groupedSetWiners.FirstOrDefault(x => x.setCount == 1) != null)
                    {
                        //One player has won a set; 
                        //Check in the current set if the player is one game away and is on set point
                        var setWinner = groupedSetWiners.FirstOrDefault(x => x.setCount == 1).WinnerIsPlayerOne;
                        //var setWinner.WinnerIsPlayerOne
                        if (playerOneGamesCount >= gamesCount - 1 || playerTwoGamesCount >= gamesCount - 1)
                        {
                            if (playerOneGamesCount.DiffValueWith(playerTwoGamesCount) >= 1) //Lead by a game and on poossibly the last game
                            {
                                var lastSituation = currentMatch.MatchStats.Last().MatchSituations.LastOrDefault();
                                if (lastSituation != null)
                                {
                                    //TODO Abdul here
                                    if ((lastSituation.MatchSituationType == MatchSituationType.GamePoint && lastSituation.Player.IsPlayerOne == setWinner)
                                        || (lastSituation.MatchSituationType == MatchSituationType.BreakPoint && currentMatch.CurrentServer.IsPlayerOne != setWinner))
                                    {
                                        currentMatch.MatchStats.Last().MatchSituations.Add(new MatchSituation()
                                        {
                                            GameId = currentGame.GameId,
                                            Id = Guid.NewGuid().ToString(),
                                            MatchSituationType = MatchSituationType.MatchPoint,
                                            Player = setWinner ? currentMatch.PlayerOne : currentMatch.PlayerTwo,
                                            SetId = currentMatch.CurrentSet().SetId
                                        });
                                    }
                                }

                            }
                        }

                    }
                }

        }

            //  TODO : 
            return false;
        }

        private bool CheckMatchIsOverRule(Match currentMatch)
        {
            if (currentMatch.Sets.Any(x => x.Winner == null)) return false;
            if (currentMatch.Sets.Any())
            {
                var groupedSetWiners = (from n in currentMatch.Sets
                    group n by n.Winner.IsPlayerOne
                    into setWinners
                    select new {WinnerIsPlayerOne = setWinners.Key, setCount = setWinners.Count()}).ToList();

                var winner = groupedSetWiners.FirstOrDefault(x => x.setCount >= 2);
                if (winner != null)
                {
                    currentMatch.Winner = winner.WinnerIsPlayerOne ? currentMatch.PlayerOne : currentMatch.PlayerTwo;
                    currentMatch.IsMatchOver = true;
                    currentMatch.MatchStats.Last().MatchSituations.Add(new MatchSituation()
                    {
                        Id = Guid.NewGuid().ToString(),
                        GameId = currentMatch.Sets.Last().Games.Last().GameId,
                        Player = currentMatch.Winner,
                        SetId = currentMatch.Sets.Last().SetId,
                        MatchSituationType = MatchSituationType.MatchPointWon
                    });
                }
            }
            return false;
        }

        private bool CheckSetIsOverAndInitializeNewSetIfNeedBe(Match currentMatch)
        {
            if (currentMatch.IsMatchOver) return true;
            var gamesCount = (int)currentMatch.MatchFormat.SetsFormat;

            var currentSetGames = currentMatch.CurrentSet().Games;
            if (currentSetGames.Any(x => x.Winner == null)) return false;
            if (currentSetGames.Any())
            {
                var groupedWinner = (from n in currentSetGames
                    group n by n.Winner.IsPlayerOne
                    into winloss
                    select new {WinnerIsPlayerOne = winloss.Key, gameCount = winloss.Count()}).ToList();

                //Get games won by each player
                var playerOneGamesCount = 0;
                    if(groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne) != null) 
                        playerOneGamesCount = groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne).gameCount;
                var playerTwoGamesCount = 0;
                    if(groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne == false) != null)
                        playerTwoGamesCount =groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne == false).gameCount;

                //Has any player reached 4 or 6
                if (playerOneGamesCount >= gamesCount - 1 || playerTwoGamesCount >= gamesCount - 1)
                {
                    var setsToPlay = ((int)currentMatch.MatchFormat.SetsFormat);

                    //Has that player won the set
                    if (playerOneGamesCount.DiffValueWith(playerTwoGamesCount) >= 2)
                    {
                        var playerOneIsWinner = playerOneGamesCount > playerTwoGamesCount;
                        currentMatch.CurrentSet().SetWonBy(playerOneIsWinner
                            ? currentMatch.PlayerOne
                            : currentMatch.PlayerTwo);
                        currentMatch.CurrentSet().IsCurrentSet = false;

                        if (CheckMatchIsOverRule(currentMatch)) return true;
                        currentMatch.Sets.Add(new Set() {IsCurrentSet = true});

                        if (setsToPlay.DiffValueWith(currentMatch.Sets.Count) == 1 && currentMatch.MatchFormat.FinalSetType == FinalSetFormats.TenPointChampionShipTieBreak)
                        {
                            //This is the final set and it is a Tiebreaker                        
                            currentMatch.CurrentSet().Games.Add(new Game() { IsCurrentGame = true, GameType = GameType.TenPointer });
                            SwitchCurrentServer(currentMatch);
                        }
                        else
                        {
                            AddNewGameToCurrentSetAndSwitchCurrentServer(currentMatch);
                        }
        
                        return true;
                    }
                    // 4-5(5-4) or 6-7(7-6) --> Code is repetitive but simpler to read
                    if (playerOneGamesCount.DiffValueWith(playerTwoGamesCount) == 1 
                        && playerOneGamesCount >= gamesCount
                        && playerTwoGamesCount >= gamesCount)
                    {
                        var playerOneIsWinner = playerOneGamesCount > playerTwoGamesCount;
                        currentMatch.CurrentSet().SetWonBy(playerOneIsWinner
                            ? currentMatch.PlayerOne
                            : currentMatch.PlayerTwo);
                        currentMatch.CurrentSet().IsCurrentSet = false;

                        if (CheckMatchIsOverRule(currentMatch)) return true;
                        currentMatch.Sets.Add(new Set() { IsCurrentSet = true });

                        //Is this the final set?
                        if (setsToPlay.DiffValueWith(currentMatch.Sets.Count) == 1 &&
                            currentMatch.MatchFormat.FinalSetType == FinalSetFormats.TenPointChampionShipTieBreak)
                        {
                            //This is the final set                            
                            currentMatch.CurrentSet().Games.Add(new Game() {IsCurrentGame = true, GameType = GameType.TenPointer});
                            SwitchCurrentServer(currentMatch);
                        }
                        else
                        {
                            AddNewGameToCurrentSetAndSwitchCurrentServer(currentMatch);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private static void SwitchCurrentServer(Match currentMatch)
        {
            currentMatch.CurrentServer = currentMatch.CurrentServer.IsPlayerOne
                ? currentMatch.PlayerTwo
                : currentMatch.PlayerOne;
        }

        private int GetNumberOfGamesForSet(Match currentMatch)
        {
            if (currentMatch.Sets.Count == 3 && currentMatch.MatchFormat.FinalSetType == FinalSetFormats.TenPointChampionShipTieBreak)
            {
                return 1;
            }
            return (int)currentMatch.MatchFormat.SetsFormat;
        }

        private bool CheckGameIsOverAndInitializeNewGameIfNeedBe(Match currentMatch)
        {
            CheckMatchIsOverRule(currentMatch);
            if (currentMatch.IsMatchOver) return false;

            var gamesCount = GetNumberOfGamesForSet(currentMatch);
            var currentGame = currentMatch.CurrentGame();
            if (currentGame.GameStatus.Status == Status.GameOver)
            {
                currentGame.IsCurrentGame = false;
                var currentSetGames = currentMatch.CurrentSet().Games;
                if (currentSetGames.Any())
                {
                    //Any player that has reached the number of required games?
                    var groupedWinner = (from n in currentSetGames
                        group n by n.Winner.IsPlayerOne
                        into winloss
                        select new {WinnerIsPlayerOne = winloss.Key, gameCount = winloss.Count()}).ToList();
                    var maxGamesWon = groupedWinner.Max(x => x.gameCount);

                    if (currentMatch.CurrentSet() != null)
                    {
                        if( maxGamesWon < gamesCount)
                        {
                            AddNewGameToCurrentSetAndSwitchCurrentServer(currentMatch);
                            return true;
                        }

                        var playerOneGamesCount = 0;
                        if (groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne) != null) 
                             playerOneGamesCount = groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne).gameCount;
                        var playerTwoGamesCount = 0;
                        if(groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne == false) != null)     
                             playerTwoGamesCount = groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne == false).gameCount;
                        
                        //4-3 (3-4) or 6-5 (5-6) --> We can create a new game
                        if (maxGamesWon == gamesCount && playerOneGamesCount.DiffValueWith(playerTwoGamesCount) == 1)
                        {
                            AddNewGameToCurrentSetAndSwitchCurrentServer(currentMatch);
                            return true;
                        }

                        // 4-4 or 6 - 6
                        if (maxGamesWon == gamesCount && playerOneGamesCount == playerTwoGamesCount)
                        {
                            currentMatch.CurrentSet().Games.Add(new Game() { IsCurrentGame = true, GameType = GameType.SevenPointer });
                            SwitchCurrentServer(currentMatch);
                            return true;
                        }
                    }
                    
                    //
                }
            }
            return false;
        }

        private static void AddNewGameToCurrentSetAndSwitchCurrentServer(Match currentMatch)
        {
            currentMatch.CurrentSet().Games.Add(new Game() {IsCurrentGame = true});
            SwitchCurrentServer(currentMatch);
        }

        private static bool CheckDeuceRule(Match currentMatch, ref Game currentGame)
        {
            if (currentGame.GameType == GameType.Normal && currentGame.PlayerOneScore == currentGame.PlayerTwoScore && currentGame.PlayerOneScore >= 3)
            {
                if (currentMatch.MatchFormat.DeuceFormat == DeuceFormat.SuddenDeath)
                {
                    //It is Game point to either player
                    currentGame.GameStatus.Status = Status.GamePoint;
                }
                else
                {
                    currentGame.GameStatus.Status = Status.Deuce;
                }
                currentGame.GameStatus.Player = null; // TODO: We should set this to the player that just earned the point but it is not passed in Should refactor later
                return true;
            }
            return false;
        }

        private static bool CheckTieBreakerGameOverRule(Match currentMatch, ref Game currentGame)
        {

            if (currentGame.GameType == GameType.SevenPointer)
            {
                //We should switch if the total score is an odd number
                if ((currentGame.PlayerOneScore + currentGame.PlayerTwoScore)%2 == 1)
                {
                   SwitchCurrentServer(currentMatch); 
                }

                if (currentGame.PlayerOneScore >= 7)
                {
                    currentGame.Winner = currentMatch.PlayerOne;
                    currentGame.GameStatus = new GameStatus
                    {
                        Status = Status.GameOver,
                        Player = currentMatch.PlayerOne
                    };

                    currentMatch.MatchStats.Last().MatchSituations.Add(new MatchSituation()
                    {
                        GameId = currentGame.GameId,
                        Id = Guid.NewGuid().ToString(),
                        MatchSituationType = currentMatch.CurrentServer.IsPlayerOne ? MatchSituationType.GamePointWon : MatchSituationType.BreakPointWon,
                        Player = currentMatch.PlayerOne,
                        SetId = currentMatch.CurrentSet().SetId
                    });

                    return true;
                }
                if (currentGame.PlayerTwoScore >= 7)
                {
                    currentGame.Winner = currentMatch.PlayerTwo;
                    currentGame.GameStatus = new GameStatus
                    {
                        Status = Status.GameOver,
                        Player = currentMatch.PlayerTwo
                    };
                    currentMatch.MatchStats.Last().MatchSituations.Add(new MatchSituation()
                    {
                        GameId = currentGame.GameId,
                        Id = Guid.NewGuid().ToString(),
                        MatchSituationType = currentMatch.CurrentServer.IsPlayerOne == false ? MatchSituationType.GamePointWon : MatchSituationType.BreakPointWon,
                        Player = currentMatch.PlayerTwo,
                        SetId = currentMatch.CurrentSet().SetId
                    });
                    return true;
                }
            }
            return false;
        }

        private static bool CheckGameOverRule(Match currentMatch, ref Game currentGame)
        {
            //Player player;
            if (currentGame.GameType == GameType.Normal)
            {
                if (currentGame.PlayerOneScore > currentGame.PlayerTwoScore)
                {
                    //Player one is leading by two points after 4 points
                    if (currentGame.PlayerOneScore >= 4 &&
                        currentGame.PlayerTwoScore <= (currentGame.PlayerOneScore - 2))
                    {
                        currentGame.Winner = currentMatch.PlayerOne;
                        SetPlayerOneAsWinnerOfGame(currentMatch, currentGame);
                        currentMatch.MatchStats.Last().MatchSituations.Add(new MatchSituation()
                        {
                            GameId = currentGame.GameId,
                            Id = Guid.NewGuid().ToString(),
                            MatchSituationType = currentMatch.CurrentServer.IsPlayerOne ? MatchSituationType.GamePointWon : MatchSituationType.BreakPointWon,
                            Player = currentMatch.PlayerOne,
                            SetId = currentMatch.CurrentSet().SetId
                        });
                        return true;
                    }
                }
                else if (currentGame.PlayerTwoScore > currentGame.PlayerOneScore)
                {
                    //Player two is leading by two points after 4 points
                    if (currentGame.PlayerTwoScore >= 4 &&
                        currentGame.PlayerOneScore <= (currentGame.PlayerTwoScore - 2))
                    {
                        currentGame.Winner = currentMatch.PlayerTwo;
                        SetPlayerTwoAsWinnerOfGame(currentMatch, currentGame);
                        currentMatch.MatchStats.Last().MatchSituations.Add(new MatchSituation()
                        {
                            GameId = currentGame.GameId,
                            Id = Guid.NewGuid().ToString(),
                            MatchSituationType = currentMatch.CurrentServer.IsPlayerOne == false ? MatchSituationType.GamePointWon : MatchSituationType.BreakPointWon,
                            Player = currentMatch.PlayerTwo,
                            SetId = currentMatch.CurrentSet().SetId
                        });
                        return true;
                    }
                }
            }

            if (currentGame.GameType != GameType.Normal)
            {
                var gameCounts = (int) currentGame.GameType;
                if (currentGame.PlayerOneScore == gameCounts)
                {
                    SetPlayerOneAsWinnerOfGame(currentMatch, currentGame);
                    //If this is the final set -> the match is over
                    FlagSetWinnerForChampionShipTieBreak(currentMatch, currentMatch.PlayerOne);
                    return true;
                }

                if (currentGame.PlayerTwoScore == gameCounts)
                {
                    currentGame.Winner = currentMatch.PlayerTwo;
                    SetPlayerTwoAsWinnerOfGame(currentMatch, currentGame);
                    //If this is the final set -> the match is over
                    FlagSetWinnerForChampionShipTieBreak(currentMatch, currentMatch.PlayerTwo);
                    return true;
                }

            }
            return false;
        }

        private static void FlagSetWinnerForChampionShipTieBreak(Match currentMatch, Player setWinner)
        {
            if (currentMatch.Sets.Count == 3)
            {
                currentMatch.CurrentSet().SetWonBy(setWinner);
            }
        }

        private static void SetPlayerTwoAsWinnerOfGame(Match currentMatch, Game currentGame)
        {
            currentGame.Winner = currentMatch.PlayerTwo;
            currentGame.GameStatus = new GameStatus
            {
                Status = Status.GameOver,
                Player = currentMatch.PlayerTwo
            };
        }

        private static void SetPlayerOneAsWinnerOfGame(Match currentMatch, Game currentGame)
        {
            currentGame.Winner = currentMatch.PlayerOne;
            currentGame.GameStatus = new GameStatus
            {
                Status = Status.GameOver,
                Player = currentMatch.PlayerOne
            };
        }

        private static bool CheckAdvantageRule(Match currentMatch, ref Game currentGame)
        {
            if (currentGame.PlayerOneScore >= 3 && currentGame.PlayerTwoScore >= 3)
            {
                if (currentGame.PlayerOneScore == currentGame.PlayerTwoScore + 1)
                {
                    //Advantage to playerOne
                    SetGameStatusForPlayer(currentMatch.PlayerOne, currentGame, Status.Advantage);
                    currentMatch.MatchStats.Last().MatchSituations.Add(new MatchSituation()
                    {
                        GameId = currentGame.GameId,
                        MatchSituationType = MatchSituationType.GamePoint,
                        Player = currentMatch.PlayerOne,
                        SetId = currentMatch.CurrentSet().SetId,
                        Id = Guid.NewGuid().ToString()
                    });

                    return true;
                }
                if (currentGame.PlayerTwoScore == currentGame.PlayerOneScore + 1)
                {
                    //Advantage to PlayerTwo
                    currentMatch.MatchStats.Last().MatchSituations.Add(new MatchSituation()
                    {
                        GameId = currentGame.GameId,
                        MatchSituationType = MatchSituationType.GamePoint,
                        Player = currentMatch.PlayerTwo,
                        SetId = currentMatch.CurrentSet().SetId,
                        Id = Guid.NewGuid().ToString()
                    });

                    SetGameStatusForPlayer(currentMatch.PlayerTwo, currentGame, Status.Advantage);
                    return true;
                }
            }
            return false;
        }

        private static void SetGameStatusForPlayer(Player player, Game currentGame, Status status)
        {
            currentGame.GameStatus = new GameStatus
            {
                Status = status,
                Player = player
            };

        }

        private static bool CheckBreakPointForPlayerTwoRule(Match currentMatch, ref Game currentGame)
        {
            if ((currentGame.PlayerOneScore == 3) && (currentGame.PlayerTwoScore <= 2))
            {
                if (currentMatch.CurrentServer.IsPlayerOne)
                {
                    SetGameStatusForPlayer(currentMatch.PlayerOne, currentGame, Status.GamePoint);
                    var matchSituation = new MatchSituation()
                    {
                        GameId = currentGame.GameId,
                        MatchSituationType = MatchSituationType.GamePoint,
                        Player = currentMatch.PlayerOne,
                        SetId = currentMatch.CurrentSet().SetId,
                        Id = Guid.NewGuid().ToString()
                    };
                    currentMatch.MatchStats.Last().MatchSituations.Add(matchSituation);
                    


                    return true;
                }
                else
                {
                    SetGameStatusForPlayer(currentMatch.PlayerTwo, currentGame, Status.BreakPoint);
                    currentMatch.MatchStats.Last().MatchSituations.Add(new MatchSituation()
                    {
                        GameId = currentGame.GameId,
                        MatchSituationType = MatchSituationType.BreakPoint,
                        Player = currentMatch.PlayerTwo,
                        SetId = currentMatch.CurrentSet().SetId,
                        Id = Guid.NewGuid().ToString()
                    });
                    return true;
                }
            }
            return false;
        }

        private static bool CheckGamePointOnSuddenDeathDeuceRule(Match currentMatch, ref Game currentGame)
        {
            if (currentGame.GameType != GameType.Normal) return false;
            if (currentMatch.MatchFormat.DeuceFormat != DeuceFormat.SuddenDeath) return false;
            if (currentGame.PlayerOneScore == 4)
            {
                currentGame.Winner = currentMatch.PlayerOne;
                SetGameStatusForPlayer(currentMatch.PlayerOne, currentGame, Status.GameOver);
                currentMatch.MatchStats.Add(new MatchStat()
                {
                    Player = currentMatch.PlayerOne,
                    PointWonLostOrNone = PointWonLostOrNone.NotAPoint,
                    Reason = StatDescription.GameOver,
                    Server = currentMatch.CurrentServer
                });
                return true;
            }
            if (currentGame.PlayerTwoScore == 4)
            {
                currentGame.Winner = currentMatch.PlayerTwo;
                SetGameStatusForPlayer(currentMatch.PlayerTwo, currentGame, Status.GameOver);
                currentMatch.MatchStats.Add(new MatchStat()
                {
                    Player = currentMatch.PlayerTwo,
                    PointWonLostOrNone = PointWonLostOrNone.NotAPoint,
                    Reason = StatDescription.GameOver,
                    Server = currentMatch.CurrentServer
                });
                return true;
            }
            return false;
        }

        private static bool CheckBreakPointForPlayerOneRule(Match currentMatch, ref Game currentGame)
        {
            if ((currentGame.PlayerTwoScore == 3) && (currentGame.PlayerOneScore <= 2))
            {
                if (currentMatch.CurrentServer.IsPlayerOne)
                {
                    SetGameStatusForPlayer(currentMatch.PlayerTwo, currentGame, Status.BreakPoint);
                    currentMatch.MatchStats.Last().MatchSituations.Add(new MatchSituation()
                    {
                        GameId = currentGame.GameId,
                        MatchSituationType = MatchSituationType.BreakPoint,
                        Player = currentMatch.PlayerTwo,
                        SetId = currentMatch.CurrentSet().SetId,
                        Id = Guid.NewGuid().ToString()
                    });
                    return true;
                }

                SetGameStatusForPlayer(currentMatch.PlayerTwo, currentGame, Status.GamePoint);
                currentMatch.MatchStats.Last().MatchSituations.Add(new MatchSituation()
                    {GameId = currentGame.GameId, MatchSituationType = MatchSituationType.GamePoint, 
                        Player = currentMatch.PlayerTwo, 
                        SetId = currentMatch.CurrentSet().SetId,
                        Id = Guid.NewGuid().ToString()});
                return true;
            }
            return true;
        }
    }

    public class UserService : IUserService
    {
        public async Task<string> GetCurrentUserAsync()
        {
            return await UserInformation.GetFirstNameAsync();
        }

        public async Task<BitmapImage> LoadUserImageAsync()
        {
            var bitmapImage = new BitmapImage();

            var image = UserInformation.GetAccountPicture(AccountPictureKind.SmallImage) as StorageFile;
            if (image != null)
            {
                using (IRandomAccessStreamWithContentType stream = await image.OpenReadAsync())
                {
                    await bitmapImage.SetSourceAsync(stream);
                }
                //var imageStream = await image.OpenReadAsync();
            }
            return bitmapImage;
        }
    }

    public interface IUserService
    {
        Task<BitmapImage> LoadUserImageAsync();
        Task<string> GetCurrentUserAsync();
    }

    public class FakeUserSevice : IUserService
    {
        public async Task<BitmapImage> LoadUserImageAsync()
        {
            return null;
        }

        public async Task<string> GetCurrentUserAsync()
        {
            return "Abdul";
        }
    }
}
