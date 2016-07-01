using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.iOS;

/// <summary>
/// 帮助拼接字符串
/// </summary>
public class PathTool {

	/// <summary>
	/// 组成FishGameObject名字
	/// </summary>
	public static string GetFishGoName(int roleId,int fishId) {
		return roleId + "_fish_" + fishId;
	}

	/// <summary>
	/// 组合鱼的纹理所在路径
	/// </summary>
	public static string GetFishTexPathPng(int roleId,int fishId){
		FileTool.CheckAndCreateDir (LocalPath.FishTexAndAudioPath);
		return LocalPath.FishTexAndAudioPath+roleId+"_fish_"+fishId+".png";
	}


	/// <summary>
	/// 拼接
	/// </summary>
	public static string CombineFishAudioPath(string append)
	{
		return Path.Combine (LocalPath.FishTexAndAudioPath, append);
	}
		
	/// <summary>
	/// 获取鱼的音频
	/// </summary>
	public static string GetFishAudioPathWav(int fishId)
	{
		FileTool.CheckAndCreateDir (LocalPath.FishTexAndAudioPath);
		return LocalPath.FishTexAndAudioPath + fishId + ".wav";
	}

	public static string GetFishAudioPathWav(string fishId)
	{
		FileTool.CheckAndCreateDir (LocalPath.FishTexAndAudioPath);
		return LocalPath.FishTexAndAudioPath + fishId + ".wav";
	}
		
	/// <summary>
	/// 获取维度贴图的路径
	/// </summary>
	public static string GetWeiDuTexPath(int roleId,int fishId){
		FileTool.CheckAndCreateDir (LocalPath.FishTexAndAudioPath);
		return LocalPath.FishTexAndAudioPath+roleId+"_fish_"+fishId+"_model.png";
	}


	/// <summary>
	/// 获取音频资源路径
	/// </summary>
	public static string GetAudioABPath(string audioABName)
	{
		return LocalPath.AudioABPath + audioABName;
	}


	/// <summary>
	/// 获取玩家截屏路径
	/// </summary>
	public static string GetPlayerFishTexPath(string texName)
	{
		return  LocalPath.PlayerFishTexturePath + texName;

	}

	public static string GetPlayerFishTexDir(string dirName)
	{
		return  LocalPath.PlayerFishTexturePath + dirName;

	}


	/// <summary>
	/// 初始化
	/// </summary>
	public static void InitLocalPath()
	{
		LocalPath.PlayerFishTexturePath = Application.persistentDataPath + "/PlayerFishTextures/";
		//保存图片路径
		#if UNITY_IOS
		LocalPath.SaveTexturePath = Application.persistentDataPath + "/savetexture.jpg";
		#else
		LocalPath.SaveTexturePath = "SaveTexture.jpg";
		#endif

		LocalPath.FishTexAndAudioPath = Application.persistentDataPath + "/fishtexandaudio/";

		LocalPath.TexCachePath = Application.persistentDataPath + "/oceancache/";


		//Audio路径
		#if UNITY_EDITOR
		LocalPath.AudioABPath = Application.streamingAssetsPath + "/";
		#elif UNITY_IPHONE
		LocalPath.AudioABPath =Application.streamingAssetsPath + "/";
		#elif UNITY_ANDROID
		LocalPath.AudioABPath ="jar:file://" + Application.dataPath + "!/assets//Audios//";
		#endif
	}


	public static string GetPersistPath(string resName)
	{
		string path =
#if UNITY_IOS
		Application.persistentDataPath + "/" + resName ;
#elif UNITY_EDITOR
		Application.dataPath + "/" + resName;
#endif
		return path;
	}

	public static string GetPersistPathWWW(string resName)
	{
		string path =
#if UNITY_IOS
		"file:///"+Application.persistentDataPath + "/" + resName ;
#elif UNITY_EDITOR
		"file:///" + Application.dataPath + "/" + resName;
#endif
		return path;
	}


	public static string GetStreamPathWWW(string resName)
	{
		string path =
#if UNITY_EDITOR
		"file:///" + Application.dataPath + "/StreamingAssets" + "/" + resName;
#elif UNITY_IOS
		"file:///"+Application.dataPath + "/Raw" + "/" + resName ;
#endif
		return path;
	}


}
