//#define Firebase_Enabled
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.SharpZipLib.Zip;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildCommands
{
    private const ulong KebiByte = 1024;
    private const ulong Mebibyte = KebiByte * KebiByte;
    private const string CustomBuildPathKey = "CustomBuildPath";
    private const string CleanUpBuildsKey = "CleanUpBuildsAuto";
    private const int MaxBuildsPerDay = 4;

    [MenuItem("File/Build App &b")]
    public static void BuildAppAuto()
    {
        string path = SetBuildPath(false);
        if (string.IsNullOrWhiteSpace(path))
            return;

        (bool autoRun, bool canceled) = AutoRunCheck("Build Auto Run");
        if (canceled)
            return;
#if Firebase_Enabled
        SetupFireBase(CheckIsDevelopmentBuild());
#endif
        CleanupOldSymbols(path);

        string zipPath = FormatZipPath(path);
        path = FormatBuildName(path);

        PreProcess();

        var buildTarget = EditorUserBuildSettings.activeBuildTarget;
        BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, path, buildTarget, autoRun ? BuildOptions.AutoRunPlayer : BuildOptions.None);

        ProcessReport(report);
        ZipResult(zipPath,path);
        CleanupBuilds(false);
    }

    [MenuItem("File/Build App Select Path")]
    public static void BuildApp()
    {
        string path = SetBuildPath(true);
        if (string.IsNullOrWhiteSpace(path))
            return;

        (bool autoRun, bool canceled) = AutoRunCheck("Build Manual");
        if (canceled)
            return;
#if Firebase_Enabled
        SetupFireBase(CheckIsDevelopmentBuild());
#endif
        CleanupOldSymbols(path);
        
        string zipPath = FormatZipPath(path);
        path = FormatBuildName(path);

        PreProcess();

        var buildTarget = EditorUserBuildSettings.activeBuildTarget;
        BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, path, buildTarget, autoRun ? BuildOptions.AutoRunPlayer : BuildOptions.None);

        ProcessReport(report);
        ZipResult(zipPath,path);
        CleanupBuilds(false);
    }

    [MenuItem("File/Build Development App #&b")]
    public static void BuildDevelopmentBuild()
    {
        string path = SetBuildPath(false);
        if (string.IsNullOrWhiteSpace(path))
            return;

        (bool autoRun, bool canceled) = AutoRunCheck("Development Build");
        if (canceled)
            return;
#if Firebase_Enabled
        SetupFireBase(CheckIsDevelopmentBuild());
#endif
        CleanupOldSymbols(path);
        
        string zipPath = FormatZipPath(path);
        path = FormatBuildName(path);

        PreProcess();

        var buildTarget = EditorUserBuildSettings.activeBuildTarget;
        BuildOptions options = autoRun ?
            BuildOptions.AutoRunPlayer | BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging :
            BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging;

        BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, path, buildTarget, options);

        ProcessReport(report);
        ZipResult(zipPath,path);
        CleanupBuilds(false);
    }

    [MenuItem("File/Set Build Path")]
    public static void ChangeBuildPath() => SetBuildPath(true);

    private static string FormatBuildName(string path)
    {
        var buildTarget = EditorUserBuildSettings.activeBuildTarget;
        path = $"{path}/{PlayerSettings.companyName}/{PlayerSettings.productName}/{System.DateTime.Now:dd-MM-yyyy_HH-mm}";
        path = buildTarget switch
        {
            BuildTarget.StandaloneWindows or BuildTarget.StandaloneWindows64 => $"{path}/{PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup)}/{PlayerSettings.productName}.exe",
            BuildTarget.StandaloneOSX => $"{path}/{PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup)}/{PlayerSettings.productName}",
            BuildTarget.Android=>$"{path}/{PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup)}/{PlayerSettings.productName}.apk",
            _ => $"{path}-{buildTarget}/{PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup)}/{PlayerSettings.productName}"
        };
        Debug.Log($"Building to path - {path}");
        return path;
    }

    private static string FormatZipPath(string path)
    {
        var buildTarget = EditorUserBuildSettings.activeBuildTarget;
        var date = System.DateTime.Now;
        path = $"{path}/{PlayerSettings.companyName}/{PlayerSettings.productName}/{date:dd-MM-yyyy_HH-mm}";
        path = buildTarget switch
        {
            BuildTarget.StandaloneWindows or BuildTarget.StandaloneWindows64 => $"{path}/{date:yyyyMMddmmhh}-{PlayerSettings.productName}.zip",
            _ => string.Empty
        };
        return path;
    }

    private static void ProcessReport(BuildReport report)
    {
        var summary = report.summary;
        switch (summary.result)
        {
            case BuildResult.Succeeded:
                Debug.Log($"Build succeeded: {(summary.totalSize / Mebibyte)} MiB");
                Debug.Log($"Build took: {summary.totalTime}");
                break;
            case BuildResult.Failed:
                Debug.Log("Build failed");
                break;
        }
    }

    [MenuItem("File/Cleanup Builds")]
    private static void CleanupBuilds()
    {
        CleanupBuilds(true);
    }

    [MenuItem("File/Change Cleanup Builds")]
    private static void ChangeCleanupSettings()
    {
        EditorPrefs.SetBool(CleanUpBuildsKey, EditorUtility.DisplayDialog("Auto Cleanup", "Should script limit number of builds per day?", "Yes", "No"));
    }

    private static void CleanupBuilds(bool byPassCheck)
    {
        string directoryPath = SetBuildPath(false);
        if (string.IsNullOrWhiteSpace(directoryPath))
            return;

        if (!byPassCheck)
        {
            bool canDoCleanup;
            if (!EditorPrefs.HasKey(CleanUpBuildsKey))
            {
                canDoCleanup = EditorUtility.DisplayDialog("Auto Cleanup", "Should script limit number of builds per day?", "Yes", "No");
                EditorPrefs.SetBool(CleanUpBuildsKey, canDoCleanup);
            }
            else canDoCleanup = EditorPrefs.GetBool(CleanUpBuildsKey);

            if (!canDoCleanup)
                return;
        }

        DirectoryInfo directory = new DirectoryInfo(directoryPath);
        var groupedFiles = directory.GetFiles("*.apk").GroupBy(x => x.CreationTime.Date).Where(g => g.Count() > MaxBuildsPerDay);
        
        foreach (var fileGroup in groupedFiles)
        {
            var orderedGroup = fileGroup.OrderBy(x => x.CreationTime).ToArray();
            for (int i = 0; i < orderedGroup.Length - 4; i++)
            {
                orderedGroup[i].Delete();
            }
        }
    }

    private static void CleanupOldSymbols(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            var paths = Directory.GetFiles(directoryPath, "*.symbols.zip");
            foreach (var path in paths)
            {
                File.Delete(path);
            }
        }
    }

    private static (bool autoRun, bool canceled) AutoRunCheck(string buildTypeName)
    {
        int result = EditorUtility.DisplayDialogComplex(buildTypeName, $"Target: {EditorUserBuildSettings.activeBuildTarget}\nShould Autorun?", "Yes", "No", "Cancel");
        return (result == 0, result == 2);
    }

    private static string SetBuildPath(bool forceUpdate)
    {
        string projectname = GetProjectName();
        string path = EditorPrefs.GetString($"{CustomBuildPathKey}-{projectname}", "");
        if (string.IsNullOrWhiteSpace(path) || forceUpdate)
        {
            path = EditorUtility.SaveFolderPanel("Choose location of Build artifact", path, "");
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        EditorPrefs.SetString($"{CustomBuildPathKey}-{projectname}", path);
        return path;

        string GetProjectName()
        {
            string[] s = Application.dataPath.Split('/');
            string projectName = s[^2];
            Debug.Log("project = " + projectName);
            return projectName;
        }
    }

    private static void PreProcess()
    {
        PlayerSettings.Android.bundleVersionCode++;
    }

    private static void ZipResult(string zipFilePath, string outputPath)
    {
        if (string.IsNullOrEmpty(zipFilePath)) return;
        
        
    }

    #region Firebase

#if Firebase_Enabled
    const string isDevelopmentBuildSaveKey = "isDevelopmentBuild";

    private static bool CheckIsDevelopmentBuild()
    {
        bool isDevelopmentBuild = EditorUtility.DisplayDialog("Build", "Build type", "Development", "Release");
        EditorPrefs.SetBool(isDevelopmentBuildSaveKey, isDevelopmentBuild);
        return isDevelopmentBuild;
    }

    private static void SetupFireBase(bool isDevelopmentBuild)
    {
        const string basePathGoogleServicesFolder = "Configuration/Firebase";
        const string debug = "Debug";
        const string release = "Release";
        const string serviceFileNameAndroid = "google-services.json";
        const string inactiveServiceFileNameAndroid = "google-services.json.disabled";

        const string serviceFileNameiOS = "GoogleService-Info.plist";
        const string inactiveServiceFileNameiOS = "GoogleService-Info.plist.disabled";

        UpdateServiceFiles();
        AssetDatabase.Refresh();
        void UpdateServiceFiles()
        {
            string sourceFilePath = $"{Application.dataPath}/{basePathGoogleServicesFolder}/{(isDevelopmentBuild ? debug : release)}/{inactiveServiceFileNameAndroid}";
            string targetFilePath = $"{Application.dataPath}/{basePathGoogleServicesFolder}/{serviceFileNameAndroid}";
            if (File.Exists(sourceFilePath))
            {
                File.Copy(sourceFilePath, targetFilePath, true);
            }

            sourceFilePath = $"{Application.dataPath}/{basePathGoogleServicesFolder}/{(isDevelopmentBuild ? debug : release)}/{inactiveServiceFileNameiOS}";
            targetFilePath = $"{Application.dataPath}/{basePathGoogleServicesFolder}/{serviceFileNameiOS}";

            if (File.Exists(sourceFilePath))
            {
                File.Copy(sourceFilePath, targetFilePath, true);
            }
        }
    }

#endif

    #endregion Firebase
}