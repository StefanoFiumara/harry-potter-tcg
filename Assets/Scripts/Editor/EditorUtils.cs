using System;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public static class E
{
    public static class Colors
    {
        public static readonly Color Success = new Color(137f/255f, 214f / 255f, 98f / 255f);
        public static readonly Color Error = new Color(1f, 91f / 255f, 91f / 255f);
        public static readonly Color Action = new Color(1f, 0.93f, 0.25f);
    }
    
    public static void DrawLine(Color color, int thickness = 2, int padding = 10)
    {
        var rect = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
        rect.height = thickness;
        rect.y += padding / 2;
        rect.x-=2;
        rect.width +=6;
        EditorGUI.DrawRect(rect, color);
    }

    public static void Button(string text, Color color, Action onClick, params GUILayoutOption[] options)
    {
        var prevColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        if (GUILayout.Button(text, options))
        {
            onClick();
        }
        GUI.backgroundColor = prevColor;
    }

    public static void CloseButton(Action onClick)
    {
        Button("X", Colors.Error, onClick, GUILayout.Width(20), GUILayout.Height(20));
    }
    
    public static void Dropdown(string label, ref string currentValue, string[] choices, Action<string, string> onChangedCallback = null)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label);

        int selected = choices.IndexOf(currentValue);
        selected = EditorGUILayout.Popup(selected, choices);
        
        GUILayout.EndHorizontal();

        string newValue = choices[selected];

        if (newValue != currentValue)
        {
            onChangedCallback?.Invoke(currentValue, newValue);
            currentValue = newValue;
        }
    }

    private static int IndexOf(this string[] array, string value)
    {
        int selected = 0;
        for (int j = 0; j < array.Length; j++)
        {
            if (array[j] == value)
            {
                selected = j;
                break;
            }
        }

        return selected;
    }
}