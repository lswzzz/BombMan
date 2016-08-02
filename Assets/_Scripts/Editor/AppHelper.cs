using UnityEngine;
using System.Collections;
using UnityEditor;

namespace BombEditor
{
    public class AppHelper
    {
        [MenuItem("BombEditor/QuitGame")]
        public static void Quit()
        {
#if UNITY_EDITOR
            if (ConnectSocket.getSocketInstance().isConnect())
            {
                ConnectSocket.getSocketInstance().Close();
            }
            if (UDPSocket.Instance.isInit)
            {
                UDPSocket.Instance.Close();
            }
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
#else
         Application.Quit();
#endif
        }

        [MenuItem("BombEditor/PlayGame")]
        public static void Play()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = true;
#elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
#else
         Application.Play();
#endif
        }
    }
}
