using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.iOS;

/// <summary>
/// 文件的读写等,存不存在等,对File类提供一层封装
/// </summary>
public class FileTool {


	/// <summary>
	/// 检查并创建路径
	/// </summary>
	public static void CheckAndCreateDir(string dirPath)
	{
		if(!Directory.Exists(dirPath)){
			Directory.CreateDirectory (dirPath);
		}
	}

	/// <summary>
	/// 检查并删除文件
	/// </summary>
	public static void CheckAndDeleteFile(string filePath)
	{
		if (File.Exists (filePath)) {

			File.Delete (filePath);
		}
	}



	public static void SaveScreenCapture(Texture2D texture)
	{
		byte[] imagebytes = texture.EncodeToJPG(40);//转化为png图
// 		File.WriteAllBytes(LocalPath.SaveTexturePath, imagebytes);//存储png
		Device.SetNoBackupFlag(LocalPath.SaveTexturePath);     //取消自动备份到icloud




		//保存到相册
		Utils.WriteDataAsImageToPhotosAlbum(LocalPath.SaveTexturePath, imagebytes);
		imagebytes = null;

	}
}
