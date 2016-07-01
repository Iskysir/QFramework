using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using UnityEngine;


/// <summary>
/// 异常错误处理/记录
///     . 当 异常发生(或调用Debug.LogError) 时,  会把错误信息记录起来, 并上传给上层应用(后者可以发送给服务器做记录)
/// </summary>
public static class MyExpception
{
    public static event Action<LogType, string> OnErrorEvent;       // 日志事件
    public static event Action<Exception> OnExceptionEvent;         // 异常发生时, 会先输出日志, 再发该事件

    const int MAX_STACK_TRACE = 600;

    /// <summary>
    /// 日志输出接管
    ///     . 先派发事件
    ///     . 后进行真正的输出操作
    /// </summary>
    public static void OnLogCallback(string condition, string stackTrace, LogType type)
    {
        condition = condition.Trim();
        stackTrace = stackTrace.Trim();

        // 获取日志内容
        //var msg = string.Format("type:{0}, condition:{1}", type, condition);
        var msg = condition;

        // 判断日志级别
        var is_error = (type == LogType.Assert || type == LogType.Error || type == LogType.Exception);      // error 级别
        var is_warn = (type == LogType.Warning);        // warning 级别

        // 如果是 错误 级别, 加入堆栈
        if (is_error)
        {
            //msg = msg + string.Format(", stackTrance:{0}, tmplog:{1}", stackTrace.Limit(MAX_STACK_TRACE), Log.GetTempLog());
        }

        // 错误日志
        if (is_error)
        {
            _err_list.Add(msg);
            if (OnErrorEvent != null) OnErrorEvent(type, msg);
        }
        // 警告日志
        else if (is_warn)
        {
            _warn_list.Add(msg);
        }
        else
        {
            _info_list.Add(msg);
        }

        // 保存到文件
        if (is_error || is_warn)
        {
            GameLog.Log2File(msg);
        }
    }

    // 日志记录
    const int max_logs = 200;
    //MaxSizeList
    static List<string> _err_list = new List<string>(max_logs);
    static List<string> _warn_list = new List<string>(max_logs);
    static List<string> _info_list = new List<string>(max_logs);

//     // 获取错误信息
//     public static int ErrorCount
//     {
//         get { return _err_list.TotalCount; }
//     }
//     public static List<string> ErrorList
//     {
//         get { return _err_list.List; }
//     }
//     public static List<string> WarningList
//     {
//         get { return _warn_list.List; }
//     }
//     public static List<string> InfoList
//     {
//         get { return _info_list.List; }
//     }

    public static void ClearLogs()
    {
        _err_list.Clear();
        _warn_list.Clear();
        _info_list.Clear();
    }


    /// <summary>
    /// 输出堆栈信息
    /// </summary>
    public static string GetFramesDesc(int start, int count)
    {
        StackFrame[] frames = new StackTrace().GetFrames();
        StringBuilder sb = new StringBuilder();
        sb.Append(DateTime.Now.ToLongTimeString());
        sb.Append(" : ");
        for (int i = start; i < (start + count); i++)
        {
            if (i >= frames.Length) break;
            sb.AppendFormat("{0}.{1} ", frames[i].GetMethod().DeclaringType.Name, frames[i].GetMethod().Name);
        }
        return sb.ToString();
    }

    // 处理异常, 返回是否被识别
    public static bool HandleException(Exception e)
    {
        GameLog.LogError("MyExpception.HandleException, e:{0}", e);

        //
        if (OnExceptionEvent != null) OnExceptionEvent(e);

        // 为了游戏不奔溃, 忽略所有异常
        return true;

        // 可识别的异常
        if (e.GetType() == typeof(Exception)) return true;  // 默认异常
        if (e is NullReferenceException) return true;   // 一般程序错误
        if (e is KeyNotFoundException) return true; // 字典错误
        if (e is IndexOutOfRangeException) return true; // 数组越界
        if (e is InvalidCastException) return true; // 类型转换
        if (e is DivideByZeroException) return true;    // 除以0
        if (e is ArgumentOutOfRangeException) return true;  // 参数越界
        if (e is InvalidOperationException) return true;  // 非法操作

        //
        return false;
    }

}

