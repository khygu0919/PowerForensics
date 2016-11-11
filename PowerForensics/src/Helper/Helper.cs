﻿using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace PowerForensics
{
    public static class Helper
    {
        #region Enum

        public enum FILE_SYSTEM_TYPE
        {
            EXFAT = 0x00,
            FAT = 0x01,
            NTFS = 0x02,
            SOMETHING_ELSE = 0x03
        }

        #endregion Enum

        #region Helper Functions

        public static FILE_SYSTEM_TYPE GetFileSystemType(string volume)
        {
            byte[] bytes = Utilities.DD.Get(volume, 0x00, 0x200, 0x01);
            return GetFileSystemType(bytes);
        }

        internal static FILE_SYSTEM_TYPE GetFileSystemType(byte[] bytes)
        {
            switch (Encoding.ASCII.GetString(bytes, 0x03, 0x08))
            {
                case "EXFAT   ":
                    return FILE_SYSTEM_TYPE.EXFAT;
                case "MSDOS5.0":
                    return FILE_SYSTEM_TYPE.FAT;
                case "NTFS    ":
                    return FILE_SYSTEM_TYPE.NTFS;
                default:
                    return FILE_SYSTEM_TYPE.SOMETHING_ELSE;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        internal static string getVolumeName(ref string volume)
        {
            if (volume == null)
            {
                volume = Helper.GetVolumeFromPath(System.IO.Directory.GetCurrentDirectory());
            }

            if (!(volume.Contains(@"\\.\")))
            {
                volume = @"\\.\" + volume;
            }

            return volume;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetVolumeFromPath(string path)
        {
            return "\\\\.\\" + path.Split('\\')[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        internal static string GetVolumeLetter(string volume)
        {
            return volume.Split('\\')[3];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        internal static string GetSecurityIdentifier(byte[] bytes)
        {
            IntPtr ptrSid;
            string sidString;

            if(!NativeMethods.ConvertSidToStringSid(bytes, out ptrSid))
            {
                int HRESULT = Marshal.GetHRForLastWin32Error();
                Marshal.ThrowExceptionForHR(HRESULT);
            }

            return Marshal.PtrToStringAnsi(ptrSid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static FileStream getFileStream(string fileName)
        {
            if (!(fileName.Contains(@"\\.\PHYSICALDRIVE")))
            {
                getVolumeName(ref fileName);
            }

            // Get Handle to specified Volume/File/Directory
            SafeFileHandle hDevice = NativeMethods.CreateFile(
                fileName: fileName,
                fileAccess: FileAccess.Read,
                fileShare: FileShare.Write | FileShare.Read | FileShare.Delete,
                securityAttributes: IntPtr.Zero,
                creationDisposition: FileMode.Open,
                flags: NativeMethods.FILE_FLAG_BACKUP_SEMANTICS,
                template: IntPtr.Zero);

            // Check if handle is valid
            if (hDevice.IsInvalid)
            {
                int HRESULT = Marshal.GetHRForLastWin32Error();
                Marshal.ThrowExceptionForHR(HRESULT);
                //throw new Exception(@"Invalid handle to Volume/Drive returned. PowerShell must be run as Administrator to get a device handle.");
            }

            // Return a FileStream to read from the specified handle
            return new FileStream(hDevice, FileAccess.Read);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="offset"></param>
        /// <param name="sizeToRead"></param>
        /// <returns></returns>
        internal static byte[] readDrive(string device, long offset, long sizeToRead)
        {
            // Create a FileStream to read from hDrive
            using (FileStream streamToRead = getFileStream(device))
            {
                return readDrive(streamToRead, offset, sizeToRead);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="sectorOffset"></param>
        /// <param name="sectorCount"></param>
        /// <returns></returns>
        internal static byte[] readSector(string device, long sectorOffset, long sectorCount)
        {
            // Create a FileStream to read from hDrive
            using (FileStream streamToRead = getFileStream(device))
            {
                return readDrive(streamToRead, sectorOffset * NativeMethods.BYTES_PER_SECTOR, sectorCount * NativeMethods.BYTES_PER_SECTOR);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamToRead"></param>
        /// <param name="offset"></param>
        /// <param name="sizeToRead"></param>
        /// <returns></returns>
        internal static byte[] readDrive(FileStream streamToRead, long offset, long sizeToRead)
        {
            // Bytes must be read by sector
            //if ((sizeToRead < 1)) throw new System.ArgumentException("Size parameter cannot be null or 0 or less than 0!");
            if (((sizeToRead % 512) != 0)) throw new System.ArgumentException("Size parameter must be divisible by 512");
            if (((offset % 512) != 0)) throw new System.ArgumentException("Offset parameter must be divisible by 512");

            // Set offset to begin reading from the drive
            streamToRead.Position = offset;

            // Create a byte array to read into
            byte[] buf = new byte[sizeToRead];

            // Read buf.Length bytes (sizeToRead) from offset 
            try
            {
                Int32 bytesRead = streamToRead.Read(buf, 0, buf.Length);

                if (bytesRead != buf.Length)
                {
                    if (bytesRead > buf.Length)
                    {
                        throw new Exception("The readDrive method read more bytes from disk than expected.");
                    }
                    /*else if (bytesRead < buf.Length)
                    {
                        throw new Exception("The readDrive method read less bytes from disk than expected.");
                    }*/
                }

            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("The readDrive method experienced an ArgumentNullException.");
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException("The readDrive method experienced an ArgumentOutOfRangeException.");
            }
            catch (EndOfStreamException)
            {
                throw new EndOfStreamException("The readDrive method experienced an EndOfStreamException.");
            }
            catch (IOException)
            {
                throw new IOException("The readDrive method experienced an IOException.");
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("The readDrive method experienced an ArgumentException");
            }
            catch (ObjectDisposedException)
            {
                throw new ObjectDisposedException("The readDrive method experienced an ObjectDisposedException");
            }

            return buf;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        internal static DateTime FromUnixTime(uint unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        internal static long ToUnixTime(DateTime dateTime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime - sTime).TotalSeconds;
        }

        internal static DateTime GetFATTime(ushort date)
        {
            return GetFATTime(date, 0, 0);
        }

        internal static DateTime GetFATTime(ushort date, ushort time)
        {
            return GetFATTime(date, time, 0);
        }

        internal static DateTime GetFATTime(ushort date, ushort time, byte tenth)
        {
            if (date == 0)
            {
                return new DateTime(1980, 1, 1);
            }
            else
            {
                //Date Format. A FAT directory entry date stamp is a 16 - bit field that is basically a date relative to the MS-DOS epoch of 01 / 01 / 1980.Here is the format(bit 0 is the LSB of the 16 - bit word, bit 15 is the MSB of the 16 - bit word):  
                //Bits 0–4: Day of month, valid value range 1 - 31 inclusive.
                int day = date & 0x1F;
                //Bits 5–8: Month of year, 1 = January, valid value range 1–12 inclusive.
                int month = (date & 0x1E0) >> 5;
                //Bits 9–15: Count of years from 1980, valid value range 0–127 inclusive(1980–2107).
                int year = ((date & 0xFE00) >> 9) + 1980;

                //Time Format.A FAT directory entry time stamp is a 16 - bit field that has a granularity of 2 seconds.Here is the format(bit 0 is the LSB of the 16 - bit word, bit 15 is the MSB of the 16 - bit word).
                //Bits 0–4: 2 - second count, valid value range 0–29 inclusive(0 – 58 seconds).
                int second = (time & 0x1F) * 2;
                //Bits 5–10: Minutes, valid value range 0–59 inclusive.
                int minute = (time & 0x7E0) >> 5;
                //Bits 11–15: Hours, valid value range 0–23 inclusive.
                int hour = (time & 0xF800) >> 11;

                // Millisecond stamp at file creation time. This field actually contains a count of tenths of a second. The granularity of the seconds part of DIR_CrtTime is 2 seconds so this field is a count of tenths of a second and its valid value range is 0-199 inclusive. 
                //Console.WriteLine(tenth);
                int millisecond = tenth;

                return new DateTime(year, month, day, hour, minute, second, 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="InputBytes"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        internal static byte[] GetSubArray(byte[] InputBytes, long offset, long length)
        {
            byte[] outputBytes = new byte[length];
            Array.Copy(InputBytes, offset, outputBytes, 0x00, outputBytes.Length);
            return outputBytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="InputBytes"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        internal static byte[] GetSubArray(byte[] InputBytes, int offset, int length)
        {
            byte[] outputBytes = new byte[length];
            Array.Copy(InputBytes, offset, outputBytes, 0x00, outputBytes.Length);
            return outputBytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        internal static void ApplyFixup(ref byte[] bytes, int offset)
        {
            // Take UpdateSequence into account
            ushort usoffset = BitConverter.ToUInt16(bytes, 0x04);
            ushort ussize = BitConverter.ToUInt16(bytes, 0x06);

            if (ussize != 0)
            {
                ushort UpdateSequenceNumber = BitConverter.ToUInt16(bytes, usoffset + offset);
                byte[] UpdateSequenceArray = Helper.GetSubArray(bytes, (usoffset + 2 + offset), (2 * ussize));

                bytes[0x1FE + offset] = UpdateSequenceArray[0x00];
                bytes[0x1FF + offset] = UpdateSequenceArray[0x01];
                bytes[0x3FE + offset] = UpdateSequenceArray[0x02];
                bytes[0x3FF + offset] = UpdateSequenceArray[0x03];
                bytes[0x5FE + offset] = UpdateSequenceArray[0x04];
                bytes[0x5FF + offset] = UpdateSequenceArray[0x05];
                bytes[0x7FE + offset] = UpdateSequenceArray[0x06];
                bytes[0x7FF + offset] = UpdateSequenceArray[0x07];
                bytes[0x9FE + offset] = UpdateSequenceArray[0x08];
                bytes[0x9FF + offset] = UpdateSequenceArray[0x09];
                bytes[0xBFE + offset] = UpdateSequenceArray[0x0A];
                bytes[0xBFF + offset] = UpdateSequenceArray[0x0B];
                bytes[0xDFE + offset] = UpdateSequenceArray[0x0C];
                bytes[0xDFF + offset] = UpdateSequenceArray[0x0D];
                bytes[0xFFE + offset] = UpdateSequenceArray[0x0E];
                bytes[0xFFF + offset] = UpdateSequenceArray[0x0F];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string FromRot13(string value)
        {
            char[] array = value.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                int number = (int)array[i];

                if (number >= 'a' && number <= 'z')
                {
                    if (number > 'm')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                else if (number >= 'A' && number <= 'Z')
                {
                    if (number > 'M')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                array[i] = (char)number;
            }
            return new string(array);
        }

        #endregion Helper Functions
    }
}
