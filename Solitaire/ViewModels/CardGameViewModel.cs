using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Solitaire.Models;
using Solitaire.Utils;
using Solitaire.ViewModels.Pages;

namespace Solitaire.ViewModels;

public abstract partial class CardGameViewModel: ViewModelBase
{
    public ImmutableArray<PlayingCardViewModel>? Deck;
    
    public ICommand AutoMoveCommand { get; protected set; }

    public readonly Stack<CardOperation[]> _moveStack = new();
    
    public abstract string? GameName { get; }

    /// <summary>
    /// Уничтожаем записанные ходы
    /// </summary>
    private void ClearUndoStack()
    {
        _moveStack.Clear();
    }

    /// <summary>
    /// Записываем ход
    /// </summary>
    protected void RecordMoves(params CardOperation[] operations)
    {
        _moveStack.Push(operations);
    }

    /// <summary>
    /// Отменяем ход
    /// </summary>
    private void UndoMove()
    {
        if (_moveStack.Count > 0)
        {
            var operations = _moveStack.Pop();
            
            foreach(var operation in operations){
                operations.Revert(this);
            }
        }
    }




    protected CardGameViewModel(CasinoViewModel casinoViewModel)
    {
        NavigateToCasinoCommand = new RelayCommand(() =>
        {
            if (Moves > 0)
            {
                _gameStats.UpdateStatistics();
                casinoViewModel.Save();
            }

            casinoViewModel.CurrentView = casinoViewModel.TitleInstance;
        });

        UndoCommand = new RelayCommand(UndoMove);

        DoInitialize();
    }

    private void DoInitialize()
    {
        _timer.Interval = TimeSpan.FromSeconds(500);
        _timer.Tick += timer_Tick;
        InitializeDeck();
    }

    protected virtual void InitializeDeck()
    {
        if (Deck is { }) return;

        var playingCards = Enum
            .GetValuesAsUnderlyingType(typeof(CardType))
            .Cast<CardType>()
            .Select(cardType => new PlayingCardViewModel(this)
            {
                CardType = cardType, IsFaceDown = true
            })
            .ToImmutableArray();

        Deck = playingCards;
    }

    protected IList<PlayingCardViewModel> GetNewShuffledDeck()
    {
        foreach (var card in Deck!)
        {
            card.Reset();
        }

        var playingCards = Deck.Value.OrderBy(_ => PlatformProviders.NextRandomDouble()).ToList();
        
        return playingCards.Count == 0
            ? throw new InvalidOperationException("No playing cards have been played.")
            : playingCards;
    }

    public abstract IList<PlayingCardViewModel> GetCardCollection(PlayingCardViewModel card);

    public abstract bool CheckAndMoveCard(
        IList<PlayingCardViewModel> from,
        PlayingCardViewModel card,
        bool checkOnly = false);

    /// <summary>
    /// начинаем новую игру
    /// </summary>
    protected void ResetInternalState()
    {
        ClearUndoStack();

        StopTimer();
        ElapsedTime = TimeSpan.FromSeconds(0);
        Moves = 0;
        Score = 0;
        IsGameWon = false;
        OnPropertyChanged(nameof(IsGameWon));
    }

    protected void StartTimer()
    {
        _lastTick = DateTime.Now;
        _timer.Start();
    }

    protected void StopTimer()
    {
        _timer.Stop();
    }


    private void timer_Tick(object? sender, EventArgs e)
    {
        var timeNow = DateTime.Now;
        ElapsedTime += timeNow - _lastTick;
        _lastTick = timeNow;
    }
    

    /// <summary>
    /// Вызывает событие окончания игры
    /// </summary>
    protected void FireFameWonEvent()
    {
        _gameStats.UpdateStatistics();

        var wonEvent = GameWon;
        if (wonEvent is not { })
        {
            wonEvent?.Invoke();
        }
    }


    private readonly DispatcherTimer _timer = new();
    
    private DateTime _lastTick;

    [ObservableProperty] private int _score;
    [ObservableProperty] private TimeSpan _elapsedTime;
    [ObservableProperty] private int _moves;
    [ObservableProperty] private bool _isGameWon;

    private GameStatisticsViewModel _gameStats = null;
    
    
    public ICommand? NavigateToCasinoCommand { get; }

    public ICommand? NewGameCommand { get; protected set; }
    
    public ICommand? UndoCommand { get; protected set; }

    /// <summary>
    /// Случается когда игра выиграна
    /// </summary>
    public event Action GameWon = null;

    public void RegisterStatsInstance(GameStatisticsViewModel gameStatsInstance)
    {
        _gameStats = gameStatsInstance;
    }
    

    public abstract class CardOperation
    {
        public abstract void Revert(CardGameViewModel game);
    }

    public class GenericOperation : CardOperation
    {
        private readonly Action _action;

        public GenericOperation(Action action)
        {
            _action = action;
        }

        public override void Revert(CardGameViewModel game)
        {
            _action();
        }
    }
    

    public class MoveOperation : CardOperation
    {
        public MoveOperation(IList<PlayingCardViewModel> from, IList<PlayingCardViewModel> to,
            IList<PlayingCardViewModel> run, int score)
        {
            From = from;
            To = to;
            Run = run;
            Score = score;
        }
        
        /// <summary>
        /// Колода откуда брали карты
        /// </summary>
        public IList<PlayingCardViewModel> From { get; }
        
        /// <summary>
        /// Колода куда переместили карты
        /// </summary>
        public IList<PlayingCardViewModel> To { get; }
        
        /// <summary>
        /// Список карт которые перемещались
        /// </summary>
        public IList<PlayingCardViewModel> Run { get; }
        
        /// <summary>
        /// Очки, полученные за ход
        /// </summary>
        public int Score { get; }

        public override void Revert(CardGameViewModel game)
        {
            game.Score -= Score;
            
            foreach(var runCard in Run)
                From.Add(runCard);
            foreach (var runCard in From)
                To.Remove(runCard);

            game.Moves--;
        }
    }
}