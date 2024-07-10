using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FlowField;

namespace VectorPath {

    [CustomEditor(typeof(NavigationFlowField))]
    public class NavigationFlowFieldEditor : Editor
    {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            NavigationFlowField flowField = (NavigationFlowField)target;
            flowField.PreComputed = flowField.NavMap != null;
            if(GUILayout.Button("Pre Generate NavMap")) {
                flowField.Instantiate();
                flowField.CalculateNavMap();
                flowField.PreComputed = true;
            }
        }
    }
    
}
