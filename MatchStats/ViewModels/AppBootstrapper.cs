﻿using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using MatchStats.Views;
using ReactiveUI;
using ReactiveUI.Mobile;

namespace MatchStats.ViewModels
{
    [DataContract]
    public class AppBootstrapper : ReactiveObject, IApplicationRootState
    {

        [DataMember]private RoutingState _router;
        public IRoutingState Router
        {
            get { return _router; }
            set { _router = (RoutingState) value; }
        }

        public AppBootstrapper()
        {
            Router = new RoutingState();
            var resolver = RxApp.MutableResolver;

            resolver.Register(() => new MyMatchStatsView(), typeof(IViewFor<MyMatchStatsViewModel>), "FullScreenLandscape");
            resolver.Register(() => new MyMatchStatsViewModel(), typeof(MyMatchStatsViewModel));

            resolver.Register(() => new MatchScoreView(), typeof(IViewFor<MatchScoreViewModel>), "FullScreenLandscape");
            resolver.Register(() => new MatchScoreViewModel(), typeof(MatchScoreViewModel));

            resolver.RegisterConstant(this, typeof(IApplicationRootState));
            resolver.RegisterConstant(this,typeof(IScreen));
            resolver.RegisterConstant(new MainPage(), typeof(IViewFor), "InitialPage");

            Router.Navigate.Execute(new MyMatchStatsViewModel(this));
        }
    }
}