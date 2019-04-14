using System;
using ContainelDll;

namespace CheckShow
{
    class Container_socket_DLL
    {
        public Action<bool>   SocketStatusCallBack=null;
        public Action<DateTime, string> LpnCallBack=null;
        public Action<DateTime, string,string> ConNumCallBack=null;
        public Action<string> MessageCallBack=null;

        /// <summary>
        /// 动态库对象
        /// </summary>
        private ContainelDll.Container _Container = null;

        public Container_socket_DLL(string Ip,int Port,int Intervals)
        {
            _Container = new ContainelDll.Container(Ip, Port, Intervals);
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
    }
}
