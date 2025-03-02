using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public static class LLSystemSharingManager
{
    #region Variables

    #if UNITY_IOS && !UNITY_EDITOR
    [DllImport ("__Internal")]
    static extern void LLSystemSharingShareItems(string text, string imagePath, string urlString);
    [DllImport ("__Internal")]
    static extern void LLSystemSharingShareItemsWithData(string text, byte[] bytes, int width, int height, string urlString);
    #endif

    #endregion

 
    #region Public methods

    public static void Share(string text, string imagePath, string url)
    {
        #if UNITY_IOS && !UNITY_EDITOR
        LLSystemSharingShareItems(text, imagePath, url);
        #endif
    } 


    public static void Share(string text, Texture2D imageTexture, string url)
    {
        byte[] bytes = imageTexture.EncodeToJPG(100);
        int width = imageTexture.width;
        int height = imageTexture.height;
        
        Share(text, bytes, width, height, url);
    } 


    public static void Share(string text, byte[] imageBytes, int widthImage, int heightImage, string url)
    {
        #if UNITY_IOS && !UNITY_EDITOR
        LLSystemSharingShareItemsWithData(text, imageBytes, widthImage, heightImage, url);
        #endif
    } 

    #endregion
}