using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.CodeEditor;

namespace GameFunctions.Editors {

    public static class CSProjGeneratorMenuTool {

        [MenuItem(nameof(GameFunctions) + "/重新生成 CSProj")]
        public static void CleanCSProj() {

            List<string> files = FindAllFileWithExt(Environment.CurrentDirectory, "*.csproj");
            foreach (var file in files) {
                File.Delete(file);
            }
            Debug.Log("消除 CSProj 成功: " + files.Count.ToString());

            IExternalCodeEditor codeEditor = CodeEditor.CurrentEditor;
            codeEditor.SyncAll();

            Debug.Log("重新生成了 .csproj");
        }

        static List<string> FindAllFileWithExt(string rootPath, string ext) {

            List<string> fileList = new List<string>();

            DirectoryInfo directoryInfo = new DirectoryInfo(rootPath);
            FileInfo[] allFiles = directoryInfo.GetFiles(ext);
            for (int i = 0; i < allFiles.Length; i += 1) {
                var file = allFiles[i];
                fileList.Add(file.FullName);
            }

            DirectoryInfo[] childrenDirs = directoryInfo.GetDirectories();
            for (int i = 0; i < childrenDirs.Length; i += 1) {
                var dir = childrenDirs[i];
                fileList.AddRange(FindAllFileWithExt(dir.FullName, ext));
            }

            return fileList;

        }

    }
}