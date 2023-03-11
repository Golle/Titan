using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.XAudio2;

//NOTE(Jens): This is a mess, wrapping unmanaged vtable to callbacks on a struct. Maybe we can revisit this and see if it can be simplified.

public unsafe interface IXAudio2VoiceCallbackFunctions
{
    //NOTE(Jens): we have default implementations for all of these so they can just be ignored when not needed.
    void OnVoiceProcessingPassStart(uint bytesRequired){}
    void OnVoiceProcessingPassEnd() { }
    void OnStreamEnd() { }
    void OnBufferStart(void* pBufferContext) { }
    void OnBufferEnd(void* pBufferContext) { }
    void OnLoopEnd(void* pBufferContext) { }
    void OnVoiceError(void* pBufferContext, HRESULT error) { }
}

internal unsafe struct IXAudio2VoiceCallbackContext
{
    private delegate*<void*, uint, void> _onVoicePocessingPassStart;
    private delegate*<void*, void> _onVoiceProcessingPassEnd;
    private delegate*<void*, void> _onStreamEnd;
    private delegate*<void*, void*, void> _onBufferStart;
    private delegate*<void*, void*, void> _onBufferEnd;
    private delegate*<void*, void*, void> _onLoopEnd;
    private delegate*<void*, void*, HRESULT, void> _onVoiceError;
    private void* _context;
    public static IXAudio2VoiceCallbackContext Create<T>(T* context) where T : unmanaged, IXAudio2VoiceCallbackFunctions =>
        new()
        {
            _context = context,
            _onVoicePocessingPassStart = &IXAudio2VoiceCallbackFunctionsWrapper<T>.OnVoiceProcessingPassStart,
            _onVoiceProcessingPassEnd = &IXAudio2VoiceCallbackFunctionsWrapper<T>.OnVoiceProcessingPassEnd,
            _onStreamEnd = &IXAudio2VoiceCallbackFunctionsWrapper<T>.OnStreamEnd,
            _onBufferStart = &IXAudio2VoiceCallbackFunctionsWrapper<T>.OnBufferStart,
            _onBufferEnd = &IXAudio2VoiceCallbackFunctionsWrapper<T>.OnBufferEnd,
            _onLoopEnd = &IXAudio2VoiceCallbackFunctionsWrapper<T>.OnLoopEnd,
            _onVoiceError = &IXAudio2VoiceCallbackFunctionsWrapper<T>.OnVoiceError,
        };

    private struct IXAudio2VoiceCallbackFunctionsWrapper<T> where T : unmanaged, IXAudio2VoiceCallbackFunctions
    {
        public static void OnVoiceProcessingPassStart(void* context, uint bytesRequired)
            => ((T*)context)->OnVoiceProcessingPassStart(bytesRequired);
        public static void OnVoiceProcessingPassEnd(void* context)
            => ((T*)context)->OnVoiceProcessingPassEnd();
        public static void OnStreamEnd(void* context)
            => ((T*)context)->OnStreamEnd();
        public static void OnBufferStart(void* context, void* pBufferContext)
            => ((T*)context)->OnBufferStart(pBufferContext);
        public static void OnBufferEnd(void* context, void* pBufferContext)
            => ((T*)context)->OnBufferEnd(pBufferContext);
        public static void OnLoopEnd(void* context, void* pBufferContext)
            => ((T*)context)->OnLoopEnd(pBufferContext);
        public static void OnVoiceError(void* context, void* pBufferContext, HRESULT error)
            => ((T*)context)->OnVoiceError(pBufferContext, error);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnVoicePocessingPassStart(uint bytesRequired)
        => _onVoicePocessingPassStart(_context, bytesRequired);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnVoiceProcessingPassEnd()
        => _onVoiceProcessingPassEnd(_context);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnStreamEnd()
        => _onStreamEnd(_context);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnBufferStart(void* pBufferContext)
        => _onBufferStart(_context, pBufferContext);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnBufferEnd(void* pBufferContext)
        => _onBufferEnd(_context, pBufferContext);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnLoopEnd(void* pBufferContext)
        => _onLoopEnd(_context, pBufferContext);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnVoiceError(void* pBufferContext, HRESULT error)
        => _onVoiceError(_context, pBufferContext, error);
}

public unsafe struct IXAudio2VoiceCallback
{
    private static readonly Vtbl* _lpvtbl = InitVtbl();
#pragma warning disable CS0414
    private Vtbl* _vtbl;
#pragma warning restore CS0414

    private IXAudio2VoiceCallbackContext _context;

    public struct Vtbl
    {
        public delegate* unmanaged[Stdcall]<IXAudio2VoiceCallback*, uint, void> OnVoiceProcessingPassStart;
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
        callback->OnVoiceProcessingPassStart = &InternalOnVoiceProcessingPassStart;
        callback->OnVoiceProcessingPassEnd = &InternalOnVoiceProcessingPassEnd;
        return callback;
    }

    public static IXAudio2VoiceCallback Create<T>(T* context) where T : unmanaged, IXAudio2VoiceCallbackFunctions =>
        new()
        {
            _context = IXAudio2VoiceCallbackContext.Create(context),
            _vtbl = _lpvtbl
        };


    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void InternalOnVoiceProcessingPassStart(IXAudio2VoiceCallback* callback, uint bytesRequired)
        => callback->_context.OnVoicePocessingPassStart(bytesRequired);

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void InternalOnVoiceProcessingPassEnd(IXAudio2VoiceCallback* callback)
        => callback->_context.OnVoiceProcessingPassEnd();

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void InternalOnStreamEnd(IXAudio2VoiceCallback* callback)
        => callback->_context.OnStreamEnd();

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void InternalOnBufferStart(IXAudio2VoiceCallback* callback, void* pBufferContext)
        => callback->_context.OnBufferStart(pBufferContext);

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void InternalOnBufferEnd(IXAudio2VoiceCallback* callback, void* pBufferContext)
        => callback->_context.OnBufferEnd(pBufferContext);

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void InternalOnLoopEnd(IXAudio2VoiceCallback* callback, void* pBufferContext)
        => callback->_context.OnLoopEnd(pBufferContext);

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void InternalOnVoiceError(IXAudio2VoiceCallback* callback, void* pBufferContext, HRESULT error)
        => callback->_context.OnVoiceError(pBufferContext, error);
}
