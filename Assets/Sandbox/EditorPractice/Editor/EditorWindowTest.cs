using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections;

public class EditorWindowTest : EditorWindow {
    bool m_toggle = false;
    AnimFloat m_animFloat = new AnimFloat(0.0001f);
    Texture m_tex;

    [MenuItem("Window/Example")]
    static void Open()
    {
        GetWindow(typeof(EditorWindowTest), false, "My Empty Window");
    }

    void OnGUI()
    {
        bool on = (m_animFloat.value == 1.0f);

        if (GUILayout.Button(on ? "Close" : "Open", GUILayout.Width(64f)))
        {
            m_animFloat.target = on ? 0.0001f : 1.0f;
            m_animFloat.speed = 1.0f;
            
            var uniEvent = new UnityEvent();
            uniEvent.AddListener(() => Repaint());
            m_animFloat.valueChanged = uniEvent;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginFadeGroup(m_animFloat.value);
        Display();
        EditorGUILayout.EndFadeGroup();
        Display();
        EditorGUILayout.EndHorizontal();
    }

    void Display()
    {
        EditorGUILayout.BeginVertical();
        m_toggle = EditorGUILayout.ToggleLeft("Toggle", m_toggle);
        var options = new[] { GUILayout.Width(128f), GUILayout.Height(128f) };

        m_tex = EditorGUILayout.ObjectField(
                    m_tex, typeof(Texture), false, options
                ) as Texture;
        GUILayout.Button("Button");
        EditorGUILayout.EndVertical();
    }
}
