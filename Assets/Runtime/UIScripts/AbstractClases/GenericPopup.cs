namespace Jumpy
{
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// This class should be extended by any popup in the game
    /// It contains all methods necessary to interact with a popup
    /// </summary>
    public abstract class GenericPopup : MonoBehaviour
    {
        /// <summary>
        /// reference to required game components
        /// </summary>
        private AssetsLoaderManager _assetsLoader;
        protected AssetsLoaderManager AssetsLoader
        {
            get
            {
                if (_assetsLoader == null)
                {
                    _assetsLoader = GameManager.Instance.AssetsLoader;
                }
                return _assetsLoader;
            }
        }

        private SoundLoaderManager _soundLoader;
        protected SoundLoaderManager SoundLoader
        {
            get
            {
                if (_soundLoader == null)
                {
                    _soundLoader = GameManager.Instance.SoundLoader;
                }
                return _soundLoader;
            }
        }

        private GameProgressManager _gameStatus;
        protected GameProgressManager GameStatus
        {
            get
            {
                if (_gameStatus == null)
                {
                    _gameStatus = GameManager.Instance.GameStatus;
                }
                return _gameStatus;
            }
        }
        protected GenericInterfaceController Parent;


        private CanvasGroup _canvasGroup;
        private CanvasGroup _popupCanvasGroup;
        private UnityAction _actionOnDestroy;
        private GameObject _popupBg;
        private bool _enableInterface;
        private bool _backButtonPressed;
        private bool _buttonsAreEnabled;

        private readonly float _maxAlpha = 0.5f;

        /// <summary>
        /// Called every time a popup activates
        /// Used for initializations
        /// </summary>
        public virtual void Initialize()
        {
            //a popup background is added automatically each time a popup is opened
            _popupBg = Instantiate(Resources.Load<GameObject>("Loading/PopupBg"),
	            GameObject.Find("PopupCanvas").transform, false);
            _popupBg.transform.SetSiblingIndex(transform.GetSiblingIndex());
            _popupCanvasGroup = _popupBg.GetComponent<CanvasGroup>();
            _popupCanvasGroup.alpha = 0;

            _canvasGroup = GetComponent<CanvasGroup>();
            SoundLoader.AddFxSound("Woosh");

            //fade the popup background
            GameManager.Instance.Tween.ValueTo(0, _maxAlpha, 0.5f, UpdateAlpha);

            //Start the intro animation and wait for it to finish
            AnimatorEventsTrigger.onDoneAnimation += LoadingAnimationDone;
            Animator animator = gameObject.GetComponent<Animator>();
            animator.enabled = true;
            animator.SetTrigger("StartAnimation");
        }


        /// <summary>
        /// Called by tween every frame and updates the popup background alpha 
        /// </summary>
        /// <param name="alpha">the alpha value received</param>
        public virtual void UpdateAlpha(float alpha)
        {
            _popupCanvasGroup.alpha = alpha;
        }

        /// <summary>
        /// Triggers when intro animation finishes
        /// </summary>
        /// <param name="stateinfo"></param>
        public virtual void LoadingAnimationDone(AnimatorStateInfo stateinfo)
        {
            if (stateinfo.IsName("PopupInAnimation"))
            {
                AnimatorEventsTrigger.onDoneAnimation -= LoadingAnimationDone;
                EnablePopup();
            }
        }


        /// <summary>
        /// Enable user interaction with this popup
        /// </summary>
        public virtual void EnablePopup()
        {
            if (_buttonsAreEnabled == false)
            {
                _buttonsAreEnabled = true;
                UserInputManager.onButtonUp += PerformClickActionsPopup;
                UserInputManager.onBackButtonPressed += PerformBackButtonAction;
                _canvasGroup.interactable = true;
            }
        }


        /// <summary>
        /// Disable user interaction with this popup
        /// </summary>
        public virtual void DisablePopup()
        {
            if (_buttonsAreEnabled)
            {
                _buttonsAreEnabled = false;
                UserInputManager.onButtonUp -= PerformClickActionsPopup;
                UserInputManager.onBackButtonPressed -= PerformBackButtonAction;
                _canvasGroup.interactable = false;
            }
        }


        /// <summary>
        /// Should be overridden to be able to implement popup button interactions
        /// </summary>
        /// <param name="button"></param>
        public virtual void PerformClickActionsPopup(GameObject button)
        {
            SoundLoader.AddFxSound("Button");
        }


        /// <summary>
        /// Called to close this popup
        /// </summary>
        /// <param name="enableInterface">he interface from which the current popup was opened</param>
        /// <param name="actionWhenPopupCLoses">an Unity action that is triggered after the current popup is closed</param>
        public virtual void ClosePopup(bool enableInterface = true, UnityAction actionWhenPopupCLoses = null)
        {
            if (actionWhenPopupCLoses != null)
            {
                _actionOnDestroy = actionWhenPopupCLoses;
            }
            DisablePopup();
            this._enableInterface = enableInterface;


            GameManager.Instance.Tween.ValueTo(_maxAlpha, 0, 0.3f, UpdateAlpha);

            AnimatorEventsTrigger.onDoneAnimation += EndAnimationDone;
            gameObject.GetComponent<Animator>().SetTrigger("EndAnimation");
        }


        /// <summary>
        /// popup close animation is done, so may the close action
        /// </summary>
        /// <param name="stateinfo"></param>
        public void EndAnimationDone(AnimatorStateInfo stateinfo)
        {
            if (stateinfo.IsName("PopupOutAnimation"))
            {
                AnimatorEventsTrigger.onDoneAnimation -= EndAnimationDone;
                DestroyPopup();
            }
        }


        /// <summary>
        /// Hide the current popup 
        /// </summary>
        public virtual void DestroyPopup()
        {
            if (_actionOnDestroy != null)
            {
                _actionOnDestroy.Invoke();
                _actionOnDestroy = null;
            }
            gameObject.SetActive(false);
            Destroy(_popupBg);

            if (_enableInterface)
            {
                AssetsLoaderManager.TriggerPopupWasClosedEvent(gameObject.name);
            }
        }


        /// <summary>
        /// Called when back button on android is pressed
        /// </summary>
        private void PerformBackButtonAction()
        {
            if (_backButtonPressed == false)
            {
                BackButtonPresssed();
            }
            _backButtonPressed = true;
        }


        /// <summary>
        /// Should be overridden to implement a more complex action when back button is pressed 
        /// Currently it closes the popup
        /// </summary>
        public virtual void BackButtonPresssed()
        {
            ClosePopup();
        }
    }
}
