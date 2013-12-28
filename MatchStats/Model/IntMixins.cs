using System;

namespace MatchStats.Model
{
    public static class IntMixins
    {
        public static int DiffValueWith(this int lhs, int rhs)
        {
            return Math.Abs(lhs - rhs);
        }
    }
}