using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AgToolkit.Core.Helper
{
#if UNITY_EDITOR
    public class GameStateHelper : EditorWindow
    {
        private static string _ClassName = "YourClassName";
        private static string _DirToSave = "Assets/GameStates";
        private static string _KeywordInTemplate = "#CLASSNAME#";
        private static string _GameStateTemplate = "GameState.template";
        private static string _GameStateMachineBehaviorTemplate = "GameStateMachineBehavior.template";
        private static string _GameStateExtention = "GameState.cs";
        private static string _GameStateMachineBehaviorExtention = "GameStateMachineBehavior.cs";


        private static string[] HandleFileContent(string path)
        {
            string[] content = File.ReadAllLines(path);

            for (int i = 0; i < content.Length; i++)
            {
                string line = content[i];

                if (!line.Contains(_KeywordInTemplate)) continue;

                content[i] = line.Replace(_KeywordInTemplate, _ClassName);
            }

            return content;
        }

        private static bool CreateFile(string[] content, string classExtention)
        {
            string path = $"{_DirToSave}/{_ClassName}{classExtention}";

            if (File.Exists(path))
            {
                Debug.LogError($"{path} is already created !");
                return false;
            }
            
            // Create a new file     
            using (FileStream fs = File.Create(path))
            {
                for (int i = 0; i < content.Length; i++)
                {
                    // Add some text to file    
                    Byte[] line = new UTF8Encoding(true).GetBytes($"{content[i]} \n");
                    fs.Write(line, 0, line.Length);
                }
            }

            return true;
        }

        private static void CreateGameState()
        {
            string pathGameState = Directory.GetFiles(Application.dataPath, _GameStateTemplate, SearchOption.AllDirectories)[0];
            string pathMachineBehavior = Directory.GetFiles(Application.dataPath, _GameStateMachineBehaviorTemplate, SearchOption.AllDirectories)[0];

            Debug.Assert(File.Exists(pathGameState));
            Debug.Assert(File.Exists(pathMachineBehavior));

            //Create directory
            if (!Directory.Exists(_DirToSave))
            {
                Directory.CreateDirectory(_DirToSave);
            }

            //Create GameState files | Todo: check errors
            CreateFile(HandleFileContent(pathGameState), _GameStateExtention);
            CreateFile(HandleFileContent(pathMachineBehavior), _GameStateMachineBehaviorExtention);

            //Refresh project files
            AssetDatabase.Refresh();
        }

        private void OnGUI()
        {
            GUILayout.Label("GameState Helper", EditorStyles.boldLabel);
            GUILayout.Space(5f);

            GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("State name");
                    _ClassName = GUILayout.TextField(_ClassName);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Directory location");
                    _DirToSave = GUILayout.TextField(_DirToSave);
                GUILayout.EndHorizontal();

                GUILayout.Space(5f);

                if (GUILayout.Button("Create"))
                {
                    CreateGameState();
                }

            GUILayout.EndVertical();
        }

        [MenuItem("AgToolkit/GameMode/CreateGameState")]
        public static void ShowWindow()
        {
            EditorWindow window = EditorWindow.GetWindow(typeof(GameStateHelper), true, "AgToolKit GameState Helper", true);
            window.minSize = new Vector2(300, 100);
        }

    }
#endif
}
