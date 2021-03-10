using System.Reflection;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface ITestAssemblyProvider
    {
        Assembly TestAssembly { get; }

        void RegisterTestAssembly(Assembly testAssembly);
    }
}
