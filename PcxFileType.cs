/////////////////////////////////////////////////////////////////////////////////
// PCX Plugin for Paint.NET
// Copyright (C) 2006 Joshua Bell (inexorabletash@gmail.com)
// Portions Copyright (C) 2006 Rick Brewster, et. al.
// See License.txt for complete licensing and attribution information.
/////////////////////////////////////////////////////////////////////////////////

// References: 
//  * PCX File Format - http://courses.ece.uiuc.edu/ece390/books/labmanual/graphics-pcx.html
//  * PCX File Format - http://www.fileformat.info/format/pcx/
//  * EGA Color Palette - http://wasteland.wikispaces.com/EGA+Colour+Palette

// Test images:
//  * 1, 2, 8, 24: http://www.efg2.com/Lab/Library/Delphi/Graphics/FileFormatsAndConversion.htm (PCX.ZIP)
//  * 8, 24: http://www.fileformat.info/format/pcx/sample/index.htm
//  * 1: http://memory.loc.gov/ammem/help/view.html

// NOTE: Only supports saving 256-color RLE encoded images


using PaintDotNet;
using PaintDotNet.IO;
using PcxFileTypePlugin.Quantize;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;


namespace PcxFileTypePlugin
{
    #region Paint.NET Registration
    // Register our PcxFileType object
    public class PcxFileTypes : IFileTypeFactory
    {
        public static readonly FileType Pcx = new PcxFileType();
        private static FileType[] fileTypes = new FileType[] { Pcx };

        public FileType[] GetFileTypeInstances()
        {
            return (FileType[])fileTypes.Clone();
        }
    }
    #endregion

    [Guid("D22EBDD9-0A86-4e00-957B-550010B82C09")]
    public class PcxFileType : FileType
    {
        #region Paint.NET File Format API

        public PcxFileType() : base(
            "PCX",
            FileTypeFlags.SupportsSaving | FileTypeFlags.SupportsLoading,
            new string[] { ".pcx" })    // has extension of '.pcx'
        {
        }

        public override SaveConfigWidget CreateSaveConfigWidget()
        {
            return new PcxSaveConfigWidget();
        }

        protected override SaveConfigToken OnCreateDefaultSaveConfigToken()
        {
            return new PcxSaveConfigToken(0, false, false, 8, true);
        }

        #endregion

        #region File Structure, Header and Constants

        ////////////////////////////////////////////////////////////
        // PCX File Structure
        //
        //    Header        128 bytes
        //
        //    Pixel Data    scan0 plane0
        //                  scan0 plane1
        //                  ..
        //                  scan0 planeN
        //                  scan1 plane0
        //                  scan1 plane1
        //                  ..
        //                  scan1 planeN
        //                  ...
        //                  scanM planeN
        //
        //    Palette       0x0C         
        //    (8-bit only)  r0,g0,b0
        //                  r1,g1,b1
        //                  ...
        //                  r256,g256,b256
        ////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        // struct PCXHeader 
        // {
        //     BYTE Manufacturer;  // Constant Flag   10 = ZSoft .PCX
        //     BYTE Version;       // Version Information
        //                         // 0 = Version 2.5
        //                         // 2 = Version 2.8 w/palette information
        //                         // 3 = Version 2.8 w/o palette information
        //                         // 4 = (PC Paintbrush for Windows)
        //                         // 5 = Version 3.0
        //     BYTE Encoding;      // 1 = .PCX run length encoding
        //     BYTE BitsPerPixel;  // Number of bits/pixel per plane (1, 2, 4 or 8)
        //     WORD XMin;          // Picture Dimensions 
        //     WORD YMin;          // (Xmin, Ymin) - (Xmax - Ymax) inclusive
        //     WORD XMax;
        //     WORD YMax;
        //     WORD HDpi;          // Horizontal Resolution of creating device
        //     WORD VDpi;          // Vertical Resolution of creating device
        //     BYTE ColorMap[48];  // Color palette for 16-color palette
        //     BYTE Reserved;
        //     BYTE NPlanes;       // Number of color planes
        //     WORD BytesPerLine;  // Number of bytes per scan line per color plane (always even for .PCX files)
        //     WORD PaletteInfo;   // How to interpret palette - 1 = color/BW, 2 = grayscale
        //     BYTE Filler[58];
        // };
        ////////////////////////////////////////////////////////////

        private enum PcxId : byte
        {
            ZSoftPCX = 10
        };

        private enum PcxVersion : byte
        {
            Version2_5 = 0,
            Version2_8_Palette = 2,
            Version2_8_DefaultPalette = 3,
            Version3_0 = 5
        };

        private enum PcxEncoding : byte
        {
            None = 0,
            RunLengthEncoded = 1
        };

        private enum PcxPaletteType : byte
        {
            Indexed = 1,
            Grayscale = 2
        };

        private const int PcxRleMask = 0xC0;
        private const int PcxPaletteMarker = 0x0C;


        private class PcxHeader
        {
            public PcxId id = PcxId.ZSoftPCX;
            public PcxVersion version = PcxVersion.Version3_0;
            public PcxEncoding encoding = PcxEncoding.RunLengthEncoded;
            public byte bitsPerPixel;
            public ushort xMin;
            public ushort yMin;
            public ushort xMax;
            public ushort yMax;
            public ushort hDpi;
            public ushort vDpi;
            public byte[] colorMap = new byte[48];
            public byte reserved = 0;
            public byte nPlanes;
            public ushort bytesPerLine;
            public PcxPaletteType paletteInfo;
            public byte[] filler = new byte[58];

            public void Write(Stream output)
            {
                output.WriteByte((byte)id);
                output.WriteByte((byte)version);
                output.WriteByte((byte)encoding);
                output.WriteByte((byte)bitsPerPixel);
                StreamExtensions.WriteUInt16(output, xMin);
                StreamExtensions.WriteUInt16(output, yMin);
                StreamExtensions.WriteUInt16(output, xMax);
                StreamExtensions.WriteUInt16(output, yMax);
                StreamExtensions.WriteUInt16(output, hDpi);
                StreamExtensions.WriteUInt16(output, vDpi);
                for (int i = 0; i < colorMap.Length; i++)
                {
                    output.WriteByte(colorMap[i]);
                }
                output.WriteByte(reserved);
                output.WriteByte(nPlanes);
                StreamExtensions.WriteUInt16(output, bytesPerLine);
                StreamExtensions.WriteUInt16(output, (ushort)PcxPaletteType.Indexed);
                for (int i = 0; i < filler.Length; i++)
                {
                    output.WriteByte(filler[i]);
                }

            }

            public PcxHeader()
            {
            }

            public PcxHeader(Stream input)
            {
                id = (PcxId)ReadByte(input);
                version = (PcxVersion)ReadByte(input);
                encoding = (PcxEncoding)ReadByte(input);
                bitsPerPixel = ReadByte(input);
                xMin = ReadUInt16(input);
                yMin = ReadUInt16(input);
                xMax = ReadUInt16(input);
                yMax = ReadUInt16(input);
                hDpi = ReadUInt16(input);
                vDpi = ReadUInt16(input);
                for (int i = 0; i < colorMap.Length; i++)
                {
                    colorMap[i] = ReadByte(input);
                }
                reserved = ReadByte(input);
                nPlanes = ReadByte(input);
                bytesPerLine = ReadUInt16(input);
                paletteInfo = (PcxPaletteType)ReadUInt16(input);
                for (int i = 0; i < filler.Length; i++)
                {
                    filler[i] = ReadByte(input);
                }
            }

            private byte ReadByte(Stream input)
            {
                int byteRead = input.ReadByte();
                if (byteRead == -1)
                {
                    throw new EndOfStreamException();
                }
                else
                {
                    return (byte)byteRead;
                }
            }

            private ushort ReadUInt16(Stream input)
            {
                int shortRead = StreamExtensions.ReadUInt16(input);
                if (shortRead == -1)
                {
                    throw new EndOfStreamException();
                }
                else
                {
                    return (ushort)shortRead;
                }
            }
        }

        #endregion

        #region Palette

        /**
		 * PcxPalette 
		 * 
		 * Always either 16 or 256 entries
		 */
        private class PcxPalette
        {
            public static readonly uint[] MONO_PALETTE = new uint[] { 0x000000, 0xFFFFFF, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000 };
            public static readonly uint[] CGA_PALETTE = new uint[] { 0x000000, 0x00AAAA, 0xAA00AA, 0xAAAAAA, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000 };
            public static readonly uint[] EGA_PALETTE = new uint[] { 0x000000, 0x0000A8, 0x00A800, 0x00A8A8, 0xA80000, 0xA800A8, 0xA85400, 0xA8A8A8, 0x545454, 0x5454FE, 0x54FE54, 0x54FEFE, 0xFE5454, 0xFE54FE, 0xFEFE54, 0xFEFEFE };

            private ColorBgra[] m_palette;

            public uint Size
            {
                get { return (uint)m_palette.Length; }
            }

            public ColorBgra this[uint index]
            {
                get { return m_palette[index]; }
                set { m_palette[index] = value; }
            }

            public PcxPalette(uint size)
            {
                if (size != 16 && size != 256)
                {
                    throw new FormatException("Unsupported palette size");
                }

                m_palette = new ColorBgra[size];
            }

            public PcxPalette(ColorPalette palette)
            {
                Color[] entries = palette.Entries;

                uint size;
                if (entries.Length <= 16)
                {
                    size = 16;
                }
                else if (entries.Length <= 256)
                {
                    size = 256;
                }
                else
                {
                    throw new FormatException("Unsupported palette size");
                }

                m_palette = new ColorBgra[size];

                for (uint i = 0; i < entries.Length; i++)
                {
                    m_palette[i] = ColorBgra.FromColor(entries[i]);
                }

                // Fill rest of the palette with black
                for (uint i = size - 1; i >= entries.Length; i--)
                {
                    m_palette[i] = ColorBgra.Black;
                }
            }

            public PcxPalette(Stream input, int size)
            {
                if (size != 16 && size != 256)
                {
                    throw new FormatException("Unsupported palette size");
                }

                m_palette = new ColorBgra[size];

                for (int i = 0; i < m_palette.Length; ++i)
                {
                    int red = input.ReadByte();
                    if (red == -1)
                    {
                        throw new EndOfStreamException();
                    }

                    int green = input.ReadByte();
                    if (green == -1)
                    {
                        throw new EndOfStreamException();
                    }

                    int blue = input.ReadByte();
                    if (blue == -1)
                    {
                        throw new EndOfStreamException();
                    }

                    m_palette[i] = ColorBgra.FromBgra((byte)blue, (byte)green, (byte)red, 255);
                }
            }

            public void Write(Stream output)
            {
                for (int i = 0; i < m_palette.Length; i++)
                {
                    ColorBgra c = m_palette[i];
                    output.WriteByte(c.R);
                    output.WriteByte(c.G);
                    output.WriteByte(c.B);
                }
            }

            public byte[] ToColorMap()
            {
                if (m_palette.Length != 16)
                {
                    throw new FormatException("Trying to write an unsupported palette size to a header ColorMap");
                }

                byte[] colorMap = new byte[48];
                uint index = 0;
                for (int i = 0; i < 16; i++)
                {
                    ColorBgra c = m_palette[i];

                    colorMap[index++] = c.R;
                    colorMap[index++] = c.G;
                    colorMap[index++] = c.B;
                }

                return colorMap;
            }

            public static PcxPalette FromColorMap(byte[] colorMap)
            {
                if (colorMap == null)
                {
                    throw new ArgumentNullException("colorMap");
                }
                if (colorMap.Length != 48)
                {
                    throw new FormatException("Trying to read an unsupported palette size from a header ColorMap");
                }

                PcxPalette palette = new PcxPalette(16);

                uint index = 0;
                for (uint i = 0; i < 16; i++)
                {
                    byte r = colorMap[index++];
                    byte g = colorMap[index++];
                    byte b = colorMap[index++];

                    palette[i] = ColorBgra.FromBgr(b, g, r);
                }

                return palette;
            }

            public static PcxPalette FromEgaPalette(uint[] egaPalette)
            {
                if (egaPalette == null)
                {
                    throw new ArgumentNullException("egaPalette");
                }
                if (egaPalette.Length != 16)
                {
                    throw new FormatException("Trying to read an unsupported palette size from a header ColorMap");
                }

                PcxPalette palette = new PcxPalette(16);

                for (uint i = 0; i < 16; i++)
                {
                    byte r = (byte)((egaPalette[i] >> 16) & 0xff);
                    byte g = (byte)((egaPalette[i] >> 8) & 0xff);
                    byte b = (byte)((egaPalette[i]) & 0xff);

                    palette[i] = ColorBgra.FromBgr(b, g, r);
                }

                return palette;
            }

            public string Serialize()
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (ColorBgra colorBgra in m_palette)
                {
                    if (sb.Length > 0)
                        sb.Append(",");

                    Color c = colorBgra.ToColor();
                    uint argb = (uint)c.ToArgb();
                    string s = argb.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    sb.Append(s);
                }

                return sb.ToString();
            }

            public static List<Color> Deserialize(string s)
            {
                List<Color> colors = new List<Color>();

                string[] ss = s.Split(new char[] { ',' });
                foreach (string item in ss)
                {
                    uint argb = uint.Parse(item, System.Globalization.CultureInfo.InvariantCulture);
                    Color color = Color.FromArgb((int)argb);
                    colors.Add(color);
                }

                return colors;
            }
        }

        #endregion


        protected override Document OnLoad(Stream input)
        {
            //
            // Load and validate header
            //
            PcxHeader header = new PcxHeader(input);

            if (header.id != PcxId.ZSoftPCX)
            {
                throw new FormatException("Not a PCX file.");
            }

            if (header.version != PcxVersion.Version3_0 &&
                header.version != PcxVersion.Version2_8_Palette &&
                header.version != PcxVersion.Version2_8_DefaultPalette &&
                header.version != PcxVersion.Version2_5)
            {
                throw new FormatException(String.Format("Unsupported PCX version: {0}", header.version));
            }

            if (header.bitsPerPixel != 1 &&
                header.bitsPerPixel != 2 &&
                header.bitsPerPixel != 4 &&
                header.bitsPerPixel != 8)
            {
                throw new FormatException(String.Format("Unsupported PCX bits per pixel: {0} bits per pixel", header.bitsPerPixel));
            }

            int width = header.xMax - header.xMin + 1;
            int height = header.yMax - header.yMin + 1;
            if (width < 0 || height < 0 || width > 0xffff || height > 0xffff)
            {
                throw new FormatException(String.Format("Invalid image dimensions: ({0},{1})-({2},{3})", header.xMin, header.yMin, header.xMax, header.yMax));
            }

            // Pixels per line, including PCX's even-number-of-pixels buffer
            int pixelsPerLine = header.bytesPerLine * 8 /*bitsPerByte*/ / header.bitsPerPixel;

            // Bits per pixel, including all bit planes
            int bitsPerPixel = header.bitsPerPixel * header.nPlanes;

            if (bitsPerPixel != 1 &&
                bitsPerPixel != 2 &&
                bitsPerPixel != 4 &&
                bitsPerPixel != 8 &&
                bitsPerPixel != 24)
            {
                throw new FormatException(String.Format("Unsupported PCX bit depth: {0}", bitsPerPixel));
            }


            //
            // Load the palette
            //
            PcxPalette palette;

            if (bitsPerPixel == 1)
            {
                // HACK: Monochrome images don't always include a resonable palette in v3.0.
                // Default them to black and white in all cases

                palette = PcxPalette.FromEgaPalette(PcxPalette.MONO_PALETTE);
            }
            else if (bitsPerPixel < 8)
            {
                // 16-color palette in the ColorMap portion of the header

                switch (header.version)
                {
                    case PcxVersion.Version2_5:
                    case PcxVersion.Version2_8_DefaultPalette:
                        {
                            uint[] paletteEga;

                            switch (bitsPerPixel)
                            {
                                // 4-color CGA palette 
                                case 2:
                                    paletteEga = PcxPalette.CGA_PALETTE;
                                    break;

                                // 16-color EGA palette
                                default:
                                case 4:
                                    paletteEga = PcxPalette.EGA_PALETTE;
                                    break;
                            }

                            palette = PcxPalette.FromEgaPalette(paletteEga);
                            break;
                        }

                    default:
                    case PcxVersion.Version2_8_Palette:
                    case PcxVersion.Version3_0:
                        {
                            palette = PcxPalette.FromColorMap(header.colorMap);
                            break;
                        }

                }
            }
            else if (bitsPerPixel == 8)
            {
                // 256-color palette is saved at the end of the file, with one byte marker

                long dataPosition = input.Position;
                input.Seek(-(1 + (256 * 3)), SeekOrigin.End);

                if (input.ReadByte() != PcxPaletteMarker)
                {
                    throw new FormatException("PCX palette marker not present in file");
                }

                palette = new PcxPalette(input, 256);

                input.Seek(dataPosition, SeekOrigin.Begin);
            }
            else
            {
                // Dummy palette for 24-bit images

                palette = new PcxPalette(256);
            }


            //
            // Load the pixel data
            //
            BitmapLayer layer = Layer.CreateBackgroundLayer(width, height);

            try
            {
                Surface surface = layer.Surface;
                surface.Clear((ColorBgra)0xffffffff);

                // Accumulate indices across bit planes
                uint[] indexBuffer = new uint[width];

                for (int y = 0; y < height; y++)
                {
                    MemoryBlock dstRow = surface.GetRow(y);
                    Array.Clear(indexBuffer, 0, width);

                    int offset = 0;

                    // Decode the RLE byte stream
                    PcxByteReader byteReader = (header.encoding == PcxEncoding.RunLengthEncoded)
                        ? (PcxByteReader)(new PcxRleByteReader(input))
                        : (PcxByteReader)(new PcxRawByteReader(input));

                    // Read indices of a given length out of the byte stream
                    PcxIndexReader indexReader = new PcxIndexReader(byteReader, header.bitsPerPixel);

                    // Planes are stored consecutively for each scan line
                    for (int plane = 0; plane < header.nPlanes; plane++)
                    {
                        for (int x = 0; x < pixelsPerLine; x++)
                        {
                            uint index = indexReader.ReadIndex();

                            // Account for padding bytes
                            if (x < width)
                            {
                                indexBuffer[x] = indexBuffer[x] | (index << (plane * header.bitsPerPixel));
                            }
                        }
                    }

                    for (int x = 0; x < width; x++)
                    {
                        uint index = indexBuffer[x];
                        ColorBgra color;

                        if (bitsPerPixel == 24)
                        {
                            byte r = (byte)((index) & 0xff);
                            byte g = (byte)((index >> 8) & 0xff);
                            byte b = (byte)((index >> 16) & 0xff);

                            color = ColorBgra.FromBgr(b, g, r);
                        }
                        else
                        {
                            color = palette[index];
                        }

                        dstRow[offset + 0] = color[0];
                        dstRow[offset + 1] = color[1];
                        dstRow[offset + 2] = color[2];
                        dstRow[offset + 3] = color[3];

                        offset += ColorBgra.SizeOf;
                    }
                }

                Document document = new Document(surface.Width, surface.Height);
                document.Layers.Add(layer);
                document.Metadata.SetUserValue("Palette", palette.Serialize());

                return document;
            }
            catch
            {
                if (layer != null)
                {
                    layer.Dispose();
                    layer = null;
                }

                throw;
            }
        }


        protected override void OnSave(Document input, Stream output, SaveConfigToken token, Surface scratchSurface, ProgressEventHandler callback)
        {
            PcxSaveConfigToken pcxToken = (PcxSaveConfigToken)token;

            scratchSurface.Clear(ColorBgra.FromBgra(255, 255, 255, 0));

            using (RenderArgs ra = new RenderArgs(scratchSurface))
            {
                input.Render(ra, true);
            }

            for (int y = 0; y < scratchSurface.Height; ++y)
            {
                unsafe
                {
                    ColorBgra* ptr = scratchSurface.GetRowAddressUnchecked(y);

                    for (int x = 0; x < scratchSurface.Width; ++x)
                    {
                        if (ptr->A < pcxToken.Threshold)
                        {
                            ptr->Bgra = 0;
                        }
                        else
                        {
                            if (pcxToken.PreMultiplyAlpha)
                            {
                                int r = ((ptr->R * ptr->A) + (255 * (255 - ptr->A))) / 255;
                                int g = ((ptr->G * ptr->A) + (255 * (255 - ptr->A))) / 255;
                                int b = ((ptr->B * ptr->A) + (255 * (255 - ptr->A))) / 255;
                                int a = 255;

                                *ptr = ColorBgra.FromBgra((byte)b, (byte)g, (byte)r, (byte)a);
                            }
                            else
                            {
                                ptr->Bgra |= 0xff000000;
                            }
                        }

                        ++ptr;
                    }
                }
            }

            // TODO: Use FileType.Quantize() for new palette cases
            using (Bitmap bitmap = scratchSurface.CreateAliasedBitmap(input.Bounds, true))
            {
                Quantizer quantizer;

                string serializedPalette = input.Metadata.GetUserValue("Palette");
                if (pcxToken.UseOriginalPalette && serializedPalette != null)
                {
                    List<Color> palette = PcxPalette.Deserialize(serializedPalette);
                    quantizer = new PaletteQuantizer(palette);
                }
                else
                {
                    quantizer = new OctreeQuantizer(255, 8);
                }

                quantizer.DitherLevel = pcxToken.DitherLevel;

                using (Bitmap quantized = quantizer.Quantize(bitmap, callback))
                {
                    SavePcx(quantized, output, pcxToken, callback);
                }
            }
        }


        private void SavePcx(Bitmap bitmap, Stream output, PcxSaveConfigToken token, ProgressEventHandler progressCallback)
        {
            // 
            // NOTE: Only supports 16 or 256 color PCX files (single plane)
            //

            // Determine palette size
            if (bitmap.Palette.Entries.Length == 0 || bitmap.Palette.Entries.Length > 256)
            {
                throw new FormatException("Unsupported palette size");
            }
            PcxPalette palette = new PcxPalette(bitmap.Palette);

            //
            // Write header
            //
            PcxHeader header = new PcxHeader();
            header.version = PcxVersion.Version3_0;
            header.encoding = token.RleCompress ? PcxEncoding.RunLengthEncoded : PcxEncoding.None;
            header.bitsPerPixel =
                palette.Size == 16 ? (byte)4 :
                palette.Size == 256 ? (byte)8 : (byte)0;
            header.xMin = 0;
            header.yMin = 0;
            header.xMax = (ushort)(bitmap.Width - 1); // inclusive
            header.yMax = (ushort)(bitmap.Height - 1); // inclusive
            header.hDpi = (ushort)bitmap.HorizontalResolution;
            header.vDpi = (ushort)bitmap.VerticalResolution;
            header.nPlanes = 1;
            header.bytesPerLine = (ushort)(bitmap.Width + ((bitmap.Width % 2 == 1) ? 1 : 0)); // Bytes per scan line - must be even
            header.paletteInfo = PcxPaletteType.Indexed;

            if (palette.Size == 16)
            {
                header.colorMap = palette.ToColorMap();
            }

            header.Write(output);

            int pixelsPerLine = header.bytesPerLine * 8 /*bitsPerByte*/ / header.bitsPerPixel;

            //
            // Write pixel data
            //
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format8bppIndexed);

            try
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    byte[] row = new byte[bitmap.Width];

                    unsafe
                    {
                        byte* pData = (byte*)bitmapData.Scan0.ToPointer() + (y * bitmapData.Stride);
                        System.Runtime.InteropServices.Marshal.Copy(new IntPtr(pData), row, 0, bitmap.Width);
                    }

                    PcxByteWriter byteWriter = (header.encoding == PcxEncoding.RunLengthEncoded)
                        ? (PcxByteWriter)(new PcxRleByteWriter(output))
                        : (PcxByteWriter)(new PcxRawByteWriter(output));

                    PcxIndexWriter indexWriter = new PcxIndexWriter(byteWriter, header.bitsPerPixel);

                    for (int x = 0; x < pixelsPerLine; x++)
                    {
                        if (x < bitmap.Width)
                        {
                            indexWriter.WriteIndex(row[x]);
                        }
                        else
                        {
                            indexWriter.WriteIndex(0); // Padding until to produce the required number of bytes
                        }
                    }

                    indexWriter.Flush(); // Write out any buffered bits

                    byteWriter.Flush(); // Force RLE encoding reset

                    if (progressCallback != null)
                    {
                        progressCallback(this, new ProgressEventArgs(100.0 * ((double)(bitmap.Height - y) / (double)bitmap.Height)));
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            //
            // Write palette
            //
            if (palette.Size == 256)
            {
                output.WriteByte(PcxPaletteMarker);
                palette.Write(output);
            }
        }


        #region Index Reader/Writer
        /**
		 * Classes to handle reading/writing indices of various bit depths to/from encoded streams.
		 * 
		 */

        private class PcxIndexReader
        {
            private PcxByteReader m_reader;
            private uint m_bitsPerPixel;
            private uint m_bitMask;

            private uint m_bitsRemaining = 0;
            private uint m_byteRead;

            public PcxIndexReader(PcxByteReader reader, uint bitsPerPixel)
            {
                if (!(bitsPerPixel == 1 || bitsPerPixel == 2 || bitsPerPixel == 4 || bitsPerPixel == 8))
                {
                    throw new ArgumentException("bitsPerPixel must be 1, 2, 4 or 8", "bitsPerPixel");
                }

                m_reader = reader;
                m_bitsPerPixel = bitsPerPixel;

                // Compute bit mask
                m_bitMask = 1;
                for (uint i = m_bitsPerPixel; i > 0; i--)
                {
                    m_bitMask = m_bitMask << 1;
                }
                m_bitMask--;
            }

            public uint ReadIndex()
            {
                // NOTE: This does not work for non-power-of-two bits per pixel (e.g. 6)
                // since it does not concatenate shift adjacent bytes together

                if (m_bitsRemaining == 0)
                {
                    m_byteRead = m_reader.ReadByte();
                    m_bitsRemaining = 8;
                }

                // NOTE: Reads from the most significant bits
                uint index = (m_byteRead >> (int)(8 - m_bitsPerPixel)) & m_bitMask;
                m_byteRead = m_byteRead << (int)m_bitsPerPixel;
                m_bitsRemaining -= m_bitsPerPixel;

                return index;
            }
        }

        private class PcxIndexWriter
        {
            private PcxByteWriter m_writer;
            private uint m_bitsPerPixel;
            private uint m_bitMask;

            private uint m_bitsUsed = 0;
            private uint m_byteAccumulated;

            public PcxIndexWriter(PcxByteWriter writer, uint bitsPerPixel)
            {
                if (!(bitsPerPixel == 1 || bitsPerPixel == 2 || bitsPerPixel == 4 || bitsPerPixel == 8))
                {
                    throw new ArgumentException("bitsPerPixel must be 1, 2, 4 or 8", "bitsPerPixel");
                }

                m_writer = writer;
                m_bitsPerPixel = bitsPerPixel;

                // Compute bit mask
                m_bitMask = 1;
                for (uint i = m_bitsPerPixel; i > 0; i--)
                {
                    m_bitMask = m_bitMask << 1;
                }
                m_bitMask--;
            }

            public void WriteIndex(uint index)
            {
                // NOTE: This does not work for non-power-of-two bits per pixel (e.g. 6)
                // since it does not concatenate shift adjacent bytes together

                // NOTE: Are bits shifted onto the right end? Especially at the right edge of the picture?
                m_byteAccumulated = (m_byteAccumulated << (int)m_bitsPerPixel) | (index & m_bitMask);
                m_bitsUsed += m_bitsPerPixel;

                if (m_bitsUsed == 8)
                {
                    Flush();
                }
            }

            public void Flush()
            {
                // TODO: If this is called when not all bytes are used, are the bits on the right end?

                // BUG: If there's nothing to flush, this will execute twice!
                // Contrariwise, this will never call the RLE flush!

                if (m_bitsUsed > 0)
                {
                    m_writer.WriteByte((byte)m_byteAccumulated);
                    m_bitsUsed = 0;
                }
            }
        }
        #endregion

        #region RLE Encoding/Decoding

        /**
		 * Classes to handle RLE encoding/decoding to/from streams.
		 * 
		 */

        private abstract class PcxByteReader
        {
            public abstract byte ReadByte();
        }

        private abstract class PcxByteWriter
        {
            public abstract void WriteByte(byte value);
            public abstract void Flush();
        }

        private class PcxRawByteReader : PcxByteReader
        {
            private Stream m_stream;
            public PcxRawByteReader(Stream stream)
            {
                m_stream = stream;
            }
            public override byte ReadByte()
            {
                return (byte)m_stream.ReadByte();
            }
        }

        private class PcxRawByteWriter : PcxByteWriter
        {
            private Stream m_stream;
            public PcxRawByteWriter(Stream stream)
            {
                m_stream = stream;
            }
            public override void WriteByte(byte value)
            {
                m_stream.WriteByte(value);
            }
            public override void Flush()
            {
                // no-op
            }
        }

        private class PcxRleByteWriter : PcxByteWriter
        {
            private Stream m_stream;
            private byte m_lastValue;
            private uint m_count = 0;

            public PcxRleByteWriter(Stream output)
            {
                m_stream = output;
            }

            public override void WriteByte(byte value)
            {
                if (m_count == 0 || m_count == 63 || value != m_lastValue)
                {
                    Flush();

                    m_lastValue = value;
                    m_count = 1;
                }
                else
                {
                    m_count++;
                }
            }

            public override void Flush()
            {
                if (m_count == 0)
                {
                    return;
                }
                else if ((m_count > 1) || ((m_count == 1) && ((m_lastValue & PcxRleMask) == PcxRleMask)))
                {
                    m_stream.WriteByte((byte)(PcxRleMask | m_count));
                    m_stream.WriteByte(m_lastValue);
                    m_count = 0;
                }
                else
                {
                    m_stream.WriteByte(m_lastValue);
                    m_count = 0;
                }
            }


        }

        private class PcxRleByteReader : PcxByteReader
        {
            private Stream m_stream;
            private uint m_count = 0;
            private byte m_rleValue;

            public PcxRleByteReader(Stream input)
            {
                m_stream = input;
            }

            public override byte ReadByte()
            {
                if (m_count > 0)
                {
                    m_count--;
                    return m_rleValue;
                }
                else
                {
                    byte code = (byte)m_stream.ReadByte();

                    if ((code & PcxRleMask) == PcxRleMask)
                    {
                        m_count = (uint)(code & (PcxRleMask ^ 0xff));
                        m_rleValue = (byte)m_stream.ReadByte();

                        m_count--;
                        return m_rleValue;
                    }
                    else
                    {
                        return code;
                    }
                }
            }
        }

        #endregion
    }
}
