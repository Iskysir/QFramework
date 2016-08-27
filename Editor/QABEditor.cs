using UnityEditor;
using System.Collections;
using UnityEngine;
using System.IO;

using System.Collections.Generic;

namespace QFramework.Editor {
	public class QABEditor
	{
		[MenuItem("QFramework/AssetBundle/Build")]
		public static void BuildAssetBundle()
		{
			string outPath = Application.streamingAssetsPath + "/QAssetBundle";

			CheckDirAndCreate (outPath);

			BuildPipeline.BuildAssetBundles (outPath, 0, EditorUserBuildSettings.activeBuildTarget);

			AssetDatabase.Refresh ();
		}

		[MenuItem("Qframework/AssetBundle/Mark")]
		public static void MarkAssetBundle()
		{
			AssetDatabase.RemoveUnusedAssetBundleNames ();

			string path = Application.dataPath + "/Art/Scenes/";

			DirectoryInfo dir = new DirectoryInfo (path);

			FileSystemInfo[] fileInfos = dir.GetFileSystemInfos ();

			for (int i = 0; i < fileInfos.Length; i++) 
			{
				FileSystemInfo tmpFile = fileInfos[i];

				if (tmpFile is DirectoryInfo) 
				{
					string tmpPath = Path.Combine (path,tmpFile.Name);

					SceneOverView (tmpPath);
				}
			}
		}

		public static void SceneOverView(string scenePath)
		{
			string textFileName = "Record.txt";

			string tmpPath = scenePath + textFileName;

			FileStream fs = new FileStream (tmpPath, FileMode.OpenOrCreate);

			StreamWriter bw = new StreamWriter (fs);

			Dictionary<string,string> readDict = new Dictionary<string, string> ();

			bw.Close ();

			fs.Close ();
		}
		/// <summary>
		/// 判断路径是否存在
		/// </summary>
		public static void CheckDirAndCreate(string dirPath)
		{
			if (!Directory.Exists (dirPath)) {
				Directory.CreateDirectory (dirPath);
			}
		}

	}

}
