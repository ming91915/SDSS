﻿using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SDSS.Definitions;
using SDSS.Entities;
using SDSS.StationModel;

namespace SDSS.UIControls
{
    internal partial class MainForm : Form
    {
        private StationModel1 _stationModel;

        public MainForm(StationModel1 sm)
        {
            InitializeComponent();
            //
            _stationModel = sm;
            // 对不同的车站模型，在界面中进行绘图显示
            RefreshModel(_stationModel);
        }


        #region ---   前处理界面效果显示

        /// <summary> 对不同的车站模型，在界面中进行绘图显示 </summary>
        /// <param name="sm"></param>
        private void RefreshModel(StationModel.StationModel sm)
        {

        }

        private void modelDrawer1_Paint(object sender, PaintEventArgs e)
        {
            SoilStructureGeometry ssg = ConstructSSG();
            Graphics g = e.Graphics;
            modelDrawer1.DrawSoilStructureModel(g, ssg);
        }

        //获取车站模型参数类
        private SoilStructureGeometry ConstructSSG()
        {
            var geo = _stationModel.GetStationGeometry();

            return geo as SoilStructureGeometry;
        }

        #endregion

        private void button_Profiles_Click(object sender, System.EventArgs e)
        {
            DefinitionManager<Profile> dm = new DefinitionManager<Profile>(_stationModel.ProfileDefinitions);
            dm.ShowDialog();
            // 刷新表格界面
            //RefreshComboBox(ColumnSegment, _stationModel.ProfileDefinitions);
        }

        private void button_Materials_Click(object sender, System.EventArgs e)
        {
            DefinitionManager<Material> dm = new DefinitionManager<Material>(_stationModel.MaterialDefinitions);
            dm.ShowDialog();
            // 刷新表格界面
            //RefreshComboBox(ColumnSegment, _stationModel.ProfileDefinitions);
        }

        private void button_GenerateFrame_Click(object sender, System.EventArgs e)
        {
            // create a new form
            var layerCount = (ushort)textBoxNum_layers.ValueNumber;
            var spanCount = (ushort)textBoxNum_spans.ValueNumber;
            if (layerCount > 0 && spanCount > 0)
            {
                FrameConstructor fc = FrameConstructor.GetUniqueInstance(layerCount, spanCount);
                //
                var res = fc.ShowDialog();
                if (res == DialogResult.OK)
                {
                    _stationModel.GenerateFrame(fc.LayerHeights, fc.SpanWidths);


                    // 将生成好的框架模型显示在 Datagridview 表格中
                    List<Component> components = new List<Component>();
                    components.AddRange(_stationModel.Beams);
                    components.AddRange(_stationModel.Columns);
                    //
                    eZDataGridViewFrame.AutoGenerateColumns = true;
                    eZDataGridViewFrame.DataSource = components;
                }
            }
        }

    }
}