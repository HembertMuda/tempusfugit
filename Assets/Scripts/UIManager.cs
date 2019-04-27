using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text interactionText;

    void Start()
    {
        FindObjectOfType<Player>().onInteractionLayerChanged += ChangeInteractionText;
        GameManager.Instance.onGameStateChanged += HideInteractionText;
    }

    void ChangeInteractionText(int interactionLayer)
    {
        switch (interactionLayer)
        {
            case 9:
                interactionText.text = "Open / Close";
                break;
            case 10:
                interactionText.text = "Talk";
                break;
            default:
                interactionText.text = string.Empty;
                break;
        }
    }

    void HideInteractionText(GameManager.GameState newGameState)
    {
        ChangeInteractionText(0);
    }
}
