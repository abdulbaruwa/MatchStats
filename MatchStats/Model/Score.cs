using System.Collections.Generic;

namespace MatchStats.Model
{
    public class Score
    {
        public Score()
        {
            Games = new List<Game>();
        }

        public List<Game> Games { get; set; }
        public Game GameOne { get; set; }
        public Game GameTwo { get; set; }
        public Game GameThree { get; set; }
    }
}