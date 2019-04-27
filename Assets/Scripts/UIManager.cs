using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text interactionText;

    void Start()
    {
        FindObjectOfType<Player>().onInteractionLayerChanged += ChangeInteractionText;
    }

    void ChangeInteractionText(int interactionLayer)
    {
        switch (interactionLayer)
        {
            case 9:
                interactionText.text = "Open / Close";
                break;
            default:
                interactionText.text = string.Empty;
                break;
        }
    }
}
