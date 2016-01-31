using System;
using System.Linq;
using System.Reflection;

namespace TerrariaBridge.Model.ID
{
    public static class IdLookup
    {
        private static T GetStaticFieldValue<T>(Type target, string name)
        {
            foreach (
                FieldInfo field in
                    target.GetFields(BindingFlags.Static | BindingFlags.Public)
                        .Where(field => field.FieldType == typeof (T))
                        .Where(field => field.Name.ToLowerInvariant().StartsWith(name.ToLowerInvariant())))
                return (T) field.GetValue(null);

            return default(T);
        }

        public static short GetInvasion(string name)
            => GetStaticFieldValue<short>(typeof (InvasionId), name);

        public static short GetItem(string name)
            => GetStaticFieldValue<short>(typeof (ItemId), name);

        public static short GetNpc(string name)
            => GetStaticFieldValue<short>(typeof (NpcId), name);

        public static ushort GetTile(string name)
            => GetStaticFieldValue<ushort>(typeof (TileId), name);

        public static byte GetWall(string name)
            => GetStaticFieldValue<byte>(typeof (WallId), name);

        public static byte GetBuff(string name)
            => GetStaticFieldValue<byte>(typeof (BuffId), name);
    }
}