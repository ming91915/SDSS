﻿using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using SDSS.Definitions;
using SDSS.Structures;

namespace SDSS.Models
{
    [Serializable()]
    public class Model2 : ModelBase
    {
        #region ---   XmlElement

        /// <summary> 模型所对应的框架结构 </summary>
        [XmlElement]
        public Frame Frame { get; set; }

        #endregion

        #region ---   构造函数

        /// <summary> 构造函数 </summary>
        public Model2() : base(ModelType.Frame, CalculationMethod.FanYingWeiYi)
        {
            DescriptionName = @"矩形车站反应位移法";
            //
            Frame = new Frame();
        }

        #endregion

        #region ---   模型检验

        /// <summary> 对模型进行检查，如果此模型不满足进行计算的必备条件，则返回false </summary>
        public override bool Validate(ref StringBuilder errorMessage)
        {
            errorMessage.AppendLine("模型检验完成，可以进行计算");
            return true;
        }

        #endregion

        /// <summary> 将模型信息写入一个文本文件中，用来作为 Ansys 计算的初始参数提供给 APDL 命令流 </summary>
        /// <param name="filePath">要写入的文件路径，此文件当前可以不存在</param>
        /// <param name="errMsg">出错信息</param>
        /// <returns>是否写入成功</returns>
        public override bool WriteCalculateFileForAnsys(string filePath, ref StringBuilder errMsg)
        {
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                using (var sw = new StreamWriter(fs))
                {





                }
            }
            return true;
        }


        #region ---   几何绘图

        public override StationGeometry GetStationGeometry()
        {
            SoilFrameGeometry ssg = null;
            return ssg;
        }

        #endregion
    }
}