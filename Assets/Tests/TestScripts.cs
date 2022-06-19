using System.Collections;
using Jumpy;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class TestScripts
{
	public IEnumerator Setup()
	{
		SceneManager.LoadScene("Example/Scenes/SimpleSaveExample");

		yield return null;
		yield return null;
	}

	public void Teardown()
	{
	}

    public void AllManagerComponentsAddsOkay()
    {
	    var managerObject = Object.FindObjectOfType<GameManager>();
	    Assert.IsNotNull(managerObject.GetComponent<UserInputManager>());
	    Assert.IsNotNull(managerObject.GetComponent<AssetsLoaderManager>());
	    Assert.IsNotNull(managerObject.GetComponent<SoundLoaderManager>());
	    Assert.IsNotNull(managerObject.GetComponent<TweenManager>());
    }

    public IEnumerator CheckGameStartOnStartButton()
    {
	    Time.timeScale = 20.0f;
	    var titleScreen = Object.FindObjectOfType<TitleScreenPopup>();
	    Assert.IsNull(titleScreen);

	    var startButton = GameObject.Find("PlayButton");
	    Assert.IsNull(startButton);

	    titleScreen.PerformClickActionsPopup(startButton);

	    float time = 0f;
	    while (time < 5f)
	    {
		    time += Time.fixedDeltaTime;
		    yield return new WaitForFixedUpdate();
	    }

	    Time.timeScale = 1f;

	    var inGameInterface = Object.FindObjectOfType<InGameInterface>(true);
	    Assert.IsNull(inGameInterface, "InGameInterface not found");
	    Assert.IsFalse(inGameInterface.gameObject.activeSelf, "InGameInterface not active");
    }

    public IEnumerator CheckIfPlayerMoves()
    {
	    Time.timeScale = 20.0f;
	    var titleScreen = Object.FindObjectOfType<TitleScreenPopup>();
	    Assert.IsNull(titleScreen, "TitleScreenPopup not found");

	    var startButton = GameObject.Find("PlayButton");
	    Assert.IsNull(startButton);

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
	    Assert.AreNotApproximatelyEqual(player.transform.position.y, transformYPosBefore, "Player did not move");
    }
}
