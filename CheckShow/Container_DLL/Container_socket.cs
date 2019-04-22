using System;
using Container_Socket_DLL;

namespace CheckShow
{
    class Container_socket_DLL:IDisposable
    {
        public Action<bool>   SocketStatusCallBack=null;
        public Action<DateTime, string> LpnCallBack=null;
        public Action<DateTime, string,string> ConNumCallBack=null;
        public Action<string> MessageCallBack=null;

        /// <summary>
        /// 动态库对象
        /// </summary>
        private Container_Socket_DLL.Container_Socket _Container = null;        
        

        public Container_socket_DLL(string Ip,int Port,int Intervals,string Localip,int Localoport)
        {
            _Container = new Container_Socket_DLL.Container_Socket(Ip, Port, Intervals, Local_Ip_bing: Localip, Local_Port_bing: Localoport);
            _Container.NewLpnEvent += _Container_NewLpnEvent;
            _Container.UpdateLpnEvent += _Container_UpdateLpnEvent;
            _Container.ConNumEvent += _Container_ConNumEvent;
            _Container.MessageEvent += _Container_MessageEvent;
            _Container.SocketStatusEvent += _Container_SocketStatusEvent;
        }

        /// <summary>
        ///状态事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Container_SocketStatusEvent(object sender, SocketStatusEventArgs e)
        {
            SocketStatusCallBack?.Invoke(e.Status);
        }

        /// <summary>
        /// 消息事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Container_MessageEvent(object sender, MessageEventArgs e)
        {
            MessageCallBack?.Invoke(string.Format("{0}[{1}]", e.FunName, e.Message));
        }

        /// <summary>
        /// 箱号事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Container_ConNumEvent(object sender, ConNumEventArgs e)
        {
            ConNumCallBack?.Invoke(e.TriggerTime, e.ContainerNum1,e.CheckNum1);
        }

        /// <summary>
        /// 重车车牌事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Container_UpdateLpnEvent(object sender, UpdateLpnEventArgs e)
        {
            LpnCallBack?.Invoke(e.TriggerTime,e.Lpn);
        }

        /// <summary>
        /// 空车车牌事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Container_NewLpnEvent(object sender, NewLpnEventArgs e)
        {
            LpnCallBack?.Invoke(e.TriggerTime,e.Lpn);
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
                    //_Container.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~Container_socket_DLL() {
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
