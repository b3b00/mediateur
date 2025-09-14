using System.Reflection;
using System.Reflection.Emit;

namespace mediateur;
//https://sharplab.io/#v2:C4LglgNgNAJiDUAfAAgJgIwFgBQyAMABMugCwDcOOyAzAWAHbACmATgGYCGAxkwQJIBxFkybMWBAN44CMorWQkCQkcAAUxQgFsAlBWwBfSrnmoCAEQCe9AiH7LRrSdNnOZABxZgAbh2b8AMmAAzsAAPILCDiwAfAQA+gwhHPQ8QXqyBK5y5laqfIEh4fZisYnAyanaTtgZGQn0SSlMQQQAvHQN5U1pWYY1Lv2yNESKxeroWlVSg7VZtWwA9uKqDMB0bQR4ZOuh8WUVzQB0AMILAK6MZGDw8FNztbL1jakA2mAAuodjOukPMn0ZPr6IA=
public static class DynamicTypeBuilder
{
    
    
    public static object CreateTypeForNotificationHandler<I>(List<I> instances)
    {
        Type interfaceType = typeof(I);
        if (!interfaceType.IsInterface)
            throw new ArgumentException("Le type doit être une interface.");

        var assemblyName = new AssemblyName("DynamicAssembly");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

        var typeBuilder = moduleBuilder.DefineType(
            "DynamicImpl_" + interfaceType.Name,
            TypeAttributes.Public | TypeAttributes.Class);

        typeBuilder.AddInterfaceImplementation(interfaceType);

        // Champ privé pour stocker les instances
        var field = typeBuilder.DefineField("_instances", typeof(List<I>), FieldAttributes.Private);

        // Constructeur prenant les instances
        var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(List<I>) });
        var ilCtor = ctor.GetILGenerator();
        ilCtor.Emit(OpCodes.Ldarg_0);
        ilCtor.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!);
        ilCtor.Emit(OpCodes.Ldarg_0);
        ilCtor.Emit(OpCodes.Ldarg_1);
        ilCtor.Emit(OpCodes.Stfld, field);
        ilCtor.Emit(OpCodes.Ret);

        // Implémentation des méthodes: appel sur chaque instance
        foreach (var method in interfaceType.GetMethods())
        {
            var paramTypes = Array.ConvertAll(method.GetParameters(), p => p.ParameterType);
            var methodBuilder = typeBuilder.DefineMethod(
                method.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                method.ReturnType,
                paramTypes);

            var il = methodBuilder.GetILGenerator();

            var loop = il.DefineLabel();
            var end = il.DefineLabel();
            // int i = 0;
            var iLocal = il.DeclareLocal(typeof(int));
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, iLocal);
            // Début boucle
            il.MarkLabel(loop);
            // if (i >= _instances.Length) break;
            il.Emit(OpCodes.Ldloc, iLocal);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Callvirt, typeof(List<I>).GetProperty("Count")!.GetGetMethod()!);
            il.Emit(OpCodes.Bge, end);
            // Appel de la méthode sur _instances[i]
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ldloc, iLocal);
            il.Emit(OpCodes.Callvirt, typeof(List<I>).GetMethod("get_Item")!);
            il.Emit(OpCodes.Castclass, interfaceType);

            // Charger les arguments
            for (int p = 0; p < paramTypes.Length; p++)
                il.Emit(OpCodes.Ldarg, p + 1);

            il.Emit(OpCodes.Callvirt, method);

            // Si la méthode retourne une valeur, ignorer le résultat
            if (method.ReturnType != typeof(void))
                il.Emit(OpCodes.Pop);

            // i++
            il.Emit(OpCodes.Ldloc, iLocal);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc, iLocal);

            il.Emit(OpCodes.Br, loop);

            il.MarkLabel(end);

            // Retourner la valeur par défaut si non void
            if (method.ReturnType != typeof(void))
            {
                if (method.ReturnType.IsValueType)
                {
                    var local = il.DeclareLocal(method.ReturnType);
                    il.Emit(OpCodes.Ldloca_S, local);
                    il.Emit(OpCodes.Initobj, method.ReturnType);
                    il.Emit(OpCodes.Ldloc, local);
                }
                else
                {
                    il.Emit(OpCodes.Ldnull);
                }
            }
            il.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, method);
        }

        var dynamicType = typeBuilder.CreateType()!;
        return Activator.CreateInstance(dynamicType, new object[] { instances });
    }
    
public static object CreateTypeForRequestHandler<I>(List<I> instances) where I : IRequestHandler
 {
     var interfaceType = typeof(I);
     if (!interfaceType.IsInterface)
         throw new ArgumentException("Le type doit être une interface.");
 
     var assemblyName = new AssemblyName("DynamicAssemblyWithReturn");
     var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
     var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
 
     var typeBuilder = moduleBuilder.DefineType(
         "DynamicImplWithReturn_" + interfaceType.Name,
         TypeAttributes.Public | TypeAttributes.Class);
 
     typeBuilder.AddInterfaceImplementation(interfaceType);
 
     // Champ privé pour stocker les instances
     var field = typeBuilder.DefineField("_instances", typeof(List<I>), FieldAttributes.Private);
 
     // Constructeur
     var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(List<I>) });
     var ilCtor = ctor.GetILGenerator();
     ilCtor.Emit(OpCodes.Ldarg_0);
     ilCtor.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!);
     ilCtor.Emit(OpCodes.Ldarg_0);
     ilCtor.Emit(OpCodes.Ldarg_1);
     ilCtor.Emit(OpCodes.Stfld, field);
     ilCtor.Emit(OpCodes.Ret);
 
     foreach (var method in interfaceType.GetMethods())
     {
         if (method.ReturnType == typeof(void))
             throw new NotSupportedException("Toutes les méthodes de l'interface doivent retourner une valeur.");
 
         var paramTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
         var methodBuilder = typeBuilder.DefineMethod(
             method.Name,
             MethodAttributes.Public | MethodAttributes.Virtual,
             method.ReturnType,
             paramTypes);
 
         var il = methodBuilder.GetILGenerator();
 
         // Locaux: i, returnValue
         var iLocal = il.DeclareLocal(typeof(int));
         var returnLocal = il.DeclareLocal(method.ReturnType);
 
         // Initialisation i = 0
         il.Emit(OpCodes.Ldc_I4_0);
         il.Emit(OpCodes.Stloc, iLocal);
 
         // Optionnel: init returnLocal (pour cas aucune instance)
         if (method.ReturnType.IsValueType)
         {
             il.Emit(OpCodes.Ldloca_S, returnLocal);
             il.Emit(OpCodes.Initobj, method.ReturnType);
         }
         else
         {
             il.Emit(OpCodes.Ldnull);
             il.Emit(OpCodes.Stloc, returnLocal);
         }
 
         var loopLabel = il.DefineLabel();
         var endLabel = il.DefineLabel();
 
         il.MarkLabel(loopLabel);
 
         // if (i >= _instances.Count) break;
         il.Emit(OpCodes.Ldloc, iLocal);
         il.Emit(OpCodes.Ldarg_0);
         il.Emit(OpCodes.Ldfld, field);
         il.Emit(OpCodes.Callvirt, typeof(List<I>).GetProperty("Count")!.GetGetMethod()!);
         il.Emit(OpCodes.Bge, endLabel);
 
         // Charger instance: _instances[i]
         il.Emit(OpCodes.Ldarg_0);
         il.Emit(OpCodes.Ldfld, field);
         il.Emit(OpCodes.Ldloc, iLocal);
         il.Emit(OpCodes.Callvirt, typeof(List<I>).GetMethod("get_Item")!);
         il.Emit(OpCodes.Castclass, interfaceType);
 
         // Charger les arguments
         for (int p = 0; p < paramTypes.Length; p++)
             il.Emit(OpCodes.Ldarg, p + 1);
 
         // Appel
         il.Emit(OpCodes.Callvirt, method);
 
         // Stocker la valeur (écrase la précédente)
         il.Emit(OpCodes.Stloc, returnLocal);
 
         // i++
         il.Emit(OpCodes.Ldloc, iLocal);
         il.Emit(OpCodes.Ldc_I4_1);
         il.Emit(OpCodes.Add);
         il.Emit(OpCodes.Stloc, iLocal);
 
         il.Emit(OpCodes.Br, loopLabel);
 
         il.MarkLabel(endLabel);
 
         // Charger la dernière valeur et retourner
         il.Emit(OpCodes.Ldloc, returnLocal);
         il.Emit(OpCodes.Ret);
 
         typeBuilder.DefineMethodOverride(methodBuilder, method);
     }
 
     var dynamicType = typeBuilder.CreateType()!;
     return Activator.CreateInstance(dynamicType, new object[] { instances })!;
 }
 
}
