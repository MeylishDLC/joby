using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace _Source.Editor
{
    public class BuildTools
    {
        private const string BuildPathRoot = "Builds";
        private const string WebGLTemplateName = "Better2020";

        [MenuItem("Build/Build All", priority = 0)]
        public static void BuildAll()
        {
            if (!Directory.Exists(BuildPathRoot))
            {
                Directory.CreateDirectory(BuildPathRoot);
            }

            BuildForPC();
            BuildForAndroid();

            EditorUtility.DisplayDialog("Build Complete", "All builds completed", "OK");
        }
        [MenuItem("Build/Build PC", priority = 1)]
        public static void BuildPC() => BuildForPC();
        
        [MenuItem("Build/Build Android", priority = 2)]
        public static void BuildAndroid() => BuildForAndroid();
        
        [MenuItem("Build/Build WebGL", priority = 3)]
        public static void BuildWebGL() => BuildForWebGL();
        
        private static void BuildForPC()
        {
            var path = Path.Combine(BuildPathRoot, "PC");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var exePath = Path.Combine(path, Application.productName + ".exe");
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = GetEnabledScenes(),
                locationPathName = exePath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };
            BuildReportReport("PC", BuildPipeline.BuildPlayer(buildPlayerOptions));
        }

        private static void BuildForAndroid()
        {
            var path = Path.Combine(BuildPathRoot, "Android");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var apkPath = Path.Combine(path, Application.productName + ".apk");
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = GetEnabledScenes(),
                locationPathName = apkPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };
            BuildReportReport("Android", BuildPipeline.BuildPlayer(buildPlayerOptions));
        }
        private static void BuildForWebGL()
        {
            var path = Path.Combine(BuildPathRoot, "WebGL");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            PlayerSettings.WebGL.template = $"PROJECT:{WebGLTemplateName}";
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = GetEnabledScenes(),
                locationPathName = path,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };
            BuildReportReport("WebGL", BuildPipeline.BuildPlayer(buildPlayerOptions));
        }

        private static string[] GetEnabledScenes()
        {
            return EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();
        }
        private static void BuildReportReport(string platform, BuildReport report)
        {
            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"{platform} build succeeded.");
            }
            else
            {
                Debug.LogError($"{platform} build was not succeed: {report.summary.result}");
            }
        }
    }
}