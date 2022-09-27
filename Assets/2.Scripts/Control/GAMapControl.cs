using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAMapControl
{
    private static GAMapControl m_api;

    public static GAMapControl Api
    {
        get { return m_api; }
        set { m_api = value; }
    }

    public Action onChooseGameType;
    public Action onStartWalking;
    public Action onUpdateStats;
    public Action onToggleAnimation;
    public Action onShowRewardsPopup;
    public Action onShowNextPopup;
    public Action onSetPlaces;
    public Action onSetStepsRequirement;
    public Action onSetSteps;
    public Action onShowCancelWalk;
    public Action onRemoveMarkers;
    public Action onChooseReward;
    public Action onResetStats;

    public Action<bool> onCancelWalk;
    public Action<bool> onSetNewGoal;

    public void ChooseGameType()
    {
        CancelWalk(true);
    }

    public void UpdateStats(Vector2 newPos)
    {
        GAMapModel.Api.currentStepsCount += UnityEngine.Random.Range(50, 101); // TEMP SHOULD USE HA STEPS COUNTER???

        float distanceToGoal = 0f;

        if (!GAMapModel.Api.isInitialUpdate)
        {
            var distanceTraveled = OnlineMapsUtils.DistanceBetweenPoints(GAMapModel.Api.previousPlayerCoordinates, newPos).magnitude;
            GAMapModel.Api.distanceTraveled += distanceTraveled;
            GAMapModel.Api.totalDistanceTraveled += distanceTraveled;
        }

        if (GAMapModel.Api.hasGoal)
            distanceToGoal = OnlineMapsUtils.DistanceBetweenPoints(GAMapModel.Api.playerMarker.position, GAMapModel.Api.currentGoalMarker.position).magnitude;
        else
            distanceToGoal = GAMapModel.Api.distanceToTravel - GAMapModel.Api.distanceTraveled;

        GAMapModel.Api.distanceRemaining = distanceToGoal;
        GAMapModel.Api.previousPlayerCoordinates = newPos;
        GAMapModel.Api.isInitialUpdate = false;

        onUpdateStats?.Invoke();

        CheckProgress(distanceToGoal);
    }

    public void MapClicked()
    {
        if (GAMapModel.Api.currentGoalType != GAMapModel.GoalType.Marker)
            return;

        if (GAMapModel.Api.menuState != GAMapModel.MapState.SetPlaces)
            return;

        double lng, lat;
        OnlineMapsControlBase.instance.GetCoords(out lng, out lat);
        OnlineMapsMarker3DManager.CreateItem(lng, lat, OnlineMapsMarker3DManager.instance.defaultPrefab);
    }

    public void SetNewGoal(OnlineMapsMarker3D marker)
    {
        if (OnlineMapsMarker3DManager.instance.items.Count == 1)
            GAMapModel.Api.playerMarker = marker;

        if (GAMapModel.Api.currentGoalType != GAMapModel.GoalType.Marker)
            return;

        if (OnlineMapsMarker3DManager.instance.items.Count <= 1)
            return;

        ComputePointsTotalDistance();

        var enableStartButton = false;
        if (GAMapModel.Api.menuState == GAMapModel.MapState.SetPlaces && (GAMapModel.Api.pointsTotalDistance * 1000) >= GAMapModel.Api.minimumDistanceToTravel)
            enableStartButton = true;

        GAMapModel.Api.hasGoal = true;
        GAMapModel.Api.currentGoalMarker = OnlineMapsMarker3DManager.instance.items[1];
        onSetNewGoal?.Invoke(enableStartButton);

        UpdateLines();
        ResetStats();
    }

    public void ChooseGameType(int val)
    {
        GAMapModel.Api.currentGoalType = (GAMapModel.GoalType)val;

        switch ((GAMapModel.GoalType)val)
        {
            case GAMapModel.GoalType.Distance:
                GAMapModel.Api.menuState = GAMapModel.MapState.SetDistance;
                break;
            case GAMapModel.GoalType.Marker:
                GAMapModel.Api.menuState = GAMapModel.MapState.SetPlaces;
                break;
            case GAMapModel.GoalType.Steps:
                GAMapModel.Api.menuState = GAMapModel.MapState.SetSteps;
                break;
        }

        onChooseGameType?.Invoke();
    }

    public void SetWalkDistance(float distanceToTravel)
    {
        GAMapModel.Api.distanceToTravel = distanceToTravel;
        GAMapModel.Api.menuState = GAMapModel.MapState.IsPlaying;
        UpdateStats(GAMapModel.Api.playerMarker.position);
        onStartWalking?.Invoke();
    }

    public void ConfirmPlaces()
    {
        GAMapModel.Api.menuState = GAMapModel.MapState.IsPlaying;
        onSetPlaces?.Invoke();

        UpdateStats(GAMapModel.Api.playerMarker.position);
        onStartWalking?.Invoke();
    }

    public void ConfirmSteps()
    {
        GAMapModel.Api.menuState = GAMapModel.MapState.IsPlaying;
        GAMapModel.Api.currentStepsCount = 900;

        UpdateStats(GAMapModel.Api.playerMarker.position);

        onStartWalking?.Invoke();
    }

    public void ShowCancelWalk()
    {
        GAMapModel.Api.isAutoPilot = false;
        GAMapModel.Api.menuState = GAMapModel.MapState.StopWalking;

        onShowCancelWalk?.Invoke();
        onShowNextPopup?.Invoke();
    }

    public void CancelWalk(bool val)
    {
        GAMapModel.Api.menuState = GAMapModel.MapState.SetGametype;

        onCancelWalk?.Invoke(val);

        if (val)
        {
            onShowNextPopup?.Invoke();

            GAMapModel.Api.distanceRemaining = 0f;
            GAMapModel.Api.distanceTraveled = 0f;

            RemoveMarkers();
        }
    }

    private void RemoveMarkers()
    {
        while (OnlineMapsMarker3DManager.instance.items.Count > 1)
        {
            OnlineMapsMarker3DManager.instance.RemoveAt(1);
        }

        OnlineMapsDrawingElementManager.instance.RemoveAll(true);
        UpdateLines();
    }

    public void ChooseReward(bool val)
    {
        GAMapModel.Api.menuState = GAMapModel.MapState.SetGametype;

        if (val)
        {
            onChooseReward?.Invoke();
        }

        onShowNextPopup?.Invoke();
    }

    public void ToggleAutoPilot()
    {
        GAMapModel.Api.isAutoPilot = !GAMapModel.Api.isAutoPilot;
        OnlineMapsLocationService.instance.enabled = !GAMapModel.Api.isAutoPilot;
    }

    public void MoveToTarget()
    {
        var onlineMaps = OnlineMaps.instance;

        GAMapModel.Api.playerMarker.GetPosition(out GAMapModel.Api.lng, out GAMapModel.Api.lat);
        GAMapModel.Api.currentGoalMarker.GetPosition(out GAMapModel.Api.targetLng, out GAMapModel.Api.targetLat);

        double tx1, ty1, tx2, ty2;
        onlineMaps.projection.CoordinatesToTile(GAMapModel.Api.lng, GAMapModel.Api.lat, onlineMaps.zoom, out tx1, out ty1);
        onlineMaps.projection.CoordinatesToTile(GAMapModel.Api.targetLng, GAMapModel.Api.targetLat, onlineMaps.zoom, out tx2, out ty2);

        GAMapModel.Api.rotation = (float)OnlineMapsUtils.Angle2D(tx1, ty1, tx2, ty2) - 90;

        double dx, dy;
        OnlineMapsUtils.DistanceBetweenPoints(GAMapModel.Api.lng, GAMapModel.Api.lat, GAMapModel.Api.targetLng, GAMapModel.Api.targetLat, out dx, out dy);

        double distance = Math.Sqrt(dx * dx + dy * dy);
        float cMaxSpeed = GAMapModel.Api.maxSpeed;
        if (distance < 0.1) cMaxSpeed = GAMapModel.Api.maxSpeed * (float)(distance / 0.1);

        GAMapModel.Api.speed = Mathf.Lerp(GAMapModel.Api.speed, cMaxSpeed, Time.deltaTime);

        OnlineMapsUtils.GetCoordinateInDistance(GAMapModel.Api.lng, GAMapModel.Api.lat, GAMapModel.Api.speed * Time.deltaTime / 3600, GAMapModel.Api.rotation + 180, out GAMapModel.Api.lng, out GAMapModel.Api.lat);

        OnlineMapsUtils.DistanceBetweenPoints(GAMapModel.Api.lng, GAMapModel.Api.lat, GAMapModel.Api.targetLng, GAMapModel.Api.targetLat, out dx, out dy);

        if (Math.Sqrt(dx * dx + dy * dy) < 0.001)
        {
            GAMapModel.Api.speed = 0;
        }

        GAMapModel.Api.playerMarker.SetPosition(GAMapModel.Api.lng, GAMapModel.Api.lat);

        UpdateStats(GAMapModel.Api.playerMarker.position);

        onlineMaps.SetPosition(GAMapModel.Api.lng, GAMapModel.Api.lat);
    }

    private void ResetStats()
    {
        float distanceToGoal = 0f;
        if (OnlineMapsMarker3DManager.instance.items.Count > 1)
            distanceToGoal = OnlineMapsUtils.DistanceBetweenPoints(GAMapModel.Api.playerMarker.position, GAMapModel.Api.currentGoalMarker.position).magnitude;

        GAMapModel.Api.distanceRemaining = distanceToGoal;
        GAMapModel.Api.distanceTraveled = 0f;
        GAMapModel.Api.isInitialUpdate = true;

        onResetStats?.Invoke();
    }

    private void ComputePointsTotalDistance()
    {
        var distance = 0f;
        Vector2 prevPos = GAMapModel.Api.playerMarker.position;

        for (int i = 1; i < OnlineMapsMarker3DManager.instance.items.Count; i++)
        {
            distance += OnlineMapsUtils.DistanceBetweenPoints(prevPos, OnlineMapsMarker3DManager.instance.items[i].position).magnitude;
            prevPos = OnlineMapsMarker3DManager.instance.items[i].position;
        }

        GAMapModel.Api.pointsTotalDistance = distance;
    }

    private void CheckProgress(float val)
    {
        if (GAMapModel.Api.menuState != GAMapModel.MapState.IsPlaying)
            return;

        CreateDistanceCircle();
        UpdateLines();

        if (GAMapModel.Api.currentGoalType == GAMapModel.GoalType.Marker && val <= GAConstants.MARKER_DISTANCE_THRESHOLD)
        {
            GoalAchieved();
        }
        else if (GAMapModel.Api.currentGoalType == GAMapModel.GoalType.Distance && val <= 0)
        {
            OnlineMapsDrawingElementManager.instance.RemoveAll(true);
            GoalAchieved();
        }
        else if (GAMapModel.Api.currentGoalType == GAMapModel.GoalType.Steps && GAMapModel.Api.currentStepsCount >= GAConstants.MINIMUM_STEPS_REQUIRED)
        {
            GoalAchieved();
        }

        ToggleAnimation();
    }

    private void GoalAchieved()
    {
        if (GAMapModel.Api.hasGoal)
            OnlineMapsMarker3DManager.instance.Remove(GAMapModel.Api.currentGoalMarker, true);

        if (OnlineMapsMarker3DManager.instance.items.Count > 1)
        {
            GAMapModel.Api.currentGoalMarker = OnlineMapsMarker3DManager.instance.items[1];
        }
        else
        {
            ShowRewardsPopup();

            GAMapModel.Api.currentStepsCount = 0;
            GAMapModel.Api.distanceTraveled = 0;
            GAMapModel.Api.hasGoal = false;
        }
    }

    private void ShowRewardsPopup()
    {
        GAMapModel.Api.isAutoPilot = false;
        GAMapModel.Api.menuState = GAMapModel.MapState.Rewards;

        GAMissionsControl.Api.UnlockNewMission();

        onShowRewardsPopup?.Invoke();
        onShowNextPopup?.Invoke();
    }

    private void ToggleAnimation()
    {
        if (GAMapModel.Api.playerMarker == null)
            return;

        GAMapModel.Api.isMoving = !GAMapModel.Api.isMoving;
        onToggleAnimation?.Invoke();
    }

    private void UpdateLines()
    {
        if (GAMapModel.Api.currentGoalType != GAMapModel.GoalType.Marker)
            return;

        OnlineMapsDrawingElementManager.instance.RemoveAll(true);
        if (OnlineMapsMarker3DManager.instance.items.Count <= 1)
            return;

        List<Vector2> line = new List<Vector2>();
        line.Add(GAMapModel.Api.playerMarker.position);
        for (int i = 1; i < OnlineMapsMarker3DManager.instance.items.Count; i++)
        {
            line.Add(OnlineMapsMarker3DManager.instance.items[i].position);
        }

        OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(line, Color.red, 1f));
    }

    private void CreateDistanceCircle()
    {
        if (GAMapModel.Api.currentGoalType != GAMapModel.GoalType.Distance)
            return;

        OnlineMapsDrawingElementManager.instance.RemoveAll(true);

        float distance = GAMapModel.Api.distanceRemaining; // in Km
        int segments = GAConstants.CIRCLE_SEGMENTS; // sides count

        double playerLng = GAMapModel.Api.playerMarker.position.x;
        double playerLat = GAMapModel.Api.playerMarker.position.y;

        double nlng, nlat;
        OnlineMapsUtils.GetCoordinateInDistance(playerLng, playerLat, distance, 90, out nlng, out nlat);

        double tx1, ty1, tx2, ty2;

        OnlineMaps map = OnlineMaps.instance;

        // Convert the coordinate under cursor to tile position
        map.projection.CoordinatesToTile(playerLng, playerLat, 20, out tx1, out ty1);

        // Convert remote coordinate to tile position
        map.projection.CoordinatesToTile(nlng, nlat, 20, out tx2, out ty2);

        // Calculate radius in tiles
        double r = tx2 - tx1;

        // Create a new array for points
        OnlineMapsVector2d[] points = new OnlineMapsVector2d[segments];

        // Calculate a step
        double step = 360d / segments;

        // Calculate each point of circle
        for (int i = 0; i < segments; i++)
        {
            double px = tx1 + Math.Cos(step * i * OnlineMapsUtils.Deg2Rad) * r;
            double py = ty1 + Math.Sin(step * i * OnlineMapsUtils.Deg2Rad) * r;
            map.projection.TileToCoordinates(px, py, 20, out playerLng, out playerLat);
            points[i] = new OnlineMapsVector2d(playerLng, playerLat);
        }

        // Create a new polygon to draw a circle
        OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingPoly(points, Color.red, 2));
    }
}
