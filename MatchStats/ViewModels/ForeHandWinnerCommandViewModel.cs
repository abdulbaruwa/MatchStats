using System;
using MatchStats.Model;

namespace MatchStats.ViewModels
{
    public class ForeHandWinnerCommandViewModel : GameActionViewModel
    {
        public ForeHandWinnerCommandViewModel(Player player = null)
        {
            Player = player ?? new Player();
        }
        
            public new string Name
        {
            get
            {
                return "ForeHandWinner";
            }
        }
        public new string DisplayName
        {
            get
            {
                return "Fore Hand Winner";
            }
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}