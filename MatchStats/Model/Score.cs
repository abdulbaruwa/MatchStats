using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinRTXamlToolkit.Tools;

namespace MatchStats.Model
{
    public class Set
    {
        public Set()
        {
            Games = new List<Game>();
            SetId = Guid.NewGuid().ToString();
            StartTime = DateTime.Now;
        }

        public List<Game> Games { get; set; }
        public bool IsCurrentSet { get; set; }
        public Player Winner { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string SetId { get; set; }
        public int DurationInMinutes
        {
            get
            {
                if (StartTime.HasValue && EndTime.HasValue)
                {
                    return EndTime.Value.Subtract(StartTime.Value).Minutes;
                }
                return 0;
            }
        }

        public void SetWonBy(Player player)
        {
            Winner = player;
            EndTime = DateTime.Now;
        }

    }
}
