using System;

namespace MatchStats.Model
{
    public class ForeHandWinnerCommand : GameActionViewModel
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

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}