using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Jumpy;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Profiling.Experimental;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TestSuite
{
    // A Test behaves as an ordinary method
    [Test]
    public void TestSuiteSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TestSuiteWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}


public class TestScripts
{
	// [UnitySetUp] -  Метод,в котором делаем Предусловия для тестов ниже
	// Используем корутины ( IEnumerator) для выполнения Предусловия
	// Обращаемся к SceneManager - родительскому классу загрузки сцен
	// Предусловие для каждого теста - загрузку сцены "Game"
	//
	//
	// 'yield return null'  - пропускаем два кадра, чтобы дождаться загрузки сцены

	[UnitySetUp]
	public IEnumerator Setup()
	{
		SceneManager.LoadScene("Game");
		yield return null;
		yield return null;
	}

	// [TearDown] - Метод,в котором делаем Постусловие. То есть то, что будет выполняться ПОСЛЕ тестов
	// Наш метод пустой, так как мы не используем пост-условие в этих тестах
	// Можно навести на каждый из компонентов и перейти на соответствующий скрипт, который выпадает в подсказке
	[TearDown]
	public void Teardown()
	{
	}
	/*
[UnityTest] - это тест. Давайте проверим что игра стартует по кнопке Start

1) это тест - он проверяем что игра стартует по кнопке Start
2) Ставим ускорение игрового времени в 20 раз, чтобы тест был быстрее
3) Затем находим заголовок попапа
4) Сделаем утверждение (Assert), что заголовок попапа не пустой
5) Следущим этапом объявим переменной startButton найденную нами кнопку PlayButton
6) Делаем утверждение (Assert), что кнопка находится на экране (не равна отсутствию кнопки (null))
7) Нажимаем на кнопку startButton
8) Задаю переменную time с типом float и запущу цикл while (будем за каждую единицу времени пропускать кадр примерно 5 раз)

Time.DeltaTime стандартный класс юнити, почитать о нем можно тут: https://docs.unity3d.com/Manual/TimeFrameManagement.html 
	*/
	[UnityTest]
	public IEnumerator CheckGameStartOnStartButton()
	{
		Time.timeScale = 20.0f;
		var titleScreen = Object.FindObjectOfType<TitleScreenPopup>();
		{
			Assert.IsNotNull(titleScreen);
		}

		var startButton = GameObject.Find("PlayButton");
		{
			Assert.IsNotNull(startButton);
		}

		titleScreen.PerformClickActionsPopup(startButton);
		var time = 0f;
		while (time < 5f)      // цикл                      			   
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}

		/*
9) После того как функция выйдет из цикла устанавливаем скейл времени кратный единице(нормальный)
10) Находим на сцене объект InGameInterface - это как раз старт нашей игры
11) Затем делаю утверждение(Assert), что игровой интерфейс загрузился и он активен 

Результат можно записать в лог
		*/
		Time.timeScale = 1f;
		var inGameInterface = Object.FindObjectOfType<InGameInterface>(true);
		{
			Assert.IsNotNull(inGameInterface);
			Assert.IsTrue(inGameInterface.gameObject.activeSelf);
		}
		Debug.Log(inGameInterface + (" loaded sucsess"));
	}
}