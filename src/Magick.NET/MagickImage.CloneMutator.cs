﻿// Copyright Dirk Lemstra https://github.com/dlemstra/Magick.NET.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Globalization;
using ImageMagick.Drawing;

#if Q8
using QuantumType = System.Byte;
#elif Q16
using QuantumType = System.UInt16;
#elif Q16HDRI
using QuantumType = System.Single;
#else
#error Not implemented!
#endif

namespace ImageMagick;

/// <content />
public partial class MagickImage
{
    private class CloneMutator : IMagickImageCloneMutator<QuantumType>, IDisposable
    {
        private IntPtr _result = IntPtr.Zero;

        public CloneMutator(NativeMagickImage nativeMagickImage)
            => NativeMagickImage = nativeMagickImage;

        protected NativeMagickImage NativeMagickImage { get; }

        public void Dispose()
        {
            if (_result != IntPtr.Zero)
                NativeMagickImage.DisposeInstance(_result);
        }

        public IntPtr GetResult()
        {
            var result = _result;
            _result = IntPtr.Zero;
            return result;
        }

        public void AdaptiveBlur()
            => AdaptiveBlur(0.0, 1.0);

        public void AdaptiveBlur(double radius)
            => AdaptiveBlur(radius, 1.0);

        public void AdaptiveBlur(double radius, double sigma)
            => SetResult(NativeMagickImage.AdaptiveBlur(radius, sigma));

        public void AdaptiveResize(uint width, uint height)
            => AdaptiveResize(new MagickGeometry(width, height));

        public void AdaptiveResize(IMagickGeometry geometry)
        {
            Throw.IfNull(nameof(geometry), geometry);

            SetResult(NativeMagickImage.AdaptiveResize(geometry.ToString()));
        }

        public void AdaptiveSharpen()
            => AdaptiveSharpen(0.0, 1.0);

        public void AdaptiveSharpen(Channels channels)
            => AdaptiveSharpen(0.0, 1.0, channels);

        public void AdaptiveSharpen(double radius, double sigma)
            => AdaptiveSharpen(radius, sigma, ImageMagick.Channels.Undefined);

        public void AdaptiveSharpen(double radius, double sigma, Channels channels)
            => SetResult(NativeMagickImage.AdaptiveSharpen(radius, sigma, channels));

        public void AdaptiveThreshold(uint width, uint height)
            => AdaptiveThreshold(width, height, 0.0, ImageMagick.Channels.Undefined);

        public void AdaptiveThreshold(uint width, uint height, Channels channels)
            => AdaptiveThreshold(width, height, 0.0, channels);

        public void AdaptiveThreshold(uint width, uint height, double bias)
            => AdaptiveThreshold(width, height, bias, ImageMagick.Channels.Undefined);

        public void AdaptiveThreshold(uint width, uint height, double bias, Channels channels)
            => SetResult(NativeMagickImage.AdaptiveThreshold(width, height, bias, channels));

        public void AdaptiveThreshold(uint width, uint height, Percentage biasPercentage)
            => AdaptiveThreshold(width, height, biasPercentage, ImageMagick.Channels.Undefined);

        public void AdaptiveThreshold(uint width, uint height, Percentage biasPercentage, Channels channels)
            => AdaptiveThreshold(width, height, PercentageHelper.ToQuantum(nameof(biasPercentage), biasPercentage), channels);

        public void AddNoise(NoiseType noiseType)
            => AddNoise(noiseType, ImageMagick.Channels.Undefined);

        public void AddNoise(NoiseType noiseType, Channels channels)
            => AddNoise(noiseType, 1.0, channels);

        public void AddNoise(NoiseType noiseType, double attenuate)
            => AddNoise(noiseType, attenuate, ImageMagick.Channels.Undefined);

        public void AddNoise(NoiseType noiseType, double attenuate, Channels channels)
            => SetResult(NativeMagickImage.AddNoise(noiseType, attenuate, channels));

        public void AffineTransform(IDrawableAffine affineMatrix)
        {
            Throw.IfNull(nameof(affineMatrix), affineMatrix);

            SetResult(NativeMagickImage.AffineTransform(affineMatrix.ScaleX, affineMatrix.ScaleY, affineMatrix.ShearX, affineMatrix.ShearY, affineMatrix.TranslateX, affineMatrix.TranslateY));
        }

        public void AutoOrient()
            => SetResult(NativeMagickImage.AutoOrient());

        public void BilateralBlur(uint width, uint height)
        {
            var intensitySigma = Math.Sqrt((width * width) + (height * height));
            BilateralBlur(width, height, intensitySigma, intensitySigma * 0.25);
        }

        public void BilateralBlur(uint width, uint height, double intensitySigma, double spatialSigma)
            => SetResult(NativeMagickImage.BilateralBlur(width, height, intensitySigma, spatialSigma));

        public void BlueShift()
            => BlueShift(1.5);

        public void BlueShift(double factor)
            => SetResult(NativeMagickImage.BlueShift(factor));

        public void Blur()
            => Blur(0.0, 1.0);

        public void Blur(Channels channels)
            => Blur(0.0, 1.0, channels);

        public void Blur(double radius, double sigma)
            => Blur(radius, sigma, ImageMagick.Channels.Undefined);

        public void Blur(double radius, double sigma, Channels channels)
            => SetResult(NativeMagickImage.Blur(radius, sigma, channels));

        public void Border(uint size)
            => Border(size, size);

        public void Border(uint width, uint height)
        {
            var rectangle = new MagickRectangle(0, 0, width, height);
            SetResult(NativeMagickImage.Border(rectangle));
        }

        public void Border(Percentage percentage)
            => Border((uint)(NativeMagickImage.Width_Get() * percentage), (uint)(NativeMagickImage.Height_Get() * percentage));

        public void CannyEdge()
            => CannyEdge(0.0, 1.0, new Percentage(10), new Percentage(30));

        public void CannyEdge(double radius, double sigma, Percentage lower, Percentage upper)
            => SetResult(NativeMagickImage.CannyEdge(radius, sigma, lower.ToDouble() / 100, upper.ToDouble() / 100));

        public void Charcoal()
            => Charcoal(0.0, 1.0);

        public void Charcoal(double radius, double sigma)
            => SetResult(NativeMagickImage.Charcoal(radius, sigma));

        public void Chop(IMagickGeometry geometry)
            => SetResult(NativeMagickImage.Chop(MagickRectangle.FromGeometry(geometry, (uint)NativeMagickImage.Width_Get(), (uint)NativeMagickImage.Height_Get())));

        public void ChopHorizontal(int offset, uint width)
             => Chop(new MagickGeometry(offset, 0, width, 0));

        public void ChopVertical(int offset, uint height)
            => Chop(new MagickGeometry(0, offset, 0, height));

        public void Colorize(IMagickColor<QuantumType> color, Percentage alpha)
        {
            Throw.IfNegative(nameof(alpha), alpha);

            Colorize(color, alpha, alpha, alpha);
        }

        public void Colorize(IMagickColor<QuantumType> color, Percentage alphaRed, Percentage alphaGreen, Percentage alphaBlue)
        {
            Throw.IfNull(nameof(color), color);
            Throw.IfNegative(nameof(alphaRed), alphaRed);
            Throw.IfNegative(nameof(alphaGreen), alphaGreen);
            Throw.IfNegative(nameof(alphaBlue), alphaBlue);

            var blend = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}", alphaRed.ToInt32(), alphaGreen.ToInt32(), alphaBlue.ToInt32());

            SetResult(NativeMagickImage.Colorize(color, blend));
        }

        public void ColorMatrix(IMagickColorMatrix matrix)
        {
            Throw.IfNull(nameof(matrix), matrix);

            SetResult(NativeMagickImage.ColorMatrix(matrix));
        }

        public void Convolve(IConvolveMatrix matrix)
        {
            Throw.IfNull(nameof(matrix), matrix);

            SetResult(NativeMagickImage.Convolve(matrix));
        }

        public void Crop(uint width, uint height)
            => Crop(width, height, Gravity.Undefined);

        public void Crop(uint width, uint height, Gravity gravity)
            => Crop(new MagickGeometry(0, 0, width, height), gravity);

        public void Crop(IMagickGeometry geometry)
            => Crop(geometry, Gravity.Undefined);

        public void Crop(IMagickGeometry geometry, Gravity gravity)
        {
            Throw.IfNull(nameof(geometry), geometry);

            SetResult(NativeMagickImage.Crop(geometry.ToString(), gravity));
        }

        public double Deskew(Percentage threshold)
            => Deskew(threshold, autoCrop: false);

        public double DeskewAndCrop(Percentage threshold)
            => Deskew(threshold, autoCrop: true);

        public void Despeckle()
            => SetResult(NativeMagickImage.Despeckle());

        public void Distort(DistortMethod method, params double[] arguments)
            => Distort(new DistortSettings(method), arguments);

        public void Distort(IDistortSettings settings, params double[] arguments)
        {
            Throw.IfNull(nameof(settings), settings);
            Throw.IfNullOrEmpty(nameof(arguments), arguments);

            using var temporaryDefines = new TemporaryDefines(NativeMagickImage);
            temporaryDefines.SetArtifact("distort:scale", settings.Scale);
            temporaryDefines.SetArtifact("distort:viewport", settings.Viewport);

            SetResult(NativeMagickImage.Distort(settings.Method, settings.Bestfit, arguments, (nuint)arguments.Length));
        }

        public void Edge(double radius)
            => SetResult(NativeMagickImage.Edge(radius));

        public void Emboss()
            => Emboss(0.0, 1.0);

        public void Emboss(double radius, double sigma)
            => SetResult(NativeMagickImage.Emboss(radius, sigma));

        public void Enhance()
            => SetResult(NativeMagickImage.Enhance());

        public void Extent(uint width, uint height)
           => Extent(new MagickGeometry(width, height));

        public void Extent(int x, int y, uint width, uint height)
            => Extent(new MagickGeometry(x, y, width, height));

        public void Extent(uint width, uint height, IMagickColor<QuantumType> backgroundColor)
            => Extent(new MagickGeometry(width, height), backgroundColor);

        public void Extent(uint width, uint height, Gravity gravity)
            => Extent(new MagickGeometry(width, height), gravity);

        public void Extent(uint width, uint height, Gravity gravity, IMagickColor<QuantumType> backgroundColor)
            => Extent(new MagickGeometry(width, height), gravity, backgroundColor);

        public void Extent(IMagickGeometry geometry)
            => Extent(geometry, Gravity.Undefined);

        public void Extent(IMagickGeometry geometry, IMagickColor<QuantumType> backgroundColor)
        {
            Throw.IfNull(nameof(backgroundColor), backgroundColor);

            NativeMagickImage.BackgroundColor_Set(backgroundColor);
            Extent(geometry);
        }

        public void Extent(IMagickGeometry geometry, Gravity gravity)
        {
            Throw.IfNull(nameof(geometry), geometry);

            SetResult(NativeMagickImage.Extent(geometry.ToString(), gravity));
        }

        public void Extent(IMagickGeometry geometry, Gravity gravity, IMagickColor<QuantumType> backgroundColor)
        {
            Throw.IfNull(nameof(backgroundColor), backgroundColor);

            NativeMagickImage.BackgroundColor_Set(backgroundColor);
            Extent(geometry, gravity);
        }

        public void Flip()
            => SetResult(NativeMagickImage.Flip());

        public void Flop()
            => SetResult(NativeMagickImage.Flop());

        public void Frame()
            => Frame(new MagickGeometry(6, 6, 25, 25));

        public void Frame(IMagickGeometry geometry)
            => SetResult(NativeMagickImage.Frame(MagickRectangle.FromGeometry(geometry, (uint)NativeMagickImage.Width_Get(), (uint)NativeMagickImage.Height_Get())));

        public void Frame(uint width, uint height)
            => Frame(new MagickGeometry(6, 6, width, height));

        public void Frame(uint width, uint height, int innerBevel, int outerBevel)
            => Frame(new MagickGeometry(innerBevel, outerBevel, width, height));

        public void GaussianBlur(double radius)
            => GaussianBlur(radius, 1.0);

        public void GaussianBlur(double radius, Channels channels)
            => GaussianBlur(radius, 1.0, channels);

        public void GaussianBlur(double radius, double sigma)
            => GaussianBlur(radius, sigma, ImageMagick.Channels.Undefined);

        public void GaussianBlur(double radius, double sigma, Channels channels)
            => SetResult(NativeMagickImage.GaussianBlur(radius, sigma, channels));

        public void HoughLine()
            => HoughLine(0, 0, 40);

        public void HoughLine(uint width, uint height, uint threshold)
            => SetResult(NativeMagickImage.HoughLine(width, height, threshold));

        public void Implode(double amount, PixelInterpolateMethod method)
            => SetResult(NativeMagickImage.Implode(amount, method));

        public void InterpolativeResize(uint width, uint height, PixelInterpolateMethod method)
            => InterpolativeResize(new MagickGeometry(width, height), method);

        public void InterpolativeResize(IMagickGeometry geometry, PixelInterpolateMethod method)
        {
            Throw.IfNull(nameof(geometry), geometry);

            SetResult(NativeMagickImage.InterpolativeResize(geometry.ToString(), method));
        }

        public void InterpolativeResize(Percentage percentage, PixelInterpolateMethod method)
            => InterpolativeResize(new MagickGeometry(percentage, percentage), method);

        public void InterpolativeResize(Percentage percentageWidth, Percentage percentageHeight, PixelInterpolateMethod method)
            => InterpolativeResize(new MagickGeometry(percentageWidth, percentageHeight), method);

        public void Kuwahara()
            => Kuwahara(0.0, 1.0);

        public void Kuwahara(double radius, double sigma)
            => SetResult(NativeMagickImage.Kuwahara(radius, sigma));

        public void LiquidRescale(uint width, uint height)
        => LiquidRescale(new MagickGeometry(width, height));

        public void LiquidRescale(uint width, uint height, double deltaX, double rigidity)
        {
            var geometry = new MagickGeometry(width, height);

            SetResult(NativeMagickImage.LiquidRescale(geometry.ToString(), deltaX, rigidity));
        }

        public void LiquidRescale(IMagickGeometry geometry)
        {
            Throw.IfNull(nameof(geometry), geometry);

            SetResult(NativeMagickImage.LiquidRescale(geometry.ToString(), geometry.X, geometry.Y));
        }

        public void LiquidRescale(Percentage percentage)
            => LiquidRescale(new MagickGeometry(percentage, percentage));

        public void LiquidRescale(Percentage percentageWidth, Percentage percentageHeight)
            => LiquidRescale(new MagickGeometry(percentageWidth, percentageHeight));

        public void LiquidRescale(Percentage percentageWidth, Percentage percentageHeight, double deltaX, double rigidity)
        {
            var geometry = new MagickGeometry(percentageWidth, percentageHeight);

            SetResult(NativeMagickImage.LiquidRescale(geometry.ToString(), deltaX, rigidity));
        }

        public void Magnify()
            => SetResult(NativeMagickImage.Magnify());

        public void MeanShift(uint size)
            => MeanShift(size, size);

        public void MeanShift(uint size, Percentage colorDistance)
            => MeanShift(size, size, colorDistance);

        public void MeanShift(uint width, uint height)
            => MeanShift(width, height, new Percentage(10));

        public void MeanShift(uint width, uint height, Percentage colorDistance)
            => SetResult(NativeMagickImage.MeanShift(width, height, PercentageHelper.ToQuantum(nameof(colorDistance), colorDistance)));

        public void Minify()
            => SetResult(NativeMagickImage.Minify());

        public void Morphology(IMorphologySettings settings)
        {
            Throw.IfNull(nameof(settings), settings);
            Throw.IfTrue(nameof(settings), settings.Iterations < -1, "The number of iterations must be unlimited (-1) or positive");

            using var temporaryDefines = new TemporaryDefines(NativeMagickImage);
            temporaryDefines.SetArtifact("convolve:bias", settings.ConvolveBias);
            temporaryDefines.SetArtifact("convolve:scale", settings.ConvolveScale);

            if (settings.UserKernel is not null && settings.UserKernel.Length > 0)
            {
                SetResult(NativeMagickImage.Morphology(settings.Method, settings.UserKernel, settings.Channels, settings.Iterations));
            }
            else
            {
                var kernel = EnumHelper.GetName(settings.Kernel).ToLowerInvariant() + ":" + settings.KernelArguments;
                SetResult(NativeMagickImage.Morphology(settings.Method, kernel, settings.Channels, settings.Iterations));
            }
        }

        public void MotionBlur(double radius, double sigma, double angle)
            => SetResult(NativeMagickImage.MotionBlur(radius, sigma, angle));

        public void OilPaint()
            => OilPaint(3.0, 1.0);

        public void OilPaint(double radius, double sigma)
            => SetResult(NativeMagickImage.OilPaint(radius, sigma));

        public void Resize(uint width, uint height)
            => Resize(new MagickGeometry(width, height));

        public void Resize(IMagickGeometry geometry)
        {
            Throw.IfNull(nameof(geometry), geometry);

            SetResult(NativeMagickImage.Resize(geometry.ToString()));
        }

        public void Resample(double resolutionX, double resolutionY)
            => SetResult(NativeMagickImage.Resample(resolutionX, resolutionY));

        public void Resample(PointD density)
            => Resample(density.X, density.Y);

        public void Resize(Percentage percentage)
            => Resize(new MagickGeometry(percentage, percentage));

        public void Resize(Percentage percentageWidth, Percentage percentageHeight)
            => Resize(new MagickGeometry(percentageWidth, percentageHeight));

        protected virtual void SetResult(IntPtr result)
        {
            if (_result != IntPtr.Zero)
                throw new InvalidOperationException("Only a single operation can be executed.");

            _result = result;
        }

        private double Deskew(Percentage threshold, bool autoCrop)
        {
            using var temporaryDefines = new TemporaryDefines(NativeMagickImage);
            temporaryDefines.SetArtifact("deskew:auto-crop", autoCrop);

            SetResult(NativeMagickImage.Deskew(PercentageHelper.ToQuantum(nameof(threshold), threshold)));

            var artifact = NativeMagickImage.GetArtifact("deskew:angle");
            if (!double.TryParse(artifact, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return 0.0;

            return result;
        }
    }
}
