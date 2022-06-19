namespace Jumpy
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Controls level complete UI
    /// </summary>
    public class LevelCompletePopup : GenericPopup
    {
        public Text scoreText;
        public Text highScoreText;
        private int _highScore;

        /// <summary>
        /// Called every time a popup is opened
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            //check if the new score is a highScore
            _highScore = GameStatus.SetHighScore(InGameInterface.currentDistance);

            //if it is a highScore play a sound with a 0.5 seconds delay
            if (_highScore == InGameInterface.currentDistance)
            {
                Invoke("NewHighscore", 0.5f);
            }

            //update popup texts
            scoreText.text = "Score: " + InGameInterface.currentDistance;
            highScoreText.text = "HighScore: " + _highScore;
        }


        private void NewHighscore()
        {
            SoundLoader.AddFxSound("Highscore");
        }

        /// <summary>
        /// handles the button click actions
        /// </summary>
        /// <param name="button">the gameObject that was clicked</param>
        public override void PerformClickActionsPopup(GameObject button)
        {
            base.PerformClickActionsPopup(button);

            if (button.name == "RestartButton")
            {
                ClosePopup(true, () => LevelManager.Instance.RestartLevel());
            }
        }
    }
}
