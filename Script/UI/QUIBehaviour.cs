using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using QFramework;
using UnityEngine.UI;

namespace QFramework.UI {
	public class QUIBehaviour : QMonoBehaviour {

		void OnDestroy()
		{
			DestroyUI();
			if (mUIComponentsDic != null)
			{
				mUIComponentsDic.Clear();
			}
			if (mIComponents != null)
			{
				mIComponents.Clear();
			}
			UnRegisterSelf();
			//			UIManager.Instance.InternalRemoveMenu(this);
			Debug.Log(name + " unLoad Success");
		}

		public void Init(object uiData = null)
		{
			InnerInit(uiData);
			RegisterUIEvent();
		}

		public Transform Get(string behaivourName)
		{
			if (mUIComponentsDic.ContainsKey(behaivourName))
			{
				return mUIComponentsDic[behaivourName];
			}
			return null;
		}

		public void Close()
		{
			SetVisible(false);
			OnClose();
			if (mUnloadAll == false)
			{
				//                PTResourceManager.UnloadAssetBundle(this.name.ToLower(), false);
			}
		}

		public void SetVisible(bool visible)
		{
			this.gameObject.SetActive(visible);
			if(visible)
			{
				OnShow();
			}
		}

		void InnerInit(object uiData = null)
		{
			FindAllCanHandleWidget(this.transform);
			mIComponents = QUIFactory.Instance.CreateUIComponents(this.name);
			mIComponents.InitUIComponents();
			InitUI(uiData);
			SetVisible(true);
		}

		protected virtual void OnAwake() { }
		protected virtual void OnStart() { }
		protected virtual void InitUI(object uiData = null) { }
		protected virtual void RegisterUIEvent() { }
		protected virtual void OnUpdate() { }
		protected virtual void OnFixedUpdate() { }
		protected virtual void OnClose() { }
		protected virtual void DestroyUI() { }

		protected void SetUIBehavior(IUIComponents uiChild)
		{
			mIComponents = uiChild;
			mIComponents.InitUIComponents();
		}

		protected void RegisterSelf(ushort[] msgIds)
		{
			mMsgIds = msgIds;
			for (int i = 0; i < msgIds.Length; i++)
			{
				//                UIManager.Instance.RegisterMsg(msgIds[i], ProcessMsg);
			}
		}

		//        protected void SendMsg(MsgEventArgs args)
		//        {
		//            args.Sender = this.name;
		//            UIManager.Instance.SendMsg(args);
		//        }

		protected void UnRegisterSelf()
		{
			if (mMsgIds != null)
			{
				for (int i = 0; i < mMsgIds.Length; i++)
				{
					//                    UIManager.Instance.UnRegisterMsg(mMsgIds[i], ProcessMsg);
				}
			}
		}

		void FindAllCanHandleWidget(Transform trans)
		{
			for (int i = 0; i < trans.childCount; i++)
			{
				Transform childTrans = trans.GetChild(i);
				QUIMark uiMark = childTrans.GetComponent<QUIMark>();
				if (null != uiMark)
				{
					if (mUIComponentsDic.ContainsKey(childTrans.name))
					{
						Debug.LogError("Repeat Id: " + childTrans.name);
					}
					else
					{
						mUIComponentsDic.Add(childTrans.name, childTrans);
					}
				}
				FindAllCanHandleWidget(childTrans);
			}
		}

		protected virtual bool mUnloadAll
		{
			get { return false; }
		}
		protected ushort[] mMsgIds = null;
		protected IUIComponents mIComponents = null;
		private Dictionary<string, Transform> mUIComponentsDic = new Dictionary<string, Transform>();



		// 把空间注册到UIManager
		// 可以直接查找 物体把物体本身注册到UIManager
		//		void Awake()
		//		{
		//			UIManager.Instance.RegisterGameObject(name,gameObject);
		//		}

		public void AddButtonListener(UnityAction action)
		{
			if (null != action)
			{
				Button btn = transform.GetComponent<Button> ();

				btn.onClick.AddListener (action);
			}
		}


		public void RemoveButtonListener(UnityAction action)
		{
			if (null != action) 
			{
				Button btn = transform.GetComponent<Button> ();

				btn.onClick.RemoveListener (action);
			}
		}


		public void AddSliderListener(UnityAction<float> action)
		{
			if (null != action) 
			{
				Slider slider = transform.GetComponent<Slider> ();

				slider.onValueChanged.AddListener (action);
			}
		}

		public void RemoveSliderListener(UnityAction<float> action)
		{
			if (null != action) 
			{
				Slider slider = transform.GetComponent<Slider> ();

				slider.onValueChanged.RemoveListener (action);
			}
		}

		public void AddInputListener(UnityAction<string> action)
		{
			if (null != action) 
			{
				InputField btn = transform.GetComponent<InputField> ();

				btn.onValueChanged.AddListener (action);
			}
		}

		public override void ProcessMsg (QMsg msg)
		{
			throw new System.NotImplementedException ();
		}


		public void RegisterSelf(QMonoBehaviour mono,params ushort[] msgs)
		{
			QUGUIMgr.Instance.RegisterMsg(mono,msgIds);
		}

		public void UnRegisterSelf(QMonoBehaviour mono,params ushort[] msg)
		{
			QUGUIMgr.Instance.UnRegisterMsg(mono,msgIds);
		}

		public void SendMsg(QMsg msg)
		{
			QUGUIMgr.Instance.SendMsg(msg);
		}
		protected ushort[] msgIds;

		void OnDestory()
		{
			if (msgIds != null)
			{
				UnRegisterSelf(this,msgIds);
			}
		}


		#region 原来自己的框架
		public void Enter(object uiData)
		{
			gameObject.SetActive (false);
			OnEnter (uiData);
		}

		/// <summary>
		/// 资源加载之后用
		/// </summary>
		protected virtual void OnEnter(object uiData)
		{
			Debug.LogWarning ("On Enter:" + name);
		}


		public void Show()
		{
			OnShow ();
		}

		/// <summary>
		/// 显示时候用,或者,Active为True
		/// </summary>
		protected virtual void OnShow()
		{
			gameObject.SetActive (true);
			Debug.LogWarning ("On Show:" + name);

		}


		public void Hide()
		{
			OnHide ();
		}

		/// <summary>
		/// 隐藏时候调用,即将删除 或者,Active为False
		/// </summary>
		protected virtual void OnHide()
		{
			gameObject.SetActive (false);
			Debug.LogWarning ("On Hide:" + name);
		}

		public void Exit()
		{
			OnExit ();
		}

		/// <summary>
		/// 删除时候调用
		/// </summary>
		protected virtual void OnExit()
		{
			Debug.LogWarning ("On Exit:" + name);
		}
		#endregion

	}

}