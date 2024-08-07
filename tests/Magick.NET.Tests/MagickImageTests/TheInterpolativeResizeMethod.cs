﻿// Copyright Dirk Lemstra https://github.com/dlemstra/Magick.NET.
// Licensed under the Apache License, Version 2.0.

using System;
using ImageMagick;
using Xunit;

namespace Magick.NET.Tests;

public partial class MagickImageTests
{
    public class TheInterpolativeResizeMethod
    {
        [Fact]
        public void ShouldThrowExceptionWhenPercentageIsNegative()
        {
            using var image = new MagickImage(Files.RedPNG);
            Assert.Throws<ArgumentException>("percentageWidth", () => image.InterpolativeResize(new Percentage(-1), PixelInterpolateMethod.Mesh));
        }

        [Fact]
        public void ShouldThrowExceptionWhenPercentageWidthIsNegative()
        {
            using var image = new MagickImage(Files.RedPNG);
            Assert.Throws<ArgumentException>("percentageWidth", () => image.InterpolativeResize(new Percentage(-1), new Percentage(10), PixelInterpolateMethod.Mesh));
        }

        [Fact]
        public void ShouldThrowExceptionWhenPercentageHeightIsNegative()
        {
            using var image = new MagickImage(Files.RedPNG);
            Assert.Throws<ArgumentException>("percentageHeight", () => image.InterpolativeResize(new Percentage(10), new Percentage(-1), PixelInterpolateMethod.Mesh));
        }

        [Fact]
        public void ShouldResizeTheImage()
        {
            using var image = new MagickImage(Files.RedPNG);
            image.InterpolativeResize(32, 32, PixelInterpolateMethod.Mesh);

            Assert.Equal(32U, image.Width);
            Assert.Equal(11U, image.Height);
        }

        [Fact]
        public void ShouldUseThePixelInterpolateMethod()
        {
            using var image = new MagickImage(Files.FujiFilmFinePixS1ProPNG);
            image.InterpolativeResize(150, 100, PixelInterpolateMethod.Mesh);

            Assert.Equal(150U, image.Width);
            Assert.Equal(100U, image.Height);

            ColorAssert.Equal(new MagickColor("#acacbcbcb2b2"), image, 20, 37);
            ColorAssert.Equal(new MagickColor("#08891d1d4242"), image, 117, 39);
        }
    }
}
