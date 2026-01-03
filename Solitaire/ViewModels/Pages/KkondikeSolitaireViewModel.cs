using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Solitaire.Models;

namespace Solitaire.ViewModels.Pages;

public partial class KkondikeSolitaireViewModel: CardGameViewModel
{
    public override string GameName => "Klondike";

    [ObservableProperty] private DrawMode _drawMode;
    private bool _isTurning;

    public KlondikeSolitaireViewModel(CasinoViewModel casinoViewModel) : base(casinoViewModel)
    {
        _casinoViewModel = casinoViewModel;
        InitializeFoundationsAndTableauSet();
    }

    private void InitializeFoundationsAndTableauSet()
    {
        
    }




    private void CheckForVictory()
    {
        foreach(var foundation in _foundations)
            if (foundation.Count < 13)
                return;

        IsGameWon = true;
        StopTimer();
        FireGameWonEvent();
    }
    
    
    
    // для легкого доступа мы имеем массивы
    
    private readonly List<BatchObservableCollection<PlayingCardViewModel>> _foundations = new();
    private readonly List<BatchObservableCollection<PlayingCardViewModel>> _tableauSet = new();
    private readonly CasinoViewModel _casinoViewModel;

    public BatchObservableCollection<PlayingCardViewModel> Foundation1 { get; } = new();

    public BatchObservableCollection<PlayingCardViewModel> Foundation2 { get; } = new();

    public BatchObservableCollection<PlayingCardViewModel> Foundation3 { get; } = new();

    public BatchObservableCollection<PlayingCardViewModel> Foundation4 { get; } = new();

    public BatchObservableCollection<PlayingCardViewModel> Tableau1 { get; } = new();

    public BatchObservableCollection<PlayingCardViewModel> Tableau2 { get; } = new();

    public BatchObservableCollection<PlayingCardViewModel> Tableau3 { get; } = new();

    public BatchObservableCollection<PlayingCardViewModel> Tableau4 { get; } = new();

    public BatchObservableCollection<PlayingCardViewModel> Tableau5 { get; } = new();

    public BatchObservableCollection<PlayingCardViewModel> Tableau6 { get; } = new();

    public BatchObservableCollection<PlayingCardViewModel> Tableau7 { get; } = new();

    public BatchObservableCollection<PlayingCardViewModel> Stock { get; } = new();

    public BatchObservableCollection<PlayingCardViewModel> Waste { get; } = new();
    
    public ICommand? TurnStockCommand { get; }
}