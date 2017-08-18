﻿
namespace SpeedyMVVM.DataAccess
{
    /// <summary>
    /// Define the actual status of the Entity.
    /// </summary>
    public enum EntityStatusEnum
    {
        Null = 0,
        Detached = 1,
        Unchanged = 2,
        Added = 4,
        Deleted = 8,
        Modified = 16
    }
}
