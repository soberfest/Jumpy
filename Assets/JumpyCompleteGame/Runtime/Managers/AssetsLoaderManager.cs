namespace Jumpy
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    //all full scree UI from the game
    public enum GameInterfeces
    {
        InGameInterface
    }

    //all pupup UI from the game
    public enum GamePopups
    {
        TitleScreenPopup,
        PausePopup,
        LevelCompletePopup
    }


    /// <summary>
    /// Used to load all UI from the game
    /// </summary>
    public class AssetsLoaderManager : SingleReference<AssetsLoaderManager>
    {
        /// <summary>
        /// The canvas for full screen UI
        /// </summary>
        private Transform _mainCanvas;
        private Transform MainCanvas
        {
            get
            {
                if (_mainCanvas == null)
                {
                    _mainCanvas = GameObject.Find("MainCanvas").transform;
                }
                return _mainCanvas;
            }
        }

        /// <summary>
        /// The canvas for popups, needs to be on top of MainCnavas 
        /// </summary>
        private Transform _popupCanvas;
        private Transform PopupCanvas
        {
            get
            {
                if (_popupCanvas == null)
                {
                    _popupCanvas = GameObject.Find("PopupCanvas").transform;
                }
                return _popupCanvas;
            }
        }


        private List<GameObject> _allInterfaces = new List<GameObject>();
        private List<GameObject> _allPopups = new List<GameObject>();
        private GameObject _currentInterface;

        /// <summary>
        /// Event triggered when a game popup is closed
        /// </summary>
        /// <param name="parent">the full scree UI that needs to become interactable after popup is closed</param>
        public delegate void PopupWasClosed(string parent);
        public static event PopupWasClosed OnPopupWasClosed;
        public static void TriggerPopupWasClosedEvent(string parent)
        {
	        OnPopupWasClosed?.Invoke(parent);
        }

        /// <summary>
        /// Preload all game UI before showing
        /// </summary>
        public void PrepareGameUI()
        {
            List<GameInterfeces> all = HelperMethods.GetValues<GameInterfeces>();
            for (int i = 0; i < all.Count; i++)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("UI/" + all[i]), MainCanvas, false);
                go.name = all[i].ToString();
                go.SetActive(false);
                _allInterfaces.Add(go);
            }

            List<GamePopups> popups = HelperMethods.GetValues<GamePopups>();
            for (int i = 0; i < popups.Count; i++)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("UI/" + popups[i]), PopupCanvas, false);
                go.name = popups[i].ToString();
                go.SetActive(false);
                _allPopups.Add(go);
            }
        }


        /// <summary>
        /// Load a full screen UI
        /// </summary>
        /// <param name="whatToLoad">The name of the full screen UI</param>
        public void LoadInterface(GameInterfeces whatToLoad)
        {
            _currentInterface = _allInterfaces.First(cond => cond.name == whatToLoad.ToString());
            _currentInterface.SetActive(true);
            _currentInterface.GetComponent<CanvasGroup>().interactable = false;
        }


        /// <summary>
        /// Load a popup
        /// </summary>
        /// <param name="popupName">the name of the popup</param>
        /// <param name="parent">The full screen UI it is loaded from(this will not be interactable anymore)</param>
        public void LoadPopup(GamePopups popupName, GenericInterfaceController parent)
        {
            GameObject currentPopup = _allPopups.First(cond => cond.name == popupName.ToString());
            currentPopup.SetActive(true);
            currentPopup.GetComponent<CanvasGroup>().interactable = false;
            currentPopup.GetComponent<GenericPopup>().Initialize();
            if (parent)
            {
                parent.PopupIsActive(currentPopup.GetComponent<GenericPopup>());
            }
        }


        /// <summary>
        /// Returns the current full screen UI GameObject 
        /// </summary>
        /// <returns></returns>
        public GameObject GetCurrentInterface()
        {
            return _currentInterface;
        }


        /// <summary>
        /// Returns the main canvas transform
        /// </summary>
        /// <returns></returns>
        public Canvas GetMainCanvas()
        {
            return MainCanvas.GetComponent<Canvas>();
        }
    }
}
