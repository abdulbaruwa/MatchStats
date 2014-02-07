using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MatchStats.Model
{
    public class Game
    {
        public Game()
        {
            GameStatus = new GameStatus();
            GameType = GameType.Normal;
            Points = new List<Point>();
            GameId = Guid.NewGuid().ToString();
        }

        public Player Winner { get; set; }
        public int PlayerOneScore { get; set; }
        public int PlayerTwoScore { get; set; }
        public bool IsCurrentGame { get; set; }
        public GameStatus GameStatus { get; set; }
        public GameType GameType { get; set; }
        public List<Point> Points { get; set; }
        public string GameId { get; set; }
        public MatchSituation LastMatchSituation { get; set; }
    }

    public class Point
    {

        public Player Server { get; set; }
        public Player Player { get; set; }
        public PointReason PointReason { get; set; }
        public MatchSituation MatchSituationBefore { get; set; } 
        public MatchSituation MatchSituationAfter { get; set; }
        public List<Serve> Serves { get; set; } 

    }

    public class Serve
    {
        public Player Server { get; set; }
        public bool IsFirstServe{get; set;}
        public bool ServeIsIn { get; set; }
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
        Normal = 0,
        [Display(Name = "Seven Point Game")]
        SevenPointer = 7,
        [Display(Name = "Ten Point Game")]
        TenPointer = 10
    }

    public enum Status
    {
        [Display(Name = "Neutral")]
        Neutral,
        [Display(Name = "Deuce")]
        Deuce,
        [Display(Name = "Deuce")]
        BreakPoint,
        [Display(Name = "Advantage")]
        Advantage,
        [Display(Name = "Game Over")]
        GameOver,
        [Display(Name = "Game Point")]
        GamePoint
    }
}
