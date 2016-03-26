using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileSet : MonoBehaviour {

    [System.Serializable]
    public class CommonTexture
    {
        public Sprite Sprite;
        public Texture2D Texture;

        public Object Value
        {
            get
            {
                if (Texture != null)
                {
                    return Texture;
                }
                return Sprite;
            }
        }
    }

    public CommonTexture CenterTiles;

    [System.Flags]
    public enum EdgeFlags
    {
        Left =   0x01,
        Right =  0x02,
        Top =    0x04,
        Bottom = 0x08
    }

    [System.Flags]
    public enum CornerFlags
    {
        TopLeft = 0x01,
        TopRight = 0x02,
        BottomRight = 0x04,
        BottomLeft = 0x08
    }

    public CommonTexture[] EdgeTiles = new CommonTexture[16];
    public CommonTexture[] CornerTiles = new CommonTexture[16];
}
