using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinRTXamlToolkit.Tools;

namespace MatchStats.Model
{
    public class Match
    {

        public Match()
        {
            MatchStats = new List<MatchStat>();
            Sets = new List<Set>();
        }
        public Guid MatchGuid { get; set; }
        public DateTime MatchTime { get; set; }
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
        public Tournament Tournament { get; set; }
        public MatchFormat MatchFormat { get; set; }
        public List<MatchStat> MatchStats { get; set; }
        public List<Set> Sets { get; set; }
        public Player CurrentServer { get; set; }
        public bool IsMatchOver { get; set; }
        public Player Winner { get; set; }
        public Status Status { get; set; }

        public string DisplayScore
        {
            get
            {
                var score = new StringBuilder();
                Sets.ForEach(x =>
                {
                    var playerOne = x.Games.Count(y => y.Winner != null && y.Winner.IsPlayerOne);
                    var playerTwo = x.Games.Count(y => y.Winner != null && !y.Winner.IsPlayerOne);
                    var space = score.Length > 1 ? " " : "";
                    if (Winner != null)
                    {
                        if (Winner.IsPlayerOne)
                        {
                            score.Append(space + playerOne.ToString() + "-" + playerTwo.ToString());
                        }
                        else
                            score.Append(space + playerTwo.ToString() + "-" + playerOne.ToString());
                    }

                });
                return score.ToString();
            }
        }



        public int Duration
        {
            get
            {
                if (Sets != null && Sets.Count <= 0) return 0;
                return Sets.Sum(x => x.DurationInMinutes);
            }

        }
    }
}
