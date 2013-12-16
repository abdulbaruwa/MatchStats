using System;

namespace MatchStats.Model
{
    public class MyMatchStats
    {
        //There is a duplication between this entity and the 'Match' entity. 
        //Eventually I need to merge them.
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
        public Score Score { get; set; }
        public int IndexWithinParentCollection { get; set; }

        public string DefaultPlayerWinLose
        {
            get { return DefaultPlayerWon ? "&#xE071" : "&#xE071;"; }
        }

        public string WonLoss
        {
            get { return DefaultPlayerWon ? "Won" : "Lost"; }
        }

        public string MatchScore
        {
            get
            {
                return GetGameScoreText(Score.GameOne) + " " + GetGameScoreText(Score.GameTwo) + " " +
                       GetGameScoreText(Score.GameThree);
            }
        }

        private string GetGameScoreText(Game game)
        {
            string matchScore;
            if (Score.GameOne.Winner == PlayerOne)
            {
                matchScore = game.PlayerOneScore + "-" + game.PlayerTwoScore;
            }
            else
            {
                matchScore = game.PlayerTwoScore + "-" + game.PlayerOneScore;
            }
            return matchScore;
        }
    }
}