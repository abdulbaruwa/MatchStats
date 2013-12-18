using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using MatchStats.DesignTimeStuff;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.Test.ViewModels
{
    public class FakeMatchStatsApi : IMatchStatsApi
    {
        public void SaveMatchStats(IEnumerable<MyMatchStats> matchStats)
        {
            
        }

        public void SaveMatchStats(List<MyMatchStats> matchStats)
        {
            throw new NotImplementedException();
        }

        public IObservable<Unit> SaveMatch(Match match)
        {
            throw new NotImplementedException();
        }

        IObservable<List<MyMatchStats>> IMatchStatsApi.FetchMatchStats()
        {
            var outputList = new List<MyMatchStats>();
            outputList.AddRange(new DummyDataBuilder().BuildMatchStatsForDesignTimeView());
            //create Cold stream observable from output list. We need the whole list as an observable out put not the items in the list
            IObservable<List<MyMatchStats>> observable = Observable.Create<List<MyMatchStats>>(o =>
            {
                o.OnNext(outputList);
                o.OnCompleted();
                return () => { };
            });

            return observable;
        }

        public IObservable<Match> ExecuteActionCommand(ICommand command)
        {
            throw new NotImplementedException();
        }

        public IObservable<Match> GetCurrentMatch()
        {
            throw new NotImplementedException();
        }

        public IObservable<MyMatchStats> FetchMatchStats()
        {
            var outputList = new ReactiveList<MyMatchStats>();
            outputList.AddRange(new DummyDataBuilder().BuildMatchStatsForDesignTimeView());
            return outputList.ToObservable();
        }
    }
}