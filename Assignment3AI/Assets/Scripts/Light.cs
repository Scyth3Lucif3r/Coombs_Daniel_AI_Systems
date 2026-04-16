using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Light : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        
        StateMachineSimple npc = other.GetComponent<StateMachineSimple>();
        //If a spider walks into a safe zone, this will catch them and put
        //them into their flee state.
        if (npc != null )
        {
            npc.RunAway();
        }

    }
}
