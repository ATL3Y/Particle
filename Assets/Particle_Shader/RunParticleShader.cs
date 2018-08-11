using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunParticleShader : MonoBehaviour
{
    private Material mat;
    private Mesh mesh;

    // Use this for initialization
    void Start ( )
    {
        mat = GetComponent<Renderer> ( ).material;
        mesh = GetComponent<MeshFilter> ( ).mesh;
        mesh.SetIndices ( mesh.GetIndices(0), MeshTopology.Points, 0 );
    }
}
