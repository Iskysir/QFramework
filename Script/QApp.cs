﻿using UnityEngine;
using System.Collections;
using QFramework;
using QFramework.UI;

namespace QFramework {
	public enum QAppMode {
		Developing,
		QA,
		Release
	}


	/// <summary>
	/// 全局唯一继承于MonoBehaviour的单例类，保证其他公共模块都以App的生命周期为准
	/// </summary>
	public class QApp : MonoBehaviour
	{

		/// <summary>
		/// 组合的方式实现单例的模板
		/// </summary>
		/// <value>The instance.</value>
		public static QApp Instance {
			get {
				return QMonoSingletonComponent<QApp>.Instance;
			}
		}

		public QAppMode mode = QAppMode.Developing;

		private QApp() {}

		void Awake()
		{
			// 确保不被销毁
			DontDestroyOnLoad(gameObject);

			// 进入欢迎界面
			Application.targetFrameRate = 60;
		}

		void  Start()
		{
			QCoroutineMgr.Instance.StartCoroutine (ApplicationDidFinishLaunching());
		}

		/// <summary>
		/// 进入游戏
		/// </summary>
		IEnumerator ApplicationDidFinishLaunching()
		{
			// 配置文件加载 类似PlayerPrefs
			QSetting.Load();

			// 日志输出 
			var log =  QLog.Instance;

			var console = QConsole.Instance;

			// 初始化框架
			yield return QFramework.Instance.Init ();


			yield return QUGUIMgr.Init ();

			yield return GameMgr.Instance.Init ();

			// 加载配置表和固定的数据
			yield return ConfigManager.Instance.Init();

			// 初始化内存数据,可更改的数据
			yield return InfoManager.Instance.Init ();

			// 音频资源加载
			yield return SoundManager.Instance.Init();

			yield return QResMgr.Instance.LoadAB (QAB.SOUND.BUNDLENAME);


			// 进入测试逻辑
			if (QApp.Instance.mode == QAppMode.Developing) {

				yield return GetComponent<ITestEntry> ().Launch ();

				// 进入正常游戏逻辑
			} else {
				yield return GameMgr.Instance.Launch ();

			}

			yield return null;
		}

		#region 全局生命周期回调
		public delegate void LifeCircleCallback();

		public LifeCircleCallback onUpdate = null;
		public LifeCircleCallback onFixedUpdate = null;
		public LifeCircleCallback onLatedUpdate = null;
		public LifeCircleCallback onGUI = null;
		public LifeCircleCallback onDestroy = null;
		public LifeCircleCallback onApplicationQuit = null;

		void Update()
		{
			if (this.onUpdate != null)
				this.onUpdate();
		}

		void FixedUpdate()
		{
			if (this.onFixedUpdate != null)
				this.onFixedUpdate ();

		}

		void LatedUpdate()
		{
			if (this.onLatedUpdate != null)
				this.onLatedUpdate ();
		}

		void OnGUI()
		{
			if (this.onGUI != null)
				this.onGUI();
		}

		protected  void OnDestroy() 
		{
			QMonoSingletonComponent<QApp>.Dispose ();

			if (this.onDestroy != null)
				this.onDestroy();
		}

		void OnApplicationQuit()
		{
			if (this.onApplicationQuit != null)
				this.onApplicationQuit();
		}
		#endregion
	}
}
