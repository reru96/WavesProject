using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToMenu : MonoBehaviour
{
    public Button button;

    public void GoToMenuButton()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
