using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileSet : MonoBehaviour {

    [System.Serializable]
    public class CommonTexture
    {
        public Sprite Sprite;
        public Texture2D Texture;
        public bool Animated;
        public float FrameRate = 10;

        public bool HasValue
        {
            get { return Sprite != null || Texture != null; }
        }
        public bool IsTexture
        {
            get { return Texture != null; }
        }

        public Object Value
        {
            get
            {
                if (IsTexture)
                {
                    return Texture;
                }
                return Sprite;
            }
        }
    }

    public string Name;
    public float ZOrdering = 0.0f;
    public CommonTexture CenterTiles;

    [System.Flags]
    public enum EdgeFlags
    {
        None =   0x00,
        Left =   0x01,
        Right =  0x02,
        Top =    0x04,
        Bottom = 0x08
    }

    [System.Flags]
    public enum CornerFlags
    {
        None =          0x00,
        TopLeft =       0x01,
        TopRight =      0x02,
        BottomLeft =    0x04,
        BottomRight =   0x08
    }

    public CommonTexture[] EdgeTiles = new CommonTexture[16];
    public CommonTexture[] CornerTiles = new CommonTexture[16];
}
