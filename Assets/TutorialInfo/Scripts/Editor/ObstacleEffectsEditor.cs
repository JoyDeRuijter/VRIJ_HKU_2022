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
        draftHeight_prop;

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

        }

        serializedObject.ApplyModifiedProperties();
    }
}
