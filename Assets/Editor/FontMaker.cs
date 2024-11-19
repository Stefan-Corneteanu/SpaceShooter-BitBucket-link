using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FontMaker : EditorWindow
{
    public string letters = "";
    public Object fontObject = null;
    public Object texObject = null;

    [MenuItem("SpaceShooter/FontCreator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<FontMaker>("Font Creator");
    }

    private void OnGUI()
    {
        texObject = EditorGUILayout.ObjectField("Sprite Source", texObject, typeof(Texture2D), false);
        GUILayout.Space(10);
        fontObject = EditorGUILayout.ObjectField("Font Destination", fontObject, typeof(Font), false);
        GUILayout.Space(10);
        letters = EditorGUILayout.TextField("Characters", letters);

        if (GUILayout.Button("Create") && texObject && fontObject)
        {
            Font font = fontObject as Font;
            Texture2D tex = texObject as Texture2D;

            if (font && tex)
            {
                Debug.Log("Creating Font");
                Sprite[] sprites = Resources.LoadAll<Sprite>(tex.name);
                CharacterInfo[] charInfos = new CharacterInfo[sprites.Length];

                float width = tex.width;
                float height = tex.height;

                if (letters.Length != sprites.Length)
                {
                    Debug.LogError(letters.Length + " letters and " + sprites.Length + " sprites");
                }

                for (int i = 0; i < sprites.Length; i++)
                {
                    float charWidth = sprites[i].rect.width;
                    float charHeight = sprites[i].rect.height;
                    float charX = sprites[i].rect.x;
                    float charY = sprites[i].rect.y;
                    int charAdv = (int)charWidth + 1;

                    CharacterInfo charInfo = new CharacterInfo();
                    charInfo.index = letters[i];

                    charInfo.uvBottomLeft = new Vector2(charX / width, charY / height);
                    charInfo.uvBottomRight = new Vector2( (charX + charWidth) / width , charY / height);
                    charInfo.uvTopLeft = new Vector2(charX / width, (charY + charHeight) / height);
                    charInfo.uvTopRight = new Vector2( (charX + charWidth) / width, (charY + charHeight) / height);

                    charInfo.minX = 0;
                    charInfo.maxX = (int)charWidth;
                    charInfo.minY = -(int)charHeight;
                    charInfo.maxY = 0;

                    charInfo.advance = charAdv;

                    charInfos[i] = charInfo;
                }
                font.characterInfo = charInfos;
                Debug.Log("Font Created");
            }
        }
    }
}

//!"#$%&'()*+,-./0123456789:;<=>?ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~@ 