   using System.Collections.Generic;
using SgJson;
using UnityEngine;
using System.Collections;

public class UpdateResMgr {

    private static UpdateResMgr mInstance;

    public static UpdateResMgr Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new UpdateResMgr();
            }
            return mInstance;
        }
    }

    public Dictionary<string, ResCnf> cnfDicLocal = new Dictionary<string, ResCnf>();
    public Dictionary<string, ResCnf> cnfDicRemote = new Dictionary<string, ResCnf>();

    public void LoadLocalData(string text)
    {
        LoadData(text , true);
    }

    public void LoadRemoteData(string text)
    {
        LoadData(text, false);
    }

    private void LoadData(string text , bool isLocalCnf)
    {
        string[] lineArray =
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE
        StringExtention.SplitWithString(text, "\r\n");
#elif UNITY_IOS
        StringExtention.SplitWithString(text, "\n");
#endif


        int l = lineArray.Length - 1;
//        if (l < 2)
//        {
//            Gdb.E("No updateRes config!");
//            return;
//        }

        string[] keys = lineArray[0].Split(',');
        int keyLenth = keys.Length;
        List<Hashtable> hList = new List<Hashtable>();

        for (int i = 1; i <= l; i++)
        {
            if (!lineArray[i].Contains(","))
            {
                continue;
            }
            string[] strArr = lineArray[i].Split(',');

            Hashtable hs = new Hashtable();
            for (int j = 0; j < keyLenth; j++)
            {
                keys[j] = keys[j].Replace("\n", "");
                keys[j] = keys[j].Replace("\r", "");
                hs.Add(keys[j], strArr[j] == "null" ? null : strArr[j]);
            }
            hList.Add(hs);
        }

        foreach (Hashtable hashtable in hList)
        {
            ResCnf conf = JsonDataReflector.Instance.getCustomObjectFromJsonData<ResCnf>(hashtable);
            if (isLocalCnf)
            {
                cnfDicLocal[conf.name] = conf;
            }
            else
            {
                cnfDicRemote[conf.name] = conf;
            }
        }
    }

    public List<string> GetNeedUpdateRes()
    {
        List<string> needUpdateResList = new List<string>();

        foreach (KeyValuePair<string, ResCnf> pair in cnfDicLocal)
        {
            string resName = pair.Value.name;
            string resVersionOld = pair.Value.ver;
            string resVersionNew = cnfDicRemote[resName].ver;
            if (int.Parse(resVersionOld) != int.Parse(resVersionNew))
            {
                needUpdateResList.Add(resName);
            }
        }

        return needUpdateResList;
    }
}


public class StringExtention
{

    public static string[] SplitWithString(string sourceString, string splitString)
    {
        //        string tempSourceString = sourceString;
        List<string> arrayList = new List<string>();
        string s = string.Empty;
        while (sourceString.IndexOf(splitString) > -1)  //分割
        {
            s = sourceString.Substring(0, sourceString.IndexOf(splitString));
            sourceString = sourceString.Substring(sourceString.IndexOf(splitString) + splitString.Length);
            arrayList.Add(s);
        }
        arrayList.Add(sourceString);
        return arrayList.ToArray();
    }
}
