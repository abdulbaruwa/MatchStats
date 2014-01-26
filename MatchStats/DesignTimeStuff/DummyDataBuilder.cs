using System;
using System.Collections.Generic;
using MatchStats.Enums;
using MatchStats.Model;

namespace MatchStats.DesignTimeStuff
{
    public class MatchBuilder
    {
        public MatchBuilder()
        {
            _match = new Match();
            _match.Score = new Score();
            _match.MatchGuid = Guid.NewGuid();
            _playerOne = new Player();
            _playerTwo = new Player();
        }

        private Match _match;
        private Tournament _tournament;
        private Player _playerOne;
        private Player _playerTwo;
        private bool _matchWinnerIsPlayerOne;
        private FinalSetFormats _finalSetFormat;

        public Match Build()
        {
            _match.Score.Winner = _matchWinnerIsPlayerOne ? _playerOne : _playerTwo;
            _match.Tournament = _tournament;
            _match.PlayerOne = _playerOne;
            _match.PlayerTwo = _playerTwo;
            _match.MatchFormat = new MatchFormat() {FinalSetType = _finalSetFormat};
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

        public MatchBuilder WithFinalSetMatchFormat(FinalSetFormats finalSetFormat)
        {
            _finalSetFormat = finalSetFormat;
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
                Winner = _playerTwo
            };
        }

        private Game GameForPlayerOne()
        {
            return new Game()
            {
                GameType = GameType.Normal,
                PlayerOneScore = 4,
                PlayerTwoScore = 2,
                Winner = _playerOne
            };
        }

        public MatchBuilder WithMatchWinnerIsPlayerOne(bool isPlayerOne)
        {
            _matchWinnerIsPlayerOne = isPlayerOne;
            return this;

        }
        public MatchBuilder WithPlayer(string firstName, string surname, bool isPlayerOne, string rating)
        {
            var player = new Player()
            {
                FirstName = firstName,
                SurName = surname,
                IsPlayerOne = isPlayerOne,
                Rating = rating
            };

            if (isPlayerOne)
                _playerOne = player;
            else
            {
                _playerTwo = player;
            }

            return this;
        }
    }
    public class DummyDataBuilder
    {
        public IEnumerable<Match> BuildMatchStatsForDesignTimeView()
        {
            var matchStats = new List<Match>();
            matchStats.Add(new MatchBuilder()
                .WithTournamentNameAndGrade("Sutton Open", Grade.Grade4)
                .WithPlayer("Ademola","Adekola",true,"7.2")
                .WithPlayer("Chris","Cole",false,"7.2")
                .WithSetForPlayerTwo()
                .WithSetForPlayerOne()
                .WithSetForPlayerOne()
                .WithMatchWinnerIsPlayerOne(true)
                .Build());
            
            matchStats.Add(new MatchBuilder()
                .WithTournamentNameAndGrade("Sutton Open", Grade.Grade4)
                .WithPlayer("Ademola","Adekola",true,"7.2")
                .WithPlayer("Benjamin","Cole",false,"7.2")
                .WithSetForPlayerTwo()
                .WithSetForPlayerOne()
                .WithSetForPlayerOne()
                .WithMatchWinnerIsPlayerOne(true)

                .Build());
            
            matchStats.Add(new MatchBuilder()
                .WithTournamentNameAndGrade("Sutton Open", Grade.Grade4)
                .WithPlayer("Bola","Smith",true,"7.2")
                .WithPlayer("Ademola","Adekola",false,"7.2")
                .WithSetForPlayerTwo()
                .WithSetForPlayerTwo()
                .Build());

            matchStats.Add(new MatchBuilder()
                .WithTournamentNameAndGrade("Surrey Close", Grade.Grade4)
                .WithPlayer("Ademola","Adekola",true,"7.2")
                .WithPlayer("Smith","Michael",false,"7.2")
                .WithSetForPlayerOne()
                .WithSetForPlayerOne()
                .WithMatchWinnerIsPlayerOne(true)
                .Build());

            return matchStats;
        }
    }
}