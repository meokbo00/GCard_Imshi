using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteExporter : MonoBehaviour
{
    [MenuItem("Tools/Export Sprites")]
    static void ExportSprites()
    {
        string path = "Assets/ExportedSprites"; // 저장될 폴더 경로
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        Object[] sprites = Selection.objects; // 선택한 이미지

        foreach (Object obj in sprites)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GetAssetPath(obj));
            string texturePath = AssetDatabase.GetAssetPath(texture);
            Object[] data = AssetDatabase.LoadAllAssetsAtPath(texturePath);

            foreach (Object asset in data)
            {
                if (asset is Sprite sprite)
                {
                    SaveSpriteToFile(sprite, path);
                }
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Sprites Exported Successfully!");
    }

    static void SaveSpriteToFile(Sprite sprite, string path)
    {
        Texture2D spriteTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        Color[] pixels = sprite.texture.GetPixels(
            (int)sprite.rect.x, (int)sprite.rect.y,
            (int)sprite.rect.width, (int)sprite.rect.height
        );
        spriteTexture.SetPixels(pixels);
        spriteTexture.Apply();

        byte[] bytes = spriteTexture.EncodeToPNG();
        File.WriteAllBytes($"{path}/{sprite.name}.png", bytes);
    }
}
