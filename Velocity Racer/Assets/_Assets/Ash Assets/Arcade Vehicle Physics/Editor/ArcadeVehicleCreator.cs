using UnityEngine;
using UnityEditor;
using System;

namespace ArcadeVP
{
    public class ArcadeVehicleCreator : EditorWindow
    {

        GameObject preset;
        Transform VehicleParent;
        Transform wheelFL;
        Transform wheelFR;
        Transform wheelRL;
        Transform wheelRR;

        MeshRenderer bodyMesh;
        MeshRenderer wheelMesh;

        private GameObject NewVehicle;


        [MenuItem("Tools/Arcade Vehicle Physics")]

        static void OpenWindow()
        {
            ArcadeVehicleCreator vehicleCreatorWindow = (ArcadeVehicleCreator)GetWindow(typeof(ArcadeVehicleCreator));
            vehicleCreatorWindow.minSize = new Vector2(400, 300);
            vehicleCreatorWindow.Show();
        }

        private void OnGUI()
        {
            var style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = Color.green;
            GUILayout.Label("Arcade Vehicle Creator", style);
            preset = EditorGUILayout.ObjectField("Vehicle preset", preset, typeof(GameObject), true) as GameObject;
            GUILayout.Label("Your Vehicle", style);
            VehicleParent = EditorGUILayout.ObjectField("Vehicle Parent", VehicleParent, typeof(Transform), true) as Transform;
            wheelFL = EditorGUILayout.ObjectField("wheel FL", wheelFL, typeof(Transform), true) as Transform;
            wheelFR = EditorGUILayout.ObjectField("wheel FR", wheelFR, typeof(Transform), true) as Transform;
            wheelRL = EditorGUILayout.ObjectField("wheel RL", wheelRL, typeof(Transform), true) as Transform;
            wheelRR = EditorGUILayout.ObjectField("wheel RR", wheelRR, typeof(Transform), true) as Transform;

            if (GUILayout.Button("Create Vehicle"))
            {
                createVehicle();
            }

            bodyMesh = EditorGUILayout.ObjectField("Body Mesh", bodyMesh, typeof(MeshRenderer), true) as MeshRenderer;
            wheelMesh = EditorGUILayout.ObjectField("Wheel Mesh", wheelMesh, typeof(MeshRenderer), true) as MeshRenderer;

            if (GUILayout.Button("Adjust Colliders"))
            {
                adjustColliders();
            }

        }

        private void adjustColliders()
        {
            if (NewVehicle.GetComponent<BoxCollider>())
            {
                NewVehicle.GetComponent<BoxCollider>().center = Vector3.zero;
                NewVehicle.GetComponent<BoxCollider>().size = bodyMesh.bounds.size;
            }

            if (NewVehicle.GetComponent<CapsuleCollider>())
            {
                NewVehicle.GetComponent<CapsuleCollider>().center = Vector3.zero;
                NewVehicle.GetComponent<CapsuleCollider>().height = bodyMesh.bounds.size.z;
                NewVehicle.GetComponent<CapsuleCollider>().radius = bodyMesh.bounds.size.x / 2;

            }

            Vector3 SpheareRBOffset = new Vector3(NewVehicle.transform.position.x,
                                                  wheelFL.position.y + bodyMesh.bounds.extents.y - wheelMesh.bounds.size.y / 2,
                                                  NewVehicle.transform.position.z);

            NewVehicle.GetComponent<ArcadeVehicleController>().skidWidth = wheelMesh.bounds.size.x / 2;
            if (NewVehicle.transform.Find("SphereRB"))
            {
                NewVehicle.transform.Find("SphereRB").GetComponent<SphereCollider>().radius = bodyMesh.bounds.extents.y;
                NewVehicle.transform.Find("SphereRB").position = SpheareRBOffset;
            }

            NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("Skid marks FL").position = wheelFL.position - Vector3.up * (wheelMesh.bounds.size.y / 2 - 0.02f);
            NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("Skid marks FR").position = wheelFR.position - Vector3.up * (wheelMesh.bounds.size.y / 2 - 0.02f);
            NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("Skid marks RL").position = wheelRL.position - Vector3.up * (wheelMesh.bounds.size.y / 2 - 0.02f);
            NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("Skid marks RR").position = wheelRR.position - Vector3.up * (wheelMesh.bounds.size.y / 2 - 0.02f);

        }

        private void createVehicle()
        {
            Make_Vehicle_Ready_For_Setup();

            NewVehicle = Instantiate(preset, bodyMesh.bounds.center, VehicleParent.rotation);
            NewVehicle.name = "Arcade_" + VehicleParent.name;
            GameObject.DestroyImmediate(NewVehicle.transform.Find("Mesh").Find("Body").GetChild(0).gameObject);
            if (NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelFL"))
            {
                GameObject.DestroyImmediate(NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelFL").Find("WheelFL Axel").GetChild(0).gameObject);
            }
            if (NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelFR"))
            {
                GameObject.DestroyImmediate(NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelFR").Find("WheelFR Axel").GetChild(0).gameObject);
            }
            if (NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelRL"))
            {
                GameObject.DestroyImmediate(NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelRL").Find("WheelRL Axel").GetChild(0).gameObject);
            }
            if (NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelRR"))
            {
                GameObject.DestroyImmediate(NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelRR").Find("WheelRR Axel").GetChild(0).gameObject);
            }

            VehicleParent.parent = NewVehicle.transform.Find("Mesh").Find("Body");

            NewVehicle.transform.Find("Mesh").transform.Find("Wheels").position = VehicleParent.position;

            if (NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelFL"))
            {
                NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelFL").position = wheelFL.position;
                wheelFL.parent = NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelFL").Find("WheelFL Axel");
            }
            if (NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelFR"))
            {
                NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelFR").position = wheelFR.position;
                wheelFR.parent = NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelFR").Find("WheelFR Axel");
            }
            if (NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelRL"))
            {
                NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelRL").position = wheelRL.position;
                wheelRL.parent = NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelRL").Find("WheelRL Axel");
            }
            if (NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelRR"))
            {
                NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelRR").position = wheelRR.position;
                wheelRR.parent = NewVehicle.transform.Find("Mesh").transform.Find("Wheels").Find("WheelRR").Find("WheelRR Axel");
            }

        }

        private void Make_Vehicle_Ready_For_Setup()
        {

            var AllVehicleColliders = VehicleParent.GetComponentsInChildren<Collider>();
            foreach (var collider in AllVehicleColliders)
            {
                DestroyImmediate(collider);
            }

            var AllRigidBodies = VehicleParent.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in AllRigidBodies)
            {
                DestroyImmediate(rb);
            }

        }
    }
}
