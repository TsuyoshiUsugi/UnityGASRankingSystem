using System.Collections;
using System.Collections.Generic;
using GASRankingSystem.Unity;
using UnityEngine;
using UnityEditor;

namespace GASRankingSystem.Editor
{
    /// <summary>
    /// Editor class to manage ranking system using GAS
    /// </summary>
    [CustomEditor(typeof(GASRankingManager))]
    public class RankingEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var script = (GASRankingManager)target;
            if (GUILayout.Button("Clear Ranking Data"))
            {
                script.ClearRankingData();
            }
        }
    }
}
