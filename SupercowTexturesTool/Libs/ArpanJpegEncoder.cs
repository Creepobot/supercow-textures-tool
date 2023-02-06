
// @author : Arpan Jati <arpan4017@yahoo.com> | 01 June 2010 
// http://www.codeproject.com/KB/graphics/SimpleJpeg.aspx

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace JpegEncoder
{
    public class ArpanJpegEncoder
    {
        readonly sbyte[] Y_Data = new sbyte[64];
        readonly sbyte[] Cb_Data = new sbyte[64];
        readonly sbyte[] Cr_Data = new sbyte[64];

        private byte[] _luminance_table = Tables.Standard_Luminance_Quantization_Table;
        private byte[] _chromiance_table = Tables.Standard_Chromiance_Quantization_Table;

        /// <summary>
        /// A 64 byte array which corresponds to a JPEG Luminance Quantization table.
        /// </summary>
        public byte[] LuminanceTable
        {
            get { return _luminance_table; }
            set { _luminance_table = value; }
        }

        /// <summary>
        /// A 64 byte array which corresponds to a JPEG Chromiance Quantization table.
        /// </summary>
        public byte[] ChromianceTable
        {
            get { return _chromiance_table; }
            set { _chromiance_table = value; }
        }

        readonly UInt16[] mask = { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768 };

        Byte bytenew = 0;
        SByte bytepos = 7;

        int Width = 0;
        int Height = 0;

        byte[,,] Bitmap_RGB_Buffer = new byte[1, 1, 1];

        private Int16[] Do_FDCT_Quantization_And_ZigZag(SByte[] channel_data, float[] quant_table)
        {

            float tmp0, tmp1, tmp2, tmp3, tmp4, tmp5, tmp6, tmp7;
            float tmp10, tmp11, tmp12, tmp13;
            float z1, z2, z3, z4, z5, z11, z13;
            float[] temp_data = new float[64];
            Int16[] outdata = new Int16[64];
            float temp;
            SByte ctr;
            Byte i;
            int k = 0;

            for (i = 0; i < 64; i++)
            {
                temp_data[i] = channel_data[i];
            }

            /* Pass 1: process rows. */

            for (ctr = 7; ctr >= 0; ctr--)
            {
                tmp0 = temp_data[0 + k] + temp_data[7 + k];
                tmp7 = temp_data[0 + k] - temp_data[7 + k];
                tmp1 = temp_data[1 + k] + temp_data[6 + k];
                tmp6 = temp_data[1 + k] - temp_data[6 + k];
                tmp2 = temp_data[2 + k] + temp_data[5 + k];
                tmp5 = temp_data[2 + k] - temp_data[5 + k];
                tmp3 = temp_data[3 + k] + temp_data[4 + k];
                tmp4 = temp_data[3 + k] - temp_data[4 + k];

                /* Even part */

                tmp10 = tmp0 + tmp3;    /* phase 2 */
                tmp13 = tmp0 - tmp3;
                tmp11 = tmp1 + tmp2;
                tmp12 = tmp1 - tmp2;

                temp_data[0 + k] = tmp10 + tmp11; /* phase 3 */
                temp_data[4 + k] = tmp10 - tmp11;

                z1 = (tmp12 + tmp13) * ((float)0.707106781); /* c4 */
                temp_data[2 + k] = tmp13 + z1;  /* phase 5 */
                temp_data[6 + k] = tmp13 - z1;

                /* Odd part */

                tmp10 = tmp4 + tmp5;    /* phase 2 */
                tmp11 = tmp5 + tmp6;
                tmp12 = tmp6 + tmp7;

                /* The rotator is modified from fig 4-8 to avoid extra negations. */
                z5 = (tmp10 - tmp12) * ((float)0.382683433); /* c6 */
                z2 = ((float)0.541196100) * tmp10 + z5; /* c2-c6 */
                z4 = ((float)1.306562965) * tmp12 + z5; /* c2+c6 */
                z3 = tmp11 * ((float)0.707106781); /* c4 */

                z11 = tmp7 + z3;        /* phase 5 */
                z13 = tmp7 - z3;

                temp_data[5 + k] = z13 + z2;    /* phase 6 */
                temp_data[3 + k] = z13 - z2;
                temp_data[1 + k] = z11 + z4;
                temp_data[7 + k] = z11 - z4;

                k += 8;  /* advance pointer to next row */
            }

            /* Pass 2: process columns. */

            k = 0;

            for (ctr = 7; ctr >= 0; ctr--)
            {
                tmp0 = temp_data[0 + k] + temp_data[56 + k];
                tmp7 = temp_data[0 + k] - temp_data[56 + k];
                tmp1 = temp_data[8 + k] + temp_data[48 + k];
                tmp6 = temp_data[8 + k] - temp_data[48 + k];
                tmp2 = temp_data[16 + k] + temp_data[40 + k];
                tmp5 = temp_data[16 + k] - temp_data[40 + k];
                tmp3 = temp_data[24 + k] + temp_data[32 + k];
                tmp4 = temp_data[24 + k] - temp_data[32 + k];

                /* Even part */

                tmp10 = tmp0 + tmp3;    /* phase 2 */
                tmp13 = tmp0 - tmp3;
                tmp11 = tmp1 + tmp2;
                tmp12 = tmp1 - tmp2;

                temp_data[0 + k] = tmp10 + tmp11; /* phase 3 */
                temp_data[32 + k] = tmp10 - tmp11;

                z1 = (tmp12 + tmp13) * ((float)0.707106781); /* c4 */
                temp_data[16 + k] = tmp13 + z1; /* phase 5 */
                temp_data[48 + k] = tmp13 - z1;

                /* Odd part */

                tmp10 = tmp4 + tmp5;    /* phase 2 */
                tmp11 = tmp5 + tmp6;
                tmp12 = tmp6 + tmp7;

                /* The rotator is modified from fig 4-8 to avoid extra negations. */
                z5 = (tmp10 - tmp12) * ((float)0.382683433); /* c6 */
                z2 = ((float)0.541196100) * tmp10 + z5; /* c2-c6 */
                z4 = ((float)1.306562965) * tmp12 + z5; /* c2+c6 */
                z3 = tmp11 * ((float)0.707106781); /* c4 */

                z11 = tmp7 + z3;        /* phase 5 */
                z13 = tmp7 - z3;

                temp_data[40 + k] = z13 + z2; /* phase 6 */
                temp_data[24 + k] = z13 - z2;
                temp_data[8 + k] = z11 + z4;
                temp_data[56 + k] = z11 - z4;


                k++;   /* advance pointer to next column */
            }

            // Do Quantization, ZigZag and proper roundoff.
            for (i = 0; i < 64; i++)
            {
                temp = temp_data[i] * quant_table[i];
                outdata[Tables.ZigZag[i]] = (Int16)((Int16)(temp + 16384.5) - 16384);
            }

            return outdata;
        }

        private void Update_Global_Pixel_8_8_Data(int posX, int posY)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    byte R = Bitmap_RGB_Buffer[i + posX, j + posY, 0];
                    byte G = Bitmap_RGB_Buffer[i + posX, j + posY, 1];
                    byte B = Bitmap_RGB_Buffer[i + posX, j + posY, 2];

                    /*Y_Data[i, j] = (Byte)(0.299 * R + 0.587 * G + 0.114 * B);
					Cb_Data[i, j] = (Byte)(-0.1687 * R - 0.3313 * G + 0.5 * B + 128);
					Cr_Data[i, j] = (Byte)(0.5 * R - 0.4187 * G - 0.0813 * B + 128);  */

                    Y_Data[i + j * 8] = (sbyte)(((Tables.Y_Red_Table[(R)] + Tables.Y_Green_Table[(G)] + Tables.Y_Blue_Table[(B)]) >> 16) - 128);
                    Cb_Data[i + j * 8] = (sbyte)((Tables.Cb_Red_Table[(R)] + Tables.Cb_Green_Table[(G)] + Tables.Cb_Blue_Table[(B)]) >> 16);
                    Cr_Data[i + j * 8] = (sbyte)((Tables.Cr_Red_Table[(R)] + Tables.Cr_Green_Table[(G)] + Tables.Cr_Blue_Table[(B)]) >> 16);
                }
            }
        }

        void DoHuffmanEncoding(Int16[] DU, ref Int16 DC, Tables.BitString[] HTDC, Tables.BitString[] HTAC, BinaryWriter bw)
        {
            Tables.BitString EOB = HTAC[0x00];
            Tables.BitString M16zeroes = HTAC[0xF0];
            Byte i;
            Byte startpos;
            Byte end0pos;
            Byte nrzeroes;
            Byte nrmarker;
            Int16 Diff;

            // Encode DC
            Diff = (Int16)(DU[0] - DC);
            DC = DU[0];

            if (Diff == 0)
                WriteBits(HTDC[0], bw);
            else
            {
                WriteBits(HTDC[Tables.Category[32767 + Diff]], bw);
                WriteBits(Tables.BitCode[32767 + Diff], bw);
            }

            // Encode ACs
            for (end0pos = 63; (end0pos > 0) && (DU[end0pos] == 0); end0pos--) ;
            //end0pos = first element in reverse order != 0

            i = 1;
            while (i <= end0pos)
            {
                startpos = i;
                for (; (DU[i] == 0) && (i <= end0pos); i++) ;
                nrzeroes = (byte)(i - startpos);
                if (nrzeroes >= 16)
                {
                    for (nrmarker = 1; nrmarker <= nrzeroes / 16; nrmarker++)
                        WriteBits(M16zeroes, bw);
                    nrzeroes = (byte)(nrzeroes % 16);
                }
                WriteBits(HTAC[nrzeroes * 16 + Tables.Category[32767 + DU[i]]], bw);
                WriteBits(Tables.BitCode[32767 + DU[i]], bw);
                i++;
            }

            if (end0pos != 63)
                WriteBits(EOB, bw);
        }

        void WriteBits(Tables.BitString bs, BinaryWriter bw)
        {
            UInt16 value;
            SByte posval;

            value = bs.value;
            posval = (SByte)(bs.length - 1);
            while (posval >= 0)
            {
                if ((value & mask[posval]) != 0)
                {
                    bytenew = (Byte)(bytenew | mask[bytepos]);
                }
                posval--;
                bytepos--;
                if (bytepos < 0)
                {
                    // Write to stream
                    if (bytenew == 0xFF)
                    {
                        // Handle special case
                        bw.Write((byte)(0xFF));
                        bw.Write((byte)(0x00));
                    }
                    else bw.Write(bytenew);
                    // Reinitialize
                    bytepos = 7;
                    bytenew = 0;
                }
            }
        }

        /// <summary>
        /// Encodes a provided ImageBuffer[,,] to a JPG Image.
        /// </summary>
        /// <param name="ImageBuffer">The ImageBuffer containing the pixel data.</param>
        /// <param name="originalDimension">Dimension of the original image. This value is written to the image header.</param>
        /// <param name="actualDimension">Dimension on which the Encoder works. As the Encoder works in 8*8 blocks, if the image size is not divisible by 8 the remaining blocks are set to '0' (in this implementation)</param>
        /// <param name="OutputStream">Stream to which the JPEG data is to be written.</param>
        /// <param name="Quantizer_Quality">Required quantizer quality; Default: 50 , Lower value higher quality.</param>
        /// <param name="progress">Interface for updating Progress.</param>
        /// <param name="currentOperation">Interface for updating CurrentOperation.</param>
        public void EncodeImageBufferToJpg(Byte[,,] ImageBuffer, Point originalDimension, Point actualDimension, BinaryWriter OutputStream, float Quantizer_Quality, Utils.IProgress progress, Utils.ICurrentOperation currentOperation)
        {
            Width = actualDimension.X;
            Height = actualDimension.Y;

            Bitmap_RGB_Buffer = ImageBuffer;

            UInt16 xpos, ypos;

            currentOperation?.SetOperation(Utils.CurrentOperation.InitializingTables);
            Tables.InitializeAllTables(Quantizer_Quality, _luminance_table, _chromiance_table);

            currentOperation?.SetOperation(Utils.CurrentOperation.WritingJPEGHeader);
            JpegHeader.WriteJpegHeader(OutputStream, new Point(originalDimension.X, originalDimension.Y));

            Int16 prev_DC_Y = 0;
            Int16 prev_DC_Cb = 0;
            Int16 prev_DC_Cr = 0;

            currentOperation?.SetOperation(Utils.CurrentOperation.EncodeImageBufferToJpg);

            for (ypos = 0; ypos < Height; ypos += 8)
            {
                progress?.SetProgress(Height * Width, Width * ypos);

                for (xpos = 0; xpos < Width; xpos += 8)
                {
                    Update_Global_Pixel_8_8_Data(xpos, ypos);

                    // Process Y Channel
                    Int16[] DCT_Quant_Y = Do_FDCT_Quantization_And_ZigZag(Y_Data, Tables.FDCT_Y_Quantization_Table);
                    DoHuffmanEncoding(DCT_Quant_Y, ref prev_DC_Y, Tables.Y_DC_Huffman_Table, Tables.Y_AC_Huffman_Table, OutputStream);

                    // Process Cb Channel
                    Int16[] DCT_Quant_Cb = Do_FDCT_Quantization_And_ZigZag(Cb_Data, Tables.FDCT_CbCr_Quantization_Table);
                    DoHuffmanEncoding(DCT_Quant_Cb, ref prev_DC_Cb, Tables.Cb_DC_Huffman_Table, Tables.Cb_AC_Huffman_Table, OutputStream);

                    // Process Cr Channel
                    Int16[] DCT_Quant_Cr = Do_FDCT_Quantization_And_ZigZag(Cr_Data, Tables.FDCT_CbCr_Quantization_Table);
                    DoHuffmanEncoding(DCT_Quant_Cr, ref prev_DC_Cr, Tables.Cb_DC_Huffman_Table, Tables.Cb_AC_Huffman_Table, OutputStream);
                }
            }
            Utils.WriteHex(OutputStream, 0xFFD9); //Write End of Image Marker    

            currentOperation?.SetOperation(Utils.CurrentOperation.Ready);
        }

        public void Save(Image image, string path, float quantizerQuality)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    EncodeImageToJpg(image, writer, quantizerQuality, null, null);
                }
            }
        }

        /// <summary>
        /// Encodes a provided Image to a JPG Image.
        /// </summary>
        /// <param name="ImageToBeEncoded">The Image to be encoded.</param>
        /// <param name="OutputStream">Stream to which the JPEG data is to be written.</param>
        /// <param name="Quantizer_Quality">Required quantizer quality; Default: 50 , Lower value higher quality.</param>
        /// <param name="progress">Interface for updating Progress.</param>
        /// <param name="currentOperation">Interface for updating CurrentOperation.</param>
        public void EncodeImageToJpg(Image ImageToBeEncoded, BinaryWriter OutputStream, float Quantizer_Quality, Utils.IProgress progress, Utils.ICurrentOperation currentOperation)
        {
            Bitmap b_in = new Bitmap(ImageToBeEncoded);
            Width = b_in.Width;
            Height = b_in.Height;
            Point originalSize = new Point(b_in.Width, b_in.Height);

            currentOperation?.SetOperation(Utils.CurrentOperation.FillImageBuffer);

            Bitmap_RGB_Buffer = Utils.Fill_Image_Buffer(b_in, progress, currentOperation);

            EncodeImageBufferToJpg(Bitmap_RGB_Buffer, originalSize, Utils.GetActualDimension(originalSize), OutputStream,
                Quantizer_Quality, progress, currentOperation);
        }
    }

    /// <summary>
    /// Generates Y, Cb, Cr, R, G and B values from given RGB_Buffer
    /// </summary>
    public class Imaging
    {
        /// <summary>
        /// Defines the different possible channel types.
        /// </summary>
        public enum ChannelType { Y, Cb, Cr, R, G, B };

        /// <summary>
        /// Generates Y, Cb, Cr, R, G and B values from given RGB_Buffer
        /// </summary>
        /// <param name="RGB_Buffer">The input RGB_Buffer.</param>
        /// <param name="drawInGrayscale">Draw in grayscale.</param>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <param name="channel">Enum specifying the channel type required.</param>
        /// <param name="progress">Interface for updating progress.</param>
        /// <param name="operation">Interface for updating current operation.</param>
        /// <returns>3D array of the specified channel type.</returns>
        public static byte[,,] Get_Channel_Data(byte[,,] RGB_Buffer, bool drawInGrayscale, int width, int height, ChannelType channel, Utils.IProgress progress, Utils.ICurrentOperation operation)
        {
            operation.SetOperation(Utils.CurrentOperation.GetChannelData);

            int fullProgress = width * height;
            byte[,,] outData = new byte[width, height, 3];

            switch (channel)
            {
                case ChannelType.R:

                    if (drawInGrayscale)
                    {
                        for (int i = 0; i < width; i++)
                        {
                            progress.SetProgress(fullProgress, height * i);
                            for (int j = 0; j < height; j++)
                            {
                                outData[i, j, 0] = RGB_Buffer[i, j, 0];
                                outData[i, j, 1] = RGB_Buffer[i, j, 0];
                                outData[i, j, 2] = RGB_Buffer[i, j, 0];
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < width; i++)
                        {
                            progress.SetProgress(fullProgress, height * i);
                            for (int j = 0; j < height; j++)
                            {
                                outData[i, j, 0] = RGB_Buffer[i, j, 0];
                            }
                        }
                    }
                    break;

                case ChannelType.G:

                    if (drawInGrayscale)
                    {
                        for (int i = 0; i < width; i++)
                        {
                            progress.SetProgress(fullProgress, height * i);
                            for (int j = 0; j < height; j++)
                            {
                                outData[i, j, 0] = RGB_Buffer[i, j, 1];
                                outData[i, j, 1] = RGB_Buffer[i, j, 1];
                                outData[i, j, 2] = RGB_Buffer[i, j, 1];
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < width; i++)
                        {
                            progress.SetProgress(fullProgress, height * i);
                            for (int j = 0; j < height; j++)
                            {
                                outData[i, j, 1] = RGB_Buffer[i, j, 1];
                            }
                        }
                    }
                    break;

                case ChannelType.B:

                    if (drawInGrayscale)
                    {
                        for (int i = 0; i < width; i++)
                        {
                            progress.SetProgress(fullProgress, height * i);
                            for (int j = 0; j < height; j++)
                            {
                                outData[i, j, 0] = RGB_Buffer[i, j, 2];
                                outData[i, j, 1] = RGB_Buffer[i, j, 2];
                                outData[i, j, 2] = RGB_Buffer[i, j, 2];
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < width; i++)
                        {
                            progress.SetProgress(fullProgress, height * i);
                            for (int j = 0; j < height; j++)
                            {
                                outData[i, j, 2] = RGB_Buffer[i, j, 2];
                            }
                        }
                    }
                    break;

                case ChannelType.Y:

                    for (int i = 0; i < width; i++)
                    {
                        progress.SetProgress(fullProgress, height * i);
                        for (int j = 0; j < height; j++)
                        {
                            outData[i, j, 0] = ((Byte)(((Tables.Y_Red_Table[(RGB_Buffer[i, j, 0])] + Tables.Y_Green_Table[(RGB_Buffer[i, j, 1])] + Tables.Y_Blue_Table[(RGB_Buffer[i, j, 2])]) >> 16) - 128));
                            outData[i, j, 1] = outData[i, j, 0];
                            outData[i, j, 2] = outData[i, j, 0];
                        }
                    }
                    break;

                case ChannelType.Cb:

                    for (int i = 0; i < width; i++)
                    {
                        progress.SetProgress(fullProgress, height * i);
                        for (int j = 0; j < height; j++)
                        {
                            outData[i, j, 0] = ((Byte)((Tables.Cb_Red_Table[(RGB_Buffer[i, j, 0])] + Tables.Cb_Green_Table[(RGB_Buffer[i, j, 1])] + Tables.Cb_Blue_Table[(RGB_Buffer[i, j, 2])]) >> 16));
                            outData[i, j, 1] = outData[i, j, 0];
                            outData[i, j, 2] = outData[i, j, 0];
                        }
                    }
                    break;

                case ChannelType.Cr:

                    for (int i = 0; i < width; i++)
                    {
                        progress.SetProgress(fullProgress, height * i);
                        for (int j = 0; j < height; j++)
                        {
                            outData[i, j, 0] = ((Byte)((Tables.Cr_Red_Table[(RGB_Buffer[i, j, 0])] + Tables.Cr_Green_Table[(RGB_Buffer[i, j, 1])] + Tables.Cr_Blue_Table[(RGB_Buffer[i, j, 2])]) >> 16));
                            outData[i, j, 1] = outData[i, j, 0];
                            outData[i, j, 2] = outData[i, j, 0];
                        }
                    }
                    break;
            }
            operation.SetOperation(Utils.CurrentOperation.Ready);
            return outData;
        }
    }

    public class InteropGDI
    {
        /// <summary>
        /// The CreateCompatibleDC function creates a memory device context (DC) compatible with the specified device. 
        /// </summary>
        /// <param name="hdc">[in] Handle to an existing DC. If this handle is NULL, the function creates a memory DC compatible with the application's current screen. </param>
        /// <returns>
        /// If the function succeeds, the return value is the handle to a memory DC.
        /// If the function fails, the return value is NULL. 
        /// </returns>
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        /// <summary>
        /// The SelectObject function selects an object into the specified device context (DC). 
        /// The new object replaces the previous object of the same type. 
        /// </summary>
        /// <param name="hdc">[in] Handle to the DC.</param>
        /// <param name="hgdiobj">[in] Handle to the object to be selected. The specified object must have been created by using one of the following functions. </param>
        /// <returns></returns>
        [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        /// <summary>
        /// The SetStretchBltMode function sets the bitmap stretching mode in the specified device context. 
        /// </summary>
        /// <param name="hdc">[in] Handle to the device context. </param>
        /// <param name="iStretchMode">[in] Specifies the stretching mode. This parameter can be one of the values from StretchBltModes enum.</param>
        /// <returns>
        /// If the function succeeds, the return value is the previous stretching mode.
        /// If the function fails, the return value is zero. 
        /// </returns>
        [DllImport("gdi32.dll")]
        public static extern int SetStretchBltMode(IntPtr hdc, int iStretchMode);

        /// <summary>
        /// The GetObject function retrieves information for the specified graphics object. 
        /// </summary>
        /// <param name="hgdiobj">[in] Handle to the graphics object of interest. This can be a handle to one of the following: a logical bitmap, a brush, a font, a palette, a pen, or a device independent bitmap created by calling the CreateDIBSection function. </param>
        /// <param name="cbBuffer">[in] Specifies the number of bytes of information to be written to the buffer. </param>
        /// <param name="lpvObject">[out] Pointer to a buffer that receives the information about the specified graphics object. </param>
        /// <returns>
        /// If the function succeeds, and lpvObject is a valid pointer, the return value is the number of bytes stored into the buffer.
        /// If the function succeeds, and lpvObject is NULL, the return value is the number of bytes required to hold the information the function would store into the buffer.
        /// If the function fails, the return value is zero. 
        /// </returns>
        [DllImport("gdi32.dll")]
        public static extern int GetObject(IntPtr hgdiobj, int cbBuffer, ref BITMAP lpvObject);

        /// <summary>
        /// The StretchBlt function copies a bitmap from a source rectangle into a destination 
        /// rectangle, stretching or compressing the bitmap to fit the dimensions of the destination 
        /// rectangle, if necessary. The system stretches or compresses the bitmap according to 
        /// the stretching mode currently set in the destination device context. 
        /// </summary>
        /// <param name="hdcDest">[in] Handle to the destination device context. </param>
        /// <param name="nXOriginDest">[in] Specifies the x-coordinate, in logical units, of the upper-left corner of the destination rectangle. </param>
        /// <param name="nYOriginDest">[in] Specifies the y-coordinate, in logical units, of the upper-left corner of the destination rectangle. </param>
        /// <param name="nWidthDest">[in] Specifies the width, in logical units, of the destination rectangle. </param>
        /// <param name="nHeightDest">[in] Specifies the height, in logical units, of the destination rectangle. </param>
        /// <param name="hdcSrc">[in] Handle to the source device context. </param>
        /// <param name="nXOriginSrc">[in] Specifies the x-coordinate, in logical units, of the upper-left corner of the source rectangle. </param>
        /// <param name="nYOriginSrc">[in] Specifies the y-coordinate, in logical units, of the upper-left corner of the source rectangle. </param>
        /// <param name="nWidthSrc">[in] Specifies the width, in logical units, of the source rectangle. </param>
        /// <param name="nHeightSrc">[in] Specifies the height, in logical units, of the source rectangle. </param>
        /// <param name="dwRop">[in] Specifies the raster operation to be performed. Raster operation codes define how the system combines colors in output operations that involve a brush, a source bitmap, and a destination bitmap. </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. 
        /// If the function fails, the return value is zero. 
        /// </returns>
        [DllImport("gdi32.dll")]
        public static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest,
        int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc,
        int nWidthSrc, int nHeightSrc, TernaryRasterOperations dwRop);

        /// <summary>
        /// The CreateCompatibleBitmap function creates a bitmap compatible with the device that is associated with the specified device context. 
        /// </summary>
        /// <param name="hdc">[in] Handle to a device context. </param>
        /// <param name="nWidth">[in] Specifies the bitmap width, in pixels. </param>
        /// <param name="nHeight">[in] Specifies the bitmap height, in pixels. </param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the compatible bitmap (DDB).
        /// If the function fails, the return value is NULL.
        /// </returns>
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth,
          int nHeight);

        /// <summary>
        /// The GetDIBits function retrieves the bits of the specified compatible bitmap 
        /// and copies them into a buffer as a DIB using the specified format. 
        /// </summary>
        /// <param name="hdc">[in] Handle to the device context. </param>
        /// <param name="hbmp">[in] Handle to the bitmap. This must be a compatible bitmap (DDB). </param>
        /// <param name="uStartScan">[in] Specifies the first scan line to retrieve.</param>
        /// <param name="cScanLines">[in] Specifies the number of scan lines to retrieve.</param>
        /// <param name="lpvBits">[out] Pointer to a buffer to receive the bitmap data. If this parameter is NULL, the function passes the dimensions and format of the bitmap to the BITMAPINFOHEADER structure pointed to by the lpbi parameter.</param>
        /// <param name="lpbmi">[in/out] Pointer to a BITMAPINFOHEADER structure that specifies the desired format for the DIB data. </param>
        /// <param name="uUsage">[in] Specifies the format of the bmiColors member of the BITMAPINFOHEADER structure.</param>
        /// <returns>If the lpvBits parameter is non-NULL and the function succeeds, the return value is the number of scan lines copied from the bitmap.</returns>
        [DllImport("gdi32.dll")]
        public static extern int GetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan,
          uint cScanLines, [Out] byte[] lpvBits, ref BITMAPINFOHEADER lpbmi, uint uUsage);

        /// <summary>
        /// The SetDIBits function sets the pixels in a compatible bitmap (DDB) 
        /// using the color data found in the specified DIB . 
        /// </summary>
        /// <param name="hdc">[in] Handle to a device context. </param>
        /// <param name="hbmp">[in] Handle to the compatible bitmap (DDB) that is to be altered using the color data from the specified DIB.</param>
        /// <param name="uStartScan">[in] Specifies the starting scan line for the device-independent color data in the array pointed to by the lpvBits parameter. </param>
        /// <param name="cScanLines">[in] Specifies the number of scan lines found in the array containing device-independent color data. </param>
        /// <param name="lpvBits">[in] Pointer to the DIB color data, stored as an array of bytes. The format of the bitmap values depends on the biBitCount member of the BITMAPINFO structure pointed to by the lpbmi parameter. </param>
        /// <param name="lpbmi">[in] Pointer to a BITMAPINFOHEADER structure that contains information about the DIB. </param>
        /// <param name="fuColorUse">[in] Specifies whether the bmiColors member of the BITMAPINFO structure was provided and, if so, whether bmiColors contains explicit red, green, blue (RGB) values or palette indexes.</param>
        /// <returns>If the function succeeds, the return value is the number of scan lines copied.</returns>
        [DllImport("gdi32.dll")]
        public static extern int SetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan, uint
          cScanLines, byte[] lpvBits, [In] ref BITMAPINFOHEADER lpbmi, uint fuColorUse);

        /// <summary>
        /// The GetDC function retrieves a handle to a display device context (DC) 
        /// for the client area of a specified window or for the entire screen.        
        /// </summary>
        /// <param name="hWnd">[in] Handle to the window whose DC is to be retrieved. If this value is NULL, GetDC retrieves the DC for the entire screen. </param>
        /// <returns>If the function succeeds, the return value is a handle to the DC for the specified window's client area. I
        /// If the function fails, the return value is NULL. 
        /// </returns>  
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        /// <summary>
        /// The GetClientRect function retrieves the coordinates of a window's client area.
        /// The client coordinates specify the upper-left and lower-right corners of the client area. 
        /// </summary>
        /// <param name="hWnd">[in] Handle to the window whose client coordinates are to be retrieved.</param>
        /// <param name="lpRect">[out] Pointer to a RECT structure that receives the client coordinates.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        ///    Performs a bit-block transfer of the color data corresponding to a
        ///    rectangle of pixels from the specified source device context into
        ///    a destination device context.
        /// </summary>
        /// <param name="hdc">Handle to the destination device context.</param>
        /// <param name="nXDest">The leftmost x-coordinate of the destination rectangle (in pixels).</param>
        /// <param name="nYDest">The topmost y-coordinate of the destination rectangle (in pixels).</param>
        /// <param name="nWidth">The width of the source and destination rectangles (in pixels).</param>
        /// <param name="nHeight">The height of the source and the destination rectangles (in pixels).</param>
        /// <param name="hdcSrc">Handle to the source device context.</param>
        /// <param name="nXSrc">The leftmost x-coordinate of the source rectangle (in pixels).</param>
        /// <param name="nYSrc">The topmost y-coordinate of the source rectangle (in pixels).</param>
        /// <param name="dwRop">A raster-operation code.</param>
        /// <returns>
        ///    <c>true</c> if the operation succeeded, <c>false</c> otherwise.
        /// </returns>
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        /// <summary>
        /// The DeleteObject function deletes a logical pen, brush, font, bitmap, region, or palette, 
        /// freeing all system resources associated with the object. After the object is deleted, 
        /// the specified handle is no longer valid. 
        /// </summary>
        /// <param name="hObject">[in] Handle to a logical pen, brush, font, bitmap, region, or palette.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// The ReleaseDC function releases a device context (DC), freeing it for use by other applications. 
        /// The effect of the ReleaseDC function depends on the type of DC.
        /// </summary>
        /// <param name="hWnd">[in] Handle to the window whose DC is to be released. </param>
        /// <param name="hDC">[in] Handle to the DC to be released. </param>
        /// <returns>
        /// The return value indicates whether the DC was released. 
        /// If the DC was released, the return value is 1.
        /// If the DC was not released, the return value is zero.
        /// </returns>
        [DllImport("user32.dll", EntryPoint = "ReleaseDC", SetLastError = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        /// <summary>
        /// The SetPixel function sets the pixel at the specified coordinates to the specified color. 
        /// </summary>
        /// <param name="hdc">[in] Handle to the device context. </param>
        /// <param name="X">[in] Specifies the x-coordinate, in logical units, of the point to be set. </param>
        /// <param name="Y">[in] Specifies the y-coordinate, in logical units, of the point to be set. </param>
        /// <param name="crColor">[in] Specifies the color to be used to paint the point.</param>
        /// <returns>If the function succeeds, the return value is the RGB value that the function sets the pixel to. 
        /// This value may differ from the color specified by crColor; that occurs when an exact match for the 
        /// specified color cannot be found.</returns>
        [DllImport("gdi32.dll")]
        public static extern uint SetPixel(IntPtr hdc, int X, int Y, uint crColor);

        /// <summary>
        ///     Specifies a raster-operation code. These codes define how the color data for the
        ///     source rectangle is to be combined with the color data for the destination
        ///     rectangle to achieve the final color.
        /// </summary>
        public enum TernaryRasterOperations : uint
        {
            /// <summary>dest = source</summary>
            SRCCOPY = 0x00CC0020,
            /// <summary>dest = source OR dest</summary>
            SRCPAINT = 0x00EE0086,
            /// <summary>dest = source AND dest</summary>
            SRCAND = 0x008800C6,
            /// <summary>dest = source XOR dest</summary>
            SRCINVERT = 0x00660046,
            /// <summary>dest = source AND (NOT dest)</summary>
            SRCERASE = 0x00440328,
            /// <summary>dest = (NOT source)</summary>
            NOTSRCCOPY = 0x00330008,
            /// <summary>dest = (NOT src) AND (NOT dest)</summary>
            NOTSRCERASE = 0x001100A6,
            /// <summary>dest = (source AND pattern)</summary>
            MERGECOPY = 0x00C000CA,
            /// <summary>dest = (NOT source) OR dest</summary>
            MERGEPAINT = 0x00BB0226,
            /// <summary>dest = pattern</summary>
            PATCOPY = 0x00F00021,
            /// <summary>dest = DPSnoo</summary>
            PATPAINT = 0x00FB0A09,
            /// <summary>dest = pattern XOR dest</summary>
            PATINVERT = 0x005A0049,
            /// <summary>dest = (NOT dest)</summary>
            DSTINVERT = 0x00550009,
            /// <summary>dest = BLACK</summary>
            BLACKNESS = 0x00000042,
            /// <summary>dest = WHITE</summary>
            WHITENESS = 0x00FF0062
        }

        public enum StretchBltModes { BLACKONWHITE = 1, WHITEONBLACK = 2, COLORONCOLOR = 3, HALFTONE = 4, MAXSTRETCHBLTMODE = 4 };

        public enum DIB_COLORS { DIB_RGB_COLORS = 0, DIB_PAL_COLORS = 1 };

        public enum BMP_Compression_Modes { BI_RGB = 0, BI_RLE8 = 1, BI_RLE4 = 2, BI_BITFIELDS = 3, BI_JPEG = 4, BI_PNG = 5 };

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER : IEquatable<BITMAPINFOHEADER>
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;

            public static bool operator ==(BITMAPINFOHEADER left, BITMAPINFOHEADER right)
            {
                return ((left.biSize == right.biSize) &&
                        (left.biWidth == right.biWidth) &&
                        (left.biHeight == right.biHeight) &&
                        (left.biPlanes == right.biPlanes) &&
                        (left.biBitCount == right.biBitCount) &&
                        (left.biCompression == right.biCompression) &&
                        (left.biSizeImage == right.biSizeImage) &&
                        (left.biXPelsPerMeter == right.biXPelsPerMeter) &&
                        (left.biYPelsPerMeter == right.biYPelsPerMeter) &&
                        (left.biClrUsed == right.biClrUsed) &&
                        (left.biClrImportant == right.biClrImportant));
            }

            public static bool operator !=(BITMAPINFOHEADER left, BITMAPINFOHEADER right)
            {
                return !(left == right);
            }
            public bool Equals(BITMAPINFOHEADER other)
            {
                return (this == other);
            }
            public override bool Equals(object obj)
            {
                return (obj is BITMAPINFOHEADER bITMAPINFOHEADER) && (this == bITMAPINFOHEADER);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            public RGBQUAD bmiColors;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAP
        {
            public int bmType;
            public int bmWidth;
            public int bmHeight;
            public int bmWidthBytes;
            public ushort bmPlanes;
            public ushort bmBitsPixel;
            public IntPtr bmBits;
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct BITMAPFILEHEADER
        {
            public ushort bfType;
            public ulong bfSize;
            public ushort bfReserved1;
            public ushort bfReserved2;
            public ulong bfOffBits;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COLORREF
        {
            public uint ColorDWORD;

            public COLORREF(Color color)
            {
                ColorDWORD = color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            }

            public Color GetColor()
            {
                return Color.FromArgb((int)(0x000000FFU & ColorDWORD),
               (int)(0x0000FF00U & ColorDWORD) >> 8, (int)(0x00FF0000U & ColorDWORD) >> 16);
            }

            public void SetColor(Color color)
            {
                ColorDWORD = color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(Rectangle rectangle)
            {
                Left = rectangle.X;
                Top = rectangle.Y;
                Right = rectangle.Right;
                Bottom = rectangle.Bottom;
            }

            public Rectangle ToRectangle()
            {
                return new Rectangle(Left, Top, Right - Left, Bottom - Top);
            }

            public override string ToString()
            {
                return "Left: " + Left + ", " + "Top: " + Top + ", Right: " + Right + ", Bottom: " + Bottom;
            }
        }

        public static RECT GetClientRect(IntPtr hWnd)
        {
            GetClientRect(hWnd, out RECT result);
            return result;
        }

        public static int MakeCOLORREF(byte Red, byte Green, byte Blue)
        {
            return (int)(Red | (((uint)Green) << 8) | (((uint)Blue) << 16));
        }
    }

    public class JpegHeader
    {
        public class APP0infotype
        {
            readonly UInt16 marker = 0xFFE0;
            readonly UInt16 length = 16; // = 16 for usual JPEG, no thumbnail		
            readonly byte versionhi = 1; // 1
            readonly byte versionlo = 1; // 1
            readonly byte xyunits = 0;   // 0 = no units, normal density
            readonly UInt16 xdensity = 1;  // 1
            readonly UInt16 ydensity = 1;  // 1
            readonly byte thumbnwidth = 0; // 0
            readonly byte thumbnheight = 0; // 0

            public void Write_APP0info(BinaryWriter bw)
            {
                Utils.WriteHex(bw, 0xFFD8); // JPEG INIT
                Utils.WriteHex(bw, marker);
                Utils.WriteHex(bw, length);
                bw.Write('J');
                bw.Write('F');
                bw.Write('I');
                bw.Write('F');
                bw.Write((byte)0x0);
                bw.Write(versionhi);
                bw.Write(versionlo);
                bw.Write(xyunits);
                Utils.WriteHex(bw, xdensity);
                Utils.WriteHex(bw, ydensity);
                bw.Write(thumbnheight);
                bw.Write(thumbnwidth);
            }
        };

        public class SOF0infotype
        {
            readonly UInt16 marker = 0xFFC0;
            readonly UInt16 length = 17; // = 17 for a truecolor YCbCr JPG
            readonly byte precision = 8;// Should be 8: 8 bits/sample            
            readonly byte nrofcomponents = 3;//Should be 3: We encode a truecolor JPG
            readonly byte IdY = 1;  // = 1
            readonly byte HVY = 0x11; // sampling factors for Y (bit 0-3 vert., 4-7 hor.)
            readonly byte QTY = 0;  // Quantization Table number for Y = 0
            readonly byte IdCb = 2; // = 2
            readonly byte HVCb = 0x11;
            readonly byte QTCb = 1; // 1
            readonly byte IdCr = 3; // = 3
            readonly byte HVCr = 0x11;
            readonly byte QTCr = 1; // Normally equal to QTCb = 1
            public void Write_S0FInfo(BinaryWriter bw, int wid, int ht)
            {
                Utils.WriteHex(bw, marker);
                Utils.WriteHex(bw, length);
                bw.Write(precision);
                Utils.WriteHex(bw, ht);
                Utils.WriteHex(bw, wid);
                bw.Write(nrofcomponents);
                bw.Write(IdY);
                bw.Write(HVY);
                bw.Write(QTY);
                bw.Write(IdCb);
                bw.Write(HVCb);
                bw.Write(QTCb);
                bw.Write(IdCr);
                bw.Write(HVCr);
                bw.Write(QTCr);
            }
        };

        public class DQTinfotype
        {
            readonly UInt16 marker = 0xFFDB;
            readonly UInt16 length = 132;  // = 132
            readonly byte QTYinfo = 0;// = 0:  bit 0..3: number of QT = 0 (table for Y)
                                      //       bit 4..7: precision of QT, 0 = 8 bit		 

            readonly byte QTCbinfo = 1; // = 1 (quantization table for Cb,Cr}             

            public void Write_DQT(BinaryWriter bw)
            {
                Utils.WriteHex(bw, marker);
                Utils.WriteHex(bw, length);
                bw.Write(QTYinfo);
                Utils.WriteByteArray(bw, Tables.Y_Table, 0);
                bw.Write(QTCbinfo);
                Utils.WriteByteArray(bw, Tables.CbCr_Table, 0);
            }
        };

        public class DHTinfotype
        {
            readonly UInt16 marker = 0xFFC4;
            readonly UInt16 length = 0x01A2;
            readonly byte HTYDCinfo = 0x00; // bit 0..3: number of HT (0..3), for Y =0
                                            //bit 4  :type of HT, 0 = DC table,1 = AC table
                                            //bit 5..7: not used, must be 0

            readonly byte[] YDC_nrcodes = Tables.Standard_DC_Luminance_NRCodes; //at index i = nr of codes with length i
            readonly byte[] YDC_values = Tables.Standard_DC_Luminance_Values;
            readonly byte HTYACinfo = 0x10; // = 0x10
            readonly byte[] YAC_nrcodes = Tables.Standard_AC_Luminance_NRCodes;
            readonly byte[] YAC_values = Tables.Standard_AC_Luminance_Values;//we'll use the standard Huffman tables
            readonly byte HTCbDCinfo = 0x01; // = 1
            readonly byte[] CbDC_nrcodes = Tables.Standard_DC_Chromiance_NRCodes;
            readonly byte[] CbDC_values = Tables.Standard_DC_Chromiance_Values;
            readonly byte HTCbACinfo = 0x11; //  = 0x11
            readonly byte[] CbAC_nrcodes = Tables.Standard_AC_Chromiance_NRCodes;
            readonly byte[] CbAC_values = Tables.Standard_AC_Chromiance_Values;
            public void Write_DHT(BinaryWriter bw)
            {
                Utils.WriteHex(bw, marker);
                Utils.WriteHex(bw, length);
                bw.Write(HTYDCinfo);
                Utils.WriteByteArray(bw, YDC_nrcodes, 1);
                Utils.WriteByteArray(bw, YDC_values, 0);
                bw.Write(HTYACinfo);
                Utils.WriteByteArray(bw, YAC_nrcodes, 1);
                Utils.WriteByteArray(bw, YAC_values, 0);
                bw.Write(HTCbDCinfo);
                Utils.WriteByteArray(bw, CbDC_nrcodes, 1);
                Utils.WriteByteArray(bw, CbDC_values, 0);
                bw.Write(HTCbACinfo);
                Utils.WriteByteArray(bw, CbAC_nrcodes, 1);
                Utils.WriteByteArray(bw, CbAC_values, 0);
            }
        };

        public class SOSinfotype
        {
            readonly UInt16 marker = 0xFFDA;
            readonly UInt16 length = 12;
            readonly byte nrofcomponents = 3; // Should be 3: truecolor JPG
            readonly byte IdY = 1;
            readonly byte HTY = 0; // bits 0..3: AC table (0..3)
                                   // bits 4..7: DC table (0..3)

            readonly byte IdCb = 2;
            readonly byte HTCb = 0x11;
            readonly byte IdCr = 3;
            readonly byte HTCr = 0x11;
            readonly byte Ss = 0, Se = 0x3F, Bf = 0; // not interesting, they should be 0,63,0

            public void Write_S0S(BinaryWriter bw)
            {
                Utils.WriteHex(bw, marker);
                Utils.WriteHex(bw, length);
                bw.Write(nrofcomponents);
                bw.Write(IdY);
                bw.Write(HTY);
                bw.Write(IdCb);
                bw.Write(HTCb);
                bw.Write(IdCr);
                bw.Write(HTCr);
                bw.Write(Ss);
                bw.Write(Se);
                bw.Write(Bf);
            }
        };

        public static void WriteJpegHeader(BinaryWriter writer, Point imageDimensions)
        {
            APP0infotype App0Info = new APP0infotype();
            DHTinfotype HuffmanTables = new DHTinfotype();
            DQTinfotype QuantizationTables = new DQTinfotype();
            SOF0infotype S0F0 = new SOF0infotype();
            SOSinfotype S0S = new SOSinfotype();

            App0Info.Write_APP0info(writer);
            QuantizationTables.Write_DQT(writer);
            S0F0.Write_S0FInfo(writer, imageDimensions.X, imageDimensions.Y);
            HuffmanTables.Write_DHT(writer);
            S0S.Write_S0S(writer);
        }
    }

    public static class Tables
    {
        public struct BitString
        {
            public Byte length;
            public UInt16 value;

            public BitString(Byte len, UInt16 val)
            {
                length = len;
                value = val;
            }
        };

        public static Byte[] Category = new Byte[65535];
        public static BitString[] BitCode = new BitString[65535];

        public static BitString[] Y_DC_Huffman_Table = new BitString[12];
        public static BitString[] Cb_DC_Huffman_Table = new BitString[12];
        public static BitString[] Y_AC_Huffman_Table = new BitString[256];
        public static BitString[] Cb_AC_Huffman_Table = new BitString[256];

        // Y, Cb, Cr Tables
        public static Int32[] Y_Red_Table = new Int32[256];
        public static Int32[] Y_Green_Table = new Int32[256];
        public static Int32[] Y_Blue_Table = new Int32[256];
        public static Int32[] Cb_Red_Table = new Int32[256];
        public static Int32[] Cb_Green_Table = new Int32[256];
        public static Int32[] Cb_Blue_Table = new Int32[256];
        public static Int32[] Cr_Red_Table = new Int32[256];
        public static Int32[] Cr_Green_Table = new Int32[256];
        public static Int32[] Cr_Blue_Table = new Int32[256];

        // Quant data tables
        public static Byte[] Y_Table = new Byte[64];
        public static Byte[] CbCr_Table = new Byte[64];

        // Quant data tables after scaling and cos.
        public static float[] FDCT_Y_Quantization_Table = new float[64];
        public static float[] FDCT_CbCr_Quantization_Table = new float[64];

        public static Byte[] Standard_Luminance_Quantization_Table =
        {
            16,  11,  10,  16,  24,  40,  51,  61,
            12,  12,  14,  19,  26,  58,  60,  55,
            14,  13,  16,  24,  40,  57,  69,  56,
            14,  17,  22,  29,  51,  87,  80,  62,
            18,  22,  37,  56,  68, 109, 103,  77,
            24,  35,  55,  64,  81, 104, 113,  92,
            49,  64,  78,  87, 103, 121, 120, 101,
            72,  92,  95,  98, 112, 100, 103,  99
        };
        public static Byte[] Standard_Chromiance_Quantization_Table =
        {
            17,  18,  24,  47,  99,  99,  99,  99,
            18,  21,  26,  66,  99,  99,  99,  99,
            24,  26,  56,  99,  99,  99,  99,  99,
            47,  66,  99,  99,  99,  99,  99,  99,
            99,  99,  99,  99,  99,  99,  99,  99,
            99,  99,  99,  99,  99,  99,  99,  99,
            99,  99,  99,  99,  99,  99,  99,  99,
            99,  99,  99,  99,  99,  99,  99,  99
        };

        public static Byte[] Standard_DC_Luminance_NRCodes = { 0, 0, 1, 5, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 };
        public static Byte[] Standard_DC_Luminance_Values = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

        public static Byte[] Standard_DC_Chromiance_NRCodes = { 0, 0, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 };
        public static Byte[] Standard_DC_Chromiance_Values = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

        public static Byte[] Standard_AC_Luminance_NRCodes = { 0, 0, 2, 1, 3, 3, 2, 4, 3, 5, 5, 4, 4, 0, 0, 1, 0x7d };
        public static Byte[] Standard_AC_Luminance_Values =
        {
            0x01, 0x02, 0x03, 0x00, 0x04, 0x11, 0x05, 0x12,
            0x21, 0x31, 0x41, 0x06, 0x13, 0x51, 0x61, 0x07,
            0x22, 0x71, 0x14, 0x32, 0x81, 0x91, 0xa1, 0x08,
            0x23, 0x42, 0xb1, 0xc1, 0x15, 0x52, 0xd1, 0xf0,
            0x24, 0x33, 0x62, 0x72, 0x82, 0x09, 0x0a, 0x16,
            0x17, 0x18, 0x19, 0x1a, 0x25, 0x26, 0x27, 0x28,
            0x29, 0x2a, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39,
            0x3a, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49,
            0x4a, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59,
            0x5a, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69,
            0x6a, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79,
            0x7a, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89,
            0x8a, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98,
            0x99, 0x9a, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7,
            0xa8, 0xa9, 0xaa, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6,
            0xb7, 0xb8, 0xb9, 0xba, 0xc2, 0xc3, 0xc4, 0xc5,
            0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xd2, 0xd3, 0xd4,
            0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xe1, 0xe2,
            0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea,
            0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8,
            0xf9, 0xfa
        };

        public static Byte[] Standard_AC_Chromiance_NRCodes = { 0, 0, 2, 1, 2, 4, 4, 3, 4, 7, 5, 4, 4, 0, 1, 2, 0x77 };
        public static Byte[] Standard_AC_Chromiance_Values =
        {
            0x00, 0x01, 0x02, 0x03, 0x11, 0x04, 0x05, 0x21,
            0x31, 0x06, 0x12, 0x41, 0x51, 0x07, 0x61, 0x71,
            0x13, 0x22, 0x32, 0x81, 0x08, 0x14, 0x42, 0x91,
            0xa1, 0xb1, 0xc1, 0x09, 0x23, 0x33, 0x52, 0xf0,
            0x15, 0x62, 0x72, 0xd1, 0x0a, 0x16, 0x24, 0x34,
            0xe1, 0x25, 0xf1, 0x17, 0x18, 0x19, 0x1a, 0x26,
            0x27, 0x28, 0x29, 0x2a, 0x35, 0x36, 0x37, 0x38,
            0x39, 0x3a, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48,
            0x49, 0x4a, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58,
            0x59, 0x5a, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68,
            0x69, 0x6a, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78,
            0x79, 0x7a, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87,
            0x88, 0x89, 0x8a, 0x92, 0x93, 0x94, 0x95, 0x96,
            0x97, 0x98, 0x99, 0x9a, 0xa2, 0xa3, 0xa4, 0xa5,
            0xa6, 0xa7, 0xa8, 0xa9, 0xaa, 0xb2, 0xb3, 0xb4,
            0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xc2, 0xc3,
            0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xd2,
            0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda,
            0xe2, 0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9,
            0xea, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8,
            0xf9, 0xfa
        };

        public static Byte[] ZigZag =
        {
            0, 1, 5, 6,14,15,27,28,
            2, 4, 7,13,16,26,29,42,
            3, 8,12,17,25,30,41,43,
            9,11,18,24,31,40,44,53,
            10,19,23,32,39,45,52,54,
            20,22,33,38,46,51,55,60,
            21,34,37,47,50,56,59,61,
            35,36,48,49,57,58,62,63
        };



        static Byte[] Scale_And_ZigZag_Quantization_Table(Byte[] inTable, float quality_scale)
        {
            Byte[] outTable = new Byte[64];
            long temp;
            for (Byte i = 0; i < 64; i++)
            {
                temp = ((long)(inTable[i] * quality_scale + 50L) / 100L);
                if (temp <= 0L)
                    temp = 1L;
                if (temp > 255L)
                    temp = 255L;
                outTable[ZigZag[i]] = (Byte)temp;
            }
            return outTable;
        }

        static void Initialize_Quantization_Tables(float scaleFactor, byte[] luminance_table, byte[] chromiance_table)
        {
            Y_Table = Scale_And_ZigZag_Quantization_Table(luminance_table, scaleFactor);
            CbCr_Table = Scale_And_ZigZag_Quantization_Table(chromiance_table, scaleFactor);
        }

        static void Compute_Huffman_Table(Byte[] nrCodes, Byte[] std_table, ref BitString[] Huffman_Table)
        {
            Byte k, j;
            Byte pos_in_table;
            UInt16 code_value;

            code_value = 0;
            pos_in_table = 0;
            for (k = 1; k <= 16; k++)
            {
                for (j = 1; j <= nrCodes[k]; j++)
                {
                    Huffman_Table[std_table[pos_in_table]].value = code_value;
                    Huffman_Table[std_table[pos_in_table]].length = k;
                    pos_in_table++;
                    code_value++;
                }
                code_value <<= 1;
            }
        }

        static void Initialize_Huffman_Tables()
        {
            // Compute the Huffman tables used for encoding
            Compute_Huffman_Table(Standard_DC_Luminance_NRCodes, Standard_DC_Luminance_Values, ref Y_DC_Huffman_Table);
            Compute_Huffman_Table(Standard_AC_Luminance_NRCodes, Standard_AC_Luminance_Values, ref Y_AC_Huffman_Table);
            Compute_Huffman_Table(Standard_DC_Chromiance_NRCodes, Standard_DC_Chromiance_Values, ref Cb_DC_Huffman_Table);
            Compute_Huffman_Table(Standard_AC_Chromiance_NRCodes, Standard_AC_Chromiance_Values, ref Cb_AC_Huffman_Table);
        }

        static void Initialize_Category_And_Bitcode()
        {
            Int32 nr;
            Int32 nr_lower, nr_upper;
            Byte cat;

            nr_lower = 1;
            nr_upper = 2;
            for (cat = 1; cat <= 15; cat++)
            {
                //Positive numbers
                for (nr = nr_lower; nr < nr_upper; nr++)
                {
                    Category[32767 + nr] = cat;
                    BitCode[32767 + nr].length = cat;
                    BitCode[32767 + nr].value = (ushort)nr;
                }
                //Negative numbers
                for (nr = -(nr_upper - 1); nr <= -nr_lower; nr++)
                {
                    Category[32767 + nr] = cat;
                    BitCode[32767 + nr].length = cat;
                    BitCode[32767 + nr].value = (ushort)(nr_upper - 1 + nr);
                }
                nr_lower <<= 1;
                nr_upper <<= 1;
            }
        }

        static void Initialize_FDCT_Quantization_Tables()
        {
            double[] CosineScaleFactor = { 1.0, 1.387039845, 1.306562965, 1.175875602, 1.0, 0.785694958, 0.541196100, 0.275899379 };

            Byte i = 0;

            for (Byte row = 0; row < 8; row++)
            {
                for (Byte col = 0; col < 8; col++)
                {
                    FDCT_Y_Quantization_Table[i] = (float)(1.0 / (Y_Table[ZigZag[i]] *
                        CosineScaleFactor[row] * CosineScaleFactor[col] * 8.0));
                    FDCT_CbCr_Quantization_Table[i] = (float)(1.0 / (CbCr_Table[ZigZag[i]] *
                        CosineScaleFactor[row] * CosineScaleFactor[col] * 8.0));
                    i++;
                }
            }
        }

        public static void Precalculate_YCbCr_Tables()
        {
            UInt16 R, G, B;

            for (R = 0; R < 256; R++)
            {
                Y_Red_Table[R] = (Int32)((65536 * 0.299 + 0.5) * R);
                Cb_Red_Table[R] = (Int32)((65536 * -0.16874 + 0.5) * R);
                Cr_Red_Table[R] = (32768) * R;
            }
            for (G = 0; G < 256; G++)
            {
                Y_Green_Table[G] = (Int32)((65536 * 0.587 + 0.5) * G);
                Cb_Green_Table[G] = (Int32)((65536 * -0.33126 + 0.5) * G);
                Cr_Green_Table[G] = (Int32)((65536 * -0.41869 + 0.5) * G);
            }
            for (B = 0; B < 256; B++)
            {
                Y_Blue_Table[B] = (Int32)((65536 * 0.114 + 0.5) * B);
                Cb_Blue_Table[B] = (32768) * B;
                Cr_Blue_Table[B] = (Int32)((65536 * -0.08131 + 0.5) * B);
            }
        }

        public static void InitializeAllTables(float quantizer_quality, byte[] luminance_table, byte[] chromiance_table)
        {
            Precalculate_YCbCr_Tables();
            Initialize_Quantization_Tables((float)quantizer_quality, luminance_table, chromiance_table);
            Initialize_Huffman_Tables();
            Initialize_Category_And_Bitcode();
            Initialize_FDCT_Quantization_Tables();
        }
    }
    public class Utils
    {
        public interface IProgress
        {
            void SetProgress(Int32 FullProgress, Int32 CurrentProgress);
        };

        public class ProgressUpdater : IProgress
        {
            public Int32 Full, Current;

            void IProgress.SetProgress(Int32 FullProgress, Int32 CurrentProgress)
            {
                Full = FullProgress;
                Current = CurrentProgress;
            }
        }

        public enum CurrentOperation { PrecalculateYCbCrTables, InitializingTables, WritingJPEGHeader, FillImageBuffer, EncodeImageBufferToJpg, GetChannelData, WriteChannelImages, Ready };

        public interface ICurrentOperation
        {
            void SetOperation(CurrentOperation currentOperation);
        };

        public class CurrentOperationUpdater : ICurrentOperation
        {
            public CurrentOperation operation;

            void ICurrentOperation.SetOperation(CurrentOperation currentOperation)
            {
                operation = currentOperation;
            }
        }

        public static void WriteHex(BinaryWriter bwX, Int32 data)
        {
            bwX.Write((byte)(data / 256));
            bwX.Write((byte)(data % 256));
        }

        public static void WriteByteArray(BinaryWriter bwX, Byte[] data, int startPos)
        {
            int len = data.Length;
            for (int i = startPos; i < len; i++)
            {
                bwX.Write(data[i]);
            }
        }

        private unsafe static byte[] BytesFromBitmap(Bitmap bmp, out long stride)
        {
            BitmapData bData = bmp.LockBits(new Rectangle(new Point(), bmp.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            int bufferSize = bData.Stride * Math.Abs(bmp.Height);
            byte[] buffer = new byte[bufferSize];
            Marshal.Copy(bData.Scan0, buffer, 0, bufferSize);

            bmp.UnlockBits(bData);
            stride = bData.Stride;
            return buffer;
        }

        public static byte[,,] Fill_Image_Buffer(Bitmap bmp, IProgress progress, ICurrentOperation operation)
        {
            operation?.SetOperation(CurrentOperation.FillImageBuffer);
            Point originalSize = GetActualDimension(new Point(bmp.Width, bmp.Height));
            byte[,,] Bitmap_Buffer = new byte[originalSize.X, originalSize.Y, 3];

            long stride = 0;
            byte[] raw = BytesFromBitmap(bmp, out stride);
            for (int y = 0; y < bmp.Height; y++)
            {
                long offset = y * stride;
                progress?.SetProgress((int)raw.GetLength(0), (int)offset);
                for (int x = 0; x < bmp.Width; x++)
                    for (int channel = 2; channel >= 0; channel--)
                        Bitmap_Buffer[x, y, channel] = raw[offset++];
            }
            return Bitmap_Buffer;
        }

        public static Bitmap Write_Bmp_From_Data(byte[,,] pixel_data, Point dimensions, IProgress progress, ICurrentOperation operation)
        {
            operation.SetOperation(CurrentOperation.WriteChannelImages);

            int k = 0;
            Bitmap bmp = new Bitmap(dimensions.X, dimensions.Y);
            IntPtr hBmpOutput = bmp.GetHbitmap();

            InteropGDI.BITMAPINFOHEADER bi;

            bi.biSize = 40;
            bi.biWidth = dimensions.X;
            bi.biHeight = -dimensions.Y;
            bi.biPlanes = 1;
            bi.biBitCount = 32;
            bi.biCompression = (uint)InteropGDI.BMP_Compression_Modes.BI_RGB;
            bi.biSizeImage = 0;
            bi.biXPelsPerMeter = 0;
            bi.biYPelsPerMeter = 0;
            bi.biClrUsed = 0;
            bi.biClrImportant = 0;

            ulong bitmapLengthBytes = (ulong)(((dimensions.X * bi.biBitCount + 31) / 32) * 4 * dimensions.Y);

            byte[] bitmap_array = new byte[bitmapLengthBytes];

            for (int j = 0; j < dimensions.Y; j++)
            {
                progress.SetProgress((int)bitmapLengthBytes, k);
                for (int i = 0; i < dimensions.X; i++)
                {
                    bitmap_array[k++] = pixel_data[i, j, 2];
                    bitmap_array[k++] = pixel_data[i, j, 1];
                    bitmap_array[k++] = pixel_data[i, j, 0];
                    k++;
                }
            }

            IntPtr hdcWindow = InteropGDI.CreateCompatibleDC(IntPtr.Zero);

            InteropGDI.SetDIBits(hdcWindow, hBmpOutput, 0,
                (uint)dimensions.Y,
                bitmap_array,
                ref bi, (uint)InteropGDI.DIB_COLORS.DIB_RGB_COLORS);

            bmp = Image.FromHbitmap(hBmpOutput);

            InteropGDI.DeleteObject(hBmpOutput);
            InteropGDI.DeleteObject(hdcWindow);

            return bmp;
        }

        public static Point GetActualDimension(Point originalDim)
        {
            int width_8, height_8;

            if (originalDim.X % 8 != 0)
                width_8 = (originalDim.X / 8) * 8 + 8;
            else
                width_8 = originalDim.X;

            if (originalDim.Y % 8 != 0)
                height_8 = (originalDim.Y / 8) * 8 + 8;
            else
                height_8 = originalDim.Y;

            return new Point(width_8, height_8);
        }
    }
}