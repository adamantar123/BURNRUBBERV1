using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GercStudio.DragRacingFramework
{
    [CustomEditor(typeof(MenuManager))]
    public class MenuManagerEditor : Editor
    {
        public MenuManager script;
        private ReorderableList mapsList;
        private ReorderableList carsList;
        private ReorderableList opponentsCarsList;

        private ReorderableList avatarsList;
        private ReorderableList levelsList;

        private ReorderableList engineUpgradeList;
        private ReorderableList turboUpgradeList;
        private ReorderableList transmissionUpgradeList;
        private ReorderableList nitroUpgardeList;
        private ReorderableList weightUpgardeList;

        private GUIStyle grayBackground;


        public void Awake()
        {
            script = (MenuManager) target;
        }

        private void OnEnable()
        {
            mapsList = new ReorderableList(serializedObject, serializedObject.FindProperty("currentMapsInEditor"), false, true, true, true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Scenes"); },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.currentMapsInEditor.Add(null);
                        script.levelsNames.Add("");
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.currentMapsInEditor.Remove(script.currentMapsInEditor[items.index]);
                        script.levelsNames.Remove(script.levelsNames[items.index]);
                    }
                },


                drawElementCallback = (rect, index, isActive, isFocused) => { script.currentMapsInEditor[index] = (SceneAsset) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), script.currentMapsInEditor[index], typeof(SceneAsset), false); }
            };

            engineUpgradeList = new ReorderableList(serializedObject, serializedObject.FindProperty("upgradeParameters.engineUpgrades"), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x + 10, rect.y, rect.width / 10 - 10, EditorGUIUtility.singleLineHeight), "№");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 20 + 20, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Price");
                    EditorGUI.LabelField(new Rect(rect.x + 3.5f * rect.width / 20 + 25, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Level");
                    EditorGUI.LabelField(new Rect(rect.x + 6 * rect.width / 20 + 34, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Speed");
                    EditorGUI.LabelField(new Rect(rect.x + 8.5f * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Boost");
                    EditorGUI.LabelField(new Rect(rect.x + 11 * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Power");
                    EditorGUI.LabelField(new Rect(rect.x + 13.5f * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Nitro");
                    EditorGUI.LabelField(new Rect(rect.x + 16 * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Mass");
                },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.upgradeParameters.engineUpgrades.Add(new GameHelper.UpgradeParameter());
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.upgradeParameters.engineUpgrades.Remove(script.upgradeParameters.engineUpgrades[items.index]);
                    }
                },


                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 20, EditorGUIUtility.singleLineHeight), (index + 1).ToString(), EditorStyles.boldLabel);
                    script.upgradeParameters.engineUpgrades[index].price = EditorGUI.IntField(new Rect(rect.x + rect.width / 20 + 10, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.engineUpgrades[index].price);
                    script.upgradeParameters.engineUpgrades[index].level = EditorGUI.IntField(new Rect(rect.x + 3.5f * rect.width / 20 + 15, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.engineUpgrades[index].level);
                    script.upgradeParameters.engineUpgrades[index].addSpeedValue = EditorGUI.IntField(new Rect(rect.x + 6 * rect.width / 20 + 25, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.engineUpgrades[index].addSpeedValue);
                    script.upgradeParameters.engineUpgrades[index].addAccelerationValue = EditorGUI.IntField(new Rect(rect.x + 8.5f * rect.width / 20 + 28, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.engineUpgrades[index].addAccelerationValue);
                    script.upgradeParameters.engineUpgrades[index].addPowerValue = EditorGUI.IntField(new Rect(rect.x + 11f * rect.width / 20 + 31, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.engineUpgrades[index].addPowerValue);
                    script.upgradeParameters.engineUpgrades[index].addNitroValue = EditorGUI.IntField(new Rect(rect.x + 13.5f * rect.width / 20 + 34, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.engineUpgrades[index].addNitroValue);
                    script.upgradeParameters.engineUpgrades[index].addMassValue = EditorGUI.IntField(new Rect(rect.x + 16f * rect.width / 20 + 37, rect.y, rect.width - 16 * rect.width / 20 - 37, EditorGUIUtility.singleLineHeight), script.upgradeParameters.engineUpgrades[index].addMassValue);
                }
            };

            turboUpgradeList = new ReorderableList(serializedObject, serializedObject.FindProperty("upgradeParameters.turboUpgrades"), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x + 10, rect.y, rect.width / 10 - 10, EditorGUIUtility.singleLineHeight), "№");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 20 + 20, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Price");
                    EditorGUI.LabelField(new Rect(rect.x + 3.5f * rect.width / 20 + 25, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Level");
                    EditorGUI.LabelField(new Rect(rect.x + 6 * rect.width / 20 + 34, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Speed");
                    EditorGUI.LabelField(new Rect(rect.x + 8.5f * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Boost");
                    EditorGUI.LabelField(new Rect(rect.x + 11 * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Power");
                    EditorGUI.LabelField(new Rect(rect.x + 13.5f * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Nitro");
                    EditorGUI.LabelField(new Rect(rect.x + 16 * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Mass");
                },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.upgradeParameters.turboUpgrades.Add(new GameHelper.UpgradeParameter());
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.upgradeParameters.turboUpgrades.Remove(script.upgradeParameters.turboUpgrades[items.index]);
                    }
                },


                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 20, EditorGUIUtility.singleLineHeight), (index + 1).ToString(), EditorStyles.boldLabel);
                    script.upgradeParameters.turboUpgrades[index].price = EditorGUI.IntField(new Rect(rect.x + rect.width / 20 + 10, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.turboUpgrades[index].price);
                    script.upgradeParameters.turboUpgrades[index].level = EditorGUI.IntField(new Rect(rect.x + 3.5f * rect.width / 20 + 15, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.turboUpgrades[index].level);
                    script.upgradeParameters.turboUpgrades[index].addSpeedValue = EditorGUI.IntField(new Rect(rect.x + 6 * rect.width / 20 + 25, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.turboUpgrades[index].addSpeedValue);
                    script.upgradeParameters.turboUpgrades[index].addAccelerationValue = EditorGUI.IntField(new Rect(rect.x + 8.5f * rect.width / 20 + 28, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.turboUpgrades[index].addAccelerationValue);
                    script.upgradeParameters.turboUpgrades[index].addPowerValue = EditorGUI.IntField(new Rect(rect.x + 11f * rect.width / 20 + 31, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.turboUpgrades[index].addPowerValue);
                    script.upgradeParameters.turboUpgrades[index].addNitroValue = EditorGUI.IntField(new Rect(rect.x + 13.5f * rect.width / 20 + 34, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.turboUpgrades[index].addNitroValue);
                    script.upgradeParameters.turboUpgrades[index].addMassValue = EditorGUI.IntField(new Rect(rect.x + 16f * rect.width / 20 + 37, rect.y, rect.width - 16 * rect.width / 20 - 37, EditorGUIUtility.singleLineHeight), script.upgradeParameters.turboUpgrades[index].addMassValue);
                }
            };

            transmissionUpgradeList = new ReorderableList(serializedObject, serializedObject.FindProperty("upgradeParameters.transmissionUpgrades"), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x + 10, rect.y, rect.width / 10 - 10, EditorGUIUtility.singleLineHeight), "№");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 20 + 20, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Price");
                    EditorGUI.LabelField(new Rect(rect.x + 3.5f * rect.width / 20 + 25, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Level");
                    EditorGUI.LabelField(new Rect(rect.x + 6 * rect.width / 20 + 34, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Speed");
                    EditorGUI.LabelField(new Rect(rect.x + 8.5f * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Boost");
                    EditorGUI.LabelField(new Rect(rect.x + 11 * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Power");
                    EditorGUI.LabelField(new Rect(rect.x + 13.5f * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Nitro");
                    EditorGUI.LabelField(new Rect(rect.x + 16 * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Mass");
                },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.upgradeParameters.transmissionUpgrades.Add(new GameHelper.UpgradeParameter());
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.upgradeParameters.transmissionUpgrades.Remove(script.upgradeParameters.transmissionUpgrades[items.index]);
                    }
                },


                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 20, EditorGUIUtility.singleLineHeight), (index + 1).ToString(), EditorStyles.boldLabel);
                    script.upgradeParameters.transmissionUpgrades[index].price = EditorGUI.IntField(new Rect(rect.x + rect.width / 20 + 10, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.transmissionUpgrades[index].price);
                    script.upgradeParameters.transmissionUpgrades[index].level = EditorGUI.IntField(new Rect(rect.x + 3.5f * rect.width / 20 + 15, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.transmissionUpgrades[index].level);
                    script.upgradeParameters.transmissionUpgrades[index].addSpeedValue = EditorGUI.IntField(new Rect(rect.x + 6 * rect.width / 20 + 25, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.transmissionUpgrades[index].addSpeedValue);
                    script.upgradeParameters.transmissionUpgrades[index].addAccelerationValue = EditorGUI.IntField(new Rect(rect.x + 8.5f * rect.width / 20 + 28, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.transmissionUpgrades[index].addAccelerationValue);
                    script.upgradeParameters.transmissionUpgrades[index].addPowerValue = EditorGUI.IntField(new Rect(rect.x + 11f * rect.width / 20 + 31, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.transmissionUpgrades[index].addPowerValue);
                    script.upgradeParameters.transmissionUpgrades[index].addNitroValue = EditorGUI.IntField(new Rect(rect.x + 13.5f * rect.width / 20 + 34, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.transmissionUpgrades[index].addNitroValue);
                    script.upgradeParameters.transmissionUpgrades[index].addMassValue = EditorGUI.IntField(new Rect(rect.x + 16f * rect.width / 20 + 37, rect.y, rect.width - 16 * rect.width / 20 - 37, EditorGUIUtility.singleLineHeight), script.upgradeParameters.transmissionUpgrades[index].addMassValue);
                }
            };

            nitroUpgardeList = new ReorderableList(serializedObject, serializedObject.FindProperty("upgradeParameters.nitroUpgrades"), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x + 10, rect.y, rect.width / 10 - 10, EditorGUIUtility.singleLineHeight), "№");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 20 + 20, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Price");
                    EditorGUI.LabelField(new Rect(rect.x + 3.5f * rect.width / 20 + 25, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Level");
                    EditorGUI.LabelField(new Rect(rect.x + 6 * rect.width / 20 + 34, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Speed");
                    EditorGUI.LabelField(new Rect(rect.x + 8.5f * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Boost");
                    EditorGUI.LabelField(new Rect(rect.x + 11 * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Power");
                    EditorGUI.LabelField(new Rect(rect.x + 13.5f * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Nitro");
                    EditorGUI.LabelField(new Rect(rect.x + 16 * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Mass");
                },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.upgradeParameters.nitroUpgrades.Add(new GameHelper.UpgradeParameter());
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.upgradeParameters.nitroUpgrades.Remove(script.upgradeParameters.nitroUpgrades[items.index]);
                    }
                },


                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 20, EditorGUIUtility.singleLineHeight), (index + 1).ToString(), EditorStyles.boldLabel);
                    script.upgradeParameters.nitroUpgrades[index].price = EditorGUI.IntField(new Rect(rect.x + rect.width / 20 + 10, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.nitroUpgrades[index].price);
                    script.upgradeParameters.nitroUpgrades[index].level = EditorGUI.IntField(new Rect(rect.x + 3.5f * rect.width / 20 + 15, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.nitroUpgrades[index].level);
                    script.upgradeParameters.nitroUpgrades[index].addSpeedValue = EditorGUI.IntField(new Rect(rect.x + 6 * rect.width / 20 + 25, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.nitroUpgrades[index].addSpeedValue);
                    script.upgradeParameters.nitroUpgrades[index].addAccelerationValue = EditorGUI.IntField(new Rect(rect.x + 8.5f * rect.width / 20 + 28, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.nitroUpgrades[index].addAccelerationValue);
                    script.upgradeParameters.nitroUpgrades[index].addPowerValue = EditorGUI.IntField(new Rect(rect.x + 11f * rect.width / 20 + 31, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.nitroUpgrades[index].addPowerValue);
                    script.upgradeParameters.nitroUpgrades[index].addNitroValue = EditorGUI.IntField(new Rect(rect.x + 13.5f * rect.width / 20 + 34, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.nitroUpgrades[index].addNitroValue);
                    script.upgradeParameters.nitroUpgrades[index].addMassValue = EditorGUI.IntField(new Rect(rect.x + 16f * rect.width / 20 + 37, rect.y, rect.width - 16 * rect.width / 20 - 37, EditorGUIUtility.singleLineHeight), script.upgradeParameters.nitroUpgrades[index].addMassValue);
                }
            };

            weightUpgardeList = new ReorderableList(serializedObject, serializedObject.FindProperty("upgradeParameters.weightUpgrades"), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x + 10, rect.y, rect.width / 10 - 10, EditorGUIUtility.singleLineHeight), "№");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 20 + 20, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Price");
                    EditorGUI.LabelField(new Rect(rect.x + 3.5f * rect.width / 20 + 25, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Level");
                    EditorGUI.LabelField(new Rect(rect.x + 6 * rect.width / 20 + 34, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Speed");
                    EditorGUI.LabelField(new Rect(rect.x + 8.5f * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Boost");
                    EditorGUI.LabelField(new Rect(rect.x + 11 * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Power");
                    EditorGUI.LabelField(new Rect(rect.x + 13.5f * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Nitro");
                    EditorGUI.LabelField(new Rect(rect.x + 16 * rect.width / 20 + 36, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "Mass");
                },


                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.upgradeParameters.weightUpgrades.Add(new GameHelper.UpgradeParameter());
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.upgradeParameters.weightUpgrades.Remove(script.upgradeParameters.weightUpgrades[items.index]);
                    }
                },


                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 20, EditorGUIUtility.singleLineHeight), (index + 1).ToString(), EditorStyles.boldLabel);
                    script.upgradeParameters.weightUpgrades[index].price = EditorGUI.IntField(new Rect(rect.x + rect.width / 20 + 10, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.weightUpgrades[index].price);
                    script.upgradeParameters.weightUpgrades[index].level = EditorGUI.IntField(new Rect(rect.x + 3.5f * rect.width / 20 + 15, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.weightUpgrades[index].level);
                    script.upgradeParameters.weightUpgrades[index].addSpeedValue = EditorGUI.IntField(new Rect(rect.x + 6 * rect.width / 20 + 25, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.weightUpgrades[index].addSpeedValue);
                    script.upgradeParameters.weightUpgrades[index].addAccelerationValue = EditorGUI.IntField(new Rect(rect.x + 8.5f * rect.width / 20 + 28, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.weightUpgrades[index].addAccelerationValue);
                    script.upgradeParameters.weightUpgrades[index].addPowerValue = EditorGUI.IntField(new Rect(rect.x + 11f * rect.width / 20 + 31, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.weightUpgrades[index].addPowerValue);
                    script.upgradeParameters.weightUpgrades[index].addNitroValue = EditorGUI.IntField(new Rect(rect.x + 13.5f * rect.width / 20 + 34, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), script.upgradeParameters.weightUpgrades[index].addNitroValue);
                    script.upgradeParameters.weightUpgrades[index].addMassValue = EditorGUI.IntField(new Rect(rect.x + 16f * rect.width / 20 + 37, rect.y, rect.width - 16 * rect.width / 20 - 37, EditorGUIUtility.singleLineHeight), script.upgradeParameters.weightUpgrades[index].addMassValue);
                }
            };

            carsList = new ReorderableList(serializedObject, serializedObject.FindProperty("cars"), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight), "Cars");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width / 6, EditorGUIUtility.singleLineHeight), "Price");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 1.5f + rect.width / 5 + 10, rect.y, rect.width - (rect.width / 1.5f + rect.width / 5 + 10), EditorGUIUtility.singleLineHeight), "Level");
                },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.cars.Add(null);
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.cars.Remove(script.cars[items.index]);
                    }
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.cars[index].vehicleController = (VehicleController) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight), script.cars[index].vehicleController, typeof(VehicleController), false);
                    script.cars[index].price = EditorGUI.IntField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width / 6, EditorGUIUtility.singleLineHeight), script.cars[index].price);
                    script.cars[index].level = EditorGUI.IntField(new Rect(rect.x + rect.width / 1.5f + rect.width / 5 + 10, rect.y, rect.width - (rect.width / 1.5f + rect.width / 5 + 10), EditorGUIUtility.singleLineHeight), script.cars[index].level);
                }
            };
            
            opponentsCarsList = new ReorderableList(serializedObject, serializedObject.FindProperty("opponentCars"), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    if (!script.randomOpponents)
                    {
                        EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 2.5f, EditorGUIUtility.singleLineHeight), "Cars");
                        EditorGUI.LabelField(new Rect(rect.x + rect.width / 2.5f + 15 + rect.width / 30 + 15, rect.y, rect.width / 4, EditorGUIUtility.singleLineHeight), "Names");
                        EditorGUI.LabelField(new Rect(rect.x + rect.width / 2.5f + 15 + rect.width / 30 + 15 + rect.width / 4 + 5, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Avatars");
                    }
                    else
                    {
                        EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Cars");
                    }
                },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.opponentCars.Add(null);
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.opponentCars.Remove(script.opponentCars[items.index]);
                    }
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    if (!script.randomOpponents)
                    {
                        EditorGUI.LabelField(new Rect(rect.x + 5, rect.y, rect.width / 30, EditorGUIUtility.singleLineHeight), (index + 1).ToString(), EditorStyles.boldLabel);
                        script.opponentCars[index].vehicleController = (VehicleController) EditorGUI.ObjectField(new Rect(rect.x + rect.width / 30 + 15, rect.y, rect.width / 2.5f, EditorGUIUtility.singleLineHeight), script.opponentCars[index].vehicleController, typeof(VehicleController), false);
                        script.opponentCars[index].name = EditorGUI.TextField(new Rect(rect.x + rect.width / 30 + 15 + rect.width / 2.5f + 10, rect.y, rect.width / 4, EditorGUIUtility.singleLineHeight), script.opponentCars[index].name);
                        script.opponentCars[index].avatar = (Texture) EditorGUI.ObjectField(new Rect(rect.x + rect.width / 30 + 15 + rect.width / 2.5f + 10 + rect.width / 4 + 5, rect.y, rect.width - rect.width / 30 - 15 - rect.width / 2.5f - 5 - rect.width / 4 - 10, EditorGUIUtility.singleLineHeight), script.opponentCars[index].avatar, typeof(Texture), false);
                    }
                    else
                    {
                        script.opponentCars[index].vehicleController = (VehicleController) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), script.opponentCars[index].vehicleController, typeof(VehicleController), false);
                    }
                }
            };

            avatarsList = new ReorderableList(serializedObject, serializedObject.FindProperty("defaultAvatars"), true, true, true, true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Images"); },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.defaultAvatars.Add(null);
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.defaultAvatars.Remove(script.defaultAvatars[items.index]);
                    }
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.defaultAvatars[index] = (Texture) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        script.defaultAvatars[index], typeof(Texture), false);
                }
            };

            levelsList = new ReorderableList(serializedObject, serializedObject.FindProperty("levels"), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 7, EditorGUIUtility.singleLineHeight), "Levels");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 2, rect.y, rect.width - rect.width / 2, EditorGUIUtility.singleLineHeight), "Score Limits");
                },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.levels.Add(null);
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.levels.Remove(script.levels[items.index]);
                    }
                },


                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x + 5, rect.y, rect.width / 10 - 5, EditorGUIUtility.singleLineHeight), (index + 1).ToString(), EditorStyles.boldLabel);

                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 10, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "from");


                    EditorGUI.BeginDisabledGroup(true);

                    if (index > 0)
                        script.levels[index].limits.x = script.levels[index - 1].limits.y;
                    else script.levels[index].limits.x = 0;

                    EditorGUI.IntField(new Rect(rect.x + 2 * rect.width / 10, rect.y, rect.width - rect.width / 1.5f, EditorGUIUtility.singleLineHeight), (int) script.levels[index].limits.x);
                    EditorGUI.EndDisabledGroup();

                    EditorGUI.LabelField(new Rect(rect.x + 2 * rect.width / 10 + rect.width - rect.width / 1.5f + 25, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "to");
                    script.levels[index].limits.y = EditorGUI.IntField(new Rect(rect.x + 3 * rect.width / 10 + rect.width - rect.width / 1.5f + 10, rect.y, rect.width - (rect.x + 3 * rect.width / 10 + rect.width - rect.width / 1.5f - 20), EditorGUIUtility.singleLineHeight), (int) script.levels[index].limits.y);

                }
            };

            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        void Update()
        {
            if (!Application.isPlaying)
            {
                for (var i = 0; i < script.currentMapsInEditor.Count; i++)
                {
                    var level = script.currentMapsInEditor[i];
                    if (level == null) continue;

                    if (!string.Equals(level.name, script.levelsNames[i], StringComparison.Ordinal))
                    {
                        script.levelsNames[i] = level.name;
                        EditorUtility.SetDirty(script.gameObject);
                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    }
                }

                if (script.currentInspectorTab != script.lastInspectorTab)
                {
                    UpdateScenesInBuildSettings();
                    script.currentInspectorTab = script.lastInspectorTab;
                }

                if (script.currentInspectorTab == 1)
                {
                    CheckScenesInBuildSettings(script.oldMapsInEditor, script.currentMapsInEditor);
                }

            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            UIHelper.InitStyles(ref grayBackground, new Color32(160, 160, 160, 200));

            EditorGUILayout.Space();

            script.inspectorTabTop = GUILayout.Toolbar(script.inspectorTabTop, new[] {"Cars", "Maps", "Score & Levels"});

            switch (script.inspectorTabTop)
            {
                case 0:
                    script.inspectorTabBottom = 3;
                    script.currentInspectorTab = 0;
                    break;

                case 1:
                    script.inspectorTabBottom = 3;
                    script.currentInspectorTab = 1;
                    break;

                case 2:
                    script.inspectorTabBottom = 3;
                    script.currentInspectorTab = 2;
                    break;
            }

            script.inspectorTabBottom = GUILayout.Toolbar(script.inspectorTabBottom, new[] {"Upgrades", "Events", "Other Parameters"});


            switch (script.inspectorTabBottom)
            {
                case 0:
                    script.inspectorTabTop = 3;
                    script.currentInspectorTab = 3;
                    break;

                case 1:
                    script.inspectorTabTop = 3;
                    script.currentInspectorTab = 4;
                    break;
                case 2:
                    script.inspectorTabTop = 3;
                    script.currentInspectorTab = 5;
                    break;
            }

            EditorGUILayout.Space();

            switch (script.currentInspectorTab)
            {
                case 0:

                    EditorGUILayout.HelpBox("Make sure that all cars you set below are in the [Resources] folder", MessageType.Info);
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Player Cars", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical(grayBackground);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("carSpawnPoint"), new GUIContent("Spawn Point°", "Cars will spawn at this point"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    carsList.DoLayoutList();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Opponents Cars", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical(grayBackground);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("randomOpponents"), new GUIContent("Random Opponents°", "• If this option is active, the opponent's car will be randomly selected + the same upgrades will be installed on it as on the player's car" + "\n\n" +
                                                                                                                                        "• If not, you can manually set opponent cars. And if the player wins one, then he goes to the next opponent"));
                    EditorGUILayout.Space();
                    opponentsCarsList.DoLayoutList();
                    EditorGUILayout.EndVertical();
                    break;

                case 1:
                    EditorGUILayout.BeginVertical("helpbox");
                    mapsList.DoLayoutList();
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("All scenes will be automatically added to the Build Settings.", MessageType.Info);
                    EditorGUILayout.EndVertical();
                    break;

                case 2:

                    var backgroundColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(1, 0, 0, 0.3f);
                    if (GUILayout.Button(new GUIContent("Reset All Progress°", "This button deletes all progress and all purchases." + "\n" +
                                                                               "The game settings has a button with the same action.")))
                    {
                        script.gameAssets = Resources.Load("GameAssets", typeof(GameAssets)) as GameAssets;

                        if (script.gameAssets != null)
                        {
                            script.ResetAllData();
                        }
                    }
                    GUI.backgroundColor = backgroundColor;
                    
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.HelpBox("The lower value will always be equal to the upper value of the previous level.", MessageType.Info);

                    levelsList.DoLayoutList();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Race Profit Values", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.LabelField("Victory in a race", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("scoreValues.winRace"), new GUIContent("Score"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("moneyValues.winRace"), new GUIContent("Money"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Losing in a race", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("scoreValues.loseRace"), new GUIContent("Score"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("moneyValues.loseRace"), new GUIContent("Money"));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Perfect Start", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("scoreValues.perfectStart"), new GUIContent("Score"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("moneyValues.perfectStart"), new GUIContent("Money"));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Good Start", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("scoreValues.goodStart"), new GUIContent("Score"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("moneyValues.goodStart"), new GUIContent("Money"));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Perfect Shift", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("scoreValues.perfectShift"), new GUIContent("Score"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("moneyValues.perfectShift"), new GUIContent("Money"));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(new GUIContent("Distance Bonus°", "If a player wins a race, this value will be multiplied by the distance between the player's and opponent's cars."), EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("scoreValues.distanceBonus"), new GUIContent("Score"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("moneyValues.distanceBonus"), new GUIContent("Money"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();

                    break;

                case 3:
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("upgradesType"), new GUIContent("Upgrades Type°", "• Add Value - the specified value will be added" + "\n" + "[200 + 3 = 203]" + "\n\n" +
                                                                                                                                  "• Add Percent - the specified percentage (of the current value) will be added" + "\n" + "[200 + 3% = 206]"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Engine Upgrades", EditorStyles.boldLabel);
                    engineUpgradeList.DoLayoutList();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Turbo Upgrades", EditorStyles.boldLabel);
                    turboUpgradeList.DoLayoutList();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Transmission Upgrades", EditorStyles.boldLabel);
                    transmissionUpgradeList.DoLayoutList();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Nitro Upgrades", EditorStyles.boldLabel);
                    nitroUpgardeList.DoLayoutList();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Weight Upgrades", EditorStyles.boldLabel);
                    weightUpgardeList.DoLayoutList();
                    EditorGUILayout.EndVertical();
                    break;

                case 4:

                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.HelpBox("When one of these actions happens in the game, all the events that you added for it will be triggered.", MessageType.Info);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("carPurchaseEvent"), new GUIContent("Purchase Car"));
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("carSelectEvent"), new GUIContent("Select Car"));
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("upgradeInstallEvent"), new GUIContent("Install Upgrade"));
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("startRaceEvent"), new GUIContent("Start Race"));
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("resetAllDataEvent"), new GUIContent("Reset Settings"));
                    EditorGUILayout.EndVertical();
                    break;

                case 5:

                    // EditorGUILayout.BeginVertical(grayBackground);

                    // EditorGUILayout.BeginVertical("helpbox");
                    // EditorGUILayout.PropertyField(serializedObject.FindProperty("currentUIManager"), new GUIContent("UI Manager"));
                    // EditorGUILayout.EndVertical();
                    // EditorGUILayout.Space();

                    EditorGUILayout.BeginVertical("helpbox");
                    script.checkInternetConnection = EditorGUILayout.ToggleLeft(new GUIContent("Check Internet Connection°", "Checking if the game has an internet connection" + "\n" + "Disable this if you are going to build a web game"), script.checkInternetConnection, EditorStyles.boldLabel);
                    if (script.checkInternetConnection)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("This server is needed to check the internet connection." + "\n" +
                                                "It should be like 'http://[name].[domain]'", MessageType.Info);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("checkConnectionServer"), new GUIContent("Server"));
                    }

                    EditorGUILayout.EndVertical();
                    
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.LabelField(new GUIContent("In-game Timers (sec)"), EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("findingOpponentsTimer"), new GUIContent("Finding Opponents"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("startGameTimer"), new GUIContent("Start Race"));
                    EditorGUILayout.EndVertical();
                    
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.LabelField(new GUIContent("Cameras°", "When players go to the corresponding menu, the cameras will switch."), EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("switchCamerasImmediately"), new GUIContent("Switch Cameras Immediately°", "[ON] - The cameras will switch immediately." + "\n" +
                                                                                                                                                           "[OFF] - The cameras will smoothly move to another position."));

                    if (!script.switchCamerasImmediately)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraTransitionSpeed"), new GUIContent("Transition Speed"));
                    }

                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultCamera"), new GUIContent("Main Menu"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("carsShopCamera"), new GUIContent("Cars Shop"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("carsUpgradesCamera"), new GUIContent("Cars Upgrades"));

                    EditorGUILayout.EndVertical();

                    // script.currentCameraMode = GUILayout.Toolbar(script.currentCameraMode, new[] {"Main Menu", "Selection Menu", "Upgrade Menu"});

                    // if (script.lastCameraMode != script.currentCameraMode)
                    // {
                    //     switch (script.currentCameraMode)
                    //     {
                    //         case 0:
                    //             script.defaultCamera.transform.position = script.mainMenuPositions.position;
                    //             script.defaultCamera.transform.rotation = script.mainMenuPositions.rotation;
                    //             break;
                    //
                    //         case 1:
                    //             script.defaultCamera.transform.position = script.carSelectionPositions.position;
                    //             script.defaultCamera.transform.rotation = script.carSelectionPositions.rotation;
                    //             break;
                    //
                    //         case 2:
                    //             script.defaultCamera.transform.position = script.carUpgradePositions.position;
                    //             script.defaultCamera.transform.rotation = script.carUpgradePositions.rotation;
                    //             break;
                    //     }
                    //
                    //     script.lastCameraMode = script.currentCameraMode;
                    // }

                    // EditorGUILayout.Space();


                    // if (GUILayout.Button("Save"))
                    // {
                    //     switch (script.currentCameraMode)
                    //     {
                    //         case 0:
                    //             script.mainMenuPositions.position = script.defaultCamera.transform.position;
                    //             script.mainMenuPositions.rotation = script.defaultCamera.transform.rotation;
                    //             break;
                    //         case 1:
                    //             script.carSelectionPositions.position = script.defaultCamera.transform.position;
                    //             script.carSelectionPositions.rotation = script.defaultCamera.transform.rotation;
                    //             break;
                    //         case 2:
                    //             script.carUpgradePositions.position = script.defaultCamera.transform.position;
                    //             script.carUpgradePositions.rotation = script.defaultCamera.transform.rotation;
                    //             break;
                    //     }
                    // }


                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.LabelField(new GUIContent("Default Avatars°", "Players will be able to select one of these avatars in the game menu"), EditorStyles.boldLabel);
                    avatarsList.DoLayoutList();
                    // EditorGUILayout.EndVertical();

                    break;
            }

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(script);
                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            // DrawDefaultInspector();
        }

        void UpdateScenesInBuildSettings()
        {
            var editorBuildSettingsScenes = new List<EditorBuildSettingsScene> {new EditorBuildSettingsScene(SceneManager.GetActiveScene().path, true)};

            foreach (var sceneAsset in script.currentMapsInEditor)
            {
                if (sceneAsset == null) continue;

                var scenePath = AssetDatabase.GetAssetPath(sceneAsset);

                if (!string.IsNullOrEmpty(scenePath))
                    editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
            }

            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
        }

        void CheckScenesInBuildSettings(List<SceneAsset> oldScenes, List<SceneAsset> currentScenes)
        {
            foreach (var map in currentScenes)
            {
                if (map && !oldScenes.Contains(map))
                {
                    oldScenes.Add(map);
                    UpdateScenesInBuildSettings();
                    break;
                }
            }

            foreach (var map in oldScenes)
            {
                if (!currentScenes.Exists(level => level == map))
                {
                    oldScenes.Remove(map);
                    UpdateScenesInBuildSettings();
                    break;
                }
            }
        }
    }
}
