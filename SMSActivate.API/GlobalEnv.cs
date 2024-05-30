global using uint8_t = System.Byte;
global using _BYTE = System.Byte;
global using int8_t = System.SByte;
global using __int8 = System.SByte;
global using sint8 = System.SByte;
global using uint16_t = System.UInt16;
global using _WORD = System.UInt16;
global using int16_t = System.Int16;
global using __int16 = System.Int16;
global using sint16 = System.Int16;
global using uint32_t = System.UInt32;
global using _DWORD = System.UInt32;
global using int32_t = System.Int32;
global using __int32 = System.Int32;
global using uint64_t = System.UInt64;
global using _QWORD = System.UInt64;
global using int64_t = System.Int64;
global using __int64 = System.Int64;    
global using intptr_t = System.IntPtr;
global using uintptr_t = System.UIntPtr;
global using size_t = System.IntPtr;

using SmsActivate.API.Network;

namespace SmsActivate.API
{
    internal static class GlobalEnv
    {        
        internal static readonly Client ApiClient = new();
    }
}
