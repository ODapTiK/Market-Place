namespace ProductService
{
    public interface IBackgroundTaskService
    {
        void Run(Func<Task> task);
    }
}
