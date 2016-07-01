using System.Collections;
using System.Collections.Generic;
//using Vuforia;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.UI;
using System;
using PTGame.Core;

/// <summary>
/// TODO:这个也要干掉
/// </summary>
public class Utils : MonoBehaviour
{
	static GameObject mVideo = null;
	static GameObject Video {
		get {
			if (null == mVideo) {
				mVideo = Camera.main.transform.Find("OceanCamera").gameObject;
			}
			return mVideo;
		}
	}
	static PTARWebCam mWebTexture = null;
	static PTARWebCam WebTexture {
		get {
			if (null == mWebTexture) {
				mWebTexture = Video.GetComponent<PTARWebCam>();
			}
			return mWebTexture;
		}
	}


	static GameObject oceanModel;
	static GameObject ground;



	/// <summary>
	/// 保存图片到系统相册
	/// </summary>
	/// <param name="filename"></param>
	/// <param name="imageData"></param>
	public static void WriteDataAsImageToPhotosAlbum(string filename, byte[] imageData)
	{
		#if UNITY_IOS && !UNITY_EDITOR
		PTUniInterface.SavePhotoToAlbum (filename, imageData, imageData.Length);
		#endif
	}

	public static bool AlumsPermissionIsOpen()
	{
		#if UNITY_IOS && !UNITY_EDITOR
		return 	PTUniInterface.HasAlbumPermission ();
		#endif

		return true;
	}

	public static void OpenAlbumsPermissionSettingIOS()
	{
		#if UNITY_IOS && !UNITY_EDITOR
		PTUniInterface.GotoCameraPermissionSettingView(LocalizeText.GetString(TextContent.AlbumPermission),LocalizeText.GetString(TextContent.GrantAccess),LocalizeText.GetString(TextContent.NotGrantAccess));
		#endif
	}



	/// <summary>
	/// 获取对应摄像机下，屏幕坐标对应的世界坐标
	/// </summary>
	/// <param name="cam"></param>
	/// <param name="screenPos"></param>
	/// <returns></returns>
	public static Vector3 GetWorldPos(Camera cam, Vector2 screenPos)
	{
		Ray ray = cam.ScreenPointToRay(screenPos);

		float t = -ray.origin.z / ray.direction.z;

		return ray.GetPoint(t);
	}

	public static void GetMemory ()
	{

	}

	public static void ClearMemory()
	{
		//Resources.UnloadUnusedAssets ();
		// 		System.GC.Collect ();
	}

	/// <summary>
	/// 获取摄像机射线下的物体
	/// </summary>
	/// <param name="cam"></param>
	/// <param name="screenPos"></param>
	/// <returns></returns>
	public static GameObject GetWorldGameObject(Camera cam, Vector2 screenPos)
	{
		//从摄像机发出到点击坐标的射线
		Ray ray = Camera.main.ScreenPointToRay(screenPos);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo))
		{
			#if UNITY_STANDALONE_WIN
			//划出射线，只有在scene视图中才能看到
			Debug.DrawLine(ray.origin, hitInfo.point);
			#endif
			GameObject gameObj = hitInfo.collider.gameObject;
			if (gameObj != null)
			{
				return gameObj;
			}
		}

		return null;
	}

	//#if UNITY_IOS
	////	[DllImport("__Internal")]
	////	private static extern bool cameraPermissionIsOpen();
	//	
	//	[DllImport("__Internal")]
	//	private static extern void openCameraPermissionSettingView();
	//#endif
	//

	//    check camera is open
	public static bool CheckCameraOpen()
	{

		#if UNITY_IOS && !UNITY_EDITOR
		if (PTUniInterface.HasCameraPermission() == false)
		{
		PTUniInterface.GotoCameraPermissionSettingView (LocalizeText.GetString(TextContent.CameraPermission),LocalizeText.GetString(TextContent.GrantAccess),LocalizeText.GetString(TextContent.NotGrantAccess));
		return false;
		}
		#endif
		return true;
	}

	/*
	public static void CheckMicOpen(){
		#if UNITY_IOS && !UNITY_EDITOR
			PTGame.WeiDu.PTUniInterface.CheckMicOpen ();
		#endif
	}
	*/

	//    check camera is open
	public static bool CheckMicOpen()
	{
		#if UNITY_IOS && !UNITY_EDITOR
		if (PTUniInterface.HasMicPermission() == false)
		{
		PTUniInterface.GotoCameraPermissionSettingView (LocalizeText.GetString(TextContent.MicPermission),LocalizeText.GetString(TextContent.GrantAccess),LocalizeText.GetString(TextContent.NotGrantAccess));
		return false;
		}
		#endif
		return true;
	}


	/// <summary>
	/// 删除指定文件夹
	/// </summary>
	/// 
	public static void deleteDir(string strDir)
	{
		if (Directory.Exists(strDir))
		{
			Directory.Delete(strDir, true);
			Debug.Log("文件删除成功");
		}
		else
		{
			Debug.Log("此文件夹不存在");
		}
	}

	/// <summary>
	/// 删除指定文件
	/// </summary>
	/// 
	public static void deleteFile(string strFile)
	{
		if (File.Exists(strFile))
		{
			try
			{
				File.Delete(strFile);
				Debug.Log("文件删除成功");
			}
			catch (System.IO.IOException e)
			{
				Debug.Log(e.Message);
				return;
			}
		}
		else
		{
			Debug.Log("此文件夹不存在");
		}
	}

	public static void InitUtils(){
		mVideo = Camera.main.transform.Find("OceanCamera").gameObject;
		//oceanModel = GameObject.Find("newscene").gameObject;
		//oceanModel = GameObject.Find("newscene").gameObject;
		//int themeId = GameInfo.Instance.GetCurTheme ();
		//ground = oceanModel.transform.Find("NO_" + themeId).gameObject.transform.Find("beijing").gameObject;
		//         shuimian = GameObject.Find("Game/Main Camera/shuimian").gameObject;
		//         Lightbeam_Rays = GameObject.Find("Game/Main Camera/Lightbeam_Rays").gameObject;
		mWebTexture = Video.GetComponent<PTARWebCam>();

	}


//
//	//开启和关闭
//	//OceanVideo
//	public static void openOceanVideo(bool boo)
//	{
//		Debug.LogWarning ("1");
//		oceanModel = GameObject.Find("newscene").gameObject;
//		int themeId = GameInfo.curThemeId;
//		ground = oceanModel.transform.Find("NO_" + themeId  + "(Clone)").gameObject.transform.Find("offset/beijing").gameObject;
//
//		Debug.LogWarning ("2");
//		//打开
//		if (boo && GameInfo.isOceanVideo == false)
//		{
//			Debug.LogWarning ("3");
//			if (Video != null) {
//				Debug.LogWarning ("11");
//				Video.SetActive (true);
//			}
//
//			if (WebTexture != null) {
//				Debug.LogWarning ("12");
//				WebTexture.StartCamera (WebTexture.isFrontFacing);
//			}
//
//			// 			Camera.main.transform.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
//			ground.SetActive(false);
//			//             shuimian.SetActive(false);
//			//             Lightbeam_Rays.SetActive(false);
//			//开启相机
//			UIManager.Instance.OpenUI(UIGoPath.Photo, UIManager.CanvasLevel.Middle);
//
//			Debug.LogWarning ("4");
//
//		}
//
//		if (!boo && GameInfo.isOceanVideo == true)
//		{
//			Debug.LogWarning ("5");
//
//			Camera.main.transform.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
//			if (WebTexture != null)
//				WebTexture.CloseCamera();
//			if (Video != null)
//				Video.SetActive(false);
//
//			if (ground != null)
//				ground.SetActive(true);
//
//			if (UIManager.Instance.GetPanelControl<UIPhoto>(UIGoPath.Photo))
//			{
//				Debug.LogWarning ("7");
//
//				//关闭uiPhoto
//				UIManager.Instance.GetPanelControl<UIPhoto>(UIGoPath.Photo).ShiftOutUI();
//				//                 shuimian.SetActive(true);
//				//                 Lightbeam_Rays.SetActive(true);
//			}   
//			Debug.LogWarning ("6");
//
//		}
//
//		GameInfo.isOceanVideo = boo;
//	}
//


	public static string getPlatform()
	{
		//ipad1，iPad2Gen
		string platform = "ipad234";
		switch (Device.generation)
		{
		case DeviceGeneration.iPad1Gen:
		case DeviceGeneration.iPad2Gen:
		case DeviceGeneration.iPad3Gen:
		case DeviceGeneration.iPad4Gen:
		case DeviceGeneration.iPadUnknown:
			platform = "ipad234";
			break;

		case DeviceGeneration.iPadAir1:
		case DeviceGeneration.iPadAir2:
		case DeviceGeneration.iPadPro10Inch1Gen:
			platform = "ipadari12";
			break;

		case DeviceGeneration.iPadPro1Gen:
			platform = "ipadpro";
			break;

		case DeviceGeneration.iPadMini1Gen:
		case DeviceGeneration.iPadMini2Gen:
		case DeviceGeneration.iPadMini3Gen:
			platform = "ipadmini123";
			break;

		case DeviceGeneration.iPadMini4Gen:
			platform = "ipadmini4";
			break;
		}

		return platform;
	}
}
