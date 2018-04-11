using UnityEngine;
using System.Collections;

public class BackRoundControl : MonoBehaviour {

    void Awake()
    {
        UnityEngine.Object.DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (InputDevice.SystemButton)
        {
            foreach (Transform child in GameRoot.uiOrthographicCamera.transform)
            {
                child.gameObject.SetActive(false);
            }
            foreach (Transform child in GameRoot.soundEffectPlayer.transform)
            {
                child.gameObject.SetActive(false);
            }
            MusicPlayer.activeMyListener = false;
            MusicPlayer.Stop(true);
            InputDevice.ShutDownIO();
            Application.LoadLevel("BackRoundSence");
        }
    }
}
