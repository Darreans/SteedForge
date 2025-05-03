using Unity.Entities;

namespace SteedForge
{
    internal static class VWorldUtils
    {
        private static World? _serverWorld;

        public static World Server
        {
            get
            {
                if (_serverWorld != null && _serverWorld.IsCreated)
                {
                    return _serverWorld;
                }

                _serverWorld = GetWorld("Server");

                if (_serverWorld == null)
                {
                    throw new System.Exception("Could not find Server world. Ensure this mod is running on the server.");
                }

                return _serverWorld;
            }
        }

        private static World? GetWorld(string name)
        {
            foreach (var world in World.s_AllWorlds)
            {
                if (world.Name == name)
                {
                    return world;
                }
            }
            return null;
        }
    }
}