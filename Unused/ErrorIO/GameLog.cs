//#define OUTPUT_NET_LOG       // 是否输入网络
#define ENABLE_LOG
using UnityEngine;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using System.IO;
using System.Text;

/// <summary>
/// 日志输出 
/// 
///     . 支持多线程
///     
/// </summary>
public static class GameLog
{
    #region info/wran/error 日志

    static object _info_lock = new object();
    static object _wran_lock = new object();
    static object _err_lock = new object();
    static HashSet<string> _errs = new HashSet<string>();

    // 普通信息输出
    public static void LogInfo ( string fmt, params object[] args )
    {
#if ENABLE_LOG
        lock (_info_lock)
        {
            var str = args.Length == 0 ? fmt : string.Format(fmt, args);
            var now = DateTime.Now;
            str = string.Format("INFO {0}({1}): {2}", now.ToLongTimeString(), now.Millisecond, str);
            Debug.Log(str);
        }
#endif
    }

    // 游戏状态, 和 LogInfo 类似, 但可以批量屏蔽
    public static void LogGameState ( string fmt, params object[] args )
    {
        LogInfo(fmt, args);
    }

    // 奔溃日志
    public static void LogCrash ( string fmt, params object[] args )
    {
        //LogInfo(fmt, args);
    }

    // 输出 警告
    public static void LogWarning ( string fmt, params object[] args )
    {
#if ENABLE_LOG
        lock (_wran_lock)
        {
            var str = args.Length == 0 ? fmt : string.Format(fmt, args);
            Debug.LogWarning(str);
        }
#endif
    }

    // 输出错误
    public static void LogError ( string fmt, params object[] args )
    {
        var str = args.Length == 0 ? fmt : string.Format(fmt, args);
        lock (_err_lock)
        {
            if (!_errs.Contains(str))   // 避免重复报错
            {
                _errs.Add(str);
                Debug.LogError(str);
            }
        }
    }

    #endregion

    #region 网络日志

    const int MAX_NET_LOGS = 4 * 1024;

    static List<string> _net_logs = new List<string>();

    // 添加网络收发日志
    public static void AddNetLog ( string fmt, params object[] args )
    {
#if ENABLE_LOG
        var str = args.Length == 0 ? fmt : string.Format(fmt, args);
        str = DateTime.Now.ToLongTimeString() + " " + str;
        lock (_net_logs)
        {
#if OUTPUT_NET_LOG
            Debug.Log(str);
#endif
            _net_logs.Add(str);
            if (_net_logs.Count > MAX_NET_LOGS) _net_logs.RemoveAt(0);
        }
#endif
    }

    // 获取网络日志
    public static string GetNetLog ()
    {
        string[] arr = null;
        lock (_net_logs)
        {
            arr = _net_logs.ToArray();
        }
        return string.Join("\r\n", arr);
    }

    #endregion

    #region 临时日志

    const int MAX_TEMP_LOGS = 3;

    static List<string> _temp_log = new List<string>();


    // 添加临时日志
    public static void AddTempLog ( string fmt, params object[] args )
    {
        var str = args.Length == 0 ? fmt : string.Format(fmt, args);
        lock (_temp_log)
        {
            _temp_log.Add(str);
            if (_temp_log.Count > MAX_TEMP_LOGS)
            {
                _temp_log.RemoveAt(0);
            }
        }
    }

    // 获取临时日志
    public static string GetTempLog ()
    {
        string[] arr = null;
        lock (_temp_log)
        {
            arr = _temp_log.ToArray();
        }
        return string.Join("\r\n", arr);
    }

    #endregion

    #region 记录到文件

    static string log_file;

    // 初始化
    static void InitLog2File ()
    {
        if (log_file != null) return;

        // 获取日志目录
        var logpath = Path.Combine(Application.persistentDataPath, "logs/");
		FileTool.CheckAndCreateDir (logpath);

        Func<int, string> getName = ( id ) =>
        {
            return Path.Combine(logpath, string.Format("log{0}.txt", id));
        };

        // 保留最近10个日志文件
        var name9 = getName(9);
        if (File.Exists(name9)) File.Delete(name9);
        for (int i = 8; i >= 0; i--)
        {
            var name1 = getName(i);
            if (File.Exists(name1))
            {
                var name2 = getName(i + 1);
                File.Move(name1, name2);
            }
        }
        log_file = getName(0);
    }

    // 记录到文件
    public static void Log2File ( string fmt, params object[] args )
    {
        if (log_file == null) InitLog2File();

        var str = string.Format(fmt, args) + Environment.NewLine;
        File.AppendAllText(log_file, str, Encoding.UTF8);
    }

    #endregion

}

