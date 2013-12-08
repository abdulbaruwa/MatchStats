using System;
using MatchStats.Enums;

namespace MatchStats.Model
{
    public class Tournament
    {
        public string TournamentName { get; set; }
        public string TournamentGrade { get; set; }
        public Grade Grade { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Endate { get; set; }
    }
}