using System;
using System.Collections.Generic;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.DesignTimeStuff
{
    public class DummyDataBuilder
    {
        public IEnumerable<MyMatchStats> BuildMatchStatsForDesignTimeView()
        {
            var matchStats = new List<MyMatchStats>();
            matchStats.Add(DummyMatchStats1(0));
            matchStats.Add(DummyMatchStats2(1));
            matchStats.Add(DummyMatchStats3(2));
            matchStats.Add(DummyMatchStats2(3));
            matchStats.Add(DummyMatchStats1(4));
            return matchStats;
        }

        private static MyMatchStats DummyMatchStats1(int index)
        {
            var playerOne = new Player() { FirstName = "Adeomola", Rating = "7.2", SurName = "Baruwa" };
            var playerTwo = new Player() { FirstName = "Kieran", Rating = "7.1", SurName = "Reed" };
            var gameOne = new Game() { PlayerTwoScore = 7, PlayerOneScore = 5, Winner = playerTwo };
            var gameTwo = new Game() { PlayerTwoScore = 7, PlayerOneScore = 6, Winner = playerOne };
            var gameThree = new Game() { PlayerTwoScore = 6, PlayerOneScore = 3, Winner = playerOne };

            return new MyMatchStats()
            {
                Date = DateTime.Now,
                Duration = 99,
                PlayerOne = playerOne,
                PlayerTwo = playerTwo,
                Score = new Score() { GameOne = gameOne, GameTwo = gameTwo, GameThree = gameThree },
                TournamentName = "Sutton Clay Open",
                DefaultPlayerWon = true,
                IndexWithinParentCollection = index
            };
        }

        private static MyMatchStats DummyMatchStats2(int index)
        {
            var playerOne = new Player() { FirstName = "Adeomola", Rating = "7.2", SurName = "Baruwa" };
            var playerTwo = new Player() { FirstName = "Adam", Rating = "7.1", SurName = "Keop" };
            var gameOne = new Game() { PlayerTwoScore = 7, PlayerOneScore = 5, Winner = playerTwo };
            var gameTwo = new Game() { PlayerTwoScore = 7, PlayerOneScore = 6, Winner = playerOne };
            var gameThree = new Game() { PlayerTwoScore = 6, PlayerOneScore = 3, Winner = playerOne };

            return new MyMatchStats()
            {
                Date = DateTime.Now,
                Duration = 59,
                PlayerOne = playerOne,
                PlayerTwo = playerTwo,
                Score = new Score() { GameOne = gameOne, GameTwo = gameTwo, GameThree = gameThree },
                TournamentName = "DL Open",
                DefaultPlayerWon = true,
                IndexWithinParentCollection = index
            };

        }

        private static MyMatchStats DummyMatchStats3(int index)
        {
            var playerOne = new Player() { FirstName = "Adeomola", Rating = "7.2", SurName = "Baruwa" };
            var playerTwo = new Player() { FirstName = "Quinton", Rating = "6.1", SurName = "Kumaressagem" };
            var gameOne = new Game() { PlayerTwoScore = 7, PlayerOneScore = 5, Winner = playerTwo };
            var gameTwo = new Game() { PlayerTwoScore = 7, PlayerOneScore = 6, Winner = playerOne };
            var gameThree = new Game() { PlayerTwoScore = 6, PlayerOneScore = 2, Winner = playerTwo };

            return new MyMatchStats()
            {
                Date = DateTime.Now,
                Duration = 59,
                PlayerOne = playerOne,
                PlayerTwo = playerTwo,
                Score = new Score() { GameOne = gameOne, GameTwo = gameTwo, GameThree = gameThree },
                TournamentName = "Heston Summer Championship",
                DefaultPlayerWon = false,
                IndexWithinParentCollection = index
            };

        }

        //public UpcomingMatchesControlViewModel BuildUpcomingMatchesDataForDesignView()
        //{
        //    var playerOne = new Player() { FirstName = "Adeomola", Rating = "7.2", SurName = "Baruwa" };
        //    var upcomingMatch = new UpcomingMatchesControlViewModel();
        //    upcomingMatch.DefaultPlayer = playerOne;
        //    upcomingMatch.MatchFormat = new MatchFormat() { FinalSetType = FinalSetFormats.TenPointChampionShipTieBreak, Sets = 3 };
        //    upcomingMatch.StartTime = DateTime.Now.AddHours(3.0);

        //    upcomingMatch.Matches = new ReactiveList<MyMatchStats>();
        //    upcomingMatch.Matches.Add(new MyMatchStats()
        //    {
        //        Date = DateTime.Now.AddHours(4.0),
        //        PlayerOne = playerOne,
        //        PlayerTwo = new Player() { FirstName = "Winston", SurName = "Willis", Rating = "6.1" },
        //        TournamentName = "Sutton Clay Open",
        //        Tournament = new Tournament() { StartDate = DateTime.Now, TournamentGrade = "4", TournamentName = "Sutton Clay Open" },
        //        MatchFormat = upcomingMatch.MatchFormat,
        //        IndexWithinParentCollection = 0
        //    });

        //    upcomingMatch.Matches.Add(new MyMatchStats()
        //    {
        //        Date = DateTime.Now.AddHours(8.0),
        //        PlayerOne = playerOne,
        //        PlayerTwo = new Player() { FirstName = "Elliot", SurName = "Chad", Rating = "5.2" },
        //        TournamentName = "Sutton Clay Open",
        //        Tournament = new Tournament() { StartDate = DateTime.Now, TournamentGrade = "4", TournamentName = "Sutton Clay Open" },
        //        MatchFormat = upcomingMatch.MatchFormat,
        //        IndexWithinParentCollection = 1
        //    });

        //    upcomingMatch.Matches.Add(new MyMatchStats()
        //    {
        //        Date = DateTime.Now.AddDays(1.0),
        //        PlayerOne = playerOne,
        //        PlayerTwo = new Player() { FirstName = "Michael", SurName = "Brownie", Rating = "5.2" },
        //        TournamentName = "Sutton Clay Open",
        //        Tournament = new Tournament() { StartDate = DateTime.Now, TournamentGrade = "4", TournamentName = "Sutton Clay Open" },
        //        MatchFormat = upcomingMatch.MatchFormat,
        //        IndexWithinParentCollection = 2
        //    });

        //    upcomingMatch.Matches.Add(new MyMatchStats()
        //    {
        //        Date = DateTime.Now.AddDays(1.0).AddHours(4.0),
        //        PlayerOne = playerOne,
        //        PlayerTwo = new Player() { FirstName = "Benjamin", SurName = "Dada", Rating = "6.2" },
        //        TournamentName = "Sutton Clay Open",
        //        Tournament = new Tournament() { StartDate = DateTime.Now, TournamentGrade = "4", TournamentName = "Sutton Clay Open" },
        //        MatchFormat = upcomingMatch.MatchFormat,
        //        IndexWithinParentCollection = 3
        //    });
        //    return upcomingMatch;
        //}
    }
}