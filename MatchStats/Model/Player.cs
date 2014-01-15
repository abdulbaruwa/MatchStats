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
        public Player Server { get; set; }
        public StatDescription Reason { get; set; }
        public PointWonLostOrNone PointWonLostOrNone { get; set; }
        public Player Player { get; set; }
        public bool UndoPrevious
        {
            get
            {
                return Reason == StatDescription.BreakPoint || Reason == StatDescription.GamePoint ||
                       Reason == StatDescription.GameOver;
            }
        }
    }

    public enum PointWonLostOrNone
    {
        NotAPoint = 0,
        PointWon = 1,
        PointLost = 2
    }
}
