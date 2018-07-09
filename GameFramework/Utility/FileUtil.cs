//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年06月10日 23:49:11
//Assembly-CSharp

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace Icarus.GameFramework
{
    public static partial class Utility
    {
        public class FileUtil
        {
            /// <summary>
            /// 获取文件长度
            /// </summary>
            /// <param name="filePath"></param>
            /// <returns></returns>
            public static long GetFileLenght(string filePath)
            {
                if (File.Exists(filePath))
                {
                    var file = File.Open(filePath, FileMode.Open);
                    var lenght = file.Length;
                    file.Close();
                    file.Dispose();
                    return lenght;
                }

                return 0;
            }

            /// <summary>
            /// 写入文件 - 同步
            /// </summary>
            /// <param name="content">写入的内容</param>
            /// <param name="path">文件路径</param>
            /// <param name="fileName">文件名</param>
            /// <param name="extensionName">扩展名,默认为:"IcarusLog"</param>
            /// <param name="errorMessage">写入错误信息,写入成功为 ""</param>
            /// <param name="offset">从什么地方开始写入</param>
            /// <param name="count">写入数量,-1为全部写入</param>
            /// <param name="fileMode">文件打开模式,默认为 追加模式</param>
            /// <returns>是否写入成功,成功: True 失败: False</returns>
            public static bool WritingFile(byte[] content, string path, string fileName, string extensionName,
                out string errorMessage, int offset = 0, int count = -1, FileMode fileMode = FileMode.Append)
            {
                var filePath = System.IO.Path.Combine(path, $"{fileName}.{extensionName}");
                return WritingFile(content, filePath, out errorMessage, offset, count, fileMode);
            }

            /// <summary>
            /// 写入文件 - 同步
            /// </summary>
            /// <param name="content">写入的内容</param>
            /// <param name="filePath">文件路径</param>
            /// <param name="errorMessage">写入错误信息,写入成功为 ""</param>
            /// <param name="offset">从什么地方开始写入</param>
            /// <param name="count">写入数量,-1为全部写入</param>
            /// <param name="fileMode">文件打开模式,默认为 追加模式</param>
            /// <returns>是否写入成功,成功: True 失败: False</returns>
            public static bool WritingFile(byte[] content, string filePath, out string errorMessage, int offset = 0,
                int count = -1, FileMode fileMode = FileMode.Append)
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    errorMessage = "filePath 是空的";
                    return false;
                }

                var dir = System.IO.Path.GetDirectoryName(filePath);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                FileStream file;
                try
                {
                    file = File.Open(filePath, fileMode);
                }
                catch (Exception e)
                {
                    errorMessage = $"打开文件失败.\n" +
                                   $"错误信息:{e.Message}\n" +
                                   $"堆栈信息:{e.StackTrace}";
                    return false;
                }

                try
                {
                    file.Write(content, offset, count == -1 ? content.Length - offset : count);
                    errorMessage = "";
                    file.Close();
                    file.Dispose();
                    return true;
                }
                catch (Exception e)
                {
                    errorMessage = $"写入错误.\n" +
                                   $"错误信息:{e.Message}\n" +
                                   $"堆栈信息:{e.StackTrace}";
                    return false;
                }
                finally
                {
                    if (file != null)
                    {
                        file.Close();
                        file.Dispose();
                    }
                }

                ;
            }

            //异步写入缓存
            static readonly Dictionary<string, FileStream> _fileAsyncs = new Dictionary<string, FileStream>();

            ///异步写入引用计数
            static readonly Dictionary<string, int> _fileAsyncsReferenceCount = new Dictionary<string, int>();

            //异步写入是否完成
            static readonly Dictionary<string, bool> _fileAsyncsComplete = new Dictionary<string, bool>();

            //异步写入队列
            static readonly Dictionary<string, Queue<FileAsyncsInfo>> _fileAsyncsQueue =
                new Dictionary<string, Queue<FileAsyncsInfo>>();

            public class FileAsyncsInfo
            {
                public FileAsyncsInfo(byte[] content, string filePath, bool isAppend, int offset, int count,
                    FileMode fileMode, Action<string> completeHandle, Action<string> errorHandle)
                {
                    Contnet = content;
                    FilePath = filePath;
                    IsAppend = isAppend;
                    OffSet = offset;
                    Count = count;
                    FileMode = fileMode;
                    CompleteHandle = completeHandle;
                    ErrorHandle = errorHandle;
                }

                public byte[] Contnet { get; }
                public string FilePath { get; }
                public bool IsAppend { get; }
                public int OffSet { get; }
                public int Count { get; }
                public FileMode FileMode { get; }
                public Action<string> CompleteHandle { get; }
                public Action<string> ErrorHandle { get; }
            }

            /// <summary>
            /// 异步-写入文件
            /// 如果频繁向一个文件写入并且isAppend为true将会等待所有的写入完成才会调用completeHandle,失败回调不受影响
            /// </summary>
            /// <param name="content">写入的内容</param>
            /// <param name="path">文件路径</param>
            /// <param name="fileName">文件名</param>
            /// <param name="extensionName">扩展名,默认为:"IcarusLog"</param>
            /// <param name="isAppend">写入追加(True : 全部写入完成才会调用completeHandle , False : 完成一次写入就调用completeHandle)</param>
            /// <param name="offSet">从什么位置开始写入</param>
            /// <param name="count">写入数量,-1 为全部写入</param>
            /// <param name="fileMode">文件模式</param>
            /// <param name="completeHandle">成功回调,参数为:文件路径</param>
            /// <param name="errorHandle">失败回调,参数为:失败信息</param>
            public static void WritingFileAsync(byte[] content, string path, string fileName, string extensionName,
                bool isAppend = false, int offSet = 0, int count = -1, FileMode fileMode = FileMode.Append,
                Action<string> completeHandle = null, Action<string> errorHandle = null)
            {
                //获取路径
                var filePath = System.IO.Path.Combine(path, $"{fileName}.{extensionName}");
                WritingFileAsync(content, filePath, isAppend, offSet, count, fileMode, completeHandle, errorHandle);
            }

            /// <summary>
            /// 异步-写入文件
            /// 如果频繁向一个文件写入并且isAppend为true将会等待所有的写入完成才会调用completeHandle,失败回调不受影响
            /// </summary>
            /// <param name="content">写入的内容</param>
            /// <param name="filePath">文件路径</param>
            /// <param name="isAppend">写入追加(True : 全部写入完成才会调用completeHandle , False : 完成一次写入就调用completeHandle)</param>
            /// <param name="offSet">从什么位置开始写入</param>
            /// <param name="count">写入数量,-1 为全部写入</param>
            /// <param name="fileMode">文件模式</param>
            /// <param name="completeHandle">成功回调,参数为:文件路径</param>
            /// <param name="errorHandle">失败回调,参数为:失败信息</param>
            public static async void WritingFileAsync(byte[] content, string filePath, bool isAppend = false,
                int offSet = 0, int count = -1, FileMode fileMode = FileMode.Append,
                Action<string> completeHandle = null, Action<string> errorHandle = null)
            {
                if (content == null || content.Length == 0)
                {
                    errorHandle?.Invoke("异步 - 写入失败,content 是空的或长度为0.");
                    return;
                }

                if (string.IsNullOrEmpty(filePath))
                {
                    errorHandle?.Invoke("异步 - 写入失败,filePath 是空的");
                    return;
                }

                FileStream fileAsync;
                //查找是否有写入实例
                if (_fileAsyncs.ContainsKey(filePath))
                {
                    //上次写入还未完成
                    if (!_fileAsyncsComplete[filePath])
                    {
                        //++实例引用次数
                        ++_fileAsyncsReferenceCount[filePath];
                        //加入队列
                        _fileAsyncsQueue[filePath].Enqueue(new FileAsyncsInfo(content, filePath, isAppend, offSet,
                            count, fileMode, completeHandle, errorHandle));
                        return;
                    }

                    //获取写入实例
                    fileAsync = _fileAsyncs[filePath];
                    //设置状态为未完成写入
                    _fileAsyncsComplete[filePath] = false;
                }
                else
                {
                    if (!System.IO.Path.IsPathRooted(filePath))
                    {
                        errorHandle?.Invoke($"文件路径格式是错误的,没有找到目录.文件路径:{filePath}");
                        return;
                    }

                    var directory = System.IO.Path.GetDirectoryName(filePath);
                    //如果目录不存在创建目录
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    try
                    {
                        fileAsync = File.Open(filePath, fileMode);
                    }
                    catch (Exception e)
                    {
                        errorHandle?.Invoke($"打开文件失败.路径为:{filePath}\n" +
                                            $"error:{e.Message}\n" +
                                            $"Stack:{e.StackTrace}");
                        return;
                    }

                    //添加写入实例
                    _fileAsyncs.Add(filePath, fileAsync);
                    //实例引用次数 1
                    _fileAsyncsReferenceCount.Add(filePath, 1);

                    _fileAsyncsComplete.Add(filePath, false);
                    _fileAsyncsQueue.Add(filePath, new Queue<FileAsyncsInfo>());
                }

                try
                {
                    //异步写入缓存
                    await fileAsync.WriteAsync(content, offSet, count == -1 ? content.Length - offSet : count);
                    //异步将缓存写入文件
                    //await _fileAsync.FlushAsync();
                    if (!isAppend)
                    {
                        completeHandle?.Invoke(filePath);
                    }
                }
                catch (Exception e)
                {
                    errorHandle?.Invoke($"写入失败.\nerror:{e.Message}\nStack:{e.StackTrace}");
                }
                finally
                {
                    //写入完成
                    _fileAsyncsComplete[filePath] = true;
                    //--实例引用次数
                    --_fileAsyncsReferenceCount[filePath];
                    UnityEngine.Debug.Log($"剩余引用次数:{_fileAsyncsReferenceCount[filePath]}");
                    //如果实例引用次数 == 0 表示没有了追加写入,进行释放 
                    if (_fileAsyncsReferenceCount[filePath] == 0)
                    {
                        fileAsync.Close();
                        fileAsync.Dispose();
                        //删除实例
                        _fileAsyncs.Remove(filePath);
                        //删除实例引用
                        _fileAsyncsReferenceCount.Remove(filePath);
                        //删除写入完成状态
                        _fileAsyncsComplete.Remove(filePath);
                        //删除写入队列
                        _fileAsyncsQueue.Remove(filePath);
                        //如果不是追加模式那全部写入完成无需调用,不然就多一次调用了
                        if (isAppend)
                        {
                            //调用写入完成回调
                            completeHandle?.Invoke(filePath);
                        }
                    }
                }

                //存在状态
                if (_fileAsyncsComplete.ContainsKey(filePath))
                {
                    //写入完成
                    if (_fileAsyncsComplete[filePath])
                    {
                        //还有没有写入的Log
                        if (_fileAsyncsQueue[filePath].Count > 0)
                        {
                            //获取Log信息
                            var fileAsyncsInfo = _fileAsyncsQueue[filePath].Dequeue();
                            WritingFileAsync(fileAsyncsInfo.Contnet, fileAsyncsInfo.FilePath, fileAsyncsInfo.IsAppend,
                                fileAsyncsInfo.OffSet, fileAsyncsInfo.Count, fileAsyncsInfo.FileMode,
                                fileAsyncsInfo.CompleteHandle, fileAsyncsInfo.ErrorHandle);
                        }
                    }
                }
            }


            private static readonly Encoding DefaultEncoding = new UTF8Encoding();

            /// <summary>
            /// 以UTF-8的编码方式,获取文件行内容
            /// </summary>
            /// <param name="filePath">文件路径</param>
            /// <param name="row">行号,从'1'开始</param>
            /// <returns>文件不存在或超出行号返回null</returns>
            public static string GetFileLinesContentUTF8(string filePath, int row)
            {
                return GetFileLinesContent(filePath, row, DefaultEncoding);
            }

            /// <summary>
            /// 获取文件行内容
            /// </summary>
            /// <param name="filePath">文件路径</param>
            /// <param name="row">行号,从'1'开始</param>
            /// <param name="encoding">编码</param>
            /// <returns>文件不存在或超出行号返回null</returns>
            public static string GetFileLinesContent(string filePath, int row, Encoding encoding)
            {
                if (!File.Exists(filePath))
                {
                    return null;
                }

                var lines = File.ReadLines(filePath, encoding).ToArray();
                if (lines.Length >= row)
                {
                    return lines[row - 1];
                }

                return null;
            }

            /// <summary>
            /// 获取文件范围内容
            /// </summary>
            /// <param name="filePath">文件路径</param>
            /// <param name="startRow">从第几行开始</param>
            /// <param name="count">获取多少行,-1或大于剩余行数为获取到全部</param>
            /// <returns>文件不存在或没有内容返回null</returns>
            public static byte[] GetFileRanageContentByte(string filePath, int startRow = 0, int count = -1)
            {
                var str = GetFileRanageContent(filePath, DefaultEncoding, startRow, count);
                if (string.IsNullOrEmpty(str))
                {
                    return null;
                }

                return Encoding.UTF8.GetBytes(str);
            }

            /// <summary>
            /// 以UTF-8的编码方式,获取文件范围内容
            /// </summary>
            /// <param name="filePath">文件路径</param>
            /// <param name="startRow">从第几行开始</param>
            /// <param name="count">获取多少行,-1或大于剩余行数为获取到全部</param>
            /// <returns>文件不存在返回null,内容为空返回Empty</returns>
            public static string GetFileRanageContentUTF8(string filePath, int startRow = 0, int count = -1)
            {
                return GetFileRanageContent(filePath, DefaultEncoding, startRow, count);
            }

            /// <summary>
            /// 获取文件范围内容
            /// </summary>
            /// <param name="filePath">文件路径</param>
            /// <param name="encoding">编码</param>
            /// <param name="startRow">从第几行开始,1开头</param>
            /// <param name="count">获取多少行,-1或大于剩余行数为获取到全部</param>
            /// <returns>文件不存在或超出行号返回null,内容为空返回Empty</returns>
            public static string GetFileRanageContent(string filePath, Encoding encoding, int startRow = 0,
                int count = -1)
            {
                if (!File.Exists(filePath))
                {
                    return null;
                }

                StringBuilder sb = new StringBuilder();
                var lines = File.ReadLines(filePath, encoding).ToArray();
                var result = lines.Skip(startRow - 1).Take(count);
                foreach (var str in result)
                {
                    sb.AppendLine(str);
                }

                return sb.ToString();
            }

            ///文件是否存在
            public static bool IsFileExists(string filePath)
            {
                return File.Exists(filePath);
            }

            /// <summary>
            /// 删除文件
            /// </summary>
            /// <param name="filePath"></param>
            public static void DeleteFile(string filePath)
            {
                if (IsFileExists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            /// <summary>
            /// 移动文件
            /// </summary>
            public static void MoveFile(string oldPath, string newPath)
            {
                if (IsFileExists(oldPath))
                {
                    File.Move(oldPath, newPath);
                }
            }

            /// <summary>
            /// 获取文件的长度
            /// </summary>
            /// <param name="savePath"></param>
            /// <returns></returns>
            private static long GetDownloadLenght(string filePath)
            {
                return FileUtil.GetFileLenght(filePath);
            }

            /// <summary>
            /// 更新路径中的目录分隔符为调用平台的符号
            /// </summary>
            /// <returns></returns>
            public static string UpdatePathDirectorySeparator(string path)
            {
                var result = path.Replace("/", System.IO.Path.DirectorySeparatorChar.ToString());
                result = result.Replace("\\", System.IO.Path.DirectorySeparatorChar.ToString());
                return result;
            }
        }
    }
}