using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Web.Script;
using System.Threading;
using System.Net;
using System.IO;
using System.Web;
using FDSReader;
namespace FDSDemo
{
    public partial class Main : Form
    {
        int ServicePort = 801;
        Thread HTTP = null;
        
        public Main()
        {
            InitializeComponent();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            
            FDSReader.FDSReader.FileName = textBox1.Text;
            //FDSReader.struCommand[] info = FDSDemo.FDSReader.ProcCommand( FDSReader.PreProcCommand());
            string ttmp = FDSReader.FDSReader.Get3DObject();
            string tmp = FDSReader.FDSReader.GetCommandNote();
            System.IO.File.WriteAllText(@"c:\data\json.txt", tmp);
            // foreach(FDSReader.struCommand command in info)
            {
                //listBox1.Items.Add(command);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            init();
            DoStart();
        }

        void init()
        {
            PhyCommon.SystemLog.SetShowLog(true);
            PhyCommon.SystemLog.WirteFile = true;
            PhyCommon.SystemLog.SetTextbox(txtLog);
            PhyCommon.SystemLog.Write(txtLog, "初始化日志代理成功。", true);
        }
        void DoStart()
        {
            //启动日志代理端口
            
            try
            {
                ServicePort = PhyCommon.IniFiles.ReadInteger(Application.StartupPath + "\\config.ini", "Setup", "ServicePort", 801);
                if (ServicePort != 0)
                {
                    HTTP = new Thread(new ThreadStart(StartListener));
                    HTTP.Start();
                    //PhyCommon.SystemLog.Write(txtLog, "初始化监听端口成功："+ServicePort, true);
                }
            }
            catch (Exception ex){ PhyCommon.SystemLog.Write(txtLog, "初始化监听端口失败：" + ServicePort +" "+ex.Message, true); }
        }

        void StartListener()
        {
            Thread.CurrentThread.IsBackground = true;
            HttpListener lis = new HttpListener();
            lis.Prefixes.Add("http://*:" + ServicePort.ToString() + "/");
            bool isLis = false;
            while (!isLis)
            {
                try
                {
                    lis.Start();
                    isLis = true;
                }
                catch (Exception ex){
                    PhyCommon.SystemLog.Write(txtLog, "监听端口失败：" + ex.Message, true);
                    Thread.Sleep(5000);
                }

            }
            

            while (true)
            {
                HttpListenerContext context = lis.GetContext();
                //将其处理过程放入线程池
                System.Threading.ThreadPool.QueueUserWorkItem(ListenerCallback, context);

            }
        }
        public void ListenerCallback(object obj)
        {
            HttpListenerContext context = obj as HttpListenerContext;
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            string responseString = string.Empty;

            /*
            地址：http://121.40.31.30:801
            调用方式：POST
            参数说明：
                action  "ftom"为FDS文件转为中间格式
                type    "json"输出json格式   
                        "xml"输出XML格式
                data    原始文件， ftom命令下为FDS源文件。
    */
            try
            {
                if (request.HttpMethod.ToLower().Equals("post"))
                {
                    Stream SourceStream = request.InputStream;
                    byte[] currentChunk = ReadLineAsBytes(SourceStream);
                    //获取数据中有空白符需要去掉，输出的就是post请求的参数字符串 如：username=linezero
                    string[] temp = Encoding.UTF8.GetString(currentChunk).Split('&');
                    Dictionary<string, string> Dict = new Dictionary<string, string>();
                    foreach (string info in temp)
                    {
                        string[] str = info.Split('=');
                        if (str.Length != 2) continue;
                        Dict.Add(str[0].Trim().ToLower(), str[1].Trim());
                    }
                    if (Dict.Count != 0)
                    {
                        string action = string.Empty;
                        if (Dict.TryGetValue("action", out action))
                        {
                            if (action == "ftom")
                            {
                                responseString = ftom(Dict);
                            }
                            if (action == "cmdlist")
                            {
                                responseString =FDSReader.FDSReader.GetCommandNote();
                            }

                        }
                    }

                }

            }
            catch(Exception ex)
            {
                responseString = ex.Message;
            }


                //do something as you want
                
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            //关闭输出流，释放相应资源
            output.Close();
        }


        string ftom(Dictionary<string,string> Dict)
        {
            string ReturnStr = string.Empty;
            string data = string.Empty;
            if (Dict.TryGetValue("data", out data))
            {
                data = System.Web.HttpUtility.UrlDecode(data, Encoding.UTF8);
                string temp = string.Empty;
                Dict.TryGetValue("type", out temp);
                if (string.IsNullOrEmpty(temp)) temp = "json";
                ReturnStr = FDSReader.FDSReader.Get3DObject(data,temp);
            }

            return ReturnStr;
        }

        byte[] ReadLineAsBytes(Stream SourceStream)
        {
            var resultStream = new MemoryStream();
            while (true)
            {
                int data = SourceStream.ReadByte();
                resultStream.WriteByte((byte)data);
                if (data <= 10)
                    break;
            }
            resultStream.Position = 0;
            byte[] dataBytes = new byte[resultStream.Length];
            resultStream.Read(dataBytes, 0, dataBytes.Length);
            return dataBytes;
        }



        byte[] SS_NewBuffer(object sender, string Command, byte[] e, out byte[] SendBack)
        {
            SendBack = new byte[0];
            string tmp = UTF8Encoding.UTF8.GetString(e);
            Console.WriteLine(e.Length.ToString());
            Console.WriteLine(tmp.ToString());
            string GetStr = "";
            try
            {
                GetStr = tmp.Substring(5, tmp.IndexOf("HTTP") - 5).Trim().ToLower();
            }
            catch { }

            return new byte[0];
        }



        private void button2_Click(object sender, EventArgs e)
        {
            string temp = @"C:\Data\info.json";
            temp = System.IO.File.ReadAllText(temp);
            System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            object o = Serializer.DeserializeObject(temp);
            Dictionary<string, object> t = (Dictionary<string, object>)o;

          //  stru3DObj obj = Serializer.Deserialize<stru3DObj>(temp);

            //string t = obj.components[0].objects[0].pos.ToString();
            string json = Serializer.Serialize(temp);

        }
    }
}
