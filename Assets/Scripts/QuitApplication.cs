using UnityEngine;
using System.Collections;

public class QuitApplication : MonoBehaviour
{

    public void Quit()
    {

        //Quit the application
        Application.Quit();

        //Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;

    }
}
