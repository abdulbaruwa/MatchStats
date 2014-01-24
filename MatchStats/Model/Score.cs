using System;
using System.Collections.Generic;

namespace MatchStats.Model
{
    public class Score
    {
        public Score()
        {
            Sets = new List<Set>();
        }

        public List<Set> Sets { get; set; }
        public Game GameOne { get; set; }
        public Game GameTwo { get; set; }
        public Game GameThree { get; set; }
        public Player CurrentServer { get; set; }
        public bool IsMatchOver { get; set; }
        public Player Winner { get; set; }
        public Status Status { get; set; }
    }

    public class Set
    {
        public Set()
        {
            Games = new List<Game>();
        }
        public List<Game> Games { get; set; }
        public bool IsCurrentSet { get; set; }
        public Player Winner { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

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
