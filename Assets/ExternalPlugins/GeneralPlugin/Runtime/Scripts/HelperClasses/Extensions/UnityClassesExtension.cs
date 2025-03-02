using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class UnityClassesExtension
{
    /// <summary>
    /// Gets first found component from root objects on scene.
    /// </summary>
    public static T GetRootComponent<T>(this Scene scene) where T : MonoBehaviour
    {
        T result = null;
        
        if (scene.isLoaded)
        {
            // search for active objects
            T[] objects = Object.FindObjectsOfType<T>();
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].gameObject.scene == scene)
                {
                    result = objects[i];
                    break;
                }
            }
            
            if (result == null)
            {
                // if root wasn't found try to get hidden objects instead
                List<T> results = new List<T>();
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene currentScene = SceneManager.GetSceneAt(i);
                    if (currentScene.isLoaded)
                    {
                        var allGameObjects = currentScene.GetRootGameObjects();
                        for (int j = 0; j < allGameObjects.Length; j++)
                        {
                            GameObject go = allGameObjects[j];
                            results.AddRange(go.GetComponentsInChildren<T>(true));
                        }
                    }
                }
                
                objects = results.ToArray();
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i].gameObject.scene == scene)
                    {
                        result = objects[i];
                        break;
                    }
                }
            }
        }

        return result;
    }


    /// <summary>
    /// RectTransform extension.
    /// </summary>
    public static void FillParent(this Transform transform)
    {
        RectTransform t = transform as RectTransform;
        if (t != null)
        {
            t.anchorMin = Vector2.zero;
            t.anchorMax = new Vector2(1f, 1f);
            t.offsetMin = Vector2.zero;
            t.offsetMax = Vector2.zero;
            t.localScale = new Vector3(1f, 1f, 1f);
        }
    }


    public static void SafeDestroy(this Object o)
    {
        #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Object.Destroy(o);
            }
            else
            {
                Object.DestroyImmediate(o);
            }
        #else
            Object.Destroy(o);
        #endif
    }


    public static T Instantiate<T>(this string path, Transform parent = null) where T : Object
    {
        return Object.Instantiate(Resources.Load<T>(path), parent);
    }


    public static void LocalReset(this Transform transform, float z = 0.0f)
    {
        transform.localPosition = new Vector3(0.0f, 0.0f, z);
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }


    public static void GlobalReset(this Transform transform, float z = 0.0f)
    {
        transform.position = new Vector3(0.0f, 0.0f, z);
        transform.rotation = Quaternion.identity;     
        transform.SetGlobalScale(Vector3.one);
    }


    public static void SetGlobalScale(this Transform transform, Vector3 scale)
    {
        transform.localScale = Vector3.one;
        Vector3 lossyScale = transform.lossyScale;
        transform.localScale = new Vector3(
            scale.x / (Mathf.Approximately(lossyScale.x, 0.0f) ? Mathf.Epsilon : lossyScale.x), 
            scale.y / (Mathf.Approximately(lossyScale.y, 0.0f) ? Mathf.Epsilon : lossyScale.y), 
            scale.z / (Mathf.Approximately(lossyScale.z, 0.0f) ? Mathf.Epsilon : lossyScale.z));
    }
}
