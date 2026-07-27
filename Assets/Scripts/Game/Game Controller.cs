using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameController
{
    public enum GameMode
    {
        Gameplay  = 0,
        Inventory = 1,
        Dialogue  = 2,
        Paused    = 3,
    }

    public static GameMode Current { get; private set; } = GameMode.Gameplay;

    public static void SetGameMode(GameMode mode)
    {
        if(Current == GameMode.Paused)
        {
            Time.timeScale = 1.0f;
        }
        else if(mode == GameMode.Paused)
        {
            Time.timeScale = 0.0f;
        }

        Current = mode;
    }

    public enum Scene
    {
        Menu = 0,
        Game = 1,
    }

    private static string SceneToName(Scene scene)
    {
        string result = scene switch
        {
            Scene.Game => "GameScene",
            Scene.Menu => "MenuScene",
            _ => null,
        };

        return result;
    }

    public static void SetScene(Scene scene)
    {
        string sceneAsName = SceneToName(scene);
        if(sceneAsName != null)
        {
            SceneManager.LoadScene(sceneAsName);
        }
    }
}
