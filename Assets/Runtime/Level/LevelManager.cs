namespace Jumpy
{
    using System.Collections;
    using UnityEngine;

    public class LevelManager : SingleReference<LevelManager>
    {
	    private const int CountTo = 4; // the length of the start counter

	    #region GameVariables
        private float _cornDistance = 25; // distance at which a power up appears

        private Camera _mainCamera;
        private GameObject _playerStartPoz;
        private GameObject _player;
        private InGameInterface _inGameInterface;
        private Player _playerScript;
        private Corn _cornScript;
        private Cylinder _cylinderScript;
        private LevelBuilder _levelBuilder;
        private LevelObstacles _levelObstacles;

        private int _counter;
        private bool _idleAnimation;

        private bool _cameraFollow;
        private bool _levelStarted;
        private bool _levelComplete;
        private Transform _yPozObstacle;
        private Transform _bottomLeftPoz;
        private Transform _bottomRightPoz;
        #endregion


        /// Contains methods related to start, restart, update level, and level complete 
        #region MainGameplayMethods
        private void Start()
        {
            //make game camera reference
            _mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

            //load all level features
            ConstructLevel();

            //load player gameObject
            LoadPlayer();

            //load bottom cylinder
            LoadCylinder();

            //load power up
            LoadCorn();

            //load Obstacles
            LoadObstacles();

            //start idle animations
            _idleAnimation = true;
            _cameraFollow = true;

        }


        private void Update()
        {
            //move the background without starting the level
            if (_idleAnimation)
            {
                MakeIdleAnimation();
            }

            //follow the player with main camera
            if (_cameraFollow)
            {
                FollowPlayer();
            }

            //level is in progress
            if (_levelStarted)
            {
                //generate corn when a certain distance is reached
                if (_yPozObstacle.position.y > _cornDistance)
                {
                    _cornDistance += _cornDistance;
                    ShowCorn();
                }

                //update level obstacles
                _levelObstacles.UpdateObstacles();

                //check if bottom cylinder touched the player
                if (_cylinderScript.CheckForDeath())
                {
                    GameManager.instance.SoundLoader.AddFxSound("Death(cylinder)");
                    LevelComplete();
                }
            }
        }


        /// <summary>
        /// This method is used to start a level
        /// </summary>
        internal void RestartLevel()
        {
            //show Rate Popup
            // RateGame.Instance.ShowRatePopup();

            ResetVars();

            //reset vars
            _levelComplete = false;
            _idleAnimation = false;
            _cameraFollow = false;

            _counter = CountTo;

            //reset player
            _playerScript.ResetPlayer();
            _player.transform.position = _playerStartPoz.transform.position;

            //reset corn
            _cornScript.ResetCornPosition();

            //reset camera with animation
            var transform1 = _mainCamera.transform;
            transform1.position = new Vector3(0, 3, -10);
            GameManager.Instance.Tween.MoveTo(transform1, 0, 0, -100, 0.4f, StartFollowingPlayer);

            //reset bottom cylinder
            _cylinderScript.ResetPosition();

            //reset bg animations
            _levelBuilder.ResetBG();

            //reset obstacles position
            _levelObstacles.ResetObstacles();

            //reset interface
            _inGameInterface = GameManager.instance.AssetsLoader.GetCurrentInterface().GetComponent<InGameInterface>();
            _inGameInterface.RestartLevel();
        }


        /// <summary>
        /// Load all level objects
        /// </summary>
        private void ConstructLevel()
        {
            _levelBuilder = gameObject.AddComponent<LevelBuilder>();
            _levelBuilder.ConstructLevel(_mainCamera);

            //make screen edge references          
            _bottomLeftPoz = _levelBuilder.GetBottomLeftPoz();
            _bottomRightPoz = _levelBuilder.GetBottomRightPoz();
        }


        /// <summary>
        /// Load level obstacles
        /// </summary>
        private void LoadObstacles()
        {
            _levelObstacles = gameObject.AddComponent<LevelObstacles>();
            _levelObstacles.LoadObstacles(_bottomLeftPoz, _bottomRightPoz, _mainCamera);
            _yPozObstacle = _levelObstacles.GetYPozObstacle();
        }

        /// <summary>
        /// Add bottom cylinder
        /// </summary>
        private void LoadCylinder()
        {
            var cylinder = Instantiate(Resources.Load<GameObject>("Level/Roll"));
            _cylinderScript = cylinder.AddComponent<Cylinder>();
            _cylinderScript.Follow(false);
            _cylinderScript.SetPlayer(_player.transform);
            _cylinderScript.ResetPosition();
        }


        /// <summary>
        /// Add corn power up
        /// </summary>
        private void LoadCorn()
        {
            var corn = Instantiate(Resources.Load<GameObject>("Level/Corn"));
            _cornScript = corn.GetComponent<Corn>();
        }

        /// <summary>
        /// Enables camera follow after camera animation is done
        /// </summary>
        private void StartFollowingPlayer()
        {
            _cameraFollow = true;
        }


        /// <summary>
        /// properties that need to be reset at the moment of death
        /// </summary>
        private void ResetVars()
        {
            GameManager.instance.Tween.StopAllTweens();
            _playerScript.Slide(false);
            _cylinderScript.Follow(false);
            _levelStarted = false;
        }


        /// <summary>
        /// called after the countdown is finished
        /// starts generating level obstacles
        /// </summary>
        private void StartLevel()
        {
            _cylinderScript.Follow(true);
            _levelObstacles.AddObstacles();
            _playerScript.Slide(true);
            _inGameInterface.ShowCounter(false);
            _levelStarted = true;
        }


        /// <summary>
        /// called when player is death
        /// </summary>
        public void LevelComplete()
        {
            if (_levelComplete == false)
            {
                ResetVars();

                _levelComplete = true;
                _playerScript.DeathAnimation();

                Invoke("LoadCompletepopup", 1);
            }
        }


        /// <summary>
        /// Loads level complete popup
        /// </summary>
        private void LoadCompletepopup()
        {
            _inGameInterface.LevelComplete();
        }
        #endregion


        /// Contains user interaction methods
        #region PlayerMovement
        /// <summary>
        /// Called by InGameInterface when user taps the screen and plays the required prepare to jump animation
        /// </summary>
        internal void ButtonPressed()
        {
            _playerScript.PrepareToJump();
        }


        /// <summary>
        /// Called by InGameInterface during button hold 
        /// </summary>
        /// <param name="pressTime"></param>
        internal void ScaleHold(float pressTime)
        {
            _playerScript.ScaleChicken(pressTime);
        }


        /// <summary>
        /// Called by InGameInterface when user released a button
        /// </summary>
        /// <param name="pressTime">total press time</param>
        internal void ButtonReleased(float pressTime)
        {
            //make player jump based on press time
            _playerScript.Jump(pressTime, _bottomRightPoz, _bottomLeftPoz);
        }


        /// <summary>
        /// called when a jump is complete and counts the jumps
        /// </summary>
        internal void JumpComplete()
        {
            //updates start counter
            _counter--;
            if (_counter >= 0)
            {
                if (_counter == 0)
                {
                    StartLevel();
                }
                _inGameInterface.UpdateCounter(_counter.ToString());
            }
            else
            {
                _playerScript.Slide(true);
            }

            //update in game interface
            _inGameInterface.EnableGameplayButton();
            _inGameInterface.UpdateDistance(((int)_mainCamera.transform.position.y));
        }
        #endregion


        ///Contains camera update methods, and environment update methods
        #region BackgroundUpdate
        /// <summary>
        /// used for background animation in Title Screen
        /// </summary>
        private void MakeIdleAnimation()
        {
            _player.transform.Translate(0, Time.deltaTime, 0);
        }


        /// <summary>
        /// Updates the entire background and camera based on player position
        /// </summary>
        private void FollowPlayer()
        {
            _levelBuilder.FollowPlayer();

            if (_mainCamera.transform.position.y < _player.transform.position.y)
            {
                _mainCamera.transform.position = new Vector3(0, _player.transform.position.y, -10);
            }
        }
        #endregion


        ///Loads the player prefabs and scripts associated
        #region LoadPlayer
        /// <summary>
        /// Load player object
        /// </summary>
        private void LoadPlayer()
        {
            _playerStartPoz = new GameObject("PlayerStartPoz");
            _playerStartPoz.transform.SetParent(_bottomLeftPoz);
            _playerStartPoz.transform.localPosition = new Vector3(1, 1.56f, 0);

            _player = Instantiate(Resources.Load<GameObject>("Level/Player"));

            _playerScript = _player.GetComponent<Player>();
            _playerScript.ShowPlayer(false);
        }
        #endregion


        ///Contains all power up related methods
        #region CornPowerUp
        /// <summary>
        /// instantiates a corn power up when needed
        /// </summary>
        private void ShowCorn()
        {
            _cornScript.ShowCorn(_yPozObstacle.position.y);
        }


        /// <summary>
        /// Triggers when a corn was collected
        /// </summary>
        public void CornCollected()
        {
            _cornScript.HideCorn();
            GameManager.instance.SoundLoader.AddFxSound("Collect");
            _playerScript.ShowBubble(true);
            _playerScript.EnableCollider(false);
            _player.GetComponent<Collider>().enabled = false;
            StartCoroutine(CollectedAnimation());
        }


        /// <summary>
        /// Waits for animation to finish and to disable the power up
        /// </summary>
        /// <returns></returns>
        private IEnumerator CollectedAnimation()
        {
            Invoke("EndSound", 3.5f);
            float timeToEnd = 5;
            while (timeToEnd > 0)
            {
                timeToEnd -= Time.deltaTime;
                yield return null;
            }
            DisableSpecialAbility();
        }


        /// <summary>
        /// Plays a sound to alert the user that power up will end soon
        /// </summary>
        private void EndSound()
        {
            GameManager.instance.SoundLoader.AddFxSound("CollectableAlert");
            _playerScript.ShowBubble(false);
        }


        /// <summary>
        /// End power up
        /// </summary>
        private void DisableSpecialAbility()
        {
            _playerScript.EnableCollider(true);
        }
        #endregion
    }
}
