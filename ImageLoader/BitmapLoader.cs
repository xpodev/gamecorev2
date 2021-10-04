using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GameCore.ImageLoader
{
    public class BitmapLoader
    {
        struct FileHeader
        {
            public const int StructSize = 14;

            private string identifier;
            private int fileSize;
            //unused and therefore not necessary
            //private short reserved1;
            //private short reserved2;
            private int dataOffset;

            public string Identifer => identifier;

            public int FileSize => fileSize;

            public int DataOffset => dataOffset;

            public static FileHeader ReadFrom(BinaryReader stream)
            {
                string id = string.Empty;
                int fileSize, dataOffset;
                id += stream.ReadByte();
                id += stream.ReadByte();
                fileSize = stream.ReadInt32();
                // short form of stream.ReadInt16() twice because we don't use these values
                stream.ReadInt32();
                dataOffset = stream.ReadInt32();
                return new FileHeader()
                {
                    identifier = id,
                    fileSize = fileSize,
                    dataOffset = dataOffset
                };
            }
        }

        struct BitmapHeader
        {
            public interface IBitmapHeader
            {
                int Size { get; set; }

                int Width { get; }

                int Height { get; }

                int ColorPlanesCount { get; }

                int BitsPerPixel { get; }
            }

            public struct BitmapInfoHeader : IBitmapHeader
            {
                public enum BitmapCompressionMethod
                {
                    RGB,
                    RLE8,
                    RLE4,
                    BitFields,
                    JPEG,
                    PNG,
                    AlphaBitFields,
                    CMYK,
                    CMYKRLE8,
                    CMYKRLE4
                }

                public int Size { get; set; }

                public int Width { get; private set; }
                public int Height { get; private set; }

                public int ColorPlanesCount { get; private set; }

                public int BitsPerPixel { get; private set; }

                public BitmapCompressionMethod CompressionMethod { get; set; }

                public int DataLength { get; set; }

                public int ResolutionX { get; set; }
                public int ResolutionY { get; set; }

                public int ColorsCount { get; set; }
                public int ImportantColorsCount { get; set; }

                public static BitmapInfoHeader ReadFrom(BinaryReader stream)
                {
                    BitmapInfoHeader header = new BitmapInfoHeader()
                    {
                        // size is assigned outside
                        Size = 0,
                        Width = stream.ReadInt32(),
                        Height = stream.ReadInt32(),
                        ColorPlanesCount = stream.ReadInt16(),
                        BitsPerPixel = stream.ReadInt16(),
                        CompressionMethod = (BitmapCompressionMethod)stream.ReadInt32(),
                        DataLength = stream.ReadInt32(),
                        ResolutionX = stream.ReadInt32(),
                        ResolutionY = stream.ReadInt32(),
                        ColorsCount = stream.ReadInt32(),
                        ImportantColorsCount = stream.ReadInt32()
                    };
                    header.ColorsCount = header.ColorsCount == 0 ? 1 << header.BitsPerPixel : header.ColorsCount;
                    header.ImportantColorsCount = header.ImportantColorsCount == 0 ? header.ColorsCount : header.ImportantColorsCount;
                    return header;
                }
            }
            
            public static IBitmapHeader ReadFrom(BinaryReader stream)
            {
                int size = stream.ReadInt32();
                IBitmapHeader header;
                switch (size)
                {
                    case 40:
                        header = BitmapInfoHeader.ReadFrom(stream);
                        break;
                    default:
                        throw new Exception($"Invalid header size: {size}");
                }
                header.Size = size;
                return header;
            }
        }

        public static Texture2D LoadImage(string path)
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(path));
            FileHeader fileHeader = FileHeader.ReadFrom(reader);
            BitmapHeader.IBitmapHeader bitmapHeader = BitmapHeader.ReadFrom(reader);
            reader.BaseStream.Seek(fileHeader.DataOffset, SeekOrigin.Begin);
            Texture2D texture2D = new Texture2D(bitmapHeader.Width, bitmapHeader.Height, TextureFormat.RGBA32, true);
            if (bitmapHeader is BitmapHeader.BitmapInfoHeader header)
            {
                DecompressRLE4BitmapData(fileHeader, header, reader, texture2D);
            }
            //reader.BaseStream.Read(texture2D.GetRawTextureData<byte>().ToArray(), 0, bitmapHeader.Width * bitmapHeader.Height * bitmapHeader.BitsPerPixel / 8);
            return texture2D;
        }

        public static void SaveImage(Texture2D texture, string path)
        {
            throw new NotImplementedException();
        }

        private static void DecompressBitmapData(FileHeader file, BitmapHeader.BitmapInfoHeader header, BinaryReader stream, Texture2D texture)
        {
            switch (header.CompressionMethod)
            {
                case BitmapHeader.BitmapInfoHeader.BitmapCompressionMethod.RGB:
                    break;
                case BitmapHeader.BitmapInfoHeader.BitmapCompressionMethod.RLE8:
                    break;
                case BitmapHeader.BitmapInfoHeader.BitmapCompressionMethod.RLE4:
                    DecompressRLE4BitmapData(file, header, stream, texture);
                    break;
                case BitmapHeader.BitmapInfoHeader.BitmapCompressionMethod.BitFields:
                    break;
                case BitmapHeader.BitmapInfoHeader.BitmapCompressionMethod.JPEG:
                    break;
                case BitmapHeader.BitmapInfoHeader.BitmapCompressionMethod.PNG:
                    break;
                case BitmapHeader.BitmapInfoHeader.BitmapCompressionMethod.AlphaBitFields:
                    break;
                case BitmapHeader.BitmapInfoHeader.BitmapCompressionMethod.CMYK:
                    break;
                case BitmapHeader.BitmapInfoHeader.BitmapCompressionMethod.CMYKRLE8:
                    break;
                case BitmapHeader.BitmapInfoHeader.BitmapCompressionMethod.CMYKRLE4:
                    break;
                default:
                    break;
            }
        }

        private static void DecompressRLE4BitmapData(FileHeader file, BitmapHeader.BitmapInfoHeader header, BinaryReader stream, Texture2D texture)
        {
            Color32[] colorTable = new Color32[header.ColorsCount];
            stream.BaseStream.Seek(FileHeader.StructSize + header.Size, SeekOrigin.Begin);
            for (int i = 0; i < colorTable.Length; i++)
            {
                byte r, g, b;
                b = stream.ReadByte();
                g = stream.ReadByte();
                r = stream.ReadByte();
                colorTable[i] = new Color32(r, g, b, 255);
                stream.ReadByte(); // should be 0
            }
            stream.BaseStream.Seek(file.DataOffset, SeekOrigin.Begin);
            Color32[] pixels = new Color32[header.Width * header.Height];
            for (int i = 0; i < header.DataLength; i += 2)
            {
                byte v = stream.ReadByte();
                pixels[i] = colorTable[v & 0x0F];
                pixels[i + 1] = colorTable[v >> 4];
            }
            texture.SetPixels32(pixels);
        }
    }
}
