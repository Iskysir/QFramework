using UnityEngine;
using System.Collections;

[System.Serializable]
/**
 * 加密整型定义
 */
public class CryptoIntDefined {
	public static int CryptoCode = 0x7feffefe;

	public int sVal = 0;
	[SerializeField]
	private int mVal = 0;
	
	private int Val {
		get {
//			return (this.mVal ^ AssetsHelper.Instance.SvConfig.IntCode);
			if(0 == this.mVal) {
				this.mVal = (this.sVal ^ CryptoIntDefined.CryptoCode);
			}
			return (this.mVal ^ CryptoIntDefined.CryptoCode);
		}
	}
	public static implicit operator int(CryptoIntDefined value)
	{
		return (value.Val);
	}

	public override string ToString ()
	{
		return this.Val.ToString();
	}

//	public static implicit operator CryptoIntDefined(CryptoIntDefined value) {
//		CryptoIntDefined t = new CryptoIntDefined();
//		t.sVal = value.sVal;
//#if UNITY_EDITOR
//		t.refresh();
//#endif
//		return t;
//	}

    /**
     * 隐藏定义构造函数
     */
	public static implicit operator CryptoIntDefined(int value)
	{
		CryptoIntDefined t = new CryptoIntDefined();
		t.sVal = value;
#if UNITY_EDITOR
		t.refresh();
#endif
		return t;
	}

#if UNITY_EDITOR
	public void refresh() {
//		this.mVal = (this.sVal ^ AssetsHelper.Instance.SvConfig.IntCode);
		this.mVal = (this.sVal ^ CryptoIntDefined.CryptoCode);
	}
#endif
}

public struct CryptoIntVar {
	private int mVal;
	private bool mInit;

	public CryptoIntVar(int val):this() {
		this.Val = val;
	}
	//real
	private int Val {
		get {
			//CryptoIntDefined.CryptoCode
//			return (this.mVal ^ AssetsHelper.Instance.SvConfig.IntCode);
#if UNITY_EDITOR
			if(!this.mInit) {
				Debug.LogError("Please init you int var first!");
			}
#endif
			if(!this.mInit)
			{
				this.mInit = true;
				this.mVal = (0 ^ CryptoUtility.EICode);
			}

			return (this.mVal ^ CryptoUtility.EICode);
		}
		set {
//			this.mVal = (value ^ AssetsHelper.Instance.SvConfig.IntCode);
			this.mInit = true;
			this.mVal = (value ^ CryptoUtility.EICode);
		}
	}

	public static implicit operator CryptoIntVar(CryptoIntDefined value) {
		int tVal = value;
		CryptoIntVar t = new CryptoIntVar();
		t.Val = tVal;
		return t;
	}

	public static implicit operator CryptoIntVar(int value)
	{
		CryptoIntVar t = new CryptoIntVar();
		t.Val = value;
		return t;
	}
	public static implicit operator int(CryptoIntVar val)
	{
		return (val.Val);
	}

	public static CryptoIntVar operator ++(CryptoIntVar val)
	{
		return ++val.Val;
	}

	public static CryptoIntVar operator --(CryptoIntVar val)
	{
		return --val.Val;
	}

	public override string ToString ()
	{
		return this.Val.ToString();
	}
}