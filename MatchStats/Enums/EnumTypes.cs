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
        [Display(Name = "Short Set to Four")] ShortSetToFour = 4,
        [Display(Name = "Normal Set to Six")] LongSetSix = 6
    }

    public enum DeuceFormat
    {
        [Display(Name = "Normal Deuce lead by two")] Normal,
        [Display(Name = "Sudden death Deuce")] SuddenDeath
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

    public enum Rating
    {
        [Display(Name = "3.1")] ThreeOne,
        [Display(Name = "3.2")] ThreeTwo,
        [Display(Name = "4.1")] FourOne,
        [Display(Name = "4.2")] FourTwo,
        [Display(Name = "5.1")] FiveOne,
        [Display(Name = "5.2")] FiveTwo,
        [Display(Name = "6.1")] SixOne,
        [Display(Name = "6.2")] SixTwo,
        [Display(Name = "7.1")] SevenOne,
        [Display(Name = "7.2")] SevenTwo,
        [Display(Name = "8.1")] EightOne,
        [Display(Name = "8.2")] EightTwo,
        [Display(Name = "9.1")] NineOne,
        [Display(Name = "9.2")] NineTwo,
        [Display(Name = "10.1")] TenOne,
        [Display(Name = "10.2")] TenTwo,
    }
}