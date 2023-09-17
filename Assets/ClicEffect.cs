using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClicEffect : MonoBehaviour
{
    static ClicEffect instance;
    

    void Awake()
    {

        //TODO : Improve this "unique instance" system
        /*
        if (instance != null)
        {
            instance.GetComponent<ParticleSystem>().Stop();
            Destroy(instance.gameObject);
            Destroy(this.gameObject);


            return;
        }

        instance = this;
        */
    }
    
}
