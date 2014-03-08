using MatchStats.Model;

namespace MatchStats.ViewModels
{

    public class VolleyWinnerCommandViewModel : BackHandWinnerCommandViewModel
    {
        public VolleyWinnerCommandViewModel(Player player = null) : base(player)
        {
            Name = "VolleyWinner";
            DisplayNameTop = "Volley";
            DisplayNameBottom = "Winner";
        }
    }

    public class DropShotWinnerCommandViewModel : BackHandWinnerCommandViewModel
    {
        public DropShotWinnerCommandViewModel(Player player = null) : base(player)
        {
            Name = "DropShotWinner";
            DisplayNameTop = "Dropshot";
            DisplayNameBottom = "Winner";
        }
    }

    public class OverHeadWinnerCommandViewModel : BackHandWinnerCommandViewModel
    {
        public OverHeadWinnerCommandViewModel(Player player = null) : base(player)
        {
            Name = "OverheadWinner";
            DisplayNameTop = "OverHead";
            DisplayNameBottom = "Winner";
        }
    }

    public class BackHandWinnerCommandViewModel : ForeHandWinnerCommandViewModel
    {
        public BackHandWinnerCommandViewModel(Player player = null): base(player)
        {
            Name = "BackHandWinner";
            DisplayNameTop = "Backhand";
            DisplayNameBottom = "Winner";
        }
    }
}