using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAnnouncement : MonoBehaviour
{
    private bool Started = false;
    private bool Finished = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Finished) return;
        var current = gameObject.transform.position;
        if (!Started)
        {
            gameObject.transform.position = new Vector3(Screen.currentResolution.width, current.y, current.z);
            Started = true;
            return;
        }

        if (current.x <= -0)
        {
            gameObject.SetActive(false);
            Finished = true;
        }

        gameObject.transform.position = new Vector3(current.x - 10, current.y, current.z);
    }
}
