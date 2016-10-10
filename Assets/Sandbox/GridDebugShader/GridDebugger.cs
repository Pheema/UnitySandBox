using UnityEngine;
using System.Collections;

public class GridDebugger : MonoBehaviour {
    Material m_material;

    void Start () {
        m_material = GetComponent<MeshRenderer>().material;
	}
	
	void Update () {
        m_material.SetMatrix("_MainCameraVP", Camera.main.projectionMatrix * Camera.main.worldToCameraMatrix);
	}
}
