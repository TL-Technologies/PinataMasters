using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;


namespace Modules.Hive.Editor
{
    /// <summary>
    /// Editor post processor who will related the T4 template to the generated file
    /// </summary>
    /// <seealso href="https://forum.unity.com/threads/editor-call-back-after-generate-visual-studio-files.508061/"/>
    public sealed class T4TemplatePostProcessor : AssetPostprocessor
    {
        private static readonly Regex CompileFileRegex = new Regex(@"^.*<Compile Include=""(.*)"".*/>.*$");

        
        /// <summary>
        /// Method automatically called upon CS project generation
        /// </summary>
        public static void OnGeneratedCSProjectFiles()
        {
            foreach (UnityAssemblyDefinition assemblyDefinition in HivePluginHierarchy.Instance.EnumerateUnityAssemblyDefinitions())
            {
                string csProjFullPath = UnityPath.Combine(UnityPath.ProjectPath, $"{assemblyDefinition.Name}.csproj");
                if (File.Exists(csProjFullPath))
                {
                    bool isModified = false;
                    string tempProjectFile = FileUtil.GetUniqueTempPathInProject();
                    
                    // Check templates with generated files
                    using (var reader = new StreamReader(csProjFullPath))
                    using (var writer = new StreamWriter(tempProjectFile))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var match = CompileFileRegex.Match(line);
                            if (!match.Success || !HasTemplate(match.Groups[1].Value, out var template))
                            {
                                writer.WriteLine(line);
                                continue;
                            }

                            isModified = true;
                            WriteGeneratedFileData(writer, match.Groups[1].Value, template);
                        }
                    }
                    
                    // Check templates without generated files
                    string csProjContent = File.ReadAllText(tempProjectFile);
                    StringBuilder insertContent = new StringBuilder();
                    foreach (string filePath in Directory.EnumerateFiles(HivePluginHierarchy.Instance.RootPath, "*.*", SearchOption.AllDirectories)
                        .Where(file => file.ToLower().EndsWith("tt") || file.ToLower().EndsWith("ttinclude") || file.ToLower().EndsWith("t4")))
                    {
                        string includePattern = $"<Content Include=\"{UnityPath.GetAssetPathFromFullPath(filePath)}\"";
                        if (!csProjContent.Contains(includePattern))
                        {
                            insertContent.Append(includePattern);
                            insertContent.Append(" />\n     ");
                        }
                    }
                    if (insertContent.Length > 0)
                    {
                        isModified = true;
                        
                        int insertionIndex = csProjContent.IndexOf("<Compile Include=", StringComparison.InvariantCulture);
                        csProjContent = csProjContent.Insert(insertionIndex, insertContent.ToString());
                        File.WriteAllText(tempProjectFile, csProjContent);
                    }
                    
                    if (isModified)
                    {
                        File.Copy(tempProjectFile, csProjFullPath, true);
                    }
                    File.Delete(tempProjectFile);
                }
            }
        }
        
        
        private static bool HasTemplate(string filePath, out string templatePath)
        {
            string directory = UnityPath.GetDirectoryName(filePath) ?? string.Empty;
            string bareFileName = UnityPath.GetFileName(filePath);
            bareFileName = Path.GetFileNameWithoutExtension(bareFileName) ?? bareFileName;

            if (bareFileName.EndsWith(".g"))
            {
                bareFileName = Path.GetFileNameWithoutExtension(bareFileName);
            }
            templatePath = UnityPath.Combine(directory, $"{bareFileName}.tt");

            return File.Exists(templatePath);
        }
        

        private static void WriteGeneratedFileData(TextWriter writer, string fileName, string ttFileName)
        {
            fileName = UnityPath.FixPathSeparator(fileName);
            ttFileName = UnityPath.FixPathSeparator(ttFileName);
            
            writer.WriteLine($@"     <Content Include=""{ttFileName}"">                                      ");
            writer.WriteLine($@"         <Generator>TextTemplatingFileGenerator</Generator>                  ");
            writer.WriteLine($@"         <LastGenOutput>{UnityPath.GetFileName(fileName)}</LastGenOutput>    ");
            writer.WriteLine($@"     </Content>                                                              ");
            writer.WriteLine($@"     <Compile Include=""{fileName}"">                                        ");
            writer.WriteLine($@"         <AutoGen>True</AutoGen>                                             ");
            writer.WriteLine($@"         <DesignTime>True</DesignTime>                                       ");
            writer.WriteLine($@"         <DependentUpon>{UnityPath.GetFileName(ttFileName)}</DependentUpon>  ");
            writer.WriteLine($@"     </Compile>                                                              ");
        }
    }
}