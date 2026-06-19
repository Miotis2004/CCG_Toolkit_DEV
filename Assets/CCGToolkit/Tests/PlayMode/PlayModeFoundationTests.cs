using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace CCGToolkit.Tests.PlayMode
{
    public sealed class PlayModeFoundationTests
    {
        [UnityTest]
        public IEnumerator PlayModeTestAssemblyLoads()
        {
            yield return null;
            Assert.Pass();
        }
    }
}
