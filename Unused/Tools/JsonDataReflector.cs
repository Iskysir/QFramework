using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SgJson
{
	public class OptionalAttribute : System.Attribute
	{
		private int mFlag = 0;

		public OptionalAttribute()
			: this(0)
		{
		}
		public OptionalAttribute(int flag)
		{
			this.mFlag = flag;
		}
		public int Flag
		{
			get
			{
				return this.mFlag;
			}
		}
	}

	public class IgnoreFieldAttribute : System.Attribute
	{
	}

	public class IntToFloatAttribute : System.Attribute
	{
		private int mRate = 0;

		public IntToFloatAttribute()
			: this(1)
		{
		}
		public IntToFloatAttribute(int rate)
		{
			this.mRate = rate;
		}
		public int Rate
		{
			get
			{
				return this.mRate;
			}
		}
	}

	public class PlusAttribute : System.Attribute{
	}

	public class CopyAttribute : System.Attribute {
		private string mName = string.Empty;
		public CopyAttribute(string n) : base() {
			this.mName = n;
		}
		public string Name {
			get {
				return this.mName;
			}
		}
	}

	//add by nstar.
	public class RetainAttribute : System.Attribute {
	}
	//add by nstar.
	public class InitAttribute : System.Attribute {
	}
	//just for debug.
	public class CopyAssetsAttribute : System.Attribute {}

	public class ServerNameAttribute : System.Attribute
	{
		private string mUpName = null;
		private string mDownName = null;
		public ServerNameAttribute()
			: this("name")
		{
		}
		public ServerNameAttribute(string n)
			: this(n, n)
		{
		}
		public ServerNameAttribute(string dName, string uName)
		{
			this.mDownName = dName;
			this.mUpName = uName;
		}
		public string UploadName
		{
			get
			{
				return this.mUpName;
			}
		}
		public string DownloadName
		{
			get
			{
				return this.mDownName;
			}
		}
	}

	public class EnumExAttribute : System.Attribute
	{
		private string mName = null;
		public EnumExAttribute()
			: this("name")
		{
		}
		public EnumExAttribute(string n)
		{
			this.mName = n;
		}
		public static string GetName(object enm)
		{
			string str = enm.ToString();
			System.Reflection.MemberInfo[] mi = enm.GetType().GetMember(enm.ToString());
			if ((null != mi) && (mi.Length > 0))
			{
				EnumExAttribute attr = (EnumExAttribute.GetCustomAttribute(mi[0], typeof(EnumExAttribute)) as EnumExAttribute);
				if (null != attr)
				{
					str = attr.mName;
				}
			}
			return str;
		}
	}

	public class IgnoreDefaultAttribute : System.Attribute
	{
		private object mDefaultObj = null;
		public IgnoreDefaultAttribute()
			: this(0)
		{
		}
		public IgnoreDefaultAttribute(object obj)
		{
			this.mDefaultObj = obj;
		}
		public bool IsDefault(object obj)
		{
			if (null == this.mDefaultObj)
			{
				if (null == obj)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			if (this.mDefaultObj.Equals(obj))
			{
				return true;
			}
			return false;
		}
	}

	public class JsonDataReflector
	{

		private static JsonDataReflector _instance = null;
		public static JsonDataReflector Instance
		{
			get
			{
				if (null == _instance)
				{
					_instance = new JsonDataReflector();
				}
				return _instance;
			}
		}

		//	public System.Action<string, string> evtExceptionCatched = null;

		public delegate object CustomDeserialize(Hashtable hs);

		private class CustomParser
		{
			public System.Type cType = null;
			public CustomDeserialize parser = null;
		};
		private List<System.Action<string, string>> mExceptionListeners = null;
		private List<CustomParser> mParsers = null;

		private JsonDataReflector()
		{
			this.mParsers = new List<CustomParser>();
			this.mExceptionListeners = new List<Action<string, string>>();
		}

		public void AddExceptionListener(System.Action<string, string> listener)
		{
			if (!this.mExceptionListeners.Contains(listener))
			{
				this.mExceptionListeners.Add(listener);
			}
		}

		public bool RemoveExceptionListener(System.Action<string, string> listener)
		{
			return this.mExceptionListeners.Remove(listener);
		}

		public void RegisterCustomParser(System.Type type, CustomDeserialize parser)
		{
			CustomParser p = null;
			p = this.GetParser(type);
			if (null == p)
			{
				p = new CustomParser();
				p.cType = type;
				p.parser = parser;
				this.mParsers.Add(p);
			}
		}

		public bool UnregisterCustomParser(System.Type type)
		{
			CustomParser p = null;
			p = this.GetParser(type);
			return this.mParsers.Remove(p);
		}

		#region reflector custom object.
		public T getCustomObjectFromJsonData<T>(Hashtable jsData) where T : class
		{
			T rObj = null;
			rObj = (this.getObjectFromJsonData(typeof(T), jsData) as T);
			return rObj;
		}

		public T getCustomGenericObjectFromArray<T>(ArrayList srcArr) where T : class
		{
			T rObj = null;
			rObj = (this.getGenericObjectFromArray(typeof(T), srcArr) as T);
			return rObj;
		}

		public object getObjectFromJsonData(System.Type oType, Hashtable jsData)
		{
			object rObj = null;
			object tObj = null;
			int tInt = 0;
			string tStr = null;
			string svName = null;

			FieldInfo[] fInfos = oType.GetFields();
			ArrayList tArr = null;
			Hashtable sHash = null;

			CustomParser cParser = null;
			cParser = this.GetParser(oType);
			if (null != cParser)
			{
				rObj = cParser.parser(jsData);
				return rObj;
			}

			rObj = System.Activator.CreateInstance(oType);
			foreach (FieldInfo f in fInfos)
			{
				svName = this.getServerName(f, false);
				
				try{
				if (!jsData.ContainsKey(svName))
				{
					continue;
				}
				}catch(Exception){
					Gdb.L("abcde====="+f.Name+"|"+svName);
				}
				if (f.IsLiteral)
				{
					continue;
				}
				if (this.IsFalse(jsData[svName]))
				{
#if UNITY_EDITOR
					//				Gdb.E("False key=>" + svName + ":" + jsData[svName]);
#endif
					continue;
				}
				if ((typeof(int) == f.FieldType) || (f.FieldType.IsEnum))
				{
					try
					{
						tInt = System.Convert.ToInt32(jsData[svName]);
						f.SetValue(rObj, tInt);
					}
					catch (System.Exception e)
					{
						Gdb.E(e.Message + ":" + svName);
						this.DispatchException((e.Message + ":" + svName), "FormatException: Input string was not in the correct format\r\n");
					}
				}
				//			else if(f.FieldType.IsEnum) {
				//				tInt = System.Convert.ToInt32(jsData[svName]);
				//				f.SetValue(rObj, tInt);
				//			}
				else if (typeof(string) == f.FieldType)
				{
					//				tStr = (jsData[svName] as string);
					tStr = System.Convert.ToString(jsData[svName]);
					f.SetValue(rObj, tStr);
				}
				else if (f.FieldType.IsArray)
				{
					tArr = (jsData[svName] as ArrayList);
					tObj = this.getArrayObjectFromArray(f.FieldType, tArr);
					f.SetValue(rObj, tObj);
				}
				else if (f.FieldType.IsGenericType)
				{
					tArr = (jsData[svName] as ArrayList);
					tObj = this.getGenericObjectFromArray(f.FieldType, tArr);
					f.SetValue(rObj, tObj);
				}
				else if (f.FieldType.IsValueType)
				{
					if (typeof(CryptoIntVar) == f.FieldType)
					{
						object cObj = f.GetValue(rObj);
						int civ = 0;
						try
						{
							civ = System.Convert.ToInt32(jsData[svName]);
						}
						catch (System.Exception e)
						{
							Gdb.E("Err:" + svName + ":" + e.Message);
						}
						PropertyInfo pInfo = f.FieldType.GetProperty("Val", BindingFlags.NonPublic | BindingFlags.Instance);
						pInfo.SetValue(cObj, civ, null);
						f.SetValue(rObj, cObj);
					}
					else if (typeof(CryptoFloatVar) == f.FieldType)
					{
						object cObj = f.GetValue(rObj);

						IntToFloatAttribute sAtt = GetCustomAttribute<IntToFloatAttribute>(f);
						float cfv = System.Convert.ToSingle(jsData[svName]);
						if (sAtt != null)
						{
							cfv /= sAtt.Rate;
						}
						PropertyInfo pInfo = f.FieldType.GetProperty("Val", BindingFlags.NonPublic | BindingFlags.Instance);
						pInfo.SetValue(cObj, cfv, null);
						f.SetValue(rObj, cObj);
					}
					else
					{
						//no such struct.
					}
				}
				else if (f.FieldType.IsClass)
				{
					if (typeof(CryptoIntDefined) == f.FieldType)
					{
						//can not set. just for predefined assets file.
						//					CryptoIntDefined cid = (f.GetValue(rObj) as CryptoIntDefined);
						//					cid = (int)(System.Convert.ToInt32(jsData[f.Name]));
					}
					else if (typeof(CryptoFloatDefined) == f.FieldType)
					{
						//can not set. just for predefined assets file.
					}
					else
					{
						sHash = (jsData[svName] as Hashtable);
						tObj = this.getObjectFromJsonData(f.FieldType, sHash);
						f.SetValue(rObj, tObj);
					}
				}
			}
			return rObj;
		}

		public T GetCustomAttribute<T>(System.Reflection.FieldInfo f) where T : class
		{
			T rcObj = null;
			object[] attrs = f.GetCustomAttributes(false);
			foreach (object t in attrs)
			{
				rcObj = (t as T);
				if (null != rcObj)
				{
					break;
				}
			}
			return rcObj;
		}

		public object getArrayObjectFromArray(System.Type aTp, ArrayList srcArr)
		{
			int idx = 0;
			System.Type eleTp = null;
			Hashtable aHash = null;
			Array rtObj = null;
			object aObj = null;

			eleTp = aTp.GetElementType();
//			rObj = System.Activator.CreateInstance(aTp, srcArr.Count);
			rtObj = Array.CreateInstance(eleTp, srcArr.Count);
			//		rObj = (tt as object[]);

			for (idx = 0; idx < srcArr.Count; idx++)
			{
				if (eleTp == typeof(int))
				{
					//				rObj[idx] = (int)(srcArr[idx]);
//					(rObj as int[])[idx] = System.Convert.ToInt32(srcArr[idx]);
					rtObj.SetValue(System.Convert.ToInt32(srcArr[idx]), idx);
				}
				else if (eleTp == typeof(string))
				{
//					(rObj as string[])[idx] = System.Convert.ToString(srcArr[idx]);
					//				rObj[idx] = (string)(srcArr[idx]);
					rtObj.SetValue(System.Convert.ToString(srcArr[idx]), idx);
				}
				else if (eleTp.IsArray)
				{
					//can not support array in array.
					ArrayList tArr = (srcArr[idx] as ArrayList);
//					(rObj as object[])[idx] = this.getArrayObjectFromArray(eleTp, tArr);
					rtObj.SetValue(tArr, idx);
				}
				else if (typeof(CryptoIntVar) == eleTp)
				{
//					(rObj as CryptoIntVar[])[idx] = System.Convert.ToInt32(srcArr[idx]);
					CryptoIntVar cInt = System.Convert.ToInt32(srcArr[idx]);
					rtObj.SetValue(cInt, idx);
				}
				else if (typeof(CryptoFloatVar) == eleTp)
				{
//					(rObj as CryptoFloatVar[])[idx] = System.Convert.ToSingle(srcArr[idx]);
					CryptoFloatVar cFloat = System.Convert.ToSingle(srcArr[idx]);
					rtObj.SetValue(cFloat, idx);
				}
				else if (eleTp.IsClass)
				{
					aHash = (srcArr[idx] as Hashtable);
					aObj = this.getObjectFromJsonData(eleTp, aHash);
//					(rObj as object[])[idx] = aObj;
					rtObj.SetValue(aObj, idx);
				}
			}
			return rtObj;
		}

		public object getGenericObjectFromArray(System.Type gType, ArrayList srcArr)
		{
			System.Type dType = null;
//			System.Type constructed = null;
			object rObj = null;
			object tObj = null;
			System.Reflection.MethodInfo addMInfo = null;

			dType = gType.GetGenericArguments()[0];
			if (gType.IsGenericTypeDefinition)
			{
				//constructed = 
                gType.MakeGenericType(new System.Type[] { dType });
			}
			else
			{
				//constructed = gType;
			}
			rObj = System.Activator.CreateInstance(gType);
			addMInfo = rObj.GetType().GetMethod("Add");

			if (dType == typeof(int))
			{
//				for (int i = 0; i < srcArr.Count; i++)
//				{
//					Debug.Log("====="+srcArr[i].GetType());
//					Debug.Log("=====1" + srcArr[i]);
//				}
				foreach (object tmp in srcArr)
				{
					addMInfo.Invoke(rObj, new object[] { System.Convert.ToInt32(tmp) });
				}
			}
			else if (dType == typeof(string))
			{
				foreach (object tmp in srcArr)
				{
					addMInfo.Invoke(rObj, new object[] { System.Convert.ToString(tmp) });
				}
			}
			else if (typeof(CryptoIntVar) == dType)
			{
				foreach (int tmp in srcArr)
				{
					addMInfo.Invoke(rObj, new object[] { new CryptoIntVar(tmp) });
				}
			}
			else if (typeof(CryptoFloatVar) == dType)
			{
                foreach (float tmp in srcArr)
                {
                    addMInfo.Invoke(rObj, new object[] { new CryptoFloatVar(tmp) });
                }
            }
			else if (dType.IsGenericType)
			{
				foreach(ArrayList obj in srcArr) {
					tObj = this.getGenericObjectFromArray(dType, obj);
					addMInfo.Invoke(rObj, new object[]{tObj});
				} 
			}
			else if (dType.IsClass)
			{
				foreach (Hashtable obj in srcArr)
				{
					tObj = this.getObjectFromJsonData(dType, obj);
					addMInfo.Invoke(rObj, new object[] { tObj });
				}
			}

			return rObj;
		}
		#endregion reflector custom object.


		#region ---------- reflector Hashtable----------
		public Hashtable getJsonDataFromObject(object obj)
		{
			Hashtable rHash = null;
			Hashtable tHash = null;
			ArrayList tArr = null;
			FieldInfo[] fInfos = null;
			object tmp = null;
			string svName = null;

			rHash = new Hashtable();

			fInfos = obj.GetType().GetFields();
			foreach (FieldInfo f in fInfos)
			{
				if (f.IsLiteral)
				{
					continue;
				}
				if (this.hasIgnoreFieldAttribute(f))
				{
					continue;
				}
				svName = this.getServerName(f, true);
				if ((typeof(int) == f.FieldType) || (typeof(string) == f.FieldType))
				{
					tmp = f.GetValue(obj);
					if (this.hasIgnoreValue(f, tmp))
					{
						continue;
					}
					rHash.Add(svName, tmp);
				}
				else if (f.FieldType.IsEnum)
				{
					rHash.Add(svName, (int)(f.GetValue(obj)));
				}
				else if (f.FieldType.IsGenericType || f.FieldType.IsArray)
				{
					tArr = this.getArrayFromGenericObject(f.GetValue(obj));
					rHash.Add(svName, tArr);
				}
				else if (f.FieldType.IsClass)
				{
					if (typeof(CryptoIntDefined) == f.FieldType)
					{
						CryptoIntDefined cid = (f.GetValue(obj) as CryptoIntDefined);
						rHash.Add(svName, (int)(cid));
					}
					else if (typeof(CryptoFloatDefined) == f.FieldType)
					{
						CryptoFloatDefined cfd = (f.GetValue(obj) as CryptoFloatDefined);
						rHash.Add(svName, (float)(cfd));
					}
					else
					{
						tHash = this.getJsonDataFromObject(f.GetValue(obj));
						rHash.Add(svName, tHash);
					}
				}
				else if (f.FieldType.IsValueType)
				{
					if (typeof(CryptoIntVar) == f.FieldType)
					{
						CryptoIntVar civ = (CryptoIntVar)f.GetValue(obj);
						rHash.Add(svName, (int)(civ));
					}
					else if (typeof(CryptoFloatVar) == f.FieldType)
					{
						CryptoFloatVar cfv = (CryptoFloatVar)f.GetValue(obj);
						rHash.Add(svName, (float)(cfv));
					}
				}
			}
			return rHash;
		}

		public ArrayList getArrayFromGenericObject(object obj)
		{
			ArrayList rList = null;
			ICollection iList = null;
			System.Type tTp = null;
			Hashtable tHash = null;
			rList = new ArrayList();
			iList = (obj as ICollection);
			foreach (object t in iList)
			{
				tTp = t.GetType();
				if ((t is int) || (t is string))
				{
					rList.Add(t);
				}
				else if (tTp.IsGenericType || tTp.IsArray)
				{
					//skip list. dictionary
					ArrayList tArr = this.getArrayFromGenericObject(t);
					rList.Add(tArr);
				}
				else if (tTp.IsValueType)
				{
					if (typeof(CryptoIntVar) == tTp)
					{
						CryptoIntVar cObj = (CryptoIntVar)(t);
						rList.Add((int)cObj);
					}
					else if (typeof(CryptoFloatVar) == tTp)
					{
						CryptoFloatVar cObj = (CryptoFloatVar)(t);
						rList.Add((float)cObj);
					}
				}
				else if (tTp.IsClass)
				{
					if (t is CryptoIntDefined)
					{
						CryptoIntDefined cid = (t as CryptoIntDefined);
						rList.Add((int)(cid));
					}
					else if (t is CryptoFloatDefined)
					{
						CryptoFloatDefined cfd = (t as CryptoFloatDefined);
						rList.Add((float)(cfd));
					}
					else
					{
						tHash = this.getJsonDataFromObject(t);
						rList.Add(tHash);
					}
				}
			}
			return rList;
		}
		#endregion ----------reflector Hashtable----------

		private bool hasIgnoreValue(FieldInfo f, object obj)
		{
			IgnoreDefaultAttribute igAtt = null;
			object[] attrs = f.GetCustomAttributes(false);
			foreach (object att in attrs)
			{
				igAtt = (att as IgnoreDefaultAttribute);
				if (null == igAtt)
				{
					continue;
				}
				if (igAtt.IsDefault(obj))
				{
					return true;
				}
			}
			return false;
		}

		private bool hasIgnoreFieldAttribute(FieldInfo f)
		{
			object[] attrs = f.GetCustomAttributes(false);
			foreach (object att in attrs)
			{
				if (att is IgnoreFieldAttribute)
				{
					return true;
				}
			}
			return false;
		}

		private string getServerName(FieldInfo f, bool bUpload)
		{
			string svName = f.Name;
			object[] attrs = f.GetCustomAttributes(false);
			foreach (object att in attrs)
			{
				if (att is ServerNameAttribute)
				{
					if (bUpload)
					{
						svName = (att as ServerNameAttribute).UploadName;
					}
					else
					{
						svName = (att as ServerNameAttribute).DownloadName;
					}
					break;
				}
			}
			return svName;
		}

		private bool IsFalse(object obj)
		{
			bool bVal = false;
			string bStr = string.Empty;
			if (obj == null)
			{
				return true;
			}
			if (obj.GetType() == typeof(bool))
			{
				bVal = (bool)(obj);
			}
			else if (obj.GetType() == typeof(string))
			{
				bStr = obj.ToString();
				bStr = bStr.ToLower().Trim();
				if (bStr.Equals("false"))
				{
					bVal = true;
				}
			}
			return bVal;
		}

		private CustomParser GetParser(System.Type type)
		{
			foreach (CustomParser p in this.mParsers)
			{
				if (p.cType == type)
				{
					return p;
				}
			}
			return null;
		}

		private void DispatchException(string msg, string stackTrace)
		{
			if (null == this.mExceptionListeners)
			{
				return;
			}
			foreach (System.Action<string, string> tmp in this.mExceptionListeners)
			{
				tmp(msg, stackTrace);
			}
		}

		//赋值，对象A中的属性赋值到对象B中相同的属性上
		public void CopySamePropertyValue(object source, object target)
		{
			System.Type sourceType = source.GetType();
			FieldInfo[] fInfos = sourceType.GetFields();
			System.Type targetType = target.GetType();
			object rObj = null;
			foreach (FieldInfo fInfo in fInfos)
			{
				if (fInfo.IsLiteral)
				{
					continue;
				}
				string fieldName = fInfo.Name;
				FieldInfo targetField = targetType.GetField(fieldName);
				if (targetField != null)
				{
					try
					{
						rObj = this.ConvertType(targetField.FieldType, fInfo.GetValue(source));
						targetField.SetValue(target, rObj);
					}
					catch (Exception e)
					{
						Gdb.E(e.Message + ":" + fieldName);
					}
				}
			}
		}

		//加值，对象A中的数值类属性加值到对象B中相同的属性上
		public void PlusSamePropertyValue(object source, object target)
		{
			System.Type sourceType = source.GetType();
			FieldInfo[] fInfos = sourceType.GetFields();
			System.Type targetType = target.GetType();
			object rObj = null;
			foreach (FieldInfo fInfo in fInfos)
			{
				if (fInfo.IsLiteral)
				{
					continue;
				}
				string fieldName = fInfo.Name;
				FieldInfo targetField = targetType.GetField(fieldName);
				if (targetField != null)
				{
					try
					{
						rObj = this.ConvertAndPlusType(targetField.FieldType, fInfo.GetValue(source), targetField.GetValue(target));
						targetField.SetValue(target, rObj);
					}
					catch (Exception e)
					{
						Gdb.E(e.Message + ":" + fieldName);
					}
				}
			}
		}

        //减值，对象A中的数值类属性加值到对象B中相同的属性上
        public void SubSamePropertyValue(object source, object target)
        {
            System.Type sourceType = source.GetType();
            FieldInfo[] fInfos = sourceType.GetFields();
            System.Type targetType = target.GetType();
            object rObj = null;
            foreach (FieldInfo fInfo in fInfos)
            {
                if (fInfo.IsLiteral)
                {
                    continue;
                }
                string fieldName = fInfo.Name;
                FieldInfo targetField = targetType.GetField(fieldName);
                if (targetField != null)
                {
                    try
                    {
                        rObj = this.ConvertAndSubType(targetField.FieldType, fInfo.GetValue(source), targetField.GetValue(target));
                        targetField.SetValue(target, rObj);
                    }
                    catch (Exception e)
                    {
                        Gdb.E(e.Message + ":" + fieldName);
                    }
                }
            }
        }

		//乘值，对象A中的数值类属性乘到对象B中相同的属性上
		public void MultiSamePropertyValue(object source, object target , float extraRatio = 0.0f)
		{
			System.Type sourceType = source.GetType();
			FieldInfo[] fInfos = sourceType.GetFields();
			System.Type targetType = target.GetType();
			object rObj = null;
			foreach (FieldInfo fInfo in fInfos)
			{
				if (fInfo.IsLiteral)
				{
					continue;
				}
				string fieldName = fInfo.Name;
				FieldInfo targetField = targetType.GetField(fieldName);
				if (targetField != null)
				{
					try
					{
						rObj = this.ConvertAndMultiType(targetField.FieldType, fInfo.GetValue(source), targetField.GetValue(target), extraRatio);
						targetField.SetValue(target, rObj);
					}
					catch (Exception e)
					{
						Gdb.E(e.Message + ":" + fieldName);
					}
				}
			}
		}

		private object ConvertAndPlusType(System.Type tgtType, object srcObj , object tftObj)
		{
			if (typeof(int) == tgtType)
			{
				int rInt = (int)(srcObj) + (int)(tftObj);
				return rInt;
			}
			else if (typeof(float) == tgtType)
			{
				float rFloat = (float)(srcObj) + (float)(tftObj);
				return rFloat;
			}
			else if (typeof(CryptoIntVar) == tgtType)
			{
				CryptoIntVar cvInt = 0;
				if (srcObj.GetType() == typeof(int))
				{
					cvInt = (int)(srcObj) + (CryptoIntVar)(tftObj);
				}
				else if (srcObj.GetType() == typeof(CryptoIntDefined))
				{
					CryptoIntDefined cdInt = (srcObj as CryptoIntDefined);
					CryptoIntVar cdInt2 = (CryptoIntVar)tftObj;
					cvInt = cdInt + cdInt2;
				}
				else if (srcObj.GetType() == typeof(CryptoFloatVar))
				{
					CryptoFloatVar cdInt = (CryptoFloatVar)srcObj;
					CryptoIntVar cdInt2 = (CryptoIntVar)tftObj;
					cvInt = (CryptoIntVar)(cdInt + cdInt2);
				}
				else
				{
					cvInt = (CryptoIntVar)(srcObj) + (CryptoIntVar)(tftObj);
				}
				return cvInt;
			}
			else if (typeof(CryptoFloatVar) == tgtType)
			{
				CryptoFloatVar cvFloat = 0;
				if (srcObj.GetType() == typeof(float))
				{
					cvFloat = (float)(srcObj) + (CryptoFloatVar)(tftObj);
				}
				else if (srcObj.GetType() == typeof(CryptoFloatDefined))
				{
					CryptoFloatDefined cdFloat = (srcObj as CryptoFloatDefined);
					CryptoFloatVar cdFloat2 = (CryptoFloatVar)tftObj;
					cvFloat = cdFloat + cdFloat2;
				}
				else if (srcObj.GetType() == typeof(CryptoIntVar))
				{
					CryptoIntVar cdFloat = (CryptoIntVar)srcObj;
					CryptoFloatVar cdFloat2 = (CryptoFloatVar)tftObj;
					cvFloat = cdFloat + cdFloat2;
				}
				else
				{
					cvFloat = (CryptoFloatVar)(srcObj) + (CryptoFloatVar)(tftObj);
				}
				return cvFloat;
			}
			else
			{
				return srcObj;
			}
//			return null;
		}

        private object ConvertAndSubType(System.Type tgtType, object srcObj, object tftObj)
        {
            if (typeof(int) == tgtType)
            {
                int rInt = (int)(tftObj) - (int)(srcObj);
                return rInt;
            }
            else if (typeof(float) == tgtType)
            {
                float rFloat = (float)(tftObj) - (float)(srcObj);
                return rFloat;
            }
            else if (typeof(CryptoIntVar) == tgtType)
            {
                CryptoIntVar cvInt = 0;
                if (srcObj.GetType() == typeof(int))
                {
                    cvInt = (CryptoIntVar)(tftObj) - (int)(srcObj);
                }
                else if (srcObj.GetType() == typeof(CryptoIntDefined))
                {
                    CryptoIntDefined cdInt = (srcObj as CryptoIntDefined);
                    CryptoIntVar cdInt2 = (CryptoIntVar)tftObj;
                    cvInt = cdInt2 - cdInt;
                }
                else if (srcObj.GetType() == typeof(CryptoFloatVar))
                {
                    CryptoFloatVar cdInt = (CryptoFloatVar)srcObj;
                    CryptoIntVar cdInt2 = (CryptoIntVar)tftObj;
                    cvInt = (CryptoIntVar)(cdInt2 - cdInt);
                }
                else
                {
                    cvInt = (CryptoIntVar)(tftObj) - (CryptoIntVar)(srcObj);
                }
                return cvInt;
            }
            else if (typeof(CryptoFloatVar) == tgtType)
            {
                CryptoFloatVar cvFloat = 0;
                if (srcObj.GetType() == typeof(float))
                {
                    cvFloat = (CryptoFloatVar)(tftObj) - (float)(srcObj);
                }
                else if (srcObj.GetType() == typeof(CryptoFloatDefined))
                {
                    CryptoFloatDefined cdFloat = (srcObj as CryptoFloatDefined);
                    CryptoFloatVar cdFloat2 = (CryptoFloatVar)tftObj;
                    cvFloat = cdFloat2 - cdFloat;
                }
                else if (srcObj.GetType() == typeof(CryptoIntVar))
                {
                    CryptoIntVar cdFloat = (CryptoIntVar)srcObj;
                    CryptoFloatVar cdFloat2 = (CryptoFloatVar)tftObj;
                    cvFloat = cdFloat2 - cdFloat;
                }
                else
                {
                    cvFloat = (CryptoFloatVar)(tftObj) - (CryptoFloatVar)(srcObj);
                }
                return cvFloat;
            }
            else
            {
                return srcObj;
            }
//            return null;
        }

		private object ConvertAndMultiType(System.Type tgtType, object srcObj, object tftObj, float extraRatio)
		{
			if (typeof(int) == tgtType)
			{
				int rInt = (int)(((float)(srcObj) + extraRatio) * (int)(tftObj));
				return rInt;
			}
			else if (typeof(float) == tgtType)
			{
				float rFloat = ((float)(srcObj) + extraRatio) * (float)(tftObj);
				return rFloat;
			}
			else if (typeof(CryptoIntVar) == tgtType)
			{
				CryptoIntVar cvInt = 0;
				if (srcObj.GetType() == typeof(int))
				{
                    cvInt = (int)(((float)(srcObj) + extraRatio) * (CryptoIntVar)(tftObj));
				}
				else if (srcObj.GetType() == typeof(CryptoIntDefined))
				{
                    CryptoFloatDefined cdInt = (srcObj as CryptoFloatDefined);
					CryptoIntVar cdInt2 = (CryptoIntVar)tftObj;
					cvInt = (CryptoIntVar)((cdInt + extraRatio) * cdInt2);
				}
				else if (srcObj.GetType() == typeof(CryptoFloatVar))
				{
					CryptoFloatVar cdInt = (CryptoFloatVar)srcObj;
					CryptoIntVar cdInt2 = (CryptoIntVar)tftObj;
					cvInt = (CryptoIntVar)((cdInt + extraRatio) * cdInt2);
				}
				else
				{
                    cvInt = (CryptoIntVar)(((CryptoFloatVar)(srcObj) + extraRatio) * (CryptoIntVar)(tftObj));
				}
				return cvInt;
			}
			else if (typeof(CryptoFloatVar) == tgtType)
			{
				CryptoFloatVar cvFloat = 0;
				if (srcObj.GetType() == typeof(float))
				{
					cvFloat = ((float)(srcObj) + extraRatio) * (CryptoFloatVar)(tftObj);
				}
				else if (srcObj.GetType() == typeof(CryptoFloatDefined))
				{
					CryptoFloatDefined cdFloat = (srcObj as CryptoFloatDefined);
					CryptoFloatVar cdFloat2 = (CryptoFloatVar)tftObj;
					cvFloat = (cdFloat + extraRatio) * cdFloat2;
				}
				else if (srcObj.GetType() == typeof(CryptoIntVar))
				{
                    CryptoFloatVar cdFloat = (CryptoFloatVar)srcObj;
					CryptoFloatVar cdFloat2 = (CryptoFloatVar)tftObj;
					cvFloat = (cdFloat + extraRatio) * cdFloat2;
				}
				else
				{
					cvFloat = ((CryptoFloatVar)(srcObj)+extraRatio) * (CryptoFloatVar)(tftObj);
				}
				return cvFloat;
			}
			else
			{
				return srcObj;
			}
//			return null;
		}

		private object ConvertType(System.Type tgtType, object srcObj) {
			if(typeof(int) == tgtType) {
				int rInt = (int)(srcObj);
				return rInt;
			}
			else if(typeof(float) == tgtType) {
				float rFloat = (float)(srcObj);
				return rFloat;
			}
			else if(typeof(bool) == tgtType) {
				bool rBool = (bool)(srcObj);
				return rBool;
			}
			else if(typeof(CryptoIntVar) == tgtType) {
				CryptoIntVar cvInt = 0;
				if(srcObj.GetType() == typeof(int)) {
					cvInt = (int)(srcObj);
				}
				else if(srcObj.GetType() == typeof(CryptoIntDefined)) {
					CryptoIntDefined cdInt = (srcObj as CryptoIntDefined);
					cvInt = cdInt;
				}
				else {
					cvInt = (CryptoIntVar)(srcObj);
				}
				return cvInt;
			}
			else if(typeof(CryptoFloatVar) == tgtType) {
				CryptoFloatVar cvFloat = 0;
				if(srcObj.GetType() == typeof(float)) {
					cvFloat = (float)(srcObj);
				}
				else if(srcObj.GetType() == typeof(CryptoFloatDefined)) {
					CryptoFloatDefined cdFloat = (srcObj as CryptoFloatDefined);
					cvFloat = cdFloat;
				}
				else {
					cvFloat = (CryptoFloatVar)(srcObj);
				}
				return cvFloat;
			}else
			{
				return srcObj;
			}
//			return null;
		}
	}
}


