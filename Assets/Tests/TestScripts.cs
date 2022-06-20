using System.Collections;
using System.Collections.Generic;
using Jumpy;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TestScripts
{
	private GameObject _game;
	private List<GameObject> _instances = new List<GameObject>();

	[UnitySetUp]
	public IEnumerator Setup()
	{
		SceneManager.LoadScene("Game");

		yield return null;
		yield return null;

		// _instances.Add(Object.Instantiate(Resources.Load<GameObject>("Prefabs/Main Camera")));
		// _instances.Add(Object.Instantiate(Resources.Load<GameObject>("Prefabs/MainCanvas")));
		// _instances.Add(Object.Instantiate(Resources.Load<GameObject>("Prefabs/PopupCanvas")));
		// _instances.Add(Object.Instantiate(Resources.Load<GameObject>("Prefabs/EventSystem")));
		// _instances.Add(Object.Instantiate(Resources.Load<GameObject>("Prefabs/LevelObjects")));
	}

	[TearDown]
	public void Teardown()
	{
		foreach (var o in _instances)
		{
			Object.Destroy(o);
		}
	}

    [Test]
    public void AllManagerComponentsAddsOkay()
    {
	    var managerObject = Object.FindObjectOfType<GameManager>();
	    Assert.IsNotNull(managerObject.GetComponent<UserInputManager>());
	    Assert.IsNotNull(managerObject.GetComponent<AssetsLoaderManager>());
	    Assert.IsNotNull(managerObject.GetComponent<SoundLoaderManager>());
	    Assert.IsNotNull(managerObject.GetComponent<TweenManager>());
    }

    [UnityTest]
    public IEnumerator CheckGameStartOnStartButton()
    {
	    Time.timeScale = 20.0f;
	    var titleScreen = Object.FindObjectOfType<TitleScreenPopup>();
	    Assert.IsNotNull(titleScreen);

	    var startButton = GameObject.Find("PlayButton");
	    Assert.IsNotNull(startButton);

	    titleScreen.PerformClickActionsPopup(startButton);

	    float time = 0f;
	    while (time < 5f)
	    {
		    time += Time.fixedDeltaTime;
		    yield return new WaitForFixedUpdate();
	    }

	    Time.timeScale = 1f;

	    var inGameInterface = Object.FindObjectOfType<InGameInterface>(true);
	    Assert.IsNotNull(inGameInterface);
	    Assert.IsTrue(inGameInterface.gameObject.activeSelf);
    }
}
