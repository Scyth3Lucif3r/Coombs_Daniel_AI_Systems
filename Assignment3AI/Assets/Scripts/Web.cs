using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Web : SoundObject
{
    public Material WebMaterial;
    public Material WebMaterialSprung;

    MeshRenderer meshRenderer;

    Character character;

    public override void Awake()
    {
        base.Awake();

        character = GetComponent<Character>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        other.GetComponent<Character>();
        if (character != null)
        {
            //characters speed should be slowed when walking through webs.
            character.moveSpeed = 4;
        }

        meshRenderer.material = WebMaterialSprung;
    }

    public void OnTriggerExit(Collider other)
    {
        meshRenderer.material = WebMaterial;
    }

}
