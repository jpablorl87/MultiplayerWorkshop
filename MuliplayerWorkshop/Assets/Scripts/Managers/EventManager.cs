using System;
using UnityEditor;
using UnityEngine;

public static class EventManager
{
    //Action<Int> have the ID of the player
    public static event Action<int> OnPlayerJump;
    public static event Action<int> OnPlayerSlide;
    public static event Action<int> OnPlayerDied;
    //Action<Int> have the ID of the player who make points
    public static event Action<int> OnScoreIncreased;
    //Game flow
    public static event Action<int> OnGameStart;
    public static event Action<int> OnGameOver;
    //Audio
    public static event Action<string> OnPlaySound;
    //Action triggers
    public static void TriggerPlayerJump(int playerId) => OnPlayerJump?.Invoke(playerId);
    public static void TriggerPlayerSlide(int playerId) => OnPlayerSlide?.Invoke(playerId);
    public static void TriggerPlayerDied(int playerId) => OnPlayerDied?.Invoke(playerId);
    //Score Trigger
    public static void TriggerScoreIncreased(int playerId) => OnScoreIncreased?.Invoke(playerId);
    //Game flow triggers
    public static void TriggerGameStart(int playerId) => OnGameStart?.Invoke(playerId);
    public static void TriggerGameOver(int playerId) => OnGameOver?.Invoke(playerId);
    //Audio trigger
    public static void TriggerSound(string soundName) => OnPlaySound?.Invoke(soundName);
}
