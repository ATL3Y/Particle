using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run_Particle_Storm : MonoBehaviour
{
    private Vector2 cursorPos;

    // struct
    struct Particle
    {
        public Vector3 position;
        public Vector3 velocity;
        public float life;
    }
    private const int SIZE_PARTICLE = 28;
    private int particleCount = 1000000;
    public Material material;
    public ComputeShader computeShader;
    private int mComputeShaderKernelID;
    ComputeBuffer particleBuffer;
    private const int WARP_SIZE = 256;
    private int mWarpCount;

    void Start ( )
    {
        InitComputeShader ( );
    }

    void InitComputeShader ( )
    {
        mWarpCount = Mathf.CeilToInt ( ( float ) particleCount / WARP_SIZE );

        // initialize the particles
        Particle[] particleArray = new Particle[particleCount];

        for ( int i = 0; i < particleCount; i++ )
        {
            float x = Random.value * 2 - 1.0f;
            float y = Random.value * 2 - 1.0f;
            float z = Random.value * 2 - 1.0f;
            Vector3 xyz = new Vector3(x, y, z);
            xyz.Normalize ( );
            xyz *= Random.value;
            xyz *= 0.5f;

            particleArray [ i ].position.x = xyz.x;
            particleArray [ i ].position.y = xyz.y;
            particleArray [ i ].position.z = xyz.z + 3;

            particleArray [ i ].velocity.x = 0;
            particleArray [ i ].velocity.y = 0;
            particleArray [ i ].velocity.z = 0;

            // Initial life value
            particleArray [ i ].life = Random.value * 5.0f + 1.0f;
        }

        // create compute buffer
        particleBuffer = new ComputeBuffer ( particleCount, SIZE_PARTICLE );

        particleBuffer.SetData ( particleArray );

        // find the id of the kernel
        mComputeShaderKernelID = computeShader.FindKernel ( "CSParticleStorm" );

        // bind the compute buffer to the shader and the compute shader
        computeShader.SetBuffer ( mComputeShaderKernelID, "particleStormBuffer", particleBuffer );
        material.SetBuffer ( "particleStormBuffer", particleBuffer );
    }

    void OnRenderObject ( )
    {
        material.SetPass ( 0 );
        //Graphics.DrawProcedural ( MeshTopology.Points, 1, particleCount );
        // TO DO: Draw quads somehow 
        Graphics.DrawProcedural ( MeshTopology.Triangles, particleCount);
    }

    void OnDestroy ( )
    {
        if ( particleBuffer != null )
        {
            particleBuffer.Release ( );
        }
    }

    void Update ( )
    {
        // TODO: Turn into tornado
        float[] mousePosition2D = { cursorPos.x, cursorPos.y };

        // Send datas to the compute shader
        computeShader.SetFloat ( "deltaTime", Time.deltaTime );
        computeShader.SetFloats ( "mousePosition", mousePosition2D );

        // Update the Particles
        computeShader.Dispatch ( mComputeShaderKernelID, mWarpCount, 1, 1 );
    }

    // Getting mouse position
    void OnGUI ( )
    {
        Vector3 p = new Vector3();
        Camera c = Camera.main;
        Event e = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = e.mousePosition.x;
        mousePos.y = c.pixelHeight - e.mousePosition.y;

        p = c.ScreenToWorldPoint ( new Vector3 ( mousePos.x, mousePos.y, c.nearClipPlane + 14 ) );// z = 3.

        cursorPos.x = p.x;
        cursorPos.y = p.y;
    }
}
