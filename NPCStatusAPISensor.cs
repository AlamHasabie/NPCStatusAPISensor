/**
 * Copyright (c) 2019 LG Electronics, Inc.
 *
 * This software contains code licensed as described in LICENSE.
 *
 * 
 * Author : Hasabie, Nur (hasabie2@illinois.edu)
 */

using SimpleJSON;
using Simulator.Api;
using Simulator.Bridge;
using Simulator.Sensors.UI;
using Simulator.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Simulator.Sensors
{
    [SensorType("NPCStatusAPI", new System.Type[] { typeof(NPCStatusAPIData) })]
    public class NPCStatusAPISensor : SensorBase
    {
        [SensorParameter]
        public float EventRate = 0.1f;

        private float EventTimer = 0f;

        protected override void Initialize()
        {
            //
        }

        protected override void Deinitialize()
        {
            //
        }

        public override void OnBridgeSetup(BridgeInstance bridge)
        {
            //
        }
        public override Type GetDataBridgePlugin()
        {
            return typeof(NPCStatusAPISensor);
        }

        public void FixedUpdate()
        {
            if (Time.timeScale == 0f)
                return;

            EventTimer += Time.fixedDeltaTime;

            if (EventTimer > EventRate)
            {
                EventTimer = 0f;

                var api = ApiManager.Instance;
                if (api != null)
                {
                    var jsonData = new JSONObject();
                    foreach(KeyValuePair<string, GameObject> dictObject in api.Agents) 
                    {
                        var result = new JSONObject();
                        var obj = dictObject.Value;
                        var ped  = obj.GetComponent<PedestrianController>();

                        // If it's not a pedestrian
                        if (ped == null)
                        {
                            var tr = obj.transform;
                            var rb = obj.GetComponent<Rigidbody>();

                            var transform = new JSONObject();
                            transform.Add("position", tr.position);
                            transform.Add("rotation", tr.rotation.eulerAngles);

                            result.Add("transform", transform);
                            var npc = obj.GetComponent<NPCController>();
                            if (npc != null)
                            {
                                result.Add("velocity", npc.GetVelocity());
                                result.Add("angular_velocity", npc.GetAngularVelocity());
                            }
                            else
                            {
                            result.Add("velocity", rb.velocity);
                            result.Add("angular_velocity", rb.angularVelocity);
                            }
                        }
                        else 
                        {
                            var agent = ped.GetComponent<NavMeshAgent>();
                            var tr = agent.transform;

                            var transform = new JSONObject();
                            transform.Add("position", tr.position);
                            transform.Add("rotation", tr.rotation.eulerAngles);

                            result.Add("transform", transform);
                            result.Add("velocity", agent.velocity);
                            result.Add("angular_velocity", Vector3.zero);
                        }

                        // We use UID as the key -> result
                        jsonData.Add(dictObject.Key, result);
                    }
                    
                    api.AddCustom(transform.parent.gameObject, "npcstatus", jsonData);
                }
            }
        }

        public override void OnVisualize(Visualizer visualizer)
        {
            //
        }

        public override void OnVisualizeToggle(bool state)
        {
            //
        }
    }
}
