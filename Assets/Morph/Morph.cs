using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morph : MonoBehaviour {

    // Structured Buffer version vars.
    struct VectMatPair
    {
        public Vector3 point;
        public Matrix4x4 matrix;
    }

    VectMatPair[] data;
    VectMatPair[] output;
    ComputeBuffer buffer;

    Mesh mesh;
    Vector3[] vertices;

    private void Start ( )
    {
        
        InitStructuredBuffer ( );
        //InitRendTex ( );
    }

    private void Update ( )
    {
        UpdateStructuredBuffer ( );
        //UpdateRendTex ( );
    }

    void InitStructuredBuffer ( )
    {
        mesh = GetComponent<MeshFilter> ( ).mesh;


        // Initialize data.
        vertices = mesh.vertices;

        data = new Morph.VectMatPair [ vertices.Length ];
        output = new Morph.VectMatPair [ vertices.Length ];

        int i = 0;
        while ( i < vertices.Length )
        {
            data [ i ].point = vertices [ i ];
            data [ i ].matrix = transform.worldToLocalMatrix;

            output [ i ].point = vertices [ i ];
            output [ i ].matrix = transform.localToWorldMatrix;

            i++;
        }

        buffer = new ComputeBuffer ( vertices.Length, 76 );

        shader.SetFloat ( "_Wavelength", 0.01f );
    }

    void UpdateStructuredBuffer ( )
    {


        // Set shader properties. 
        float sinTime = Mathf.Sin(Time.timeSinceLevelLoad);
        shader.SetFloat ( "_SinTime", sinTime );

        // Run shader.
        buffer.SetData ( data );
        int kernel = shader.FindKernel("Multiply");
        shader.SetBuffer ( kernel, "dataBuffer", buffer );
        shader.Dispatch ( kernel, data.Length, 1, 1 );
        buffer.GetData ( output );

        // Write output to verts.
        int i = 0;
        while ( i < vertices.Length )
        {
            vertices [ i ] = transform.localRotation * output [ i ].point + transform.localPosition;
            i++;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds ( );
    }



    // Render texture version vars.
    public ComputeShader shader;
    private Material mat;
    RenderTexture tex;

    void InitRendTex ( )
    {
        mat = GetComponent<Renderer> ( ).material;
        tex = new RenderTexture ( 256, 256, 24 );
        tex.enableRandomWrite = true;
        tex.Create ( );
        mat.SetTexture ( "_MainTex", tex );
    }

    void UpdateRendTex ( )
    {
        // Set shader properties. 
        //float sinTime = Mathf.Sin(Time.timeSinceLevelLoad);
        //shader.SetFloat ( "_SinTime", sinTime );

        // Run shader.
        int kernelHandle = shader.FindKernel("CSMain");
        shader.SetTexture ( kernelHandle, "Result", tex );
        shader.Dispatch ( kernelHandle, 256 / 8, 256 / 8, 1 );
    }
}
