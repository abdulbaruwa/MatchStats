﻿using System;
using ReactiveUI;
using Windows.UI.Xaml;
using MatchStats.ViewModels;
using Windows.UI.Xaml.Controls;

namespace MatchStats.Views
{
    public sealed partial class MatchesPlayedView : IViewFor<MatchesPlayedViewModel>
    {
        public MatchesPlayedView()
        {
            this.InitializeComponent();
            this.BindCommand(ViewModel, x => x.AddMatch);
            this.Bind(ViewModel, x => x.MyMatchStats, x => x.itemGridView.ItemsSource);
            this.Bind(ViewModel, x => x.ShowNewMatchPopup, x => x.AddMatchDialog.IsOpen);
            itemGridView.Events().ItemClick.Subscribe(x => ViewModel.SelectedMatchStat = x.ClickedItem);
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MatchesPlayedViewModel) value; }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof (MatchesPlayedViewModel), typeof (MatchesPlayedView), new PropertyMetadata(null));

        public MatchesPlayedViewModel ViewModel
        {
            get { return (MatchesPlayedViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
} 