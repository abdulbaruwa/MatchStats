using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinRTXamlToolkit.Tools;

namespace MatchStats.Model
{
    //public class Score
    //{
    //    public Score()
    //    {
    //        Sets = new List<Set>();
    //    }

    //    public List<Set> Sets { get; set; }
    //    public Player CurrentServer { get; set; }
    //    public bool IsMatchOver { get; set; }
    //    public Player Winner { get; set; }
    //    public Status Status { get; set; }

    //    public string DisplayScore
    //    {
    //        get
    //        {
    //            var score = new StringBuilder();
    //            Sets.ForEach(x =>
    //            {
    //                var playerOne = x.Games.Count(y => y.Winner != null && y.Winner.IsPlayerOne);
    //                var playerTwo = x.Games.Count(y => y.Winner != null && ! y.Winner.IsPlayerOne);
    //                var space = score.Length > 1 ? " " : "";
    //                if (Winner != null)
    //                {
    //                    if (Winner.IsPlayerOne)
    //                    {
    //                        score.Append(space + playerOne.ToString() + "-" + playerTwo.ToString());
    //                    }
    //                    else
    //                        score.Append(space + playerTwo.ToString() + "-" + playerOne.ToString());
    //                }

    //            });
    //            return score.ToString();
    //        }
    //    }

    //}

    public class Set
    {
        public Set()
        {
            Games = new List<Game>();
            SetId = Guid.NewGuid().ToString();
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

    }
}
