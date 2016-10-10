using UnityEngine;
using System.Collections;

public class RotateByTouch : MonoBehaviour {
    
    public float rotationSpeed = 30.0f;

    float h, v;
    Vector2 touchOrig, touchPos;
    Rigidbody m_rigidBody;

    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        var mainCam = Camera.main;

#if false//UNITY_STANDALONE || UNITY_EDITOR
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        float d = Mathf.Sqrt(h * h + v * v);
#else
        float d = 0.0f;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];

            switch (touch.phase)
            {
                // タッチ開始
                case TouchPhase.Began:
                    
                    break;
                // タッチ途中
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    touchOrig = touchPos;
                    touchPos = touch.position;
                    break;

                // タッチ終了
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    // touchPos = touchOrig;
                    break;
            }

            float ratio = Mathf.Min(Screen.width, Screen.height);
            h = (touchPos.x - touchOrig.x) / ratio;
            v = (touchPos.y - touchOrig.y) / ratio;
            d = (touchPos - touchOrig).magnitude / ratio;
            d /= Time.fixedDeltaTime; 

            Vector3 rotationAxis = (-h * mainCam.transform.up + v * mainCam.transform.right).normalized;
            m_rigidBody.angularVelocity = rotationAxis * d * Mathf.Deg2Rad * rotationSpeed;
        }
#endif
        
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0.0f, 0.0f, 100.0f, 50.0f), touchOrig.ToString());
        GUI.Label(new Rect(0.0f, 50.0f, 100.0f, 100.0f), touchPos.ToString());
    }
}
