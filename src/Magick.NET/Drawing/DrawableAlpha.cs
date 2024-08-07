// Copyright Dirk Lemstra https://github.com/dlemstra/Magick.NET.
// Licensed under the Apache License, Version 2.0.

namespace ImageMagick.Drawing;

/// <summary>
/// Paints on the image's alpha channel in order to set effected pixels to transparent.
/// </summary>
public sealed class DrawableAlpha : IDrawableAlpha, IDrawingWand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawableAlpha"/> class.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <param name="paintMethod">The paint method to use.</param>
    public DrawableAlpha(double x, double y, PaintMethod paintMethod)
    {
        X = x;
        Y = y;
        PaintMethod = paintMethod;
    }

    /// <summary>
    /// Gets the X coordinate.
    /// </summary>
    public double X { get; }

    /// <summary>
    /// Gets the Y coordinate.
    /// </summary>
    public double Y { get; }

    /// <summary>
    /// Gets the paint method to use.
    /// </summary>
    public PaintMethod PaintMethod { get; }

    /// <summary>
    /// Draws this instance with the drawing wand.
    /// </summary>
    /// <param name="wand">The want to draw on.</param>
    void IDrawingWand.Draw(DrawingWand wand)
        => wand?.Alpha(X, Y, PaintMethod);
}
