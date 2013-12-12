﻿using System;

namespace MatchStats.Model
{
    public class Match
    {
        //There is a duplication between this Entity and 'MyMatchStats'. Going forward I plan to use this one and depricate 'MyMatchStats' 
        //so will need to bring more properties into this as and when needed to support scoring and match history.
        public Guid MatchGuid { get; protected set; }
        public DateTime MatchTime { get; set; }
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
        public Tournament Tournament { get; set; }
        public MatchFormat MatchFormat { get; set; }
    }
}