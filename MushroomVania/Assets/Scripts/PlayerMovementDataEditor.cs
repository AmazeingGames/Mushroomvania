using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using System.Text;
using static PlayerMovementData;
using System;
using UnityEngine.Assertions;
using static UnityEditor.Progress;

[CustomEditor(typeof(PlayerMovementData))]
class PlayerMovementDataEditor : Editor
{
    SerializedProperty movementType;
    SerializedProperty frictionType;
    SerializedProperty quickTurnType;
    SerializedProperty clampType;

    SerializedProperty movementForceMode;
    SerializedProperty movementSpeed;
    SerializedProperty conserveMomentum;
    SerializedProperty accelerationSpeed;
    SerializedProperty decelerationSpeed;
    SerializedProperty airAccelerationMultiplier;
    SerializedProperty airDecelerationMultiplier;
    SerializedProperty velocityPower;
    SerializedProperty groundFrictionAmount;
    SerializedProperty forceFrictionAmount;

    SerializedProperty lerpCurve;
    SerializedProperty lerpSpeed;

    SerializedProperty turnForceMode;
    SerializedProperty turnForceDelay;
    SerializedProperty turnForceAmount;
    SerializedProperty turnForceMultiplier;
    SerializedProperty turnForceAddition;
    SerializedProperty maxMoveVelocity;
    SerializedProperty maxFallVelocity;
    SerializedProperty maxJumpVelocity;

    private void OnEnable()
    {
        movementType = serializedObject.FindProperty("movementType");
        frictionType = serializedObject.FindProperty(nameof(frictionType));
        quickTurnType = serializedObject.FindProperty(nameof(quickTurnType));
        clampType = serializedObject.FindProperty(nameof(clampType));

        movementForceMode = serializedObject.FindProperty(nameof(movementForceMode));
        movementSpeed = serializedObject.FindProperty(nameof(movementSpeed));
        conserveMomentum = serializedObject.FindProperty(nameof(conserveMomentum));
        accelerationSpeed = serializedObject.FindProperty(nameof(accelerationSpeed));
        decelerationSpeed = serializedObject.FindProperty(nameof(decelerationSpeed));
        airAccelerationMultiplier = serializedObject.FindProperty(nameof(airAccelerationMultiplier));
        airDecelerationMultiplier = serializedObject.FindProperty(nameof(airDecelerationMultiplier));

        velocityPower = serializedObject.FindProperty(nameof(velocityPower));
        groundFrictionAmount = serializedObject.FindProperty(nameof(groundFrictionAmount));
        forceFrictionAmount = serializedObject.FindProperty(nameof(forceFrictionAmount));

        lerpCurve = serializedObject.FindProperty(nameof(lerpCurve));
        lerpSpeed = serializedObject.FindProperty(nameof(lerpSpeed));

        turnForceMode = serializedObject.FindProperty(nameof(turnForceMode));
        turnForceDelay = serializedObject.FindProperty(nameof(turnForceDelay));
        turnForceAmount = serializedObject.FindProperty(nameof(turnForceAmount));
        turnForceMultiplier = serializedObject.FindProperty(nameof(turnForceMultiplier));
        turnForceAddition = serializedObject.FindProperty(nameof(turnForceAddition));
        maxMoveVelocity = serializedObject.FindProperty(nameof(maxMoveVelocity));
        maxFallVelocity = serializedObject.FindProperty(nameof(maxFallVelocity));
        maxJumpVelocity = serializedObject.FindProperty(nameof(maxJumpVelocity));
    }

    private void Awake()
    {
        OnValidate();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        DrawSettings();
        DrawMovement();
        DrawFriction();
        DrawQuickTurn();
        DrawClamping();

        serializedObject.ApplyModifiedProperties();
    }

    void DrawSettings()
    {
        EditorGUILayout.PropertyField(movementType, new GUIContent(VariableToLabel(nameof(movementType))));
        EditorGUILayout.PropertyField(frictionType, new GUIContent(VariableToLabel(nameof(frictionType))));
        EditorGUILayout.PropertyField(quickTurnType, new GUIContent(VariableToLabel(nameof(quickTurnType))));
        EditorGUILayout.PropertyField(clampType, new GUIContent(VariableToLabel(nameof(clampType))));
    }

    void DrawMovement()
    {
        EditorGUILayout.PropertyField(movementForceMode, new GUIContent(VariableToLabel(nameof(movementForceMode))));
        EditorGUILayout.PropertyField(movementSpeed, new GUIContent(VariableToLabel(nameof(movementSpeed))));

        string enumName = movementType.enumNames[movementType.enumValueIndex];

        if (enumName == nameof(MovementTypes.Complex) || enumName == nameof(MovementTypes.Complex2))
        {
            EditorGUILayout.PropertyField(accelerationSpeed, new GUIContent(VariableToLabel(nameof(accelerationSpeed))));
            EditorGUILayout.PropertyField(decelerationSpeed, new GUIContent(VariableToLabel(nameof(decelerationSpeed))));
            EditorGUILayout.PropertyField(airDecelerationMultiplier, new GUIContent(VariableToLabel(nameof(airDecelerationMultiplier))));
            EditorGUILayout.PropertyField(airAccelerationMultiplier, new GUIContent(VariableToLabel(nameof(airAccelerationMultiplier))));
            EditorGUILayout.PropertyField(conserveMomentum, new GUIContent(VariableToLabel(nameof(conserveMomentum))));
        }
        if (enumName == nameof(MovementTypes.Complex))
            EditorGUILayout.PropertyField(velocityPower, new GUIContent(VariableToLabel(nameof(velocityPower))));
    }

    void DrawFriction()
    {
        if (CompareEnumToVariableName(movementType, new { MovementTypes.Complex2 }))
            return;

        if (movementType.enumNames[movementType.enumValueIndex] == nameof(MovementTypes.Complex2))
            return;

        string enumName = frictionType.enumNames[frictionType.enumValueIndex];

        switch (enumName)
        {
            case nameof(FrictionTypes.Simple):
                EditorGUILayout.PropertyField(groundFrictionAmount, new GUIContent(VariableToLabel(nameof(groundFrictionAmount))));
            break;

            case nameof(FrictionTypes.Force):
                EditorGUILayout.PropertyField(forceFrictionAmount, new GUIContent(VariableToLabel(nameof(forceFrictionAmount))));
            break;

            case nameof(FrictionTypes.Lerp):
                EditorGUILayout.PropertyField(lerpCurve, new GUIContent(VariableToLabel(nameof(lerpCurve))));
                EditorGUILayout.PropertyField(lerpSpeed, new GUIContent(VariableToLabel(nameof(lerpSpeed))));
            break;

        }

    }

    void DrawQuickTurn()
    {
        string enumName = quickTurnType.enumNames[quickTurnType.enumValueIndex];

        switch (enumName)
        {
            case nameof(QuickTurnTypes.None):
            break;

            default:
                EditorGUILayout.PropertyField(turnForceMode, new GUIContent(VariableToLabel(nameof(turnForceMode))));
                EditorGUILayout.PropertyField(turnForceDelay, new GUIContent(VariableToLabel(nameof(turnForceDelay))));
            break;
        }

        switch (enumName)
        {
            case nameof(QuickTurnTypes.Additive):
                EditorGUILayout.PropertyField(turnForceAddition, new GUIContent(VariableToLabel(nameof(turnForceAddition))));
            break;

            case nameof(QuickTurnTypes.Multiplicative):
                EditorGUILayout.PropertyField(turnForceMultiplier, new GUIContent(VariableToLabel(nameof(turnForceMultiplier))));
            break;

            case nameof(QuickTurnTypes.Constant):
                EditorGUILayout.PropertyField(turnForceAmount, new GUIContent(VariableToLabel(nameof(turnForceAmount))));
            break;
        }
    }

    void DrawClamping()
    {
        string enumName = clampType.enumNames[clampType.enumValueIndex];

        if (enumName == nameof(ClampTypes.None))
            return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Clamping", EditorStyles.boldLabel);

        switch (enumName)
        {
            case nameof(ClampTypes.All):
                EditorGUILayout.PropertyField(maxMoveVelocity, new GUIContent(VariableToLabel(nameof(maxMoveVelocity))));
                EditorGUILayout.PropertyField(maxFallVelocity, new GUIContent(VariableToLabel(nameof(maxFallVelocity))));
                EditorGUILayout.PropertyField(maxJumpVelocity, new GUIContent(VariableToLabel(nameof(maxJumpVelocity))));
            break;

            case nameof(ClampTypes.Vertical):
                EditorGUILayout.PropertyField(maxFallVelocity, new GUIContent(VariableToLabel(nameof(maxFallVelocity))));
                EditorGUILayout.PropertyField(maxJumpVelocity, new GUIContent(VariableToLabel(nameof(maxJumpVelocity))));
            break;

            case nameof(ClampTypes.Horizontal):
                EditorGUILayout.PropertyField(maxMoveVelocity, new GUIContent(VariableToLabel(nameof(maxMoveVelocity))));
            break;

            case nameof(ClampTypes.FallOnly):
                EditorGUILayout.PropertyField(maxFallVelocity, new GUIContent(VariableToLabel(nameof(maxFallVelocity))));
            break;
        }

    }

    static string VariableToLabel(string variableName)
    {
        StringBuilder stringBuilder = new();

        foreach (char c in variableName)
        {
            if (char.IsUpper(c))
                stringBuilder.Append(' ');
            stringBuilder.Append(c);
        }

        return stringBuilder.ToString().FirstCharacterToUpper();
    }

    static bool CompareEnumToVariableName<T>(SerializedProperty serializedProperty, T value) where T : class
    {
        if (value == null)
            return false;

        string compare = typeof(T).GetProperties()[0].Name;

        return serializedProperty.enumNames[serializedProperty.enumValueIndex] == compare;
    }


private void OnValidate()
    {
        // Compare Enum Test
        string stringTest = "A";
        string stringTest2 = "B";
        MyObject obj = ScriptableObject.CreateInstance<MyObject>();
        SerializedObject serializedObject = new(obj);
        SerializedProperty serializedPropertyMyTest = serializedObject.FindProperty("a");
        SerializedProperty bString = serializedObject.FindProperty("b");

        Assert.IsTrue(CompareEnumToVariableName(serializedPropertyMyTest, new { MyObject.Test.A } ));
        Assert.IsFalse(CompareEnumToVariableName(serializedPropertyMyTest, new { MyObject.Test.B } ));
    }

    public class MyObject : ScriptableObject
    {
        public enum Test { A, B }
        public Test a = Test.A;
        public string b = "B";
    }
}

 
