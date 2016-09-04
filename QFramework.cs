using UnityEngine;
using System.Collections;

namespace QFramework {
	
	public class Instance {

		public static IEnumerator Init()
		{
			// 初始化消息中心
			yield return QMsgCenter.Instance.Init ();

		}
	}
}
