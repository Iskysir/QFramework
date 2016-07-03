using UnityEngine;
using System.Collections;
using System.Text;
using System.Xml;

using QFramework;
/// <summary>
/// 1.Developing 开发版本,此模式下 可以快速定位到某个界面。
/// 2.QA对应测试版本。
/// 3.发布饭,发布时候一些调试的冗余信息删除掉。
/// </summary>
public enum AppMode {
	Developing,
	QA,
	Release
}
	
public partial class AppConfig : QMonoSingleton<AppConfig> {

	public AppMode mode = AppMode.Developing;


}
