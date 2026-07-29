using Survivor.Player;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerController m_PlayerController;
    [SerializeField] private RectTransform    m_GameOverContainer;
    [SerializeField] private TextMeshProUGUI  m_GameOverText;

    private void Update()
    {
        if(GameController.IsGameMode(GameController.GameMode.Finished))
        {
            if(GameController.IsNewMode())
            {
                m_GameOverContainer.gameObject.SetActive(true);

                if(m_PlayerController.Health.Current <= 0.0f)
                {
                    m_GameOverText.color = Color.red;
                }
                else if(m_PlayerController.Hunger.Current <= 0.0f)
                {
                    m_GameOverText.color = Color.yellow;
                }
                else
                {
                    //
                    // NOTE:
                    // Uhm.. This branch should not be reachable, but it's unclear to me if we should
                    // check these death conditions here... we can, but it's fragile and doesn't scale
                    // super well. It's the simplest approach and should be fine for this use case.
                    // We're also mirroring the logic from the player code :/
                    //

                    Debug.Assert("Why did you die?" == null);
                }
            }
        }
        else if(GameController.IsNewMode())
        {
            m_GameOverContainer.gameObject.SetActive(false);
        }
    }
}
