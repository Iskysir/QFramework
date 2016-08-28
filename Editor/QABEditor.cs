using UnityEditor;
using System.Collections;
using UnityEngine;
using System.IO;

using System.Collections.Generic;
using QFramework.AB;

namespace QFramework.PRIVATE {
	
	public class QABEditor
	{

		/// <summary>
		/// 判断路径是否存在
		/// </summary>
		public static void CheckDirAndCreate(string dirPath)
		{
			if (!Directory.Exists (dirPath)) {
				Directory.CreateDirectory (dirPath);
			}
		}


		[MenuItem("QFramework/AB/Build")]
		public static void BuildAssetBundle()
		{
			string outPath = Application.streamingAssetsPath + "/QAB";

			CheckDirAndCreate (outPath);

			BuildPipeline.BuildAssetBundles (outPath, 0, EditorUserBuildSettings.activeBuildTarget);

			AssetDatabase.Refresh ();
		}

		[MenuItem("QFramework/AB/Mark")]
		public static void MarkAssetBundle()
		{
			CheckDirAndCreate (Application.streamingAssetsPath + "/QAB");

			AssetDatabase.RemoveUnusedAssetBundleNames ();

			string path = Application.dataPath + "/QArt/QAB/";

			DirectoryInfo dir = new DirectoryInfo (path);

			FileSystemInfo[] fileInfos = dir.GetFileSystemInfos ();

			for (int i = 0; i < fileInfos.Length; i++) 
			{
				FileSystemInfo tmpFile = fileInfos[i];

				if (tmpFile is DirectoryInfo) 
				{
					string tmpPath = Path.Combine (path,tmpFile.Name);

					MarkABDir (tmpPath);
				}
			}

			AssetDatabase.Refresh ();
		}

		/// <summary>
		/// 标记ABDir
		/// </summary>
		public static void MarkABDir(string scenePath)
		{
			Dictionary<string,string> readDict = new Dictionary<string, string> ();

			ChangeHead (scenePath, readDict);

			QABConfigMgr.Instance.Refresh ();

			foreach (string key in readDict.Keys) 
			{
				if (!string.IsNullOrEmpty (key)) {

					QABConfigMgr.Instance.AddItem (readDict[key]);
				}
			}

			QABConfigMgr.Instance.OverrideConfigFile ();
		}



		/// <summary>
		/// 截取相对路径
		/// </summary>
		public static void ChangeHead(string fullPath,Dictionary<string,string> theWriter)
		{
			int count = fullPath.IndexOf ("Assets");

			int length = fullPath.Length;

			string replacePath = fullPath.Substring (count);

			DirectoryInfo dir = new DirectoryInfo (fullPath);

			if (null != dir) 
			{
				ListFiles (dir, replacePath, theWriter);
			} 
			else 
			{
				Debug.Log ("this path is null");
			}
		}
			
		public static void ListFiles(FileSystemInfo info, string replacePath,Dictionary<string,string> theWriter)
		{
			if (!info.Exists) {
				Debug.Log (" is not exist");
				return;
			}

			DirectoryInfo dir = info as DirectoryInfo;
			FileSystemInfo[] files = dir.GetFileSystemInfos ();

			for (int i = 0; i < files.Length; i++) {
				FileInfo file = files [i] as FileInfo;

				// 对于文件的操作
				if (file != null) 
				{
					ChangeMark (file, replacePath, theWriter);
				} 
				else 	// 对于目录的操作
				{
					ListFiles (files[i], replacePath, theWriter);
				}
			}
		}

		public static string GetBundlePath(FileInfo file,string replacePath)
		{
			string tmpPath = file.FullName;

			int assetCount = tmpPath.IndexOf (replacePath);

			assetCount += replacePath.Length + 1;

			int nameCount = tmpPath.LastIndexOf (file.Name);

			int tmpCount = replacePath.LastIndexOf ("/");

			string sceneHead = replacePath.Substring (tmpCount + 1, replacePath.Length - tmpCount - 1);

			int tmpLength = nameCount - assetCount;

			if (tmpLength > 0) {
				string substring = tmpPath.Substring (assetCount, tmpPath.Length - assetCount);

				string[] result = substring.Split ("/".ToCharArray ());

				return sceneHead + "/" + result [0];
			} else {
				return sceneHead;
			}

		}

		public static void ChangeAssetMark(FileInfo tmpFile,string markStr,Dictionary<string,string> theWriter)
		{
			string fullPath = tmpFile.FullName;

			int assetCount = fullPath.IndexOf ("Assets");


			string assetPath = fullPath.Substring (assetCount, fullPath.Length - assetCount);


			AssetImporter importer = AssetImporter.GetAtPath (assetPath);


			importer.assetBundleName = markStr;

			string modelName = "";

			string[] subMark = markStr.Split ("/".ToCharArray ());
			if (subMark.Length > 1) {
				modelName = subMark [1];	
			} else {
				modelName = markStr;
			}

			string modelPath = markStr.ToLower ();

			if (!theWriter.ContainsKey (modelName)) {
				theWriter.Add (modelName,modelPath);
			}
		}

		public static void ChangeMark(FileInfo tmpFile,string replacePath,Dictionary<string,string> theWriter)
		{
			if (tmpFile == null ||  tmpFile.Extension == ".meta") {
				return;
			}

			string markSr = GetBundlePath (tmpFile, replacePath);

			ChangeAssetMark (tmpFile, markSr, theWriter);
		}
	}
}
