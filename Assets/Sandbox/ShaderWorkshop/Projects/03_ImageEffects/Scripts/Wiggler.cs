using UnityEngine;
using System.Collections;

public class Wiggler : MonoBehaviour {
    [Header("Position wiggler")]
    public Vector3 posWiggleAmp = Vector3.one;
    public float posWiggheFreq = 0.1f;
    [Range(0f, 1f)] public float posSeed = 0.0f;

    [Header("Rotation wiggler")]
    public Vector3 rotWiggleAmp = Vector3.one;
    public float rotWiggleFreq = 0.1f;
    [Range(0f, 1f)] public float rotSeed = 0.0f;

    Vector3 m_localPos0;
    Quaternion m_localRot0;

	void Start()
    {
        m_localPos0 = transform.localPosition;
        m_localRot0 = transform.localRotation;

    }
	
	void Update () {
        transform.localPosition = m_localPos0;
        transform.localRotation = m_localRot0;

        Vector3 posWiggleVec = 2.0f * NoiseVector(posWiggheFreq * Time.time, posSeed) - Vector3.one;
        posWiggleVec = MulEachComp(posWiggleVec, posWiggleAmp);

        Vector3 rotWiggleVec = 2.0f * NoiseVector(rotWiggleFreq * Time.time, rotSeed) - Vector3.one;
        rotWiggleVec = MulEachComp(rotWiggleVec, rotWiggleAmp);

        transform.localPosition += posWiggleVec;
        transform.localEulerAngles += rotWiggleVec;
    }

    Vector3 NoiseVector(float x, float y, float xOffset = 0.25f, float yOffset = 0.25f)
    {
        return new Vector3(
            Mathf.PerlinNoise(x, y),
            Mathf.PerlinNoise(x + xOffset, y + yOffset),
            Mathf.PerlinNoise(x + 2.0f * xOffset, y + 2.0f * yOffset)
        );
    }

    Vector3 MulEachComp(Vector3 v0, Vector3 v1)
    {
        return new Vector3(
            v0.x * v1.x,
            v0.y * v1.y,
            v0.z * v1.z
        );
    }
}
