using System;
using UnityEditor;
using UnityEngine;

namespace PinataMasters
{
    [CustomEditor(typeof(SortingLayerRenderer))]
    public class SortingLayerRendererEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var renderer = target as SortingLayerRenderer;

            string[] sortingLayerNames = new string[SortingLayer.layers.Length];
            for (int i = 0; i < SortingLayer.layers.Length; i++)
            {
                sortingLayerNames[i] = SortingLayer.layers[i].name;
            }

            int oldLayerIndex = Array.IndexOf(sortingLayerNames, renderer.SortingLayerName);
            int newLayerIndex = EditorGUILayout.Popup("Sorting Layer", oldLayerIndex, sortingLayerNames);
            renderer.SortingLayerName = sortingLayerNames[newLayerIndex];

            renderer.SortingOrder = EditorGUILayout.IntField("Sorting Layer Order", renderer.SortingOrder);
        }
    }
}