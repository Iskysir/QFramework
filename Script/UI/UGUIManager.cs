using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using QFramework;
using QFramework.UI;


public enum CanvasLevel
{
	Top,
	Middle,
	Bottom,
	Root,
	MainCamera,
}


// 可以做成自动从 Resource中加载的
public class UGUIManager : QMonoSingleton<UGUIManager>{ 

	/// <summary>
	/// 初始化
	/// </summary>
	public static void Init() {
		if (GameObject.Find ("UGUIManager")) {
		
		} else if (null == mInstance){
			GameObject.Instantiate (Resources.Load ("UGUIManager"));
		}
			
		UGUIManager.Instance ();
	}


	void Awake()
	{
		mInstance = this;
		DontDestroyOnLoad (this);
	}


	[SerializeField]
	Dictionary<string,UILayer> mAllLayers = new Dictionary<string, UILayer> ();

	[SerializeField] Transform mCanvasTopTrans;
	[SerializeField] Transform mCanvasMidTrans;
	[SerializeField] Transform mCanvasBottomTrans;
	[SerializeField] Transform mCanvasTrans;
	[SerializeField] Transform mCanvasGuideTrans;

	/// <summary>
	/// 增加UI层
	/// </summary>
	public UILayer AddLayer(string layerName,CanvasLevel level,object uiData = null) {

		if (mAllLayers.ContainsKey (layerName)) {

			QPrint.Warn (layerName + ": already exist");

			mAllLayers [layerName].transform.localPosition = Vector3.zero;
			mAllLayers [layerName].transform.localEulerAngles = Vector3.zero;
			mAllLayers [layerName].transform.localScale = Vector3.one;

			mAllLayers [layerName].Enter (uiData);


		} else {
			GameObject prefab = ResMgr.Instance ().LoadUIPrefabSync (layerName);

			GameObject uiLayer = Instantiate (prefab);
			switch (level) {
			case CanvasLevel.Top:
				uiLayer.transform.SetParent (mCanvasTopTrans);
				break;
			case CanvasLevel.Middle:
				uiLayer.transform.SetParent (mCanvasMidTrans);
				break;
			case CanvasLevel.Bottom:
				uiLayer.transform.SetParent (mCanvasBottomTrans);
				break;
			case CanvasLevel.Root:
				uiLayer.transform.SetParent (transform);
				break;
			case CanvasLevel.MainCamera:
				uiLayer.transform.SetParent (Camera.main.transform);
				break;

			}


			uiLayer.transform.localPosition = Vector3.zero;
			uiLayer.transform.localEulerAngles = Vector3.zero;
			uiLayer.transform.localScale = Vector3.one;

			uiLayer.gameObject.name = layerName;

			mAllLayers.Add (layerName, uiLayer.GetComponent<UILayer> ());
			uiLayer.GetComponent<UILayer> ().Enter (uiData);
		}


		return mAllLayers [layerName];
	}

	/// <summary>
	/// 删除掉层
	/// </summary>
	public void DeleteLayer(string layerName)
	{
		if (mAllLayers.ContainsKey (layerName)) 
		{
			mAllLayers [layerName].Exit ();
			GameObject.Destroy (mAllLayers [layerName].gameObject);
			mAllLayers.Remove (layerName);
		}
	}


	/// <summary>
	/// 显示UI层
	/// </summary>
	/// <param name="layerName">Layer name.</param>
	public void ShowLayer(string layerName)
	{
		if (mAllLayers.ContainsKey(layerName))
		{
			mAllLayers[layerName].Show ();
		}
	}


	/// <summary>
	/// 隐藏UI层
	/// </summary>
	/// <param name="layerName">Layer name.</param>
	public void HideLayer(string layerName)
	{
		if (mAllLayers.ContainsKey (layerName)) 
		{
			mAllLayers [layerName].Hide ();
		}
	}


	/// <summary>
	/// 删除所有UI层
	/// </summary>
	public void DeleteAllLayer()
	{
		foreach (var layer in mAllLayers) 
		{
			layer.Value.Exit ();
			GameObject.Destroy (layer.Value);
		}

		mAllLayers.Clear ();
	}


	/// <summary>
	/// 获取所有UI层
	public T GetLayer<T>(string layerName)
	{
		if (mAllLayers.ContainsKey (layerName)) 
		{
			return mAllLayers [layerName].GetComponent<T> ();
		}
		return default(T);
	}
}
