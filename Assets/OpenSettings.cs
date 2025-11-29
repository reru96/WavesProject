using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class OpenSettings : MonoBehaviour
{

    public Button button;
    public CanvasGroup canvas;

    public void OpenCanvas()
    {
      canvas.alpha = 1;
      canvas.blocksRaycasts = true;
      canvas.interactable = true;
    }

}
