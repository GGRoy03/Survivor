using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void OnNewGameClicked()
    {
        Debug.Log("New Game!");
    }

    public void OnLoadGameClicked()
    {
        Debug.Log("Load Game!");
    }

    public void OnQuitGameClicked()
    {
        Debug.Log("Quit Game!");
    }
}
