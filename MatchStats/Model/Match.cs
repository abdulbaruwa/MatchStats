using System;

namespace MatchStats.Model
{
    public class Match
    {
        public Guid MatchGuid { get; protected set; }
        public DateTime MatchTime { get; set; }
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
        public Tournament Tournament { get; set; }
        public MatchFormat MatchFormat { get; set; }
    }
}