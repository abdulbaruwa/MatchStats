using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MatchStats.Model
{
    public class ScorePoint
    {
        public string Name { get; set; }
        public PointReason PointReason { get; set; }
        public Player Player { get; set; }
    }

    public enum PointReason
    {
        //[Display9"Forehand Winner"]
        ForeHandWinner,
        BackHandWinner,

    }

    public class GameScoreEngine
    {
        
        private IList<ScorePoint> _playerOnePoints;
        private IList<ScorePoint> _playerTwoPoints;

        public GameScoreEngine()
        {
            _playerOnePoints = new List<ScorePoint>();
            _playerTwoPoints = new List<ScorePoint>();
        }

        public void PointScored(ScorePoint scorePoint)
        {
            //if(ScorePoint.Player)
        }
        
    }
}