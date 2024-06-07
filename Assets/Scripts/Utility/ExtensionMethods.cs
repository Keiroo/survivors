using Unity.Entities;

namespace Survivors
{
    public static class ExtensionMethods
    {
        public static void PlaybackAndDispose(this ref EntityCommandBuffer buffer, EntityManager entityManager)
        {
            buffer.Playback(entityManager);
            buffer.Dispose();
        }
    }
}
