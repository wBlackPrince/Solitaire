using CommunityToolkit.Mvvm.ComponentModel;
using Solitaire.Models;

namespace Solitaire.ViewModels;

public partial class PlayingCardViewModel: ViewModelBase
{
    public CardGameViewModel? CardGameInstance { get; }

    public PlayingCardViewModel(CardGameViewModel? cardGameInstance)
    {
        CardGameInstance = cardGameInstance;
    }

    /// <summary>
    /// Возвращает тип карты
    /// </summary>
    public CardSuit Suit
    {
        get
        {
            var enumVal = (int)CardType;
            return enumVal switch
            {
                < 13 => CardSuit.Hearts,
                < 26 => CardSuit.Diamonds,
                < 39 => CardSuit.Clubs,
                _ => CardSuit.Spades,
            };
        }
    }

    /// <summary>
    /// Возвращает значение карты
    /// </summary>
    public int Value => (int)CardType % 13;

    /// <summary>
    /// Возвращает цвет карты
    /// </summary>
    public CardColour Colour => (int)CardType < 26 ? CardColour.Red : CardColour.Black;

    [ObservableProperty] private CardType _cardType = CardType.SA;
    [ObservableProperty] private bool _isFaceDown;
    [ObservableProperty] private bool _isPlayable;
    [ObservableProperty] private double _faceDownOffset;
    [ObservableProperty] private double _faceUpOffset;

    public void Reset()
    {
        IsPlayable = false;
        IsFaceDown = true;
        FaceDownOffset = 0;
        FaceUpOffset = 0;
    }
}