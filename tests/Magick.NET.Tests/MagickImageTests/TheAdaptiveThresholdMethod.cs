﻿// Copyright Dirk Lemstra https://github.com/dlemstra/Magick.NET.
// Licensed under the Apache License, Version 2.0.

using System;
using ImageMagick;
using Xunit;

namespace Magick.NET.Tests;

public partial class MagickImageTests
{
    public class TheAdaptiveThresholdMethod
    {
#if !Q16HDRI
        [Fact]
        public void ShouldThrowExceptionWhenBiasPercentagetIsNegative()
        {
            using var image = new MagickImage(Files.MagickNETIconPNG);
            Assert.Throws<ArgumentException>("biasPercentage", () => image.AdaptiveThreshold(10, 10, new Percentage(-1), Channels.Red));
        }
#else
        [Fact]
        public void ShouldNotThrowExceptionWhenBiasPercentagetIsNegative()
        {
            using var image = new MagickImage(Files.MagickNETIconPNG);
            image.AdaptiveThreshold(10, 10, new Percentage(-1), Channels.Red);
        }
#endif

        [Fact]
        public void ShouldAllowNegativeBiasValue()
        {
            using var image = new MagickImage(Files.MagickNETIconPNG);
            image.AdaptiveThreshold(10, 10, -1, Channels.Red);
        }

        [Fact]
        public void ShouldThresholdTheImage()
        {
            using var image = new MagickImage(Files.MagickNETIconPNG);
            image.AdaptiveThreshold(10, 10);

            ColorAssert.Equal(MagickColors.White, image, 50, 75);
        }

        [Fact]
        public void ShouldUseTheCorrectDefaultValueForBias()
        {
            using var imageA = new MagickImage(Files.MagickNETIconPNG);
            using var imageB = imageA.Clone();
            imageA.AdaptiveThreshold(10, 10);
            imageB.AdaptiveThreshold(10, 10, 0.0);

            var distortion = imageA.Compare(imageB, ErrorMetric.RootMeanSquared);
            Assert.Equal(0.0, distortion);
        }

        [Fact]
        public void ShouldUseTheCorrectDefaultValueForBiasWithChannels()
        {
            using var imageA = new MagickImage(Files.MagickNETIconPNG);
            using var imageB = imageA.Clone();
            imageA.AdaptiveThreshold(10, 10, Channels.Red);
            imageB.AdaptiveThreshold(10, 10, 0.0, Channels.Red);

            var distortion = imageA.Compare(imageB, ErrorMetric.RootMeanSquared);
            Assert.Equal(0.0, distortion);
        }
    }
}
