using System;
using Microsoft.Build.Framework;
using System.Collections.Generic;

namespace Microsoft.Extensions.ProjectModel.Internal
{
    internal class InMemoryLogger : ILogger
    {
        public InMemoryLogger()
        {
        }

        public IList<BuildErrorEventArgs> Errors = new List<BuildErrorEventArgs>();
        private readonly Stack<Action> _shutdown = new Stack<Action>();

        public string Parameters { get; set; }

        public LoggerVerbosity Verbosity { get; set; }
        public void Initialize(IEventSource eventSource)
        {
            eventSource.ErrorRaised += OnError;
            _shutdown.Push(() =>
            {
                eventSource.ErrorRaised -= OnError;
            });
        }

        private void OnError(object sender, BuildErrorEventArgs e)
        {
            Errors.Add(e);
        }

        public void Shutdown()
        {
            Action action;
            while(_shutdown.Count > 0)
            {
                action = _shutdown.Pop();
                action();
            }
        }
    }
}