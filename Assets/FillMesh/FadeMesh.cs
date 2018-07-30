using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMesh : MonoBehaviour
{
    [SerializeField]
    private float fadeAmount = 0.0f;

    private Transform skin;
    private Transform wireframe;
    private Transform particles;
    [SerializeField]
    private Material wireframeMat;
    [SerializeField]
    private GameObject psPrefab;

    private void Start ( )
    {
        skin = this.transform;
        wireframe = GameObject.Instantiate ( this.gameObject ).transform;
        wireframe.gameObject.GetComponent<FadeMesh> ( ).enabled = false; // HACKL3Y
        wireframe.SetParent ( this.transform.parent );
        SetWireFrame ( wireframe, wireframeMat );
        SetParticles ( skin );
        
    }
    private void Update ( )
    {
        SetAlpha ( skin, fadeAmount );
        SetAlpha ( wireframe, 1.0f - fadeAmount );
    }

    private void SetAlpha ( Transform t, float value )
    {
        Renderer renderer = t.GetComponent< Renderer > ();
        if ( renderer != null )
        {
            if ( renderer.material.HasProperty ( "_Color" ) )
            {
                Color tempColor = renderer.material.color;
                tempColor.a = value;
                renderer.material.color = tempColor;
            }
        }
    }

    private void SetWireFrame ( Transform t, Material mat )
    {
        t.GetComponent<Renderer> ( ).material = mat;
        MeshFilter filter = t.GetComponent< MeshFilter > ();
        if ( filter != null )
        {
            Mesh tempMesh = (Mesh)Instantiate( filter.mesh );
            tempMesh.SetIndices ( filter.mesh.GetIndices ( 0 ), MeshTopology.Lines, 0 );
            filter.mesh = tempMesh;
        }
    }

    private void SetParticles ( Transform t )
    {
        particles = new GameObject ( ).transform;
        particles.name = "particles";
        particles.SetParent ( t.parent );
        particles.localPosition = t.localPosition;
        particles.localRotation = t.localRotation;
        particles.localScale = t.localScale;

        MeshFilter filter = t.GetComponent< MeshFilter > ();
        if ( filter != null )
        {
            Vector3 [] verts = filter.mesh.vertices;
            Vector3[] nors = filter.mesh.normals;
            for(int i=0; i < verts.Length; i++ )
            {
                GameObject temp = GameObject.Instantiate(psPrefab);
                temp.transform.SetParent ( particles );
                temp.transform.localPosition = verts[i];
                temp.transform.localRotation *= Quaternion.LookRotation ( -nors[i] );
            }

        }
        
    }
}
