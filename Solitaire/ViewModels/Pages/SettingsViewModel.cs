using System;
using System.Windows.Input;
using Avalonia.Reactive;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using Solitaire.Models;

namespace Solitaire.ViewModels.Pages;

public partial class SettingsViewModel: ViewModelBase
{
    [ObservableProperty] private Difficulty _difficulty =  Difficulty.Easy;
    [ObservableProperty] private DrawMode _drawMode = Models.DrawMode.DrawOne;
    [ObservableProperty] private string? _drawModeText;
    [ObservableProperty] private string? _difficultyText;
    
    public ICommand NavigateToTitleCommand { get; }
    public ICommand DrawModeCommand { get; }
    public ICommand DifficultyCommand { get; }

    public SettingsViewModel(CasinoViewModel casinoViewModel)
    {
        var casinoViewModel1 = casinoViewModel;

        NavigateToTitleCommand = new RelayCommand(() =>
        {
            casinoViewModel1.CurrentView = casinoViewModel1.CurrentView;
            casinoViewModel1.Save();
        });

        DrawModeCommand = new RelayCommand(() =>
        {
            DrawMode = (DrawMode == DrawMode.DrawOne ? DrawMode.DrawThree : DrawMode.DrawOne);
        });

        DifficultyCommand = new RelayCommand(() =>
        {
            Difficulty = Difficulty switch
            {
                Difficulty.Easy => Difficulty.Medium,
                Difficulty.Medium => Difficulty.Hard,
                Difficulty.Hard => Difficulty.Easy,
                _ => throw new ArgumentOutOfRangeException()
            };
        });

        this
            .WhenAnyValue(x => x.DrawMode)
            .Subscribe(new AnonymousObserver<DrawMode>(diff =>
            {
                DrawModeText = $"{DrawMode.ToString()
                    .Replace("Draw", "")} Card{(DrawMode == DrawMode.DrawThree ? "s" : "")}";
            }));
        
        this
            .WhenAnyValue(x => x.Difficulty)
            .Subscribe(new AnonymousObserver<Difficulty>(diff =>
            {
                DrawModeText = $"{DrawMode.ToString()
                    .Replace("Draw", "")} Card{(DrawMode == DrawMode.DrawThree ? "s" : "")}";
            }));
    }

    public void ApplyState(SettingState state)
    {
        Difficulty = state.Difficulty;
        DrawMode = state.DrawMode;
    }

    public SettingState GetState()
    {
        return new SettingState(Difficulty, DrawMode);
    }
}