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

        public UploadableImage(HttpPostedFileWrapper httpFile)
            : base(httpFile)
        {
            using (var img = Image.FromStream(FileStream))
            {
                Height = img.Height;
                Width = img.Width;
                img.Dispose();
            }
        }

        public bool IsSmallerThan(int size)
        {
            return Width <= size && Height <= size;
        }

        public bool IsSmallerThan(int height, int width)
        {
            return Width <= width && Height <= height;
        }

        public bool IsLargerThan(int size)
        {
            return Width >= size || Height >= size;
        }

        public bool IsLargerThan(int height, int width)
        {
            return Width >= width || Height >= height;
        }

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
