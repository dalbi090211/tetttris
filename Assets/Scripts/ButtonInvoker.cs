using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonInvoker : MonoBehaviour
{
    public void StartGame(){
        SceneManager.LoadScene("Play");
        // SoundManager.instance.createBGMInstance(FmodEvents.instance.TotalBGM);
    }

    public void ExitGame(){
        Application.Quit();
    }
}
