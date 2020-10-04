using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class CreateAttributeWindow: EditorWindow
{
    private static Dictionary<string, Type> GetAttributeMap()
    {
        return
            AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(CardAttribute)))
                .ToDictionary(t => t.Name, t => t);
    }
    
    private static readonly Lazy<Dictionary<string, Type>> AttributeMapLoader = new Lazy<Dictionary<string, Type>>(GetAttributeMap);
    private static readonly Lazy<string[]> DropdownOptionsLoader = new Lazy<string[]>(() => new[] { "None" }
        .Concat(AttributeMap.Keys)
        .ToArray());
    
    private static Dictionary<string, Type> AttributeMap => AttributeMapLoader.Value;
    private string[] DropdownOptions => DropdownOptionsLoader.Value;
    
    
    private string _selectedAttribute = "None";

    private CardAttribute _attribute;
    private Action<CardAttribute> _callback;

    private bool _isValid;
    private bool _isInit;

    public void InitWindow(Action<CardAttribute> onCreatedCallback)
    {
        _isValid = true;
        _isInit = true;
        _callback = onCreatedCallback;
    }

    private void OnGUI()
    {
        if (!_isInit) return;
        
        GUILayout.BeginVertical();
        GUILayout.Label("Card Attribute Editor", EditorStyles.boldLabel);
        
        GUILayout.Space(15);
        
        E.Dropdown("Select Attribute", ref _selectedAttribute, DropdownOptions, (oldAttr, newAttr) =>
        {
            if (newAttr == "None")
            {
                _attribute = null;
            }
            else if (oldAttr != newAttr)
            {
                _attribute = (CardAttribute) CreateInstance(AttributeMap[newAttr]);
                if (_attribute is HarryPotter.Data.Cards.CardAttributes.Abilities.Ability a)
                {
                    a.Actions.Add(new ActionDefinition());
                }
            }
        });
        
        GUILayout.Space(10);

        if (_attribute != null)
        {
            var editor = Editor.CreateEditor(_attribute);
            
            if (editor is IEditable editable)
            {
                editable.IsEditMode = true;
            }
            if (editor is IValidator validator)
            {
                _isValid = validator.IsValid();
            }
            
            editor.OnInspectorGUI();
        }
        
        if (!_isValid || _attribute == null)
        {
            GUI.enabled = false;
        }
        
        E.Button("Create", E.Colors.Action, () =>
        {
            _callback.Invoke(_attribute);
            Close();
        });
        
        GUI.enabled = true;
        GUILayout.EndVertical();
    }
}