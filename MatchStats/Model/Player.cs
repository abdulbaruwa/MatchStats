using System.Collections.Generic;

namespace MatchStats.Model
{
    public class Player
    {
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string FullName
        {
            get { return FirstName + " " + SurName; }
        }
        public string Rating { get; set; }
        public bool IsPlayerOne { get; set; }
    }

    public class MatchStat
    {
        public MatchStat()
        {
            MatchSituations = new List<MatchSituation>();
        }

        public Player Server { get; set; }
        public StatDescription Reason { get; set; }
        public PointWonLostOrNone PointWonLostOrNone { get; set; }
        public Player Player { get; set; }
        public List<MatchSituation> MatchSituations {get; set;}
        public bool UndoPrevious
        {
            get
            {
                return Reason == StatDescription.BreakPoint || Reason == StatDescription.GamePoint || Reason == StatDescription.GameOver;
            }
        }
        public string GameId { get; set; }
        public string SetId { get; set; }
    }

    public class MatchSituation
    {
        public MatchSituationType MatchSituationType { get; set; }
        public Player Player { get; set; }
        public string GameId { get; set; }
        public string SetId { get; set; }
    }

    public enum  MatchSituationType
    {
        GamePoint,
        GamePointWon,
        GamePointLost,
        BreakPoint,
        BreakPointWon,
        BreakPointLost,
        MatchPoint,
        MatchPointWon,
        MatchPointLost
    }

    public enum PointWonLostOrNone
    {
        NotAPoint = 0,
        PointWon = 1,
        PointLost = 2
    }
}
