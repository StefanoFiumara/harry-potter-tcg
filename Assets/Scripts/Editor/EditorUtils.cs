using System;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public static class EditorUtils
{
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

    public static string Dropdown(string label, string currentValue, string[] choices)
    {
        GUILayout.Label(label);

        int selected = 0;
        for (int j = 0; j < choices.Length; j++)
        {
            if (choices[j] == currentValue)
            {
                selected = j;
                break;
            }
        }

        selected = EditorGUILayout.Popup(selected, choices);
        return choices[selected];
    }
    
    
    public static int IndexOf(this string[] array, string value)
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