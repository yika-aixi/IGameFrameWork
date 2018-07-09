using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;

namespace Icarus.UnityGameFramework.Runtime
{
    public static partial class Utility
    {
        public static class ZipUtil
        {
            /// <summary>
            /// 创建或覆盖压缩包
            /// </summary>
            /// <param name="outZipFilePath">压缩包输出路径</param>
            /// <param name="parentFolder">删除了目录</param>
            /// <param name="filePaths">要压缩的文件路径集合</param>
            public static void CreateZip(string outZipFilePath, string parentFolder, IEnumerable<string> filePaths)
            {
                var dir = System.IO.Path.GetDirectoryName(outZipFilePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var zipFile = File.Create(outZipFilePath);
                ZipOutputStream outputStream = new ZipOutputStream(zipFile);
                int folderOffset = parentFolder.Length;
                foreach (var filePath in filePaths)
                {
                    AddFile(filePath, outputStream, folderOffset);
                }

                //关闭zip时同时关闭文件Stream
                outputStream.IsStreamOwner = true;
                outputStream.Close();
            }

            /// <summary>
            /// 向zip添加文件
            /// </summary>
            /// <param name="zipFilePath"></param>
            /// <param name="parentFolder"></param>
            /// <param name="filePaths"></param>
            public static void UpdateZipAdd(string zipFilePath, string parentFolder, IEnumerable<string> filePaths)
            {
                ZipFile zip = new ZipFile(zipFilePath);
                int folderOffset = parentFolder.Length;
                zip.BeginUpdate();
                foreach (var filePath in filePaths)
                {
                    zip.Add(filePath, filePath.Substring(folderOffset));
                }

                zip.CommitUpdate();
                //关闭zip时同时关闭文件Stream
                zip.IsStreamOwner = true;
                zip.Close();
            }

            /// <summary>
            /// 解压压缩包,会覆盖文件
            /// </summary>
            public static void UnzipZip(string zipFilePath,string outputFolder)
            {
                var file = File.Open(zipFilePath, FileMode.Open);
                _unzipFromStream(file, outputFolder);
            }

            private static void _unzipFromStream(Stream zipStream, string outFolder)
            {
                ZipInputStream zipInputStream = new ZipInputStream(zipStream);
                ZipEntry zipEntry = zipInputStream.GetNextEntry();
                while (zipEntry != null)
                {
                    String entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    byte[] buffer = new byte[4096]; // 4K is optimum

                    // Manipulate the output filename here as desired.
                    String fullZipToPath = System.IO.Path.Combine(outFolder, entryFileName);
                    string directoryName = System.IO.Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                    {
                        if (!Directory.Exists(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }
                    }

                    // Skip directory entry
                    string fileName = System.IO.Path.GetFileName(fullZipToPath);
                    if (fileName.Length == 0)
                    {
                        zipEntry = zipInputStream.GetNextEntry();
                        continue;
                    }

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipInputStream, streamWriter, buffer);
                    }

                    zipEntry = zipInputStream.GetNextEntry();
                }

                zipInputStream.IsStreamOwner = true;
                zipInputStream.Close();
            }

            private static void AddFile(string filepath, ZipOutputStream zipStream, int folderOffset)
            {
                FileInfo fi = new FileInfo(filepath);

                string entryName = filepath.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note
                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filepath))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }

                zipStream.CloseEntry();
            }
        }
    }
}