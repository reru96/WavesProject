using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Retry : MonoBehaviour
{
    public Button button;

    public void GoToLevel1()
    {
        SceneManager.LoadScene("Level1");
    }
}
