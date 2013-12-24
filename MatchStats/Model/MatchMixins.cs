using System;
using System.Linq;

namespace MatchStats.Model
{
    public static class MatchMixins
    {
        public static Game CurrentGame(this Match This)
        {
            var currentSet = This.CurrentSet();
            if (currentSet == null) return null;
            var currentGame = currentSet.Games.FirstOrDefault(x => x.IsCurrentGame);
            if (currentGame == null) return null;
            return currentGame;
        }

        public static Set CurrentSet(this Match This)
        {
            var currentSet = This.Score.Sets.FirstOrDefault(x => x.IsCurrentSet);
            if (currentSet == null) return null;
            return currentSet;
        }

        public static Game GetGameForKnownSetAndGame(this Match This, int setNumber, int gameNumber)
        {
            Game returnGame = null;
            var setCounter = 1;
            var gameCounter = 1;
            if(gameNumber < 1 || gameNumber > (int)This.MatchFormat.SetsFormat) throw new ArgumentException("Game number request is not valid for the selected Match Set Format");
            if(setNumber < 1) throw new ArgumentException("Sets number cannot be less than 1");
            if(setNumber > 3) throw new ArgumentException("Sets number cannot be more than 5");
            foreach (var set in This.Score.Sets)
            {
                if (setCounter == setNumber)
                {
                    foreach (var game in set.Games)
                    {
                        if (gameCounter == gameNumber)
                        {
                            returnGame = game;
                        }
                        gameCounter++;
                    }
                    setCounter++;
                }
            }
            return returnGame;
        }
    }
}