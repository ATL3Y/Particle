using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField]
    private float speed;

    void Update ( )
    {
        // transform.Rotate ( speed * Vector3.right * Time.deltaTime );
        float offset = (Time.timeSinceLevelLoad * 60f) % 360;
        float angle = -180f + offset;
        transform.rotation = Quaternion.AngleAxis ( speed * angle, Vector3.up );

    }
}
