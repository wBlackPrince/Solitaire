namespace Solitaire.Models;

public record PersistenceState(
    SettingState Settings,
    GameStatisticsState KlondikeStateInstance,
    GameStatisticsState SpiderStatsInstance,
    GameStatisticsState FreeCellStatsInstance);