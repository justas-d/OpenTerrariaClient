using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenTerrariaClient.Model.ID
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

        private static string GetStaticFieldName<T>(Type target, T value)
        {
            foreach (
                FieldInfo field in
                    target.GetFields(BindingFlags.Static | BindingFlags.Public)
                        .Where(field => field.FieldType == typeof (T))
                        .Where(field => EqualityComparer<T>.Default.Equals(value, (T) field.GetValue(null))))
                return field.Name;

            return "Not found.";
        }


        public static short GetInvasion(string name)
            => GetStaticFieldValue<short>(typeof (InvasionId), name);
        public static string GetInvasion(short id)
            => GetStaticFieldName(typeof (InvasionId), id);

        public static short GetItem(string name)
            => GetStaticFieldValue<short>(typeof (ItemId), name);
        public static string GetItem(short id)
            => GetStaticFieldName(typeof(ItemId), id);


        public static short GetNpc(string name)
            => GetStaticFieldValue<short>(typeof (NpcId), name);
        public static string GetNpc(short id)
            => GetStaticFieldName(typeof(NpcId), id);


        public static ushort GetTile(string name)
            => GetStaticFieldValue<ushort>(typeof (TileId), name);
        public static string GetTile(short id)
            => GetStaticFieldName(typeof(TileId), id);


        public static byte GetWall(string name)
            => GetStaticFieldValue<byte>(typeof (WallId), name);
        public static string GetWall(short id)
            => GetStaticFieldName(typeof(WallId), id);


        public static byte GetBuff(string name)
            => GetStaticFieldValue<byte>(typeof (BuffId), name);
        public static string GetBuff(short id)
            => GetStaticFieldName(typeof(BuffId), id);

    }
}