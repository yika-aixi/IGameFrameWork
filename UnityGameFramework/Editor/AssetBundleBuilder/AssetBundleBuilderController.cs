//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2018 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using Icarus.GameFramework;
using Icarus.UnityGameFramework.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Icarus.UnityGameFramework.Editor.AssetBundleTools
{
    internal sealed partial class AssetBundleBuilderController
    {
        private const string VersionListFileName = "version";
        private const string RecordName = "GameResourceVersion";
        private const string NoneOptionName = "<None>";
        private static readonly char[] PackageListHeader = new char[] { 'E', 'L', 'P' };
        private static readonly int AssetsSubstringLength = "Assets/".Length;
        private const byte PackageListVersion = 0;
        private const int QuickEncryptLength = 220;

        private readonly string m_ConfigurationPath;
        private readonly AssetBundleCollection m_AssetBundleCollection;
        private readonly AssetBundleAnalyzerController m_AssetBundleAnalyzerController;
        private readonly SortedDictionary<string, AssetBundleData> m_AssetBundleDatas;
        private readonly Dictionary<BuildTarget, VersionListData> m_VersionListDatas;
        private readonly BuildReport m_BuildReport;
        private readonly List<string> m_BuildEventHandlerTypeNames;
        private IBuildEventHandler m_BuildEventHandler;

        public AssetBundleBuilderController()
        {
            m_ConfigurationPath = Type.GetConfigurationPath<AssetBundleBuilderConfigPathAttribute>() ?? Icarus.GameFramework.Utility.Path.GetCombinePath(Application.dataPath, "GameFramework/Configs/AssetBundleBuilder.xml");

            m_AssetBundleCollection = new AssetBundleCollection();

            m_AssetBundleCollection.OnLoadingAssetBundle += delegate (int index, int count)
            {
                if (OnLoadingAssetBundle != null)
                {
                    OnLoadingAssetBundle(index, count);
                }
            };

            m_AssetBundleCollection.OnLoadingAsset += delegate (int index, int count)
            {
                if (OnLoadingAsset != null)
                {
                    OnLoadingAsset(index, count);
                }
            };

            m_AssetBundleCollection.OnLoadCompleted += delegate ()
            {
                if (OnLoadCompleted != null)
                {
                    OnLoadCompleted();
                }
            };

            m_AssetBundleAnalyzerController = new AssetBundleAnalyzerController(m_AssetBundleCollection);

            m_AssetBundleAnalyzerController.OnAnalyzingAsset += delegate (int index, int count)
            {
                if (OnAnalyzingAsset != null)
                {
                    OnAnalyzingAsset(index, count);
                }
            };

            m_AssetBundleAnalyzerController.OnAnalyzeCompleted += delegate ()
            {
                if (OnAnalyzeCompleted != null)
                {
                    OnAnalyzeCompleted();
                }
            };

            m_AssetBundleDatas = new SortedDictionary<string, AssetBundleData>();
            m_VersionListDatas = new Dictionary<BuildTarget, VersionListData>();
            m_BuildReport = new BuildReport();

            m_BuildEventHandlerTypeNames = new List<string>();
            m_BuildEventHandlerTypeNames.Add(NoneOptionName);
            m_BuildEventHandlerTypeNames.AddRange(Type.GetEditorTypeNames(typeof(IBuildEventHandler)));
            m_BuildEventHandler = null;

            WindowsSelected = MacOSXSelected = IOSSelected = AndroidSelected = WindowsStoreSelected = true;
            RecordScatteredDependencyAssetsSelected = false;
            DeterministicAssetBundleSelected = ChunkBasedCompressionSelected = true;
            UncompressedAssetBundleSelected = DisableWriteTypeTreeSelected = ForceRebuildAssetBundleSelected = IgnoreTypeTreeChangesSelected = AppendHashToAssetBundleNameSelected = false;
            BuildEventHandlerTypeName = string.Empty;
            OutputDirectory = string.Empty;
        }

        public string ProductName
        {
            get
            {
                return PlayerSettings.productName;
            }
        }

        public string CompanyName
        {
            get
            {
                return PlayerSettings.companyName;
            }
        }

        public string GameIdentifier
        {
            get
            {
#if UNITY_5_6_OR_NEWER
                return PlayerSettings.applicationIdentifier;
#else
                return PlayerSettings.bundleIdentifier;
#endif
            }
        }

        public string ApplicableGameVersion
        {
            get
            {
                return Application.version;
            }
        }

        public int InternalResourceVersion
        {
            get;
            set;
        }

        public string UnityVersion
        {
            get
            {
                return Application.unityVersion;
            }
        }

        public bool WindowsSelected
        {
            get;
            set;
        }

        public bool MacOSXSelected
        {
            get;
            set;
        }

        public bool IOSSelected
        {
            get;
            set;
        }

        public bool AndroidSelected
        {
            get;
            set;
        }

        public bool WindowsStoreSelected
        {
            get;
            set;
        }
        
        public bool RecordScatteredDependencyAssetsSelected
        {
            get;
            set;
        }

        public bool UncompressedAssetBundleSelected
        {
            get;
            set;
        }

        public bool DisableWriteTypeTreeSelected
        {
            get;
            set;
        }

        public bool DeterministicAssetBundleSelected
        {
            get;
            set;
        }

        public bool ForceRebuildAssetBundleSelected
        {
            get;
            set;
        }

        public bool IgnoreTypeTreeChangesSelected
        {
            get;
            set;
        }

        public bool AppendHashToAssetBundleNameSelected
        {
            get;
            set;
        }

        public bool ChunkBasedCompressionSelected
        {
            get;
            set;
        }

        public bool IsCopyStreamingAssets
        {
            get;
            set;
        }

        public string BuildEventHandlerTypeName
        {
            get;
            set;
        }

        public string OutputDirectory
        {
            get;
            set;
        }

        public bool IsValidOutputDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(OutputDirectory))
                {
                    return false;
                }

                if (!Directory.Exists(OutputDirectory))
                {
                    return false;
                }

                return true;
            }
        }

        public string WorkingPath
        {
            get
            {
                if (!IsValidOutputDirectory)
                {
                    return string.Empty;
                }

                return string.Format("{0}/Working/", OutputDirectory);
            }
        }

        public string OutputPackagePath
        {
            get
            {
                if (!IsValidOutputDirectory)
                {
                    return string.Empty;
                }

                return string.Format("{0}/Package/{1}_{2}/", OutputDirectory, ApplicableGameVersion.Replace('.', '_'), InternalResourceVersion.ToString());
            }
        }
      
        public string OutputZipPath
        {
            get
            {
                if (!IsValidOutputDirectory)
                {
                    return string.Empty;
                }

                return string.Format("{0}/Zip/{1}_{2}/", OutputDirectory, ApplicableGameVersion.Replace('.', '_'), InternalResourceVersion.ToString());
            }
        }

        public string BuildReportPath
        {
            get
            {
                if (!IsValidOutputDirectory)
                {
                    return string.Empty;
                }

                return string.Format("{0}/BuildReport/{1}_{2}/", OutputDirectory, ApplicableGameVersion.Replace('.', '_'), InternalResourceVersion.ToString());
            }
        }

        public event GameFrameworkAction<int, int> OnLoadingAssetBundle = null;

        public event GameFrameworkAction<int, int> OnLoadingAsset = null;

        public event GameFrameworkAction OnLoadCompleted = null;

        public event GameFrameworkAction<int, int> OnAnalyzingAsset = null;

        public event GameFrameworkAction OnAnalyzeCompleted = null;

        public event GameFrameworkFunc<string, float, bool> ProcessingAssetBundle = null;

        public event GameFrameworkAction<BuildTarget, string, int, int, int, int> ProcessAssetBundleComplete = null;

        public event GameFrameworkAction<string> BuildAssetBundlesError = null;

        public bool Load()
        {
            if (!File.Exists(m_ConfigurationPath))
            {
                return false;
            }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(m_ConfigurationPath);
                XmlNode xmlRoot = xmlDocument.SelectSingleNode("UnityGameFramework");
                XmlNode xmlEditor = xmlRoot.SelectSingleNode("AssetBundleBuilder");
                XmlNode xmlSettings = xmlEditor.SelectSingleNode("Settings");

                XmlNodeList xmlNodeList = null;
                XmlNode xmlNode = null;

                xmlNodeList = xmlSettings.ChildNodes;
                for (int i = 0; i < xmlNodeList.Count; i++)
                {
                    xmlNode = xmlNodeList.Item(i);
                    switch (xmlNode.Name)
                    {
                        case "InternalResourceVersion":
                            InternalResourceVersion = int.Parse(xmlNode.InnerText) + 1;
                            break;
                        case "WindowsSelected":
                            WindowsSelected = bool.Parse(xmlNode.InnerText);
                            break;
                        case "MacOSXSelected":
                            MacOSXSelected = bool.Parse(xmlNode.InnerText);
                            break;
                        case "IOSSelected":
                            IOSSelected = bool.Parse(xmlNode.InnerText);
                            break;
                        case "AndroidSelected":
                            AndroidSelected = bool.Parse(xmlNode.InnerText);
                            break;
                        case "WindowsStoreSelected":
                            WindowsStoreSelected = bool.Parse(xmlNode.InnerText);
                            break;
                        case "RecordScatteredDependencyAssetsSelected":
                            RecordScatteredDependencyAssetsSelected = bool.Parse(xmlNode.InnerText);
                            break;
                        case "UncompressedAssetBundleSelected":
                            UncompressedAssetBundleSelected = bool.Parse(xmlNode.InnerText);
                            if (UncompressedAssetBundleSelected)
                            {
                                ChunkBasedCompressionSelected = false;
                            }
                            break;
                        case "DisableWriteTypeTreeSelected":
                            DisableWriteTypeTreeSelected = bool.Parse(xmlNode.InnerText);
                            if (DisableWriteTypeTreeSelected)
                            {
                                IgnoreTypeTreeChangesSelected = false;
                            }
                            break;
                        case "DeterministicAssetBundleSelected":
                            DeterministicAssetBundleSelected = bool.Parse(xmlNode.InnerText);
                            break;
                        case "ForceRebuildAssetBundleSelected":
                            ForceRebuildAssetBundleSelected = bool.Parse(xmlNode.InnerText);
                            break;
                        case "IgnoreTypeTreeChangesSelected":
                            IgnoreTypeTreeChangesSelected = bool.Parse(xmlNode.InnerText);
                            if (IgnoreTypeTreeChangesSelected)
                            {
                                DisableWriteTypeTreeSelected = false;
                            }
                            break;
                        case "AppendHashToAssetBundleNameSelected":
                            AppendHashToAssetBundleNameSelected = false;
                            break;
                        case "ChunkBasedCompressionSelected":
                            ChunkBasedCompressionSelected = bool.Parse(xmlNode.InnerText);
                            if (ChunkBasedCompressionSelected)
                            {
                                UncompressedAssetBundleSelected = false;
                            }
                            break;
                        case "BuildEventHandlerTypeName":
                            BuildEventHandlerTypeName = xmlNode.InnerText;
                            RefreshBuildEventHandler();
                            break;
                        case "OutputDirectory":
                            OutputDirectory = xmlNode.InnerText;
                            break;
                        case "CopyStreamingAssets":
                            IsCopyStreamingAssets = bool.Parse(xmlNode.InnerText);
                            break;
                    }
                }
            }
            catch
            {
                File.Delete(m_ConfigurationPath);
                return false;
            }

            return true;
        }

        public bool Save()
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));

                XmlElement xmlRoot = xmlDocument.CreateElement("UnityGameFramework");
                xmlDocument.AppendChild(xmlRoot);

                XmlElement xmlBuilder = xmlDocument.CreateElement("AssetBundleBuilder");
                xmlRoot.AppendChild(xmlBuilder);

                XmlElement xmlSettings = xmlDocument.CreateElement("Settings");
                xmlBuilder.AppendChild(xmlSettings);

                XmlElement xmlElement = null;

                xmlElement = xmlDocument.CreateElement("InternalResourceVersion");
                xmlElement.InnerText = InternalResourceVersion.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("WindowsSelected");
                xmlElement.InnerText = WindowsSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("MacOSXSelected");
                xmlElement.InnerText = MacOSXSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("IOSSelected");
                xmlElement.InnerText = IOSSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("AndroidSelected");
                xmlElement.InnerText = AndroidSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("WindowsStoreSelected");
                xmlElement.InnerText = WindowsStoreSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("RecordScatteredDependencyAssetsSelected");
                xmlElement.InnerText = RecordScatteredDependencyAssetsSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("UncompressedAssetBundleSelected");
                xmlElement.InnerText = UncompressedAssetBundleSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("DisableWriteTypeTreeSelected");
                xmlElement.InnerText = DisableWriteTypeTreeSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("DeterministicAssetBundleSelected");
                xmlElement.InnerText = DeterministicAssetBundleSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("ForceRebuildAssetBundleSelected");
                xmlElement.InnerText = ForceRebuildAssetBundleSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("IgnoreTypeTreeChangesSelected");
                xmlElement.InnerText = IgnoreTypeTreeChangesSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("AppendHashToAssetBundleNameSelected");
                xmlElement.InnerText = AppendHashToAssetBundleNameSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("ChunkBasedCompressionSelected");
                xmlElement.InnerText = ChunkBasedCompressionSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("BuildEventHandlerTypeName");
                xmlElement.InnerText = BuildEventHandlerTypeName;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("OutputDirectory");
                xmlElement.InnerText = OutputDirectory;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("CopyStreamingAssets");
                xmlElement.InnerText = IsCopyStreamingAssets.ToString();
                xmlSettings.AppendChild(xmlElement);

                string configurationDirectoryName = Path.GetDirectoryName(m_ConfigurationPath);
                if (!Directory.Exists(configurationDirectoryName))
                {
                    Directory.CreateDirectory(configurationDirectoryName);
                }

                xmlDocument.Save(m_ConfigurationPath);
                AssetDatabase.Refresh();
                return true;
            }
            catch
            {
                if (File.Exists(m_ConfigurationPath))
                {
                    File.Delete(m_ConfigurationPath);
                }

                return false;
            }
        }

        public void SetBuildEventHandler(IBuildEventHandler buildEventHandler)
        {
            m_BuildEventHandler = buildEventHandler;
        }

        public string[] GetBuildEventHandlerTypeNames()
        {
            return m_BuildEventHandlerTypeNames.ToArray();
        }

        public bool RefreshBuildEventHandler()
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(BuildEventHandlerTypeName) && m_BuildEventHandlerTypeNames.Contains(BuildEventHandlerTypeName))
            {
                System.Type buildEventHandlerType = Icarus.GameFramework.Utility.Assembly.GetType(BuildEventHandlerTypeName);
                if (buildEventHandlerType != null)
                {
                    IBuildEventHandler buildEventHandler = (IBuildEventHandler)Activator.CreateInstance(buildEventHandlerType);
                    if (buildEventHandler != null)
                    {
                        SetBuildEventHandler(buildEventHandler);
                        return true;
                    }
                }
            }
            else
            {
                retVal = true;
            }

            BuildEventHandlerTypeName = string.Empty;
            SetBuildEventHandler(null);
            return retVal;
        }

        public bool BuildAssetBundles()
        {
            //初始化版本文件信息
            _initVersion(string.Format("{0}({1})", ApplicableGameVersion, InternalResourceVersion));

            if (!IsValidOutputDirectory)
            {
                return false;
            }

            Icarus.GameFramework.Utility.Zip.SetZipHelper(new DefaultZipHelper());

            if (Directory.Exists(OutputPackagePath))
            {
                Directory.Delete(OutputPackagePath, true);
            }

            Directory.CreateDirectory(OutputPackagePath);

            if (Directory.Exists(BuildReportPath))
            {
                Directory.Delete(BuildReportPath, true);
            }

            Directory.CreateDirectory(BuildReportPath);

            if (Directory.Exists(OutputZipPath))
            {
                Directory.Delete(OutputZipPath, true);
            }

            Directory.CreateDirectory(OutputZipPath);

            BuildAssetBundleOptions buildAssetBundleOptions = GetBuildAssetBundleOptions();
            m_BuildReport.Initialize(BuildReportPath, ProductName, CompanyName, GameIdentifier, ApplicableGameVersion, InternalResourceVersion, UnityVersion,
                WindowsSelected, MacOSXSelected, IOSSelected, AndroidSelected, WindowsStoreSelected, RecordScatteredDependencyAssetsSelected, (int)buildAssetBundleOptions, m_AssetBundleDatas);
            
            try
            {
                m_VersionListDatas.Clear();

                m_BuildReport.LogInfo("Build Start Time: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                if (m_BuildEventHandler != null)
                {
                    m_BuildReport.LogInfo("Execute build event handler 'PreProcessBuildAll'...");
                    m_BuildEventHandler.PreProcessBuildAll(
                        ProductName, CompanyName, GameIdentifier, ApplicableGameVersion, 
                        InternalResourceVersion, UnityVersion, buildAssetBundleOptions, 
                        OutputDirectory, WorkingPath, OutputPackagePath, 
                        OutputZipPath, BuildReportPath);
                }

                m_BuildReport.LogInfo("Start prepare AssetBundle collection...");
                if (!m_AssetBundleCollection.Load())
                {
                    m_BuildReport.LogError("Can not parse 'AssetBundleCollection.xml', please use 'AssetBundle Editor' tool first.");
                    m_BuildReport.SaveReport();
                    return false;
                }

                m_BuildReport.LogInfo("Prepare AssetBundle collection complete.");
                m_BuildReport.LogInfo("Start analyze assets dependency...");

                m_AssetBundleAnalyzerController.Analyze();

                m_BuildReport.LogInfo("Analyze assets dependency complete.");
                m_BuildReport.LogInfo("Start prepare build map...");

                AssetBundleBuild[] buildMap = GetBuildMap();
                if (buildMap == null || buildMap.Length <= 0)
                {
                    m_BuildReport.LogError("Build map is empty.");
                    m_BuildReport.SaveReport();
                    return false;
                }

                m_BuildReport.LogInfo("Prepare build map complete.");
                m_BuildReport.LogInfo("Start build AssetBundles for selected build targets...");

                if (WindowsSelected)
                {
                    BuildAssetBundles(buildMap, buildAssetBundleOptions, BuildTarget.StandaloneWindows);
                }

                if (MacOSXSelected)
                {
#if UNITY_2017_3_OR_NEWER
                    BuildTarget buildTarget = BuildTarget.StandaloneOSX;
#else
                    BuildTarget buildTarget = BuildTarget.StandaloneOSXUniversal;
#endif
                    BuildAssetBundles(buildMap, buildAssetBundleOptions,buildTarget);
                }

                if (IOSSelected)
                {
                    BuildAssetBundles(buildMap, buildAssetBundleOptions, BuildTarget.iOS);
                }

                if (AndroidSelected)
                {
                    BuildAssetBundles(buildMap, buildAssetBundleOptions, BuildTarget.Android);
                }

                if (WindowsStoreSelected)
                {
                    BuildAssetBundles(buildMap, buildAssetBundleOptions, BuildTarget.WSAPlayer);
                }

                ProcessRecord(OutputDirectory);

                if (m_BuildEventHandler != null)
                {
                    m_BuildReport.LogInfo("Execute build event handler 'PostProcessBuildAll'...");
                    m_BuildEventHandler.PostProcessBuildAll(ProductName, CompanyName, GameIdentifier, 
                        ApplicableGameVersion, InternalResourceVersion, UnityVersion, 
                        buildAssetBundleOptions, OutputDirectory, WorkingPath,
                        OutputPackagePath, OutputZipPath, BuildReportPath);
                }

                m_BuildReport.LogInfo("Build AssetBundles for selected build targets complete.");
                m_BuildReport.SaveReport();
                return true;
            }
            catch (Exception exception)
            {
                m_BuildReport.LogError(exception.Message);
                m_BuildReport.SaveReport();
                if (BuildAssetBundlesError != null)
                {
                    BuildAssetBundlesError(exception.Message);
                }

                throw exception;

                return false;
            }
        }

        private void BuildAssetBundles(AssetBundleBuild[] buildMap, BuildAssetBundleOptions buildOptions, BuildTarget buildTarget)
        {
            m_BuildReport.LogInfo("Start build AssetBundles for '{0}'...", buildTarget.ToString());

            string buildTargetUrlName = GetBuildTargetName(buildTarget);

            string workingPath = string.Format("{0}{1}/", WorkingPath, buildTargetUrlName);
            m_BuildReport.LogInfo("Working path is '{0}'.", workingPath);

            string outputPackagePath = string.Format("{0}{1}/", OutputPackagePath, buildTargetUrlName);
            Directory.CreateDirectory(outputPackagePath);
            m_BuildReport.LogInfo("Output package path is '{0}'.", outputPackagePath);

            string outputZipPath = string.Format("{0}{1}/", OutputZipPath, buildTargetUrlName);
            Directory.CreateDirectory(outputZipPath);
            m_BuildReport.LogInfo("output Zip Path is '{0}'.", outputZipPath);

            // Clean working path
            List<string> validNames = new List<string>();
            foreach (AssetBundleBuild i in buildMap)
            {
                string assetBundleName = GetAssetBundleFullName(i.assetBundleName, i.assetBundleVariant);
                validNames.Add(assetBundleName);
            }

            if (Directory.Exists(workingPath))
            {
                Uri workingUri = new Uri(workingPath, UriKind.RelativeOrAbsolute);
                string[] fileNames = Directory.GetFiles(workingPath, "*", SearchOption.AllDirectories);
                foreach (string fileName in fileNames)
                {
                    if (fileName.EndsWith(".manifest"))
                    {
                        continue;
                    }

                    string relativeName = workingUri.MakeRelativeUri(new Uri(fileName)).ToString();
                    if (!validNames.Contains(relativeName))
                    {
                        File.Delete(fileName);
                    }
                }

                string[] manifestNames = Directory.GetFiles(workingPath, "*.manifest", SearchOption.AllDirectories);
                foreach (string manifestName in manifestNames)
                {
                    if (!File.Exists(manifestName.Substring(0, manifestName.LastIndexOf('.'))))
                    {
                        File.Delete(manifestName);
                    }
                }

                Icarus.GameFramework.Utility.Path.RemoveEmptyDirectory(workingPath);
            }

            if (!Directory.Exists(workingPath))
            {
                Directory.CreateDirectory(workingPath);
            }

            if (m_BuildEventHandler != null)
            {
                m_BuildReport.LogInfo("Execute build event handler 'PreProcessBuild' for '{0}'...", buildTarget.ToString());
                m_BuildEventHandler.PreProcessBuild(buildTarget, workingPath, outputPackagePath,outputZipPath);
            }

            // Build AssetBundles
            m_BuildReport.LogInfo("Unity start build AssetBundles for '{0}'...", buildTarget.ToString());
            AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(workingPath, buildMap, buildOptions, buildTarget);
            if (assetBundleManifest == null)
            {
                m_BuildReport.LogError("Build AssetBundles for '{0}' failure.", buildTarget.ToString());
                return;
            }

            m_BuildReport.LogInfo("Unity build AssetBundles for '{0}' complete.", buildTarget.ToString());

            // Process AssetBundles
            for (int i = 0; i < buildMap.Length; i++)
            {
                string assetBundleFullName = GetAssetBundleFullName(buildMap[i].assetBundleName, buildMap[i].assetBundleVariant);
                if (ProcessingAssetBundle != null)
                {
                    if (ProcessingAssetBundle(assetBundleFullName, (float)(i + 1) / buildMap.Length))
                    {
                        m_BuildReport.LogWarning("The build has been canceled by user.");
                        return;
                    }
                }

                m_BuildReport.LogInfo("Start process '{0}' for '{1}'...", assetBundleFullName, buildTarget.ToString());

                ProcessAssetBundle(workingPath, outputPackagePath, outputZipPath, buildTarget, buildMap[i].assetBundleName, buildMap[i].assetBundleVariant);

                m_BuildReport.LogInfo("Process '{0}' for '{1}' complete.", assetBundleFullName, buildTarget.ToString());
            }

            //写入version.info
            _writeVersionInfoFile(outputZipPath);

            ProcessPackageList(outputPackagePath, buildTarget);
            m_BuildReport.LogInfo("Process package list for '{0}' complete.", buildTarget.ToString());

            if (m_BuildEventHandler != null)
            {
                m_BuildReport.LogInfo("Execute build event handler 'PostProcessBuild' for '{0}'...", buildTarget.ToString());
                m_BuildEventHandler.PostProcessBuild(buildTarget, workingPath, outputPackagePath,outputZipPath);
            }

            if (ProcessAssetBundleComplete != null)
            {
                ProcessAssetBundleComplete(buildTarget, "", 0, 0,0, 0);
            }

            m_BuildReport.LogInfo("Build AssetBundles for '{0}' success.", buildTarget.ToString());
        }
        //todo 修改的函数
        private void ProcessAssetBundle(string workingPath, string outputPackagePath, string outputFullPath, BuildTarget buildTarget, string assetBundleName, string assetBundleVariant)
        {
            string assetBundleFullName = GetAssetBundleFullName(assetBundleName, assetBundleVariant);
            AssetBundleData assetBundleData = m_AssetBundleDatas[assetBundleFullName];
            string workingName = Icarus.GameFramework.Utility.Path.GetCombinePath(workingPath, assetBundleFullName);

            byte[] bytes = File.ReadAllBytes(workingName);
            int length = bytes.Length;
            byte[] hashBytes = Icarus.GameFramework.Utility.Verifier.GetCrc32(bytes);
            int hashCode = Icarus.GameFramework.Utility.Converter.GetInt32(hashBytes);

            if (assetBundleData.LoadType == AssetBundleLoadType.LoadFromMemoryAndQuickDecrypt)
            {
                bytes = GetQuickXorBytes(bytes, hashBytes);
            }
            else if (assetBundleData.LoadType == AssetBundleLoadType.LoadFromMemoryAndDecrypt)
            {
                bytes = GetXorBytes(bytes, hashBytes);
            }

            // Package AssetBundle
            string packageName = Icarus.GameFramework.Utility.Path.GetResourceNameWithSuffix(Icarus.GameFramework.Utility.Path.GetCombinePath(outputPackagePath, assetBundleFullName));
            string packageDirectoryName = Path.GetDirectoryName(packageName);
            if (!Directory.Exists(packageDirectoryName))
            {
                Directory.CreateDirectory(packageDirectoryName);
            }

            File.WriteAllBytes(packageName, bytes);

            #region Zip

            var zipFilePath = Path.Combine(outputFullPath, assetBundleFullName + ".zip");
            Runtime.Utility.ZipUtil.CreateZip(zipFilePath, outputPackagePath,new string[]{ packageName });
            //更新或加入资源包信息
            _addOrUpdateAsssetBundle(outputPackagePath,assetBundleData);

            #endregion

            int zipLength = length;
            int zipHashCode = hashCode;

            assetBundleData.AddCode(buildTarget, length, hashCode, zipLength, zipHashCode);
        }
        //todo 修改的函数
        private void ProcessPackageList(string outputPackagePath, BuildTarget buildTarget)
        {
            byte[] encryptBytes = new byte[4];
            Icarus.GameFramework.Utility.Random.GetRandomBytes(encryptBytes);

            if (m_AssetBundleDatas.Count > ushort.MaxValue)
            {
                throw new GameFrameworkException("Package list can only contains 65535 resources in version 0.");
            }
            foreach (AssetBundleData assetBundleData in m_AssetBundleDatas.Values)
            {
                var abName = assetBundleData.Name.Split('/').Last();
                string AbpackageListPath =
                    Icarus.GameFramework.Utility.Path.GetCombinePath(outputPackagePath,
                        string.Format("{0}_{1}~{2}", abName, assetBundleData.Variant, VersionListFileName));
                using (FileStream fileStream = new FileStream(AbpackageListPath, FileMode.CreateNew, FileAccess.Write))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                    {
                        binaryWriter.Write(PackageListHeader);
                        binaryWriter.Write(PackageListVersion);
                        binaryWriter.Write(encryptBytes);

                        byte[] applicableGameVersionBytes = GetXorBytes(Icarus.GameFramework.Utility.Converter.GetBytes(ApplicableGameVersion), encryptBytes);
                        binaryWriter.Write((byte)applicableGameVersionBytes.Length);
                        binaryWriter.Write(applicableGameVersionBytes);
                        binaryWriter.Write(InternalResourceVersion);
                        if (m_AssetBundleDatas.Count > ushort.MaxValue)
                        {
                            throw new GameFrameworkException("Package list can only contains 65535 resources in version 0.");
                        }
                        
                        byte[] nameBytes = GetXorBytes(Icarus.GameFramework.Utility.Converter.GetBytes(assetBundleData.Name), encryptBytes);
                        
                        if (nameBytes.Length > byte.MaxValue)
                        {
                            throw new GameFrameworkException(string.Format("AssetBundle name '{0}' is too long.", assetBundleData.Name));
                        }
                        binaryWriter.Write((byte)nameBytes.Length);
                        binaryWriter.Write(nameBytes);

                        if (assetBundleData.Variant == null)
                        {
                            binaryWriter.Write((byte)0);
                        }
                        else
                        {
                            byte[] variantBytes = GetXorBytes(Icarus.GameFramework.Utility.Converter.GetBytes(assetBundleData.Variant), encryptBytes);
                            if (variantBytes.Length > byte.MaxValue)
                            {
                                throw new GameFrameworkException(string.Format("AssetBundle variant '{0}' is too long.", assetBundleData.Variant));
                            }

                            binaryWriter.Write((byte)variantBytes.Length);
                            binaryWriter.Write(variantBytes);
                        }

                        binaryWriter.Write((byte)assetBundleData.LoadType);
                        AssetBundleCode assetBundleCode = assetBundleData.GetCode(buildTarget);
                        binaryWriter.Write(assetBundleCode.Length);
                        binaryWriter.Write(assetBundleCode.HashCode);

                        string[] assetNames = assetBundleData.GetAssetNames();
                        binaryWriter.Write(assetNames.Length);
                        foreach (string assetName in assetNames)
                        {
                            byte[] assetNameBytes = GetXorBytes(Icarus.GameFramework.Utility.Converter.GetBytes(assetName), Icarus.GameFramework.Utility.Converter.GetBytes(assetBundleCode.HashCode));
                            if (assetNameBytes.Length > byte.MaxValue)
                            {
                                throw new GameFrameworkException(string.Format("Asset name '{0}' is too long.", assetName));
                            }

                            binaryWriter.Write((byte)assetNameBytes.Length);
                            binaryWriter.Write(assetNameBytes);

                            AssetData assetData = assetBundleData.GetAssetData(assetName);
                            string[] dependencyAssetNames = assetData.GetDependencyAssetNames();
                            binaryWriter.Write(dependencyAssetNames.Length);
                            foreach (string dependencyAssetName in dependencyAssetNames)
                            {
                                byte[] dependencyAssetNameBytes = GetXorBytes(Icarus.GameFramework.Utility.Converter.GetBytes(dependencyAssetName), Icarus.GameFramework.Utility.Converter.GetBytes(assetBundleCode.HashCode));
                                if (dependencyAssetNameBytes.Length > byte.MaxValue)
                                {
                                    throw new GameFrameworkException(string.Format("Dependency asset name '{0}' is too long.", dependencyAssetName));
                                }

                                binaryWriter.Write((byte)dependencyAssetNameBytes.Length);
                                binaryWriter.Write(dependencyAssetNameBytes);
                            }
                        }


                        // TODO: Resource group.
                        binaryWriter.Write(0);

                        binaryWriter.Close();
                    }
                }

                var abVersionPath = Icarus.GameFramework.Utility.Path.GetResourceNameWithSuffix(AbpackageListPath);
                File.Move(AbpackageListPath, abVersionPath);

                #region Zip

                var zipPath = outputPackagePath.Replace(OutputPackagePath, OutputZipPath);
                var abFullName = GetAssetBundleFullName(assetBundleData.Name, assetBundleData.Variant);
                var zipFilePath = Path.Combine(zipPath, abFullName + ".zip");
                Runtime.Utility.ZipUtil.UpdateZipAdd(zipFilePath, outputPackagePath,new []
                {
                    abVersionPath,
                });
                #endregion
            }
        }
        private void ProcessRecord(string outputRecordPath)
        {
            string recordPath = Icarus.GameFramework.Utility.Path.GetCombinePath(outputRecordPath, string.Format("{0}_{1}.xml", RecordName, ApplicableGameVersion.Replace('.', '_')));

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));

            XmlAttribute xmlAttribute = null;
            XmlElement xmlRoot = xmlDocument.CreateElement("ResourceVersionInfo");
            xmlAttribute = xmlDocument.CreateAttribute("ApplicableGameVersion");
            xmlAttribute.Value = ApplicableGameVersion.ToString();
            xmlRoot.Attributes.SetNamedItem(xmlAttribute);
            xmlAttribute = xmlDocument.CreateAttribute("LatestInternalResourceVersion");
            xmlAttribute.Value = InternalResourceVersion.ToString();
            xmlRoot.Attributes.SetNamedItem(xmlAttribute);
            xmlDocument.AppendChild(xmlRoot);

            XmlElement xmlElement = null;
            foreach (KeyValuePair<BuildTarget, VersionListData> i in m_VersionListDatas)
            {
                xmlElement = xmlDocument.CreateElement(i.Key.ToString());
                xmlAttribute = xmlDocument.CreateAttribute("Length");
                xmlAttribute.Value = i.Value.Length.ToString();
                xmlElement.Attributes.SetNamedItem(xmlAttribute);
                xmlAttribute = xmlDocument.CreateAttribute("HashCode");
                xmlAttribute.Value = i.Value.HashCode.ToString();
                xmlElement.Attributes.SetNamedItem(xmlAttribute);
                xmlAttribute = xmlDocument.CreateAttribute("ZipLength");
                xmlAttribute.Value = i.Value.ZipLength.ToString();
                xmlElement.Attributes.SetNamedItem(xmlAttribute);
                xmlAttribute = xmlDocument.CreateAttribute("ZipHashCode");
                xmlAttribute.Value = i.Value.ZipHashCode.ToString();
                xmlElement.Attributes.SetNamedItem(xmlAttribute);

                xmlRoot.AppendChild(xmlElement);
            }

            xmlDocument.Save(recordPath);
        }

        private BuildAssetBundleOptions GetBuildAssetBundleOptions()
        {
            BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.None;

            if (UncompressedAssetBundleSelected)
            {
                buildOptions |= BuildAssetBundleOptions.UncompressedAssetBundle;
            }

            if (DisableWriteTypeTreeSelected)
            {
                buildOptions |= BuildAssetBundleOptions.DisableWriteTypeTree;
            }

            if (DeterministicAssetBundleSelected)
            {
                buildOptions |= BuildAssetBundleOptions.DeterministicAssetBundle;
            }

            if (ForceRebuildAssetBundleSelected)
            {
                buildOptions |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            }

            if (IgnoreTypeTreeChangesSelected)
            {
                buildOptions |= BuildAssetBundleOptions.IgnoreTypeTreeChanges;
            }

            if (AppendHashToAssetBundleNameSelected)
            {
                buildOptions |= BuildAssetBundleOptions.AppendHashToAssetBundleName;
            }

            if (ChunkBasedCompressionSelected)
            {
                buildOptions |= BuildAssetBundleOptions.ChunkBasedCompression;
            }

            return buildOptions;
        }

        private AssetBundleBuild[] GetBuildMap()
        {
            m_AssetBundleDatas.Clear();

            AssetBundle[] assetBundles = m_AssetBundleCollection.GetAssetBundles();
            foreach (AssetBundle assetBundle in assetBundles)
            {
                m_AssetBundleDatas.Add(assetBundle.FullName.ToLower(), new AssetBundleData(assetBundle.Name.ToLower(), 
                    (assetBundle.Variant != null ? assetBundle.Variant.ToLower() : null), 
                    assetBundle.LoadType, assetBundle.Optional,assetBundle.GroupTag));
            }

            Asset[] assets = m_AssetBundleCollection.GetAssets();
            foreach (Asset asset in assets)
            {
                string assetName = asset.Name;
                if (string.IsNullOrEmpty(assetName))
                {
                    m_BuildReport.LogError("Can not find asset by guid '{0}'.", asset.Guid);
                    return null;
                }

                string assetFileFullName = Icarus.GameFramework.Utility.Path.GetCombinePath(Application.dataPath, assetName.Substring(AssetsSubstringLength));
                if (!File.Exists(assetFileFullName))
                {
                    m_BuildReport.LogError("Can not find asset '{0}'.", assetFileFullName);
                    return null;
                }

                byte[] assetBytes = File.ReadAllBytes(assetFileFullName);
                int assetHashCode = Icarus.GameFramework.Utility.Converter.GetInt32(Icarus.GameFramework.Utility.Verifier.GetCrc32(assetBytes));

                List<string> dependencyAssetNames = new List<string>();
                DependencyData dependencyData = m_AssetBundleAnalyzerController.GetDependencyData(assetName);
                Asset[] dependencyAssets = dependencyData.GetDependencyAssets();
                foreach (Asset dependencyAsset in dependencyAssets)
                {
                    dependencyAssetNames.Add(dependencyAsset.Name);
                }

                if (RecordScatteredDependencyAssetsSelected)
                {
                    dependencyAssetNames.AddRange(dependencyData.GetScatteredDependencyAssetNames());
                }

                dependencyAssetNames.Sort();

                m_AssetBundleDatas[asset.AssetBundle.FullName.ToLower()].AddAssetData(asset.Guid, assetName, assetBytes.Length, assetHashCode, dependencyAssetNames.ToArray());
            }


            foreach (AssetBundleData assetBundleData in m_AssetBundleDatas.Values)
            {
                if (assetBundleData.AssetCount <= 0)
                {
                    m_BuildReport.LogError("AssetBundle '{0}' has no asset.", GetAssetBundleFullName(assetBundleData.Name, assetBundleData.Variant));
                    return null;
                }
            }

            AssetBundleBuild[] buildMap = new AssetBundleBuild[m_AssetBundleDatas.Count];
            int index = 0;
            foreach (AssetBundleData assetBundleData in m_AssetBundleDatas.Values)
            {
                buildMap[index].assetBundleName = assetBundleData.Name;
                buildMap[index].assetBundleVariant = assetBundleData.Variant;
                buildMap[index].assetNames = assetBundleData.GetAssetNames();
                index++;
            }

            return buildMap;
        }

        private string GetAssetBundleFullName(string assetBundleName, string assetBundleVariant)
        {
            return (!string.IsNullOrEmpty(assetBundleVariant) ? string.Format("{0}.{1}", assetBundleName, assetBundleVariant) : assetBundleName).ToLower();
        }

        public string GetBuildTargetName(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows:
                    return "windows";
#if UNITY_2017_3_OR_NEWER
                case BuildTarget.StandaloneOSX:
#else
                case BuildTarget.StandaloneOSXUniversal:
#endif
                    return "osx";
                case BuildTarget.iOS:
                    return "ios";
                case BuildTarget.Android:
                    return "android";
                case BuildTarget.WSAPlayer:
                    return "winstore";
                default:
                    return "notsupported";
            }
        }

        public byte[] GetQuickXorBytes(byte[] bytes, byte[] code)
        {
            return GetXorBytes(bytes, code, QuickEncryptLength);
        }

        private byte[] GetXorBytes(byte[] bytes, byte[] code)
        {
            return GetXorBytes(bytes, code, 0);
        }

        private byte[] GetXorBytes(byte[] bytes, byte[] code, int length)
        {
            if (bytes == null)
            {
                return null;
            }

            int codeLength = code.Length;
            if (code == null || codeLength <= 0)
            {
                throw new GameFrameworkException("Code is invalid.");
            }

            int codeIndex = 0;
            int bytesLength = bytes.Length;
            if (length <= 0 || length > bytesLength)
            {
                length = bytesLength;
            }

            byte[] result = new byte[bytesLength];
            System.Buffer.BlockCopy(bytes, 0, result, 0, bytesLength);

            for (int i = 0; i < length; i++)
            {
                result[i] ^= code[codeIndex++];
                codeIndex = codeIndex % codeLength;
            }

            return result;
        }
    }
}
