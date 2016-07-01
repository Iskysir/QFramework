using UnityEngine;
using System.Collections;

/// <summary>
/// 纹理工具
/// </summary>
public class TextureTool  {

	public static Sprite SpriteForTexture(Texture2D texture)
	{
		return Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.one * 0.5f);
	}

}
