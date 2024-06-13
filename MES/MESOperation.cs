using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace MES
{
    public class MESOperation
    {
        public string serverlURL = "http://MYCSMTEQUAPI.MFLEX.COM.CN";
        public string paneleCheckEntryPath = "/api/Equipments/checkpanel";
        public string paneleCheckExitPath = "/api/Equipments/saveresultpanel";

        #region Demo
        public string mestrySerializationResult;
        public void MEStry()
        {
            string personJson = "{ 'FirstName': '小坦克','LastName':'Tank xiao', 'Age':'30', 'Books':[{'BookName':'c#', 'Price':'29.9'},{'BookName':'Mac编程', 'Price':'39.9'}]}";

            string allMoveJson = @"[{ 'FirstName': '小坦克','LastName':'Tank xiao', 'Age':'30', 'Books':[{'BookName':'c#', 'Price':'29.9'},{'BookName':'Mac编程', 'Price':'39.9'}]},{
               'FirstName': '小坦克2','LastName':'Tank xiao2', 'Age':'40', 'Books':[{'BookName':'c#', 'Price':'29.9'},{'BookName':'Mac编程', 'Price':'39.9'}]}]";

            // 反序列化 单个对象
            Person oneMovie = JsonConvert.DeserializeObject<Person>(personJson);

            // 反序列化 对象集合
            List<Person> allMovie = JsonConvert.DeserializeObject<List<Person>>(allMoveJson);

            //Console.WriteLine(oneMovie.FirstName);
            //Console.WriteLine(allMovie[1].FirstName);

            // 序列化
            mestrySerializationResult = JsonConvert.SerializeObject(allMovie);
        }

        public class Person
        {
            public String FirstName
            { get; set; }

            public String LastName
            { get; set; }

            public int Age
            { get; set; }

            public List<Book> Books
            { get; set; }
        }

        public class Book
        {
            public string BookName
            { get; set; }

            public string Price
            { get; set; }
        }

        #endregion

        #region Panels

        #region deserializationPanelsCheckIn
        public PanelsCheckIn deserializationPanelsCheckIn(string panelsCheckIn)
        {
            return JsonConvert.DeserializeObject<PanelsCheckIn>(panelsCheckIn);
        }
        #endregion

        #region serializationPanelsCheckIn
        public string serializationPanelsCheckIn(PanelsCheckIn panelsCheckIn)
        {
            return JsonConvert.SerializeObject(panelsCheckIn);
        }
        #endregion

        #region deserializationPanelsCheckInResult
        public PanelsCheckInResult deserializationPanelsCheckInResult(string panelsCheckInResult)
        {
            return JsonConvert.DeserializeObject<PanelsCheckInResult>(panelsCheckInResult);
        }
        #endregion

        #region deserializationPanelsCheckOut
        public PanelsCheckOut deserializationPanelsCheckOut(string panelsCheckOut)
        {
            return JsonConvert.DeserializeObject<PanelsCheckOut>(panelsCheckOut);
        }
        #endregion

        #region serializationPanelsCheckOut
        public string serializationPanelsCheckOut(PanelsCheckOut panelsCheckOut)
        {
            return JsonConvert.SerializeObject(panelsCheckOut);
        }
        #endregion

        #region deserializationPanelsCheckOutResult
        public PanelsCheckOutResult deserializationPanelsCheckOutResult(string panelsCheckOutResult)
        {
            return JsonConvert.DeserializeObject<PanelsCheckOutResult>(panelsCheckOutResult);
        }
        #endregion

        #region Panels
        public class PanelsCheckIn
        {
            public string Panel //条码
            { get; set; }
            public string Resource //线体,无线体时线体为机台
            { get; set; }
            public string Machine //机台
            { get; set; }
            public string Product //Panel对应的产品
            { get; set; }
            public string OperatorName //操作员
            { get; set; }
            public string WorkArea //工位
            { get; set; }
            public string TestType //测试类型
            { get; set; }
            public string Site //位置，苏州：SMT，盐城：YCSMT
            { get; set; }
            public string Mac //机台MAC地址
            { get; set; }
            public string ProgramName //程序名
            { get; set; }
            public string IpAddress //机台ip地址
            { get; set; }
            public string OptTime //操作时间
            { get; set; }
        }

        public class PanelsCheckInResult
        {
            public string result //结果:0成功,其它失败
            { get; set; }
            //public string message //失败原因说明
            //{ get; set; }
            //public DateTime currentTime //返回的当前时间
            //{ get; set; }
        }

        public class PanelsCheckOut
        {
            public string Panel //条码
            { get; set; }
            public string Resource //线体,无线体时线体为机台
            { get; set; }
            public string Machine //机台
            { get; set; }
            public string Product //Panel对应的产品
            { get; set; }
            public string WorkArea //工位
            { get; set; }
            public string TestType //测试类型
            { get; set; }
            public string TestTime //操作时间
            { get; set; }
            public string OperatorName //操作员
            { get; set; }
            public string verifyOperatorName//复判人员
            { get; set; }
            public string OperatorType //操作类型
            { get; set; }
            public string testMode //操作模式，PANEL/PCS
            { get; set; }
            public string Site //位置，苏州：SMT，盐城：YCSMT
            { get; set; }
            public string Mac //机台MAC地址
            { get; set; }
            public string ProgramName //程序名
            { get; set; }
            public string trackType //轨道L Left Track, R Right Track, S Single Track
            { get; set; }
            public string hasTrackFlag //1需要过站，0不需要过站
            { get; set; }
            public string IpAddress //机台ip地址
            { get; set; }
            public string CarrierId //高温板
            { get; set; }
            public string CoverPlate //盖片
            { get; set; }
           // public detaillist detaillist
            //{ get; set; }
            //public summarylist summarylist
           // { get; set; }
        }

        public class detaillist
        {
            public string PanelId //Panel号
            { get; set; }
            public string testType //测试类型
            { get; set; }
            public string pcsSeq //pcs位置
            { get; set; }
            public string partSeq //元件位置
            { get; set; }
            public string PinSeq //L/R
            { get; set; }
            public string testResult //测试结果
            { get; set; }
            public string operatorName //测试人员
            { get; set; }
            public string verifyOperatorName//复判人员
            { get; set; }
            public string defectCode //缺陷代码,pass时为空
            { get; set; }
            public string testFile //文件位置
            { get; set; }
            public string imagePath //图片地址
            { get; set; }
        }


        public class summarylist
        {
            public string PanelId //Panel号
            { get; set; }
            public string pcsSeq //pcs位置
            { get; set; }
            public string testResult //测试结果
            { get; set; }
            public string operatorName //测试人员
            { get; set; }
            public string verifyOperatorName//复判人员
            { get; set; }
            public string operatorTime //测试时间
            { get; set; }
        }

        public class PanelsCheckOutResult
        {
            public string result //结果:0成功,其它失败
            { get; set; }
            public string message //失败原因说明
            { get; set; }
            public DateTime currentTime //返回的当前时间
            { get; set; }
        }

        #endregion

        #endregion

        #region UserName

        #region serializationUserName
        public string serializationUserName(UserName user)
        {
            return JsonConvert.SerializeObject(user);
        }
        #endregion

        #region deserializationUserNameResult
        public UserNameResult deserializationUserNameResult(string userNameResult)
        {
            return JsonConvert.DeserializeObject<UserNameResult>(userNameResult);
        }
        #endregion

        #region User
        public class UserName
        {
            public string Username //用户名称
            { get; set; }
        }

        public class UserNameResult
        {
            public string result //结果:0成功,其它失败
            { get; set; }
            public string message //失败原因说明
            { get; set; }
            public DateTime currentTime //返回的当前时间
            { get; set; }
        }

        #endregion

        #endregion

        #region Get
        /// <summary>
        /// GET传值
        /// </summary>
        /// <param name="Url">传递的地址</param>
        /// <param name="getDataStr">传递的内容（如：a=1&b=2&c=3）</param>
        /// <returns>返回结果</returns>
        public string HttpGet(string Url, string getDataStr)
        {
            string retString = string.Empty;
            try
            {
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (getDataStr == "" ? "" : "?") + getDataStr);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + "?" + getDataStr);
                request.Method = "GET";
                request.ContentType = "application/json"; //设置内容类型
                request.Timeout = 3000;//设置超时时间

                //IPAddress ip = IPAddress.Parse("10.13.28.57");
                //request.ServicePoint.BindIPEndPointDelegate = delegate
                //{
                //    return new IPEndPoint(ip, 0);
                //};
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();//返回响应
                Stream myResponseStream = response.GetResponseStream();//获得响应流
                //StreamReader streamReader = new StreamReader(myResponseStream);
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);//以UTF8编码方式读取该流
                retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// GET传值
        /// </summary>
        /// <param name="Url">传递的地址</param>
        /// <param name="getDataStr">传递的内容（如：a=1&b=2&c=3）</param>
        /// <param name="result">反馈回来的值</param>
        /// <returns>返回结果，false-未能建立链接，true-成功建立链接</returns>
        public bool HttpGet(string Url, string getDataStr,out string result)
        {
            string retString = string.Empty;
            try
            {
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (getDataStr == "" ? "" : "?") + getDataStr);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + "?" + getDataStr);
                request.Method = "GET";
                request.ContentType = "application/json"; //设置内容类型
                request.Timeout = 3000;//设置超时时间

                //IPAddress ip = IPAddress.Parse("10.13.28.57");
                //request.ServicePoint.BindIPEndPointDelegate = delegate
                //{
                //    return new IPEndPoint(ip, 0);
                //};
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();//返回响应
                Stream myResponseStream = response.GetResponseStream();//获得响应流
                //StreamReader streamReader = new StreamReader(myResponseStream);
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);//以UTF8编码方式读取该流
                retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                result = retString;
                return true;
            }
            catch (Exception ex)
            {
                //throw ex;
                result = ex.ToString();
                return false;
            }
        } 
        #endregion
 
        #region Post
        /// <summary>
        /// POST传值
        /// </summary>
        /// <param name="Url">传递的地址</param>
        /// <param name="postDataStr">传递的内容（如：a=1&b=2&c=3）</param>
        /// <returns>返回结果</returns>
        public string HttpPost(string Url, string postDataStr)
        {
            string retString = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 3000;//设置超时时间

                //固定IP地址
                //IPAddress ip = IPAddress.Parse("10.13.28.57");
                //request.ServicePoint.BindIPEndPointDelegate = delegate
                //{
                //    return new IPEndPoint(ip, 0);
                //};
                //request.ContentLength = postDataStr.Length;
                request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
                Stream requestStream = request.GetRequestStream();
                //StreamWriter writer = new StreamWriter(requestStream);
                StreamWriter writer = new StreamWriter(requestStream, Encoding.ASCII);
                writer.Write(postDataStr);
                writer.Flush();
                writer.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码 
                }
                Stream responseStream = response.GetResponseStream();
                //StreamReader reader = new StreamReader(responseStream);
                StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding(encoding));
                retString = reader.ReadToEnd();
                reader.Close();
                responseStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
                return ex.ToString();
            }
            return retString;
        }

        /// <summary>
        /// POST传值
        /// </summary>
        /// <param name="Url">传递的地址</param>
        /// <param name="postDataStr">传递的内容（如：a=1&b=2&c=3）</param>
        /// <param name="result">反馈回来的数据值</param>
        /// <returns>bool:返回结果,false-未能建立链接，true-成功建立链接</returns>
        public bool HttpPost(string Url, string postDataStr,out string result)
        {
            string retString = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 3000;//设置超时时间

                //固定IP地址
                //IPAddress ip = IPAddress.Parse("10.13.28.57");
                //request.ServicePoint.BindIPEndPointDelegate = delegate
                //{
                //    return new IPEndPoint(ip, 0);
                //};
                //request.ContentLength = postDataStr.Length;
                request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
                Stream requestStream = request.GetRequestStream();
                //StreamWriter writer = new StreamWriter(requestStream);
                StreamWriter writer = new StreamWriter(requestStream, Encoding.ASCII);
                writer.Write(postDataStr);
                writer.Flush();
                writer.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码 
                }
                Stream responseStream = response.GetResponseStream();
                //StreamReader reader = new StreamReader(responseStream);
                StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding(encoding));
                retString = reader.ReadToEnd();
                reader.Close();
                responseStream.Close();
            }
            catch (Exception ex)
            {
                //throw ex;
                result = ex.ToString();
                return false;
            }
            result = retString;
            return true;
        }

        //public static string Post(string Url, string Data, ref MachineStatusResponse res)
        //{
        //    try
        //    {
        //        HttpContent httpContent = new StringContent(Data);
        //        httpContent.Headers.ContentType = new MediaTypeHeaderValue(@"text/json");
        //        HttpClient httpClient = new HttpClient();

        //        HttpResponseMessage response = httpClient.PostAsync(Url, httpContent).Result;

        //        if (response.IsSuccessStatusCode)
        //        {
        //            try
        //            {
        //                Task<string> t = response.Content.ReadAsStringAsync();
        //                CommonSet.WriteInfo("Post——MachineStatusResponse接收数据" + t.Result);
        //                res = JsonConvert.DeserializeObject<MachineStatusResponse>(t.Result);
        //                return string.Empty;
        //            }
        //            catch (Exception)
        //            {


        //            }

        //            //zhuan json            
        //        }
        //    }
        //    catch (Exception)
        //    {


        //    }
        //    return string.Empty;
        //}

        #endregion
    }
}
