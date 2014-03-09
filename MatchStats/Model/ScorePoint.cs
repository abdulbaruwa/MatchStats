using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ReactiveUI;

namespace MatchStats.Model
{
    public interface IGameActionViewModel
    {
        string Name { get; set; }
        string DisplayNameTop { get; set; }
        string DisplayNameBottom { get; set; }
        Player Player { get; set; }
        void Execute();
        bool IsEnabled { get; set; }
        IReactiveCommand ActionCommand { get; set; }
    }

    public class ScoreGamePointActionViewModel : ReactiveObject, IGameActionViewModel
    {
        public PointReason PointReason { get; set; }
        public bool ScoreForOpponent { get; set; }


        public string Name { get; set; }
        public string DisplayNameTop { get; set; }
        public string DisplayNameBottom { get; set; }
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
        public string DisplayNameTop { get; set; }
        public string DisplayNameBottom { get; set; }
        public PointReason PointReason { get; set; }
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
        public string DisplayNameTop { get; set; }
        public string DisplayNameBottom { get; set; }
        public Player Player { get; set; }

        public void Execute()
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled { get; set; }
        public IReactiveCommand ActionCommand { get; set; }
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
        ForcedError,
        DoubleFault,
        SecondServeAce,
        FirstServeAce,
        UnforcedServeReturnError,
        ForcedServeReturnError
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
        UnforcedErrors,

        [Display(Name = "Forced Errors")]
        ForcedErrors,

        [Display(Name = "Forced Return Errors")]
        ForcedReturnErrors,

        [Display(Name = "UnForced Return Errors")]
        UnforcedReturnErrors
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
}