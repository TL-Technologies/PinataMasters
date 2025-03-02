using UnityEngine;
using UnityEditor;


public class ShaderFeatureDrawer : MaterialPropertyDrawer
{
    #region Fields

    string keyword;

    #endregion



    #region Class lifecycle

    public ShaderFeatureDrawer(string _keyword)
    {
        keyword = _keyword;
    }

    #endregion



    #region Public methods

    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
        if (IsKeywordEnabled(prop))
        {
            switch (prop.type)
            {
                case MaterialProperty.PropType.Range:
                    editor.RangeProperty(position, prop, label);
                    break;
                case MaterialProperty.PropType.Float:
                    editor.FloatProperty(position, prop, label);
                    break;
                case MaterialProperty.PropType.Color:
                    editor.ColorProperty(position, prop, label);
                    break;
                case MaterialProperty.PropType.Texture:
                    editor.TextureProperty(position, prop, label);
                    break;
                case MaterialProperty.PropType.Vector:
                    editor.VectorProperty(position, prop, label);
                    break;
                default:
                    GUI.Label(position, "Unknown property type: " + prop.name + ": " + (int)prop.type);
                    break;
            }
        }
    }


    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        return IsKeywordEnabled(prop) ? MaterialEditor.GetDefaultPropertyHeight(prop) : 0f;
    }

    #endregion



    #region Private methods

    bool IsKeywordEnabled(MaterialProperty _property)
    {
        bool isKeywordEnabled = true;

        foreach (Material material in _property.targets)
        {
            if (!material.IsKeywordEnabled(keyword))
            {
                isKeywordEnabled = false;
                break;
            }
        }

        return isKeywordEnabled;
    }

    #endregion
}
