using System;
using System.Linq;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{

    public class VolleyWinnerCommandViewModel : BackHandWinnerCommandViewModel
    {
        public VolleyWinnerCommandViewModel(Player player = null) : base(player)
        {
            Name = "VolleyWinner";
            DisplayName = "Volley Winner";
        }
    }

    public class DropShotWinnerCommandViewModel : BackHandWinnerCommandViewModel
    {
        public DropShotWinnerCommandViewModel(Player player = null) : base(player)
        {
            Name = "DropShotWinner";
            DisplayName = "Drop Shot Winner";
        }
    }

    public class OverHeadWinnerCommandViewModel : BackHandWinnerCommandViewModel
    {
        public OverHeadWinnerCommandViewModel(Player player = null) : base(player)
        {
            Name = "OverheadWinner";
            DisplayName = "Over Head Winner";
        }
    }

    public class BackHandWinnerCommandViewModel : ForeHandWinnerCommandViewModel
    {
        public BackHandWinnerCommandViewModel(Player player = null): base(player)
        {
            Name = "BackHandWinner";
            DisplayName = "Back Hand Winner";
        }
    }
}