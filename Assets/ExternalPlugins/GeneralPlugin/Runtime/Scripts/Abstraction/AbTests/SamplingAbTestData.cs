using System.Collections.Generic;


namespace Modules.General.Abstraction
{
    public class SamplingAbTestData : IAbTestData
    {
        public virtual int SamplingDropPercent { get; set; } = 95;
        public virtual List<string> SamplingDropEvents { get; set; }  = new List<string>()
        {
            "mopub_interstitial_availability_check", "level_finish", "sublevel_finish", "level_start", "sublevel_start",
            "mopub_interstitial_request", "mopub_interstitial_answer", "mopub_video_request", "mopub_video_answer",
            "mopub_interstitial_show", "mopub_video_show", "mopub_video_availability_check", "mopub_interstitial_click",
            "mopub_video_click", "mopub_banner_availability_check", "mopub_banner_show", "mopub_banner_click",
            "af_mopub_video_reward_placement_show", "af_mopub_video_reward", "matches_count_1", "matches_count_3",
            "matches_count_7", "matches_count_15", "matches_count_30", "sessions_count_3", "matches_count_60",
            "matches_count_80", "matches_count_100", "sessions_count_10", "sessions_count_15", "sessions_count_20",
            "sessions_count_30", "chests9_finish", "chest_finish", "lottery_finish", "upgrade_purchased",
            "collection_open_by_button", "collection_item_selected", "piggy_bank_finish", "mopub_interstitial_try_show",
            "mopub_interstitial_close", "mopub_video_try_show", "mopub_video_close", "mopub_interstitial_expire",
            "mopub_video_expire", 
            "max_interstitial_availability_check", "max_interstitial_request", "max_interstitial_answer", 
            "max_video_request", "max_video_answer", "max_interstitial_show", "max_video_show", "max_video_availability_check", 
            "max_interstitial_click", "max_video_click", "max_banner_availability_check", "max_banner_show", "max_banner_click",
            "af_max_video_reward_placement_show", "af_max_video_reward", "max_interstitial_try_show",
            "max_interstitial_close", "max_video_try_show", "max_video_close",
            "huawei_interstitial_availability_check", "huawei_interstitial_request", "huawei_interstitial_answer", "huawei_video_request", 
            "huawei_video_answer", "huawei_interstitial_show", "huawei_video_show", "huawei_video_availability_check", 
            "huawei_interstitial_click", "huawei_video_click", "huawei_banner_availability_check", "huawei_banner_show", 
            "huawei_banner_click", "af_huawei_video_reward_placement_show", "af_huawei_video_reward", "huawei_interstitial_close",
            "huawei_video_close"
        };
    }
}
