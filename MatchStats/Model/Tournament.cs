using System;

namespace MatchStats.Model
{
    public class Tournament
    {
        public string TournamentName { get; set; }
        public string TournamentGrade { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Endate { get; set; }
    }
}