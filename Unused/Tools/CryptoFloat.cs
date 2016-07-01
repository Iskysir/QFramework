using UnityEngine;
using System.Collections;

[System.Serializable]
public class CryptoFloatDefined {
	public static byte CryptoCode = 0xef;
	public static byte[] cBufs = new byte[4];

	public float sVal = 0.0f;
	[SerializeField]
	private byte[] mVal = null;
	
	private float Val {
		get {
			int idx = 0;
//			byte floatCode = AssetsHelper.Instance.SvConfig.FloatCode;
			byte floatCode = 0;
			if(null == this.mVal) {
				return 0.0f;
			}
			floatCode = CryptoFloatDefined.CryptoCode;
			for(idx = 0; idx < this.mVal.Length; idx++) {
				CryptoFloatDefined.cBufs[idx] = (byte)(this.mVal[idx] ^ floatCode);
			}
			return System.BitConverter.ToSingle(CryptoFloatDefined.cBufs, 0);
		}
	}
	public static implicit operator float(CryptoFloatDefined value)
	{
		return (value.Val);
	}

	public override string ToString ()
	{
		return this.Val.ToString();
	}

	public static implicit operator CryptoFloatDefined(float value)
	{
		CryptoFloatDefined t = new CryptoFloatDefined();
		t.sVal = value;
#if UNITY_EDITOR
		t.Refresh();
#endif
		return t;
	}

//	public static implicit operator CryptoFloatDefined(CryptoFloatDefined value) {
//		CryptoFloatDefined t = new CryptoFloatDefined();
//		t.sVal = value.sVal;
//#if UNITY_EDITOR
//		t.Refresh();
//#endif
//		return t;
//	}

#if UNITY_EDITOR
	public void Refresh() {
		int idx = 0;
		byte floatCode = 0;
		this.mVal = System.BitConverter.GetBytes(this.sVal);
		//crypto bytes.
//		floatCode = AssetsHelper.Instance.SvConfig.FloatCode;
		floatCode = CryptoFloatDefined.CryptoCode;
		for(idx = 0; idx < this.mVal.Length; idx++) {
			this.mVal[idx] = (byte)(this.mVal[idx] ^ floatCode);
		}
	}
#endif
}

public struct CryptoFloatVar {
	private byte[] mVal;
//	private byte[] mBuf;

	public CryptoFloatVar(float v):this() {
		this.Val = v;
	}
	private float Val {
		get {
			byte floatCode = 0;
			int idx = 0;
			if(null == this.mVal) {
				return 0.0f;
			}
//			floatCode = AssetsHelper.Instance.SvConfig.FloatCode;
			floatCode = CryptoUtility.EBCode;
			for(idx = 0; idx < this.mVal.Length; idx++) {
				CryptoFloatDefined.cBufs[idx] = (byte)(this.mVal[idx] ^ floatCode);
			}
			return System.BitConverter.ToSingle(CryptoFloatDefined.cBufs, 0);
		}
		set {
			byte floatCode = 0;
			this.mVal = System.BitConverter.GetBytes(value);
//			floatCode = AssetsHelper.Instance.SvConfig.FloatCode;
			floatCode = CryptoUtility.EBCode;
			for(int idx = 0; idx < this.mVal.Length; idx++) {
				this.mVal[idx] = (byte)(this.mVal[idx] ^ floatCode);
			}
		}
	}

	public static implicit operator CryptoFloatVar(CryptoFloatDefined value)
	{
		float tVal = value;
		CryptoFloatVar t = new CryptoFloatVar();
		t.Val = tVal;
		return t;
	}

	public static implicit operator CryptoFloatVar(float value)
	{
		CryptoFloatVar t = new CryptoFloatVar();
		t.Val = value;
		return t;
	}
	public static implicit operator float(CryptoFloatVar value)
	{
		return (value.Val);
	}
	public override string ToString ()
	{
		return this.Val.ToString();
	}
}
