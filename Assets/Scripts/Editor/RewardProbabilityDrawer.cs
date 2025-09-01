using UnityEditor;
using UnityEngine;
using static RewardManager;

[CustomPropertyDrawer(typeof(RewardProbability))]
public class RewardProbabilityDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position.height = EditorGUIUtility.singleLineHeight;

        var nameProp = property.FindPropertyRelative("name");
        var isRewardProp = property.FindPropertyRelative("isReward");
        var rewardTypeProp = property.FindPropertyRelative("rewardType");
        var rewardProbProp = property.FindPropertyRelative("reward_probability");
        var lootTypeProp = property.FindPropertyRelative("lootType");
        var lootProbProp = property.FindPropertyRelative("loot_probability");

        //  Draw name field at top (disabled)
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.PropertyField(position, nameProp, new GUIContent("Name"));
        EditorGUI.EndDisabledGroup();
        position.y += EditorGUIUtility.singleLineHeight + 2;

        //  Draw isReward toggle
        EditorGUI.PropertyField(position, isRewardProp);
        position.y += EditorGUIUtility.singleLineHeight + 2;

        //  Draw reward or loot based on isReward
        if (isRewardProp.boolValue)
        {
            EditorGUI.PropertyField(position, rewardTypeProp);
            position.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(position, rewardProbProp);

            // Auto update name from rewardType
            nameProp.stringValue = rewardTypeProp.enumNames[rewardTypeProp.enumValueIndex];
        }
        else
        {
            EditorGUI.PropertyField(position, lootTypeProp);
            position.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(position, lootProbProp);

            // Auto update name from lootType
            nameProp.stringValue = lootTypeProp.enumNames[lootTypeProp.enumValueIndex];
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 5 + 10;
    }
}
