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

        private void OnSceneUpdate(SceneView sceneview)
        {
            if (isEditing)
            {
                Editing();
            }
            else
            {
                Viewing();
            }
        }

        private void Viewing()
        {
            var length = currentPoints.Count;
            if (length > 0)
            {
                for (var i = 0; i < length; i++)
                {
                    var position = currentPoints[i];
                    
                    Handles.SphereHandleCap(
                        i,
                        position,
                       Quaternion.identity, 
                        0.1f,
                        EventType.Repaint
                    );
                    
                    // Connect via lines
                    if (i != length - 1)
                    {
                        Handles.DrawLine(currentPoints[i], currentPoints[i + 1]);
                    }
                    else
                    {
                        Handles.DrawLine(currentPoints[i], currentPoints[0]);
                    }
                }
            }
        }

        private void Editing()
        {
            EditorGUI.BeginChangeCheck();
            var camera = SceneView.GetAllSceneCameras()[0];
            var length = currentPoints.Count;
            var current = Event.current;
            if (current.isMouse && current.button == 0 && current.type == EventType.MouseDown)
            {
                var mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                
                //TODO: Handle this more elegantly
                
                if (length == 0 || !WalkableAreaEditor.IsCloseToAny(mousePosition, currentPoints.ToArray()))
                {
                    //currentPoints.Add(mousePosition);
                }
            }

            if (length > 0)
            {
                for (var i = 0; i < length; i++)
                {
                    var position = currentPoints[i];
                    currentPoints[i] = Handles.PositionHandle(position, Quaternion.identity);

                    // Connect via lines
                    if (i != length - 1)
                    {
                        Handles.DrawLine(currentPoints[i], currentPoints[i + 1]);
                    }
                    else
                    {
                        Handles.DrawLine(currentPoints[i], currentPoints[0]);
                    }
                }
            }

            EditorGUI.EndChangeCheck();
        }

        private void OnEditingEnd()
        {
            area.AreaPoints = currentPoints;
            EditorUtility.SetDirty(area);
        }

        private void OnEditingBegin()
        {
            currentPoints = area.AreaPoints;
        }

        private void OnEnable()
        {
            area = (Area) target;
            currentPoints = area.AreaPoints;
            SceneView.duringSceneGui += OnSceneUpdate;
        }
        
        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneUpdate;
        }

        private static bool IsCloseToAny(Vector3 check, Vector3[] list, float error = 0.5f)
        {
            foreach (var point in list)
            {
                var sqrDistance = Vector3.SqrMagnitude(point - check);
                //Debug.Log($"Check: ${check}, Point: ${point} Distance: {sqrDistance}");
                if (sqrDistance < error)
                {
                    return true;
                }
            }
            return false;
        }
    }
}