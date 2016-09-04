using UnityEngine;
using System.Collections;
using QFramework;

public class QMsgCenter : MonoBehaviour 
{
	public static QMsgCenter Instance {
		get {
			return QMonoSingletonComponent<QMsgCenter>.Instance;
		}
	}

	public void OnDestroy()
	{
		QMonoSingletonComponent<QMsgCenter>.Dispose ();
	}

	public IEnumerator Init()
	{
		Debug.Log ("QMsgCenter Init");
		yield return null;
	}

	public void SendToMsg(QMsg tmpMsg)
	{
		ForwardMsg(tmpMsg);
	}

	/// <summary>
	/// 转发消息
	/// </summary>
	private void ForwardMsg(QMsg tempMsg)
	{
		ManagerID tmpId = tempMsg.GetManager();

		switch (tmpId)
		{
		case ManagerID.AssetManager:
			break;
		case ManagerID.AudioManager:
			break;
		case  ManagerID.CharactorManager:
			break;
		case  ManagerID.GameManager:
			break;
		case  ManagerID.NetManager:
			break;
		case  ManagerID.NPCManager:
			break;
		case  ManagerID.UIManager:
			break;
		default:
			break;
		}
	}
}
