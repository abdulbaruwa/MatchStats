using System;
using System.Collections.Generic;
using System.Windows.Input;
using ReactiveUI;

namespace MatchStats.Model
{
    public interface IGameAction : ICommand
    {
        string Name { get; set; }
        string DisplayName { get; set; }
        Player Player { get; set; }
    }

    public abstract class GameAction : IGameAction
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Player Player { get; set; }
        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);

        public event EventHandler CanExecuteChanged;
    }
        //    ForeHandWinner, >>>
        //BackHandWinner,
        //VolleyWinner,
        //DropShotWinner,
        //OverheadWinner,
        //UnforcedForehandError,
        //UnforcedBackhadError,
        //UnforcedVolleyError,
        //ForcedError
         //DoubleFault

    public class ScoreGamePointAction : GameAction
    {
        public PointReason PointReason { get; set; }
        public bool ScoreForOpponent { get; set; }
        public override bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public override void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }

    public class NonScoreGameAction : GameAction
    {
        public NonScoreActionReason NonScoreActionReason { get; set; }
        public override bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public override void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }

    public class ScorePoint : IGameAction
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public PointReason PointReason { get; set; }
        public Player Player { get; set; }
        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        public event EventHandler CanExecuteChanged;
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
        UnforcedForehandError,
        UnforcedBackhadError,
        UnforcedVolleyError,
        ForcedError
    }

    public enum PointLossReason
    {
        UnforcedForehandError,
        UnforcedBackhadError,
        ForcedError
    }

    public enum NonScoreActionReason
    {
        FirstServeIn,
        FirstServeOut,
        SecondServeIn
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