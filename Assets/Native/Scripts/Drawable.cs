using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

namespace Molotkoff.Test2App
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class Drawable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField]
        private Color color;
        [SerializeField]
        private int width;
        [SerializeField]
        private DrawMemoData drawMemoData;

        public Color Color { get => color; set => color = value; }
        public int BrushWidth { get => width; set => width = value; }

        private Sprite sprite;
        private Texture2D texture;
        private Color[] pixels;

        private Vector2 previousPosition;

        private void Awake()
        {
            sprite           = GetComponent<SpriteRenderer>().sprite;
            texture          = sprite.texture;
            pixels           = texture.GetPixels();
            previousPosition = Vector2.zero;
        }

        public void Clear()
        {
            drawMemoData.memos.Push(new DrawMemo(pixels));

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }

            ApplyMarkedPixelChanges();
        }

        public void Return()
        {
            if (drawMemoData.memos.Count > 0)
            {
                var drawMemo = drawMemoData.memos.Pop();

                pixels = drawMemo.Colors;

                ApplyMarkedPixelChanges();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            drawMemoData.memos.Push(new DrawMemo(pixels));

            var mouse_world_position = Camera.main.ScreenToWorldPoint(eventData.position);

            PenBrush(mouse_world_position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            previousPosition = Vector2.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            var mouse_world_position = Camera.main.ScreenToWorldPoint(eventData.position);

            PenBrush(mouse_world_position);
        }

        private void PenBrush(Vector2 worldPosition)
        {
            var pixelPosition = WorldToPixelCoordinates(worldPosition);

            if (previousPosition == Vector2.zero)
            {
                // If this is the first time we've ever dragged on this image, simply colour the pixels at our mouse position
                MarkPixelsToColour(pixelPosition, width, color);
            }
            else
            {
                // Colour in a line from where we were on the last update call
                ColourBetween(previousPosition, pixelPosition, width, color);
            }

            ApplyMarkedPixelChanges();

            previousPosition = pixelPosition;
        }

        // Set the colour of pixels in a straight line from start_point all the way to end_point, to ensure everything inbetween is coloured
        private void ColourBetween(Vector2 start_point, Vector2 end_point, int width, Color color)
        {
            // Get the distance from start to finish
            var distance = Vector2.Distance(start_point, end_point);

            var cur_position = start_point;

            // Calculate how many times we should interpolate between start_point and end_point based on the amount of time that has passed since the last update
            var lerp_steps = 1 / distance;

            for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
            {
                cur_position = Vector2.Lerp(start_point, end_point, lerp);
                MarkPixelsToColour(cur_position, width, color);
            }
        }

        private void MarkPixelsToColour(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            //int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                // Check if the X wraps around the image, so we don't draw pixels on the other side of the image
                if (x >= (int)sprite.rect.width || x < 0)
                    continue;

                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    MarkPixelToChange(x, y, color_of_pen);
                }
            }
        }

        private void MarkPixelToChange(int x, int y, Color color)
        {
            // Need to transform x and y coordinates to flat coordinates of array
            int array_pos = y * (int)sprite.rect.width + x;

            // Check if this is a valid position
            if (array_pos > pixels.Length || array_pos < 0)
                return;

            try
            {
                pixels[array_pos] = color;
            }
            catch (Exception) 
            { 

            }
        }

        private void ApplyMarkedPixelChanges()
        {
            texture.SetPixels(pixels);
            texture.Apply();
        }

        private Vector2 WorldToPixelCoordinates(Vector2 worldPosition)
        {
            // Change coordinates to local coordinates of this image
            var local_pos = transform.InverseTransformPoint(worldPosition);

            // Change these to coordinates of pixels
            var pixelWidth = sprite.rect.width;
            var pixelHeight = sprite.rect.height;
            var unitsToPixels = pixelWidth / sprite.bounds.size.x * transform.localScale.x;

            // Need to center our coordinates
            var centered_x = local_pos.x * unitsToPixels + pixelWidth / 2;
            var centered_y = local_pos.y * unitsToPixels + pixelHeight / 2;

            // Round current mouse position to nearest pixel
            var pixel_pos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

            return pixel_pos;
        }
    }
}