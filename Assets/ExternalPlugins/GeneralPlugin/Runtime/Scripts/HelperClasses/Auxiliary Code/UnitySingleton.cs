/*************************************************************
 *       Unity Singleton Class (c) by ClockStone 2011        *
 * 
 * Allows to use a script components like a singleton
 * 
 * Usage:
 * 
 * Derive your script class MyScriptClass from 
 * SingletonMonoBehaviour<MyScriptClass>
 * 
 * Access the script component using the static function
 * MyScriptClass.Instance
 * 
 * use the static function SetSingletonAutoCreate( GameObject )
 * to specify a GameObject - containing the MyScriptClass component -  
 * that should be instantiated in case an instance is requested and 
 * and no objects exists with the MyScriptClass component.
 * 
 * ***********************************************************/

using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif


namespace Modules.General.HelperClasses
{
    /// <summary>
    /// Provides singleton-like access to a unique instance of a MonoBehaviour. <para/>
    /// </summary>
    /// <example>
    /// Derive your own class from SingletonMonoBehaviour. <para/>
    /// <code>
    /// public class MyScriptClass : SingletonMonoBehaviour&lt;MyScriptClass&gt;
    /// {
    ///     public void MyFunction() { }
    ///     protected override void Awake()
    ///     {
    ///         base.Awake();
    ///     }
    ///     protected override void OnDestroy()
    ///     {
    ///         base.OnDestroy();
    ///     }
    /// }
    /// </code>
    /// <para/>
    /// access the instance by writing
    /// <code>
    /// MyScriptClass.Instance.MyFunction();
    /// </code>
    /// </example>
    /// <typeparam name="T">Your singleton MonoBehaviour</typeparam>
    /// <remarks>
    /// Makes sure that an instance is available from other Awake() calls even before the singleton's Awake()
    /// was called.
    /// </remarks>
    #if ODIN_INSPECTOR
    public abstract class SingletonMonoBehaviour<T> : SerializedMonoBehaviour, InternalSingleton.ISingletonMonoBehaviour
        where T : SerializedMonoBehaviour
    #else
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour, InternalSingleton.ISingletonMonoBehaviour
        where T : MonoBehaviour
    #endif
    {
        #region Properties
    
        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        /// <returns>
        /// A reference to the instance if it exists, otherwise <c>null</c>
        /// </returns>
        /// <remarks>
        /// Outputs an error to the debug log if no instance was found.
        /// </remarks>
        public static T Instance
        {
            get { return InternalSingleton.UnitySingleton<T>.GetSingleton(true); }
        }
    
    
        /// <summary>
        /// checks whether instance is not null
        /// </summary>
        public static bool HasFoundInstance
        {
            get { return InternalSingleton.UnitySingleton<T>.HasFoundInstance(); }
        }
    
    
        /// <summary>
        /// Checks if an instance of this MonoBehaviour exists.
        /// </summary>
        /// <returns>
        /// A reference to the instance if it exists, otherwise <c>null</c>
        /// </returns>
        public static T InstanceIfExist
        {
            get { return InternalSingleton.UnitySingleton<T>.GetSingleton(false); }
        }
    
        #endregion
    
    
        #region Lifecycles
    
        protected virtual void Awake() // should be called in derived class
        {
            if ( IsSingletonObject )
            {
                InternalSingleton.UnitySingleton<T>.InternalAwake( this as T );
            }
        }
    
        protected virtual void OnDestroy()  // should be called in derived class
        {
            if ( IsSingletonObject )
            {
                InternalSingleton.UnitySingleton<T>.InternalDestroy();
            }
        }
    
        #endregion
    
    
        #region Public methods
    
        /// <summary>
        /// must return true if this instance of the object is the singleton. Can be used to allow multiple objects of this type
        /// that are "add-ons" to the singleton.
        /// </summary>
        public virtual bool IsSingletonObject
        {
            get { return true; }
        }
    
        #endregion               
    }
    
    
    #region Internal implementation
    
    namespace InternalSingleton
    {
    
        interface ISingletonMonoBehaviour
        {
            bool IsSingletonObject { get; }
        }
    
    
        public class UnitySingleton<T> where T : MonoBehaviour
        {
            #region Variables
    
            static T instanceT;
    
            internal static Type typeT = typeof( T ); // not working for Flash builds. Requires SetSingletonType() for correct Flash support
    
            static  int globalInstanceCount = 0;
            static bool destroySingletonCalled = false;
    
            static readonly string autocreateRooName = "AutoSingletons";
            static Transform autoCreateRoot;
    
            #endregion
    
    
            #region Properties
    
            static Transform AutocreateRoot
            {
                get
                {
                    if (autoCreateRoot == null)
                    {
                        GameObject result = GameObject.Find(autocreateRooName);
    
                        if (result == null)
                        {
                            result = new GameObject(autocreateRooName);
                            GameObject.DontDestroyOnLoad(result);
                        }
    
                        autoCreateRoot = result.transform;
                    }
    
                    return autoCreateRoot;
                }
            }
    
            #endregion
    
    
            #region Lifecycles
    
            UnitySingleton() { }
    
            internal static void InternalAwake( T instance )
            {
                globalInstanceCount++;
                if (globalInstanceCount > 1)
                {
                    #if UNITY_EDITOR
                    if ((Application.isPlaying) && (!UnityEditor.EditorApplication.isPaused))
                    #endif
                    {
                        // can happen if an exception occurred that prevented correct instance counting 
                        CustomDebug.LogWarning("More than one awake of SingletonMonoBehaviour " + typeof(T).Name);
                    }
                }
                else
                {
                    instanceT = (T)instance;
                }
            }
    
            internal static void InternalDestroy()
            {
                if ( globalInstanceCount > 0 )
                {
                    globalInstanceCount--;
                    if ( globalInstanceCount == 0 )
                    {
                        destroySingletonCalled = true;
                        instanceT = null;
                    }
                }
                else
                {
                    #if UNITY_EDITOR
                    if ((Application.isPlaying) && (!UnityEditor.EditorApplication.isPaused))
                    #endif
                    {
                        // can happen if an exception occurred that prevented correct instance counting 
                        CustomDebug.LogWarning( "More than one destroy of SingletonMonoBehaviour " + typeof(T).Name);
                    }
                }
            }
    
            #endregion
    
    
            #region Public methods
    
            public static bool HasFoundInstance()
            {
                return instanceT != null;
            }
    
    
            public static T GetSingleton( bool autoCreate )
            {
                if ( !instanceT ) // Unity operator to check if object was destroyed, 
                {
                    T component = null;
    
                    #if UNITY_EDITOR
                    if ((Application.isPlaying) && (!UnityEditor.EditorApplication.isPaused))
                    {
                        T[] components = GameObject.FindObjectsOfType<T>();
    
                        foreach ( T c in components )
                        {
                            ISingletonMonoBehaviour singletonCpt = (ISingletonMonoBehaviour)( c );
                            if ( singletonCpt.IsSingletonObject )
                            {
                                component = c;
                                CustomDebug.LogError("Must call base.Awake() singleton : " + typeT.Name);
                                break;
                            }
                        }
                    }
                    #endif
    
                    if ( !component )
                    {
                        if ( autoCreate
                            #if UNITY_EDITOR
                            && Application.isPlaying
                            && !UnityEditor.EditorApplication.isPaused
                            #endif
                            && !destroySingletonCalled
                        )
                        {
                            GameObject go = new GameObject(typeT.Name, new Type[] { typeT });
                            go.transform.parent = AutocreateRoot;
    
                            CustomDebug.LogWarning("Create singleton : " + typeT.Name);
    
                            component = go.GetComponent<T>();
                            if ( component == null )
                            {
                                CustomDebug.LogError( "Auto created object does not have component " + typeT.Name );
                                component = null;
                            }
                        }
                        else
                        {                
                            component = null;
                        }
                    }
    
                    instanceT = component;
                }
    
                return instanceT;
            }
    
            #endregion
        }        
    }
    
    #endregion
}
