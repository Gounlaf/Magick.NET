// Copyright Dirk Lemstra https://github.com/dlemstra/Magick.NET.
// Licensed under the Apache License, Version 2.0.

namespace ImageMagick.Drawing;

/// <summary>
/// Specifies the shape to be used at the corners of paths (or other vector shapes) when they
/// are stroked.
/// </summary>
public sealed class DrawableStrokeLineJoin : IDrawableStrokeLineJoin, IDrawingWand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawableStrokeLineJoin"/> class.
    /// </summary>
    /// <param name="lineJoin">The line join.</param>
    public DrawableStrokeLineJoin(LineJoin lineJoin)
    {
        LineJoin = lineJoin;
    }

    /// <summary>
    /// Gets the line join.
    /// </summary>
    public LineJoin LineJoin { get; }

    /// <summary>
    /// Draws this instance with the drawing wand.
    /// </summary>
    /// <param name="wand">The want to draw on.</param>
    void IDrawingWand.Draw(DrawingWand wand)
        => wand?.StrokeLineJoin(LineJoin);
}
