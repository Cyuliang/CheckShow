using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CheckShow
{
    class Uvss:IDisposable
    {
        public Action<string> MessageAction;//车底信息回掉函数
        public Action<bool> LinkStatusAction;//车底相机链接状态

        private System.Threading.Timer _TimerLink = null;
        private System.Threading.Timer _TimerTestLink = null;

        private readonly string UVSSIp = Properties.Settings.Default.UVSSIp;
        private readonly int UVSSPort = Properties.Settings.Default.UVSSPort;

        private IPAddress ip = null;
        private Socket client = null;

        // SDK回调函数类型:

        // UVSSMessageDelegate: 调试信息回调函数定义
        // code: 信息编号
        // 1: 连接成功 (UVSSConnect)
        // -1: 初始化错误 (UVSSInitialize)
        // -2: 连接失败 (UVSSConnect)
        // -3: 连接断开
        // message: 信息描述
        public delegate void UVSSMessageDelegate(int handle, int code, string message);

        // UVSSCheckInfoDelegate: 车辆检查信息回调函数定义
        // handle: 连接标识 (由Connect返回, 用来对于服务器建立的连接进行标识)
        // UVSSImagePath: 车底图像路径
        // PlateImagePath: 车牌图像路径
        // Channel: 车检通道信息
        // PlateNumber: 车牌号码
        // Direction: 车辆方向
        // CheckDateTime: 检查日期时间
        // Extension: 扩展信息
        public delegate void UVSSCheckInfoDelegate(int handle, string UVSSImagePath, string PlateImagePath, string Channel, string PlateNumber, string Direction, string CheckDateTime, string Extension);

        // 更新UI delegate:
        public delegate void UpdateMessageDelegate(int handle, int code, string message);
        public delegate void UpdateCheckInfoDelegate(int handle, string UVSSImagePath, string PlateImagePath, string Channel, string PlateNumber, string Direction, string CheckDateTime);

        public static  UVSSMessageDelegate MessageCallback;
        public static  UVSSCheckInfoDelegate CheckInfoCallback;

        public Uvss()
        {
            if (SafeNativeMethods.UVSSInitialize()==-1)
            {
                Lognet.Log.Warn("初始化车底动态库错误");
            }
            else
            {
                _TimerLink = new Timer(AutoLinkCallBack, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
                _TimerTestLink = new Timer(TestLinCallBack, null,-1,-1);
            }
            CheckInfoCallback = UVSSCheckInfoCallBack;
            MessageCallback = UVSSMessageCallback;

            SafeNativeMethods.SetUVSSMessageCallback(MessageCallback);
            SafeNativeMethods.SetUVSSCheckInfoCallback(CheckInfoCallback);
        }


        //[DllImport("kernel32.dll")]
        //static extern uint GetTickCount();        
        //static void Delay(int ms)
        //{
        //    uint start = GetTickCount();
        //    while (GetTickCount() - start < ms)
        //    {
        //        Application.DoEvents();
        //    }
        //}

        /// <summary>
        /// 循环测试链接状态
        /// </summary>
        /// <param name="state"></param>
        private void TestLinCallBack(object state)
        {
            while(true)
            {
                try
                {
                    client.Send(Encoding.ASCII.GetBytes(""));
                    _TimerLink?.Change(-1, -1);
                    LinkStatusAction?.Invoke(true);
                    Thread.Sleep(1000);
                }
                catch (Exception)
                {
                    Lognet.Log.Warn("车底系统端服务关闭");
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    _TimerLink?.Change(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
                    LinkStatusAction?.Invoke(false);
                    break;
                }
            }        
        }

        /// <summary>
        /// 链接车底系统
        /// </summary>
        /// <param name="state"></param>
        private void AutoLinkCallBack(object state)
        {
            int ret = SafeNativeMethods.UVSSConnect(UVSSIp, UVSSPort);
            if(ret>0)
            {
                _TimerLink?.Change(-1, -1);

                ip = IPAddress.Parse(UVSSIp);
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(new IPEndPoint(ip, UVSSPort));//链接测试socket

                _TimerTestLink?.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
                LinkStatusAction?.Invoke(true);

                Lognet.Log.Warn("车底系统链接成功");
            }
            else
            {
                SafeNativeMethods.UVSSDisconnect(ret);
                _TimerLink?.Change(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
                //_TimerTestLink.Change(-1, -1);
                //LinkStatusAction?.Invoke(false);
            }
        }

        /// <summary>
        /// 车辆检测信息回掉
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="UVSSImagePath"></param>
        /// <param name="PlateImagePath"></param>
        /// <param name="Channel"></param>
        /// <param name="PlateNumber"></param>
        /// <param name="Direction"></param>
        /// <param name="CheckDateTime"></param>
        /// <param name="Extension"></param>
        private void UVSSCheckInfoCallBack(int handle, string UVSSImagePath, string PlateImagePath, string Channel, string PlateNumber, string Direction, string CheckDateTime, string Extension)
        {
            MessageAction?.Invoke(UVSSImagePath);
        }

        /// <summary>
        /// 调试信息回掉
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        private void UVSSMessageCallback(int handle, int code, string message)
        {
            //throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    //client?.Dispose();
                    _TimerLink.Dispose();
                    _TimerTestLink.Dispose();
                    client?.Dispose();
                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                SafeNativeMethods.UVSSUninitialize();
                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~Uvss() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
