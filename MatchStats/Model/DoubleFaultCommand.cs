using System;

namespace MatchStats.Model
{
    public class DoubleFaultCommand : GameAction
    {

        public DoubleFaultCommand(Player player = null)
        {
            Player = player ?? new Player();
        }

        public new string Name
        {
            get
            {
                return "DoubleFault";
            }
        }

        public new string DisplayName
        {
            get
            {
                return "Double Fault";
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