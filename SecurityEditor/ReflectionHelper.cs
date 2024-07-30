namespace System.Reflection;

internal static class ReflectionHelper
{
	public static T GetPropertyValue<T>(this object obj, string propertyName)
	{
		var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance, null, typeof(T), Type.EmptyTypes, null);
		return prop == null ? throw new ArgumentException() : (T)prop.GetValue(obj, null);
	}

	public static object GetPropertyValue(this object obj, string propertyName)
	{
		var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
		return prop == null ? throw new ArgumentException() : prop.GetValue(obj, null);
	}

	public static T InvokeMethod<T>(this Type type, string methodName, params object[] args)
	{
		object o = Activator.CreateInstance(type);
		return InvokeMethod<T>(o, methodName, args);
	}

	public static T InvokeMethod<T>(this Type type, object[] instArgs, string methodName, params object[] args)
	{
		object o = Activator.CreateInstance(type, instArgs);
		return InvokeMethod<T>(o, methodName, args);
	}

	public static void InvokeMethod(this object obj, string methodName, params object[] args)
	{
		Type[] argTypes = (args == null || args.Length == 0) ? Type.EmptyTypes : Array.ConvertAll(args, delegate (object o) { return o == null ? typeof(object) : o.GetType(); });
		InvokeMethod(obj, methodName, argTypes, args);
	}

	public static T InvokeMethod<T>(this object obj, string methodName, params object[] args)
	{
		Type[] argTypes = (args == null || args.Length == 0) ? Type.EmptyTypes : Array.ConvertAll(args, delegate (object o) { return o == null ? typeof(object) : o.GetType(); });
		return InvokeMethod<T>(obj, methodName, argTypes, args);
	}

	public static void InvokeMethod(this object obj, string methodName, Type[] argTypes, object[] args)
	{
		if (obj != null)
		{
			MethodInfo mi = obj.GetType().GetMethod(methodName, argTypes);
			if (mi != null && mi.ReturnType == typeof(void))
				mi.Invoke(obj, args);
		}
	}

	public static T InvokeMethod<T>(this object obj, string methodName, Type[] argTypes, object[] args)
	{
		if (obj != null)
		{
			MethodInfo mi = obj.GetType().GetMethod(methodName, argTypes);
			if (mi != null)
			{
				Type tt = typeof(T);
				if (tt == typeof(object) || mi.ReturnType == tt || mi.ReturnType.IsSubclassOf(tt))
					return (T)mi.Invoke(obj, args);
				if (mi.ReturnType.GetInterface("IConvertible") != null)
					return (T)Convert.ChangeType(mi.Invoke(obj, args), tt);
			}
		}
		return default;
	}

	public static Type LoadType(string typeName, string asmRef)
	{
		Type ret = null;
		if (!TryGetType(Assembly.LoadFrom(asmRef), typeName, ref ret))
			if (!TryGetType(Assembly.GetExecutingAssembly(), typeName, ref ret))
				if (!TryGetType(Assembly.GetCallingAssembly(), typeName, ref ret))
					TryGetType(Assembly.GetEntryAssembly(), typeName, ref ret);
		return ret;
	}

	private static bool TryGetType(Assembly asm, string typeName, ref Type type)
	{
		if (asm != null)
		{
			type = asm.GetType(typeName, false, false);
			return type != null;
		}
		return false;
	}
}