# QFramework



​	目前这个框架只经历了一款游戏项目,所以很多地方做得不是很完善。以后还需要多做项目多打磨,就像自己的孩子一样,要慢慢养大。

如果大家想要打造自己的游戏框架的话可以在这个框架中找到借鉴,如果想商用这个框架的话,我会说坑会有很多,框架还不够健壮,存在很多设计问题,随着本人的能力提升,相信框架会越来越完善。

----------------

2016年7月1日更新:
​	这个框架已经用在公司的项目上了,开始第二次打磨了.....

2016年7月8日更新:
	有很多朋友反馈说游戏一运行就会报错,是由于GameManager没有实现。因为这个框架是边做项目边进行更新的,而GameManager是根据不同的游戏实现的,所以希望大家自己实现一份。在这里我只提供样例的源码。也谢谢反馈的朋友们。
	
```
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using QFramework;
/*
 *  1.是游戏的入口
 *  2.不同功能模块的通信的中转站（当然用发消息的模式可能更好一些)
 *  3.处理游戏的一些挂起,进入后台等待特殊事件。
 *  4.一些资源的预加载,释放资源
 *  5.游戏的主线程 控制网络消息和逻辑消息的调用(只负责调用不负责处理,当然这种写法也不是很好)
 *  6.大模块跳转,HOME->GAME,GAME->HOME
 */

/// <summary>
/// 管理所有的控制器,资源的加载 场景的切换都在左立做
/// </summary>

public class GameManager : QSingleton<GameManager> {

	/// <summary>
	/// 各种模块控制器
	/// </summary>
	public StageCtrl  stageCtrl;
	public PlayerCtrl playerCtrl;
	public GameScene gameScene; 
	public UICtrl uiCtrl;
	public HomeCtrl homeCtrl;
	public BgCtrl bgCtrl;
	public CameraCtrl cameraCtrl;

	private GameManager() {}

	/// <summary>
	/// 初始化
	/// </summary>
	public IEnumerator Init()
	{
		// 加载配置表数据
		yield return TableManager.Instance ().Init();

		// 初始化内存数据,可更改的数据
		yield return DataManager.Instance ().Init ();

		// 音频资源加载
		yield return SoundManager.Instance().Init();

	}
		

	/// <summary>
	/// 启动游戏
	/// </summary>
	public IEnumerator Launch()
	{
		SceneManager.Instance().EnterHomeScene();
		yield return null;
	}

	/// <summary>
	/// 加载游戏资源
	/// </summary>
	public IEnumerator LoadGameRes()
	{
		Debug.LogWarning ("加载游戏资源");
		yield return null;
	}


	public IEnumerator GameStart()
	{
		Debug.LogWarning ("开始游戏");
		yield return null;
	}
		
		
	/// <summary>
	/// 启动游戏
	/// </summary>
	Coroutine coEnterHome = null;
		
	/// <summary>
	/// 从Home进入游戏
	/// </summary>
	public IEnumerator EnterGame()
	{
		GAME_DATA.THEME = QMath.RandomWithParams (1, 3);
		GameManager.Instance ().uiCtrl.gameWnd.gameProgress.ResetView ();// 重置 
		GameManager.Instance ().bgCtrl.Idle ();
		Time.timeScale = 1.0f;
		if (coEnterHome != null) {
			App.Instance ().StopCoroutine (coEnterHome);
			coEnterHome = null;
		}
			
		// 重置乱七八糟的东西
//		uiCtrl.uiFade.FadeIn();
		uiCtrl.ResetProp ();
		PropModel.Instance ().InitModel ();

		StageModel.Instance().Switching = true;

		// 遮罩移动到中央 耗时0.5秒
		bgCtrl.SwitchBegan(GAME_DATA.THEME,true);		
		yield return new WaitForSeconds(0.5f);
		StageModel.Instance().Switching = true;

		GameManager.Instance ().playerCtrl.ResetPlayer ();					// 重置player

		// 移动后加载资源
		bool resLoaded = false;
		GoManager.Instance ().LoadStagePool (GAME_DATA.THEME, delegate {
			resLoaded = true;
		});

		// 等待资源加载完毕
		while (!resLoaded) {
			Debug.LogWarning (resLoaded.ToString ());
			yield return new WaitForEndOfFrame ();
		}

		//加载完毕后开始转换
		stageCtrl.DespawnStageAB();
		GameModel.Instance().Theme = GAME_DATA.THEME;
		stageCtrl.ResetLayer();

		yield return App.Instance().StartCoroutine(stageCtrl.Switch(GAME_DATA.THEME));

		GameModel.Instance().fsm.Start("idle");
		GameModel.Instance().fsm.HandleEvent("start");
		stageCtrl.BeginScroll();

		uiCtrl.uiFade.FadeOut();

		stageCtrl.SwitchEnded(GAME_DATA.THEME);

		SoundMgr.Instance ().PlayMusic (MUSIC.GAME_BG);

		yield return 0;
	}

	/// <summary>
	/// 从Game进入Home
	/// </summary>
	public IEnumerator EnterHome()
	{
		Debug.LogWarning ("@@@@@@@@@@@@@@@");
		SoundMgr.Instance ().PlayMusic (MUSIC.HOME_BG);
		yield return new WaitForEndOfFrame ();
		Time.timeScale = 0.0f;
	}

	/// <summary>
	/// Games the restart.
	/// </summary>
	public void GameRestart()
	{
		GameManager.Instance ().stageCtrl.DespawnStageAB (); 							// 回收掉当前的关卡
		App.Instance().StartCoroutine(EnterGame());
	}
		

	void OnApplicationQuit()
	{
		DataManager.Instance().Save (); // 数据加载
	}
}

```


框架主要分为三层

游戏层,游戏的逻辑等

管理层,调用底层

底层,对Api的一些封装

Unity Api