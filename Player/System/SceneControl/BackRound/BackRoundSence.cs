using UnityEngine;
using System.Collections;

public class BackRoundSence : MonoBehaviour {

    void Start()
    {
        GameRoot.gameProcessControl.ActivateProcess(typeof(GameOptionsProcess));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
