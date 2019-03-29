using System.Runtime.InteropServices;
using System.Security;

namespace CheckShow
{
    [SuppressUnmanagedCodeSecurityAttribute]
    internal class SafeNativeMethods
    {                
        const string DllName = "UVSSSDK.dll";

        // UVSSInitialize: 初始化SDK (仅需调用一次)
        // 返回值: 1: 初始化成功 -1: 出错
        [DllImport(DllName, EntryPoint = "UVSSInitialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UVSSInitialize();

        // SetUVSSMessageCallback: 设置调试信息回调函数
        [DllImport(DllName, EntryPoint = "SetUVSSMessageCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetUVSSMessageCallback(Uvss.UVSSMessageDelegate callback);

        // SetUVSSCheckInfoCallback: 设置车辆检测信息回调函数
        [DllImport(DllName, EntryPoint = "SetUVSSCheckInfoCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetUVSSCheckInfoCallback(Uvss.UVSSCheckInfoDelegate callback);

        // UVSSIPAddress: 设置UVSS服务器IP地址, 默认值: 127.0.0.1
        // UVSSPort: 设置UVSS服务器端口号, 默认值: 20145
        // 返回值:
        // 连接成功: 大于0的连接标识 (handle)
        // 出错: -1
        // 已连接: -2 (不进行连接, 对已建立连接无影响)


        [DllImport(DllName, EntryPoint = "UVSSConnect", CallingConvention = CallingConvention.Cdecl, BestFitMapping = false)]
        public static extern int UVSSConnect(string UVSSIPAddress, int port);

        // UVSSDisconnect: 与服务器断开连接
        // handle: 连接标识 (由Connect返回, 用来对于服务器建立的连接进行标识)
        // 返回值: 1: 成功, -1: 无指定handle对应的连接
        [DllImport(DllName, EntryPoint = "UVSSDisconnect", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UVSSDisconnect(int handle);

        // UVSSUninitialize: 销毁SDK, 释放占用的资源 (仅在程序退出时调用一次)
        [DllImport(DllName, EntryPoint = "UVSSUninitialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void UVSSUninitialize();
    }
}
