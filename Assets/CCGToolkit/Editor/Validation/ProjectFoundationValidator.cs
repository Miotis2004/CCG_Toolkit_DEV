using UnityEditor;

namespace CCGToolkit.Editor.Validation
{
    public static class ProjectFoundationValidator
    {
        [MenuItem("Tools/CCG Toolkit/Validate Project Foundation")]
        public static void Validate()
        {
            UnityEngine.Debug.Log("CCG Toolkit project foundation is installed. Run edit-mode and play-mode tests before feature work.");
        }
    }
}
