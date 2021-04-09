using System.Threading.Tasks;

namespace MacroFramework.Commands {
    internal class AsyncDynamicActivator : IDynamicActivator {
        public uint ID { get; }
        public bool IsCanceled { get; set; }
        public IActivator Activator { get; private set; }
        private TaskCompletionSource<bool> taskSource;

        public AsyncDynamicActivator(IActivator activator, TaskCompletionSource<bool> taskSource) {
            Activator = activator;
            this.taskSource = taskSource;
            ID = CommandContainer.UniqueDynamicActivatorID;
        }

        public void Execute() {
            taskSource.TrySetResult(true);
        }

        public bool RemoveAfterExecution() {
            return true;
        }

        public void OnRemove() {
            taskSource.TrySetResult(false);
        }
    }
}
