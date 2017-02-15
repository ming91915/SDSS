﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using eZstd.Miscellaneous;
using SDSS.Definitions;
using SDSS.Solver;

namespace SDSS.Utility
{
    /// <summary> 整个项目中与路径相关的信息 </summary>
    public static class ProjectPaths
    {
        /// <summary> 前处理程序的文件绝对路径 </summary>
        private static readonly FileInfo _f_PreProc = new FileInfo(Assembly.GetExecutingAssembly().FullName);
        /// <summary> 前处理程序的文件绝对路径 </summary>
        public static readonly string F_PreProc = _f_PreProc.FullName;

        #region ---   文件夹路径

        /// <summary> 前处理程序的文件所在文件夹的绝对路径 </summary>
        public static readonly string D_PreProc = _f_PreProc.Directory.FullName;

        /// <summary> 整个解决方案所在文件夹 </summary>
        private static readonly DirectoryInfo _d_Solution = _f_PreProc.Directory.Parent;
        /// <summary> 整个解决方案所在文件夹 </summary>
        public static readonly string D_Solution = _d_Solution.FullName;


        /// <summary> 前处理程序的文件所在文件夹的绝对路径 </summary>
        public static readonly string D_MiddleFiles = _d_Solution.GetDirectories("MidFiles").First().FullName;

        /// <summary> 利用 Abaqus 进行计算的 Python 源代码所在文件夹 </summary>
        public static readonly string D_PythonSource = _d_Solution.GetDirectories("AbaqusSolver").First().FullName;

        /// <summary> Abaqus 的工作文件夹 </summary>
        public static string D_AbaqusWorkingDir;


        #endregion

        #region ---   文件路径

        /// <summary> .txt 文件，此文本文件中记录有所有存储有模型参数、计算参数的文件所在的路径 </summary>
        public static string F_CalcutionFilePaths;

        /// <summary> .txt 文件，此文本文件中记录有所有存储有模型参数、计算参数的文件所在的路径 </summary>
        private const string CalcutionFileName = "CalculationFiles.paths";

        /// <summary> 默认的车站模型的计算文件的名称 </summary>
        public const string DefaultModelName = "StationDesginModel" + Constants.FileExtensions.Model1;


        /// <summary> 用于启动 Abaqus 的.bat文件 </summary>
        /// <remarks> 此 .bat 文件中的内容格式如下;
        /// @echo off
        /// rem : The directory for the files created during the calcution as well as the results.
        /// cd /d C:\Users\zengfy\Desktop\AbaqusScriptTest
        /// 
        /// rem : Execute Abaqus without showing the users interface.
        /// abaqus cae noGUI=beamExample.py</remarks>
        public static readonly string F_InitialBat = Path.Combine(D_MiddleFiles, "InitialSolver.bat");

        private static string _f_PySolver;
        /// <summary> 执行 Abaqus 计算的初始 .py 源代码文件 </summary>
        public static string F_PySolver
        {
            get { return _f_PySolver; }
        }

        /// <summary> 记录车站模型的 xml 文件的路径 </summary>
        public static string F_ModelFile;

        #endregion

        #region ---   路径设置或获取的方法

        /// <summary>
        /// 根据不同的模型类型来确定要使用哪一个 .py 脚本进行 Abaqus 的计算
        /// </summary>
        /// <param name="modelType"></param>
        public static void DetermineSolverSource(ModelType modelType)
        {
            switch (modelType)
            {
                //case ModelType.Model1: _f_PySolver = Path.Combine(D_PythonSource, @"Models\Model1.py"); break;
                //case ModelType.Model2: _f_PySolver = Path.Combine(D_PythonSource, @"Models\Model2.py"); break;
                default: _f_PySolver = Path.Combine(D_PythonSource, @"EnvironmentBuild.py"); break;
            }
        }

        /// <summary>
        /// 根据 Abaqus 工作空间的不同来设置对应的文件路径
        /// </summary>
        /// <param name="workingDir"></param>
        public static void SetAbaqusWorkingDir(string workingDir)
        {
            if (!Directory.Exists(workingDir))
            {
                var res = MessageBox.Show(@"指定的计算文件夹不存在，是否创建此文件夹？", @"提示", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning);
                if (res == DialogResult.OK)
                {
                    Directory.CreateDirectory(workingDir);
                }
                else
                {
                    return;
                }
            }
            D_AbaqusWorkingDir = workingDir;
            F_CalcutionFilePaths = Path.Combine(workingDir, CalcutionFileName);
        }

        #endregion

        #region ---   文件数据写入

        /// <summary>
        /// 根据前处理界面中用户输入的计算模型信息，将其写入 xml 文件中
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="stationModel"></param>
        /// <param name="errorMessage"></param>
        /// <returns>如果成功写入，则返回 true，如果失败则返回 false。</returns>
        public static bool SerializeNewModelFile(string xmlFilePath, StationModel.StationModel stationModel, out string errorMessage)
        {
            //if (File.Exists(xmlFilePath))
            //{
            //    File.Delete(xmlFilePath);
            //}
            StreamWriter fs = null;
            try
            {
                Type tp = stationModel.GetType();

                fs = new StreamWriter(xmlFilePath, append: false);
                XmlSerializer s = new XmlSerializer(tp);
                s.Serialize(fs, stationModel);
                //
                F_ModelFile = xmlFilePath;
                errorMessage = "可以成功导出模型";
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "模型信息写入失败";
                F_ModelFile = string.Empty;
                return false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// 将存储有模型参数、计算参数的文件所在的路径写入到一个单独的文本中，以供 Python 程序读取。
        /// 此文件的路径是固定的。
        /// </summary>
        public static void WriteCalcFilePaths()
        {
            StreamWriter sw = new StreamWriter(F_CalcutionFilePaths, append: false);

            // 在此文件中写入各种计算文件的路径，路径含义与路径字符之间通过“ * ”进行分隔
            string sep = @" * ";

            // 1. 记录模型信息的 xml 文件
            sw.WriteLine("ModelFile" + sep + F_ModelFile);

            // 2. Python 脚本源代码所在文件夹
            sw.WriteLine("PythonSourceDir" + sep + D_PythonSource);

            // 3. Abaqus 的工作文件夹
            sw.WriteLine("AbaqusWorkingDir" + sep + D_AbaqusWorkingDir);

            //
            sw.Close();
        }

        #endregion
    }
}