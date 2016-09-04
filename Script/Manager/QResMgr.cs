using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using QFramework;

// 资源管理器，封装开发模式和发布模式
namespace QFramework {
	
	public class QResMgr : QMonoSingleton<QResMgr>
	{

		/// <summary>
		/// 缓存的 AssetBundle
		/// </summary>
		Dictionary<string,AssetBundle> mCachedABs = new Dictionary<string, AssetBundle> ();


		public GameObject LoadUIPrefabSync(string uiName)
		{
			return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject> ("Assets/QArt/QAB" + "/UIPrefab/" + uiName + ".prefab");
		}
			

		public IEnumerator LoadAB(string bundleName) {

			if (mCachedABs.ContainsKey (bundleName.ToLower())) {

				yield return null;
			}
			else {
				WWW www = new WWW ("file://" + QPath.ABBuildOutPutDir (RuntimePlatform.IPhonePlayer) + "/" + bundleName.ToLower());

				yield return www;

				if (string.IsNullOrEmpty (www.error)) {
					mCachedABs.Add (bundleName, www.assetBundle);
				}
				else {
					Debug.LogError (www.error);
				}

				www.Dispose ();
			}
		}


		public void LoadRes(string bundleName,string resName,QVoidDelegate.WithObj loadDoneCallback) {
			
			StartCoroutine (LoadResInternal (bundleName.ToLower (), resName.ToLower (), loadDoneCallback));
		}


		IEnumerator LoadResInternal(string bundleName,string resName,QVoidDelegate.WithObj loadDoneCallback) {

//			yield return LoadAB (bundleName);

			var request = mCachedABs [bundleName].LoadAssetAsync (resName);

			yield return request;

			loadDoneCallback (request.asset);



		}

	}

}
