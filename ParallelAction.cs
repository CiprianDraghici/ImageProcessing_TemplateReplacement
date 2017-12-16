using System;
using System.Threading.Tasks.Dataflow;

namespace TemplateReplacement
{
    public class ParallelAction<T>
    {
        private int degreeParalelism;
        private Action<T> callback = (filePath) => { };
        private ActionBlock<T> workerBlock;

        public ParallelAction(int degreeParalelism, Action<T> callback)
        {
            this.degreeParalelism = degreeParalelism;
            this.callback = callback;

            ConfigureActionBlock();
        }

        public void Execute(T[] postMessages)
        {
            for (int i = 0; i < postMessages.Length; i++)
            {
                var filePath = postMessages[i];
                workerBlock.Post(filePath);
            }
            workerBlock.Complete();
            workerBlock.Completion.Wait();
        }

        private void ConfigureActionBlock()
        {
            var workerBlock = new ActionBlock<T>(
               filePath => this.callback(filePath),
               new ExecutionDataflowBlockOptions
               {
                   MaxDegreeOfParallelism = degreeParalelism
               });
            this.workerBlock = workerBlock;
        }
    }
}
