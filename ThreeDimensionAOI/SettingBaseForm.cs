using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using XMLFile;//关于xml文件的读写


namespace ThreeDimensionAVI
{
    public delegate void CloseSoftwareRequestDelegate();//声明委托-请求关闭软件
    public delegate void RequetChangeFormToMainFormDelegate();//声明委托-请求切换界面至主界面
    public delegate void RequestOpenEachAxisControlFormDelegate();//声明委托，请求打开各轴控制页面
    public delegate void SettingsBaseFormRequestXYMoveToAssignedPointDelegate(int assignedPoint,int trackIndex);//声明委托，轴控制手动界面请求XY移动至指定点位，用于传递axes control maintenance form的事件

    public partial class SettingBaseForm : Form
    {

        /*--------------------------------------------------------------------------------------
       //Copyright (C) 2022 深圳市利器精工科技有限公司
       //版权所有
       //
       //文件名：参数设定及手动界面基础界面
       //文件功能描述：作为整个设定及手动界面的基础界面，整个设定及手动界面最基础的控制都在此窗口中完成，设定及手动界面窗口的动作最终都会汇总到此窗口进行
       //
       //
       //创建标识：MaLi 20220316
       //
       //修改标识：MaLi 20220316 Change
       //修改描述：增加参数设定及手动界面基础界面
       //
       //------------------------------------------------------------------------------------*/
        //******************************事件************************************//
        public event CloseSoftwareRequestDelegate CloseSoftwareRequestEvent;//声明事件-请求关闭软件
        public event RequetChangeFormToMainFormDelegate RequetChangeFormToMainFormEvent;//声明事件-请求切换界面至主界面
        public event RequestOpenEachAxisControlFormDelegate RequestOpenEachAxisControlFormEvent;//声明事件，请求打开各轴控制页面
        public event SettingsBaseFormRequestXYMoveToAssignedPointDelegate SettingsBaseFormRequestXYMoveToAssignedPointEvent;//声明事件，轴控制手动界面请求XY移动至指定点位，用于传递axes control maintenance form的事件
        //*************************外部可读写变量*******************************//
        public MaintenanceCylinderControlForm  _maintenanceCylinderControlFormVariate = null;
        public MaintenanceAxesControlForm  _maintenanceAxesControlFormVariate = null;
        public MaintenanceCameraForm _maintenanceCameraFormVariate = null;
        public SettingsGeneralForm _settingsGeneralFormVariate = null;
        public SettingsProductForm _settingsProductFormVariate = null;
        public SettingsAdministratorForm _settingsAdminFormVariate = null;
        public SettingsCameraCalibrationForm _settingsCameraCalibrationVariate = null;
        //*************************公共静态变量*******************************//
        //*************************内部私有变量*******************************//

        ////////////////////////////////////////////////////


        //构造函数
        public SettingBaseForm()
        {
            InitializeComponent();

            //创建并隐藏手动气缸控制窗口
            _maintenanceCylinderControlFormVariate = new MaintenanceCylinderControlForm();
            _maintenanceCylinderControlFormVariate.TopLevel = false;
            pnlMaintenanceAndSettingsFormDisplay.Controls.Add(_maintenanceCylinderControlFormVariate);
            _maintenanceCylinderControlFormVariate.Hide();

            //创建并隐藏手动轴控制窗口
            _maintenanceAxesControlFormVariate = new MaintenanceAxesControlForm();
            _maintenanceAxesControlFormVariate.TopLevel = false;
            pnlMaintenanceAndSettingsFormDisplay.Controls.Add(_maintenanceAxesControlFormVariate);
            _maintenanceAxesControlFormVariate.Hide();
            _maintenanceAxesControlFormVariate.MaintenanceAxesControlFormRequestXYMoveToAssignedPointEvent += new MaintenanceAxesControlFormRequestXYMoveToAssignedPointDelegate(MaintenanceAxesControlFormRequestXYMoveToAssignedPointEventFunc);

            //创建并隐藏SettingsCameraForm窗口
            _maintenanceCameraFormVariate = new MaintenanceCameraForm();
            _maintenanceCameraFormVariate.TopLevel = false;
            pnlMaintenanceAndSettingsFormDisplay.Controls.Add(_maintenanceCameraFormVariate);
            _maintenanceCameraFormVariate.Hide();

            //创建并隐藏SettingsGeneralForm窗口
            _settingsGeneralFormVariate = new SettingsGeneralForm();
            _settingsGeneralFormVariate.TopLevel = false;
            pnlMaintenanceAndSettingsFormDisplay.Controls.Add(_settingsGeneralFormVariate);
            _settingsGeneralFormVariate.Hide();

            //创建并隐藏SettingsAdministratorForm窗口
            _settingsAdminFormVariate = new SettingsAdministratorForm();
            _settingsAdminFormVariate.TopLevel = false;
            pnlMaintenanceAndSettingsFormDisplay.Controls.Add(_settingsAdminFormVariate);
            _settingsAdminFormVariate.Hide();

            //创建并隐藏SettingsProductForm窗口
            _settingsProductFormVariate = new SettingsProductForm();
            _settingsProductFormVariate.TopLevel = false;
            pnlMaintenanceAndSettingsFormDisplay.Controls.Add(_settingsProductFormVariate);
            _settingsProductFormVariate.Hide();

            //创建并隐藏SettingsCameraCalibrationForm窗口
            _settingsCameraCalibrationVariate = new SettingsCameraCalibrationForm();
            _settingsCameraCalibrationVariate.TopLevel = false;
            pnlMaintenanceAndSettingsFormDisplay.Controls.Add(_settingsCameraCalibrationVariate);
            _settingsCameraCalibrationVariate.Hide();
        }

        //界面显示状态发生变化事件
        private void SettingBaseForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                //显示隐藏
                btnMaintenanceAxesCtrl.Visible = true;
                btnMaintenanceCylinderCtrl.Visible = true;
                btnMaintenanceCamera.Visible = true;

                btnSettingsGeneral.Visible = false;
                btnSettingsAdministrator.Visible = false;
                btnSettingsProduct.Visible = false;
                

                //设置显示的窗口
                _maintenanceAxesControlFormVariate.Show();
                _maintenanceCylinderControlFormVariate.Hide();
                _maintenanceCameraFormVariate.Hide();

                _settingsAdminFormVariate.Hide();
                _settingsGeneralFormVariate.Hide();
                _settingsProductFormVariate.Hide();
                _settingsCameraCalibrationVariate.Hide();
                
                //设置按钮颜色
                btnSetting.BackColor = Color.Gainsboro;
                btnMaintenance.BackColor = Color.Orange;

                btnMaintenanceAxesCtrl.BackColor = Color.Orange;
                btnMaintenanceCylinderCtrl.BackColor = Color.Gainsboro;
                btnMaintenanceCamera.BackColor = Color.Gainsboro;

                btnSettingsGeneral.BackColor = Color.Gainsboro;
                btnSettingsAdministrator.BackColor = Color.Gainsboro;
                btnSettingsProduct.BackColor = Color.Gainsboro;
                btnSettingsCameraCalibration.BackColor = Color.Gainsboro;
            }
            else
            {
                if (_maintenanceAxesControlFormVariate != null)
                    _maintenanceAxesControlFormVariate.Hide();
                if (_maintenanceCylinderControlFormVariate != null)
                    _maintenanceCylinderControlFormVariate.Hide();
                if (_maintenanceCameraFormVariate != null)
                    _maintenanceCameraFormVariate.Hide();

                if (_settingsAdminFormVariate != null)
                    _settingsAdminFormVariate.Hide();
                if (_settingsGeneralFormVariate != null)
                    _settingsGeneralFormVariate.Hide();
                if (_settingsProductFormVariate != null)
                    _settingsProductFormVariate.Hide();
                if (_settingsCameraCalibrationVariate != null)
                    _settingsCameraCalibrationVariate.Hide();
            }
        }


        //当点击了主界面按钮，切换到主界面
        private void btnMain_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("手动设置基础页面-打开主页面" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (_maintenanceAxesControlFormVariate != null)
                _maintenanceAxesControlFormVariate.Hide();
            if (_maintenanceCylinderControlFormVariate != null)
                _maintenanceCylinderControlFormVariate.Hide();
            if (_maintenanceCameraFormVariate != null)
                _maintenanceCameraFormVariate.Hide();

            if (_settingsAdminFormVariate != null)
                _settingsAdminFormVariate.Hide();
            if (_settingsGeneralFormVariate != null)
                _settingsGeneralFormVariate.Hide();
            if (_settingsProductFormVariate != null)
                _settingsProductFormVariate.Hide();
            if (_settingsCameraCalibrationVariate != null)
                _settingsCameraCalibrationVariate.Hide();

            if (RequetChangeFormToMainFormEvent != null)//触发切换窗口至主界面事件
            {
                RequetChangeFormToMainFormEvent();
            }
        }

        //当点击了退出按钮，退出程序
        private void btnExit_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("手动设置基础页面-请求退出软件" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (CloseSoftwareRequestEvent != null)//触发请求退出程序事件
            {
                CloseSoftwareRequestEvent();
            }
        }

        //点击了界面切换按钮事件
        private void ClickFormChangeBtnEvent(object sender, EventArgs e)
        {
            //设置显示的窗口
            _maintenanceAxesControlFormVariate.Hide();
            _maintenanceCylinderControlFormVariate.Hide();
            _maintenanceCameraFormVariate.Hide();
            _settingsAdminFormVariate.Hide();
            _settingsGeneralFormVariate.Hide();
            _settingsProductFormVariate.Hide();
            _settingsCameraCalibrationVariate.Hide();

            //设置按钮颜色
            btnSetting.BackColor = Color.Gainsboro;
            btnMaintenance.BackColor = Color.Gainsboro;

            btnMaintenanceAxesCtrl.BackColor = Color.Gainsboro;
            btnMaintenanceCylinderCtrl.BackColor = Color.Gainsboro;
            btnMaintenanceCamera.BackColor = Color.Gainsboro;

            btnSettingsGeneral.BackColor = Color.Gainsboro;
            btnSettingsAdministrator.BackColor = Color.Gainsboro;
            btnSettingsProduct.BackColor = Color.Gainsboro;
            btnSettingsCameraCalibration.BackColor = Color.Gainsboro;

            //根据触发的按钮对按钮控件等进行设置
            switch (((Button)sender).Name)
            {
                case "btnSetting":
                    {
                        MyTool.TxtFileProcess.CreateLog("手动设置基础页面-切换页面至设置页面" + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                        btnMaintenanceAxesCtrl.Visible = false;
                        btnMaintenanceCylinderCtrl.Visible = false;
                        btnMaintenanceCamera.Visible = false;

                        btnSettingsGeneral.Visible = true;
                        btnSettingsAdministrator.Visible = true;
                        btnSettingsProduct.Visible = true;
                        btnSettingsCameraCalibration.Visible = true;

                        _settingsProductFormVariate.Show();

                        btnSetting.BackColor = Color.Orange;
                        btnSettingsProduct.BackColor = Color.Orange;
                        break;
                    }
                case "btnMaintenance":
                    {
                        MyTool.TxtFileProcess.CreateLog("手动设置基础页面-切换页面至手动页面" + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                        btnMaintenanceAxesCtrl.Visible = true;
                        btnMaintenanceCylinderCtrl.Visible = true;
                        btnMaintenanceCamera.Visible = true;

                        btnSettingsGeneral.Visible = false;
                        btnSettingsAdministrator.Visible = false;
                        btnSettingsProduct.Visible = false;
                        btnSettingsCameraCalibration.Visible = false;

                        _maintenanceAxesControlFormVariate.Show();

                        btnMaintenance.BackColor = Color.Orange;
                        btnMaintenanceAxesCtrl.BackColor = Color.Orange;
                        break;
                    }
                case "btnMaintenanceAxesCtrl":
                    {
                        MyTool.TxtFileProcess.CreateLog("手动设置基础页面-切换页面至手动轴控制页面" + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                        _maintenanceAxesControlFormVariate.Show();
                        btnMaintenanceAxesCtrl.BackColor = Color.Orange;
                        btnMaintenance.BackColor = Color.Orange;
                        break;
                    }
                case "btnMaintenanceCylinderCtrl":
                    {
                        MyTool.TxtFileProcess.CreateLog("手动设置基础页面-切换页面至手动气缸控制页面" + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                        _maintenanceCylinderControlFormVariate.Show();
                        btnMaintenanceCylinderCtrl.BackColor = Color.Orange;
                        btnMaintenance.BackColor = Color.Orange;
                        break;
                    }
                case "btnMaintenanceCamera":
                    {
                        MyTool.TxtFileProcess.CreateLog("手动设置基础页面-切换页面至手动相机控制页面" + "----用户：" + BaseForm._operatorName,
                       BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                        _maintenanceCameraFormVariate.Show();
                        btnMaintenanceCamera.BackColor = Color.Orange;
                        btnMaintenance.BackColor = Color.Orange;
                        break;
                    }
                case "btnSettingsProduct":
                    {
                        MyTool.TxtFileProcess.CreateLog("手动设置基础页面-切换页面至产品设置页面" + "----用户：" + BaseForm._operatorName,
                       BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                        _settingsProductFormVariate.Show();
                        btnSettingsProduct.BackColor = Color.Orange;
                        btnSetting.BackColor = Color.Orange;
                        break;
                    }
                case "btnSettingsAdministrator":
                    {
                        MyTool.TxtFileProcess.CreateLog("手动设置基础页面-切换页面至管理员设置页面" + "----用户：" + BaseForm._operatorName,
                      BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                        _settingsAdminFormVariate.Show();
                        btnSettingsAdministrator.BackColor = Color.Orange;
                        btnSetting.BackColor = Color.Orange;
                        break;
                    }
                case "btnSettingsGeneral":
                    {
                        MyTool.TxtFileProcess.CreateLog("手动设置基础页面-切换页面至通用设置页面" + "----用户：" + BaseForm._operatorName,
                      BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                        _settingsGeneralFormVariate.Show();
                        btnSettingsGeneral.BackColor = Color.Orange;
                        btnSetting.BackColor = Color.Orange;
                        break;
                    }
                case "btnSettingsCameraCalibration":
                    {
                        MyTool.TxtFileProcess.CreateLog("手动设置基础页面-切换页面至相机标定设置页面" + "----用户：" + BaseForm._operatorName,
                      BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                        _settingsCameraCalibrationVariate.Show();
                        btnSettingsCameraCalibration.BackColor = Color.Orange;
                        btnSetting.BackColor = Color.Orange;
                        break;
                    }
            }
        }

        //点击了各轴控制页面按钮
        private void btnEachAxisCtrl_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("手动设置基础页面-打开轴手动控制页面" + "----用户：" + BaseForm._operatorName,
                      BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            //触发事件，打开各轴运动控制窗口
            if (RequestOpenEachAxisControlFormEvent != null)
            {
                RequestOpenEachAxisControlFormEvent();
            }
        }

        //axes control maintenance form的轴移动至指定点位号的事件函数
        private void MaintenanceAxesControlFormRequestXYMoveToAssignedPointEventFunc(int assignedPoint, int trackIndex)
        {
            if (SettingsBaseFormRequestXYMoveToAssignedPointEvent != null)
            {
                SettingsBaseFormRequestXYMoveToAssignedPointEvent(assignedPoint, trackIndex);
            }
        }
    }
}
