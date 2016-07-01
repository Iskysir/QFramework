using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine.EventSystems;


/// <summary>
/// 崩溃检测
/// 
///     . 功能
///         . 不包含:
///             . android 崩溃时的系统堆栈信息
///         . 只包含
///             . 崩溃前一段时间, 游戏内的 内存/对象 快照信息
///             . 是否发生了崩溃异常
///             . 上报给服务端, 做统计
///             
///     . 原理
///         . 游戏启动时( OnAppFocus, init )
///             . 调用 InitLog 创建日志
///         . 游戏运行中( OnLateUpdate )
///             . 调用 UpdateLog 写入日志(当前内存信息)
///         . 游戏正常退出( OnAppFocus, OnAppQuit )
///             . 调用 ClearLog 清空日志
///         . 游戏异常退出
///             . log 未被清空, 记录了上次崩溃前的内存信息
///             . 调用 GetPrevCrashLogs 判断是否有崩溃日志
///             
/// </summary>
class CrashDetector
{
    string _prev_crash_logs;         // 崩溃日志

    //Application.RegisterLogCallback(MyExpception.OnLogCallback);


    // 初始化
    public CrashDetector()
    {
        _prev_crash_logs = LoadLogFile();
        if (_prev_crash_logs != null)
        {
            GameLog.LogInfo("has prev crash log");
        }

        AddEventListeners();
        InitLog();
    }

    // 上次是否崩溃
    public bool HasPrevCrashLogs
    {
        get { return _prev_crash_logs != null; }
    }

    // 获取上次崩溃日志
    public string PrevCrashLogs
    {
        get { return _prev_crash_logs; }
    }

    #region 事件侦听

    float _time_next_update;

    //
    void AddEventListeners()
    {
        ///Test
//         BaseApp.OnAppFocusPauseEvent += OnAppFousePause;
//         App.OnApplicationQuitEvent += OnAppQuit;
//         App.LateUpdateEvent += OnLateUpdate;
//         MyExpception.OnErrorEvent += OnError;
//         UIBehaviour.BordcastEvent += OnUIEvent;
//         MyPanel2.OnStartShowEvent += OnPanelShow;
//         MyPanel2.OnStartHideEvent += OnPanelHide;
    }

    void OnAppFousePause()
    {
        var valid = true;//BaseApp.is_app_focus && !BaseApp.is_app_pause;
        if (valid)
        {
            _time_next_update = 0;
            InitLog();
        }
        else
        {
            ClearLog();
        }
    }

    void OnAppQuit()
    {
        ClearLog();
    }

    void OnLateUpdate()
    {
        // 记录内存信息
        if (Time.time >= _time_next_update)
        {
            _time_next_update = Time.time + 5;

           // var str = Utils.getGameMemory();
            //AddLog(str);
        }
    }

    void OnError(LogType type, string content)
    {
        AddLog(content);
    }

    /*
    void OnUIEvent(UIBehaviour sender, string type, UIEventData eventData)
    {
        // 记录点击
        if (type == UIEvent.Click)
        {
            var id = (sender != null ? sender.name : null);
            var str = string.Format("click:{0}", id);
            AddLog(str);
        }
    }

    void OnPanelShow(MyPanel2 p)
    {
        // 记录打开界面
        var str = string.Format("show:{0}", p.GetType().Name);
        AddLog(str);
    }

    void OnPanelHide(MyPanel2 p)
    {
        // 记录关闭界面
        var str = string.Format("hide:{0}", p.GetType().Name);
        AddLog(str);
    }*/

    #endregion

    #region 文件 IO

    string _log_fname;

    //
    string GetLogFileName()
    {
        if (_log_fname == null)
        {
            var path = "Doucments/";//BaseApp.GetLocalSavePath("logs");
            _log_fname = Path.Combine(path, "crash_log.txt");
        }
        return _log_fname;
    }

    void SaveLogFile(string title1, string title2, List<string> logs)
    {
        try
        {
            var fname = GetLogFileName();
            var arr = new string[logs.Count + 2];
            arr[0] = title1;
            arr[1] = title2;
            for (int i = 2; i < arr.Length; i++)
            {
                arr[i] = logs[i - 2];
            }
            File.WriteAllLines(fname, arr);

//#if UNITY_EDITOR
//            Log.LogInfo("SaveLogFile\n" + string.Join("\n", arr));
//#endif

        }
        catch (Exception e)
        {
            GameLog.LogError(e.Message);
        }
    }

    void ClearLogFile()
    {
        try
        {
            var fname = GetLogFileName();
            if (File.Exists(fname))
            {
                File.Delete(fname);
            }
        }
        catch (Exception e)
        {
            GameLog.LogError(e.Message);
        }
    }

    string LoadLogFile()
    {
        string ret = null;
        try
        {
            var fname = GetLogFileName();
            if (File.Exists(fname))
            {
                var arr = File.ReadAllLines(fname);
                ret = string.Join("|", arr);
            }
        }
        catch (Exception e)
        {
            ret = null;
            GameLog.LogError(e.Message);
        }
        return ret;
    }

    #endregion

    #region 日志

    string _title_game;                 // 全局标题
    string _title_log;                  // 日志标题
    List<string> _logs;          // 日志列表  MaxSizeList

    //
    void InitLog()
    {
        var date_str = DateTime.Now.ToLongTimeString(); //DateTime.Now.ToDateTimeStr();

        if (_title_game == null) _title_game = "game:" + date_str;
        _title_log = "log:" + date_str;
        _logs = new List<string>(20);

        ClearLogFile();
    }

    void ClearLog()
    {
        _title_log = null;
        _logs = null;

        ClearLogFile();
    }

    void AddLog(string str)
    {
        if (_logs == null) return;

        // 添加 时间/场景 信息
        {
            var now = DateTime.Now;
            var time_str = now.ToString("HH:mm:ss");
            var scene_id = "scene_id";//BaseApp.cur_scene_id;
            str = string.Format("{0}, s:{1}, {2}", time_str, scene_id, str);
        }

        // 添加到日志
        _logs.Add(str);

        // 保存日志
        SaveLogFile(_title_game, _title_log, _logs);//_logs.List
    }

    #endregion
}