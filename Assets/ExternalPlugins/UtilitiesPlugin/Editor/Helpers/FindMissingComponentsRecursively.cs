using UnityEditor;
using UnityEngine;


namespace Modules.Utilities.Editor
{
    public class FindMissingComponentsRecursively : EditorWindow
    {
        #region Fields
        
        static int gameObjectsCount;
        static int componentsCount;
        static int missingComponentsCount;
        
        #endregion
        
        
        
        #region Unity lifecycle
        
        public void OnGUI()
        {
            if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
            {
                FindInSelected();
            }
        }
        
        #endregion
        
        
        
        #region Private methods
    
        [MenuItem("Modules/Helpers/FindMissingScriptsRecursively")]
        static void ShowWindow()
        {
            GetWindow(typeof(FindMissingComponentsRecursively));
        }
    
        
        static void FindInSelected()
        {
            GameObject[] selectedGameObjects = Selection.gameObjects;
            gameObjectsCount = 0;
            componentsCount = 0;
            missingComponentsCount = 0;
            foreach (GameObject gameObject in selectedGameObjects)
            {
                CheckMissingComponents(gameObject);
            }
            CustomDebug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing components.", gameObjectsCount, componentsCount, missingComponentsCount));
        }
        
    
        static void CheckMissingComponents(GameObject gameObject)
        {
            gameObjectsCount++;
            Component[] components = gameObject.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                componentsCount++;
                if (components[i] == null)
                {
                    missingComponentsCount++;
                    string gameObjectHierarchy = gameObject.name;
                    Transform transform = gameObject.transform;
                    while (transform.parent != null)
                    {
                        gameObjectHierarchy = transform.parent.name + "/" + gameObjectHierarchy;
                        transform = transform.parent;
                    }
                    CustomDebug.Log(gameObjectHierarchy + " has an empty script attached in position: " + i, gameObject);
                }
            }
    
            foreach (Transform childTransform in gameObject.transform)
            {
                CheckMissingComponents(childTransform.gameObject);
            }
        }
        
        #endregion
    }
}