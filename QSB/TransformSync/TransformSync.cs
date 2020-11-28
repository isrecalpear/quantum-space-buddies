﻿using OWML.Common;
using QSB.Player;
using QSB.SectorSync;
using QSB.Utility;
using UnityEngine;

namespace QSB.TransformSync
{
    public abstract class TransformSync : PlayerSyncObject
    {
        public abstract bool IsReady { get; }
        protected abstract Transform InitLocalTransform();
        protected abstract Transform InitRemoteTransform();

        public Transform SyncedTransform { get; private set; }
        public QSBSector ReferenceSector { get; set; }

        private const float SmoothTime = 0.1f;
        private bool _isInitialized;
        private Vector3 _positionSmoothVelocity;
        private Quaternion _rotationSmoothVelocity;
        private bool _isVisible;

        protected virtual void Awake()
        {
            DebugLog.DebugWrite($"Awake of {AttachedNetId} ({GetType().Name})");
            QSBPlayerManager.PlayerSyncObjects.Add(this);
            DontDestroyOnLoad(gameObject);
            QSBSceneManager.OnSceneLoaded += OnSceneLoaded;
        }

        protected virtual void OnDestroy()
        {
            QSBSceneManager.OnSceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(OWScene scene, bool isInUniverse)
        {
            _isInitialized = false;
        }

        protected void Init()
        {
            DebugLog.DebugWrite($"Init of {AttachedNetId} ({Player.PlayerId}.{GetType().Name})");
            SyncedTransform = hasAuthority ? InitLocalTransform() : InitRemoteTransform();
            _isInitialized = true;
            _isVisible = true;
        }

        private void Update()
        {
            if (!_isInitialized && IsReady)
            {
                Init();
            }
            else if (_isInitialized && !IsReady)
            {
                _isInitialized = false;
                return;
            }

            if (!_isInitialized)
            {
                return;
            }

            if (SyncedTransform == null)
            {
                DebugLog.ToConsole($"Warning - SyncedTransform {AttachedNetId} ({Player.PlayerId}.{GetType().Name}) is null.", MessageType.Warning);
                return;
            }

            if (ReferenceSector == null)
            {
                DebugLog.ToConsole($"Error - {AttachedNetId} ({Player.PlayerId}.{GetType().Name}) doesn't have a reference sector", MessageType.Error);
            }

            UpdateTransform();
        }

        protected virtual void UpdateTransform()
        {
            if (hasAuthority) // If this script is attached to the client's own body on the client's side.	
            {
                if (ReferenceSector == null || ReferenceSector.Transform == null || ReferenceSector.Sector == null)
                {
                    DebugLog.ToConsole($"Error - Referencesector has null values for {AttachedNetId}. ({Player.PlayerId}.{GetType().Name})", MessageType.Error);
                    return;
                }
                transform.position = ReferenceSector.Transform.InverseTransformPoint(SyncedTransform.position);
                transform.rotation = ReferenceSector.Transform.InverseTransformRotation(SyncedTransform.rotation);
                return;
            }

            // If this script is attached to any other body, eg the representations of other players	
            if (SyncedTransform.position == Vector3.zero)
            {
                Hide();
            }
            else
            {
                Show();
            }

            SyncedTransform.localPosition = Vector3.SmoothDamp(SyncedTransform.localPosition, transform.position, ref _positionSmoothVelocity, SmoothTime);
            SyncedTransform.localRotation = QuaternionHelper.SmoothDamp(SyncedTransform.localRotation, transform.rotation, ref _rotationSmoothVelocity, Time.deltaTime);
        }

        public void SetReferenceSector(QSBSector sector)
        {
            DebugLog.DebugWrite($"Setting {Player.PlayerId}.{GetType().Name} to {sector.Name}", MessageType.Info);
            _positionSmoothVelocity = Vector3.zero;
            ReferenceSector = sector;
            if (!hasAuthority)
            {
                SyncedTransform.SetParent(sector.Transform, true);
                transform.position = sector.Transform.InverseTransformPoint(SyncedTransform.position);
                transform.rotation = sector.Transform.InverseTransformRotation(SyncedTransform.rotation);
            }
        }

        private void Show()
        {
            if (!_isVisible)
            {
                SyncedTransform.gameObject.Show();
                _isVisible = true;
            }
        }

        private void Hide()
        {
            if (_isVisible)
            {
                SyncedTransform.gameObject.Hide();
                _isVisible = false;
            }
        }

    }
}