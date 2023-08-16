using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns
{
    public interface IResiliencePolicyHandler
    {
        Task ExecuteAsync(Func<Task> asyncAction);
    }

    public class ResiliancePolicyHandler : IResiliencePolicyHandler
    {
        private readonly List<IResiliencePolicy> policies = new List<IResiliencePolicy>();

        public void AddPolicy(IResiliencePolicy policy)
        {
            policies.Add(policy);
        }

        public async Task ExecuteAsync(Func<Task> asyncAction)
        {
            async Task WrappedAction()
            {
                await asyncAction();
            }

            foreach (var policy in policies)
            {
                //await policy.ExecuteAsync(WrappedAction);
            }
        }

        async Task WrappedAction(Func<Task> asyncAction)
        {
            await asyncAction();
        }
    }
}