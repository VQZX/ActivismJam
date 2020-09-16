using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Walkable.Editor
{
    [CustomEditor(typeof(Area))]
    public class WalkableAreaEditor : UnityEditor.Editor
    {
        private Area area;

        private bool isEditing;

        private List<Vector3> currentPoints;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            // TODO: convert this to UIElements
            if (GUILayout.Button(isEditing ? "Disable Editing" : "Enable Editing"))
            {
                isEditing = !isEditing;
                if (isEditing)
                {
                    OnEditingBegin();
                }
                else
                {
                    OnEditingEnd();
                }
            }
        }

        private void OnSceneGUI()
        {
            if (isEditing)
            {
                EditorGUI.BeginChangeCheck();
                var camera = SceneView.GetAllSceneCameras()[0];
                var length = currentPoints.Count;

                if (Input.GetMouseButtonDown(0))
                {
                    var mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
                    mousePosition.z = 0;
                    if (length == 0)
                    {
                        currentPoints.Add(mousePosition);
                    }
                }

                if (length > 0)
                {
                    for (var i = 0; i < length; i++)
                    {
                        var position = currentPoints[i];
                        currentPoints[i] = Handles.PositionHandle(position, Quaternion.identity);
                    }
                }
                
                EditorGUI.EndChangeCheck();
            }
        }

        private void OnEditingEnd()
        {
            area.AreaPoints = currentPoints;
        }

        private void OnEditingBegin()
        {
            currentPoints = area.AreaPoints;
        }

        private void OnEnable()
        {
            area = (Area) target;
        }
    }
}