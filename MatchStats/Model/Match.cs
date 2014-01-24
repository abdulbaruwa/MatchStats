using System;
using System.Collections.Generic;
using System.Linq;

namespace MatchStats.Model
{
    public class Match
    {

        public Match()
        {
            MatchStats = new List<MatchStat>();
        }
        public Guid MatchGuid { get; set; }
        public DateTime MatchTime { get; set; }
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
        public Tournament Tournament { get; set; }
        public MatchFormat MatchFormat { get; set; }
        public Score Score { get; set; }
        public List<MatchStat> MatchStats { get; set; }

        public int Duration
        {
            get
            {
                if (Score == null) return 0;
                return Score.Sets.Sum(x => x.DurationInMinutes);
            }

        }
    }
}
