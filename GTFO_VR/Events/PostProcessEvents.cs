using GTFO_VR.Core;
using System;
using UnityEngine.PostProcessing;

namespace GTFO_VR.Events
{
    /// <summary>
    /// Add post process behaviour events
    /// </summary>
    public static class PostProcessEvents
    {
        public static event Action<PostProcessingBehaviour> OnPostProcessEventsEnabled;

        public static void PostProcessEventsEnabled(PostProcessingBehaviour postProcess)
        {
            OnPostProcessEventsEnabled?.Invoke(postProcess);
        }
    }
}