using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// taken straight from VivifyTemplate
namespace PackageExport.Scripts
{
    public static class PackageExport
    {
        private const string OUTPUT_PATH = "Assets/PackageExport/Output";

        private static void ExportPackage(string[] assetPaths, string packageName)
        {
            string packageFile = $"{packageName}.unitypackage";
            string packagePath = Path.Combine(OUTPUT_PATH, packageFile);
            AssetDatabase.ExportPackage(assetPaths, packagePath, ExportPackageOptions.Recurse);
            Debug.Log($"'{packageFile}' was exported to '{OUTPUT_PATH}'");
        }

        private static void OpenFolderInProject(string projectPath)
        {
            string absolutePath = Path.GetFullPath(projectPath);
            Application.OpenURL($"file://{absolutePath}");
        }

        [MenuItem("Package Export/Run")]
        public static void Run()
        {
            ExportAll();
            OpenFolderInProject(OUTPUT_PATH);
        }

        private static void ExportAll()
        {
            string[] assetPaths =
            {
                "Assets/VivifyBaker"
            };
            ExportPackage(assetPaths, "VivifyBaker");
        }
    }
}