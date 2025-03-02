using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.IO;

public static class LLTextureManager 
{
    #region Variables

    #if UNITY_IOS && !UNITY_EDITOR
    struct LLTextureInfo
    {
        public bool    exist;
        public int     width;
        public int     height;
    };

    [DllImport ("__Internal")]
    static extern void LLTextureLoaderInit(System.Action<IntPtr, IntPtr> callbackLoad, System.Action<IntPtr, IntPtr> callbackFail);

    [DllImport ("__Internal")]
    static extern LLTextureInfo LLTextureLoaderGetInfo(string imagePath);

    [DllImport ("__Internal")]
    static extern IntPtr LLTextureLoaderLoadTexture(string imagePath, bool doMipMaps);

    [DllImport ("__Internal")]
    static extern IntPtr LLTextureLoaderLoadTextureAsync(string imagePath, bool doMipMaps);

    [DllImport ("__Internal")]
    static extern void LLTextureLoaderReleaseTexture(IntPtr texturePointer);

    static LLTextureLoader instance = new LLTextureLoader();
    #endif

    #endregion


    #region Public methods

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RuntimeInitialize()
    {
        #if UNITY_IOS && !UNITY_EDITOR
        TextureManager.RegisterLoader(instance);
        #endif
    }

    #endregion


    #region Internal Plugin Loader

    sealed class LLTextureLoader : ITextureLoader
    {
        #region Variables

        static System.Action<IntPtr, IntPtr> callbackTextureLoad;
        static System.Action<IntPtr, IntPtr> callbackTextureFail;

        #endregion


        #region ITextureLoaderPlugin

        public void Init(System.Action<IntPtr, IntPtr> callbackLoad, System.Action<IntPtr, IntPtr> callbackFail)
        {
            callbackTextureLoad = callbackLoad;
            callbackTextureFail = callbackFail;
            #if UNITY_IOS && !UNITY_EDITOR
            LLTextureLoaderInit(OnTextureLoad, OnTextureFail);
            #endif
        }  

        public TextureManager.TextureInfo GetInfo(string imagePath)
        {
            TextureManager.TextureInfo result = new TextureManager.TextureInfo();
            #if UNITY_IOS && !UNITY_EDITOR
            LLTextureInfo llResult = LLTextureLoaderGetInfo(imagePath);
            result.exist = llResult.exist;
            result.width = llResult.width;
            result.height = llResult.height;
            #endif
            return result;
        }


        public IntPtr LoadTexture(string imagePath, bool doMipMaps)
        {
            IntPtr resultTexture = IntPtr.Zero;

            #if UNITY_IOS && !UNITY_EDITOR
            resultTexture = LLTextureLoaderLoadTexture(imagePath, doMipMaps);
            #endif

            return resultTexture;
        }


        public IntPtr LoadTextureAsync(string imagePath, bool doMipMaps)
        {
            IntPtr resultTexture = IntPtr.Zero;

            #if UNITY_IOS && !UNITY_EDITOR
            resultTexture = LLTextureLoaderLoadTextureAsync(imagePath, doMipMaps);
            #endif

            return resultTexture;
        }


        public void ReleaseTexture(IntPtr texturePointer)
        {
            if (texturePointer != IntPtr.Zero)
            {
                #if UNITY_IOS && !UNITY_EDITOR
                LLTextureLoaderReleaseTexture(texturePointer);
                #endif
            }
        }


        public void Deinit()
        {
            callbackTextureLoad = null;
            callbackTextureFail = null;
        }   

        #endregion


        #region Events

        [AOT.MonoPInvokeCallbackAttribute(typeof(System.Action<IntPtr, IntPtr>))]
        static void OnTextureLoad(IntPtr textureID, IntPtr newTexturePointer)
        {
            if (callbackTextureLoad != null)
            {
                callbackTextureLoad(textureID, newTexturePointer);
            }
        }


        [AOT.MonoPInvokeCallbackAttribute(typeof(System.Action<IntPtr, IntPtr>))]
        static void OnTextureFail(IntPtr textureID, IntPtr newTexturePointer)
        {
            if (callbackTextureFail != null)
            {
                callbackTextureFail(textureID, newTexturePointer);
            }
        }

        #endregion
    }

    #endregion
}
