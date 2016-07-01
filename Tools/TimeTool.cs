using UnityEngine;
using System.Collections;

public class TimeTool  {

	/// <summary>
	/// 获得当前时间 换算为分
	/// </summary>
	/// <returns></returns>
	public static int GetCurMinute ()
	{
		return System.DateTime.Now.Hour * 60 + System.DateTime.Now.Minute;
	}

	/// <summary>
	/// 获得当前时间 换算为秒
	/// </summary>
	/// <returns></returns>
	public static int GetCurSecond ()
	{
		return System.DateTime.Now.Hour * 60 + System.DateTime.Now.Minute * 60 + System.DateTime.Now.Second;
	}
}
