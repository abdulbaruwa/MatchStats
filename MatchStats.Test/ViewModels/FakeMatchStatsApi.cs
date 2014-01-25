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
        public IObservable<Unit> SaveMatch(Match match)
        {
            throw new NotImplementedException();
        }

        IObservable<List<Match>> IMatchStatsApi.FetchMatchStats()
        {
            var outputList = new List<Match>();
            outputList.AddRange(new DummyDataBuilder().BuildMatchStatsForDesignTimeView());
            //create Cold stream observable from output list. We need the whole list as an observable out put not the items in the list
            IObservable<List<Match>> observable = Observable.Create<List<Match>>(o =>
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

        public Match ApplyGameRules(Match currentMatch)
        {
            throw new NotImplementedException();
        }

        public void SaveMatchStats(List<Match> matchStats)
        {
            throw new NotImplementedException();
        }

        public IObservable<Match> FetchMatchStats()
        {
            var outputList = new ReactiveList<Match>();
            outputList.AddRange(new DummyDataBuilder().BuildMatchStatsForDesignTimeView());
            return outputList.ToObservable();
        }
    }
}
