using System;
using System.Reflection;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Common
{
    internal class AutoloadHelper
    {
        public static Type[] GetTypes(Assembly ass)
        {
            return ass.GetTypes();
        }

        public static bool TryGetInstanceOf<T>(Type t, out T instance) where T : class
        {
            instance = null;
            if (t.IsAbstract || t.IsInterface)
            {
                return false;
            }
            foreach (var i in t.GetInterfaces())
            {
                if (i.IsAssignableTo(typeof(T)))
                {
                    instance = ModContent.GetInstance<T>();
                    if (instance == null)
                    {
                        instance = (T)Activator.CreateInstance(t);
                    }
                    return instance != null;
                }
            }

            return false;
        }

        public static void AutoloadOfType<T>(Assembly ass, ArsenalLabsBeta arslab) where T : ILoadable
        {
            var m = typeof(T).GetMethod("CheckAutoload", BindingFlags.Public | BindingFlags.Static);
            foreach (var t in GetTypes(ass))
            {
                if (t.IsAbstract || t.IsInterface)
                {
                    continue;
                }
                m.Invoke(null, new object[] { arslab, t });
            }
        }
    }
    internal interface IOnModLoad : ILoadable
    {
        void OnModLoad(ArsenalLabsBeta arslab);

        public static bool CheckAutoload(ArsenalLabsBeta arslab, Type type)
        {
            if (AutoloadHelper.TryGetInstanceOf<IOnModLoad>(type, out var onModLoad))
            {
                onModLoad.OnModLoad(arslab);
                return true;
            }
            return false;
        }
    }
    internal interface IPostSetupContent : ILoadable
    {
        void PostSetupContent(ArsenalLabsBeta arslab);

        public static bool CheckAutoload(ArsenalLabsBeta arslab, Type type)
        {
            if (AutoloadHelper.TryGetInstanceOf<IPostSetupContent>(type, out var setupContent))
            {
                setupContent.PostSetupContent(arslab);
                return true;
            }
            return false;
        }
    }
    internal interface IAddRecipes : ILoadable
    {
        void AddRecipes(ArsenalLabsBeta arslab);

        public static bool CheckAutoload(ArsenalLabsBeta arslab, Type type)
        {
            if (AutoloadHelper.TryGetInstanceOf<IAddRecipes>(type, out var addRecipes))
            {
                addRecipes.AddRecipes(arslab);
                return true;
            }
            return false;
        }
    }
    internal interface IPostAddRecipes : ILoadable
    {
        void PostAddRecipes(ArsenalLabsBeta arslab);

        public static bool CheckAutoload(ArsenalLabsBeta arslab, Type type)
        {
            if (AutoloadHelper.TryGetInstanceOf<IPostAddRecipes>(type, out var postAddRecipes))
            {
                postAddRecipes.PostAddRecipes(arslab);
                return true;
            }
            return false;
        }
    }
}
