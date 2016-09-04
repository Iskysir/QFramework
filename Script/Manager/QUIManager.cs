using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using UnityEngine.UI;

namespace QFramework.UI
{
	public class QUIManager : QFramework.QSingleton<QUIManager>,QFramework.Event.IMsgReceiver
    {
		#region 万能框架
		public static QUIManager  Instance = null;

		void Awake()
		{
			Instance = this;
		}

		public void SendMsg(QMsg msg)
		{
			if (msg.GetManager() == ManagerID.UIManager)
			{
				// ManagerBase 本模块自己处理
//				ProcessMsg(msg);
			}
			else // MsgCenter
			{
				QMsgCenter.Instance.SendToMsg(msg);
			}
		}

		public GameObject GetGameObject(string name)
		{
			if (sonMembers.ContainsKey(name))
			{
				return sonMembers [name];
			}

			return null;
		}
		public void RegisterGameObject(string name,GameObject obj)
		{
			if (!sonMembers.ContainsKey(name))
			{
				sonMembers.Add(name,obj);
			}
		}

		public void UnRegisterGameObject(string name)
		{
			if (!sonMembers.ContainsKey(name))
			{
				sonMembers.Remove(name);
			}
		}

		// 规定了 开发方式 消耗内存 换区速度和方便。
		Dictionary<string,GameObject> sonMembers = new Dictionary<string,GameObject>();

		#endregion
        public Transform UIRoot
        {
            get
            {
                if (mParentTrans == null)
                {
                    GameObject obj = GameObject.Find("Canvas");
                    if (obj)
                    {
                        mParentTrans = obj.transform;
                    }
                }
                return mParentTrans;
            }
        }

        public Transform OtherUIRoot
        {
            get
            {
                if (mOtherParentTrans == null)
                {
                    GameObject obj = GameObject.Find("UI");
                    if (obj)
                    {
                        mOtherParentTrans = obj.transform;
                    }
                }
                return mOtherParentTrans;
            }
        }

        public void Init()
        {
            GameObject obj = GameObject.Find("Canvas");
            if (obj)
            {
                mParentTrans = obj.transform;
            }
            GameObject obj2 = GameObject.Find("UI");
            if (obj2)
            {
                mOtherParentTrans = obj2.transform;
            }
            if (null == mParentTrans)
            {
//                DebugUtils.LogError("UI parent node not found!");
            }
//            mMsgCenter = new MsgCenter();
//            SetSuperReceiver(ClientMain.Instance.MsgCenter);
        }

		public GameObject OpenUI<T>(bool other = false) where T : QFramework.UI.QUIBehaviour
        {
            string strDlg = typeof(T).ToString();
            if (!mDicNameToUIObject.ContainsKey(strDlg))
            {
                return CreateUI<T>(other);
            }
            SetVisible(strDlg, true);
            return mDicNameToUIObject[strDlg].gameObject;
        }

		public void CloseUI<T>() where T : QFramework.UI.QUIBehaviour
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

        /** 
         * This is internal method to remove UI and destroy the gameobject
         * Please don't call it
         * */
		public void InternalRemoveMenu(QFramework.UI.QUIBehaviour _handler)
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

        public void SetVisible(string strDlg, bool bVisible)
        {
            if (mDicNameToUIObject.ContainsKey(strDlg))
            {
                mDicNameToUIObject[strDlg].SetVisible(bVisible);
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
//                DebugUtils.LogError(string.Format("panel={0},ui={1} not exist!", strDlg, strUIName));
            }
            return null;
        }

//        public void SetSuperReceiver(MsgCenter msg)
//        {
//            mMsgCenter.SetSuperReceiver(msg);
//        }

//        public void RegisterMsg(ushort msgId, MsgEventHandler handler, bool toHighMsgCenter = false)
//        {
//            mMsgCenter.Register(msgId, handler, toHighMsgCenter);
//        }

//        public void SendMsg(MsgEventArgs args)
//        {
//            mMsgCenter.PostMsg(args);
//        }

//        public void UnRegisterMsg(ushort msgId, MsgEventHandler handler)
//        {
//            mMsgCenter.UnRegister(msgId, handler);
//        }

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


		/// <summary>
		/// 显示对话框
		/// </summary>
//		public IEnumerator ShowDialog(string title,string content,VoidDelegate.WithBool callback = null,string yesBtnText = "好的",string noBtnText = "取消")
//		{
//			var dialogScript = UIManager.Instance.OpenUI<UIDialog> ().GetComponent<UIDialog> ();
//
//			yield return dialogScript.Show(title, content, callback, yesBtnText, noBtnText);
//		}

		/// <summary>
		/// 显示提示
		/// </summary>
//		public IEnumerator ShowPrompt(string content,float duration = 1.0f)
//        {
//			var promptScript = UIManager.Instance.OpenUI<UIPrompt> ().GetComponent<UIPrompt> ();
//
//			yield return promptScript.Show (content,duration); 
//        }


		private GameObject CreateUI<T>(bool other = false) where T : QFramework.UI.QUIBehaviour
        {
            string strDlg = typeof(T).ToString();

            //Object obj = Resources.Load(string.Format("UIPrefab/{0}", strDlg));

//            PTResourceManager.Instance.LoadAssetBundle(strDlg.ToLower());//加载对应的assetbundle
//            GameObject obj = PTResourceManager.Instance.LoadAsset<GameObject>(strDlg.ToLower(), strDlg);//加载 asset

//            if (null == obj)
//            {
//                DebugUtils.LogError("Not find " + strDlg);
                return null;
//            }
//            GameObject go = (GameObject)GameObject.Instantiate(obj);
//            if (null == go)
//            {
//                DebugUtils.LogError(obj.name + "not a gameobject");
                return null;
//            }
//            if (null != UIRoot)
//            {
//                go.name = strDlg;
//                if (other)
//                {
//                    go.transform.SetParent(OtherUIRoot);
//                }
//                else
//                {
//                    go.transform.SetParent(UIRoot);
//                }
//                //go.transform.localPosition = Vector3.zero;
//                go.GetComponent<RectTransform>().offsetMax = Vector2.zero;
//                go.GetComponent<RectTransform>().offsetMin = Vector2.zero;
//                go.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
//                go.transform.localScale = Vector3.one;
//                T t = go.AddComponent<T>();
//                mDicNameToUIObject.Add(strDlg, t);
//                t.Init();
//            }

//            DebugUtils.Log(strDlg + " Load Success");
//            obj = null;
            //PTResourceManager.UnloadAssetBundle(strDlg, false);
//            return go;
        }

//        private MsgCenter mMsgCenter = null;
        private Transform mParentTrans = null;
        private Transform mOtherParentTrans = null;
        private Dictionary<string, List<ushort>> mDicMsgs = new Dictionary<string, List<ushort>>();
		private Dictionary<string, QFramework.UI.QUIBehaviour> mDicNameToUIObject = new Dictionary<string, QFramework.UI.QUIBehaviour>();
    }
}
