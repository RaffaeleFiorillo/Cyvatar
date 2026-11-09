using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace Cyvatar.src
{
    public static class GraphicManager
    {
        public static SpriteBatch Screen;
        public static string BaseImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Images");

        // Helper to draw colored rectangles (since MonoGame has no built-in DrawRectangle with color)
        private static Texture2D _pixel;

        private static Texture2D ResizeTexture(GraphicsDevice graphicsDevice, Texture2D original, Vector2 newSize)
        {
            int width = (int)newSize.X;
            int height = (int)newSize.Y;

            Texture2D resized = new Texture2D(
                graphicsDevice,
                width,
                height
            );

            Color[] originalData = new Color[original.Width * original.Height];
            original.GetData(originalData);

            Color[] resizedData = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int srcX = x * original.Width / width;
                    int srcY = y * original.Height / height;

                    resizedData[y * width + x] =
                        originalData[srcY * original.Width + srcX];
                }
            }
            resized.SetData(resizedData);

            return resized;
        }

        public static Texture2D LoadImage(GraphicsDevice graphicsDevice, string path, Vector2? size = null, float scale=1.0f)
        {
            Texture2D image = Texture2D.FromFile(graphicsDevice, Path.Combine(BaseImagePath, path));

            if (size != null)
                return ResizeTexture(graphicsDevice, image, size.Value);
            else if (scale != 1.0f)
            {
                Vector2 newSize = new Vector2((image.Width * scale), (image.Height * scale));
                return ResizeTexture(graphicsDevice, image, newSize);
            }

            return image;
        }

        public static void DrawRectangle(Rectangle rect, Color color)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(Screen.GraphicsDevice, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }

            Screen.Draw(_pixel, rect, color);
        }
    }
}
