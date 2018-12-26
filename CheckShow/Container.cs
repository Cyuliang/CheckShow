using AxVeconclientProj;
using System;

namespace CheckShow
{
    class Container: IDisposable
    {        
        private System.Threading.Timer _TimerConnect2Server=null;
        private bool _AutoLink = true;
        private AxVECONclient _AxVECONclient;

        public Action<string> SetMessage = null;
        public Action<bool> GetStatusAction = null;
        public Action<DateTime, string> LpnResult = null;//车牌结果触发插入数据库
        public Action<IVECONclientEvents_OnNewLPNEventEvent> NewLPNEvent=null;
        public Action<IVECONclientEvents_OnUpdateLPNEventEvent> UpdateLPNEvent=null;
        public Action<IVECONclientEvents_OnCombinedRecognitionResultISOEvent> CombinResult=null;


        public Container()
        {
            #region //箱号初始化
            _AxVECONclient = new AxVECONclient();
            _AxVECONclient.CreateControl();
            _AxVECONclient.ServerIPAddr = Properties.Settings.Default.Container_Ip;
            _AxVECONclient.ServerPort = Properties.Settings.Default.Container_Port;
            _AxVECONclient.OnServerConnected += _AxVECONclient_OnServerConnected;
            _AxVECONclient.OnServerDisconnected += _AxVECONclient_OnServerDisconnected;
            _AxVECONclient.OnServerError += _AxVECONclient_OnServerError;
            _AxVECONclient.OnNewLPNEvent += _AxVECONclient_OnNewLPNEvent;
            _AxVECONclient.OnUpdateLPNEvent += _AxVECONclient_OnUpdateLPNEvent;
            _AxVECONclient.OnIntermediateRecognitionResultISO += _AxVECONclient_OnIntermediateRecognitionResultISO;
            _AxVECONclient.OnCombinedRecognitionResultISO += _AxVECONclient_OnCombinedRecognitionResultISO;
            #endregion

            #region//断开定时器
            _TimerConnect2Server = new System.Threading.Timer(LinkCallBack, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(0));
            #endregion   
        }

        /// <summary>
        /// 定时回调链接
        /// </summary>
        /// <param name="o"></param>
        private void LinkCallBack(object o)
        {
            //Lognet.Log.Info("开始链接箱号识别系统");
            SetMessage?.Invoke("Link Container Init Start");
            _AxVECONclient.Connect2Server();
        }

        /// <summary>
        /// 链接箱号系统
        /// </summary>
        public void LinkC(int i)
        {
            Lognet.Log.Warn("箱号识别系统链接成功");
            SetMessage?.Invoke("Link Container Start");
            _AxVECONclient.Connect2Server();
            _AutoLink = true;
        }

        /// <summary>
        /// 主动断开链接
        /// </summary>
        public void CloseC(int i)
        {
            Lognet.Log.Warn("关闭链接箱号识别系统");
            SetMessage?.Invoke("Container Close");
            _AxVECONclient.Disconnect();
            _TimerConnect2Server.Change(-1,-1);
            _AutoLink = false;
        }

        /// <summary>
        /// 获取最后一次结果
        /// </summary>
        /// <param name="num">车道号</param>
        public void LastR(int num)
        {
            _AxVECONclient.SendLastResults(num);
            SetMessage?.Invoke(string.Format("Get {0} Last Result",num));
        }

        /// <summary>
        /// 链接错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AxVECONclient_OnServerError(object sender, System.EventArgs e)
        {
            //Lognet.Log.Info("箱号识别系统链接错误");
            SetMessage?.Invoke("Link Container Error");
            //_TimerConnect2Server.Change(TimeSpan.FromSeconds(5),TimeSpan.FromSeconds(0));
        }

        /// <summary>
        /// 链接断开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AxVECONclient_OnServerDisconnected(object sender, System.EventArgs e)
        {
            //Lognet.Log.Info("箱号识别系统链接失败");
            SetMessage?.Invoke("Link Container Disconnect");
            GetStatusAction?.Invoke(false);
            if(_AutoLink)
            {
                _TimerConnect2Server.Change(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(0));
            }
        }

        /// <summary>
        /// 链接成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AxVECONclient_OnServerConnected(object sender, System.EventArgs e)
        {
            Lognet.Log.Warn("箱号识别系统链接成功");
            SetMessage?.Invoke("Link Container Connected");
            GetStatusAction?.Invoke(true);
            _TimerConnect2Server.Change(-1,-1);
        }
        
        /// <summary>
        /// 中间结果事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AxVECONclient_OnIntermediateRecognitionResultISO(object sender, AxVeconclientProj.IVECONclientEvents_OnIntermediateRecognitionResultISOEvent e)
        {
            //Intermediate?.Invoke(e);
            SetMessage?.Invoke(string.Format("Video：{0} ContainerNum：{1} CheckSum：{2}",e.videoID,e.containerNum,e.checkSum));
        }

        /// <summary>
        /// 箱号事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AxVECONclient_OnCombinedRecognitionResultISO(object sender, AxVeconclientProj.IVECONclientEvents_OnCombinedRecognitionResultISOEvent e)
        {
            CombinResult?.Invoke(e);
            SetMessage?.Invoke(string.Format("CombinResult1：{0} CombinResult2：{1}", e.containerNum1,e.containerNum2));
            Lognet.Log.Info(string.Format("DateTimt：{0} CombinResult1：{1} CombinResult2：{2}",e.triggerTime.ToString("yyyy-MM-dd HH:mm:ss"), e.containerNum1,e.containerNum2));
        }

        /// <summary>
        /// 重车车牌事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AxVECONclient_OnUpdateLPNEvent(object sender, AxVeconclientProj.IVECONclientEvents_OnUpdateLPNEventEvent e)
        {
            UpdateLPNEvent?.Invoke(e);
            SetMessage?.Invoke(string.Format("UpdateLPN：{0}", e.lPN));
            Lognet.Log.Info(string.Format("DateTime：{0} UpdateLPN：{1}", e.triggerTime.ToString("yyyy-MM-dd HH:mm:ss"), e.lPN));
            LpnResult(e.triggerTime,e.lPN);
        }

        /// <summary>
        /// 空车车牌事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _AxVECONclient_OnNewLPNEvent(object sender, AxVeconclientProj.IVECONclientEvents_OnNewLPNEventEvent e)
        {
            NewLPNEvent?.Invoke(e);
            SetMessage?.Invoke(string.Format("NewLPN：{0}", e.lPN));
            Lognet.Log.Info(string.Format("DateTime：{0} NewLPN：{1}", e.triggerTime.ToString("yyyy-MM-dd HH:mm:ss"), e.lPN));
            LpnResult(e.triggerTime, e.lPN);
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
                    _TimerConnect2Server.Dispose();
                    _AxVECONclient.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~Container() {
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
