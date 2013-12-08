using System.ComponentModel.DataAnnotations;

namespace MatchStats.Enums
{
    public enum FinalSetFormats
    {
        [Display(Name = "Normal")] Normal,
        [Display(Name = "10 Point Championship Tie Break")] TenPointChampionShipTieBreak
    }


    public enum SetsFormat
    {
        [Display(Name = "Short Set to Four")] ShortSetToFour,
        [Display(Name = "Normal Set to Six")] LongSetSix
    }

    public enum DueceFormat
    {
        [Display(Name = "Normal Dueuce lead by two")] Normal,
        [Display(Name = "Sudden death Dueuce first to two")] SuddenDeath
    }

    public enum AgeGroup
    {
        [Display(Name = "8 and Under")] U8,
        [Display(Name = "10 and Under")] U10,
        [Display(Name = "12 and Under")] U12,
        [Display(Name = "14 and Under")] U14,
        [Display(Name = "16 and Under")] U16,
        [Display(Name = "18 and Under")] U18,
        [Display(Name = "18 and Over")] Over18
    }

    public enum Grade
    {
        [Display(Name = "National - Grade 1")] National,
        [Display(Name = "Regional Grade 2")] Regional,
        [Display(Name = "National")] Grade3,
        [Display(Name = "Grade 4")] Grade4,
        [Display(Name = "Grade 5")] Grade5,
        [Display(Name = "Match Play")] Grade6
    }
}