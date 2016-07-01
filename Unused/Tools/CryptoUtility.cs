using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CryptoUtility {
	private const int MinIntBits = 16;
	private const int MaxIntBits = 32;
	private const int MinByteBits = 4;
	private const int MaxByteBits = 8;

	static CryptoUtility() {
//		int intKey = CryptoUtility.EICode;
//		byte floatKey = CryptoUtility.EBCode;
	}
	
	private static int sEICode = 0;
	public static int EICode {
		get {
			System.Random rnd = null;
			int idx = 0;
			int delBits = 0;
			List<int> cards = null;
			if(0 == sEICode) {
				rnd = new System.Random();
				delBits = rnd.Next(1, MinIntBits);
//				delBits = Random.Range(0, MinIntBits);
				cards = shuffle(delBits, MaxIntBits);
				for(idx = 0; idx < cards.Count; idx++) {
					sEICode |= (1 << cards[idx]);
				}
#if UNITY_EDITOR
//				Gdb.L(System.Convert.ToString(sEICode, 2) + "==>" + cards.Count);
#endif
			}
			return sEICode;
		}
	}
	private static byte sEBCode = 0;
	public static byte EBCode {
		get {
			int idx = 0;
			int delBits = 0;
			List<int> cards = null;
			System.Random rnd = null;
			if(0 == sEBCode) {
				rnd = new System.Random();
				delBits = rnd.Next(1, MinByteBits);
//				delBits = Random.Range(0, MinByteBits);
				cards = shuffle(delBits, MaxByteBits);
				for(idx = 0; idx < cards.Count; idx++) {
					sEBCode |= (byte)(1 << cards[idx]);
				}
#if UNITY_EDITOR
//				Gdb.L(System.Convert.ToString(sEICode, 2) + "==>" + cards.Count);
#endif
			}
			return sEBCode;
		}
	}
	private static List<int> shuffle(int delNum, int num) {
		List<int> codes = null;
		System.Random rnd = null;
		int idx = 0;
		if(delNum >= num) {
			return null;
		}
		rnd = new System.Random();
		codes = new List<int>();
		for(idx = 0; idx < num; idx++) {
			codes.Add(idx);
		}
		while(delNum > 0) {
			delNum--;
//			idx = Random.Range(0, codes.Count);
			idx = rnd.Next(0, codes.Count);
			codes.RemoveAt(idx);
		}
		return codes;
	}
}
