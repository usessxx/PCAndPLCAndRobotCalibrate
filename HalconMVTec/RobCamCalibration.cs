using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace HalconMVTec
{
    public class RobCamCalibration
    {
        #region 计算相机机械手9点标定的变换矩阵(最少三点)
        /// <summary>
        /// robAndCamCalib_9P:计算相机机械手9点标定的变换矩阵(最少三点)
        /// </summary>
        /// <param name="_xPixel">float[],9点像素X方向坐标值</param>
        /// <param name="_yPixel">float[],9点像素Y方向坐标值</param>
        /// <param name="_xRob">float[],9点机械手X方向坐标值</param>
        /// <param name="_yRob">float[],9点机械手Y方向坐标值</param>
        /// <returns>HTuple:返回像素坐标变换为机械手坐标的变换矩阵</returns>
        public HTuple robAndCamCalib_9P(float[] _xPixel, float[] _yPixel, float[] _xRob, float[] _yRob)
        {
            HTuple hv_HomMat2D_9P = new HTuple();
            HTuple hv_X_Pixel = new HTuple();
            HTuple hv_Y_Pixel = new HTuple();
            HTuple hv_X_Rob = new HTuple();
            HTuple hv_Y_Rob = new HTuple();
            try
            {
                hv_X_Pixel.Dispose();
                hv_X_Pixel = new HTuple();
                hv_X_Pixel[0] = _xPixel[0];
                hv_X_Pixel[1] = _xPixel[1];
                hv_X_Pixel[2] = _xPixel[2];
                hv_X_Pixel[3] = _xPixel[3];
                hv_X_Pixel[4] = _xPixel[4];
                hv_X_Pixel[5] = _xPixel[5];
                hv_X_Pixel[6] = _xPixel[6];
                hv_X_Pixel[7] = _xPixel[7];
                hv_X_Pixel[8] = _xPixel[8];

                hv_Y_Pixel.Dispose();
                hv_Y_Pixel = new HTuple();
                hv_Y_Pixel[0] = _yPixel[0];
                hv_Y_Pixel[1] = _yPixel[1];
                hv_Y_Pixel[2] = _yPixel[2];
                hv_Y_Pixel[3] = _yPixel[3];
                hv_Y_Pixel[4] = _yPixel[4];
                hv_Y_Pixel[5] = _yPixel[5];
                hv_Y_Pixel[6] = _yPixel[6];
                hv_Y_Pixel[7] = _yPixel[7];
                hv_Y_Pixel[8] = _yPixel[8];

                hv_X_Rob.Dispose();
                hv_X_Rob = new HTuple();
                hv_X_Rob[0] = _xRob[0];
                hv_X_Rob[1] = _xRob[1];
                hv_X_Rob[2] = _xRob[2];
                hv_X_Rob[3] = _xRob[3];
                hv_X_Rob[4] = _xRob[4];
                hv_X_Rob[5] = _xRob[5];
                hv_X_Rob[6] = _xRob[6];
                hv_X_Rob[7] = _xRob[7];
                hv_X_Rob[8] = _xRob[8];

                hv_Y_Rob.Dispose();
                hv_Y_Rob = new HTuple();
                hv_Y_Rob[0] = _yRob[0];
                hv_Y_Rob[1] = _yRob[1];
                hv_Y_Rob[2] = _yRob[2];
                hv_Y_Rob[3] = _yRob[3];
                hv_Y_Rob[4] = _yRob[4];
                hv_Y_Rob[5] = _yRob[5];
                hv_Y_Rob[6] = _yRob[6];
                hv_Y_Rob[7] = _yRob[7];
                hv_Y_Rob[8] = _yRob[8];

                hv_HomMat2D_9P.Dispose();
                HOperatorSet.VectorToHomMat2d(hv_X_Pixel, hv_Y_Pixel, hv_X_Rob, hv_Y_Rob, out hv_HomMat2D_9P);
            }
            catch (Exception ex)
            {
                hv_HomMat2D_9P = null;
            }
            return hv_HomMat2D_9P;
        }
        #endregion

        #region 保存相机机械手9点标定的变换矩阵
        /// <summary>
        /// saveDateRobAndCamCalib_9P:保存相机机械手9点标定的变换矩阵
        /// </summary>
        /// <param name="hv_HomMat2D_9P">HTuple,9点标定变换矩阵</param>
        /// <param name="_save9PointCalibrateDataPath">string,保存路径(包含文件名的后缀)</param>
        /// <returns>bool:保存成功返回true</returns>
        public bool saveDateRobAndCamCalib_9P(HTuple hv_HomMat2D_9P, string _save9PointCalibrateDataPath)
        {
            HTuple hv_SerializedItemHandle = new HTuple();
            HTuple hv_FileHandle = new HTuple();
            try
            {
                hv_SerializedItemHandle.Dispose();
                HOperatorSet.SerializeHomMat2d(hv_HomMat2D_9P, out hv_SerializedItemHandle);
                hv_FileHandle.Dispose();
                HOperatorSet.OpenFile(_save9PointCalibrateDataPath, "output_binary", out hv_FileHandle);
                HOperatorSet.FwriteSerializedItem(hv_FileHandle, hv_SerializedItemHandle);
                HOperatorSet.CloseFile(hv_FileHandle);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region 读取相机机械手9点标定的变换矩阵
        /// <summary>
        /// readDateRobAndCamCalib_9P:保存相机机械手9点标定的变换矩阵
        /// </summary>
        /// <param name="_read9PointCalibrateDataPath">string,读取路径(包含文件名的后缀)</param>
        /// <returns>HTuple:返回像素坐标变换为机械手坐标的变换矩阵</returns>
        public HTuple readDateRobAndCamCalib_9P(string _read9PointCalibrateDataPath)
        {
            HTuple hv_HomMat2D_9P = new HTuple();
            HTuple hv_SerializedItemHandle = new HTuple();
            HTuple hv_FileHandle = new HTuple();
            try
            {
                hv_FileHandle.Dispose();
                HOperatorSet.OpenFile(_read9PointCalibrateDataPath, "input_binary", out hv_FileHandle);
                hv_SerializedItemHandle.Dispose();
                HOperatorSet.FreadSerializedItem(hv_FileHandle, out hv_SerializedItemHandle);
                hv_HomMat2D_9P.Dispose();
                HOperatorSet.DeserializeHomMat2d(hv_SerializedItemHandle, out hv_HomMat2D_9P);
                HOperatorSet.CloseFile(hv_FileHandle);



            }
            catch (Exception ex)
            {
                hv_HomMat2D_9P = null;
            }
            return hv_HomMat2D_9P;
        }
        #endregion

        #region 计算坐标系的变换矩阵(同一个坐标系内,最少两点)
        /// <summary>
        /// coordTransAndRotMat:计算坐标系的变换矩阵(同一个坐标系内,最少两点)
        /// </summary>
        /// <param name="_XPrePos">float[],变换前的X坐标</param>
        /// <param name="_YPrePos">float[],变换前的Y坐标</param>
        /// <param name="_XTransPos">float[],变换后的X坐标</param>
        /// <param name="_YTransPos">float[],变换后的Y坐标</param>
        /// <returns>HTuple:返回的变换矩阵</returns>
        public HTuple coordTransAndRotMat(float[] _XPrePos, float[] _YPrePos, float[] _XTransPos, float[] _YTransPos)
        {
            HTuple hv_HomMat2D = new HTuple();
            HTuple hv_XPrePos = new HTuple();
            HTuple hv_YPrePos = new HTuple();
            HTuple hv_XTransPos = new HTuple();
            HTuple hv_YTransPos = new HTuple();
            try
            {
                for (int i = 0; i < _XPrePos.Length; i++)
                {
                    hv_XPrePos[i] = _XPrePos[i];
                    hv_YPrePos[i] = _YPrePos[i];
                    hv_XTransPos[i] = _XTransPos[i];
                    hv_YTransPos[i] = _YTransPos[i];
                }
            }
            catch (Exception ex)
            {
                hv_HomMat2D = null;
                return hv_HomMat2D;
            }
            try
            {
                HOperatorSet.VectorToRigid(hv_XPrePos, hv_YPrePos, hv_XTransPos, hv_YTransPos, out hv_HomMat2D);
            }
            catch (Exception ex)
            {
                hv_HomMat2D = null;
            }
            return hv_HomMat2D;
        }
        #endregion

        #region 计算坐标系的变换矩阵(同一个坐标系内,一个点与角度)
        /// <summary>
        /// coordTransAndRotMat:计算坐标系的变换矩阵(同一个坐标系内,一个点与角度)
        /// </summary>
        /// <param name="_XPrePos">float,变换前的X坐标</param>
        /// <param name="_YPrePos">float,变换前的Y坐标</param>
        /// <param name="_anglePrePos">float,变换前的角度(请输入角度,函数内部会转换为弧度)</param>
        /// <param name="_XTransPos">float,变换后的X坐标</param>
        /// <param name="_YTransPos">float,变换后的Y坐标</param>
        /// <param name="_angleTransPos">float,变换后的角度(请输入角度,函数内部会转换为弧度)</param>
        /// <returns>HTuple:返回的变换矩阵</returns>
        public HTuple coordTransAndRotMat(float _XPrePos, float _YPrePos, float _anglePrePos, float _XTransPos, float _YTransPos, float _angleTransPos)
        {
            HTuple hv_HomMat2D = new HTuple();
            HTuple hv_XPrePos = _XPrePos;
            HTuple hv_YPrePos = _YPrePos;
            HTuple hv_anglePrePos = new HTuple();
            HOperatorSet.TupleRad(_anglePrePos, out hv_anglePrePos);
            HTuple hv_XTransPos = _XTransPos;
            HTuple hv_YTransPos = _YTransPos;
            HTuple hv_angleTransPos = new HTuple();
            HOperatorSet.TupleRad(_angleTransPos, out hv_angleTransPos);
            try
            {
                HOperatorSet.VectorAngleToRigid(hv_XPrePos, hv_YPrePos, hv_anglePrePos, hv_XTransPos, hv_YTransPos, hv_angleTransPos, out hv_HomMat2D);
            }
            catch (Exception ex)
            {
                hv_HomMat2D = null;
            }
            return hv_HomMat2D;
        }
        #endregion

        #region 坐标变换
        /// <summary>
        /// coordTransform:坐标变换
        /// </summary>
        /// <param name="hv_HomMat2D">HTuple,变换矩阵</param>
        /// <param name="_XPrePos">float[],要变换的X坐标</param>
        /// <param name="_YPrePos">float[],要变换的Y坐标</param>
        /// <param name="_XTransPos">float[],变换后的X坐标</param>
        /// <param name="_YTransPos">float[],变换后的Y坐标</param>
        /// <returns>bool:成功则返回true</returns>
        public bool coordTransform(HTuple hv_HomMat2D, float[] _XPrePos, float[] _YPrePos, ref float[] _XTransPos, ref float[] _YTransPos)
        {
            HTuple hv_X_PixelPos = new HTuple();
            HTuple hv_Y_PixelPos = new HTuple();
            HTuple hv_X_RobPos = new HTuple();
            HTuple hv_Y_RobPos = new HTuple();
            try
            {
                for (int i = 0; i < _XPrePos.Length; i++)
                {
                    hv_X_PixelPos[i] = _XPrePos[i];
                    hv_Y_PixelPos[i] = _YPrePos[i];
                }
                HOperatorSet.AffineTransPoint2d(hv_HomMat2D, hv_X_PixelPos, hv_Y_PixelPos, out hv_X_RobPos, out hv_Y_RobPos);
                for (int i = 0; i < hv_X_RobPos.Length; i++)
                {
                    _XTransPos[i] = hv_X_RobPos[i].F;
                    _YTransPos[i] = hv_Y_RobPos[i].F;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region 坐标变换
        /// <summary>
        /// coordTransform:坐标变换
        /// </summary>
        /// <param name="hv_HomMat2D">HTuple,变换矩阵</param>
        /// <param name="_XPrePos">float,要变换的X坐标</param>
        /// <param name="_YPrePos">float,要变换的Y坐标</param>
        /// <param name="_XTransPos">float,变换后的X坐标</param>
        /// <param name="_YTransPos">float,变换后的Y坐标</param>
        /// <returns>bool:成功则返回true</returns>
        public bool coordTransform(HTuple hv_HomMat2D, float _XPrePos, float _YPrePos, ref float _XTransPos, ref float _YTransPos)
        {
            HTuple hv_X_PixelPos = new HTuple();
            HTuple hv_Y_PixelPos = new HTuple();
            HTuple hv_X_RobPos = new HTuple();
            HTuple hv_Y_RobPos = new HTuple();
            try
            {
                hv_X_PixelPos = _XPrePos;
                hv_Y_PixelPos = _YPrePos;
                HOperatorSet.AffineTransPoint2d(hv_HomMat2D, hv_X_PixelPos, hv_Y_PixelPos, out hv_X_RobPos, out hv_Y_RobPos);
                _XTransPos = hv_X_RobPos[0].F;
                _YTransPos = hv_Y_RobPos[0].F;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

    }
}