using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Web : SoundObject
{
    public Material WebMaterial;
    public Material WebMaterialSprung;

    MeshRenderer meshRenderer;

    public override void Awake()
    {
        base.Awake();

        meshRenderer = GetComponent<MeshRenderer>();
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        meshRenderer.material = WebMaterialSprung;
    }

    public void OnTriggerExit(Collider other)
    {
        meshRenderer.material = WebMaterial;
    }

}
