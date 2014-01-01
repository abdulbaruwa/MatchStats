using MatchStats.Enums;

namespace MatchStats.Model
{
    public class MatchFormat
    {
        public int Sets { get; set; }
        public SetsFormat SetsFormat { get; set; }
        public FinalSetFormats FinalSetType { get; set; }
        public DeuceFormat DeuceFormat { get; set; }
    }
}