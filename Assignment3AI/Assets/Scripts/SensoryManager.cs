using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensoryManager : MonoBehaviour
{
    public StateMachineSimple npc1;
    public StateMachineSimple npc2;
    public GameObject webTriggers;

    private void Awake()
    {
        
        foreach (Transform child in webTriggers.transform)
        {
            SoundObject soundObject = child.GetComponent<SoundObject>();
            if (soundObject != null)
            {
                soundObject.OnSoundTriggered.AddListener(npc1.SoundTrigger);
                soundObject.OnSoundTriggered.AddListener(npc2.SoundTrigger);
            }
        }
    }
}
