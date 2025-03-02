using System;


namespace Modules.Hive.Editor.BuildUtilities
{
    public static class CommandLineBuild
    {
        /// <summary>
        /// <para>
        /// Do not use it directly !!!
        /// </para>
        /// Makes the project build
        /// </summary>
        public static void Build()
        {
            // Parse command line
            BuildCommandModel model = new BuildCommandModel(Environment.GetCommandLineArgs());

            // Create pipeline context
            var context = model.CreateDefaultPipelineContext();

            if (!string.IsNullOrEmpty(model.PipelineName))
            {
                BuildPipelineFactory.CreatePipeline(model.PipelineName, context)
                    .SetView(null)
                    .AddTask(new CloseUnityEditorTask())
                    .BuildAndSubmit();
            }
            else
            {
                BuildPipelineFactory.CreateDefaultPipeline(context)
                    .SetView(null)
                    .AddTask(new CloseUnityEditorTask())
                    .BuildAndSubmit();
            }
        }
    }
}
