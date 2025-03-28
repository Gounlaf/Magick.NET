﻿// Copyright Dirk Lemstra https://github.com/dlemstra/Magick.NET.
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace ImageMagick.ImageOptimizers;

/// <summary>
/// Class that can be used to optimize png files.
/// </summary>
public sealed class PngOptimizer : IImageOptimizer
{
    /// <summary>
    /// Gets the format that the optimizer supports.
    /// </summary>
    public IMagickFormatInfo Format
        => MagickFormatInfo.Create(MagickFormat.Png)!;

    /// <summary>
    /// Gets or sets a value indicating whether various compression types will be used to find
    /// the smallest file. This process will take extra time because the file has to be written
    /// multiple times.
    /// </summary>
    public bool OptimalCompression { get; set; }

    /// <summary>
    /// Performs compression on the specified file. With some formats the image will be decoded
    /// and encoded and this will result in a small quality reduction. If the new file size is not
    /// smaller the file won't be overwritten.
    /// </summary>
    /// <param name="file">The png file to compress.</param>
    /// <returns>True when the image could be compressed otherwise false.</returns>
    public bool Compress(FileInfo file)
        => DoCompress(file, false);

    /// <summary>
    /// Performs compression on the specified file. With some formats the image will be decoded
    /// and encoded and this will result in a small quality reduction. If the new file size is not
    /// smaller the file won't be overwritten.
    /// </summary>
    /// <param name="fileName">The file name of the png image to compress.</param>
    /// <returns>True when the image could be compressed otherwise false.</returns>
    public bool Compress(string fileName)
    {
        var filePath = FileHelper.CheckForBaseDirectory(fileName);

        return DoCompress(new FileInfo(filePath), false);
    }

    /// <summary>
    /// Performs compression on the specified stream. With some formats the image will be decoded
    /// and encoded and this will result in a small quality reduction. If the new size is not
    /// smaller the stream won't be overwritten.
    /// </summary>
    /// <param name="stream">The stream of the png image to compress.</param>
    /// <returns>True when the image could be compressed otherwise false.</returns>
    public bool Compress(Stream stream)
        => DoCompress(stream, false);

    /// <summary>
    /// Performs lossless compression on the specified file. If the new file size is not smaller
    /// the file won't be overwritten.
    /// </summary>
    /// <param name="file">The png file to optimize.</param>
    /// <returns>True when the image could be compressed otherwise false.</returns>
    public bool LosslessCompress(FileInfo file)
    {
        Throw.IfNull(file);

        return DoCompress(file, true);
    }

    /// <summary>
    /// Performs lossless compression on the specified file. If the new file size is not smaller
    /// the file won't be overwritten.
    /// </summary>
    /// <param name="fileName">The png file to optimize.</param>
    /// <returns>True when the image could be compressed otherwise false.</returns>
    public bool LosslessCompress(string fileName)
    {
        var filePath = FileHelper.CheckForBaseDirectory(fileName);

        return DoCompress(new FileInfo(filePath), true);
    }

    /// <summary>
    /// Performs lossless compression on the specified stream. If the new stream size is not smaller
    /// the stream won't be overwritten.
    /// </summary>
    /// <param name="stream">The stream of the png image to compress.</param>
    /// <returns>True when the image could be compressed otherwise false.</returns>
    public bool LosslessCompress(Stream stream)
        => DoCompress(stream, true);

    private static void StartCompression(MagickImage image, bool lossless)
    {
        ImageOptimizerHelper.CheckFormat(image, MagickFormat.Png);

        if (lossless)
        {
            image.RemoveAttribute("date:timestamp");
        }
        else
        {
            image.Strip();
            image.Settings.SetDefine(MagickFormat.Png, "exclude-chunks", "all");
            image.Settings.SetDefine(MagickFormat.Png, "include-chunks", "tRNS,gAMA");
        }
    }

    private bool DoCompress(FileInfo file, bool lossless)
    {
        using var image = new MagickImage(file);
        if (image.GetAttribute("png:acTL") is not null)
        {
            return false;
        }

        StartCompression(image, lossless);

        var isCompressed = false;
        TemporaryFile? bestFile = null;
        try
        {
            var pngHelper = new PngHelper(this);
            bestFile = pngHelper.FindBestFileQuality(image, out _);

            if (bestFile is not null && bestFile.Length < file.Length)
            {
                isCompressed = true;
                bestFile.CopyTo(file);
            }
        }
        finally
        {
            bestFile?.Dispose();
        }

        return isCompressed;
    }

    private bool DoCompress(Stream stream, bool lossless)
    {
        ImageOptimizerHelper.CheckStream(stream);

        var startPosition = stream.Position;

        using var image = new MagickImage(stream);
        StartCompression(image, lossless);

        var isCompressed = false;
        MemoryStream? bestStream = null;
        try
        {
            var pngHelper = new PngHelper(this);
            bestStream = pngHelper.FindBestStreamQuality(image, out _);

            if (bestStream is not null && bestStream.Length < (stream.Length - startPosition))
            {
                isCompressed = true;
                stream.Position = startPosition;
                bestStream.Position = 0;
                bestStream.CopyTo(stream);
                stream.SetLength(startPosition + bestStream.Length);
            }

            stream.Position = startPosition;
        }
        finally
        {
            bestStream?.Dispose();
        }

        return isCompressed;
    }
}
