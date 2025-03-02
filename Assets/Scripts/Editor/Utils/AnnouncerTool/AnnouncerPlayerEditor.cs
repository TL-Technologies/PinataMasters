using UnityEditor;
using UnityEngine;


namespace PinataMasters
{
    [CustomEditor(typeof(AnnouncerPlayer)), CanEditMultipleObjects]
    public class AnnouncerPlayerEditor : Editor
    {
        AnnouncerPlayer announcerPlayer;


        private void OnEnable()
        {
            announcerPlayer = (AnnouncerPlayer)target;
        }


        protected virtual void OnSceneGUI()
        {
            for (int i = 0; i < announcerPlayer.StartPositions.Count; i++)
            {
                announcerPlayer.StartPositions[i] = Handles.PositionHandle(announcerPlayer.StartPositions[i], Quaternion.identity);
            }

            for (int i = 0; i < announcerPlayer.FinishPositions.Count; i++)
            {
                announcerPlayer.FinishPositions[i] = Handles.PositionHandle(announcerPlayer.FinishPositions[i], Quaternion.identity);
            }
        }
    }
}
