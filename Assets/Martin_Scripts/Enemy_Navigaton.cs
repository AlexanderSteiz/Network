﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Enemy_Navigaton : NetworkBehaviour
{
    private GameObject mpu_ObjBaseTarget;
    [HideInInspector]
    public Transform mpu_Target;
    private NavMeshAgent mpi_Agent;
    private Enemy_Targeting mpi_ET;         // Nach Hause telefonieren!
    private Enemy_Stats_N_Stuff mpi_ESNS;

    public float HERO_LIFE = 5;

    // Use this for initialization
    void Awake()
    {
        // Lege das BasisTarget fest
        mpu_ObjBaseTarget = GameObject.FindGameObjectWithTag("BaseTarget");

        // Setze den agenten
        mpi_Agent = GetComponent<NavMeshAgent>();

        // Setze die Enemy_Navigation
        mpi_ET = GetComponentInChildren<Enemy_Targeting>();

        // Setze die Enemy_Stats_N_Stuff
        mpi_ESNS = GetComponent<Enemy_Stats_N_Stuff>();

        // das Leben des Spielers mit Bezug zum Spieler
        // HERO_LIFE = Was auch immer es ist().HP;


        // falls der agent nicht gesetzt wurde, erstmal ALARM machen.
        if (mpi_Agent != null)
        {
            // Das ziel setzen
            mpu_Target = SetupTarget();

            Vector3 Target = mpu_Target.transform.position;
            mpi_Agent.SetDestination(Target);

        }
        else
        {
            // ERROR
            throw new System.Exception("Der NavMeshAgent bei " + gameObject.name + " wurde nicht gesetzt bzw. nicht auf dem Objekt hinzugefügt!");
        }
    }

    [ServerCallback]
    private void Update()
    {
        UpdateTarget();

        if (mpu_Target != null)
        {
            mpi_Agent.SetDestination(mpu_Target.position);
        }

    }

    private void UpdateTarget()
    {
        // ( Ziel leer         || Ziel nicht in der Liste)
        if (mpu_Target == null || !CheckIfTargetContains())
        {
            Debug.Log("Base im Target");
            mpu_Target = mpu_ObjBaseTarget.transform;
        }
        else
        {
            Debug.Log("Spieler im Target");
            mpu_Target = mpi_ET.mpu_Players[0].PlayerTransform;
        }
    }

    private bool CheckIfTargetContains()
    {
        // Falls ein Element in der Liste mit Spielern das Ziel enthält...
        foreach (Enemy_Targeting.TargetInfoData TID in mpi_ET.mpu_Players)
        {
            if (TID.PlayerObject == mpu_Target.gameObject)
            {
                return true;
            }

        }

        return false;
    }

    private Transform SetupTarget()
    {
        // Wenn das Ziel noch nicht gesetzt wurde, wird beim Start automatisch das Ziel auf das BaseTarget gelegt, damit
        // Gegner nicht vollkommen retardet in der Gegend rumdümpelt.
        if (mpu_Target != null)
        {
            return mpu_Target;
        }
        else
        {
            return mpu_ObjBaseTarget.transform;
        }
    }

    private Vector3 GetTarget(string _Tag)
    {
        // Gibt die erstengefundene Position vom Objekt mit dem passenden Tag wieder.
        return GameObject.FindGameObjectsWithTag(_Tag)[0].transform.position;
    }

    private bool CheckforTargetsExistence(Transform _Target)
    {
        if (_Target.gameObject != null)
        {
            // Ziel existiert!
            return true;
        }
        else
        {
            // Ziel existiert nicht und muss genullt werden!
            return false;
        }
    }

}