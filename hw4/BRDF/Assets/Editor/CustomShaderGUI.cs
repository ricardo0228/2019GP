using UnityEngine;
using UnityEditor;
using System;

public class CustomShaderGUI : ShaderGUI
{
    MaterialEditor editor;
    MaterialProperty[] properties;
    Material target;
    class ShaddingChoice
    {
        public ShaddingChoice()
        {
            toon = false;
            stroke = false;
            specular = false;
        }
        public bool toon;
        public bool stroke;
        public bool specular;
    };

    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {
        this.editor = editor;
        this.properties = properties;
        this.target = editor.target as Material;

        ShaddingChoice shaddingChoice = new ShaddingChoice();
        MaterialProperty mainTex = FindProperty("_MainTex", properties);
        GUIContent mainTexLabel = new GUIContent(mainTex.displayName);
        editor.TextureProperty(mainTex, mainTexLabel.text);

        if (target.IsKeywordEnabled("USE_TOON")) shaddingChoice.toon = true;
        if (target.IsKeywordEnabled("USE_SPECULAR")) shaddingChoice.specular = true;
        if (target.IsKeywordEnabled("USE_STROKE")) shaddingChoice.stroke = true;

        EditorGUI.BeginChangeCheck();

        shaddingChoice.toon = EditorGUILayout.Toggle("Use Toon", shaddingChoice.toon);

        shaddingChoice.specular = EditorGUILayout.Toggle("Use Specular", shaddingChoice.specular);
        if (shaddingChoice.specular)
        {
            MaterialProperty shininess = FindProperty("_Shininess", properties);
            GUIContent shininessLabel = new GUIContent(shininess.displayName);
            editor.RangeProperty(shininess, shininessLabel.text);
        }

        shaddingChoice.stroke = EditorGUILayout.Toggle("Use Stroke", shaddingChoice.stroke);
        if (shaddingChoice.stroke)
        {
            MaterialProperty outline = FindProperty("_Outline", properties);
            GUIContent outlineLabel = new GUIContent(outline.displayName);
            editor.RangeProperty(outline, outlineLabel.text);
            MaterialProperty outlineColor = FindProperty("_OutlineCol", properties);
            GUIContent outlineColorLabel = new GUIContent(outlineColor.displayName);
            editor.ColorProperty(outlineColor, outlineColorLabel.text);
        }

        if (EditorGUI.EndChangeCheck())
        {
            if (shaddingChoice.toon)
                target.EnableKeyword("USE_TOON");
            else
                target.DisableKeyword("USE_TOON");

            if (shaddingChoice.stroke)
                target.EnableKeyword("USE_STROKE");
            else
                target.DisableKeyword("USE_STROKE");

            if (shaddingChoice.specular)
                target.EnableKeyword("USE_SPECULAR");
            else
                target.DisableKeyword("USE_SPECULAR");
        }
    }
}
