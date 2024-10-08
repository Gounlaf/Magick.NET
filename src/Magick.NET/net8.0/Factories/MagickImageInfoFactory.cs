﻿// Copyright Dirk Lemstra https://github.com/dlemstra/Magick.NET.
// Licensed under the Apache License, Version 2.0.

#if !NETSTANDARD2_0

using System;
using System.Buffers;

namespace ImageMagick.Factories;

/// <summary>
/// Class that can be used to create <see cref="IMagickImageInfo"/> instances.
/// </summary>
public sealed partial class MagickImageInfoFactory
{
    /// <summary>
    /// Initializes a new instance that implements <see cref="IMagickImageInfo"/>.
    /// </summary>
    /// <param name="data">The sequence of bytes to read the information from.</param>
    /// <returns>A new <see cref="IMagickImageInfo"/> instance.</returns>
    /// <exception cref="MagickException">Thrown when an error is raised by ImageMagick.</exception>
    public IMagickImageInfo Create(ReadOnlySequence<byte> data)
        => new MagickImageInfo(data);

    /// <summary>
    /// Initializes a new instance that implements <see cref="IMagickImageInfo"/>.
    /// </summary>
    /// <param name="data">The span of bytes to read the information from.</param>
    /// <returns>A new <see cref="IMagickImageInfo"/> instance.</returns>
    /// <exception cref="MagickException">Thrown when an error is raised by ImageMagick.</exception>
    public IMagickImageInfo Create(ReadOnlySpan<byte> data)
        => new MagickImageInfo(data);
}

#endif
