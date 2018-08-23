﻿using System;
using UnityEngine;
using UnityEditor;

public class Note
{
    /// <summary>
    /// The  noise function work is up front.
    /// 
    /// </summary>
    public NoiseFunction Noise { get; set; }
    public NoiseFilter[] NoiseFilters { get; set; }
    public Rect rect;
    public string title;
    public bool isDragged;
    public bool isSelected;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    


    public GUIStyle style;
    public GUIStyle defaultNoteStyle;
    public GUIStyle selectedNoteStyle;

    public Action<Note> OnRemoveNote;

    public Note(
        Vector2 position,
        float width,
        float height,
        GUIStyle noteStyle,
        GUIStyle selectedStyle,
        GUIStyle inPointStyle, 
        GUIStyle outPointStyle, 
        Action<ConnectionPoint> OnClickInPoint, 
        Action<ConnectionPoint> OnClickOutPoint,
        Action<Note> OnClickRemoveNote)
    {
        Noise = new NoiseFunction();
        NoiseFilters = null;

        rect = new Rect(position.x, position.y, width, height);
        style = noteStyle;
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);
        defaultNoteStyle = noteStyle;
        selectedNoteStyle = selectedStyle;
        OnRemoveNote = OnClickRemoveNote;
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public void Draw()
    {
        inPoint.Draw();
        outPoint.Draw();
        GUI.Box(rect, title, style);
        GetInspectorElements(Noise);
        
    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNoteStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNoteStyle;
                        
                    }
                }

                if(e.button ==1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }

                break;

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if(e.button==0&&isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }
        return false;
    }

    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove Node"), false, OnClickRemoveNote);
        genericMenu.ShowAsContext();
    }

    private void OnClickRemoveNote()
    {
        if (OnRemoveNote!=null)
        {
            OnRemoveNote(this);
        }
    }

    public void GetInspectorElements(NoiseFunction _Noise)
    {
        #region Perlin Function UI
        if (_Noise.type == NoiseFunction.NoiseType.Perlin)
        {

            string name = "Perlin Noise";
            
            GUILayout.Label(name);
            _Noise.type = (NoiseFunction.NoiseType)EditorGUILayout.EnumPopup("Type of Noise", _Noise.type);
            _Noise.enabled = EditorGUILayout.ToggleLeft("Enabled", _Noise.enabled);
            _Noise.frequency = (double)EditorGUILayout.Slider("Frequency", (float)_Noise.frequency, -20f, 20f);
            _Noise.lacunarity = (double)EditorGUILayout.Slider("Lacunarity", (float)_Noise.lacunarity, -2.0000000f, 2.5000000f);
            _Noise.persistence = (double)EditorGUILayout.Slider("Persistence", (float)_Noise.persistence, -1f, 1f);
            _Noise.octaves = EditorGUILayout.IntSlider("Octaves", _Noise.octaves, 0, 18);
            _Noise.qualityMode = (LibNoise.QualityMode)EditorGUILayout.EnumPopup("Quality Mode", _Noise.qualityMode);
            _Noise.blendMode = (NoiseFunction.BlendMode)EditorGUILayout.EnumPopup("Blend Mode", _Noise.blendMode);
        }
        #endregion

        #region Billow Function UI
        else if (_Noise.type == NoiseFunction.NoiseType.Billow)
        {

            string name = "Billow Noise";
            EditorGUILayout.LabelField(name);
            _Noise.type = (NoiseFunction.NoiseType)EditorGUILayout.EnumPopup("Type of Noise", _Noise.type);
            _Noise.enabled = EditorGUILayout.ToggleLeft("Enabled", _Noise.enabled);
            _Noise.frequency = (double)EditorGUILayout.Slider("Frequency", (float)_Noise.frequency, 0f, 20f);
            _Noise.lacunarity = (double)EditorGUILayout.Slider("Lacunarity", (float)_Noise.lacunarity, 1.5000000f, 3.5000000f);
            _Noise.persistence = (double)EditorGUILayout.Slider("Persistence", (float)_Noise.persistence, 0f, 1f);
            _Noise.octaves = EditorGUILayout.IntSlider("Octaves", _Noise.octaves, 0, 18);
            _Noise.qualityMode = (LibNoise.QualityMode)EditorGUILayout.EnumPopup("Quality Mode", _Noise.qualityMode);
            _Noise.blendMode = (NoiseFunction.BlendMode)EditorGUILayout.EnumPopup("Blend Mode", _Noise.blendMode);

        }
        #endregion

        #region Voronoi UI
        else if (_Noise.type == NoiseFunction.NoiseType.Voronoi)
        {
            EditorGUILayout.Space();
            string name = "Voronoi Noise";
            EditorGUILayout.LabelField(name);
            _Noise.type = (NoiseFunction.NoiseType)EditorGUILayout.EnumPopup("Type of Noise", _Noise.type);
            _Noise.enabled = EditorGUILayout.ToggleLeft("Enabled", _Noise.enabled);
            _Noise.frequency = (double)EditorGUILayout.Slider("Frequency", (float)_Noise.frequency, 0f, 20f);
            _Noise.displacement = (double)EditorGUILayout.Slider("Displacement", (float)_Noise.displacement, 0f, 20f);
            _Noise.distance = EditorGUILayout.ToggleLeft("Use Distance", _Noise.distance);
            _Noise.blendMode = (NoiseFunction.BlendMode)EditorGUILayout.EnumPopup("Blend Mode", _Noise.blendMode);

        }
        #endregion

        #region Ridged Multifractal UI
        else if (_Noise.type == NoiseFunction.NoiseType.RidgedMultifractal)
        {
            string name = "Ridged Multifractal";
            EditorGUI.LabelField(rect, name);
            _Noise.type = (NoiseFunction.NoiseType)EditorGUI.EnumPopup(rect,"Type of Noise", _Noise.type);
            _Noise.enabled = EditorGUI.ToggleLeft(rect,"Enabled", _Noise.enabled);
            _Noise.frequency = (double)EditorGUI.Slider(rect,"Frequency", (float)_Noise.frequency, 0f, 20f);
            _Noise.lacunarity = (double)EditorGUI.Slider(rect,"Lacunarity", (float)_Noise.lacunarity, 1.5000000f, 3.5000000f);
            _Noise.octaves = EditorGUI.IntSlider(rect, "Octaves", _Noise.octaves, 0, 18);
            _Noise.qualityMode = (LibNoise.QualityMode)EditorGUI.EnumPopup(rect,"Quality Mode", _Noise.qualityMode);
            _Noise.blendMode = (NoiseFunction.BlendMode)EditorGUI.EnumPopup(rect,"Blend Mode", _Noise.blendMode);

        }
        #endregion

        #region None UI
        else if (_Noise.type == NoiseFunction.NoiseType.None)
        {

            string name = "None";
            GUI.Label(rect,name);
            _Noise.type = (NoiseFunction.NoiseType)EditorGUI.EnumPopup(rect,"Type of Noise", _Noise.type);

        }

        #endregion
    }
}


