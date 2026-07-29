using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    //
    // Unity Hooks
    //

    [SerializeField] private RectTransform m_OverlayContainer;

    private void Update()
    {
        if(GameController.IsGameMode(GameController.GameMode.Paused))
        {
            m_OverlayContainer.gameObject.SetActive(true);
        }
        else
        {
            m_OverlayContainer.gameObject.SetActive(false);
        }
    }

    //
    // Event Hooks
    //

    public void OnResumeGameClicked()
    {
        if(GameController.IsGameMode(GameController.GameMode.Paused))
        {
            GameController.PopGameMode();
        }       
    }

    public void OnSaveGameClicked()
    {
        SaveSystem.SaveGameState();
    }

    public void OnQuitGameClicked()
    {
        GameController.SetScene(GameController.Scene.Menu);
    }
}
