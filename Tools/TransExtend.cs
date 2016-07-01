using UnityEngine;
using System.Collections;

/// <summary>
/// 扩展
/// </summary>
public static class TransExtend {

	/// <summary>
	/// 归一化
	/// </summary>
	public static void LocalIdentity(this Transform trans)
	{
		trans.localPosition = Vector3.zero;
		trans.localEulerAngles = Vector3.zero;
		trans.localScale = Vector3.one;
	}


	/// <summary>
	/// 归一化
	/// </summary>
	public static void LocalIdentity(this GameObject go)
	{
		go.transform.localPosition = Vector3.zero;
		go.transform.localEulerAngles = Vector3.zero;
		go.transform.localScale = Vector3.one;
	}
		
	public static void LocalIdentity(this MonoBehaviour mono)
	{
		mono.transform.localPosition = Vector3.zero;
		mono.transform.localEulerAngles = Vector3.zero;
		mono.transform.localScale = Vector3.one;
	}
		
	public static void SetParent(this GameObject go,Transform parentTrans)
	{
		go.transform.SetParent(parentTrans);
	}


	public static void SetParent(this GameObject go,GameObject parentGo)
	{
		go.transform.SetParent(parentGo.transform);
	}

	public static void SetParent(this GameObject go,MonoBehaviour parentMono)
	{
		go.transform.SetParent(parentMono.transform);
	}

	public static void SetParent(this MonoBehaviour mono,GameObject parentGo)
	{
		mono.transform.SetParent(parentGo.transform);
	}

	public static void SetParent(this MonoBehaviour mono,Transform parentTrans)
	{
		mono.transform.SetParent(parentTrans);
	}

	public static void SetParent(this MonoBehaviour mono,MonoBehaviour parentMono)
	{
		mono.transform.SetParent(parentMono.transform);
	}


}
