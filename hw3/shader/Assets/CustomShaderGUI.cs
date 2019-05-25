using UnityEngine;
using UnityEditor;
using System;

public class CustomShaderGUI : ShaderGUI
{
    MaterialEditor editor;
    MaterialProperty[] properties;
    Material target;
    enum ShaderType
    {
        Normal, Texture
    }
    enum SpecularType
    {
        BlinnPhong, Phong
    }
    class LightChoice
    {
        public LightChoice(){
            diffuse = false;
            ambient = false;
            specular = false;
        }
        public bool diffuse;
        public bool ambient;
        public bool specular;
    };

    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {
        this.editor = editor;
        this.properties = properties;
        this.target = editor.target as Material;

        ShaderType shaderType = ShaderType.Texture;
        LightChoice lightChoice = new LightChoice();

        if (target.IsKeywordEnabled("TYPE_NORMAL"))
            shaderType = ShaderType.Normal;

        EditorGUI.BeginChangeCheck();
        shaderType = (ShaderType)EditorGUILayout.EnumPopup(
            new GUIContent("Shader type"), shaderType
        );

        if (EditorGUI.EndChangeCheck())
        {
            if (shaderType == ShaderType.Normal)
                target.EnableKeyword("TYPE_NORMAL");
            else
                target.DisableKeyword("TYPE_NORMAL");
        }

        if (shaderType != ShaderType.Normal)
        {
            MaterialProperty mainTex = FindProperty("_MainTex", properties);
            GUIContent mainTexLabel = new GUIContent(mainTex.displayName);
            editor.TextureProperty(mainTex, mainTexLabel.text);

            if (target.IsKeywordEnabled("USE_DIFFUSE")) lightChoice.diffuse = true;
            if (target.IsKeywordEnabled("USE_AMBIENT")) lightChoice.ambient = true;
            if (target.IsKeywordEnabled("USE_SPECULAR")) lightChoice.specular = true;

            EditorGUI.BeginChangeCheck();
            lightChoice.diffuse = EditorGUILayout.Toggle("Show Diffuse", lightChoice.diffuse);
            lightChoice.ambient = EditorGUILayout.Toggle("Show Ambient", lightChoice.ambient);
            lightChoice.specular = EditorGUILayout.Toggle("Show Specular", lightChoice.specular);

            if (EditorGUI.EndChangeCheck())
            {
                if (lightChoice.diffuse)
                    target.EnableKeyword("USE_DIFFUSE");
                else
                    target.DisableKeyword("USE_DIFFUSE");

                if (lightChoice.ambient)
                    target.EnableKeyword("USE_AMBIENT");
                else
                    target.DisableKeyword("USE_AMBIENT");

                if (lightChoice.specular)
                    target.EnableKeyword("USE_SPECULAR");
                else
                    target.DisableKeyword("USE_SPECULAR");
            }
        }

        if (lightChoice.specular)
        {
            SpecularType specularType = SpecularType.BlinnPhong;
            if (target.IsKeywordEnabled("TYPE_PHONG")) specularType = SpecularType.Phong;

            EditorGUI.BeginChangeCheck();
            specularType = (SpecularType)EditorGUILayout.EnumPopup(
                new GUIContent("Specular type"), specularType
            );

            if (EditorGUI.EndChangeCheck())
            {
                if (specularType == SpecularType.BlinnPhong)
                    target.EnableKeyword("TYPE_BLINN");
                else
                    target.DisableKeyword("TYPE_BLINN");

                if (specularType == SpecularType.Phong)
                    target.EnableKeyword("TYPE_PHONG");
                else
                    target.DisableKeyword("TYPE_PHONG");
            }

            MaterialProperty shininess = FindProperty("_Shininess", properties);
            GUIContent shininessLabel = new GUIContent(shininess.displayName);
            editor.RangeProperty(shininess, "Specular Factor");
        }
    }
}
