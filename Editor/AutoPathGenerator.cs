using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JHS.Library.AutoPathGenerator.Editor
{
    [InitializeOnLoad]
    public class AutoPathGenerator
    {
        const string GENERATED_PATH = "Assets/AutoPathGenerated";

        static AutoPathGenerator()
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            EditorApplication.delayCall += Initialize;
        }

        static void Initialize() => EnsureConfigExists();

        static void OnAfterAssemblyReload() => GeneratePathTypes();

        #region Config Asset

        public class AutoPathGeneratorConfig : ScriptableObject
        {
            public string generatedScriptsPath = GENERATED_PATH.Substring(7);
            public bool autoGenerate = false;
        }

        static void EnsureConfigExists()
        {
            var config = AssetDatabase.LoadAssetAtPath<AutoPathGeneratorConfig>($"{GENERATED_PATH}/AutoPathGeneratorConfig.asset");
            if (config == null)
            {
                if (!Directory.Exists(GENERATED_PATH)) Directory.CreateDirectory(GENERATED_PATH);

                config = ScriptableObject.CreateInstance<AutoPathGeneratorConfig>();
                AssetDatabase.CreateAsset(config, $"{GENERATED_PATH}/AutoPathGeneratorConfig.asset");
                AssetDatabase.SaveAssets();
            }
        }

        static AutoPathGeneratorConfig GetConfig()
        {
            var config = AssetDatabase.LoadAssetAtPath<AutoPathGeneratorConfig>($"{GENERATED_PATH}/AutoPathGeneratorConfig.asset");
            if (config == null)
            {
                EnsureConfigExists();
                config = AssetDatabase.LoadAssetAtPath<AutoPathGeneratorConfig>($"{GENERATED_PATH}/AutoPathGeneratorConfig.asset");
            }

            return config;
        }

        #endregion

        #region Editor Window

        [CustomEditor(typeof(AutoPathGeneratorConfig))]
        public class AutoPathGeneratorConfigEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                var config = (AutoPathGeneratorConfig)target;

                EditorGUI.BeginChangeCheck();

                config.generatedScriptsPath = EditorGUILayout.DelayedTextField("Output Path:", config.generatedScriptsPath);
                config.autoGenerate = EditorGUILayout.Toggle("Auto Generate", config.autoGenerate);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(config);
                    AssetDatabase.SaveAssets();
                    if (config.autoGenerate) GeneratePathTypes();
                }
            }
        }

        [MenuItem("Tools/Auto Path Generator/Settings")]
        static void OpenSettings()
        {
            var config = AssetDatabase.LoadAssetAtPath<AutoPathGeneratorConfig>($"{GENERATED_PATH}/AutoPathGeneratorConfig.asset");
            if (config == null)
            {
                EnsureConfigExists();
                config = AssetDatabase.LoadAssetAtPath<AutoPathGeneratorConfig>($"{GENERATED_PATH}/AutoPathGeneratorConfig.asset");
            }

            Selection.activeObject = config;
        }

        #endregion

        #region Asset Post Processor

        public class AutoPathGeneratorAssetPostProcessor : AssetPostprocessor
        {
            static void OnPostprocessAllAssets(
                string[] importedAssets,
                string[] deletedAssets,
                string[] movedAssets,
                string[] movedFromAssetPaths)
            {
                var config = GetConfig();
                if (!config.autoGenerate) return;

                GeneratePathTypes();
            }
        }

        #endregion

        #region Code Generation

        [MenuItem("Tools/Auto Path Generator/Generate")]
        static void GeneratePathTypes()
        {
            var config = GetConfig();

            var paths = GetPaths();
            if (paths.Count > 0)
            {
                GenerateCode(paths, config);
            }
        }

        static List<(string category, string key, string path)> GetPaths()
        {
            var paths = Resources.LoadAll<Object>("").Select(clip =>
            {
                var fullPath = AssetDatabase.GetAssetPath(clip);
                var resourcesIndex = fullPath.IndexOf("/Resources/", StringComparison.Ordinal) + 11; // "/Resources/" 다음부터
                var resourcePath = fullPath.Substring(resourcesIndex);
                return $"{Path.ChangeExtension(resourcePath, null)}";
            }).Distinct().Select(path => ("ResourcesPaths", ToKey(path), path));

#if ADDRESSABLE_SUPPORT
            paths = paths.Concat(AddressablePathExplorer.GetAllAddressables().Select(info => $"{info.address}").Select(path => ("AddressablePaths", ToKey(path), path)));
#endif

            return paths.ToList();
        }
        
        static string ToKey(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var result = input.Replace(" ", "_").Replace("/", "_").Replace("\\", "_");
            result = Regex.Replace(result, @"[^\w\d_]", "");
            result = Regex.Replace(result, "_+", "_");

            if (char.IsDigit(result[0]))
                result = "_" + result;

            var keywords = new HashSet<string> { "class", "enum", "int", "string", "void", "return" };
            if (keywords.Contains(result.ToLower()))
                result = "@" + result;

            return result;
        }

        static void GenerateCode(List<(string category, string key, string path)> paths, AutoPathGeneratorConfig config)
        {
            var sb = new StringBuilder();

            var categories = paths.GroupBy(path => path.category).ToList();
            
            sb.AppendLine("// This file is auto-generated. Do not modify.");
            sb.AppendLine("namespace JHS.Library.AutoPathGenerator");
            sb.AppendLine("{");
            foreach (var category in categories)
            {
                sb.AppendLine($"    public static class {category.Key}");
                sb.AppendLine("    {");
                foreach (var path in category) sb.AppendLine($"        public const string {path.key} = \"{path.path}\";");
                sb.AppendLine("    }");
            }
            sb.AppendLine("}");

            if (!Directory.Exists($"Assets/{config.generatedScriptsPath}")) Directory.CreateDirectory($"Assets/{config.generatedScriptsPath}");

            string filePath = Path.Combine($"Assets/{config.generatedScriptsPath}", "GeneratedPaths.cs");
            File.WriteAllText(filePath, sb.ToString());

            AssetDatabase.Refresh();
        }

        #endregion
    }
}