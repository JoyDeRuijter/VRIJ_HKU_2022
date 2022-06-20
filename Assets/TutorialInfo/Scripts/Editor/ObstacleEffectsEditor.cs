using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObstacleEffects)), CanEditMultipleObjects]
public class ObstacleEffectsEditor : Editor
{
    public SerializedProperty
        tileEffect_prop,

        // properties for neutral
        neutralTileColor_prop,

        // properties for death
        deathTileColor_prop,

        // properties for drafting
        draftTileColor_prop,
        draftSpeed_prop,
        draftHeight_prop,

        // properties for activating
        activateTileColor_prop,
        activatedObject_prop,
        deactivateAfterTime_prop,

        // properties for no gravity
        noGravityTileColor_prop,
        hitboxHeight_prop,

        // properties for camera
        camSwitchTileColor_prop,
        setCurrentAutoNode_prop,
        setLastAutoNode_prop;

    private void OnEnable()
    {
        tileEffect_prop = serializedObject.FindProperty("tileEffect");

        // properties for none
        neutralTileColor_prop = serializedObject.FindProperty("neutralTileColor");

        // properties for death
        deathTileColor_prop = serializedObject.FindProperty("deathTileColor");

        // properties for drafting
        draftTileColor_prop = serializedObject.FindProperty("draftTileColor");
        draftSpeed_prop = serializedObject.FindProperty("draftSpeed");
        draftHeight_prop = serializedObject.FindProperty("draftHeight");

        // properties for activating
        activateTileColor_prop = serializedObject.FindProperty("activateTileColor");
        activatedObject_prop = serializedObject.FindProperty("activatedObject");
        deactivateAfterTime_prop = serializedObject.FindProperty("deactivateAfterTime");

        // properties for gravity
        noGravityTileColor_prop = serializedObject.FindProperty("noGravityTileColor");
        hitboxHeight_prop = serializedObject.FindProperty("hitboxHeight");

        // properties for camera
        camSwitchTileColor_prop = serializedObject.FindProperty("camSwitchTileColor");
        setCurrentAutoNode_prop = serializedObject.FindProperty("setCurrentAutoNode");
        setLastAutoNode_prop = serializedObject.FindProperty("setLastAutoNode");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(tileEffect_prop, new GUIContent("Tile effect"));
        EditorGUILayout.Space();
        ObstacleEffects.TileEffect effect = (ObstacleEffects.TileEffect)tileEffect_prop.enumValueIndex;
        EditorGUILayout.LabelField("Adjust settings for " + effect + " effect", EditorStyles.boldLabel);
        switch (effect)
        {
            case ObstacleEffects.TileEffect.none:
                EditorGUILayout.PropertyField(neutralTileColor_prop, new GUIContent("Tile color"));
                break;
            case ObstacleEffects.TileEffect.death:
                EditorGUILayout.PropertyField(deathTileColor_prop, new GUIContent("Tile color"));
                break;

            case ObstacleEffects.TileEffect.drafting:
                EditorGUILayout.PropertyField(draftTileColor_prop, new GUIContent("Tile color"));
                EditorGUILayout.Slider(draftSpeed_prop, 1, 100, new GUIContent("Draft speed"));
                EditorGUILayout.Slider(draftHeight_prop, 1, 10, new GUIContent("Draft height"));
                break;

            case ObstacleEffects.TileEffect.activating:
                EditorGUILayout.PropertyField(activateTileColor_prop, new GUIContent("Tile color"));
                EditorGUILayout.PropertyField(activatedObject_prop, new GUIContent("Activated object(s)"));
                EditorGUILayout.PropertyField(deactivateAfterTime_prop, new GUIContent("Deactivate after time"));
                break;

            case ObstacleEffects.TileEffect.noGravity:
                EditorGUILayout.PropertyField(noGravityTileColor_prop, new GUIContent("Tile color"));
                EditorGUILayout.Slider(hitboxHeight_prop, 1, 10, new GUIContent("Hitbox height"));
                break;

            case ObstacleEffects.TileEffect.setCamPosition:
                EditorGUILayout.PropertyField(camSwitchTileColor_prop, new GUIContent("Tile color"));
                EditorGUILayout.PropertyField(setCurrentAutoNode_prop, new GUIContent("Set the current auto node"));
                EditorGUILayout.PropertyField(setLastAutoNode_prop, new GUIContent("Set the last auto node"));
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
