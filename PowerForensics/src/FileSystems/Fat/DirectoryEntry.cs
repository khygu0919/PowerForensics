﻿using System;
using System.Text;
using System.Collections.Generic;
using PowerForensics.Generic;
using PowerForensics.Utilities;

namespace PowerForensics.Fat
{
    public class DirectoryEntry
    {
        #region Enums

        [FlagsAttribute]
        public enum FILE_ATTR
        {
            ATTR_READ_ONLY = 0x01,
            ATTR_HIDDEN = 0x02,
            ATTR_SYSTEM = 0x04,
            ATTR_VOLUME_ID = 0x08,
            ATTR_DIRECTORY = 0x10,
            ATTR_ARCHIVE = 0x20,
            ATTR_LONG_NAME = ATTR_READ_ONLY | ATTR_HIDDEN | ATTR_SYSTEM | ATTR_VOLUME_ID
        }

        #endregion Enums

        #region Properties

        private readonly string Volume;
        public readonly string FileName;
        public readonly string FullName;
        public readonly bool Deleted;
        private readonly FILE_ATTR DIR_Attribute;
        public readonly bool Directory;
        public readonly bool Hidden;
        private readonly byte DIR_CreationTimeTenth;
        private readonly ushort DIR_CreationTime;
        private readonly ushort DIR_CreationDate;
        public readonly DateTime CreationTime;
        private readonly ushort DIR_LastAccessDate;
        public readonly DateTime AccessTime;
        private readonly ushort DIR_FirstClusterHI;
        private readonly ushort DIR_WriteTime;
        private readonly ushort DIR_WriteDate;
        public readonly DateTime WriteTime;
        private readonly ushort DIR_FirstClusterLO;
        public readonly uint FirstCluster;
        public readonly uint FileSize;

        #endregion Properties

        #region Constructors

        public DirectoryEntry(byte[] bytes, int index, string volume, List<LongDirectoryEntry> longNameList, string directoryName)
        {
            Volume = volume;
            FileName = GetShortName(bytes, index);
            FullName = GetLongName(FileName, longNameList, directoryName);
            Deleted = TestIfFree(bytes, index);
            DIR_Attribute = (FILE_ATTR)bytes[11 + index];
            Directory = DIR_Attribute.HasFlag(FILE_ATTR.ATTR_DIRECTORY);
            Hidden = DIR_Attribute.HasFlag(FILE_ATTR.ATTR_HIDDEN);
            DIR_CreationTimeTenth = bytes[13 + index];
            DIR_CreationTime = BitConverter.ToUInt16(bytes, 14 + index);
            DIR_CreationDate = BitConverter.ToUInt16(bytes, 16 + index);
            CreationTime = Helper.GetFATTime(DIR_CreationDate, DIR_CreationTime, DIR_CreationTimeTenth);
            DIR_LastAccessDate = BitConverter.ToUInt16(bytes, 18 + index);
            AccessTime = Helper.GetFATTime(DIR_LastAccessDate);
            DIR_FirstClusterHI = BitConverter.ToUInt16(bytes, 20 + index);
            DIR_WriteTime = BitConverter.ToUInt16(bytes, 22 + index);
            DIR_WriteDate = BitConverter.ToUInt16(bytes, 24 + index);
            WriteTime = Helper.GetFATTime(DIR_WriteDate, DIR_WriteTime);
            DIR_FirstClusterLO = BitConverter.ToUInt16(bytes, 26 + index);
            FirstCluster = (uint)(DIR_FirstClusterHI << 16) + DIR_FirstClusterLO;
            FileSize = BitConverter.ToUInt32(bytes, 28 + index);
        }

        #endregion Constructors

        #region StaticMethods

        public static DirectoryEntry Get(string path)
        {
            path = path.TrimEnd('\\');
            string volume = Helper.GetVolumeFromPath(path);
            string[] pathParts = path.ToLower().Split('\\');

            DirectoryEntry[] entries = GetRootDirectory(volume);
            DirectoryEntry currentEntry = null;

            for (int i = 1; i < pathParts.Length; i++)
            {
                if (i != 1)
                {
                    entries = currentEntry.GetChildItem();
                }

                foreach (DirectoryEntry entry in entries)
                {
                    if (entry.FileName.ToLower() == pathParts[i])
                    {
                        currentEntry = entry;
                        break;
                    }
                }
            }

            if(currentEntry.FullName.ToLower() == path.ToLower())
            {
                return currentEntry;
            }
            else
            {
                throw new Exception("Unable to find specificed file path");
            }
        }

        internal static DirectoryEntry Get(byte[] bytes, int index, string volume, List<LongDirectoryEntry> list, string directoryName)
        {
            return new DirectoryEntry(bytes, index, volume, list, directoryName);
        }

        public static DirectoryEntry[] GetInstances(string volume)
        {
            List<DirectoryEntry> entryList = new List<DirectoryEntry>();

            DirectoryEntry[] entries = GetRootDirectory(volume);

            entryList.AddRange(entries);

            foreach (DirectoryEntry entry in entries)
            {
                if (entry.Directory)
                {
                    entryList.AddRange(entry.GetChildItem(true));
                }
            }

            return entryList.ToArray();
        }

        private static DirectoryEntry[] GetInstances(byte[] bytes, string volume, string directoryName)
        {
            List<DirectoryEntry> list = new List<DirectoryEntry>();

            List<LongDirectoryEntry> longList = new List<LongDirectoryEntry>();

            for (int i = 0; i < bytes.Length; i += 0x20)
            {
                FILE_ATTR attr = (FILE_ATTR)bytes[11 + i];

                // If entry is a Long File Name Entry then add it to the List
                if (attr == FILE_ATTR.ATTR_LONG_NAME)
                {
                    LongDirectoryEntry longEntry = LongDirectoryEntry.Get(bytes, i);
                    longList.Add(longEntry);
                }
                // Else the entry is a valid entry
                else
                {
                    DirectoryEntry entry = DirectoryEntry.Get(bytes, i, volume, longList, directoryName);

                    if (entry.DIR_Attribute != 0 && entry.FileName != "." && entry.FileName != "..")
                    {
                        list.Add(entry);
                    }
                    
                    longList.Clear();
                }
            }

            return list.ToArray();
        }

        public static DirectoryEntry[] GetChildItem(string path)
        {
            if (path.TrimEnd('\\').Split('\\').Length == 1)
            {
                string volume = Helper.GetVolumeFromPath(path);
                return GetRootDirectory(volume);
            }
            else
            {
                return Get(path).GetChildItem();
            }
        }

        private static DirectoryEntry[] GetRootDirectory(string volume)
        {
            string volLetter = Helper.GetVolumeLetter(volume);

            FatVolumeBootRecord vbr = VolumeBootRecord.Get(volume) as FatVolumeBootRecord;
            
            uint FirstRootDirSecNum = vbr.ReservedSectors + (vbr.BPB_NumberOfFATs * vbr.BPB_FatSize32);

            byte[] bytes = DD.Get(volume, vbr.BytesPerSector * FirstRootDirSecNum, vbr.BytesPerSector, 2);

            return GetInstances(bytes, volume, volLetter);
        }

        private static string GetShortName(byte[] bytes, int index)
        {
            string name = Encoding.ASCII.GetString(bytes, 0 + index, 8).TrimEnd();
            string extension = Encoding.ASCII.GetString(bytes, 8 + index, 3);

            if (((FILE_ATTR)bytes[11 + index]).HasFlag(FILE_ATTR.ATTR_DIRECTORY))
            {
                return name;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(@"{0}.{1}", name, extension);
                return sb.ToString();
            }
        }

        private static string GetLongName(string FileName, List<LongDirectoryEntry> list, string directoryName)
        {
            // Check if there are actually Long Name Entires
            if(list.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = list.Count - 1; i >= 0; i--)
                {
                    sb.Append(list[i].LDIR_NamePart);
                }

                return String.Format(@"{0}\{1}", directoryName, sb.ToString().TrimEnd((char)0x0000,(char)0xFFFF));
            }
            // else set LongName to DIR_Name
            else
            {
                return String.Format(@"{0}\{1}", directoryName, FileName);
            }
        }

        private static bool TestIfFree(byte[] bytes, int index)
        {
            if (bytes[0 + index] == 0xE5)
            {
                return true;
            }
            else if (bytes[0 + index] == 0x00)
            {
                return true;
            }
            else if (bytes[0 + index] == 0x05)
            {
                return true;
            }
            else
            {
                return false;
            }
            //The following characters are not legal in any bytes of DIR_Name:  • Values less than 0x20 except for the special case of 0x05 in DIR_Name[0] described above. • 0x22, 0x2A, 0x2B, 0x2C, 0x2E, 0x2F, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F, 0x5B, 0x5C, 0x5D, and 0x7C.
        }

        #endregion StaticMethods

        #region InstanceMethods

        public DirectoryEntry[] GetChildItem()
        {
            if (this.DIR_Attribute.HasFlag(FILE_ATTR.ATTR_DIRECTORY))
            {
                byte[] bytes = this.GetContent();
                return GetInstances(bytes, this.Volume, this.FullName);
            }
            else
            {
                DirectoryEntry[] entries = new DirectoryEntry[1];
                entries[0] = this;
                return entries;
            }
        }
        
        public DirectoryEntry[] GetChildItem(bool recurse)
        {
            if (recurse)
            {
                List<DirectoryEntry> entryList = new List<DirectoryEntry>();

                DirectoryEntry[] entries = this.GetChildItem();

                entryList.AddRange(entries);

                foreach (DirectoryEntry entry in entries)
                {
                    if (entry.Directory && entry.FileName != "." && entry.FileName != "..")
                    {
                        entryList.AddRange(entry.GetChildItem(true));
                    }
                }

                return entryList.ToArray();
            }
            else
            {
                return this.GetChildItem();
            }
        }

        public byte[] GetContent()
        {
            FatVolumeBootRecord vbr = VolumeBootRecord.Get(this.Volume) as FatVolumeBootRecord;

            int RootDirSectors = ((vbr.BPB_RootEntryCount * 32) + (vbr.BytesPerSector - 1)) / vbr.BytesPerSector;

            uint FatSize = 0;

            if (vbr.BPB_FatSize16 != 0)
            {
                FatSize = vbr.BPB_FatSize16;
            }
            else
            {
                FatSize = vbr.BPB_FatSize32;
            }

            uint FirstDataSector = (uint)(vbr.ReservedSectors + (vbr.BPB_NumberOfFATs * FatSize) + RootDirSectors);

            uint FirstSectorofCluster = ((this.FirstCluster - 2) * vbr.SectorsPerCluster) + FirstDataSector;

            byte[] bytes = DD.Get(this.Volume, (long)FirstSectorofCluster * (long)vbr.BytesPerSector, vbr.BytesPerSector, 1);

            if (this.DIR_Attribute.HasFlag(FILE_ATTR.ATTR_DIRECTORY))
            {
                return bytes;
            }
            else
            {
                if (this.FileSize <= bytes.Length)
                {
                    return Helper.GetSubArray(bytes, 0, this.FileSize);
                }
                else
                {
                    // Need to do more...
                    return bytes;
                }
            }
        }        

        #endregion InstanceMethods
    }
}