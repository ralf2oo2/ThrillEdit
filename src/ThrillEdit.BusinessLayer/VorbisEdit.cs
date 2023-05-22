using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThrillEdit.BusinessLayer.Models;

namespace ThrillEdit.BusinessLayer
{
    public class VorbisEdit
    {
        public List<VorbisData> ExtractVorbisData(string fileName, int bufferSize)
        {
            List<VorbisData> vorbisData = new List<VorbisData>();
            long oggBegin = 0;
            long oggEnd = 0;
            bool foundBegin = false;
            bool foundEnd = false;

            byte[] buffer = new byte[bufferSize];
            byte[] headerBytes = new byte[27];
            long lastOffset = 0;
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                while (lastOffset < fs.Length)
                {
                    int bytesRead = fs.Read(buffer, 0, buffer.Length);
                    if(bytesRead == 0)
                    {
                        break;
                    }
                    for (int i = 0; i < bytesRead; i++)
                    {
                        fs.Seek(lastOffset + i, SeekOrigin.Begin);
                        fs.Read(headerBytes, 0, headerBytes.Length);

                        if (headerBytes[0] == 0x4F &&  // 'O'
                            headerBytes[1] == 0x67 &&  // 'g'
                            headerBytes[2] == 0x67 &&  // 'g'
                            headerBytes[3] == 0x53)    // 'S'
                        {
                            if (headerBytes[5] == 0x2)
                            {
                                oggBegin = lastOffset + i;
                                foundBegin = true;
                            }
                            if (foundBegin && (headerBytes[5] == 0x4 || headerBytes[5] == 0x5))
                            {
                                foundEnd = true;
                                oggEnd = lastOffset + i + 27;

                                uint trailingSize = headerBytes[26];
                                byte[] trailingFrames = new byte[trailingSize];

                                long position = fs.Position;
                                long readResult = -1;
                                long seekResult = -1;
                                seekResult = fs.Seek(oggEnd, SeekOrigin.Begin);
                                readResult = fs.Read(trailingFrames, 0, (int)trailingSize);
                                if (seekResult != -1) // Finish error check
                                {
                                    //throw new Exception();
                                }

                                fs.Seek(oggEnd, SeekOrigin.Current);

                                oggEnd += trailingSize;

                                for (long j = 0; j < trailingSize; j++)
                                {
                                    oggEnd += trailingFrames[j];
                                }
                            }
                            if (foundBegin && foundEnd)
                            {
                                foundBegin = false;
                                foundEnd = false;

                                VorbisData vorbis = new VorbisData();
                                vorbis.Origin = fileName;
                                vorbis.StartPos = oggBegin;
                                vorbis.EndPos = oggEnd - 1;
                                vorbisData.Add(vorbis);
                                Debug.WriteLine("Origin: " + fileName);
                                Debug.WriteLine("Begin: " + oggBegin);
                                Debug.WriteLine("End: " + oggEnd);
                            }
                        }

                    }
                    lastOffset += bytesRead;
                }
            }
            return vorbisData;
        }

        public List<VorbisData> ExtractVorbisDataOld(string fileName, int bufferSize)
        {
            List<VorbisData> vorbisData = new List<VorbisData>();
            long oggBegin = 0;
            long oggEnd = 0;
            bool foundBegin = false;
            bool foundEnd = false;

            byte[] buffer = new byte[bufferSize];
            byte[] headerBytes = new byte[27];
            long lastOffset = 0;
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                while (lastOffset < fs.Length)
                {
                    fs.Seek(lastOffset, SeekOrigin.Begin);
                    fs.Read(buffer, 0, buffer.Length);
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        fs.Seek(lastOffset + i, SeekOrigin.Begin);
                        fs.Read(headerBytes, 0, headerBytes.Length);
                        if (headerBytes[0] == 0x4F &&  // 'O'
                            headerBytes[1] == 0x67 &&  // 'g'
                            headerBytes[2] == 0x67 &&  // 'g'
                            headerBytes[3] == 0x53)    // 'S'
                        {
                            if (headerBytes[5] == 0x2)
                            {
                                oggBegin = lastOffset + i;
                                foundBegin = true;
                            }
                            if (foundBegin && (headerBytes[5] == 0x4 || headerBytes[5] == 0x5))
                            {
                                foundEnd = true;
                                oggEnd = lastOffset + i + 27;

                                uint trailingSize = headerBytes[26];
                                byte[] trailingFrames = new byte[trailingSize];

                                long position = fs.Position;
                                long readResult = -1;
                                long seekResult = -1;
                                seekResult = fs.Seek(oggEnd, SeekOrigin.Begin);
                                readResult = fs.Read(trailingFrames, 0, Convert.ToInt32(trailingSize));
                                if (seekResult != -1) // Finish error check
                                {

                                }

                                fs.Seek(oggEnd, SeekOrigin.Current);

                                oggEnd += trailingSize;

                                for (long j = 0; j < trailingSize; j++)
                                {
                                    oggEnd += trailingFrames[j];
                                }
                            }
                            if (foundBegin && foundEnd)
                            {
                                foundBegin = false;
                                foundEnd = false;

                                VorbisData vorbis = new VorbisData();
                                vorbis.Origin = fileName;
                                vorbis.StartPos = oggBegin;
                                vorbis.EndPos = oggEnd - 1;
                                vorbisData.Add(vorbis);
                                Debug.WriteLine("Origin: " + fileName);
                                Debug.WriteLine("Begin: " + oggBegin);
                                Debug.WriteLine("End: " + oggEnd);
                            }
                        }

                    }
                    lastOffset += buffer.Length;
                }
            }
            return vorbisData;
        }

        public void ReplaceVorbisData(List<DataReplacement> dataReplacements, string fileName, string newFile)
        {
            dataReplacements = dataReplacements.OrderBy(x => x.OriginalData.StartPos).ToList();

            using (var originFs = new FileStream(fileName, FileMode.Open))
            {
                using (var targetFs = new FileStream(newFile, FileMode.Create, FileAccess.Write))
                {
                    DataReplacement dataReplacement = dataReplacements.First();
                    originFs.Seek(0, SeekOrigin.Begin);
                    byte[] startBytes = new byte[dataReplacement.OriginalData.StartPos];
                    originFs.Read(startBytes, 0, startBytes.Length);

                    targetFs.Write(startBytes, 0, startBytes.Length);

                    while (dataReplacements.Count > 0)
                    {
                        DataReplacement currentDataReplacement = dataReplacements.First();

                        byte[] newVorbis = GetVorbisBytes(currentDataReplacement.newData);
                        targetFs.Write(newVorbis, 0, newVorbis.Length);

                        originFs.Seek(currentDataReplacement.OriginalData.EndPos + 1, SeekOrigin.Begin);

                        if (dataReplacements.Count == 1)
                        {
                            byte[] endBytes = new byte[originFs.Length - currentDataReplacement.OriginalData.EndPos];
                            originFs.Read(endBytes, 0, endBytes.Length);

                            targetFs.Write(endBytes, 0, endBytes.Length);
                        }
                        else
                        {
                            DataReplacement nextDataReplacement = dataReplacements[1];

                            byte[] middleBytes = new byte[nextDataReplacement.OriginalData.StartPos - currentDataReplacement.OriginalData.EndPos];

                            originFs.Read(middleBytes, 0, middleBytes.Length);

                            targetFs.Write(middleBytes, 0, middleBytes.Length);

                        }
                        dataReplacements.Remove(currentDataReplacement);
                    }
                }
            }
        }

        public byte[] GetVorbisBytes(VorbisData vorbisData)
        {
            long size = vorbisData.Size;
            byte[] byteData = new byte[size];
            using (var fs = new FileStream(vorbisData.Origin, FileMode.Open))
            {
                fs.Seek(Convert.ToInt32(vorbisData.StartPos), SeekOrigin.Begin);
                fs.Read(byteData, 0, byteData.Length);
            }
            return byteData;
        }

        public async Task<byte[]> GetVorbisBytesAsync(VorbisData vorbisData)
        {
            long size = vorbisData.Size;
            byte[] byteData = new byte[size];
            using (var fs = new FileStream(vorbisData.Origin, FileMode.Open))
            {
                fs.Read(byteData, Convert.ToInt32(vorbisData.StartPos), byteData.Length);
            }
            return byteData;
        }
    }
}
