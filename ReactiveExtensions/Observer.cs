using System;

namespace ReactiveExtensions
{
    public class Observer<T> : IObserver<T>
    {
        public void OnNext(T value)
        {
            Console.WriteLine("Value: " + value);
        }

        public void OnError(Exception error)
        {
            Console.WriteLine("Error: " + error);

        }

        public void OnCompleted()
        {
            Console.WriteLine("Completed");
        }
    }
}