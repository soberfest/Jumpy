#if UNITY_WINRT && !UNITY_EDITOR
using UnityEngine.Windows;
#endif
using GleyAllPlatformsSave;
using UnityEngine;
/// <summary>
/// Version 1.0.3
///
/// Serializes any type of serializable class to a byte array which is then encrypted and saved to PlayerPrefs of to a file depending on your settings
///
/// For a detailed usage example see the TestSave.cs
///
/// </summary>
using UnityEngine.Events;



public class SaveManager
{
	private static ISaveClass _saveMethod;


#if JSONSerializationGooglePlaySave
    static ISaveClass cloudSaveMethod;
#endif

	private static SaveManager _instance;
    public static SaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SaveManager();
                AddRequiredScript();
            }

            return _instance;
        }
    }

    private static void AddRequiredScript()
    {
#if JSONSerializationFileSave
    _saveMethod = new JSONSerializationFileSave();
#endif

#if JSONSerializationPlayerPrefs
    _saveMethod = new JSONSerializationPlayerPrefs();
#endif

#if BinarySerializationFileSave
    _saveMethod = new BinarySerializationFileSave();
#endif

#if BinarySerializationPlayerPrefs
    _saveMethod = new BinarySerializationPlayerPrefs();
#endif
    }


    /// <summary>
    /// Save the specified dataToSave, to fileName and encrypt it.
    /// </summary>
    /// <param name="dataToSave">Any type of serializable class</param>
    /// <param name="path">File name path.</param>
    /// <param name="encrypt">If set to <c>true</c> encrypt.</param>
    /// <param name="completeMethod">Called after all is done</param>
    public void Save<T>(T dataToSave, string path, UnityAction<SaveResult, string> completeMethod, bool encrypted) where T : class, new()
    {
        if (_saveMethod == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Current platform (" + UnityEditor.EditorUserBuildSettings.activeBuildTarget + ") is not added to plugin settings. Go to Window->Gley->Save and add your current platform");
#else
             Debug.LogError("Current platform is not added to plugin settings. Go to Window->Gley->Save and add your current platform");
#endif
            return;
        }
        _saveMethod.Save<T>(dataToSave, path, completeMethod, encrypted);
    }

    public void SaveString<T>(T dataToSave, UnityAction<SaveResult, string> completeMethod, bool encrypted) where T : class, new()
    {
        _saveMethod.SaveString<T>(dataToSave, completeMethod, encrypted);
    }

    /// <summary>
    /// Load the specified fileName and decript it.
    /// Returns any type of serializable data
    /// If specified file does not exists, a new one is generated and the defauld values from serializeable class are saved 
    /// </summary>
    /// <param name="path">File name.</param>
    /// <param name="completeMethod">Called after all is done</param>
    /// <param name="encrypted">If set to <c>true</c> encrypted.</param>
    public void Load<T>(string path, UnityAction<T, SaveResult, string> completeMethod, bool encrypted) where T : class, new()
    {
        if (_saveMethod == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Current platform (" + UnityEditor.EditorUserBuildSettings.activeBuildTarget + ") is not added to plugin settings. Go to Window->Gley->Save and add your current platform");
#else
             Debug.LogError("Current platform is not added to plugin settings. Go to Window->Gley->Save and add your current platform");
#endif
            return;
        }
        _saveMethod.Load<T>(path, completeMethod, encrypted);
    }
    public void LoadString<T>(string stringToLoad, UnityAction<T, SaveResult, string> loadCompleteMethod, bool encrypted) where T : class, new()
    {
        _saveMethod.LoadString<T>(stringToLoad, loadCompleteMethod, encrypted);
    }

        public void ClearAllData(string path)
    {
        if (_saveMethod == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Current platform (" + UnityEditor.EditorUserBuildSettings.activeBuildTarget + ") is not added to plugin settings. Go to Window->Gley->Save and add your current platform");
#else
             Debug.LogError("Current platform is not added to plugin settings. Go to Window->Gley->Save and add your current platform");
#endif
            return;
        }
        _saveMethod.ClearAllData(path);
    }


    public void ClearFIle(string path)
    {
        if (_saveMethod == null)
        {
            Debug.LogError("Current platform is not added to plugin settings. Go to Window->Gley->Save and add your current platform");
            return;
        }
        _saveMethod.ClearFile(path);
    }
}
