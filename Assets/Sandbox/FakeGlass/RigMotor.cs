using UnityEngine;
using System.Collections;

public class RigMotor : MonoBehaviour {
    public float speed = 10.0f;
	void Update()
    {
        transform.Rotate(0.0f, speed * Time.deltaTime, 0.0f);
	}
}
