using UnityEngine;
using System.Collections;

public class JsonTester : MonoBehaviour {

    [System.Serializable]
    public class JsonTestClass
    {
        [SerializeField]
        string name = "hoge";

        [SerializeField]
        int number = 10;
    }


    void Start()
    {
        Debug.Log(JsonUtility.ToJson(new JsonTestClass(), true));
    }
}