using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpriteRotator))]
public class SpriteRotatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Execute Rotator"))
        {
            (target as SpriteRotator).RotateSprite();
        }
    }
}
