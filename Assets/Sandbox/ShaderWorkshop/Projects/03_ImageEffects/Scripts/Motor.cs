using UnityEngine;
using System.Collections;

public class Motor : MonoBehaviour {
    public Space spaceType = Space.World;
    public float angVelocity = 10.0f;

    void Update ()
    {
        transform.Rotate(Vector3.up, angVelocity * Time.deltaTime, spaceType);
	}
}
