﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace QFramework.UI {

	public enum CanvasLevel
	{
		Top,
		Middle,
		Bottom,
		Root,
		MainCamera,
	}
		
	//// <summary>
	/// UGUI UI界面管理器
	/// </summary>
	public class QUGUIMgr : QMgrBehaviour{ 

		public void SendMsg(QMsg msg)
		{
			if (msg.GetManager() == ManagerID.UIManager)
			{
				ProcessMsg(msg);
			}
			else 
			{
				QMsgCenter.Instance.SendToMsg(msg);
			}
		}
			

		public QUIBehaviour OpenUI<T>(CanvasLevel canvasLevel,object uiData = null) where T : QUIBehaviour
		{
			string behaviourName = typeof(T).ToString();
			if (!mAllUIBehaviour.ContainsKey(behaviourName))
			{
				return CreateUI<T>(canvasLevel,uiData);
			}
			SetVisible(behaviourName, true);
			return mAllUIBehaviour[behaviourName];
		}

		public void CloseUI<T>() where T : QUIBehaviour
		{
			string strDlg = typeof(T).ToString();
			if (mDicNameToUIObject.ContainsKey(strDlg))
			{
				//                DebugUtils.Log(strDlg + " UnLoad Success");
				mDicNameToUIObject[strDlg].Close();
			}
		}

		public void CloseAllUI()
		{
			List<QUIBehaviour> listHandler = new List<QUIBehaviour>();
			foreach (string key in mDicNameToUIObject.Keys)
			{
				listHandler.Add(mDicNameToUIObject[key]);
			}

			for (int i = 0; i < listHandler.Count; i++)
			{
				listHandler[i].Close();
			}
		}
			
		public void InternalRemoveMenu(QUIBehaviour _handler)
		{
			System.Type type = _handler.GetType();
			string key = type.ToString();
			if (mDicNameToUIObject.ContainsKey(key))
			{
				mDicNameToUIObject.Remove(key);
			}
			//GameObject.Destroy(_handler.gameObject);
			//            PTResourceManager.UnloadAssetBundle(key.ToLower(), true);
			//Resources.UnloadUnusedAssets();
		}

		public void SetVisible(string behaviourName, bool bVisible)
		{
			if (mDicNameToUIObject.ContainsKey(behaviourName))
			{
				mDicNameToUIObject[behaviourName].SetVisible(bVisible);
			}
		}

		public Transform Get<T>(string strUIName)
		{
			string strDlg = typeof(T).ToString();
			if (mDicNameToUIObject.ContainsKey(strDlg))
			{
				return mDicNameToUIObject[strDlg].Get(strUIName);
			}
			else
			{
				Debug.LogError(string.Format("panel={0},ui={1} not exist!", strDlg, strUIName));
			}
			return null;
		}

		public void Close()
		{
			//foreach (List<ushort> list in mDicMsgs.Values)
			//{
			//    for (int i = 0; i < list.Count; i++)
			//    {
			//        UnRegisterMsg(list[i]);
			//    }
			//}
			mDicMsgs.Clear();
			mDicNameToUIObject.Clear();
		}
			

		//        private MsgCenter mMsgCenter = null;
		private Transform mParentTrans = null;
		private Transform mOtherParentTrans = null;
		private Dictionary<string, List<ushort>> mDicMsgs = new Dictionary<string, List<ushort>>();
		private Dictionary<string, QUIBehaviour> mDicNameToUIObject = new Dictionary<string,QUIBehaviour>();

		public static QUGUIMgr Instance {
			get {
				return QMonoSingletonComponent<QUGUIMgr>.Instance;
			}
		}

		public void OnDestroy()
		{
			QMonoSingletonComponent<QUGUIMgr>.Dispose ();
		}

		/// <summary>
		/// 初始化
		/// </summary>
		public static IEnumerator Init() {
			if (GameObject.Find ("QUGUIMgr")) {

			} else {
				GameObject.Instantiate (Resources.Load ("QUGUIMgr"));
			}

			var init =QUGUIMgr.Instance;


			yield return null;
		}

	

		[SerializeField]
		Dictionary<string,QUIBehaviour> mAllUIBehaviour = new Dictionary<string, QUIBehaviour> ();

		[SerializeField] Transform mCanvasTopTrans;
		[SerializeField] Transform mCanvasMidTrans;
		[SerializeField] Transform mCanvasBottomTrans;
		[SerializeField] Transform mCanvasTrans;
		[SerializeField] Transform mCanvasGuideTrans;
		[SerializeField] Camera mUICamera;

		/// <summary>
		/// 增加UI层
		/// </summary>
		public QUIBehaviour CreateUI<T>  (CanvasLevel level,object uiData = null) where T : QUIBehaviour {

			string behaviourName = typeof(T).ToString();

			if (mAllUIBehaviour.ContainsKey (behaviourName)) {

                Debug.LogWarning(behaviourName + ": already exist");

				mAllUIBehaviour [behaviourName].transform.localPosition = Vector3.zero;
				mAllUIBehaviour [behaviourName].transform.localEulerAngles = Vector3.zero;
				mAllUIBehaviour [behaviourName].transform.localScale = Vector3.one;

				mAllUIBehaviour [behaviourName].Enter (uiData);


			} else {
				GameObject prefab = QResMgr.Instance.LoadUIPrefabSync (behaviourName);

				GameObject mUIGo = Instantiate (prefab);
				switch (level) {
				case CanvasLevel.Top:
					mUIGo.transform.SetParent (mCanvasTopTrans);
					break;
				case CanvasLevel.Middle:
					mUIGo.transform.SetParent (mCanvasMidTrans);
					break;
				case CanvasLevel.Bottom:
					mUIGo.transform.SetParent (mCanvasBottomTrans);
					break;
				case CanvasLevel.Root:
					mUIGo.transform.SetParent (transform);
					break;
				case CanvasLevel.MainCamera:
					mUIGo.transform.SetParent (Camera.main.transform);
					break;

				}


				mUIGo.transform.localPosition = Vector3.zero;
				mUIGo.transform.localEulerAngles = Vector3.zero;
				mUIGo.transform.localScale = Vector3.one;
				//                go.GetComponent<RectTransform>().offsetMax = Vector2.zero;
				//                go.GetComponent<RectTransform>().offsetMin = Vector2.zero;
				//                go.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
				//                go.transform.localScale = Vector3.one;

				mUIGo.gameObject.name = behaviourName;

				Debug.Log(behaviourName + " Load Success");


				T t = mUIGo.AddComponent<T>();
				mAllUIBehaviour.Add (behaviourName, t);
				t.Init (uiData);
			}


			return mAllUIBehaviour [behaviourName];
		}

		/// <summary>
		/// 删除掉层
		/// </summary>
		public void DeleteUI(string layerName)
		{
			if (mAllUIBehaviour.ContainsKey (layerName)) 
			{
				mAllUIBehaviour [layerName].Exit ();
				GameObject.Destroy (mAllUIBehaviour [layerName].gameObject);
				mAllUIBehaviour.Remove (layerName);
			}
		}


		/// <summary>
		/// 显示UI层
		/// </summary>
		/// <param name="layerName">Layer name.</param>
		public void ShowUIBehaviour(string layerName)
		{
			if (mAllUIBehaviour.ContainsKey(layerName))
			{
				mAllUIBehaviour[layerName].Show ();
			}
		}


		/// <summary>
		/// 隐藏UI层
		/// </summary>
		/// <param name="layerName">Layer name.</param>
		public void HideUIBehaviour(string layerName)
		{
			if (mAllUIBehaviour.ContainsKey (layerName)) 
			{
				mAllUIBehaviour [layerName].Hide ();
			}
		}


		/// <summary>
		/// 删除所有UI层
		/// </summary>
		public void DeleteAllUIBehaviour()
		{
			foreach (var layer in mAllUIBehaviour) 
			{
				layer.Value.Exit ();
				GameObject.Destroy (layer.Value);
			}

			mAllUIBehaviour.Clear ();
		}


		/// <summary>
		/// 获取所有UI层
		public T GetUIBehaviour<T>(string layerName)
		{
			if (mAllUIBehaviour.ContainsKey (layerName)) 
			{
				return mAllUIBehaviour [layerName].GetComponent<T> ();
			}
			return default(T);
		}

        /// <summary>
        /// 获取UI相机
        /// </summary>
        /// <returns></returns>
        public Camera GetUICamera() 
        {
            return mUICamera;
        }
	}

    

}