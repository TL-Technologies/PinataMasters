using Modules.Hive.Editor;
using Modules.General.HelperClasses;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;


namespace Modules.Legacy.TextureManagement.Editor
{
    public static class ApplePlatformTextureBuildProcess
    {
        #region Nested types
        
        class PostprocessBuild : IPostprocessBuildWithReport
        {
            public int callbackOrder => 900;
            

            public void OnPostprocessBuild(BuildReport report)
            {
                if (report.summary.platform == BuildTarget.iOS || report.summary.platform == BuildTarget.tvOS)
                {
                    CustomDebug.Log("=== Apple Platform Texture Loader postbuild start ===");

                    string path = report.summary.outputPath;
                    
                    if (!string.IsNullOrEmpty(path))
                    {
                        string dirPath = path.AppendPathComponent(XcodeFolderStreamingTextures);
                        DirectoryInfo dir = new DirectoryInfo(dirPath);

                        // unity instruments slowly
                        // ConvertPNG(dir);
                        ConvertMacPng(dir);
                    }

                    
                    CustomDebug.Log("=== Apple Platform Texture Loader postbuild end ===");
                }
            }
        }
        
        #endregion
        
        
        
        #region Fields

        const string XcodeFolderStreamingTextures = "Data/Raw/tk2dTextures/";

        const string AppBashPath = "/bin/bash";
        const string AppBashArguments = "-c \"\'{0}\' \'{1}\' \'{2}\'\"";

        static object queueLock = new object();
        static int threadLoadedAssetCount = 0;
        static int threadNeedLoadAssetCount = 0;
        static bool useThreading = false;
        
        static Action asyncTask;

        #endregion

        
        
        #region Private methods

        static void ConvertPNG(DirectoryInfo dir)
        {
            if (dir != null)
            {
                foreach (FileInfo file in dir.GetFiles())
                {
                    if (!file.Name.Contains("meta") && file.Name.Contains("png"))
                    {
                        byte[] textureData = File.ReadAllBytes(file.FullName);
                        Texture2D dtex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                        dtex.LoadImage(textureData);

                        for (int x = 0; x < dtex.width; ++x)
                        {
                            for (int y = 0; y < dtex.height; ++y)
                            {
                                Color col = dtex.GetPixel(x, y);
                                
                                if (!Mathf.Approximately(col.a, 0))
                                {
                                    col.r /= col.a;
                                    col.g /= col.a;
                                    col.b /= col.a;
                                    dtex.SetPixel(x, y, col);
                                }
                            }
                        }

                        textureData = dtex.EncodeToPNG();
                        File.WriteAllBytes(file.FullName, textureData);
                    }
                }
            }
        }


        static void ConvertMacPng(DirectoryInfo dir)
        {
            if ((dir != null) && (dir.Exists))
            {
                string converterPath = UnityPath.Combine(
                    LegacyPluginHierarchy.Instance.RootPath,
                    "Editor",
                    "PngPostProcess",
                    "PMA2NPMA");
                
                foreach (FileInfo file in dir.GetFiles())
                {
                    if (!file.Name.Contains("meta") && file.Name.Contains("png"))
                    {
                        if (!useThreading)
                        {
                            Process process = Process.Start(AppBashPath,
                                string.Format(AppBashArguments, converterPath, file.FullName, file.FullName));
                            process.WaitForExit();
                            
                            if (process.ExitCode != 0)
                            {
                                CustomDebug.Log("Error process file: " + file.Name);
                            }

                            process = null;
                            Thread.Sleep(1);
                            GC.Collect();
                        }
                        else
                        {
                            Thread thread = new Thread(() =>
                            {
                                Process process = Process.Start(AppBashPath,
                                    string.Format(AppBashArguments, converterPath, file.FullName, file.FullName));
                                process.WaitForExit();
                                lock (queueLock)
                                {
                                    asyncTask += () =>
                                    {
                                        if (process.ExitCode != 0)
                                        {
                                            CustomDebug.Log("Error process file: " + file.Name);
                                        }

                                        threadLoadedAssetCount++;
                                    };
                                }
                            });
                            thread.Start();

                            while (threadLoadedAssetCount < threadNeedLoadAssetCount)
                            {
                                lock (queueLock)
                                {
                                    if (asyncTask != null)
                                    {
                                        asyncTask();
                                        asyncTask = null;
                                    }
                                }

                                Thread.Sleep(1);
                            }

                            Thread.Sleep(1000);
                        }
                    }
                }
            }
        }

        #endregion
    }
}