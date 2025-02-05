using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public Texture[] atlases;
    public Dictionary<string, Dictionary<string, Sprite>> Sprites { get; private set; }

    private void Awake()
    {
        Sprites = new Dictionary<string, Dictionary<string, Sprite>>();
        for (int i = 0; i < atlases.Length; ++i)
        {
            Sprite[] atlassprites = Resources.LoadAll<Sprite>(atlases[i].name);

            for (int j = 0; j < atlassprites.Length; ++j)
            {
                
            }
        }
    }
}
