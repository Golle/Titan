using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core.Memory;

namespace Titan.Windows.XAudio2;

public unsafe struct IXAudio2VoiceCallback
{
    private static readonly Vtbl* _lpvtbl = InitVtbl();
    private Vtbl* _vtbl;
    private GCHandle _handle;

    public interface Interface
    {
        void OnVoiceProcessingPassStart(uint bytesRequired);
        void OnVoiceProcessingPassEnd();
        void OnStreamEnd();
        void OnBufferStart(void* pBufferContext);
        void OnBufferEnd(void* pBufferContext);
        void OnLoopEnd(void* pBufferContext);
        void OnVoiceError(void* pBufferContext, HRESULT error);
    }

    public struct Vtbl
    {
        public delegate* unmanaged[Stdcall]<IXAudio2VoiceCallback*, uint, void> OnVoicePocessingPassStart;
        public delegate* unmanaged[Stdcall]<IXAudio2VoiceCallback*, void> OnVoiceProcessingPassEnd;
        public delegate* unmanaged[Stdcall]<IXAudio2VoiceCallback*, void> OnStreamEnd;
        public delegate* unmanaged[Stdcall]<IXAudio2VoiceCallback*, void*, void> OnBufferStart;
        public delegate* unmanaged[Stdcall]<IXAudio2VoiceCallback*, void*, void> OnBufferEnd;
        public delegate* unmanaged[Stdcall]<IXAudio2VoiceCallback*, void*, void> OnLoopEnd;
        public delegate* unmanaged[Stdcall]<IXAudio2VoiceCallback*, void*, HRESULT, void> OnVoiceError;
    }

    private static Vtbl* InitVtbl()
    {
        var callback = (Vtbl*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(Vtbl), sizeof(Vtbl));
        callback->OnBufferStart = &InternalOnBufferStart;
        callback->OnBufferEnd = &InternalOnBufferEnd;
        callback->OnLoopEnd = &InternalOnLoopEnd;
        callback->OnStreamEnd = &InternalOnStreamEnd;
        callback->OnVoiceError = &InternalOnVoiceError;
        callback->OnVoicePocessingPassStart = &InternalOnVoicePocessingPassStart;
        callback->OnVoiceProcessingPassEnd = &InternalOnVoiceProcessingPassEnd;
        return callback;
    }

    public static IXAudio2VoiceCallback* Create(Interface obj)
    {
        IXAudio2VoiceCallback* callback = MemoryUtils.AllocateBlock<IXAudio2VoiceCallback>(1);
        callback->_handle= GCHandle.Alloc(obj);
        callback->_vtbl = _lpvtbl;
        return callback;
    }

    public static void Free(IXAudio2VoiceCallback* callback)
    {
        if (callback != null)
        {
            if (callback->_handle.IsAllocated)
            {
                callback->_handle.Free();
            }
            ((MemoryChunk<IXAudio2VoiceCallback>)callback).Free();
        }
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void InternalOnVoicePocessingPassStart(IXAudio2VoiceCallback* callback, uint bytesRequired) 
        => ((Interface)callback->_handle.Target)?.OnVoiceProcessingPassStart(bytesRequired);

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void InternalOnVoiceProcessingPassEnd(IXAudio2VoiceCallback* callback)
        => ((Interface)callback->_handle.Target)?.OnVoiceProcessingPassEnd();

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void InternalOnStreamEnd(IXAudio2VoiceCallback* callback)
        => ((Interface)callback->_handle.Target)?.OnStreamEnd();

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void InternalOnBufferStart(IXAudio2VoiceCallback* callback, void* pBufferContext)
        => ((Interface)callback->_handle.Target)?.OnBufferStart(pBufferContext);

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void InternalOnBufferEnd(IXAudio2VoiceCallback* callback, void* pBufferContext)
        => ((Interface)callback->_handle.Target)?.OnBufferEnd(pBufferContext);

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void InternalOnLoopEnd(IXAudio2VoiceCallback* callback, void* pBufferContext)
        => ((Interface)callback->_handle.Target)?.OnLoopEnd(pBufferContext);

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void InternalOnVoiceError(IXAudio2VoiceCallback* callback, void* pBufferContext, HRESULT error)
        => ((Interface)callback->_handle.Target)?.OnVoiceError(pBufferContext, error);
}

