using Modules.General.Abstraction.GooglePlayGameServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;


namespace Modules.General.GooglePlayGameServices
{
    internal class SocialGpgs : ISocialGpgs
    {
        #region Nested types

        // Class for interactions with native class with similar name
        private class LLSocialGPGS { }

        #endregion
        
        
        #region Fields

        private const string InitMethodName = "LLSocialGPGSInit";
        private const string UnlockAchievementMethodName = "LLSocialGPGSUnlockAchievement";
        private const string SubmitScoreMethodName = "LLSocialGPGSSubmitScore";
        private const string ShowAchievementsUiMethodName = "LLSocialGPGSShowAchievementsUI";
        private const string ShowLeaderboardUiMethodName = "LLSocialGPGSShowLeaderboardUI";
        private const string LoadScoresMethodName = "LLSocialGPGSLoadScores";
        private const string LoadAchievementsMethodName = "LLSocialGPGSLoadAchievements";

        private const string GetServerAuthCodeMethodName = "LLSocialGPGSGetServerAuthCode";
        private const string HasPermissionMethodName = "LLSocialGPGSHasPermission";
        private const string RequestPermissionMethodName = "LLSocialGPGSRequestPermission";
        private const string SignOutMethodName = "LLSocialGPGSSignOut";

        private string webClientId;

        #endregion



        #region Properties

        public bool IsRequestedAuthCode => !string.IsNullOrEmpty(webClientId);
        
        
        public Abstraction.GooglePlayGameServices.LeaderboardConfiguration LeaderboardConfiguration { get; set; }
        
        
        public ILocalUser localUser { get; }

        #endregion



        #region Class lifecycle

        public SocialGpgs(ILocalUser localUserImplementation, IGpgsSettings settings)
        {
            localUser = localUserImplementation;
            webClientId = string.IsNullOrEmpty(settings?.WebClientId) ? null : webClientId;
        }

        #endregion
        
        
        
        #region Achievements
        
        public IAchievement CreateAchievement() => new AchievementGpgs(this);
        
        
        public void ReportProgress(string achievementID, double progress, Action<bool> callback)
        {
            if (progress == 100.0)
            {
                LLAndroidJavaSingletone<LLSocialGPGS>.CallStatic(
                    UnlockAchievementMethodName,
                    achievementID,
                    LLAndroidJavaCallback.ProxyCallback(callback));
            }
            else
            {
                callback?.Invoke(false);
            }
        }
        
        
        public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
        {
            if (callback != null)
            {
                Action<AchievementGpgs[]> callbackWrapper = achievementsGpgs => callback(achievementsGpgs);
                AndroidJavaProxy jsonCallback = HandleJsonAchievements(callbackWrapper);

                LLAndroidJavaSingletone<LLSocialGPGS>.CallStatic(LoadAchievementsMethodName, jsonCallback);
            }
        }
        
        
        public void LoadAchievements(Action<IAchievement[]> callback)
        {
            if (callback != null)
            {
                Action<AchievementGpgs[]> callbackWrapper = achievementsGpgs => callback(achievementsGpgs);
                AndroidJavaProxy jsonCallback = HandleJsonAchievements(callbackWrapper);

                LLAndroidJavaSingletone<LLSocialGPGS>.CallStatic(LoadAchievementsMethodName, jsonCallback);
            }
        }
        
        
        public void ShowAchievementsUI()
        {
            LLAndroidJavaSingletone<LLSocialGPGS>.CallStatic(ShowAchievementsUiMethodName);
        }
        
        #endregion
        
        
        
        #region Leaderboards

        public ILeaderboard CreateLeaderboard() => new LeaderboardGpgs(
            LeaderboardConfiguration.leaderboardType,
            LeaderboardConfiguration.timeScope,
            this);
        
        
        public void ReportScore(long score, string board, Action<bool> callback)
        {
            LLAndroidJavaSingletone<LLSocialGPGS>.CallStatic(
                SubmitScoreMethodName,
                score,
                board,
                LLAndroidJavaCallback.ProxyCallback(callback));
        }


        public void LoadScores(string leaderboardID, Action<IScore[]> callback)
        {
            LoadScores(
                leaderboardID,
                LeaderboardConfiguration.leaderboardType,
                LeaderboardConfiguration.timeScope,
                LeaderboardConfiguration.rowCount,
                callback);
        }


        public void LoadScores(ILeaderboard board, Action<bool> callback)
        {
            board.LoadScores(callback);
        }
        
        
        public bool GetLoading(ILeaderboard board) => board.loading;


        public void ShowLeaderboardUI()
        {
            LLAndroidJavaSingletone<LLSocialGPGS>.CallStatic(ShowLeaderboardUiMethodName);
        }
        
        #endregion



        #region Methods

        public void Activate()
        {
            CustomDebug.Log("Activate social for Android");
            
            Social.Active = this;
            
            LLAndroidJavaSingletone<LLSocialGPGS>.CallStatic(InitMethodName, webClientId);
        }
        
        
        public void SignOut()
        {
            LLAndroidJavaSingletone<LLSocialGPGS>.CallStatic(SignOutMethodName);
        }
        
        
        public string GetServerAuthCode()
        {
            string result = string.Empty;
            
            if (IsRequestedAuthCode)
            {
                result = LLAndroidJavaSingletone<LLSocialGPGS>.CallStatic<string>(GetServerAuthCodeMethodName);
            }
            
            return result;
        }
        
        
        public bool HasPermission(GooglePermission permission) =>
            LLAndroidJavaSingletone<LLSocialGPGS>.CallStatic<bool>(HasPermissionMethodName, (int)permission);


        public void RequestPermission(GooglePermission permission, Action<bool> callback) =>
            LLAndroidJavaSingletone<LLSocialGPGS>.CallStatic(
                RequestPermissionMethodName,
                (int)permission,
                LLAndroidJavaCallback.ProxyCallback(callback));
        
        
        public void LoadScores(
            string leaderboardID,
            LeaderboardType leaderboardType,
            TimeScope timeScope,
            int rowCount,
            Action<IScore[]> callback)
        {
            if (callback != null)
            {
                AndroidJavaProxy jsonCallback = HandlerJsonScores();
                LLAndroidJavaSingletone<LLSocialGPGS>.CallStatic(
                    LoadScoresMethodName,
                    leaderboardID,
                    (int)leaderboardType,
                    (int)GetNativeTimeSpan(timeScope),
                    rowCount,
                    jsonCallback);
            }
            
            
            AndroidJavaProxy HandlerJsonScores()
            {
                AndroidJavaProxy handler = LLAndroidJavaCallback.ProxyCallback((string json) =>
                {
                    if (string.IsNullOrEmpty(json))
                    {
                        callback(null);
                    }
                    else
                    {
                        List<NativeScore> nativeScores = new List<NativeScore>();
                        if (leaderboardType == LeaderboardType.CurrentPlayer)
                        {
                            NativeScore nativeScore = JsonConvert.DeserializeObject<NativeScore>(json);
                            nativeScores.Add(nativeScore);
                        }
                        else
                        {
                            List<NativeScore> tempScores = JsonConvert.DeserializeObject<List<NativeScore>>(json);
                            nativeScores = tempScores;
                        }

                        ScoreGpgs[] scores = new ScoreGpgs[nativeScores.Count];
                        for (int i = 0; i < nativeScores.Count; i++)
                        {
                            scores[i] = new ScoreGpgs(leaderboardID, nativeScores[i], this);
                        }

                        callback(scores);
                    }
                });

                return handler;
            }
            
            
            LeaderboardTimeSpan GetNativeTimeSpan(TimeScope value)
            {
                LeaderboardTimeSpan timeSpan;

                switch (value)
                {
                    case TimeScope.Week:
                        timeSpan = LeaderboardTimeSpan.Weekly;
                        break;
                    case TimeScope.Today:
                        timeSpan = LeaderboardTimeSpan.Daily;
                        break;
                    case TimeScope.AllTime:
                        timeSpan = LeaderboardTimeSpan.AllTime;
                        break;
                    default:
                        timeSpan = LeaderboardTimeSpan.AllTime;
                        break;
                }

                return timeSpan;
            }
        }
        
        
        public void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback) { }
        public void Authenticate(ILocalUser user, Action<bool> callback) { }
        public void Authenticate(ILocalUser user, Action<bool, string> callback) { }
        public void LoadFriends(ILocalUser user, Action<bool> callback)  { }

        #endregion



        #region Utilities

        private AndroidJavaProxy HandleJsonAchievements(Action<AchievementGpgs[]> callback)
        {
            AndroidJavaProxy handler = LLAndroidJavaCallback.ProxyCallback((string json) =>
            {
                if (string.IsNullOrEmpty(json))
                {
                    callback(null);
                }
                else
                {
                    List<NativeAchievement> nativeAchievements = JsonConvert.DeserializeObject<List<NativeAchievement>>(json);
                    if (nativeAchievements != null && nativeAchievements.Count > 0)
                    {
                        AchievementGpgs[] achievements = new AchievementGpgs[nativeAchievements.Count];
                        for (int i = 0; i < nativeAchievements.Count; i++)
                        {
                            achievements[i] = new AchievementGpgs(nativeAchievements[i], this);
                        }
        
                        callback(achievements);
                    }
                    else
                    {
                        callback(null);
                    }
                }
            });
        
            return handler;
        }
        
        #endregion
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        

        
    }
}
