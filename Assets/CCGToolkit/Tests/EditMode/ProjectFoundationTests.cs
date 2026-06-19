using CCGToolkit;
using NUnit.Framework;

namespace CCGToolkit.Tests.EditMode
{
    public sealed class ProjectFoundationTests
    {
        [Test]
        public void ToolkitMetadataDefinesUnity6Minimum()
        {
            Assert.That(CCGToolkitInfo.RootNamespace, Is.EqualTo("CCGToolkit"));
            Assert.That(CCGToolkitInfo.MinimumUnityVersion, Does.StartWith("6000."));
        }
    }
}
