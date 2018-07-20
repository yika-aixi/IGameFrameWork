//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月20日-02:13
//Icarus.GameFramework

using System;
using System.IO;
using System.Text;
using Icarus.GameFramework;

namespace IGameFrameWork.GameFramework.Config
{
    public class ConfigLoad:IConfigLoad
    {
        public string LoadConfig(string configPath)
        {
            return File.ReadAllText(configPath);
        }

        public async void LoadConfigAsyn(string configPath,
            GameFrameworkAction<string> loadCompleteHandle = null, GameFrameworkAction<string> loadErrorHandle = null)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = File.Open(configPath, FileMode.Open);
                byte[] bytes = new byte[1024];
                StringBuilder sb = new StringBuilder();
                int count = await fileStream.ReadAsync(bytes, 0, bytes.Length);

                while (count > 0)
                {
                    sb.Append(Encoding.UTF8.GetString(bytes));
                    count = await fileStream.ReadAsync(bytes, 0, bytes.Length);
                }

                loadCompleteHandle.Handle(sb.ToString());
            }
            catch (FileNotFoundException)
            {
                loadErrorHandle.Handle($"加载失败,没有找到,路径为{configPath}的文件.");
            }
            catch (Exception ex)
            {
                loadErrorHandle.Handle($"加载失败,其他错误,路径为{configPath}.\n Message:{ex.Message}");
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }

        }

    }
}