using Modules.Hive.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Modules.General.Editor.EmptyDirectoriesCleaner
{
    public class EmptyDirectoriesCleanerWindow : EditorWindow
    {
        #region Fields

        private const float DirectoryLabelHeight = 21.0f;

        private string delayedMessage;
        private List<DirectoryInfo> emptyDirectories;
        private GUIContent folderContent;
        private Vector2 scrollPosition;

        #endregion



        #region Properties

        private GUIContent FolderContent => folderContent ??
            (folderContent = EditorGUIUtility.IconContent("Folder Icon"));

        private bool HasEmptyDirectories => emptyDirectories?.Count > 0;

        #endregion



        #region Unity Lifecycle

        private void OnEnable()
        {
            delayedMessage = "Click 'Find Empty Directories' Button.";
        }


        private void OnGUI()
        {
            if (delayedMessage != null)
            {
                ShowNotification(new GUIContent(delayedMessage));
                delayedMessage = null;
            }

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Find Empty Directories"))
                    {
                        FillEmptyDirectories(out emptyDirectories);

                        if (!HasEmptyDirectories)
                        {
                            ShowNotification(new GUIContent("No Empty Directory"));
                        }
                        else
                        {
                            RemoveNotification();
                        }
                    }

                    if (CreateColorButton("Delete All", HasEmptyDirectories, Color.red))
                    {
                        DeleteEmptyDirectories(ref emptyDirectories);
                        ShowNotification(new GUIContent("Deleted All"));
                    }
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(1));

                if (HasEmptyDirectories)
                {
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true));
                    {
                        ShowEmptyDirectories();
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
            EditorGUILayout.EndVertical();
        }

        #endregion



        #region Methods

        [MenuItem("Modules/General/Clean Empty Directories")]
        public static void ShowWindow()
        {
            EmptyDirectoriesCleanerWindow window = GetWindow<EmptyDirectoriesCleanerWindow>();
            window.titleContent = new GUIContent("Clean");
        }


        private bool CreateColorButton(string buttonTitle, bool isEnabled, Color color)
        {
            bool oldIsEnabled = GUI.enabled;
            Color oldColor = GUI.color;

            GUI.enabled = isEnabled;
            GUI.color = color;

            bool hasClick = GUILayout.Button(buttonTitle);

            GUI.enabled = oldIsEnabled;
            GUI.color = oldColor;

            return hasClick;
        }


        private void ShowEmptyDirectories()
        {
            EditorGUILayout.BeginVertical();
            {
                foreach (DirectoryInfo dirInfo in emptyDirectories)
                {
                    FolderContent.text = UnityPath.GetAssetPathFromFullPath(dirInfo.FullName);

                    GUILayout.Label(FolderContent, GUILayout.Height(DirectoryLabelHeight));
                }
            }
            EditorGUILayout.EndVertical();
        }
        
        
        private static void DeleteEmptyDirectories(ref List<DirectoryInfo> emptyDirectories)
        {
            foreach (DirectoryInfo directoryInfo in emptyDirectories)
            {
                FileSystemUtilities.DeleteEntryAndEmptyParentsDirectories(directoryInfo.FullName);
            }

            emptyDirectories = null;

            AssetDatabase.Refresh();
        }


        private static void FillEmptyDirectories(out List<DirectoryInfo> emptyDirectories)
        {
            emptyDirectories = FileSystemUtilities.EnumerateDirectories()
                .Where(directoryInfo => IsEmptyDirectory(directoryInfo))
                .ToList();
        }


        private static bool IsEmptyDirectory(DirectoryInfo directoryInfo)
        {
            if (!Directory.Exists(directoryInfo.FullName))
            {
                return true;
            }

            var allFiles = Directory.EnumerateFiles(directoryInfo.FullName, "*.*", SearchOption.AllDirectories);

            if (!allFiles.Any())
            {
                return true;
            }
            if (allFiles.All(filePath => filePath.EndsWith(".meta") || filePath.EndsWith(".DS_Store")))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
