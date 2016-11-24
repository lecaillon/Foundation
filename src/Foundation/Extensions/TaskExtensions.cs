namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static Task<TDerived> Cast<T, TDerived>(this Task<T> task) where TDerived : T
        {
            var taskCompletionSource = new TaskCompletionSource<TDerived>();

            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    taskCompletionSource.TrySetException(t.Exception.InnerExceptions);
                }
                else if (t.IsCanceled)
                {
                    taskCompletionSource.TrySetCanceled();
                }
                else
                {
                    taskCompletionSource.TrySetResult((TDerived)t.Result);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return taskCompletionSource.Task;
        }
    }
}
