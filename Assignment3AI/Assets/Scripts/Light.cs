using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Light : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {

        StateMachineSimple npc = other.GetComponent<StateMachineSimple>();

        if (npc != null )
        {
            npc.RunAway();
        }

    }
}
