using System;  
using System.Reflection;


public abstract class BaseSingleton<T> where T : BaseSingleton<T>
{
	protected static T instance = null;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // get all public constructor method
                ConstructorInfo[] ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

                // get the method with no parameter from ctors 
                ConstructorInfo ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);

                if (ctor == null)
                    throw new Exception("Non-public ctor() not found!");

                // return the method
                instance = ctor.Invoke(null) as T;
            }

            return instance;
        }
    }

    protected BaseSingleton()
    {
        init();
    }

    public virtual void init()
    {
    }

}