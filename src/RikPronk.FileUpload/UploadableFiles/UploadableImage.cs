using System;
using System.Web;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using RikPronk.FileUpload.Core;

namespace RikPronk.FileUpload
{
    public class UploadableImage : UploadableFile, IUploadableFile
    {
        public int Height { get; }
        public int Width { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadableImage" /> class.
        /// </summary>
        /// <param name="httpFile">The file to upload</param>
        /// <exception cref="NullReferenceException"></exception>
        public UploadableImage(HttpPostedFileBase httpFile)
            : this(httpFile, httpFile.FileName)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadableImage" /> class.
        /// </summary>
        /// <param name="httpFile">The file to upload</param>
        /// <param name="saveName">Name the file will be saved as on disk</param>
        /// <exception cref="NullReferenceException"></exception>
        public UploadableImage(HttpPostedFileBase httpFile, string saveName)
            : base(httpFile, saveName)
        {
            using (var img = Image.FromStream(FileStream))
            {
                Height = img.Height;
                Width = img.Width;
                img.Dispose();
            }
        }

        /// <summary>
        /// Determines whether the image size is smaller than the specified pixel size in either height or width
        /// </summary>
        /// <param name="size">The maximum size of the image in either height or width</param>
        /// <returns></returns>
        public bool IsSmallerThan(int size)
        {
            return Width <= size && Height <= size;
        }

        /// <summary>
        /// Determines whether the image size is smaller than the specified pixel sizes
        /// </summary>
        /// <param name="height">The maximum height of the image</param>
        /// <param name="width">The maximum width of the image</param>
        /// <returns></returns>
        public bool IsSmallerThan(int height, int width)
        {
            return Width <= width && Height <= height;
        }

        /// <summary>
        /// Determines whether the image size is larger than the specified pixel size in either height or width
        /// </summary>
        /// <param name="size">The minimum size of the image in either height or width</param>
        /// <returns></returns>
        public bool IsLargerThan(int size)
        {
            return Width >= size || Height >= size;
        }

        /// <summary>
        /// Determines whether the image size is larger than the specified pixel sizes
        /// </summary>
        /// <param name="height">The minimum height of the image</param>
        /// <param name="width">The minimum width of the image</param>
        /// <returns></returns>
        public bool IsLargerThan(int height, int width)
        {
            return Width >= width || Height >= height;
        }

        /// <summary>
        /// Scales the image to the specified size in either width or height, depending on which is closer.
        /// </summary>
        /// <param name="size">The size to scale towards</param>
        /// <returns></returns>
        public Stream ScaleImage(int size)
        {
            var rawStream = FileStream;
            using (rawStream)
            {
                rawStream.Position = 0;

                // Create bitmap decoder
                var decoder = BitmapDecoder.Create(
                rawStream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.None);

                // Decode single bitmap frame
                var frame = decoder.Frames[0];
                
                Double xRatio = frame.Width / size;
                Double yRatio = frame.Height / size;
                Double ratio = Math.Max(xRatio, yRatio);
                int nnx = (int)Math.Floor(frame.Width / ratio);
                int nny = (int)Math.Floor(frame.Height / ratio);

                // Resize the bitmap frame
                var resizedFrame = new TransformedBitmap(
                    frame,
                    new ScaleTransform(
                        nnx / frame.Width * 96 / frame.DpiX,
                        nny / frame.Height * 96 / frame.DpiY,
                        0, 0));

                // Re-encode the bitmap to stream
                var stream = new MemoryStream();
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(resizedFrame));
                encoder.Save(stream);

                stream.Position = 0;
                rawStream.Dispose();
                return stream;
            }
        }

        /// <summary>
        /// Scales the image to either the specified width or height, depending on which is closer.
        /// </summary>
        /// <param name="height">The height to scale towards</param>
        /// <param name="width">The width to scale towards</param>
        /// <returns></returns>
        public Stream ScaleImage(int height, int width)
        {
            var rawStream = FileStream;
            using (rawStream)
            {
                rawStream.Position = 0;

                // Create bitmap decoder
                var decoder = BitmapDecoder.Create(
                rawStream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.None);

                // Decode single bitmap frame
                var frame = decoder.Frames[0];

                double xRatio = frame.Width / width;
                Double yRatio = frame.Height / height;
                Double ratio = Math.Max(xRatio, yRatio);
                int nnx = (int)Math.Floor(frame.Width / ratio);
                int nny = (int)Math.Floor(frame.Height / ratio);

                // Resize the bitmap frame
                var resizedFrame = new TransformedBitmap(
                    frame,
                    new ScaleTransform(
                        nnx / frame.Width * 96 / frame.DpiX,
                        nny / frame.Height * 96 / frame.DpiY,
                        0, 0));

                // Re-encode the bitmap to stream
                var stream = new MemoryStream();
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(resizedFrame));
                encoder.Save(stream);

                stream.Position = 0;
                return stream;
            }
        }
    }
}
