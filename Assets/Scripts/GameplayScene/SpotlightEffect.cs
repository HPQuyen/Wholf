using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class SpotlightEffect : MonoBehaviour
{

    private Animator anim = null;
    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    
    public void SwitchLight(bool state)
    {
        anim.SetBool("InMyTurn", state);
    }
    public void SeerDetection(bool state)
    {
        // Do something
        if(state)
        {
            anim.Play("WolfDetection", -1, 0f);
        }
        else
        {
            anim.Play("NotWolfDetection", -1, 0f);
        }
    }
}
