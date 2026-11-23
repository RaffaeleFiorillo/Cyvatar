using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cyvatar.src
{
    public static class GraphicManager
    {
        public static SpriteBatch Screen;
        public static ContentManager Content;
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

        public static SpriteFont LoadFont(string fontName)
        {
           SpriteFont font = Content.Load<SpriteFont>(fontName);
            return font;
        }

        /// <summary>
        /// Displays Text on the screen, inside the limits of a given rectangle.
        /// </summary>
        /// <param name="font">Font of the Text.</param>
        /// <param name="text">The text that will be written on the screen.</param>
        /// <param name="area">Informs the bounds of the text and the position of where the text will be on the screen.</param>
        /// <param name="color">Color of the Text.</param>
        /// <param name="padding">The mount of empty space surrounding the text.</param>
        public static void DrawPaddedText
            (
                string text,
                SpriteFont font,
                Rectangle area,
                Color color,
                int padding = 0
            )
        {
            Vector2 textSize = font.MeasureString(text);

            // Available space inside the rectangle
            float availableWidth = area.Width - padding * 2;
            float availableHeight = area.Height - padding * 2;

            // Scale text down if necessary
            float scale = Math.Min(
                1f,
                Math.Min(
                    availableWidth / textSize.X,
                    availableHeight / textSize.Y
                )
            );

            Vector2 position = new Vector2(
                area.X + (area.Width - textSize.X * scale) / 2f,
                area.Y + (area.Height - textSize.Y * scale) / 2f
            );

            Screen.DrawString(
                font,
                text,
                position,
                color,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f
            );
        }

        public static void DrawCenteredText(
                string text,
                SpriteFont font,
                Rectangle area,
                Color color,
                int padding = 5,
                float scale = 1f
            )
        {
            Vector2 textSize = font.MeasureString(text) * scale;

            // Available area after padding
            int availableWidth = area.Width - padding * 2;
            int availableHeight = area.Height - padding * 2;

            // Center the text inside the padded area
            Vector2 position = new Vector2(
                area.X + padding + (availableWidth - textSize.X) / 2f,
                area.Y + padding + (availableHeight - textSize.Y) / 2f
            );

            Screen.DrawString(font, text, position, color, 0f,
                Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
