namespace Jumpy
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Loads and updates all level background elements
    /// </summary>
    public class LevelBuilder : MonoBehaviour
    {
	    private const float TopYPozChickens = 12f; //the top position of chicken background animation
	    private const float WallObjectDimension = 1.4f; //the dimension of a wall object prefab
	    private const float BgObjectDimension = 12.8f; //the dimension of a background image

	    private GameObject _firstWallObject;
        private GameObject _firstBgObject;
        private GameObject _chickens;

        private Transform _bottomLeftPoz;
        private Transform _bottomRightPoz;
        private Transform _topLeftPoz;
        private Transform _topRightPoz;

        private Queue _rightWallObjects;
        private Queue _leftWallObjects;
        private Queue _bgObjects;

        private Camera _mainCamera;


        #region PublicMethods
        /// <summary>
        /// Adding all level elements on screen
        /// </summary>
        public void ConstructLevel(Camera mainCamera)
        {
            _mainCamera = mainCamera;
            CreateScreenLimits();

            //add background and walls
            var wallElement = Resources.Load<GameObject>("Level/SideGears");
            var bg = Resources.Load<GameObject>("Level/Background");
            var leftWall = new GameObject("LeftWall");
            leftWall.transform.SetParent(_bottomLeftPoz, false);
            var rightWall = new GameObject("RightWall");
            rightWall.transform.SetParent(_bottomRightPoz, false);
            rightWall.transform.localScale = new Vector3(-1, 1, 1);

            _leftWallObjects = new Queue(ConstructWall(leftWall, wallElement, WallObjectDimension, false));
            _rightWallObjects = new Queue(ConstructWall(rightWall, wallElement, WallObjectDimension, false));
            _bgObjects = new Queue(ConstructWall(gameObject, bg, BgObjectDimension, true));

            _firstWallObject = _rightWallObjects.Dequeue() as GameObject;
            _firstBgObject = _bgObjects.Dequeue() as GameObject;

            //add background animations
            _chickens = Instantiate(Resources.Load<GameObject>("Level/BGChickens"), gameObject.transform, true);
            _chickens.transform.position = new Vector3(0, TopYPozChickens, 0);
        }


        /// <summary>
        /// Called by LevelManager to update background objects based on player position
        /// </summary>
        public void FollowPlayer()
        {
	        if (_firstWallObject.transform.position.y <
	            _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, -_mainCamera.transform.position.z)).y -
	            WallObjectDimension / 2)
            {
                UpdateWallQueue();
            }

            if (_firstBgObject.transform.position.y <
                _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, -_mainCamera.transform.position.z)).y -
                BgObjectDimension * _firstBgObject.transform.localScale.x / 2)
            {
                UpdateBgQueue();
            }

            if (_chickens.transform.position.y < _mainCamera
	                .ScreenToWorldPoint(new Vector3(Screen.width, 0, -_mainCamera.transform.position.z)).y - 1)
            {
                UpdateChickens();
            }
        }


        /// <summary>
        /// Returns bottom left position of the screen
        /// </summary>
        /// <returns></returns>
        public Transform GetBottomLeftPoz()
        {
            return _bottomLeftPoz;
        }


        /// <summary>
        /// Returns bottom right position of the screen
        /// </summary>
        /// <returns></returns>
        public Transform GetBottomRightPoz()
        {
            return _bottomRightPoz;
        }


        /// <summary>
        /// Revert to initial background configuration
        /// </summary>
        public void ResetBG()
        {
            _chickens.transform.position = new Vector3(0, TopYPozChickens, 0);
            ResetWall();
            ResetBg();
        }
        #endregion


        #region ConstructLevel
        /// <summary>
        /// Create helping gameObjects at the edge of the screen
        /// </summary>
        private void CreateScreenLimits()
        {
            _bottomLeftPoz = new GameObject("BottomLeft").transform;
            _bottomLeftPoz.transform.SetParent(gameObject.transform);
            Alignment.Align(_bottomLeftPoz, AlignPosition.BottomLeft, _mainCamera);

            _bottomRightPoz = new GameObject("BottomRight").transform;
            _bottomRightPoz.transform.SetParent(gameObject.transform);
            Alignment.Align(_bottomRightPoz, AlignPosition.BottomRight, _mainCamera);

            _topLeftPoz = new GameObject("TopLeft").transform;
            _topLeftPoz.transform.SetParent(gameObject.transform);
            Alignment.Align(_topLeftPoz, AlignPosition.TopLeft, _mainCamera);

            _topRightPoz = new GameObject("TopRight").transform;
            _topRightPoz.transform.SetParent(gameObject.transform);
            Alignment.Align(_topRightPoz, AlignPosition.TopRight, _mainCamera);
        }


        /// <summary>
        /// Create an array of objects placed side by side
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="objToMultiply"></param>
        /// <param name="_objectDimension"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        private List<GameObject> ConstructWall(GameObject holder, GameObject objToMultiply, float _objectDimension, bool scale)
        {
            var objects = new List<GameObject>();
            float wallhight = 0;
            int nr = 0;
            while (wallhight < (_topRightPoz.position.y + _objectDimension))
            {
                float scaleFactor = 1;
                if (scale)
                {
                    scaleFactor = (Screen.width * 16f) / (Screen.height * 9f);
                    if (scaleFactor < 1)
                    {
                        scaleFactor = 1;
                    }
                }
                var wall = Instantiate(objToMultiply, holder.transform, true);
                wall.transform.localPosition = new Vector3(0, nr * _objectDimension * scaleFactor, 0);
                wall.transform.localScale = new Vector3(scaleFactor, scaleFactor, wall.transform.localScale.z);
                objects.Add(wall);
                wallhight = wall.transform.position.y;
                nr++;
            }
            return objects;
        }
        #endregion


        #region UpdateLevel
        /// <summary>
        /// Update walls based on player position
        /// </summary>
        private void UpdateWallQueue()
        {
	        var localPosition = _firstWallObject.transform.localPosition;
	        localPosition = new Vector3(localPosition.x, localPosition.y + WallObjectDimension * (_rightWallObjects.Count + 1), localPosition.z);
	        _firstWallObject.transform.localPosition = localPosition;
	        _rightWallObjects.Enqueue(_firstWallObject);
            _firstWallObject = _rightWallObjects.Dequeue() as GameObject;

            GameObject leftObject = _leftWallObjects.Dequeue() as GameObject;
            var position = leftObject.transform.localPosition;
            position = new Vector3(position.x, position.y + WallObjectDimension * (_leftWallObjects.Count + 1), position.z);
            leftObject.transform.localPosition = position;
            _leftWallObjects.Enqueue(leftObject);
        }


        /// <summary>
        /// Update background images based on player position
        /// </summary>
        private void UpdateBgQueue()
        {
	        var localPosition = _firstBgObject.transform.localPosition;
	        localPosition = new Vector3(localPosition.x, localPosition.y + BgObjectDimension * (_bgObjects.Count + 1), localPosition.z);
	        _firstBgObject.transform.localPosition = localPosition;
	        _bgObjects.Enqueue(_firstBgObject);
            _firstBgObject = _bgObjects.Dequeue() as GameObject;
        }


        /// <summary>
        /// Update background chickens animation based on player position
        /// </summary>
        private void UpdateChickens()
        {
	        var position = _chickens.transform.position;
	        position = new Vector3(position.x, position.y + 20, position.z);
	        _chickens.transform.position = position;
	        _chickens.transform.localScale = new Vector3(_chickens.transform.localScale.x * -1, 1, 1);
        }
        #endregion


        #region ResetLevel
        /// <summary>
        /// Put all walls in their original position
        /// </summary>
        private void ResetWall()
        {
            for (int i = 0; i <= _rightWallObjects.Count; i++)
            {
	            var localPosition = _firstWallObject.transform.localPosition;
	            localPosition = new Vector3(localPosition.x, WallObjectDimension * i, localPosition.z);
	            _firstWallObject.transform.localPosition = localPosition;
	            _rightWallObjects.Enqueue(_firstWallObject);
                _firstWallObject = _rightWallObjects.Dequeue() as GameObject;

                GameObject leftObject = _leftWallObjects.Dequeue() as GameObject;
                var position = leftObject.transform.localPosition;
                position = new Vector3(position.x, WallObjectDimension * i, position.z);
                leftObject.transform.localPosition = position;
                _leftWallObjects.Enqueue(leftObject);
            }
        }


        /// <summary>
        /// Put all background images in their original position
        /// </summary>
        private void ResetBg()
        {
            for (int i = 0; i <= _bgObjects.Count; i++)
            {
	            var localPosition = _firstBgObject.transform.localPosition;
	            localPosition = new Vector3(localPosition.x, BgObjectDimension * i, localPosition.z);
	            _firstBgObject.transform.localPosition = localPosition;
	            _bgObjects.Enqueue(_firstBgObject);
                _firstBgObject = _bgObjects.Dequeue() as GameObject;
            }
        }
        #endregion
    }
}
