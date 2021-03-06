﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ReactiveUI;

namespace MatchStats.Model
{
    public interface IGameActionViewModel
    {
        string Name { get; set; }
        string DisplayName { get; set; }
        Player Player { get; set; }
        void Execute();
        bool IsEnabled { get; set; }
        IReactiveCommand ActionCommand { get; set; }
    }

    //public abstract class GameActionViewModel : ReactiveObject, IGameActionViewModel
    //{
    //    public string Name { get; set; }
    //    public string DisplayName { get; set; }
    //    public Player Player { get; set; }
    //    public bool IsEnabled { get; set; }
    //    public abstract void Execute();

    //    public IReactiveCommand ActionCommand { get; set; }
    //}
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

    public class ScoreGamePointActionViewModel : ReactiveObject, IGameActionViewModel
    {
        public PointReason PointReason { get; set; }
        public bool ScoreForOpponent { get; set; }


        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Player Player { get; set; }

        public void Execute()
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled { get; set; }
        public IReactiveCommand ActionCommand { get; set; }
    }

    public class NonScoreGameActionViewModel : ReactiveObject, IGameActionViewModel
    {
        public NonScoreActionReason NonScoreActionReason { get; set; }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Player Player { get; set; }

        public void Execute()
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled { get; set; }
        public IReactiveCommand ActionCommand { get; set; }
    }

    public class ScorePoint : IGameActionViewModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public PointReason PointReason { get; set; }
        public Player Player { get; set; }
        public void Execute()
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled { get; set; }

        public IReactiveCommand ActionCommand { get; set; }
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


    public enum StatDescription
    {
        ForeHandWinner,
        BackHandWinner,
        VolleyWinner,
        DropShotWinner,
        OverheadWinner,
        UnforcedForehandError,
        UnforcedBackhadError,
        UnforcedVolleyError,
        ForcedError,
        FirstServeIn,
        FirstServeOut,
        SecondServeIn,
        DoubleFault,
        GamePoint,
        BreakPoint,
        GameOver,
        FirstServeAce,
        SecondServeAce
    }

    public enum StatName
    {
        [Display(Name = "1st Serve %")]
        FirstServePercentage,

        [Display(Name = "Aces")]
        Aces,

        [Display(Name = "Double Faults")]
        DoubleFaults,

        [Display(Name = "Win % for 1st Serve")]
        WinPercentForFirstServe,

        [Display(Name = "Win % for 2nd Serve")]
        WinPercentForSecondServe,

        [Display(Name = "Winners")]
        Winners,

        [Display(Name = "Unforced Errors")]
        UnforcedErrors
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