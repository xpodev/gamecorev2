using UnityEngine;

namespace GameCore.ImageLoader
{
    public interface IImageLoader
    {
        Texture2D LoadImage(string path);

        void SaveImage(Texture2D texture, string path);
    }
}
