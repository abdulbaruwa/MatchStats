using System;
using System.Collections.Generic;
using Windows.System.Threading;
using MatchStats.Enums;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.DesignTimeStuff
{
    public class MatchBuilder
    {
        public MatchBuilder(Player playerOne, Player playerTwo)
        {
            _match = new Match();
            _match.PlayerOne = playerOne;
            _match.PlayerTwo = playerTwo;
            _match.MatchGuid = Guid.NewGuid();
        }

        private Match _match;
        private Tournament _tournament;
        public Match Build()
        {
            return _match;
        }

        public MatchBuilder WithSetForPlayerOne()
        {
            var set = new Set {StartTime = DateTime.Now, EndTime = DateTime.Now.AddMinutes(33)};
            set.Games.Add(GameForPlayerOne());
            set.Games.Add(GameForPlayerOne());
            set.Games.Add(GameForPlayerTwo());
            set.Games.Add(GameForPlayerOne());
            set.Games.Add(GameForPlayerOne());
            set.Winner = _match.PlayerOne;
            _match.Score.Sets.Add(set);
            return this;
        }

        public MatchBuilder WithSetForPlayerTwo()
        {
            var set = new Set { StartTime = DateTime.Now, EndTime = DateTime.Now.AddMinutes(33) };
            set.Games.Add(GameForPlayerOne());
            set.Games.Add(GameForPlayerTwo());
            set.Games.Add(GameForPlayerOne());
            set.Games.Add(GameForPlayerTwo());
            set.Games.Add(GameForPlayerTwo());
            set.Games.Add(GameForPlayerTwo());
            set.Winner = _match.PlayerTwo;
            _match.Score.Sets.Add(set);
            return this;
        }

        public MatchBuilder WithTournamentNameAndGrade(string tournamentName, Grade grade)
        {
            _tournament = _tournament ?? new Tournament();
            _tournament.TournamentName = tournamentName;
            _tournament.Grade = grade;
            _tournament.StartDate = DateTime.Now;
            _tournament.Endate = DateTime.Now.AddHours(7);
            return this;
        }

        private Game GameForPlayerTwo()
        {
            return new Game()
            {
                GameType = GameType.Normal,
                PlayerOneScore = 4,
                PlayerTwoScore = 2,
                Winner = _match.PlayerTwo
            };
        }

        private Game GameForPlayerOne()
        {
            return new Game()
            {
                GameType = GameType.Normal,
                PlayerOneScore = 4,
                PlayerTwoScore = 2,
                Winner = _match.PlayerOne
            };
        }

    }
    public class DummyDataBuilder
    {
        public IEnumerable<Match> BuildMatchStatsForDesignTimeView()
        {
            var playerOne = new Player() {FirstName = "Ademola", SurName = "Adedayo", IsPlayerOne = true, Rating = "7.2"};
            var playerTwo = new Player() {FirstName = "Luke", SurName = "Nadal", IsPlayerOne = true, Rating = "7.2"};

            new MatchBuilder(playerOne, playerTwo).WithSetForPlayerOne()
                .WithSetForPlayerTwo()
                .WithSetForPlayerOne()
                .WithTournamentNameAndGrade("Sutton Open", Grade.Grade4)
                .Build();

            var matchStats = new List<Match>();
            ~Here 
            return matchStats;
        }
    }
}