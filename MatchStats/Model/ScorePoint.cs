using System.Collections.Generic;

namespace MatchStats.Model
{
    public class ScorePoint
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public PointReason PointReason { get; set; }
        public Player Player { get; set; }
    }

    public class ScorPoint
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool IsForcedError { get; set; }
        public Player Player { get; set; }
    }

    public enum PointReason
    {
        ForeHandWinner,
        BackHandWinner,
        VolleyWinner,
        DropShotWinner,
        OverheadWinner,
        UnforcedForeHand,
        UnforcedForehandError,
        UnforcedBackhadError,
        ForcedError
    }

    public enum PointLossReason
    {
        UnforcedForehandError,
        UnforcedBackhadError,
        ForcedError
    }

    public class BreakPoint
    {
        public Player Player { get; set; }
        public bool Won { get; set; }
    }

    public class GameScoreEngine
    {
        private IList<BreakPoint> _breakPoints;
        private IList<ScorePoint> _playerOnePoints;
        private IList<ScorePoint> _playerTwoPoints;

        public GameScoreEngine()
        {
            _breakPoints = new List<BreakPoint>();
            _playerOnePoints = new List<ScorePoint>();
            _playerTwoPoints = new List<ScorePoint>();
        }
        
        public void PointScored(ScorePoint scorePoint)
        {
            //if(ScorePoint.Player)
        }
    }
}