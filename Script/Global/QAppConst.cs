﻿using UnityEngine;
using System.Collections;


namespace QFramework {
	/// <summary>
	/// 常量 TODO:这个要干掉,最好是自动生成的唯一表示符号
	/// </summary>
	/// <summary>
	/// 关卡需要生成的标记
	/// </summary>
	public class STAGE {
		public const int EON  = -1;  // Empty Or Not
		public const int EM  = 0;	// Empty 

		public const int BLOCK_BEGIN = 1;
		public const int BL  = 1;  // Block Left
		public const int BM  = 2;  // Block Middle
		public const int BR  = 3;  // Block Right
		public const int BA  = 4;  // Block Air
		public const int BLOCK_END = 5;

		public const int GOLD_BEGIN = 9;
		public const int G1  = 9 ; // Gold x1
		public const int G3  = 10; // Gold x3
		public const int G6  = 11; // Gold x6
		public const int GC  = 12; // Gold Circle
		public const int GOLD_END = 13;

		public const int PROP_BEGIN = 20;
		public const int PA  = 20; // Prop Auto
		public const int PB  = 21; // Prop Big
		public const int PF2 = 22; // Prop Fruit x2
		public const int PG2 = 23; // Prop Gold  x2
		public const int PM  = 24; // Prop Magnetite
		public const int PP  = 25; // Prop Propect
		public const int PROP_END = 26;
		public const int PTE = 27; // Prop Time Extra
		public const int PF  = 28; // Prop Find	// 这个不要了

		public const int FRUIT_BEGIN = 30;
		public const int FB  = 30; // Fruit Banana
		public const int FC  = 31; // Fruit Coconut
		public const int FM  = 32; // Fruit Mango
		public const int FPA = 33; // Fruit Pineapple
		public const int FP  = 34; // Fruit Pitaya
		public const int FRUIT_END = 35;

		public const int ENEMY_BEGIN = 40;
		public const int ET = 40; // Enemy Turtle
		public const int EC = 41; // Enemy Crab
		public const int ENEMY_END = 42;

		/// <summary>
		/// 前景组合
		/// </summary>
		public const int COM1_BEGIN = 100;
		public const int COM2_BEGIN = 100;

		public const int C1 = 100;	// Component1 
		public const int C2 = 101;  // Component2
		public const int C3 = 102;  // Component3
		public const int C4 = 103;  // Component3
		public const int C5 = 104;  // Component3
		public const int C6 = 105;  // Component3
		public const int C7 = 106;  // Component3
		public const int C8 = 107;  // Component3
		public const int C9 = 108;  // Component3

		public const int COM1_END = 109;
		public const int C10 = 109;
		public const int C11 = 110;
		public const int C12 = 111;
		public const int C13 = 112;
		public const int C14 = 113;
		public const int C15 = 114;
		public const int C16 = 115;
		public const int C17 = 116;
		public const int C18 = 117;
		public const int C19 = 128;
		public const int C20 = 119;
		public const int COM2_END = 120;
	}


	/// <summary>
	/// 音效:1.同时可播放多个的,2.一次只播放一个的 3.背景音乐
	/// </summary>

	public class SOUND {

		/// <summary>
		/// 设置
		/// </summary>
		public const int ON = 1;	// 声音开启
		public const int OFF = 0;	// 声音关闭

		public const int COIN = 0;	// 金币
		public const int JUMP = 1;  // 跳
		public const int DEATH = 2; // 死亡音效
		public const int SHAKE =  3; // 震屏

		public const int BTN = 4; //按钮点击
		public const int FRUIT = 5; // 吃到水果的音效

		public const int PROP_BIG = 6; // 变大道具的音效

		public const int ENEMY_DEATH1 = 7; 	// 怪物被踩死或者踢飞
		public const int ENEMY_DEATH2 = 8; // 怪物被踩死或者踢飞

		public const int HERO_HURT = 9;	// 英雄被攻击

		public const int FOREST = 10;      // 森林
		public const int FIRE   = 11;      // 火焰燃烧
		public const int BOSS_SHAKE = 12;  // 敌人出来的时候震动

		public const int COUNT = 13;	// 音效的个数,要开辟的数量

	}
	/// <summary>
	/// 音乐
	/// </summary>
	public class MUSIC {
		public const int HOME_BG = 0;		// 主界面背景音乐
		public const int GAME_BG = 1;  		// 游戏背景音乐

		public const int COUNT = 2;			// 音乐的个数
	}

	public class EMPTY {
		public const int ZERO = 0;
		public const int ONE = 1;
		public const int TWO = 2;
		public const int THREE = 3;
		public const int FOUR = 4;
		public const int FIVE = 5;
		public const int SIX = 6;
	}

	/// <summary>
	/// 这里的配置参考LuaFramework_UGUI
	/// </summary>
	public class QAppConst {
		public const bool DebugMode = false;                       //调试模式-用于内部测试
		/// <summary>
		/// 如果想删掉框架自带的例子，那这个例子模式必须要
		/// 关闭，否则会出现一些错误。
		/// </summary>
		public const bool ExampleMode = false;                       //例子模式 

		/// <summary>
		/// 如果开启更新模式，前提必须启动框架自带服务器端。
		/// 否则就需要自己将StreamingAssets里面的所有内容
		/// 复制到自己的Webserver上面，并修改下面的WebUrl。
		/// </summary>
		public const bool UpdateMode = false;                       //更新模式-默认关闭 
		public const bool LuaByteMode = false;                       //Lua字节码模式-默认关闭 
		public const bool LuaBundleMode = false;                    //Lua代码AssetBundle模式

		public const int TimerInterval = 1;
		public const int GameFrameRate = 30;                        //游戏帧频

		public const string ABPath = "QArt/QAB";               	//资源路径
		public const string LuaTempDir = "Lua/";                    //临时目录
		public const string AppPrefix = ABPath + "_";              //应用程序前缀
		public const string ExtName = ".unity3d";                   //素材扩展名
		public const string AssetDir = "StreamingAssets";           //素材目录 
		public const string WebUrl = "http://localhost:6688/";      //测试更新地址

		public static string UserId = string.Empty;                 //用户ID
		public static int SocketPort = 0;                           //Socket服务器端口
		public static string SocketAddress = string.Empty;          //Socket服务器地址

		public static string FrameworkRoot {
			get {
				return Application.dataPath + "/" + ABPath;
			}
		}
	}
}