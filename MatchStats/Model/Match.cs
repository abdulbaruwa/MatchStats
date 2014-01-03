using System;
using System.Collections.Generic;

namespace MatchStats.Model
{
    public class Match
    {

        public Match()
        {
            MatchStats = new List<MatchStat>();
        }
        //There is a duplication between this Entity and 'MyMatchStats'. Going forward I plan to use this one and depricate 'MyMatchStats' 
        //so will need to bring more properties into this as and when needed to support scoring and match history.
        public Guid MatchGuid { get; set; }
        public DateTime MatchTime { get; set; }
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
        public Tournament Tournament { get; set; }
        public MatchFormat MatchFormat { get; set; }
        public Score Score { get; set; }
        public List<MatchStat> MatchStats { get; set; }
    }
}