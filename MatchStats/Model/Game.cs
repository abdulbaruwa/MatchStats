using System.ComponentModel.DataAnnotations;

namespace MatchStats.Model
{
    public class Game
    {
        public Game()
        {
            GameStatus = new GameStatus();
            GameType = GameType.Normal;
        }

        public Player Winner { get; set; }
        public int PlayerOneScore { get; set; }
        public int PlayerTwoScore { get; set; }
        public bool IsCurrentGame { get; set; }
        public GameStatus GameStatus { get; set; }
        public GameType GameType { get; set; }
    }

    public class GameStatus
    {
        public GameStatus()
        {
            Status = Status.Neutral;
        }
        public Player Player { get; set; }
        public Status Status { get; set; }
    }

    public enum GameType
    {
        [Display(Name= "Normal Game")]
        Normal,
        [Display(Name = "Seven Point Game")]
        SevenPointer,
        [Display(Name = "Ten Point Game")]
        TenPointer
    }

    public enum Status
    {
        [Display(Name = "Neutral")]
        Neutral,
        [Display(Name = "Duece")]
        Duece,
        [Display(Name = "Duece")]
        BreakPoint,
        [Display(Name = "Advantage")]
        Advantage,
        [Display(Name = "Game Over")]
        GameOver,
        [Display(Name = "Game Point")]
        GamePoint
    }
}