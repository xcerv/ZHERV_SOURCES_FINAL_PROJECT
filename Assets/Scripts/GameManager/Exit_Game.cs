using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit_Game : MonoBehaviour
{
    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;// Quitting in Unity Editor: 
        #elif UNITY_WEBPLAYER || UNITY_WEBGL
                //Application.OpenURL(Application.absoluteURL);
                Application.ExternalEval("window.open('" + Application.absoluteURL + "','_self')");
        #else // !UNITY_WEBPLAYER
                Application.Quit(); // Quitting in all other builds: 
        #endif
    }

    public void ExitGameDelay(float delay)
    {
        Invoke(nameof(ExitGame), delay);
    }
}
