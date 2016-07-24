using UnityEngine;
using System.Collections;
using QFramework;

namespace QFramework {
	public enum AppMode {
		Developing,
		QA,
		Release
	}


	/// <summary>
	/// 全局唯一继承于MonoBehaviour的单例类，保证其他公共模块都以App的生命周期为准
	/// </summary>
	public class QApp : QMonoSingleton<QApp>
	{
		public AppMode mode = AppMode.Developing;

		private QApp() {}

		void Awake()
		{
			// 确保不被销毁
			DontDestroyOnLoad(gameObject);

			mInstance = this;

			// 进入欢迎界面
			Application.targetFrameRate = 60;
		}

		void  Start()
		{
			CoroutineMgr.Instance ().StartCoroutine (ApplicationDidFinishLaunching());
		}

		/// <summary>
		/// 进入游戏
		/// </summary>
		IEnumerator ApplicationDidFinishLaunching()
		{
			// 配置文件加载 类似PlayerPrefs
			QSetting.Load();

			// 日志输出 
			QLog.Instance ();
			QConsole.Instance ();

			yield return GameManager.Instance ().Init ();

			// 进入测试逻辑
			if (QApp.Instance().mode == AppMode.Developing) {

				yield return GetComponent<ITestEntry> ().Launch ();

				// 进入正常游戏逻辑
			} else {
				yield return GameManager.Instance ().Launch ();

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

		protected override void OnDestroy() 
		{
			base.OnDestroy ();

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
