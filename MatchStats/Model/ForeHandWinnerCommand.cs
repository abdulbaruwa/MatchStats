using System;

namespace MatchStats.Model
{
    public class ForeHandWinnerCommand : GameAction
    {
        public ForeHandWinnerCommand(Player player = null)
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

        public override bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public override void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}