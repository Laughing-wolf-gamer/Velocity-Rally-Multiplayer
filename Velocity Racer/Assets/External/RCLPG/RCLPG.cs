using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
#endif
[RequireComponent(typeof(LightProbeGroup))]
public class RCLPG : MonoBehaviour {
    public Bounds ProbeVolume = new Bounds();
    public Vector3 Subdivisions = Vector3.one * 5;
    public Vector3 steps = Vector3.one * 5;
    public string[] TagGroundAccountingObject;
    public bool Auto = false;

    private Vector3 step, probePos, step_pos, pos;
    private float Subx, Subz, Suby, ProbVolSubx, ProbVolSubz, ProbVolSuby, maxRange, rayposY, pos_step_compensx, pos_step_compensz;
    private RaycastHit hit;
    private void Update() {
        transform.position = Vector3.zero;
        transform.eulerAngles = Vector3.zero;
        if (Auto) {
            Generate();
        }
    }

    public void Generate() {
#if UNITY_EDITOR
        if (ProbeVolume.extents == Vector3.zero) {
            ProbeVolume.extents = Vector3.one * 50;
        }

        Subx = Subdivisions.x;
        Subz = Subdivisions.z;
        Suby = Subdivisions.y;

        List<Vector3> probePositions = new List<Vector3>();
        LightProbeGroup lprobe = GetComponent<LightProbeGroup>();

        for (int y = 0; y <= Suby; y++) {
            for (int x = 0; x <= Subx; x++) {
                for (int z = 0; z <= Subz; z++) {
                    if (y >= 2) {
                        ProbVolSubx = (ProbeVolume.extents.x + steps.x * y) / Subx;
                        ProbVolSubz = (ProbeVolume.extents.z + steps.z * y) / Subz;
                        ProbVolSuby = (ProbeVolume.extents.y + steps.y * y) / Suby;

                    } else {
                        ProbVolSubx = (ProbeVolume.extents.x) / Subx;
                        ProbVolSubz = (ProbeVolume.extents.z) / Subz;
                        ProbVolSuby = (ProbeVolume.extents.y) / Suby;
                    }

                    rayposY = ProbeVolume.extents.y - ProbVolSuby;
                    step = new Vector3(ProbVolSubx, ProbVolSuby, ProbVolSubz);
                    step_pos = new Vector3(step.x * x, rayposY, step.z * z);

                    if (y >= 2) {
                        pos_step_compensx = ((y * (steps.x / 2) + ProbeVolume.center.x) - step.x) - (ProbeVolume.center.x - step.x);
                        pos_step_compensz = ((y * (steps.z / 2) + ProbeVolume.center.z) - step.z) - (ProbeVolume.center.z - step.z);
                        pos = ((ProbeVolume.center - new Vector3(pos_step_compensx, 0, pos_step_compensz)) - (ProbeVolume.extents / 2)) + step_pos;
                    } else {
                        pos = (ProbeVolume.center - (ProbeVolume.extents / 2)) + step_pos;
                    }

                    Vector3 direction = Vector3.down;

                    if (y >= 2) {
                        maxRange = ProbeVolume.extents.y - step.y * y;
                    } else {
                        maxRange = ProbeVolume.extents.y * 2;
                    }

                    if (Physics.Raycast(pos, direction, out hit, maxRange)) {
                        for (int i = 0; i < TagGroundAccountingObject.Length; i++) {
                            if (hit.transform.gameObject.tag == TagGroundAccountingObject[i] && y == 1) {
                                probePos = new Vector3(pos.x, hit.point.y + 0.5f, pos.z);
                                if (
                                    Mathf.Abs(probePos.x - (ProbeVolume.center.x)) <= ProbeVolume.extents.x / 2
                                    && Mathf.Abs(probePos.z - (ProbeVolume.center.z)) <= ProbeVolume.extents.z / 2
                                    ) {
                                    probePositions.Add(probePos);
                                }
                            }
                        }
                    }
                    else{
                        probePos = new Vector3(pos.x, (ProbeVolume.extents.y - maxRange) - (ProbeVolume.extents / 2).y, pos.z);

                        if (
                            Mathf.Abs(probePos.y - (ProbeVolume.center.y)) <= ProbeVolume.extents.y / 2
                            && Mathf.Abs(probePos.x - (ProbeVolume.center.x)) <= ProbeVolume.extents.x / 2
                            && Mathf.Abs(probePos.z - (ProbeVolume.center.z)) <= ProbeVolume.extents.z / 2
                            ) {
                            probePositions.Add(probePos);
                        }
                    }
                }
            }
        }
        lprobe.probePositions = probePositions.ToArray();
#endif
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(ProbeVolume.center, ProbeVolume.extents);
    }
}