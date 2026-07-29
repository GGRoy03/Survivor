using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void OnNewGameClicked()
    {
        GameController.PushGameMode(GameController.GameMode.Gameplay);
        GameController.SetScene(GameController.Scene.Game);
    }

    public void OnLoadGameClicked()
    {
        SaveSystem.LoadGameState();
        GameController.PushGameMode(GameController.GameMode.Gameplay);
        GameController.SetScene(GameController.Scene.Game);
    }

    public void OnQuitGameClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif // UNITY_EDITOR
    }
}