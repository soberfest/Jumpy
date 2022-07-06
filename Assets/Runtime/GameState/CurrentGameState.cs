namespace Jumpy
{
    using UnityEngine;

    /// <summary>
    /// Keeps track of all game properties
    /// </summary>
    public class GameProgressManager
    {
        //The name of the filename for keeping the save game data
        private const string FileName = "GameValues";
        private SavedProperties _savedProperties;

        #region PublicMethods
        /// <summary>
        /// Load data from save
        /// </summary>
        public void LoadGameStatus()
        {
            SaveManager.Instance.Load<SavedProperties>(Application.persistentDataPath + "/" + FileName, LoadDone, false);
        }

        /// <summary>
        /// Save game data
        /// </summary>
        public void SaveGameStatus()
        {
            SaveManager.Instance.Save(_savedProperties, Application.persistentDataPath + "/" + FileName, SaveComplete, false);
        }


        /// <summary>
        /// Get player highScore
        /// </summary>
        /// <returns></returns>
        public int GetHighScore()
        {
            return _savedProperties.highScore;
        }


        /// <summary>
        /// Set player highScore only if needed
        /// </summary>
        /// <param name="score"></param>
        /// <returns>the current highScore</returns>
        public int SetHighScore(int score)
        {
            if (score > _savedProperties.highScore)
            {
                _savedProperties.highScore = score;
                SaveGameStatus();
            }
            return _savedProperties.highScore;
        }


        /// <summary>
        /// Used to set and get volume of special effects
        /// </summary>
        public float FXVolume
        {
            get => _savedProperties.fxVolume;
            set => _savedProperties.fxVolume = value;
        }


        /// <summary>
        /// Used to set and get background music volume
        /// </summary>
        public float MusicVolume
        {
	        get => _savedProperties.musicVolume;
	        set => _savedProperties.musicVolume = value;
        }

        #endregion


        #region PrivateMethods
        /// <summary>
        /// Load data callback
        /// </summary>
        /// <param name="data">the actual data</param>
        /// <param name="result">the result of the load process: Success/Error/Empty</param>
        /// <param name="message"></param>
        private void LoadDone(SavedProperties data, SaveResult result, string message)
        {
            if (result == SaveResult.Success)
            {
                _savedProperties = data;
            }
            else
            {
                _savedProperties = new SavedProperties();
                Debug.Log("Load failed " + message);
            }
        }


        /// <summary>
        /// Save completed callback
        /// </summary>
        /// <param name="result"></param>
        /// <param name="error"></param>
        private void SaveComplete(SaveResult result, string error)
        {

        }
        #endregion
    }
}
