using System;
using ReactiveUI;

namespace MatchStats.Model
{
    public class DoubleFaultCommand : GameActionViewModel
    {

        public DoubleFaultCommand(Player player = null)
        {
            Player = player ?? new Player();
            base.Name = "DoubleFault";
            base.DisplayName = "Double Fault";
            ActionCommand = new ReactiveCommand();
            ActionCommand.Subscribe(x => this.Execute());
        }

        public override void Execute()
        {
        }
    }
}