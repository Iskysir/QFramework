using UnityEngine;
using System.Collections;

public partial class QMsg {

	// 表示 65535个消息 占两个字节
	public ushort msgId;

	public ManagerID GetManager()
	{
		int tmpId = msgId / FrameTools.MsgSpan;

		return (ManagerID)(tmpId * FrameTools.MsgSpan);
	}

	public QMsg() {}


	public QMsg(ushort tmpMsg)
	{
		msgId = tmpMsg;
	}
}


public class MsgTransform :QMsg
{
	public Transform value;

	public MsgTransform(ushort msgId,Transform tmpTrans) : base(msgId)
	{
		this.value = tmpTrans;
	}
}