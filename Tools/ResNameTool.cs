using UnityEngine;
using System.Collections;


public class ResNameTool  {

	public static string GetRoleSpriteName(int roleId)
	{
		return "plotname_0" + (roleId - 1000).ToString ("D2");
	}


	public static string GetTutoABName(int roleId)
	{
		#if UNITY_EDITOR
//		return  "Tutorials/TutoAtlas_" + roleId;
		return "TutoAtlas_" + roleId;
		#else	
		return "TutoAtlas_" + roleId;
		#endif
	}


	public static string GetTutoSpriteName(int roleId)
	{
		return "tuto_" + (roleId - 1000) + "_1";
	}


	public static string GetTutoAudioName(int roleId,int index)
	{
		return "audio_tuto_" + (roleId - 1000)+"_" + (index - 1);
	}
}
