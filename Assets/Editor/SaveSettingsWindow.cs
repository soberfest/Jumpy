namespace GleyAllPlatformsSave
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;


    public class SaveSettingsWindow : EditorWindow
    {
	    private SaveSettings _saveSettings;
	    private List<SupportedBuildTargetGroup> _buildTargetGroup;
	    private List<SupportedSaveMethods> _selectedSaveMethod;

	    private string _message;
        // Get existing open window or if none, make a new one:
        [MenuItem("Window/Gley/All Platforms Save", false, 10)]
        private static void Init()
        {
            string path = "Assets/AllPlatformsSave/Scripts/Version.txt";
            var reader = new StreamReader(path);
            var window = (SaveSettingsWindow)GetWindow(typeof(SaveSettingsWindow));
            string longVersion = JsonUtility.FromJson<Gley.About.AssetVersion>(reader.ReadToEnd()).longVersion;
            window.titleContent = new GUIContent("Save - v." + longVersion);
            window.minSize = new Vector2(520, 520);
            window.Show();
        }

        private void OnEnable()
        {
            _saveSettings = Resources.Load<SaveSettings>("SaveSettingsData");
            if (_saveSettings == null)
            {
                CreateAdSettings();
                _saveSettings = Resources.Load<SaveSettings>("SaveSettingsData");
            }
            _selectedSaveMethod = _saveSettings.saveMethod;
            _buildTargetGroup = _saveSettings.buildTargetGroup;
        }

        public static void CreateAdSettings()
        {
            var asset = CreateInstance<SaveSettings>();
            if (!AssetDatabase.IsValidFolder("Assets/AllPlatformsSave/Resources"))
            {
                AssetDatabase.CreateFolder("Assets/AllPlatformsSave", "Resources");
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(asset, "Assets/AllPlatformsSave/Resources/SaveSettingsData.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void OnGUI()
        {
            GUILayout.Label("Configure your save plugin from here: ", EditorStyles.boldLabel);

            EditorGUILayout.Space();
            for (int i = 0; i < _buildTargetGroup.Count; i++)
            {
                _buildTargetGroup[i] = (SupportedBuildTargetGroup)EditorGUILayout.EnumPopup("Select your build target:", _buildTargetGroup[i]);
                _selectedSaveMethod[i] = (SupportedSaveMethods)EditorGUILayout.EnumPopup("Select save method:", _selectedSaveMethod[i]);
                if (GUILayout.Button("Remove Build Target"))
                {
                    _buildTargetGroup.RemoveAt(i);
                    _selectedSaveMethod.RemoveAt(i);
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            if (GUILayout.Button("Add Build Target"))
            {
                _buildTargetGroup.Add(SupportedBuildTargetGroup.Android);
                _selectedSaveMethod.Add(SupportedSaveMethods.JSONSerializationFileSave);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Save"))
            {
                for (int i = 0; i < _buildTargetGroup.Count - 1; i++)
                {
                    for (int j = i + 1; j < _buildTargetGroup.Count; j++)
                    {
                        if (_buildTargetGroup[i] == _buildTargetGroup[j])
                        {
                            _message = "Platform " + _buildTargetGroup[i] + " exists multiple times. \nRemove duplicate entries and save again";
                            return;
                        }
                    }
                }

                var buildTargetGroups = Enum.GetValues(typeof(SupportedBuildTargetGroup));
                var saveMethods = Enum.GetValues(typeof(SupportedSaveMethods));
                for (int i = 0; i < buildTargetGroups.Length; i++)
                {
                    for (int j = 0; j < saveMethods.Length; j++)
                    {
                        try
                        {
                            AddPreprocessorDirective(saveMethods.GetValue(j).ToString(), true, (BuildTargetGroup)buildTargetGroups.GetValue(i));
                        }
                        catch (Exception e)
						{
							_message = "Error while adding preprocessor directive: " + e.Message;
							return;
						}
                    }
                }

                for (int i = 0; i < _buildTargetGroup.Count; i++)
                {
                    AddPreprocessorDirective(_selectedSaveMethod[i].ToString(), false, (BuildTargetGroup)_buildTargetGroup[i]);
                }

                SaveSettings();
                _message = "Settings applied.";
            }
            GUILayout.Label(_message, EditorStyles.boldLabel);

            EditorGUILayout.Space();
            if (GUILayout.Button("Open Test Scene"))
            {
                EditorSceneManager.OpenScene("Assets/AllPlatformsSave/Example/Scenes/SimpleSaveExample.unity");
            }
        }

        private void SaveSettings()
        {
            _saveSettings.saveMethod = _selectedSaveMethod;
            _saveSettings.buildTargetGroup = _buildTargetGroup;
            EditorUtility.SetDirty(_saveSettings);
        }

        private void AddPreprocessorDirective(string directive, bool remove, BuildTargetGroup target)
        {
            string textToWrite = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

            if (remove)
            {
                if (textToWrite.Contains(directive))
                {
                    textToWrite = textToWrite.Replace(directive, "");
                }
            }
            else
            {
                Debug.Log(directive + " is set for " + target);
                if (!textToWrite.Contains(directive))
                {
                    if (textToWrite == "")
                    {
                        textToWrite += directive;
                    }
                    else
                    {
                        textToWrite += "," + directive;
                    }
                }
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, textToWrite);
        }
    }
}
