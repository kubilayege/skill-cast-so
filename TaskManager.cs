using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

public static class TaskManager
{
    private static readonly List<CancellationTokenSource> CancellationTokens = new();

    public static UniTask Run(Action callback)
    {
        return Run(callback, out var token);
    }

    public static UniTask Run(Action callback, out CancellationTokenSource token)
    {
        token = CreateToken();
        return UniTask.RunOnThreadPool(callback, cancellationToken: token.Token);
    }
    
    public static UniTask Delay(int millisecondsDelay)
    {
        return Delay(millisecondsDelay, out var token);
    }     

    public static UniTask Delay(int millisecondsDelay, bool ignoreTimeScale = false)
    {
        return Delay(millisecondsDelay, out var token, ignoreTimeScale);
    }     
    
    public static UniTask Delay(int millisecondsDelay, out CancellationTokenSource token, bool ignoreTimeScale = false)
    {
        token = CreateToken();
        return UniTask.Delay(millisecondsDelay, cancellationToken: token.Token, ignoreTimeScale: ignoreTimeScale);
    }   

    public static UniTask WhenAll(IEnumerable<UniTask> tasks)
    {
        return WhenAll(tasks, out var token);
    }

    public static UniTask WhenAll(IEnumerable<UniTask> tasks, out CancellationTokenSource token)
    {
        token = CreateToken();
        var source = token;
        return UniTask.WhenAll(tasks).ContinueWith(() => source.IsCancellationRequested);
    }

    public static UniTask WaitUntil(Func<bool> callback)
    {
        return WaitUntil(callback, out var token);
    }

    public static UniTask WaitUntil(Func<bool> callback, out CancellationTokenSource token)
    {
        token = CreateToken();
        return UniTask.WaitUntil(callback, cancellationToken: token.Token);
    }

    public static UniTask<T> FromResult<T>(T value)
    {
        return FromResult(value, out var token);
    }

    public static UniTask<T> FromResult<T>(T value, out CancellationTokenSource token)
    {
        token = CreateToken();
        var source = token;

        var fromResult = UniTask.FromResult(value).AttachExternalCancellation(source.Token);

        return fromResult;
    }

    public static void Cancel(CancellationTokenSource token)
    {
        if (!CancellationTokens.Contains(token)) return;
        
        token.Cancel();
        CancellationTokens.Remove(token);
    }

    public static void CancelAll()
    {
        foreach (var token in CancellationTokens) 
            token.Cancel();
    }

    public static CancellationTokenSource CreateToken()
    {
        var token = new CancellationTokenSource();
        CancellationTokens.Add(token);
        return token;
    }
}