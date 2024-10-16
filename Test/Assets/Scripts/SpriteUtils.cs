using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteUtils
{
    public static byte[] ConvertSpriteToByteArray(Sprite sprite)
    {
        if (sprite == null) return null;

        Texture2D texture = sprite.texture;
        return texture.EncodeToPNG(); // You can change this to other formats if needed
    }

    public static Sprite ConvertByteArrayToSprite(byte[] data)
    {
        if (data == null) return null;

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(data);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
}
