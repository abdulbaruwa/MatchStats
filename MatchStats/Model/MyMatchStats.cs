using System;
using MatchStats.Enums;

namespace MatchStats.Model
{
    public class MyMatchStats
    {
        public string TournamentName { get; set; }
        public DateTime Date { get; set; }

        public string DisplayDate
        {
            get { return Date.ToString("dd MMMM yyyy"); }
        }

        public int Duration { get; set; }
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
        public bool DefaultPlayerWon { get; set; }
        public Tournament Tournament { get; set; }
        public MatchFormat MatchFormat { get; set; }

        public string DefaultPlayerWinLose
        {
            get { return DefaultPlayerWon ? "&#xE071" : "&#xE071;"; }
        }

        public string WonLoss
        {
            get { return DefaultPlayerWon ? "Won" : "Lost"; }
        }

        public Score Score { get; set; }

        public string MatchScore
        {
            get
            {
                return GetGameScoreText(Score.GameOne) + " " + GetGameScoreText(Score.GameTwo) + " " +
                       GetGameScoreText(Score.GameThree);
            }
        }


        public int IndexWithinParentCollection { get; set; }

        private string GetGameScoreText(Game game)
        {
            string matchScore;
            if (Score.GameOne.Winner == PlayerOne)
            {
                matchScore = game.ForScore + "-" + game.AgainstScore;
            }
            else
            {
                matchScore = game.AgainstScore + "-" + game.ForScore;
            }
            return matchScore;
        }
    }

    public class PointReason
    {
        public string Name { get; set; }
        public string Player { get; set; }
    }


    public class Score
    {
        public Game GameOne { get; set; }
        public Game GameTwo { get; set; }
        public Game GameThree { get; set; }
    }

    public class Game
    {
        public Player Winner { get; set; }
        public int ForScore { get; set; }
        public int AgainstScore { get; set; }
    }

    public class Player
    {
        public string FirstName { get; set; }
        public string SurName { get; set; }

        public string FullName
        {
            get { return FirstName + " " + SurName; }
        }

        public string Rating { get; set; }
    }

    public class MatchFormat
    {
        public int Sets { get; set; }
        public SetsFormat SetsFormat { get; set; }
        public FinalSetFormats FinalSetType { get; set; }
        public DueceFormat DueceFormat { get; set; }
    }

    public class Tournament
    {
        public string TournamentName { get; set; }
        public string TournamentGrade { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Endate { get; set; }
    }
}