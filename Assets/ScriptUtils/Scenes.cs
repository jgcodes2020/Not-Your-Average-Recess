using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class managing scenes.
/// </summary>
public static class Scenes
{
    public const int TitleScreenID = 0;
    public const int GameLevelID = 1;
    public const int EndScreenID = 2;
    
    public static Scene TitleScreen => _titleScreen ??= SceneManager.GetSceneByBuildIndex(TitleScreenID);
    public static Scene GameLevel => _gameLevel ??= SceneManager.GetSceneByBuildIndex(GameLevelID);
    public static Scene EndScreen => _endScreen ??= SceneManager.GetSceneByBuildIndex(EndScreenID);

    private static Scene? _titleScreen;
    private static Scene? _gameLevel;
    private static Scene? _endScreen;

    public static IEnumerator ToGameLevel()
    {
        yield return SceneManager.LoadSceneAsync(GameLevelID);
        SceneManager.SetActiveScene(GameLevel);
    }

    public static IEnumerator ToTitleScreen()
    {
        Debug.Log("Going to Title Screen");
        yield return SceneManager.LoadSceneAsync(TitleScreenID);
        SceneManager.SetActiveScene(TitleScreen);
    }

    public static IEnumerator ToEndScreen()
    {
        yield return SceneManager.LoadSceneAsync(EndScreenID);
        SceneManager.SetActiveScene(EndScreen);
    }
}