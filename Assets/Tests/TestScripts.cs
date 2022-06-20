using System.Collections;
using Jumpy;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TestScripts
{
	[UnitySetUp]
	public IEnumerator Setup()
	{
		SceneManager.LoadScene("Game");

		yield return null;
		yield return null;
	}

	[TearDown]
	public void Teardown()
	{
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
	    Assert.IsNotNull(inGameInterface, "InGameInterface not found");
	    Assert.IsTrue(inGameInterface.gameObject.activeSelf, "InGameInterface not active");
    }

    [UnityTest]
    public IEnumerator CheckIfPlayerMoves()
    {
	    Time.timeScale = 20.0f;
	    var titleScreen = Object.FindObjectOfType<TitleScreenPopup>();
	    Assert.IsNotNull(titleScreen, "TitleScreenPopup not found");

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

	    var player = Object.FindObjectOfType<Player>();
	    Assert.IsNotNull(player, "Player not found");
	    var transformYPosBefore = player.transform.position.y;

	    LevelManager.Instance.ButtonReleased(1.0f);
	    yield return new WaitForFixedUpdate();
	    yield return new WaitForFixedUpdate();
	    Assert.Greater(player.transform.position.y, transformYPosBefore, "Player did not move");
    }
}
