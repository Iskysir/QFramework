using UnityEngine;
using System.Collections;
using QFramework;

/// <summary>
/// 扩展GameObject
/// </summary>
public static class GoExtend  {


	public static void Hide(this GameObject go)
	{
		go.SetActive (false);
	}

	public static void Show(this GameObject go)
	{
		go.SetActive (true);
	}

	public static void Show(this Transform trans)
	{
		trans.gameObject.SetActive (true);
	}

	public static void Hide(this Transform trans)
	{
		trans.gameObject.SetActive (false);
	}



	/// <summary>
	/// 注册按钮点击事件
	/// </summary>
	public static void OnClick(this GameObject go,VoidDelegate.WithGo onClickCallback)
	{
		var listener = EventTriggerListener.CheckAndAddListener (go);

		listener.onClick = onClickCallback;
	}


	/// <summary>
	/// 注册按钮点击事件
	/// </summary>
	public static void OnClick(this Transform trans,VoidDelegate.WithGo onClickCallback)
	{
		trans.gameObject.OnClick (onClickCallback);
	}
		









}
