namespace MatchStats.Model
{
    public class Game
    {
        public Player Winner { get; set; }
        public int PlayerOneScore { get; set; }
        public int PlayerTwoScore { get; set; }
        public bool IsCurrentGame { get; set; }
    }
}