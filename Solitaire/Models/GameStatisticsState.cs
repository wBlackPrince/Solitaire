using System;

namespace Solitaire.Models;

public record GameStatisticsState(
    int GamesPlayed,
    int GameLost,
    int HighestWinningStreak,
    int HighestLosingStreak,
    int CurrentStreak,
    int CumulativeStreak,
    int HighestScore,
    double AverageScore,
    TimeSpan CumulativeGameTime,
    TimeSpan AverageGameTime);