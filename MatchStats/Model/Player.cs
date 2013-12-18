namespace MatchStats.Model
{
    public class Player
    {
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string FullName
        {
            get { return FirstName + " " + SurName; }
        }
        public string Rating { get; set; }
        public bool IsPlayerOne { get; set; }
    }
}