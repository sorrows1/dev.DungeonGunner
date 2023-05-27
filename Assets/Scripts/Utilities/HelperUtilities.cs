using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class HelperUtilities
{
    /// Empty string debug check
    public static bool ValidateCheckEmptyString(Object obj, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.LogError($"{fieldName} is empty and must contain a value in object ${obj.name.ToString()}");
            return true;
        }
        return false;
    }

    public static bool ValidateCheckEnumerableValues(Object scriptableObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if (enumerableObjectToCheck == null)
        {
            Debug.Log($"{fieldName} is null in object {scriptableObject.name.ToString()}");
            return true;
        }

        foreach (var item in enumerableObjectToCheck)
        {
            if (item == null)
            {
                Debug.LogError($"{fieldName} has null values in object {scriptableObject.name.ToString()}");
                error = true;
                continue;
            }
            count++;
        }

        if (count == 0)
        {
            Debug.LogError($"{fieldName} has no values in object {scriptableObject.name.ToString()}");
            error = true;
        }

        return error;
    }

    public static List<T> CloneEnumerable<T>(List<T> enumerableToClone)
    {
        return enumerableToClone.ToList();
    }

    public static T[] CloneEnumerable<T>(T[] enumerableToClone)
    {
        return enumerableToClone.ToArray();
    }

    public static bool ValidateCheckNullValue(Object scriptableObject, string fieldName, Object objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debug.Log($"{fieldName} is null and must contain a value in object {scriptableObject.name}");
            return true;
        }
        return false;
    }

    public static bool ValidateCheckPositiveValue(Object scriptableObject, string fieldName, int valueToCheck, bool isZeroAllowed)
    {
        if (valueToCheck > 0) return false;


        if (valueToCheck == 0)
        {
            if (isZeroAllowed) return false;

            Debug.Log($"{fieldName} should not be zero in object {scriptableObject.name}");
        }
        else
        {
            Debug.Log($"{fieldName} should be negative or zero in object {scriptableObject.name}");
        }

        return true;
    }

    public static bool ValidateCheckPositiveValue(Object scriptableObject, string fieldName, float valueToCheck, bool isZeroAllowed)
    {
        if (valueToCheck > 0) return false;


        if (valueToCheck == 0)
        {
            if (isZeroAllowed) return false;

            Debug.Log($"{fieldName} should not be zero in object {scriptableObject.name}");
        }
        else
        {
            Debug.Log($"{fieldName} should be negative or zero in object {scriptableObject.name}");
        }

        return true;
    }

    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, float valueToCheckMinimum, string fieldNameMaximum, float valueToCheckMaximum, bool isZeroAllowed)
    {
        bool error = false;
        if (valueToCheckMinimum > valueToCheckMaximum)
        {
            Debug.Log(fieldNameMinimum + " must be less than or equal to " + fieldNameMaximum + " in object " + thisObject.name.ToString());
            error = true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed)) error = true;

        if (ValidateCheckPositiveValue(thisObject, fieldNameMaximum, valueToCheckMaximum, isZeroAllowed)) error = true;

        return error;
    }



    public static Vector3 GetSpawnPositionNearestToPlayer(Vector3 playerPosition)
    {
        Room currentRoom = GameManager.Instance.CurrentRoom;

        Grid grid = currentRoom.instantiatedRoom.grid;

        return currentRoom.spawnPositionArray.Aggregate(new Vector3(10000f, 10000f), (acc, spawnPosition) =>
        {
            Vector3 spawnPositionWorld = grid.CellToWorld((Vector3Int)spawnPosition);
            if (Vector3.Distance(spawnPositionWorld, playerPosition) < Vector3.Distance(acc, playerPosition)) return spawnPositionWorld;
            return acc;
        });
    }

    public static Vector3 GetMouseWorldPosition(Vector2 mouseScreenPosition)
    {
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        return worldPosition;
    }

    public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x);
        return radians * Mathf.Rad2Deg;
    }


    /// <summary>
    /// Get AimDirection enum value from the pased in angleDegrees
    /// </summary>
    public static AimDirection GetAimDirection(float angleDegrees)
    {
        // Set player direction
        //Up Right
        if (angleDegrees >= 22f && angleDegrees <= 67f)
        {
            return AimDirection.UpRight;
        }
        // Up
        else if (angleDegrees > 67f && angleDegrees <= 112f)
        {
            return AimDirection.Up;
        }
        // Up Left
        else if (angleDegrees > 112f && angleDegrees <= 158f)
        {
            return AimDirection.UpLeft;
        }
        // Left
        else if ((angleDegrees <= 180f && angleDegrees > 158f) || (angleDegrees > -180 && angleDegrees <= -135f))
        {
            return AimDirection.Left;
        }
        // Down
        else if ((angleDegrees > -135f && angleDegrees <= -45f))
        {
            return AimDirection.Down;
        }
        // Right
        else if ((angleDegrees > -45f && angleDegrees <= 0f) || (angleDegrees > 0 && angleDegrees < 22f))
        {
            return AimDirection.Right;
        }

        return AimDirection.Right;
    }
}
